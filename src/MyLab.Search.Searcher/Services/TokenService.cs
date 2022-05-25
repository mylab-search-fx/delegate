using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyLab.Log;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Options;
using Newtonsoft.Json;

namespace MyLab.Search.Searcher.Services
{
    class TokenService : ITokenService
    {
        private readonly SearcherOptions _options;
        readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        readonly Lazy<SymmetricSecurityKey> _securityKey;
        readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private const string IndexSettingsClaimName = "mylab:searcher:indexes";

        public TokenService(IOptions<SearcherOptions> options)
        :this(options.Value)
        {
        }

        public TokenService(SearcherOptions options)
        {
            _options = options;

            _securityKey = new Lazy<SymmetricSecurityKey>(() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Token.SignKey)));
        }

        public bool IsEnabled()
        {
            return _options.Token != null;
        }

        public string CreateSearchToken(TokenRequestV3 request)
        {
            if(!IsEnabled())
                throw new TokenizingDisabledException("Token factoring disabled");
            
            var namespaceSettings = JsonConvert.SerializeObject(request.Namespaces);

            var payload = BuildPayload(request, namespaceSettings);

            var header = new JwtHeader(new SigningCredentials(_securityKey.Value, "HS256"));
            
            try
            {
                return _tokenHandler.WriteToken(new JwtSecurityToken(header, payload));
            }
            catch (ArgumentOutOfRangeException e) when (e.Message.Contains("IDX10653"))
            {
                throw new InvalidOperationException("Token key too small", e);
            }
        }

        public NamespaceSettingsV3 ValidateAndExtractSettings(string token, string ns)
        {
            if (!IsEnabled())
                throw new TokenizingDisabledException("Token factoring disabled");

            ClaimsPrincipal tokenPrincipal;
            try
            {
                tokenPrincipal = _tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateTokenReplay = false,
                    ValidateActor = false,
                    ValidateAudience = true,
                    ValidateLifetime = _options.Token.ExpirySec.HasValue,
                    LifetimeValidator= (before, expires, securityToken, parameters) =>
                    {
                        if (!_options.Token.ExpirySec.HasValue)
                            return true;
                        return expires >= DateTime.UtcNow;
                    },
                    IssuerSigningKey = _securityKey.Value,
                    ValidAudience = ns
                }, out _);
            }
            catch (Exception e)
            {
                throw new InvalidTokenException("Search token validation failed", e);
            }

            var namespaceClaims = tokenPrincipal.Claims
                .Where(c => c.Type == IndexSettingsClaimName)
                .Select(ParseClaim)
                .ToArray();

            if (namespaceClaims.Length == 0)
            {
                throw new InvalidTokenException("Namespace claims not found in the Search Token");
            }

            var foundNs = namespaceClaims.FirstOrDefault(c => c.Name == ns);

            if (foundNs == null)
            {
                throw new InvalidTokenException("Context namespace claim not found in the Search Token");
            }

            return foundNs;

            NamespaceSettingsV3 ParseClaim(Claim claim)
            {
                try
                {
                    var normVal = claim.Value.Trim();

                    return JsonConvert.DeserializeObject<NamespaceSettingsV3>(normVal);
                }
                catch (JsonException e)
                {
                    throw new InvalidTokenException("Namespaces claim from Search Token has wrong format", e)
                        .AndFactIs("token", claim.Value);
                }
            }
        }

        private JwtPayload BuildPayload(TokenRequestV3 request, string namespaceSettings)
        {
            var payloadLines = new List<string>
            {
                $"\"{IndexSettingsClaimName}\": {namespaceSettings}"
            };

            if (_options.Token.ExpirySec.HasValue)
            {
                var expDt = (long)(DateTime.UtcNow.AddSeconds(_options.Token.ExpirySec.Value) - _epoch).TotalSeconds;
                payloadLines.Add($"\"exp\": {expDt}");
            }

            if (request.Namespaces != null)
            {
                var namespaceNames = request.Namespaces.Select(ns => "\"" + ns.Name + "\"");
                payloadLines.Add($"\"aud\": [{string.Join(',', namespaceNames)}]");
            }

            string payloadJson = "{" + string.Join(',', payloadLines) + "}";
            var payload = JwtPayload.Deserialize(payloadJson);
            return payload;
        }
    }
}
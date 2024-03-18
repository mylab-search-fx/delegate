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

        public string CreateSearchToken(TokenRequestV4 request)
        {
            if(!IsEnabled())
                throw new TokenizingDisabledException("Token factoring disabled");
            
            var idxSettings = JsonConvert.SerializeObject(request.Indexes);

            var payload = BuildPayload(request, idxSettings);

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

        public IndexSettingsV4 ValidateAndExtractSettings(string token, string idxId)
        {
            if (!IsEnabled())
                throw new TokenizingDisabledException("Token factoring disabled");

            ClaimsPrincipal tokenPrincipal;

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateTokenReplay = false,
                ValidateActor = false,
                ValidateAudience = true,
                ValidateLifetime = _options.Token.ExpirySec.HasValue,
                LifetimeValidator = ValidateLifetime,
                IssuerSigningKey = _securityKey.Value,
                ValidAudience = idxId,
                AudienceValidator = ValidateAudience
            };

            try
            {
                tokenPrincipal = _tokenHandler.ValidateToken(token, tokenValidationParams, out _);
            }
            catch (Exception e)
            {
                throw new InvalidTokenException("Search token validation failed", e);
            }

            var idxClaims = tokenPrincipal.Claims
                .Where(c => c.Type == IndexSettingsClaimName)
                .Select(ParseClaim)
                .ToArray();
            
            var targetClaim = idxClaims.FirstOrDefault(c => c.Id == idxId);

            return targetClaim ?? idxClaims.FirstOrDefault(c => c.Id == "*");

            IndexSettingsV4 ParseClaim(Claim claim)
            {
                try
                {
                    var normVal = claim.Value.Trim();

                    return JsonConvert.DeserializeObject<IndexSettingsV4>(normVal);
                }
                catch (JsonException e)
                {
                    throw new InvalidTokenException("Indexes claim from Search Token has wrong format", e)
                        .AndFactIs("token", claim.Value);
                }
            }
        }

        private bool ValidateAudience(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            var auds = audiences.ToArray();

            if (auds.Contains("*"))
            {
                return true;
            }

            return auds.Contains(validationParameters.ValidAudience);
        }

        private bool ValidateLifetime(DateTime? before, DateTime? expires, SecurityToken securityToken, TokenValidationParameters parameters)
        {
            if (!_options.Token.ExpirySec.HasValue) return true;
            return expires >= DateTime.UtcNow;
        }

        private JwtPayload BuildPayload(TokenRequestV4 request, string idxSettings)
        {
            var payloadLines = new List<string>
            {
                $"\"{IndexSettingsClaimName}\": {idxSettings}"
            };

            if (_options.Token.ExpirySec.HasValue)
            {
                var expDt = (long)(DateTime.UtcNow.AddSeconds(_options.Token.ExpirySec.Value) - _epoch).TotalSeconds;
                payloadLines.Add($"\"exp\": {expDt}");
            }

            if (request.Indexes != null)
            {
                var idxIds = request.Indexes.Select(idx => "\"" + idx.Id + "\"");
                payloadLines.Add($"\"aud\": [{string.Join(',', idxIds)}]");
            }

            string payloadJson = "{" + string.Join(',', payloadLines) + "}";
            var payload = JwtPayload.Deserialize(payloadJson);
            return payload;
        }
    }
}
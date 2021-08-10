using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyLab.Search.Delegate.Models;
using Newtonsoft.Json;

namespace MyLab.Search.Delegate.Services
{
    class TokenService : ITokenService
    {
        private readonly DelegateOptions _options;
        readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        readonly Lazy<SymmetricSecurityKey> _securityKey;
        readonly DateTime _epoch = new DateTime(1970, 1, 1);

        private const string FiltersClaimName = "mylab:search-dlgt:filters";

        public TokenService(IOptions<DelegateOptions> options)
        :this(options.Value)
        {
        }

        public TokenService(DelegateOptions options)
        {
            _options = options;

            _securityKey = new Lazy<SymmetricSecurityKey>(() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Token.SignKey)));
        }

        public bool IsEnabled()
        {
            return _options.Token != null;
        }

        public string CreateSearchToken(TokenRequest request)
        {
            if(!IsEnabled())
                throw new TokenizingDisabledException("Token factoring disabled");
            
            var filtersJson = JsonConvert.SerializeObject(request.Filters);
            var claims = new List<Claim>
            {
                new Claim(FiltersClaimName, filtersJson)
            };

            if (_options.Token.ExpirySec.HasValue)
            {
                var expDt = (long)(DateTime.Now.AddSeconds(_options.Token.ExpirySec.Value) - _epoch).TotalSeconds;
                claims.Add(new Claim("exp", expDt.ToString()));
            }
            var header = new JwtHeader(new SigningCredentials(_securityKey.Value, "HS256"));
            var payload = new JwtPayload(claims);
            try
            {
                return _tokenHandler.WriteToken(new JwtSecurityToken(header, payload));
            }
            catch (ArgumentOutOfRangeException e) when (e.Message.Contains("IDX10653"))
            {
                throw new InvalidOperationException("Token key too small", e);
            }
        }

        public FiltersCall ValidateAndExtractSearchToken(string token)
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
                    ValidateAudience = false,
                    ValidateLifetime = _options.Token.ExpirySec.HasValue,
                    LifetimeValidator= (before, expires, securityToken, parameters) =>
                    {
                        if (!_options.Token.ExpirySec.HasValue)
                            return true;

                        var now = DateTime.Now;
                        return expires >= now;
                    },
                    IssuerSigningKey = _securityKey.Value
                }, out _);
            }
            catch (Exception e)
            {
                throw new InvalidTokenException("Search token validation failed", e);
            }

            var filtersClaim = tokenPrincipal.Claims.FirstOrDefault(c => c.Type == FiltersClaimName);

            if (filtersClaim == null)
            {
                throw new InvalidTokenException("Filters Claim not found in the Search Token");
            }

            FiltersCall filtersCall;
            try
            {
                filtersCall = JsonConvert.DeserializeObject<FiltersCall>(filtersClaim.Value);
            }
            catch (JsonException e)
            {
                throw new InvalidTokenException("Filters Call from Search Token has wrong format", e);
            }

            return filtersCall;
        }
    }
}
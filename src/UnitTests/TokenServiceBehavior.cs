using System;
using System.Linq;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using Xunit;

namespace UnitTests
{
    public class TokenServiceBehavior
    {
        [Fact]
        public void ShouldNotCreateWhenNoTokenizingConfig()
        {
            //Arrange
            var opt = new DelegateOptions();
            var srv = new TokenService(opt);

            //Act & Assert
            Assert.Throws<TokenizingDisabledException>(() =>
            {
                srv.CreateSearchToken(new TokenRequest
                {
                    Filters = new FiltersCall()
                });
            });
        }

        [Fact]
        public void ShouldNotValidateWhenNoTokenizingConfig()
        {
            //Arrange
            var opt = new DelegateOptions();
            var srv = new TokenService(opt);

            //Act & Assert
            Assert.Throws<TokenizingDisabledException>(() =>
            {
                srv.ValidateAndExtractSearchToken("token");
            });
        }

        [Fact]
        public void ShouldValidateTokenBySign()
        {
            //Arrange
            var opt = new DelegateOptions
            {
                Token = new DelegateOptions.Tokenizing
                {
                    SignKey = CreateKey()
                }
            };
            var srv = new TokenService(opt);

            var token = srv.CreateSearchToken(new TokenRequest
            {
                Filters = new FiltersCall()
            });

            //Act
            srv.ValidateAndExtractSearchToken(token);

            //Assert

        }

        [Fact]
        public void ShouldNotValidateInvalidTokenBySign()
        {
            //Arrange
            var opt = new DelegateOptions
            {
                Token = new DelegateOptions.Tokenizing
                {
                    SignKey = CreateKey()
                }
            };

            var opt2 = new DelegateOptions
            {
                Token = new DelegateOptions.Tokenizing
                {
                    SignKey = CreateKey()
                }
            };
            var srv = new TokenService(opt);
            var srv2 = new TokenService(opt2);

            var token = srv.CreateSearchToken(new TokenRequest
            {
                Filters = new FiltersCall()
            });

            //Act & Assert
            Assert.Throws<InvalidTokenException>(() =>
            {
                srv2.ValidateAndExtractSearchToken(token);
            });
        }

        [Fact]
        public void ShouldValidateTokenByExpiry()
        {
            //Arrange
            var opt = new DelegateOptions
            {
                Token = new DelegateOptions.Tokenizing
                {
                    SignKey = CreateKey(),
                    ExpirySec = 10
                }
            };
            var srv = new TokenService(opt);

            var token = srv.CreateSearchToken(new TokenRequest
            {
                Filters = new FiltersCall()
            });

            //Act
            srv.ValidateAndExtractSearchToken(token);

            //Assert

        }

        [Fact]
        public void ShouldNotValidateInvalidTokenByExpiry()
        {
            //Arrange
            var opt = new DelegateOptions
            {
                Token = new DelegateOptions.Tokenizing
                {
                    SignKey = CreateKey(),
                    ExpirySec = -1
                }
            };
            var srv = new TokenService(opt);

            var token = srv.CreateSearchToken(new TokenRequest
            {
                Filters = new FiltersCall()
            });

            //Act & Assert
            Assert.Throws<InvalidTokenException>(() =>
            {
                srv.ValidateAndExtractSearchToken(token);
            });
        }

        string CreateKey()
        {
            return string.Join(':', Enumerable.Repeat(Guid.NewGuid().ToString("N"), 10));
        }
    }
}

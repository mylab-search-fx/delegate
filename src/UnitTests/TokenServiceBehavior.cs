using MyLab.Log;
using System;
using System.Linq;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class TokenServiceBehavior
    {
        private readonly ITestOutputHelper _output;
        private const string TestNamespace = "test";
        private const string TestNamespace2 = "test2";
        static readonly TokenRequest TokenRequest = new TokenRequest
        {
            Namespaces = new NamespaceSettingsMap
            {
                {
                    TestNamespace,
                    new NamespaceSettings
                    {
                        Filters = new FiltersCall()
                    }
                },
                {
                    TestNamespace2, 
                    new NamespaceSettings
                    {
                        Filters = new FiltersCall()
                    }
                }
            }
        };

        public TokenServiceBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldNotCreateWhenNoTokenizingConfig()
        {
            //Arrange
            var opt = new DelegateOptions();
            var srv = new TokenService(opt);

            //Act & Assert
            Assert.Throws<TokenizingDisabledException>(() =>
            {
                CreateSearchToken(srv, TokenRequest);
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
                srv.ValidateAndExtractSettings("token", TestNamespace);
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

            var token = CreateSearchToken(srv, TokenRequest);

            //Act
            srv.ValidateAndExtractSettings(token, TestNamespace);

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

            var token = CreateSearchToken(srv, TokenRequest);

            //Act & Assert
            Assert.Throws<InvalidTokenException>(() =>
            {
                srv2.ValidateAndExtractSettings(token, TestNamespace);
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

            var token = CreateSearchToken(srv, TokenRequest);

            //Act
            srv.ValidateAndExtractSettings(token, TestNamespace);

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

            var token = CreateSearchToken(srv, TokenRequest);

            //Act & Assert
            Assert.Throws<InvalidTokenException>(() =>
            {
                srv.ValidateAndExtractSettings(token, TestNamespace);
            });
        }

        [Fact]
        public void ShouldNotValidateIfWrongAudience()
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

            var token = CreateSearchToken(srv, TokenRequest);

            //Act & Assert
            Assert.Throws<InvalidTokenException>(() =>
            {
                srv.ValidateAndExtractSettings(token, "wrong-namespace");
            });
        }

        string CreateKey()
        {
            return string.Join(':', Enumerable.Repeat(Guid.NewGuid().ToString("N"), 10));
        }

        string CreateSearchToken(TokenService srv, TokenRequest request)
        {
            var token = srv.CreateSearchToken(request);

            _output.WriteLine("Token: " + token);

            return token;
        }
    }
}

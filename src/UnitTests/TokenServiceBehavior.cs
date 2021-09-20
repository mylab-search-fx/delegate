using MyLab.Log;
using System;
using System.Linq;
using System.Text;
using MyLab.Search.Delegate;
using MyLab.Search.Delegate.Models;
using MyLab.Search.Delegate.Services;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class TokenServiceBehavior
    {
        readonly Random _rnd = new Random(DateTime.Now.Millisecond);

        private readonly ITestOutputHelper _output;
        private const string TestNamespace = "test";
        private const string TestNamespace2 = "test2";
        static readonly TokenRequestV2 TokenRequest = new TokenRequestV2
        {
            Namespaces = new []
            {
                new NamespaceSettingsV2()
                {
                    Name = TestNamespace
                },
                new NamespaceSettingsV2()
                {
                    Name = TestNamespace2
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
            AssertThrows<TokenizingDisabledException>(() =>
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
            AssertThrows<TokenizingDisabledException>(() =>
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
            AssertThrows<InvalidTokenException>(() =>
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
            AssertThrows<InvalidTokenException>(() =>
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
            AssertThrows<InvalidTokenException>(() =>
            {
                srv.ValidateAndExtractSettings(token, "wrong-namespace");
            });
        }

        string CreateKey()
        {
            char ch = (char)('a' + _rnd.Next(10));
            var key = new string(Enumerable.Repeat(ch, 16).ToArray());

            _output.WriteLine($"Key: '{key}' ({Encoding.UTF8.GetByteCount(key)} bytes)");
            
            return key;
        }

        string CreateSearchToken(TokenService srv, TokenRequestV2 request)
        {
            var token = srv.CreateSearchToken(request);

            _output.WriteLine("Token: " + token);

            return token;
        }

        void AssertThrows<T>(Action act)
            where T : Exception
        {
            var exception = Assert.Throws<T>(act);

            _output.WriteLine("Exception: " + exception);
        }
    }
}

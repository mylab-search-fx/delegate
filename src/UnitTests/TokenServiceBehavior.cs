using MyLab.Log;
using System;
using System.Linq;
using System.Text;
using MyLab.Search.Searcher;
using MyLab.Search.Searcher.Models;
using MyLab.Search.Searcher.Options;
using MyLab.Search.Searcher.Services;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class TokenServiceBehavior
    {
        readonly Random _rnd = new Random(DateTime.Now.Millisecond);

        private readonly ITestOutputHelper _output;
        private const string TestIndex = "test";
        private const string TestIndex2 = "test2";
        static readonly TokenRequestV3 TokenRequest = new TokenRequestV3
        {
            Namespaces = new []
            {
                new NamespaceSettingsV3()
                {
                    Name = TestIndex
                },
                new NamespaceSettingsV3()
                {
                    Name = TestIndex2
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
            var opt = new SearcherOptions();
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
            var opt = new SearcherOptions();
            var srv = new TokenService(opt);

            //Act & Assert
            AssertThrows<TokenizingDisabledException>(() =>
            {
                srv.ValidateAndExtractSettings("token", TestIndex);
            });
        }

        [Fact]
        public void ShouldValidateTokenBySign()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                Token = new TokenizingOptions
                {
                    SignKey = CreateKey()
                }
            };
            var srv = new TokenService(opt);

            var token = CreateSearchToken(srv, TokenRequest);

            //Act
            srv.ValidateAndExtractSettings(token, TestIndex);

            //Assert

        }

        [Fact]
        public void ShouldNotValidateInvalidTokenBySign()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                Token = new TokenizingOptions
                {
                    SignKey = CreateKey()
                }
            };

            var opt2 = new SearcherOptions
            {
                Token = new TokenizingOptions
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
                srv2.ValidateAndExtractSettings(token, TestIndex);
            });
        }

        [Fact]
        public void ShouldValidateTokenByExpiry()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                Token = new TokenizingOptions
                {
                    SignKey = CreateKey(),
                    ExpirySec = 10
                }
            };
            var srv = new TokenService(opt);

            var token = CreateSearchToken(srv, TokenRequest);

            //Act
            srv.ValidateAndExtractSettings(token, TestIndex);

            //Assert

        }

        [Fact]
        public void ShouldNotValidateInvalidTokenByExpiry()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                Token = new TokenizingOptions
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
                srv.ValidateAndExtractSettings(token, TestIndex);
            });
        }

        [Fact]
        public void ShouldNotValidateIfWrongAudience()
        {
            //Arrange
            var opt = new SearcherOptions
            {
                Token = new TokenizingOptions
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

        string CreateSearchToken(TokenService srv, TokenRequestV3 request)
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

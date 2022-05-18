using System;

namespace MyLab.Search.Searcher
{
    public class TokenizingDisabledException : Exception
    {
        public TokenizingDisabledException(string message) : base(message)
        {
            
        }
    }
}
using System;

namespace MyLab.Search.Delegate
{
    public class TokenizingDisabledException : Exception
    {
        public TokenizingDisabledException(string message) : base(message)
        {
            
        }
    }
}
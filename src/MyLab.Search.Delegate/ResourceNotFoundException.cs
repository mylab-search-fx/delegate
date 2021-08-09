using System;

namespace MyLab.Search.Delegate
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message)
        {
            
        }
    }
}

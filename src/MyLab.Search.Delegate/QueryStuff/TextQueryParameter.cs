namespace MyLab.Search.Delegate.QueryStuff
{
    class TextQueryParameter : SearchQueryParameter<string>
    {
        public TextQueryParameter(string value, int rank) 
            : base(value, rank)
        {
        }
    }
}
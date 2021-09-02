using Nest;

namespace MyLab.Search.Delegate.QueryTools
{
    class NumericQueryExpression : IQueryExpression
    {
        public int Value { get; }

        public NumericQueryExpression(int value)
        {
            Value = value;
        }

        public bool TryCreateQuery(IProperty property, out QueryBase query)
        {
            query = null;

            if (NumericSupportedPropertyTypes.IsSupported(property.Type))
            {
                query = new TermQuery
                {
                    Field = property.Name.Name,
                    Value = Value
                };
            }

            return query != null;
        }
    }
}
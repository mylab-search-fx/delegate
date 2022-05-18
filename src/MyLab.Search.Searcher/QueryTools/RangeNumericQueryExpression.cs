using Nest;

namespace MyLab.Search.Searcher.QueryTools
{
    class RangeNumericQueryExpression : IQueryExpression
    {
        public int? Greater { get; set; }
        public int? GreaterOrEqual { get; set; }
        public int? Less { get; set; }
        public int? LessOrEqual { get; set; }

        public bool TryCreateQuery(IProperty property, out QueryBase query)
        {
            query = null;

            if (NumericSupportedPropertyTypes.IsSupported(property.Type))
            {
                query = new LongRangeQuery
                {
                    Field = property.Name.Name,
                    LessThan = Less,
                    GreaterThan = Greater,
                    LessThanOrEqualTo = LessOrEqual,
                    GreaterThanOrEqualTo = GreaterOrEqual
                };
            }

            return query != null;
        }
    }
}
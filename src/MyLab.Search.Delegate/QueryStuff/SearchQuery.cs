using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLab.Search.Delegate.QueryStuff
{
    class SearchQuery
    {
        static readonly ISearchParameterParser DefaultParameterParser = new TextSearchParameterParser();

        static readonly ISearchParameterParser[] Parsers = 
        {
            new DateTimeSearchParameterParser(),
            new DateTimeRangeSearchParameterParser(),
            new DateTimeLessSearchParameterParser(),
            new DateTimeGreaterSearchParameterParser(),
            new NumericSearchParameterParser(),
            new NumericRangeSearchParameterParser(), 
            new NumericLessSearchParameterParser(), 
            new NumericGreaterSearchParameterParser(), 
        }; 

        public IReadOnlyCollection<ISearchQueryParam> Params => _searchParams;

        private readonly ISearchQueryParam[] _searchParams;
        
        public SearchQuery(IEnumerable<ISearchQueryParam> searchParams)
        {
            _searchParams = searchParams.ToArray();
        }

        public static SearchQuery Parse(string query)
        {
            var qParams = new List<ISearchQueryParam>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var words = query.Split(' ', '\t', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length >= 3)
                    .ToArray();

                for (int i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    var parser = Parsers.FirstOrDefault(p => p.CanParse(word)) ?? DefaultParameterParser;

                    var param = parser.Parse(word, words.Length-i);
                    qParams.Add(param);
                }
            }

            return new SearchQuery(qParams);
        }
    }
}

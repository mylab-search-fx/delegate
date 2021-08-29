using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLab.Search.Delegate.QueryStuff
{
    class SearchQuery
    {
        static readonly ISearchParameterParser TextParameterParser = new TextSearchParameterParser();

        static readonly ISearchParameterParser[] NumericParsers = 
        {
            new NumericSearchParameterParser(),
            new NumericRangeSearchParameterParser(), 
            new NumericLessSearchParameterParser(), 
            new NumericGreaterSearchParameterParser(), 
        };

        static readonly ISearchParameterParser[] DateTimeParsers =
        {
            new DateTimeSearchParameterParser(),
            new DateTimeRangeSearchParameterParser(),
            new DateTimeLessSearchParameterParser(),
            new DateTimeGreaterSearchParameterParser()
        };

        public IReadOnlyCollection<ISearchQueryParam> TextParams => _textSearchParams;
        public IReadOnlyCollection<ISearchQueryParam> NumericParams => _numSearchParams;
        public IReadOnlyCollection<ISearchQueryParam> DateTimeParams => _dtSearchParams;

        public bool IsEmpty => _dtSearchParams.Length + _numSearchParams.Length + _textSearchParams.Length == 0;

        private ISearchQueryParam[] _textSearchParams;
        private ISearchQueryParam[] _numSearchParams;
        private ISearchQueryParam[] _dtSearchParams;

        SearchQuery()
        {
            
        }

        public static SearchQuery Parse(string query)
        {
            var txtParams = new List<ISearchQueryParam>();
            var numParams = new List<ISearchQueryParam>();
            var dtParams = new List<ISearchQueryParam>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var words = query.Split(' ', '\t', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length >= 3)
                    .ToArray();

                for (int i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    var rank = words.Length - i;

                    if (!TryParse(word, rank, NumericParsers, numParams))
                    {
                        if (!TryParse(word, rank, DateTimeParsers, dtParams))
                        {
                            // do nothing
                        }
                    }

                    txtParams.Add(TextParameterParser.Parse(word, rank));
                }
            }

            return new SearchQuery
            {
                _numSearchParams = numParams.ToArray(),
                _textSearchParams = txtParams.ToArray(),
                _dtSearchParams = dtParams.ToArray()
            };
        }

        static bool TryParse(string word, int rank, ISearchParameterParser[] parsers, List<ISearchQueryParam> resultParams)
        {
            var parser = parsers.FirstOrDefault(p => p.CanParse(word));

            if (parser == null)
                return false;

            var param = parser.Parse(word, rank);
            resultParams.Add(param);

            return true;
        }
    }
}

namespace MyLab.Search.Delegate
{
    public class DelegateOptions
    {
        public string DefaultFilter { get; set; }
        public string DefaultSort { get; set; }
        public int? DefaultLimit { get; set; }
        public string SortPath { get; set; } = "/etc/mylab-search-delegate/sort/";
        public string FilterPath { get; set; } = "/etc/mylab-search-delegate/filter/";
    }
}

namespace MyLab.Search.Delegate.Services
{
    class IndexMappingProperty
    {
        public string Name { get; }
        public string Type { get; }

        public IndexMappingProperty(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
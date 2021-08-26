namespace MyLab.Search.Delegate.Client
{
    /// <summary>
    /// Contains found entity
    /// </summary>
    public class FoundEntity<TContent>
    {
        public double Score { get; set; }
        public TContent Content { get; set; }
    }
}
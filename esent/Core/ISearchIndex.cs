namespace Meowth.Esentery.Core
{
    /// <summary> ESENT Search index </summary>
    public interface ISearchIndex : IIndex
    {
        /// <summary> Column which index bound to </summary>
        Column Column { get; }
    }
}
namespace Meowth.Esentery.Core
{
    /// <summary> Abstract Esent index </summary>
    public interface IIndex
    {
        /// <summary> Index name. Unique </summary>
        string Name { get; }
    }
}
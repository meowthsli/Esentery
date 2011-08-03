namespace Meowth.Esentery.Core
{
    /// <summary> Read-write cursor </summary>
    public interface ICursor : IReadonlyCursor
    {
        /// <summary> Creates insert/update buffer </summary>
        RowModification AddRow();
    }
}
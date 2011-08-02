namespace Meowth.Esentery.Core
{
    public interface ICursor : IReadonlyCursor
    {
        RowModification AddRow();
    }
}
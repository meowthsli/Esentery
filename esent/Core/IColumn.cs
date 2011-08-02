using System;

namespace Meowth.Esentery.Core
{
    public interface IColumn
    {
        Table Table { get; }
        string ColumnName { get; }
        Type ColumnType { get; }
        ColumnOptions Options { get; }
    }
}
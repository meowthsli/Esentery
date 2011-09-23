using System;
using System.Linq;
using Meowth.Esentery.Core;
using Meowth.Esentery.Querying;

namespace Meowth.Esentery.Extensions
{
    /// <summary> Simplifies query building </summary>
    public class PredicateFactory
    {
        public Table Table { get; private set;}

        /// <summary> Predicate factory. Saves many finger movement </summary>
        public PredicateFactory(Table table)
        {
            Table = table;
        }

        /// <summary> = </summary>
        public Eq<T> Eq<T>(string columnName, T val)
            where T : IComparable<T>
        {
            var index = Table.GetSearchIndexOfColumn(columnName);
            return new Eq<T>(index, val);
        }

        /// <summary> &lt;= </summary>
        public Le<T> Le<T>(string columnName, T val)
            where T : IComparable<T>
        {
            var index = Table.GetSearchIndexOfColumn(columnName);
            return new Le<T>(index, val);
        }

        /// <summary> &lt; </summary>
        public Lt<T> Lt<T>(string columnName, T val)
            where T : IComparable<T>
        {
            var index = Table.GetSearchIndexOfColumn(columnName);
            return new Lt<T>(index, val);
        }

        /// <summary> &gt;= </summary>
        public Ge<T> Ge<T>(string columnName, T val)
            where T : IComparable<T>
        {
            var index = Table.GetSearchIndexOfColumn(columnName);
            return new Ge<T>(index, val);
        }

        /// <summary> &gt; </summary>
        public Gt<T> Gt<T>(string columnName, T val)
            where T : IComparable<T>
        {
            var index = Table.GetSearchIndexOfColumn(columnName);
            return new Gt<T>(index, val);
        }

        /// <summary> &gt; </summary>
        public StartsWith StartsWith(string columnName, string val)
        {
            var index = Table.GetSearchIndexOfColumn(columnName);
            return new StartsWith(index, val);
        }

        /// <summary> &gt; </summary>
        public Between<T> Between<T>(string columnName, T val1, T val2, bool inclFrom = true, bool inclTo = true)
            where T : IComparable<T>
        {
            var index = Table.GetSearchIndexOfColumn(columnName);
            return new Between<T>(index, val1, inclFrom, val2, inclTo);
        }
    }
}
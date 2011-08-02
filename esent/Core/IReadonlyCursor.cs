using System;

namespace Meowth.Esentery.Core
{
    /// <summary> SynthesizedCursor abstraction  </summary>
    public interface IReadonlyCursor : IDisposable
    {
        /// <summary> Moves to nect record </summary>
        bool MoveNext();

        /// <summary> Returns value </summary>
        string GetString(string fieldName);
    }
}
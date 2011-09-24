using System;

namespace Meowth.Esentery.Core
{
    /// <summary> SynthesizedCursor abstraction  </summary>
    public interface IReadonlyCursor : IDisposable
    {
        /// <summary> Moves to next record </summary>
        bool MoveNext();
    }
}
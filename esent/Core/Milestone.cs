using System;
using System.Linq;
using Meowth.Esentery.Querying;
using Microsoft.Isam.Esent.Interop;

namespace Meowth.Esentery.Core
{
    /// <summary> Milestone (position) of cursor.
    /// Consolidates knowledge about key and cursor restriction on that key  </summary>
    public class Milestone : IComparable<Milestone>, IEquatable<Milestone>
    {
        /// <summary> On which it was created? </summary>
        public IIndex Index { get; private set; }
        
        /// <summary> Milestone creation </summary>
        internal Milestone(IIndex index, byte[] data, KeyOptions options)
        {
            Index = index;
            Data = data;
        }

        #region Comparable/Equatable crap

        /// <summary> Compares data </summary>
        public int CompareTo(Milestone otherMilestone)
        {
            if (Index != otherMilestone.Index)
                throw new ArgumentException("otherMilestone is built against another indes");

            return Data.CompareTo(otherMilestone.Data);
        }

        /// <summary> Checks equality </summary>
        public bool Equals(Milestone other)
        {
            if (ReferenceEquals(this, other))
                return true;

            return (CompareTo(other) == 0);
        }

        public override bool Equals(object obj)
        {
            var ms = obj as Milestone;
            return ms != null ? Equals(ms) : false;
        }

        /// <summary> Returns hash code of code </summary>
        public override int GetHashCode()
        {
            return unchecked(Index.GetHashCode() ^ Data.Aggregate(0, (current, b) => current ^ b));
        }

        public static bool operator ==(Milestone right, Milestone left)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Milestone right, Milestone left)
        {
            return left.Equals(right);
        }

        #endregion

        internal byte[] Data { get; set; }
    }
}
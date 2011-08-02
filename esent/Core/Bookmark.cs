using System;
using System.Linq;

namespace Meowth.Esentery.Core
{
    /// <summary> Bookmark to store position </summary>
    public class Bookmark : IComparable<Bookmark>, IEquatable<Bookmark>
    {
        /// <summary> Creates bookmark </summary>
        /// TODO: should be reference to table, on which this bookmark is created
        public Bookmark(byte[] bookmarkData)
        {
            Data = bookmarkData;
        }

        #region Comparable/Equatable crap
        /// <summary> Returns data </summary>
        public int CompareTo(Bookmark other)
        {
            return Data.CompareTo(other);
        }

        public bool Equals(Bookmark other)
        {
            return (Data.CompareTo(other) == 0);
        }

        public override bool Equals(object obj)
        {
            var ob = obj as Bookmark;
            if (obj == null)
                return false;

            return Equals(ob);
        }

        public override int GetHashCode()
        {
            return unchecked(Data.Aggregate(0, (current, b) => current ^ b));
        }

        /// <summary> Handy onversion </summary>
        public static implicit operator byte[](Bookmark bookmark)
        {
            return bookmark.Data;
        }

        public static bool operator ==(Bookmark right, Bookmark left)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bookmark right, Bookmark left)
        {
            return left.Equals(right);
        }

        #endregion

        private byte[] Data { get; set; }
    }
}
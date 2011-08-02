namespace Meowth.Esentery.Core
{
    /// <summary> Helpers </summary>
    static class Utils
    {
        /// <summary> Compares arrays </summary>
        public static int CompareTo(this byte[] left, byte[] right)
        {
            if (left.Length < right.Length)
                return -1;

            if (left.Length > right.Length)
                return 1;

            for (var i = 0; i < left.Length; i++)
                if (left[i] < right[i])
                    return -1;
                else if (left[i] > right[i])
                    return 1;

            return 0;
        }
    }
}
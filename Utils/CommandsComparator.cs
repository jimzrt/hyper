using System;
using System.Linq;

namespace Utils
{
    public static class CommandsComparator
    {
        public static bool IsEquals(byte[] first, byte[] second)
        {
            return IsEquals(first, second, 2);
        }
        public static bool IsEquals(byte[] first, byte[] second, int bytesCountToCompare)
        {
            if (first.Length < bytesCountToCompare || second.Length < bytesCountToCompare)
            {
                throw new ArgumentException();
            }
            return first.Take(bytesCountToCompare).SequenceEqual(second.Take(bytesCountToCompare));
        }
    }
}

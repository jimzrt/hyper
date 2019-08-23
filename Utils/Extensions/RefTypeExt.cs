using System;

namespace Utils
{
    public static class RefTypeExt
    {
        public static void ThrowIfNull<T>(this T target, string message) where T : class
        {
            if (target == null)
            {
                throw new NullReferenceException(message);
            }
        }
    }
}

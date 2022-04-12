namespace DisruptiveSoftware.Cryptography.Extensions
{
    using System.Runtime.InteropServices;
    using System.Security;

    public static class SecureStringExtensions
    {
        public static string SecureStringToString(this SecureString? secureStr)
        {
            if (secureStr == null || secureStr.Length == 0) return string.Empty;

            var ptr = IntPtr.Zero;
            string result;

            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureStr);
                result = Marshal.PtrToStringUni(ptr)!;
            }
            finally
            {
                if (ptr != IntPtr.Zero) Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }

            return result;
        }

        public static char[] ToCharArray(this SecureString value)
        {
            return SecureStringToString(value).ToCharArray();
        }

        public static SecureString ToSecureString(this string str)
        {
            var secureString = new SecureString();

            foreach (var c in str) secureString.AppendChar(c);

            return secureString;
        }
    }
}
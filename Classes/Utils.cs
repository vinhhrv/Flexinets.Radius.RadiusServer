using System;
using System.Text;

namespace Flexinets.Radius
{
    public static class Utils
    {
        public static Byte[] StringToByteArray(String hex)
        {
            Int32 NumberChars = hex.Length;
            Byte[] bytes = new Byte[NumberChars / 2];
            for (var i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static String ByteArrayToString(Byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}

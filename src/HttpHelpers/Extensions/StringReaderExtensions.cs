using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpHelpers.Extensions
{
    static class StringReaderExtensions
    {
        public static string TakeWhile(this StringReader reader, Func<char, bool> predicate)
        {
            var builder = new StringBuilder();
            while (true)
            {
                var raw = reader.Peek();
                if (raw == -1)
                {
                    break;
                }
                var ch = (char)raw;
                if (!predicate(ch))
                {
                    break;
                }
                builder.Append(ch);
                reader.Read();
            }
            return builder.ToString();
        }

        public static void SkipWhiteSpace(this StringReader reader)
        {
            while (true)
            {
                var raw = reader.Peek();
                if (raw == -1)
                {
                    break;
                }
                var ch = (char)raw;
                if (!ch.IsWhiteSpace())
                {
                    break;
                }
                reader.Read();
            }
        }

        public static int SkipWhile(this StringReader reader, Func<char, bool> predicate)
        {
            var skipped = 0;
            while (true)
            {
                var raw = reader.Peek();
                if (raw == -1)
                {
                    break;
                }
                var ch = (char)raw;
                if (!predicate(ch))
                {
                    break;
                }
                skipped++;
                reader.Read();
            }
            return skipped;
        }

        public static char PeekChar(this StringReader reader)
        {
            return (char) reader.Peek();
        }
    }
}

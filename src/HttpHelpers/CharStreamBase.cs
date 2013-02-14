using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpHelpers
{
    public abstract class CharStreamBase : IDisposable
    {
        public abstract int ReadByte();
        
        public abstract int PeekByte();
        
        public abstract char PeekChar();

        public static CharStreamBase FromStream(Stream stream)
        {
            if (stream == null) { throw new ArgumentNullException("stream"); }

            return new CharStream(stream);
        }

        public static CharStreamBase FromByteArray(IEnumerable<byte> buffer)
        {
            if (buffer == null) { throw new ArgumentNullException("buffer"); }

            return new ByteArrayCharStream(buffer);
        }

        public string TakeWhile(Func<char, bool> predicate)
        {
            var builder = new StringBuilder();
            while (true)
            {
                var raw = PeekByte();
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
                ReadByte();
            }
            return builder.ToString();
        }

        public int SkipWhile(Func<char, bool> predicate)
        {
            var skipped = 0;
            while (true)
            {
                var raw = PeekByte();
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
                ReadByte();
            }
            return skipped;
        }

        public abstract void Dispose();
    }
}
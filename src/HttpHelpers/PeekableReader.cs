using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpHelpers
{
    sealed class PeekableReader : IDisposable
    {
        public PeekableReader(Stream stream)
        {
            _stream = stream;
            _byte = _stream.ReadByte();
        }

        public int ReadByte()
        {
            var current = _byte;
            _byte = _stream.ReadByte();
            return current;
        }

        public int PeekByte()
        {
            return _byte;
        }

        public char PeekChar()
        {
            return (char) PeekByte();
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
                var ch = (char) raw;
                if (!predicate(ch))
                {
                    break;
                }
                skipped++;
                ReadByte();
            }
            return skipped;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
                _disposed = true;
            }
        }

        ~PeekableReader()
        {
            Dispose(false);
        }

        private int _byte;
        private Stream _stream;
        private bool _disposed;
    }
}

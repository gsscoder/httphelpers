using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpHelpers
{
    public class CharStream : CharStreamBase, IDisposable
    {
        public CharStream(Stream stream)
        {
            _stream = stream;
            _byte = _stream.ReadByte();
        }

        public override int ReadByte()
        {
            var current = _byte;
            _byte = _stream.ReadByte();
            return current;
        }

        public override int PeekByte()
        {
            return _byte;
        }

        public override char PeekChar()
        {
            return (char) PeekByte();
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

        ~CharStream()
        {
            Dispose(false);
        }

        private int _byte;
        private Stream _stream;
        private bool _disposed;
    }
}

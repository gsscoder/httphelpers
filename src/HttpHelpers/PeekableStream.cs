using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpHelpers
{
    sealed class PeekableStream : Stream
    {
        public PeekableStream(Stream stream)
        {
            _stream = stream;
            _peeked = new byte[16];
            _peekedLength = 0;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_peekedLength == 0)
            {
                return _stream.Read(buffer, offset, count);
            }

            for (var i = 0; i < count; i++)
            {
                buffer[offset + i] = Pop();
            }
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public int Peek(int depth)
        {
            if (_peeked.Length <= depth)
            {
                var temp = new byte[depth + 16];
                for (var i = 0; i < _peeked.Length; i++)
                {
                    temp[i] = _peeked[i];
                }
                _peeked = temp;
            }

            if (depth >= _peekedLength)
            {
                var offset = _peekedLength;
                var length = (depth - _peekedLength) + 1;
                var lengthRead = _stream.Read(_peeked, offset, length);

                if (lengthRead < 1)
                {
                    return -1;
                }

                _peekedLength = depth + 1;
            }

            return _peeked[depth];
        }

        public char PeekChar()
        {
            return (char) Peek(0);
        }

        public string TakeWhile(Func<char, bool> predicate)
        {
            var builder = new StringBuilder();
            while (true)
            {
                var raw = Peek(0);
                if (raw == -1)
                {
                    break;
                }
                var ch = (char) raw;
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
                var raw = Peek(0);
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

        private byte Pop()
        {
            var result = _peeked[0];
            _peekedLength--;
            for (var i = 0; i < _peekedLength; i++)
            {
                _peeked[i] = _peeked[i + 1];
            }

            return result;
        }

        private int _peekedLength;
        private byte[] _peeked;
        private readonly Stream _stream;
    }
}

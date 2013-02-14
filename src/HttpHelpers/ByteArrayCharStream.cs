using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpHelpers
{
    public class ByteArrayCharStream : CharStreamBase
    {
        public ByteArrayCharStream(IEnumerable<byte> buffer)
        {
            _buffer = buffer;
            _byte = _buffer.ElementAtOrDefault(_offset);
        }

        public override int ReadByte()
        {
            var current = _byte;
            if (current == default(byte))
            {
                return -1;
            }
            _byte = _buffer.ElementAtOrDefault(++_offset);
            return current;
        }

        public override int PeekByte()
        {
            if (_byte == default(byte))
            {
                return -1;
            }
            return _byte;
        }

        public override char PeekChar()
        {
            return (char)PeekByte();
        }

        private int _offset;
        private byte _byte;
        private readonly IEnumerable<byte> _buffer;
    }
}

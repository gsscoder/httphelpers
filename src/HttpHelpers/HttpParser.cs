using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpHelpers.Extensions;

namespace HttpHelpers
{
    public class HttpParser
    {
        public void ParseRequest(IHttpParserCallbacks callbacks, Stream stream)
        {
            ParseMessage(callbacks, stream, peekable =>
                {
                    var method = peekable.TakeWhile(c => !c.IsWhiteSpace());
                    if (!peekable.PeekChar().IsWhiteSpace())
                    {
                        return false;
                    }
                    peekable.SkipWhile(c => c.IsWhiteSpace());

                    var uri = peekable.TakeWhile(c => !c.IsWhiteSpace());
                    if (!peekable.PeekChar().IsWhiteSpace())
                    {
                        return false;
                    }
                    peekable.SkipWhile(c => c.IsWhiteSpace());

                    var version = peekable.TakeWhile(c => !c.IsLineTerminator());
                    if (!peekable.PeekChar().IsLineTerminator())
                    {
                        return false;
                    }
                    callbacks.OnRequestLine(method, uri, version);

                    return true;
                });
        }

        private void ParseMessage(IHttpParserCallbacks callbacks, Stream stream,
            Func<PeekableStream, bool> parseHeading)
        {
            var peekable = stream.AsPeekableStream();

            callbacks.OnMessageBegin();

            if (!parseHeading(peekable))
            {
                callbacks.OnMessageEnd();
                return;
            }

            var breakSize = peekable.SkipWhile(c => c.IsLineTerminator());

            while (true)
            {
                var headerName = peekable.TakeWhile(c => c != '\x3A'); // ':'
                if (peekable.PeekChar() != '\x3A')
                {
                    continue;
                }
                peekable.ReadByte();

                headerName = headerName.TrimStart();

                var headerValue = peekable.TakeWhile(c => !c.IsLineTerminator());
                if (!peekable.PeekChar().IsLineTerminator())
                {
                    continue;
                }
                headerValue = headerValue.Trim();

                callbacks.OnHeaderLine(headerName, headerValue);

                var lineEnd = peekable.SkipWhile(c => c.IsLineTerminator());
                if (lineEnd > breakSize)
                {
                    break;
                }
            }

            var body = new List<byte>(1024);
            if (peekable.Peek(0) != -1)
            {
                while (true)
                {
                    var raw = stream.ReadByte();
                    if (raw == -1)
                    {
                        break;
                    }
                    body.Add((byte) raw);
                }
            }

            callbacks.OnBody(new ArraySegment<byte>(body.ToArray()));

            callbacks.OnMessageEnd();
        }
    }
}
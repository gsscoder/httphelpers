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
            ParseMessage(callbacks, stream, () =>
                {
                    bool matched;
                    var method = stream.ReadUntil(c => c.IsWhiteSpace(), out matched);
                    if (!matched)
                    {
                        return false;
                    }
                    var uri = stream.ReadUntil(c => c.IsWhiteSpace(), out matched);
                    if (!matched)
                    {
                        return false;
                    }
                    var version = stream.ReadUntil(c => c.IsLineTerminator(), out matched);
                    if (!matched)
                    {
                        return false;
                    }
                    callbacks.OnRequestLine(method, uri, version);

                    return true;
                });
        }

        private void ParseMessage(IHttpParserCallbacks callbacks, Stream stream,
            Func<bool> parseHeading)
        {
            callbacks.OnMessageBegin();

            var parserState = parseHeading();
            if (!parserState)
            {
                callbacks.OnMessageEnd();
                return;
            }

            char last;
            var breakSize = (stream.SkipWhile(c => c.IsLineTerminator(), out last) + 1);

            while (true)
            {
                bool matched;
                var headerName = stream.ReadUntil(ch => ch == '\x3A', out matched); // ':'
                if (!matched)
                {
                    continue;
                }
                if (last != '\0')
                {
                    headerName = string.Concat(last, headerName);
                    last = '\0';
                }
                headerName = string.Join(string.Empty,
                    headerName.SkipWhile(c => c.IsWhiteSpace() || c.IsLineTerminator()));

                var headerValue = stream.ReadUntil(c => c.IsLineTerminator(), out matched);
                if (!matched)
                {
                    continue;
                }
                headerValue = string.Join(string.Empty,
                    headerValue.TakeWhile(c => !c.IsLineTerminator())).Trim();

                callbacks.OnHeaderLine(headerName, headerValue);

                var lineEnd = (stream.SkipWhile(c => c.IsLineTerminator(), out last) + 1);
                if (lineEnd > breakSize)
                {
                    break;
                }
            }

            var body = new List<byte>(1024);
            if (last != '\0')
            {
                body.Add((byte) last);
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
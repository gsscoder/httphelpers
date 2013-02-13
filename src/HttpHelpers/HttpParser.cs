#region License
//
// Http Core Library: HttpParser.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using HttpHelpers.Extensions;
#endregion

namespace HttpHelpers
{
    public class HttpParser
    {
        public static HttpParser Default
        {
            get { return _defaultParser ?? (_defaultParser = new HttpParser()); }
        }

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

        public void ParseResponse(IHttpParserCallbacks callbacks, Stream stream)
        {
            ParseMessage(callbacks, stream, peekable =>
            {
                var version = peekable.TakeWhile(c => !c.IsWhiteSpace());
                if (!peekable.PeekChar().IsWhiteSpace())
                {
                    return false;
                }
                peekable.SkipWhile(c => c.IsWhiteSpace());

                var rawCode = peekable.TakeWhile(c => !c.IsWhiteSpace());
                if (!peekable.PeekChar().IsWhiteSpace())
                {
                    return false;
                }
                peekable.SkipWhile(c => c.IsWhiteSpace());
                int parsedCode;
                int? code = null;
                if (int.TryParse(rawCode, out parsedCode))
                {
                    code = parsedCode;
                }

                var reason = peekable.TakeWhile(c => !c.IsLineTerminator());
                if (!peekable.PeekChar().IsLineTerminator())
                {
                    return false;
                }
                callbacks.OnResponseLine(version, code, reason);

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
                    var raw = peekable.ReadByte();
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

        private static volatile HttpParser _defaultParser;
    }
}
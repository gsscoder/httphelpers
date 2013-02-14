﻿#region License
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

        public void ParseRequest(IHttpParserCallbacks callbacks, CharStreamBase charStream)
        {
            ParseMessage(callbacks, charStream, cs => ParseRequestLine(callbacks, cs));
        }

        public void ParseResponse(IHttpParserCallbacks callbacks, CharStreamBase charStream)
        {
            ParseMessage(callbacks, charStream, cs => ParseResponseLine(callbacks, cs));
        }

        private static bool ParseRequestLine(IHttpParserCallbacks callbacks, CharStreamBase charStream)
        {
            var method = charStream.TakeWhile(c => !c.IsWhiteSpace());
            if (!charStream.PeekChar().IsWhiteSpace())
            {
                return false;
            }
            charStream.SkipWhile(c => c.IsWhiteSpace());

            var uri = charStream.TakeWhile(c => !c.IsWhiteSpace());
            if (!charStream.PeekChar().IsWhiteSpace())
            {
                return false;
            }
            charStream.SkipWhile(c => c.IsWhiteSpace());

            var version = charStream.TakeWhile(c => !c.IsLineTerminator());
            if (!charStream.PeekChar().IsLineTerminator())
            {
                return false;
            }
            callbacks.OnRequestLine(method, uri, version);

            return true;
        }

        private static bool ParseResponseLine(IHttpParserCallbacks callbacks, CharStreamBase charStream)
        {
            var version = charStream.TakeWhile(c => !c.IsWhiteSpace());
            if (!charStream.PeekChar().IsWhiteSpace())
            {
                return false;
            }
            charStream.SkipWhile(c => c.IsWhiteSpace());

            var rawCode = charStream.TakeWhile(c => !c.IsWhiteSpace());
            if (!charStream.PeekChar().IsWhiteSpace())
            {
                return false;
            }
            charStream.SkipWhile(c => c.IsWhiteSpace());
            int parsedCode;
            int? code = null;
            if (int.TryParse(rawCode, out parsedCode))
            {
                code = parsedCode;
            }

            var reason = charStream.TakeWhile(c => !c.IsLineTerminator());
            if (!charStream.PeekChar().IsLineTerminator())
            {
                return false;
            }
            callbacks.OnResponseLine(version, code, reason);

            return true;
        }

        private void ParseMessage(IHttpParserCallbacks callbacks, CharStreamBase charStream,
            Func<CharStreamBase, bool> parseHeading)
        {
            callbacks.OnMessageBegin();

            if (!parseHeading(charStream))
            {
                callbacks.OnMessageEnd();
                return;
            }

            var breakSize = charStream.SkipWhile(c => c.IsLineTerminator());

            while (true)
            {
                var headerName = charStream.TakeWhile(c => c != '\x3A'); // ':'
                if (charStream.PeekChar() != '\x3A')
                {
                    continue;
                }
                charStream.ReadByte();

                headerName = headerName.TrimStart();

                var headerValue = charStream.TakeWhile(c => !c.IsLineTerminator());
                if (!charStream.PeekChar().IsLineTerminator())
                {
                    continue;
                }
                headerValue = headerValue.Trim();

                callbacks.OnHeaderLine(headerName, headerValue);

                var lineEnd = charStream.SkipWhile(c => c.IsLineTerminator());
                if (lineEnd > breakSize)
                {
                    break;
                }
            }

            var body = new List<byte>(1024);
            if (charStream.PeekByte() != -1)
            {
                while (true)
                {
                    var raw = charStream.ReadByte();
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
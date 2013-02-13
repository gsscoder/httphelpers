#region License
//
// Http Core Library: StringExtensions.cs
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
using System.Linq;
using System.Text;
#endregion

namespace HttpHelpers.Extensions
{
    static class StreamExtensions
    {
        /*
        static bool ParseNextHttpHeader(this Stream stream, out string name, out string value)
        {
            name = string.Empty;
            value = string.Empty;
            bool matched;
            var headerName = stream.ReadUntil(ch => ch == '\x3A', out matched); // ':'
            if (!matched)
            {
                return false;
            }
            headerName = headerName.TrimStartIf('\x20', '\xA', '\xD') // ' ', '\r', '\n'
                .TrimEndIf('\x20');
            name = headerName;

            var headerValue = stream.ReadUntil(ch => (ch == '\xA' || ch == '\xD'), out matched);
            if (!matched)
            {
                return false;
            }
            headerValue = headerValue.TrimStartIf('\x20')
                .TrimEndIf('\x20');
            value = headerValue;

            return headerName.Length > 0 && headerValue.Length > 0;
        }

        static bool ParseHttpRequestLine(this Stream stream, out string method, out string uri, out string version)
        {
            method = string.Empty;
            uri = string.Empty;
            version = string.Empty;

            bool matched;
            var part1 = stream.ReadUntil(ch => ch == '\x20', out matched);
            if (!matched)
            {
                return false;
            }
            method = part1;

            var part2 = stream.ReadUntil(ch => ch == '\x20', out matched);
            if (!matched)
            {
                return false;
            }
            uri = part2;

            var part3 = stream.ReadUntil(ch => (ch == '\xA' || ch == '\xD'), out matched);
            if (!matched)
            {
                return false;
            }
            version = part3;

            return part1.Length > 0 && part2.Length > 0 && part3.Length > 0;
        }

        static bool ParseHttpResponseLine(this Stream stream, out string version, out int? statusCode, out string reason)
        {
            version = string.Empty;
            statusCode = null;
            reason = string.Empty;

            bool matched;
            var part1 = stream.ReadUntil(ch => ch == '\x20', out matched);
            if (!matched)
            {
                return false;
            }
            version = part1;

            var part2 = stream.ReadUntil(ch => ch == '\x20', out matched);
            if (!matched)
            {
                return false;
            }
            int parsed;
            if (int.TryParse(part2, out parsed))
            {
                statusCode = parsed;
            }
            else
            {
                return false;
            }

            var part3 = stream.ReadUntil(ch => (ch == '\xA' || ch == '\xD'), out matched);
            if (!matched)
            {
                return false;
            }
            reason = part3;

            return part1.Length > 0 && part2.Length > 0 && part3.Length > 0;
        }
        */

        public static string ReadUntil(this Stream stream, Func<char, bool> match, out bool matched)
        {
            matched = false;
            var builder = new StringBuilder();
            while (true)
            {
                var c = stream.ReadByte();
                var ch = (char) c;
                if (match(ch))
                {
                    matched = true;
                    break;
                }
                if (c == -1)
                {
                    break;
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string ReadUntil(this Stream stream, Func<char, bool> match)
        {
            var builder = new StringBuilder();
            while (true)
            {
                var c = stream.ReadByte();
                var ch = (char) c;
                if (match(ch) || c == -1)
                {
                    break;
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static int SkipWhile(this Stream stream, Func<char, bool> match, out char last)
        {
            last = '\0';
            var skipped = 0;
            while (true)
            {
                var c = stream.ReadByte();
                var ch = (char)c;
                if (c == -1)
                {
                    last = '\0';
                    break;
                }
                if (!match(ch))
                {
                    last = ch;
                    break;
                }
                skipped++;
            }
            return skipped;
        }
    }
}

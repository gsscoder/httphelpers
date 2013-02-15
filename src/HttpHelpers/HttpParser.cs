#region License
//
// Http Helpers Library: HttpParser.cs
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
using System.IO;
using System.Threading.Tasks;
using HttpHelpers.Extensions;
#endregion

namespace HttpHelpers
{
    public static class HttpParser
    {
        public static async Task<bool> ParseMessageAsync(Stream stream,
            Action<string, string, string> onHeadingLine,
            Action<string, string> onHeaderLine)
        {
            var reader = new StreamReader(stream);

            var headingParsing = await ParseHeadingLineAsync(reader, onHeadingLine);
            if (!headingParsing)
            {
                return false;
            }

            while (true)
            {
                var headerParsing = await ParseHeaderLineAsync(reader, onHeaderLine);
                if (headerParsing == null)
                {
                    break;
                }
                if (!headerParsing.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private static async Task<bool> ParseHeadingLineAsync(StreamReader reader,
            Action<string, string, string> onHeadingLine)
        {
            var line = await reader.ReadLineAsync();
            var stringReader = new StringReader(line);

            var item1 = stringReader.TakeWhile(c => !c.IsWhiteSpace());
            if (!stringReader.PeekChar().IsWhiteSpace())
            {
                return false;
            }
            stringReader.SkipWhiteSpace();

            var item2 = stringReader.TakeWhile(c => !c.IsWhiteSpace());
            if (!stringReader.PeekChar().IsWhiteSpace())
            {
                return false;
            }
            stringReader.SkipWhiteSpace();

            var item3 = stringReader.ReadToEnd();

            onHeadingLine(item1, item2, item3);

            return true;
        }

        private static async Task<bool?> ParseHeaderLineAsync(StreamReader reader,
            Action<string, string> onHeaderLine)
        {
            var line = await reader.ReadLineAsync();
            if (line.Length == 0)
            {
                return null;
            }

            var stringReader = new StringReader(line);

            stringReader.SkipWhiteSpace();

            var header = stringReader.TakeWhile(c => c != '\x3A');
            if (stringReader.PeekChar() != '\x3A')
            {
                return false;
            }
            stringReader.Read();

            stringReader.SkipWhiteSpace();

            var value = stringReader.ReadToEnd();

            onHeaderLine(header, value ?? string.Empty);

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpHelpers.Extensions;

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
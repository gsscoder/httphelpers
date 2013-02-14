using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpHelpers.Tests.Extensions
{
    static class StringExtensions
    {
        public static byte[] AsByteArray(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static Stream AsStream(this string value)
        {
            var stream = new MemoryStream(value.AsByteArray());
            return stream;
        }
    }
}
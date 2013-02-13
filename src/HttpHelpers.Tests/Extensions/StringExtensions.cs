using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpHelpers.Tests.Extensions
{
    static class StringExtensions
    {
        public static Stream AsStream(this string value)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            return stream;
        }
    }
}
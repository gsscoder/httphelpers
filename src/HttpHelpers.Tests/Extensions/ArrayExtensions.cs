using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpHelpers.Tests.Extensions
{
    static class ArrayExtensions
    {
        public static string ToDecodedString(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }
    }
}

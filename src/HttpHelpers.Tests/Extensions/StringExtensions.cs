using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpHelpers.Tests.Extensions
{
    static class StringExtensions
    {
        //public static Tokenizer CreateTokenizer(this string value)
        //{
        //    // Simulating scenario closest to the real
        //    var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        //    var bytes = new byte[stream.Length];
        //    stream.Seek(0, SeekOrigin.Begin);
        //    stream.Read(bytes, 0, (int)stream.Length);
        //    return new Tokenizer(new ArraySegment<byte>(bytes));
        //}

        //public static ArraySegment<byte> AsArraySegment(this string value)
        //{
        //    return new ArraySegment<byte>(Encoding.UTF8.GetBytes(value));
        //}

        public static Stream AsStream(this string value)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            return stream;
        }
    }
}
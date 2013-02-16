using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssertExLib;
using Xunit;

namespace HttpHelpers.Tests.Unit
{
    partial class HttpParserFixture
    {
        public class Async : HttpParserFixture
        {
            public Async()
                : base(true)
            {
            }

            [Fact]
            public void Should_throws_exception_on_null_stream()
            {
                // Given
                var stream = (Stream) null;

                // Than When
                AssertEx.TaskThrows<ArgumentNullException>(
                    () => HttpParser.ParseMessageAsync(stream, (segm0, segm1, segm2) => { }, (header, value) => { }));
            }
        }
    }
}

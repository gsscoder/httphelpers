using System;
using System.IO;
using Xunit;

namespace HttpHelpers.Tests.Unit
{
    partial class HttpParserFixture
    {
        public class Sync : HttpParserFixture
        {
            public Sync()
                : base(false)
            {
            }

            [Fact]
            public void Should_throws_exception_on_null_stream()
            {
                // Given
                var stream = (Stream) null;

                // When
                Action act = () => HttpParser.ParseMessage(stream, (segm0, segm1, segm2) => { }, (header, value) => { });

                // Than
                Assert.Throws<ArgumentNullException>(() => act());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HttpHelpers.Tests.Extensions;
using Xunit;

namespace HttpHelpers.Tests.Unit
{
    public class ByteArrayStreamFixture
    {
        [Fact]
        public void Should_peek_next_byte()
        {
            // Given
            var bytes = "abc".AsByteArray();

            // Wheny
            var reader = new ByteArrayCharStream(bytes);

            // Than
            reader.PeekByte().Should().Be('a');
            reader.ReadByte().Should().Be('a');

            // Than
            reader.PeekByte().Should().Be('b');
            reader.ReadByte().Should().Be('b');

            // Than
            reader.PeekByte().Should().Be('c');
            reader.ReadByte().Should().Be('c');

            // Than
            reader.PeekByte().Should().Be(-1);
            reader.ReadByte().Should().Be(-1);
        }
    }
}

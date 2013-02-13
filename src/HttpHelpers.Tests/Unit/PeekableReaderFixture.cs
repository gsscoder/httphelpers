using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HttpHelpers.Tests.Extensions;
using Xunit;

namespace HttpHelpers.Tests.Unit
{
    public class PeekableReaderFixture
    {
        [Fact]
        public void Should_peek_next_byte()
        {
            // Given
            var stream = "abc".AsStream();

            // When
            var reader = new PeekableReader(stream);

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HttpHelpers.Tests.Extensions;
using Xunit;

namespace HttpHelpers.Tests.Unit
{
    public class PeekableStreamFixture
    {
        [Fact]
        public void Should_allow_to_peek_first_byte_from_the_underlying_stream()
        {
            // Given
            var stream = "Should_allow_to_peek_first_byte_from_the_underlying_stream".AsStream();

            // When
            var peekable = new PeekableStream(stream);

            // Than
            peekable.Peek(0).Should().Be('S');
        }

        [Fact]
        public void Should_allow_to_peek_three_bytes_ahead_from_the_underlying_stream()
        {
            // Given
            var stream = "Should_allow_to_peek_three_bytes_ahead_from_the_underlying_stream".AsStream();

            // When
            var peekable = new PeekableStream(stream);

            // Than
            peekable.Peek(2).Should().Be('o');
        }

        [Fact]
        public void Should_allow_to_peek_one_bytes_after_read_from_the_underlying_stream()
        {
            // Given
            var stream = "Should_allow_to_peek_one_bytes_after_read_from_the_underlying_stream".AsStream();

            // When
            var peekable = new PeekableStream(stream);
            peekable.ReadByte();
            peekable.ReadByte();
            peekable.ReadByte();

            // Than
            peekable.Peek(0).Should().Be('u');
        }

        [Fact]
        public void Should_allow_to_peek_two_bytes_ahead_after_read_from_the_underlying_stream()
        {
            // Given
            var stream = "Should_allow_to_peek_two_bytes_ahead_after_read_from_the_underlying_stream".AsStream();

            // When
            var peekable = new PeekableStream(stream);
            peekable.ReadByte();

            // Than
            peekable.Peek(1).Should().Be('o');
        }
    }
}

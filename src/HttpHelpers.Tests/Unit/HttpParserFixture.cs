using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HttpHelpers.Tests.Extensions;
using HttpHelpers.Tests.Fakes;
using Xunit;

namespace HttpHelpers.Tests.Unit
{
    public class HttpParserFixture
    {
        [Fact]
        public void Should_parse_request_with_one_header()
        {
            // Given
            var stream = ("GET /gsscoder/httphelpers HTTP/1.1\r\n" +
                          "Content-Type: text/html; q=0.9, text/plain\r\n\r\n").AsStream();
            var parser = new HttpParser();
            var callbacks = new FakeHttpParserCallbacks();

            // When
            parser.ParseRequest(callbacks, stream);

            // Than
            callbacks.RequestLine.Method.Should().Be("GET");
            callbacks.RequestLine.Uri.Should().Be("/gsscoder/httphelpers");
            callbacks.RequestLine.Version.Should().Be("HTTP/1.1");
            callbacks.Headers.Should().HaveCount(c => c == 1);
            callbacks.Headers["Content-Type"].Should().Be("text/html; q=0.9, text/plain");
            callbacks.Body.Should().HaveCount(c => c == 0);
        }
    }
}

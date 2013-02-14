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
            var callbacks = new FakeHttpParserCallbacks();

            // When
            HttpParser.ParseRequest(CharStreamBase.FromStream(stream), callbacks);

            // Than
            callbacks.RequestLine.Method.Should().Be("GET");
            callbacks.RequestLine.Uri.Should().Be("/gsscoder/httphelpers");
            callbacks.RequestLine.Version.Should().Be("HTTP/1.1");
            callbacks.Headers.Should().HaveCount(c => c == 1);
            callbacks.Headers["Content-Type"].Should().Be("text/html; q=0.9, text/plain");
            callbacks.Body.Should().HaveCount(c => c == 0);
        }

        [Fact]
        public void Should_parse_request_with_three_headers_and_various_line_endings()
        {
            // Given
            var stream = ("GET /gsscoder/httphelpers HTTP/1.1\r\n" +
                          "Host: github.com\r\n" +
                          "Accept: */*\r\n" +
                          "Content-Type: text/html; q=0.9, text/plain\r\n\r\n" +
                          "\r\n\r\n\r\n\r\n").AsStream();
            var callbacks = new FakeHttpParserCallbacks();

            // When
            HttpParser.ParseRequest(CharStreamBase.FromStream(stream), callbacks);

            // Than
            callbacks.RequestLine.Method.Should().Be("GET");
            callbacks.RequestLine.Uri.Should().Be("/gsscoder/httphelpers");
            callbacks.RequestLine.Version.Should().Be("HTTP/1.1");
            callbacks.Headers.Should().HaveCount(c => c == 3);
            callbacks.Body.Should().HaveCount(c => c == 0);
        }

        [Fact]
        public void Should_parse_response_with_one_header_and_body()
        {
            // Given
            var stream = ("HTTP/1.1 200 OK\r\n" +
                          "Date: Sun, 08 Oct 2000 18:46:12 GMT\r\n\r\n" +
                          "<html><body><p>Heartbeat!</p></body></html>\r\n").AsStream();
            var callbacks = new FakeHttpParserCallbacks();

            // When
            HttpParser.ParseResponse(CharStreamBase.FromStream(stream), callbacks);

            // Than
            callbacks.ResponseLine.Version.Should().Be("HTTP/1.1");
            callbacks.ResponseLine.Code.Should().Be(200);
            callbacks.ResponseLine.Reason.Should().Be("OK");
            callbacks.Headers.Should().HaveCount(c => c == 1);
            callbacks.Headers["Date"].Should().Be("Sun, 08 Oct 2000 18:46:12 GMT");
            callbacks.Body.ToDecodedString().Should().Be("<html><body><p>Heartbeat!</p></body></html>\r\n");
        }
    }
}

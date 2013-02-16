using System;
using System.IO;
using FluentAssertions;
using HttpHelpers.Tests.Extensions;
using HttpHelpers.Tests.Fakes;
using Xunit;

namespace HttpHelpers.Tests.Unit
{
    public abstract partial class HttpParserFixture
    {
        protected Func<Stream,
            Action<string, string, string>, // onHeading
            Action<string, string>, // onHeader
            bool> ParseMethod;

        protected HttpParserFixture(bool asyncApi)
        {
            if (asyncApi)
            {
                ParseMethod = (stream, onHeading, onHeader) =>
                    {
                        var parseTask = HttpParser.ParseMessageAsync(stream, onHeading, onHeader);
                        parseTask.Wait();
                        return parseTask.Result;
                    };
            }
            else
            {
                ParseMethod = (stream, onHeading, onHeader) =>
                    HttpParser.ParseMessage(stream, onHeading, onHeader);
            }
        }

        [Fact]
        public void Should_parse_request_with_one_header()
        {
            // Given
            var stream = ("GET /gsscoder/httphelpers HTTP/1.1\r\n" +
                          "Content-Type: text/html; q=0.9, text/plain\r\n\r\n").AsStream();
            var target = new FakeHttpParserTarget();

            // When
            var result = ParseMethod(stream, (method, uri, version) =>
                {
                    target.RequestLine.Method = method;
                    target.RequestLine.Uri = uri;
                    target.RequestLine.Version = version;
                },
            (header, value) => 
                target.Headers.Add(header, value));

            // Than
            result.Should().BeTrue();
            target.RequestLine.Method.Should().Be("GET");
            target.RequestLine.Uri.Should().Be("/gsscoder/httphelpers");
            target.RequestLine.Version.Should().Be("HTTP/1.1");
            target.Headers.Should().HaveCount(c => c == 1);
            target.Headers["Content-Type"].Should().Be("text/html; q=0.9, text/plain");
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
            var target = new FakeHttpParserTarget();

            // When
            var result = ParseMethod(stream, (method, uri, version) =>
                {
                    target.RequestLine.Method = method;
                    target.RequestLine.Uri = uri;
                    target.RequestLine.Version = version;
                },
            (header, value) =>
                target.Headers.Add(header, value));

            // Than
            result.Should().BeTrue();
            target.RequestLine.Method.Should().Be("GET");
            target.RequestLine.Uri.Should().Be("/gsscoder/httphelpers");
            target.RequestLine.Version.Should().Be("HTTP/1.1");
            target.Headers.Should().HaveCount(c => c == 3);
        }

        [Fact]
        public void Should_parse_response_with_one_header_and_body()
        {
            // Given
            var stream = ("HTTP/1.1 200 OK\r\n" +
                          "Date: Sun, 08 Oct 2000 18:46:12 GMT\r\n\r\n" +
                          "<html><body><p>Heartbeat!</p></body></html>\r\n").AsStream();
            var target = new FakeHttpParserTarget();

            // When
            var result = ParseMethod(stream, (version, code, reason) =>
                {
                    target.ResponseLine.Version = version;
                    target.ResponseLine.Code = code;
                    target.ResponseLine.Reason = reason;
                },
            (header, value) =>
                target.Headers.Add(header, value));

            // Than
            result.Should().BeTrue();
            target.ResponseLine.Version.Should().Be("HTTP/1.1");
            target.ResponseLine.Code.Should().Be("200");
            target.ResponseLine.Reason.Should().Be("OK");
            target.Headers.Should().HaveCount(c => c == 1);
            target.Headers["Date"].Should().Be("Sun, 08 Oct 2000 18:46:12 GMT");
        }

        [Fact]
        public void Should_return_false_when_message_is_malformed()
        {
            // Given
            var stream = "ET /g\rsscoder/httphel\0pers HTTP/".AsStream();

            // When
            var result = ParseMethod(stream, (segm0, segm1, segm2) => { }, (header, value) => { });

            // Than
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_when_message_is_empty()
        {
            // Given
            var stream = "".AsStream();

            // When
            var result = ParseMethod(stream, (segm0, segm1, segm2) => { }, (header, value) => { });

            // Than
            result.Should().BeFalse();
        }
    }
}
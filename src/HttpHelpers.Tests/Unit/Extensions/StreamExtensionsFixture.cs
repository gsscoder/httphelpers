using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HttpHelpers.Extensions;
using HttpHelpers.Tests.Extensions;
using Xunit;

namespace HttpHelpers.Tests.Unit.Extensions
{
    public class StreamExtensionsFixture
    {
        #region Http Headers
        [Fact]
        public void Should_parse_a_single_header()
        {
            // Given
            var stream = "Content-Type: text/html\r\n".AsStream();

            // When
            string name;
            string value;
            var result = stream.ParseNextHttpHeader(out name, out value);

            // Than
            result.Should().BeTrue();
            name.Should().Be("Content-Type");
            value.Should().Be("text/html");
        }

        [Fact]
        public void Should_parse_a_pair_of_http_header()
        {
            // Given
            var stream = "Content-Type: text/html\r\nHost: github.com\r\n".AsStream();

            // When
            string name1;
            string value1;
            var result = stream.ParseNextHttpHeader(out name1, out value1);

            // Than
            result.Should().BeTrue();
            name1.Should().Be("Content-Type");
            value1.Should().Be("text/html");

            // When
            string name2;
            string value2;
            var result2 = stream.ParseNextHttpHeader(out name2, out value2);

            // Than
            result2.Should().BeTrue();
            name2.Should().Be("Host");
            value2.Should().Be("github.com");
        }

        [Fact]
        public void Should_put_out_empty_strings_with_bad_input()
        {
            // Given
            var stream = "non header content\r\n".AsStream();

            // When
            string name;
            string value;
            var result = stream.ParseNextHttpHeader(out name, out value);

            // Than
            result.Should().BeFalse();
            name.Should().Be(string.Empty);
            value.Should().Be(string.Empty);
        }

        [Fact]
        public void Should_put_out_empty_strings_with_empty_stream()
        {
            // Given
            var stream = string.Empty.AsStream();

            // When
            string name;
            string value;
            var result = stream.ParseNextHttpHeader(out name, out value);

            // Than
            result.Should().BeFalse();
            name.Should().Be(string.Empty);
            value.Should().Be(string.Empty);
        }

        [Fact]
        public void Should_put_out_at_least_the_header_name_when_value_is_missing()
        {
            // Given
            var stream = "Content-Type: \r\n".AsStream();

            // When
            string name;
            string value;
            var result = stream.ParseNextHttpHeader(out name, out value);

            // Than
            result.Should().BeFalse();
            name.Should().Be("Content-Type");
            value.Should().Be(string.Empty);
        }
        #endregion

        #region Request Line Extensions
        [Fact]
        public void Should_parse_a_request_line_with_root_uri()
        {
            // Given
            var stream = "GET / HTTP/1.1\r\n".AsStream();

            // When
            string method;
            string uri;
            string version;
            var result = stream.ParseHttpRequestLine(out method, out uri, out version);

            // Than
            result.Should().BeTrue();
            method.Should().Be("GET");
            uri.Should().Be("/");
            version.Should().Be("HTTP/1.1");
        }

        [Fact]
        public void Should_parse_a_request_line()
        {
            // Given
            var stream = "GET /gsscoder/httphelpers HTTP/1.1\r\n".AsStream();

            // When
            string method;
            string uri;
            string version;
            var result = stream.ParseHttpRequestLine(out method, out uri, out version);

            // Than
            result.Should().BeTrue();
            method.Should().Be("GET");
            uri.Should().Be("/gsscoder/httphelpers");
            version.Should().Be("HTTP/1.1");
        }

        [Fact]
        public void Should_return_false_on_request_line_without_version()
        {
            // Given
            var stream = "GET /gsscoder/httphelpers \r\n".AsStream();

            // When
            string method;
            string uri;
            string version;
            var result = stream.ParseHttpRequestLine(out method, out uri, out version);

            // Than
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_on_request_line_without_uri_and_version()
        {
            // Given
            var stream = "GET \r\n".AsStream();

            // When
            string method;
            string uri;
            string version;
            var result = stream.ParseHttpRequestLine(out method, out uri, out version);

            // Than
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_when_request_line_is_empty()
        {
            // Given
            var stream = "".AsStream();

            // When
            string method;
            string uri;
            string version;
            var result = stream.ParseHttpRequestLine(out method, out uri, out version);

            // Than
            result.Should().BeFalse();
        }
        #endregion

        #region Response Status Line
        [Fact]
        public void Should_parse_a_response_line()
        {
            // Given
            var stream = "HTTP/1.1 200 OK\r\n".AsStream();

            // When
            string version;
            int? code;
            string reason;
            var result = stream.ParseHttpResponseLine(out version, out code, out reason);

            // Than
            result.Should().BeTrue();
            version.Should().Be("HTTP/1.1");
            code.Should().Be(200);
            reason.Should().Be("OK");
        }

        [Fact]
        public void Should_parse_a_response_line_with_reason_that_has_white_space()
        {
            // Given
            var stream = "HTTP/1.1 405 Method Not Allowed\r\n".AsStream();

            // When
            string version;
            int? code;
            string reason;
            var result = stream.ParseHttpResponseLine(out version, out code, out reason);

            // Than
            result.Should().BeTrue();
            version.Should().Be("HTTP/1.1");
            code.Should().Be(405);
            reason.Should().Be("Method Not Allowed");
        }

        [Fact]
        public void Should_return_false_on_response_line_with_non_numeric_status_code()
        {
            // Given
            var stream = "HTTP/1.1 ABC Method Not Allowed\r\n".AsStream();

            // When
            string version;
            int? code;
            string reason;
            var result = stream.ParseHttpResponseLine(out version, out code, out reason);

            // Than
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_on_response_line_without_reason()
        {
            // Given
            var stream = "HTTP/1.1 100\r\n".AsStream();

            // When
            string version;
            int? code;
            string reason;
            var result = stream.ParseHttpResponseLine(out version, out code, out reason);

            // Than
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_on_response_line_without_status_code_and_reason()
        {
            // Given
            var stream = "HTTP/1.1\r\n".AsStream();

            // When
            string version;
            int? code;
            string reason;
            var result = stream.ParseHttpResponseLine(out version, out code, out reason);

            // Than
            result.Should().BeFalse();
        }

        [Fact]
        public void Should_return_false_when_response_line_is_empty()
        {
            // Given
            var stream = "".AsStream();

            // When
            string version;
            int? code;
            string reason;
            var result = stream.ParseHttpResponseLine(out version, out code, out reason);

            // Than
            result.Should().BeFalse();
        }
        #endregion
    }
}

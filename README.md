Http Helpers 0.1.0.7 alfa.
===
This project was born because [@davidfawl](https://twitter.com/davidfowl) wrote on Twitter about the creation of an [OWIN HTTP client](https://github.com/davidfowl/OwinHttpClient).
I was very interested due my [Surf Http Library](https://github.com/gsscoder/surfhttp), created mainly for testing purposes.
After exchanging a few words, I thought it was interesting to have some general purpose HTTP helper types;
but for me mine first project was not suitable to host them.
Please note that for now __this is just a stub__, a placeholder to start working on the subject.

To build:
---
MonoDevelop or Visual Studio.

At glance:
---
The library exposes an HTTP request/response parser.
See the example below and [unit tests](https://github.com/gsscoder/httphelpers/blob/master/src/HttpHelpers.Tests/Unit/HttpParserFixture.cs).

```csharp
// Given
var stream = (
  "GET /gsscoder/httphelpers HTTP/1.1\r\n" +
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
```

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)

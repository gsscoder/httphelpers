Http Helpers 0.1.2.0 alfa.
===
This project was born because [@davidfawl](https://twitter.com/davidfowl) wrote on Twitter about the creation of an [OWIN HTTP client](https://github.com/davidfowl/OwinHttpClient).
I was very interested due my [Surf Http Library](https://github.com/gsscoder/surfhttp), created mainly for testing purposes.
After exchanging a few words, I thought it was interesting to have some general purpose HTTP helper types;
but for me mine first project was not suitable to host them.

![NuGet: Install-Package HttpHelpers.Sources -Pre](https://raw.github.com/gsscoder/httphelpers/master/HttpHelpersNuGet.png)

To build:
---
MonoDevelop or Visual Studio.

At glance:
---
 - The library exposes an async HTTP request/response parser.
 - See the example below, [unit tests](https://github.com/gsscoder/httphelpers/blob/master/src/HttpHelpers.Tests/Unit/HttpParserFixture.cs) or this [sample web server](https://gist.github.com/gsscoder/4945688) built with library HTTP parser (now included in the solution tree).

```csharp
// Given
var stream = ("GET /gsscoder/httphelpers HTTP/1.1\r\n" +
                "Content-Type: text/html; q=0.9, text/plain\r\n\r\n").AsStream();
var request = new FakeHttpRequest();

// When
HttpParser.ParseMessageAsync(stream, (method, uri, version) =>
    {
        request.Method = method;
        request.Uri = uri;
        request.Version = version;
    },
    (header, value) => 
        request.Add(header, value)).Wait();

// Than
request.Method.Should().Be("GET");
request.Uri.Should().Be("/gsscoder/httphelpers");
request.Version.Should().Be("HTTP/1.1");
request.Headers.Should().HaveCount(c => c == 1);
request.Headers["Content-Type"].Should().Be("text/html; q=0.9, text/plain");
```

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)

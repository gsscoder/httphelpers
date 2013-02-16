Http Helpers 0.1.2.0 alfa.
===
Easy to use, one method based, asynchronous HTTP request/response parser.

![NuGet: Install-Package HttpHelpers.Sources -Pre](https://raw.github.com/gsscoder/httphelpers/master/HttpHelpersNuGet.png)

At glance:
---
```csharp
var stream = new MemoryStream(Encoding.UTF8.GetBytes(
  "GET /gsscoder/httphelpers HTTP/1.1\r\n" +
  "Content-Type: text/html; q=0.9, text/plain\r\n\r\n"));

HttpParser.ParseMessageAsync(stream, (method, uri, version) =>
  {
    Debug.WriteLine(method);
    Debug.WriteLine(uri);
    Debug.WriteLine(version);
  },
  (header, value) => 
    Debug.WriteLine(header + " " + value).Wait();

// When parsing a response, just name heading-line lambda parameters:
// version, code and reason.
// For the body: just read remaining bytes from the stream.
```

Resources:
---
- [Unit Tests](https://github.com/gsscoder/httphelpers/blob/master/src/HttpHelpers.Tests/Unit/HttpParserFixture.cs)
- [Sample web server](https://gist.github.com/gsscoder/4945688) (now included in the solution tree)
- [Owin HTTP Listener](https://github.com/gsscoder/owinhttplistener) (work in progress)

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)

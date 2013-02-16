Http Helpers 0.1.5.0 alfa.
===
Easy to use, one method based, asynchronous HTTP request/response parser. It contains also a synchronous version;
none of versions wraps the other, there are two independent implementations.

![NuGet: Install-Package HttpHelpers.Sources -Pre](https://raw.github.com/gsscoder/httphelpers/master/HttpHelpersNuGet.png)

At glance:
---
```csharp
Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(
  "GET /gsscoder/httphelpers HTTP/1.1\r\n" +
  "Content-Type: text/html; q=0.9, text/plain\r\n\r\n"));

bool result = await HttpParser.ParseMessageAsync(stream, (method, uri, version) =>
  {
    Debug.WriteLine(method);
    Debug.WriteLine(uri);
    Debug.WriteLine(version);
  },
  (header, value) => 
    Debug.WriteLine(header + " " + value);

// When parsing a response, just name heading-line lambda parameters:
// version, code and reason.
// For the body: just read remaining bytes from the stream.
```
News:
---
- Increased tests coverage.

Resources:
---
- [Unit Tests](https://github.com/gsscoder/httphelpers/blob/master/src/HttpHelpers.Tests/Unit)
- [Sample web server](https://gist.github.com/gsscoder/4945688) (now included in the solution tree)
- [Owin HTTP Listener](https://github.com/gsscoder/owinhttplistener) (work in progress)

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)

Http Helpers 0.1.0.1 alfa.
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
For the moment there's nothing more than three extension method to {{System.IO.Stream}}.

```csharp
var stream = "GET /your/web/resource HTTP/1.1\r\n" +
    "Accept-Language: en-us\r\n" +
    "Host: somewhere.com\r\n\r\n".AsStream();

string method, uri, version;
bool result = stream.ParseHttpRequestLine(out method, out uri, out version);

string headerName;
string headerValue;
result = stream.ParseNextHttpHeader(out headerName, out headerValue);

// when parsing a response:
string version;
int? code;
string reason;
result = stream.ParseHttpResponseLine(out version, out code, out reason);
```

Contacts:
---
Giacomo Stelluti Scala
  - gsscoder AT gmail DOT com
  - [Blog](http://gsscoder.blogspot.it)
  - [Twitter](http://twitter.com/gsscoder)

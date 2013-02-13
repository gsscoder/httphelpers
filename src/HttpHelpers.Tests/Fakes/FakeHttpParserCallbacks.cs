﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpHelpers.Tests.Fakes
{
    class FakeHttpParserCallbacks : IHttpParserCallbacks
    {
        internal struct FakeRequestLine
        {
            public string Method;
            public string Uri;
            public string Version;
        }

        public FakeRequestLine RequestLine = new FakeRequestLine();

        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        public byte[] Body;

        public void OnMessageBegin()
        {
        }

        public void OnRequestLine(string method, string uri, string version)
        {
            RequestLine.Method = method;
            RequestLine.Uri = uri;
            RequestLine.Version = version;
        }

        public void OnResponseLine(string version, int? code, string reason)
        {
        }

        public void OnHeaderLine(string name, string value)
        {
            Headers.Add(name, value);
        }

        public void OnBody(ArraySegment<byte> data)
        {
            Body = data.Array.Take(data.Count).ToArray();
        }

        public void OnMessageEnd()
        {
        }
    }
}

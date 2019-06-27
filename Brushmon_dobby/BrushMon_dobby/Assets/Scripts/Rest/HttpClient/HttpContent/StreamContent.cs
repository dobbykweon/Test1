﻿using System;
using System.IO;


public class StreamContent : IHttpContent
{
    private readonly Stream _stream;
    private readonly string _mediaType;

    public ContentReadAction ContentReadAction
    {
        get { return ContentReadAction.Stream; }
    }

    public StreamContent(Stream stream, string mediaType)
    {
        _stream = stream;
        _mediaType = mediaType;
    }

    public long GetContentLength()
    {
        return _stream.Length;
    }

    public string GetContentType()
    {
        return _mediaType;
    }

    public byte[] ReadAsByteArray()
    {
        throw new NotImplementedException();
    }

    public Stream ReadAsStream()
    {
        return _stream;
    }
}

using System.IO;


public interface IHttpContent
{
    ContentReadAction ContentReadAction { get; }

    long GetContentLength();
    string GetContentType();
    byte[] ReadAsByteArray();
    Stream ReadAsStream();
}

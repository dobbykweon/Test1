
public class BrushMonException : System.Exception
{
    private string message;
    private int exceptionCode;

    public BrushMonException(int exceptionCode, string message):base(message) { this.message = message; this.exceptionCode = exceptionCode; }
    public BrushMonException(string message) : base(message) { }
    public BrushMonException(string message, System.Exception inner) : base(message, inner) { }
    protected BrushMonException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

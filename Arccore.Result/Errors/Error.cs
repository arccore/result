public record Error(
    string code, 
    string message)
{

    public override string ToString() => $"{code}: {message}";
}

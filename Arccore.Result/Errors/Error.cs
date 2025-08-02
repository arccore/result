public abstract record Error(
    string code, 
    string message, string Fi)
{

    public override string ToString() => $"{code}: {message}";
}
namespace CineMan.Options;

public class AuthOptions
{
    public const string Auth = "Auth";

    public int MinimumPasswordLength { get; init; }
    public string PasswordSalt { get; init; } = null!;
    public List<string> ExcludePaths { get; init; } = null!;
}
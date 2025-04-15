namespace Jwt.Refresh.Token.Domain.Constants;

public static class TokenStatusConst
{
    public const int Authorized = 201;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int Error = 500;
}
namespace AngularAPIJWT
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken(string username);
    }
}

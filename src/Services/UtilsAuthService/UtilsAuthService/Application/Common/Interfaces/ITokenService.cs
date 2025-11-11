namespace UtilsAuthService.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Guid userId, string email);
    }
}

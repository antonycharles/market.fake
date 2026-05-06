namespace User.Core.Handlers
{
    public interface ITokenHandler
    {
        bool ValidateToken(string token);
    }
}

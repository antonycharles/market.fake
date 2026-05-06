namespace User.Application.Providers
{
    public interface IPasswordProvider
    {
        string HashPassword(string password);
    }
}

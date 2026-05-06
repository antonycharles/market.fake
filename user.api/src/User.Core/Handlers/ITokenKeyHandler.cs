using Microsoft.IdentityModel.Tokens;

namespace User.Core.Handlers
{
    public interface ITokenKeyHandler
    {
        IList<JsonWebKey> GetPublicKeys();
    }
}

using System.Threading.Tasks;

namespace Lingua.ZoomIntegration
{
    public interface IUserService
    {
        Task<UserProfile> GetUserProfile(string accessToken);
    }
}

using System.Threading.Tasks;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public interface IAdminService
    {
        Task<bool> CheckAdminUserCreationAsync();
    }
}

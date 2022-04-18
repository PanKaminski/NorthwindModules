using System.Threading.Tasks;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> CheckAdminUserCreationAsync();
    }
}

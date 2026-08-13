using Sekka.BLL.Common;
using Sekka.BLL.ViewModels.AccountVM;

namespace Sekka.BLL.Interfaces
{
    public interface IAdminService
    {
        Task<Result> RegisterAdminAsync(CreateAdminVM model);

        Task<IEnumerable<AdminVM>> GetAdminsAsync();
        Task<Result> DeleteAdminAsync(string id);
    }
}

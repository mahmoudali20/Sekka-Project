using Sekka.BLL.Common;
using Sekka.BLL.ViewModels.DriverVM;

namespace Sekka.BLL.Interfaces
{
    public interface IDriverService
    {
        Task<Result> CreateDriverAsync(CreateDriverVM model);
        Task<Result<IEnumerable<DriverVM>>> GetAllDriverAsync();
        Task<Result<DriverDetailsVM>> GetDriverDetailsByIdAsync(int id, CancellationToken ct = default);
        Task<Result> DeleteDriverAsync(int id, CancellationToken ct = default);

        Task<Result> UpdateDriverAsync(int id, DriverEditVM model, CancellationToken ct = default);
        Task<Result<DriverEditVM?>> GetForUpdateAsync(int memberId, CancellationToken ct = default);




    }
}

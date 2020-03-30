using System.Threading.Tasks;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.General;

namespace Thandizo.FacilityResources.BLL.Services
{
    public interface IHealthFacilityResourceService
    {
        Task<OutputResponse> Add(HealthFacilityResourceDTO resource);
        Task<OutputResponse> Delete(int facilityResourceId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int facilityResourceId);
        Task<OutputResponse> Update(HealthFacilityResourceDTO resource);
    }
}
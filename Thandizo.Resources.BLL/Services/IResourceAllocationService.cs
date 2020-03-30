using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Resources;

namespace Thandizo.FacilityResources.BLL.Services
{
    public interface IResourceAllocationService
    {
        Task<OutputResponse> Add(ResourceAllocationDTO resourceAllocation);
        Task<OutputResponse> Delete(int resourceAllocationId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int resourceAllocationId);
        Task<OutputResponse> Update(ResourceAllocationDTO resourceAllocation);
    }
}
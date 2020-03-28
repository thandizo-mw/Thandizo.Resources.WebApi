using System.Threading.Tasks;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Resources;

namespace Thandizo.FacilityResources.BLL.Services
{
    public interface IResourceService
    {
        Task<OutputResponse> Add(ResourceDTO resource);
        Task<OutputResponse> Delete(int resourceId);
        Task<OutputResponse> Get();
        Task<OutputResponse> Get(int resourceId);
        Task<OutputResponse> Update(ResourceDTO resource);
    }
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Resources;

namespace Thandizo.FacilityResources.BLL.Services
{
    public class ResourceService : IResourceService
    {
        private readonly thandizoContext _context;

        public ResourceService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int resourceId)
        {
            var resource = await _context.Resources.FirstOrDefaultAsync(x => x.ResourceId.Equals(resourceId));
           
            var mappedResource = new AutoMapperHelper<Resources, ResourceDTO>().MapToObject(resource);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedResource
            };
        }

        public async Task<OutputResponse> Get()
        {
            var resources = await _context.Resources.OrderBy(x => x.ResourceName).ToListAsync();

            var mappedResources = new AutoMapperHelper<Resources, ResourceDTO>().MapToList(resources);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedResources
            };
        }

        public async Task<OutputResponse> Add(ResourceDTO resource)
        {
            var isFound = await _context.Resources.AnyAsync(x => x.ResourceName.ToLower() == resource.ResourceName.ToLower());
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Resource name already exist, duplicates not allowed"
                };
            }

            var mappedResource = new AutoMapperHelper<ResourceDTO, Resources>().MapToObject(resource);
            mappedResource.RowAction = "I";
            mappedResource.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.Resources.AddAsync(mappedResource);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(ResourceDTO resource)
        {
            var resourceToUpdate = await _context.Resources.FirstOrDefaultAsync(x => x.ResourceId.Equals(resource.ResourceId));

            if (resourceToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Resource specified does not exist, update cancelled"
                };
            }

            //update details
            resourceToUpdate.ResourceName = resource.ResourceName;
            resourceToUpdate.RowAction = "U";
            resourceToUpdate.ModifiedBy = resource.CreatedBy;
            resourceToUpdate.DateModified = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.UpdateSuccess
            };
        }

        public async Task<OutputResponse> Delete(int resourceId)
        {
            //check if there are any records associated with the specified resources allocation
            var isFound = await _context.ResourcesAllocation.AnyAsync(x => x.ResourceId.Equals(resourceId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified resource is allocated to patient status, deletion denied"
                };
            }

            var resource = await _context.Resources.FirstOrDefaultAsync(x => x.ResourceId.Equals(resourceId));

            if (resource == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Resource specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}

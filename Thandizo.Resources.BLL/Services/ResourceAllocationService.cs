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
    public class ResourceAllocationService : IResourceAllocationService
    {
        private readonly thandizoContext _context;

        public ResourceAllocationService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int resourceAllocationId)
        {
            var resource = await _context.ResourcesAllocation.FirstOrDefaultAsync(x => x.ResourceAllocationId.Equals(resourceAllocationId));
           
            var mappedResourceAllocation = new AutoMapperHelper<ResourcesAllocation, ResourceAllocationDTO>().MapToObject(resource);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedResourceAllocation
            };
        }

        public async Task<OutputResponse> Get()
        {
            var resourceAllocation = await _context.ResourcesAllocation.OrderBy(x => x.ResourceAllocationId).ToListAsync();

            var mappedResourcesAllocation = new AutoMapperHelper<ResourcesAllocation, ResourceAllocationDTO>().MapToList(resourceAllocation);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedResourcesAllocation
            };
        }

        public async Task<OutputResponse> Add(ResourceAllocationDTO resource)
        {
            
            var mappedResourceAllocation = new AutoMapperHelper<ResourceAllocationDTO, ResourcesAllocation>().MapToObject(resource);
            mappedResourceAllocation.RowAction = "I";
            mappedResourceAllocation.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.ResourcesAllocation.AddAsync(mappedResourceAllocation);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(ResourceAllocationDTO resource)
        {
            var resourceToUpdate = await _context.ResourcesAllocation.FirstOrDefaultAsync(x => x.ResourceAllocationId.Equals(resource.ResourceAllocationId));

            if (resourceToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "ResourceAllocation specified does not exist, update cancelled"
                };
            }

            //update details
            resourceToUpdate.PatientStatusId = resource.PatientStatusId;
            resourceToUpdate.RequiredQuantity = resource.RequiredQuantity;
            resourceToUpdate.ResourceId = resource.ResourceId;
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

        public async Task<OutputResponse> Delete(int resourceAllocationId)
        {
            //check if there are any records associated with the specified resource allocation
            var isFound = await _context.HealthFacilityResources.AnyAsync(x => x.ResourceAllocationId.Equals(resourceAllocationId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified resource is allocated to a health facility resource status, deletion denied"
                };
            }

            var resource = await _context.ResourcesAllocation.FirstOrDefaultAsync(x => x.ResourceAllocationId.Equals(resourceAllocationId));

            if (resource == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Resource allocation specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.ResourcesAllocation.Remove(resource);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}

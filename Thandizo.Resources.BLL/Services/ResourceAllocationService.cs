using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Resources;
using Thandizo.DataModels.Resources.Responses;

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
            var resource = await _context.ResourcesAllocation.Where(x => x.ResourceAllocationId.Equals(resourceAllocationId))
                .Select(x => new ResourceAllocationResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    ModifiedBy = x.ModifiedBy,
                    PatientStatusId = x.PatientStatusId,
                    PatientStatusName = x.PatientStatus.PatientStatusName,
                    RequiredQuantity = x.RequiredQuantity,
                    ResourceAllocationId = x.ResourceAllocationId,
                    ResourceId = x.ResourceId,
                    ResourceName = x.Resource.ResourceName,
                    RowAction = x.RowAction
                }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = resource
            };
        }

        public async Task<OutputResponse> Get()
        {
            var resourceAllocations = await _context.ResourcesAllocation.OrderBy(x => x.ResourceAllocationId)
                .Select(x => new ResourceAllocationResponse
                {
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    ModifiedBy = x.ModifiedBy,
                    PatientStatusId = x.PatientStatusId,
                    PatientStatusName = x.PatientStatus.PatientStatusName,
                    RequiredQuantity = x.RequiredQuantity,
                    ResourceAllocationId = x.ResourceAllocationId,
                    ResourceId = x.ResourceId,
                    ResourceName = x.Resource.ResourceName,
                    RowAction = x.RowAction
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = resourceAllocations
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
                    Message = "Resource allocation specified does not exist, update cancelled"
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
            //check if there are any records associated with the specified record
            var isFound = await _context.HealthFacilityResources.AnyAsync(x => x.ResourceAllocationId.Equals(resourceAllocationId));
            if (isFound)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "The specified resource is allocated to a health facility, deletion denied"
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

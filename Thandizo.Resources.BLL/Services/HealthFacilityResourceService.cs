using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.DataCenters;
using Thandizo.DataModels.DataCenters.Responses;
using Thandizo.DataModels.General;

namespace Thandizo.FacilityResources.BLL.Services
{
    public class HealthFacilityResourceService : IHealthFacilityResourceService
    {
        private readonly thandizoContext _context;

        public HealthFacilityResourceService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> Get(int facilityResourceId)
        {
            var resource = await _context.HealthFacilityResources.Where(x => x.FacilityResourceId.Equals(facilityResourceId))
                .Select(x => new HealthFacilityResourceResponse
                {
                    CenterId = x.CenterId,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    FacilityResourceId = x.FacilityResourceId,
                    ModifiedBy = x.ModifiedBy,
                    PatientStatusName = x.ResourceAllocation.PatientStatus.PatientStatusName,
                    Quantity = x.Quantity,
                    ResourceAllocationId = x.ResourceAllocationId,
                    ResourceName = x.ResourceAllocation.Resource.ResourceName,
                    RowAction = x.RowAction
                }).FirstOrDefaultAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = resource
            };
        }

        public async Task<OutputResponse> GetByFacility(int centerId)
        {
            var resources = await _context.HealthFacilityResources.Where(x => x.CenterId.Equals(centerId))
                .OrderBy(x => x.FacilityResourceId)
                .Select(x => new HealthFacilityResourceResponse
                {
                    CenterId = x.CenterId,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    FacilityResourceId = x.FacilityResourceId,
                    ModifiedBy = x.ModifiedBy,
                    PatientStatusName = x.ResourceAllocation.PatientStatus.PatientStatusName,
                    Quantity = x.Quantity,
                    ResourceAllocationId = x.ResourceAllocationId,
                    ResourceName = x.ResourceAllocation.Resource.ResourceName,
                    RowAction = x.RowAction
                }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = resources
            };
        }

        public async Task<OutputResponse> Add(HealthFacilityResourceDTO resource)
        {

            var mappedHealthFacilityResource = new AutoMapperHelper<HealthFacilityResourceDTO, HealthFacilityResources>().MapToObject(resource);
            mappedHealthFacilityResource.RowAction = "I";
            mappedHealthFacilityResource.DateCreated = DateTime.UtcNow.AddHours(2);

            await _context.HealthFacilityResources.AddAsync(mappedHealthFacilityResource);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.AddNewSuccess
            };
        }

        public async Task<OutputResponse> Update(HealthFacilityResourceDTO resource)
        {
            var resourceToUpdate = await _context.HealthFacilityResources.FirstOrDefaultAsync(x => x.FacilityResourceId.Equals(resource.FacilityResourceId));

            if (resourceToUpdate == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Health facility resource specified does not exist, update cancelled"
                };
            }

            //update details
            resourceToUpdate.CenterId = resource.CenterId;
            resourceToUpdate.ResourceAllocationId = resource.ResourceAllocationId;
            resourceToUpdate.Quantity = resource.Quantity;
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

        public async Task<OutputResponse> Delete(int facilityResourceId)
        {
            var resource = await _context.HealthFacilityResources.FirstOrDefaultAsync(x => x.FacilityResourceId.Equals(facilityResourceId));

            if (resource == null)
            {
                return new OutputResponse
                {
                    IsErrorOccured = true,
                    Message = "Health facility resource specified does not exist, deletion cancelled"
                };
            }

            //deletes the record permanently
            _context.HealthFacilityResources.Remove(resource);
            await _context.SaveChangesAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Message = MessageHelper.DeleteSuccess
            };
        }
    }
}

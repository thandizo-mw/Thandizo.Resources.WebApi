using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.DataMapping;
using Thandizo.ApiExtensions.General;
using Thandizo.DAL.Models;
using Thandizo.DataModels.DataCenters;
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
            var resource = await _context.HealthFacilityResources.FirstOrDefaultAsync(x => x.FacilityResourceId.Equals(facilityResourceId));
           
            var mappedHealthFacilityResource = new AutoMapperHelper<HealthFacilityResources, HealthFacilityResourceDTO>().MapToObject(resource);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedHealthFacilityResource
            };
        }

        public async Task<OutputResponse> Get()
        {
            var resources = await _context.HealthFacilityResources.OrderBy(x => x.FacilityResourceId).ToListAsync();

            var mappedHealthFacilityResources = new AutoMapperHelper<HealthFacilityResources, HealthFacilityResourceDTO>().MapToList(resources);

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = mappedHealthFacilityResources
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

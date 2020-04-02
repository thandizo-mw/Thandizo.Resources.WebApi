using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Thandizo.ApiExtensions.Filters;
using Thandizo.ApiExtensions.General;
using Thandizo.DataModels.DataCenters;
using Thandizo.FacilityResources.BLL.Services;

namespace Thandizo.Core.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthFacilityResourcesController : ControllerBase
    {
        IHealthFacilityResourceService _service;

        public HealthFacilityResourcesController(IHealthFacilityResourceService service)
        {
            _service = service;
        }

        [HttpGet("GetById")]
        [CatchException(MessageHelper.GetItemError)]
        public async Task<IActionResult> GetById([FromQuery] int facilityResourceId)
        {
            var response = await _service.Get(facilityResourceId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpGet("GetByFacility")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetByFacility([FromQuery]int centerId)
        {            
            var response = await _service.GetByFacility(centerId);

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpPost("Add")]
        [ValidateModelState]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]HealthFacilityResourceDTO healthFacilityResource)
        {
            var outputHandler = await _service.Add(healthFacilityResource);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Created("", outputHandler.Message);
        }

        [HttpPut("Update")]
        [ValidateModelState]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]HealthFacilityResourceDTO healthFacilityResource)
        {
            var outputHandler = await _service.Update(healthFacilityResource);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }

        [HttpDelete("Delete")]
        [CatchException(MessageHelper.DeleteError)]
        public async Task<IActionResult> Delete([FromQuery]int facilityResourceId)
        {
            var outputHandler = await _service.Delete(facilityResourceId);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler.Message);
            }

            return Ok(outputHandler.Message);
        }
    }
}
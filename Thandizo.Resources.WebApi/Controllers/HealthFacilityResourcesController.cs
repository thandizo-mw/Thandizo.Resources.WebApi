﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetAll")]
        [CatchException(MessageHelper.GetListError)]
        public async Task<IActionResult> GetAll()
        {            
            var response = await _service.Get();

            if (response.IsErrorOccured)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Result);
        }

        [HttpPost("Add")]
        [CatchException(MessageHelper.AddNewError)]
        public async Task<IActionResult> Add([FromBody]HealthFacilityResourceDTO healthFacilityResource)
        {
            var outputHandler = await _service.Add(healthFacilityResource);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Created("", outputHandler);
        }

        [HttpPut("Update")]
        [CatchException(MessageHelper.UpdateError)]
        public async Task<IActionResult> Update([FromBody]HealthFacilityResourceDTO healthFacilityResource)
        {
            var outputHandler = await _service.Update(healthFacilityResource);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Ok(outputHandler);
        }

        [HttpDelete("Delete")]
        [CatchException(MessageHelper.DeleteError)]
        public async Task<IActionResult> Delete([FromQuery]int facilityResourceId)
        {
            var outputHandler = await _service.Delete(facilityResourceId);
            if (outputHandler.IsErrorOccured)
            {
                return BadRequest(outputHandler);
            }

            return Ok(outputHandler);
        }
    }
}
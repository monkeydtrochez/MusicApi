using System;
using System.Threading;
using System.Threading.Tasks;
using Mashup.Api.Interfaces;
using Mashup.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mashup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MashupController : ControllerBase
    {
        private readonly IMashupService _mashupService;
        public MashupController(IMashupService mashupService)
        {
            _mashupService = mashupService;
        }

        [HttpGet]
        public async Task<ObjectResult> Get([FromQuery]RequestModel model, CancellationToken cancellationToken)
        {
            MashupResultModel resultModel;
            try
            {
                resultModel = await _mashupService.BuildMashupModel(model.MbId, model.LangCode, cancellationToken);
            }
            catch (TaskCanceledException taskCanceledException) when (taskCanceledException.CancellationToken == cancellationToken)
            {
                return BadRequest("The request has been cancelled.");
            }

            return Ok(resultModel);
        }

    }
}

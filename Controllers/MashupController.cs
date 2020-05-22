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
        public async Task<ObjectResult> Get([FromQuery]RequestModel model)
        {
            var resultModel = await _mashupService.BuildMashupModel(model.MbId, model.LangCode);

            return Ok(resultModel);
        }

    }
}

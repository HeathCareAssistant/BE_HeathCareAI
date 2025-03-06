using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.MedicineCabinetModelViews;
using Microsoft.AspNetCore.Mvc;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/medicine-cabinet")]
    [ApiController]
    public class MedicineCabinetController : ControllerBase
    {
        private readonly IMedicineCabinetService _cabinetService;

        public MedicineCabinetController(IMedicineCabinetService cabinetService)
        {
            _cabinetService = cabinetService;
        }

        [HttpPost("create-empty")]
        public async Task<IActionResult> CreateEmptyCabinet(int userId, string cabinetName, string description)
        {
            return Ok(await _cabinetService.CreateCabinetAsync(userId, cabinetName, description));
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateCabinet([FromBody] CreateCabinetWithDrugsRequest request)
        {
            return Ok(await _cabinetService.CreateCabinetWithDrugsAsync(request));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCabinets(int userId)
        {
            return Ok(await _cabinetService.GetUserCabinetsAsync(userId));
        }

        [HttpPut("{cabinetId}")]
        public async Task<IActionResult> UpdateCabinet(int cabinetId, string cabinetName, string description)
        {
            return Ok(await _cabinetService.UpdateCabinetAsync(cabinetId, cabinetName, description));
        }

        [HttpPut("{cabinetId}/update-drugs")]
        public async Task<IActionResult> UpdateCabinetDrugs(int cabinetId, [FromBody] List<string> finalDrugIds)
        {
            var result = await _cabinetService.UpdateCabinetDrugsAsync(cabinetId, finalDrugIds);
            return Ok(new { message = result });
        }

        [HttpDelete("{cabinetId}")]
        public async Task<IActionResult> DeleteCabinet(int cabinetId)
        {
            return Ok(await _cabinetService.DeleteCabinetAsync(cabinetId));
        }

        [HttpGet("{cabinetId}/drugs")]
        public async Task<IActionResult> GetCabinetDrugs(int cabinetId)
        {
            var drugs = await _cabinetService.GetCabinetDrugsAsync(cabinetId);
            return Ok(drugs);
        }
    }
}

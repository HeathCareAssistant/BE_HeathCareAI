﻿using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using HealthyCareAssistant.Service.Service.firebase;
using Microsoft.AspNetCore.Mvc;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/drugs")]
    [ApiController]
    public class DrugController : ControllerBase
    {
        private readonly IDrugService _drugService;
        private readonly IFirebaseStorageService _firebaseStorageService;
        public DrugController(IDrugService drugService, IFirebaseStorageService firebaseStorageService)
        {
            _drugService = drugService;
            _firebaseStorageService = firebaseStorageService;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (drugs, totalElement, totalPage) = await _drugService.GetAllDrugsPaginatedAsync(page, pageSize);

            return Ok(new
            {
                totalElement,
                totalPage,
                currentPage = page,
                pageSize,
                data = drugs
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var drug = await _drugService.GetDrugByIdAsync(id);
            if (drug == null) return NotFound();

            // Increase search count
            await _drugService.IncrementSearchCountAsync(id);

            return Ok(drug);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DrugModelView model)
        {
            var result = await _drugService.CreateDrugAsync(model);
            return Ok(new { message = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _drugService.DeleteDrugAsync(id);
            return result ? Ok(new { message = "Deleted successfully" }) : NotFound();
        }

        [HttpGet("search/name")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            var drugs = await _drugService.SearchByNameAsync(name);
            return Ok(drugs);
        }

        [HttpGet("search/ingredient")]
        public async Task<IActionResult> SearchByIngredient([FromQuery] string ingredient)
        {
            var drugs = await _drugService.SearchByIngredientAsync(ingredient);
            return Ok(drugs);
        }

        [HttpGet("filter/company")]
        public async Task<IActionResult> FilterByCompany([FromQuery] string companyName)
        {
            var drugs = await _drugService.FilterByCompanyAsync(companyName);
            return Ok(drugs);
        }

        [HttpGet("filter/category")]
        public async Task<IActionResult> FilterByCategory([FromQuery] string category, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (drugs, totalElement, totalPage) = await _drugService.FilterByCategoryAsync(category, page, pageSize);

            return Ok(new
            {
                totalElement,
                totalPage,
                currentPage = page,
                pageSize,
                data = drugs
            });
        }

        [HttpGet("related/ingredient/{id}")]
        public async Task<IActionResult> GetRelatedByIngredient(string id)
        {
            var drugs = await _drugService.GetRelatedByIngredientAsync(id);
            return Ok(drugs);
        }

        [HttpGet("related/company")]
        public async Task<IActionResult> GetRelatedByCompany([FromQuery] string id)
        {
            var drugs = await _drugService.GetRelatedByCompanyAsync(id);
            return Ok(drugs);
        }

        [HttpGet("top-searched")]
        public async Task<IActionResult> GetTopSearchedDrugs()
        {
            var drugs = await _drugService.GetTopSearchedDrugsAsync();
            return Ok(drugs);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateDrugModelView model)
        {
            var result = await _drugService.UpdateDrugAsync(id, model);
            return result ? Ok(new { message = "Updated successfully" }) : NotFound();
        }


        [HttpGet("top-new-registered")]
        public async Task<IActionResult> GetTopNewRegisteredDrugs()
        {
            var drugs = await _drugService.GetTopNewRegisteredDrugsAsync();
            return Ok(drugs);
        }

        [HttpGet("top-withdrawn")]
        public async Task<IActionResult> GetTopWithdrawnDrugs()
        {
            var drugs = await _drugService.GetTopWithdrawnDrugsAsync();
            return Ok(drugs);
        }

        [HttpGet("top-companies")]
        public async Task<IActionResult> GetTopCompaniesByDrugs()
        {
            var companies = await _drugService.GetTopCompaniesByDrugsAsync();
            return Ok(companies);
        }
        [HttpPost("{drugId}/image/upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDrugImage(string drugId, [FromForm] DrugImageUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest(new { message = "Invalid file" });

            var result = await _firebaseStorageService.UploadDrugImageAsync(drugId, model.File);
            return Ok(new { imageUrl = result });
        }
        [HttpPut("{drugId}/image/update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDrugImage(string drugId, [FromForm] DrugImageUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest(new { message = "Invalid file" });

            var result = await _firebaseStorageService.UpdateDrugImageAsync(drugId, model.File);
            return Ok(new { imageUrl = result });
        }


        [HttpGet("{drugId}/image")]
        public async Task<IActionResult> GetDrugImageUrl(string drugId)
        {
            var result = await _firebaseStorageService.GetDrugImageUrlAsync(drugId);
            if (result.StartsWith("DrugID"))
                return NotFound(new { message = result });

            return Ok(new { imageUrl = result });
        }
        [HttpGet("{drugId}/images")]
        public async Task<IActionResult> GetAllDrugImages(string drugId)
        {
            var result = await _firebaseStorageService.GetAllDrugImagesAsync(drugId);
            return Ok(new { images = result });
        }
        [HttpDelete("{drugId}/image")]
        public async Task<IActionResult> DeleteDrugImage(string drugId)
        {
            var result = await _firebaseStorageService.DeleteDrugImageAsync(drugId);
            if (result.StartsWith("DrugID"))
                return NotFound(new { message = result }); // Trả về lỗi nếu không có ảnh

            return Ok(new { message = result });
        }

        [HttpPut("{drugId}/pdf/update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDrugPdf(string drugId, [FromForm] DrugPdfUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest(new { message = "Invalid PDF file" });

            var result = await _firebaseStorageService.UpdateDrugPdfAsync(drugId, model.File);
            return Ok(new { pdfUrl = result });
        }



        [HttpPost("{drugId}/pdf/upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDrugPdf(string drugId, [FromForm] DrugPdfUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest(new { message = "Invalid PDF file" });

            var result = await _firebaseStorageService.UploadDrugPdfAsync(drugId, model.File);
            return Ok(new { pdfUrl = result });
        }

        [HttpGet("{drugId}/pdf")]
        public async Task<IActionResult> GetDrugPdfUrl(string drugId)
        {
            var result = await _firebaseStorageService.GetDrugPdfUrlAsync(drugId);
            if (result.StartsWith("DrugID"))
                return NotFound(new { message = result });

            return Ok(new { pdfUrl = result });
        }
        [HttpDelete("{drugId}/pdf")]
        public async Task<IActionResult> DeleteDrugPdf(string drugId)
        {
            var result = await _firebaseStorageService.DeleteDrugPdfAsync(drugId);
            if (result.StartsWith("DrugID"))
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        [HttpGet("filter/group")]
        public async Task<IActionResult> FilterByDrugGroup([FromQuery] string group, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (drugs, totalElement, totalPage) = await _drugService.FilterByDrugGroupAsync(group, page, pageSize);

            return Ok(new
            {
                totalElement,
                totalPage,
                currentPage = page,
                pageSize,
                data = drugs
            });
        }


    }
}


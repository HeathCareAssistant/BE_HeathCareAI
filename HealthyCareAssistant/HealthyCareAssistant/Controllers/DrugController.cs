﻿using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using Microsoft.AspNetCore.Mvc;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/drugs")]
    [ApiController]
    public class DrugController : ControllerBase
    {
        private readonly IDrugService _drugService;

        public DrugController(IDrugService drugService)
        {
            _drugService = drugService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var drugs = await _drugService.GetAllDrugsAsync();
            return Ok(drugs);
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
        public async Task<IActionResult> FilterByCategory([FromQuery] string category)
        {
            var drugs = await _drugService.FilterByCategoryAsync(category);
            return Ok(drugs);
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
        public async Task<IActionResult> Update(string id, [FromBody] DrugModelView model)
        {
            var result = await _drugService.UpdateDrugAsync(id, model);
            return result ? Ok(new { message = "Updated successfully" }) : NotFound();
        }
    }
}

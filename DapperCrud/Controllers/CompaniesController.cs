using DapperCrud.Contracts;
using DapperCrud.Dto;
using DapperCrud.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DapperCrud.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository) => _companyRepository = companyRepository;

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _companyRepository.GetCompanies();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany([FromRoute]int id)
        {
            try
            {
                var company = await _companyRepository.GetCompany(id);
                if (company == null)
                    return NotFound();

                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto createCompanyDto)
        {
            try
            {
                var createdCompany = await _companyRepository.CreateCompany(createCompanyDto);
                return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompany([FromRoute]int id, [FromBody]UpdateCompanyDto updateCompanyDto)
        {
            try
            {
                var dbCompany = await _companyRepository.GetCompany(id);
                if (dbCompany == null)
                    return NotFound();

                await _companyRepository.UpdateCompany(id, updateCompanyDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompany([FromRoute] int id)
        {
            try
            {
                var dbCompany = await _companyRepository.GetCompany(id);
                if (dbCompany == null)
                    return NotFound();

                await _companyRepository.DeleteCompany(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee([FromRoute]int id)
        {
            try
            {
                var company = await _companyRepository.GetCompanyByEmployeeId(id);
                if (company == null)
                    return NotFound();

                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetCompanyEmployeesMultipleResult(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompanyEmployeesMultipleResults(id);
                if (company == null)
                    return NotFound();

                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GetCompaniesEmployeesMultipleMapping()
        {
            try
            {
                var company = await _companyRepository.GetCompaniesEmployeesMultipleMapping();

                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> CreateCompany(List<CreateCompanyDto> companies)
        {
            try
            {
                await _companyRepository.CreateMultipleCompanies(companies);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}

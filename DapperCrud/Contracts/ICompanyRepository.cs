using DapperCrud.Dto;
using DapperCrud.Entities;

namespace DapperCrud.Contracts
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetCompanies();
        public Task<Company> GetCompany(int id);
        public Task<Company> CreateCompany(CreateCompanyDto createCompanyDto);
        public Task UpdateCompany(int id, UpdateCompanyDto updateCompanyDto);
        public Task DeleteCompany(int id);
        public Task<Company> GetCompanyByEmployeeId(int id);

        public Task<Company> GetCompanyEmployeesMultipleResults(int id);
        public Task<List<Company>> GetCompaniesEmployeesMultipleMapping();
        public Task CreateMultipleCompanies(List<CreateCompanyDto> companies);

    }
}

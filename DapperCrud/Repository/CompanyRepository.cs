using Dapper;
using DapperCrud.Contracts;
using DapperCrud.Dto;
using DapperCrud.Entities;
using System.Data;

namespace DapperCrud.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context) => _context = context;

        //public async Task CreateCompany(CreateCompanyDto createCompanyDto)
        public async Task<Company> CreateCompany(CreateCompanyDto createCompanyDto)
        {
            //var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)" +
                        "SELECT CAST(SCOPE_IDENTITY() as int)";

            var parameters = new DynamicParameters();

            parameters.Add("Name", createCompanyDto.Name, DbType.String);
            parameters.Add("Address", createCompanyDto.Address, DbType.String);
            parameters.Add("Country", createCompanyDto.Country, DbType.String);

            using (var conn = _context.CreateConnection())
            {
                //await conn.ExecuteAsync(query, parameters);

                var id = await conn.QuerySingleAsync<int>(query, parameters);

                var createdCompany = new Company
                {
                    Id = id,
                    Name = createCompanyDto.Name,
                    Address = createCompanyDto.Address,
                    Country = createCompanyDto.Country
                };


                return createdCompany;
            }
        }

        public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Companies WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            var q = "select * from Companies";
            using (var conn = _context.CreateConnection())
            {
                var comps = await conn.QueryAsync<Company>(q);
                return comps.ToList();
            }
        }

        public async Task<Company> GetCompany(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });

                return company;
            }
        }

        public async Task<Company> GetCompanyByEmployeeId(int id)
        {
            var spName = "SP_GET_COMPANY_BY_EMPLOYEE_ID";
            var parameters = new DynamicParameters();
            parameters.Add("Id",id, DbType.Int32, ParameterDirection.Input);
            using (var conn = _context.CreateConnection())
            {
                var comp = await conn.QueryFirstOrDefaultAsync<Company>(spName, parameters, commandType: CommandType.StoredProcedure);
                return comp;
            }
        }

        public async Task<Company> GetCompanyEmployeesMultipleResults(int id)
        {
            var query = "SELECT * FROM COMPANIES WHERE ID = @Id;" + 
                "SELECT * FROM EMPLOYEES WHERE CompanyId = @Id";

            using (var conn = _context.CreateConnection())
            using (var multi = await conn.QueryMultipleAsync(query, new {id}))
            {
                var comp = await multi.ReadSingleOrDefaultAsync<Company>();
                if (comp is not null)
                    comp.Employees = (await multi.ReadAsync<Employee>()).ToList();

                return comp;
            }
        }

        public async Task UpdateCompany(int id, UpdateCompanyDto updateCompanyDto)
        {
            var query = "UPDATE Companies SET Name = @Name, Address = @Address, Country = @Country WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Name", updateCompanyDto.Name, DbType.String);
            parameters.Add("Address", updateCompanyDto.Address, DbType.String);
            parameters.Add("Country", updateCompanyDto.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<Company>> GetCompaniesEmployeesMultipleMapping()
        {
            var query = "SELECT * FROM Companies c JOIN Employees e ON c.Id = e.CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var companyDict = new Dictionary<int, Company>();

                var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (company, employee) =>
                    {
                        if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                        {
                            currentCompany = company;
                            companyDict.Add(currentCompany.Id, currentCompany);
                        }

                        currentCompany.Employees.Add(employee);
                        return currentCompany;
                    }
                );

                return companies.Distinct().ToList();
            }
        }

        public async Task CreateMultipleCompanies(List<CreateCompanyDto> companies)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var company in companies)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", company.Name, DbType.String);
                        parameters.Add("Address", company.Address, DbType.String);
                        parameters.Add("Country", company.Country, DbType.String);

                        await connection.ExecuteAsync(query, parameters, transaction: transaction);
                        //throw new Exception();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}

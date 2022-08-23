using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface ICompanyService
    {
        Company GetById(int Id);

        void Create(Company company);

        void Destroy(Company company);

        IEnumerable<Company> GetAll();

        bool Exists(string companyName);

        void Update(Company company);
    }
}

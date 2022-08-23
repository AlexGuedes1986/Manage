using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class CompanyService : ICompanyService
    {
        BioTechContext _db;
        public CompanyService(BioTechContext db)
        {
            _db = db;
        }

        public void Create(Company company)
        {
            if (Exists(company.Name))
                throw new Exception($"Company with name '{company.Name}' already exists");

            _db.Company.Add(company);
            _db.SaveChanges();
        }

        public void Destroy(Company company)
        {
            _db.Company.Remove(company);
            _db.SaveChanges();
        }

        public Company GetById(int Id)
        {
            return _db.Company.Find(Id);
        }

        public IEnumerable<Company> GetAll()
        {
            return _db.Company.Where(c => !String.IsNullOrEmpty(c.Name)).ToList();
        }

        public bool Exists(string companyName)
        {
            return _db.Company.FirstOrDefault(c => c.Name == companyName) != null;
        }

        public void Update(Company company)
        {
            _db.Company.Update(company);
            _db.SaveChanges();
        }
    }
}

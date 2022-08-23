//using BioTech.Data;
using BioTech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class BillingRoleService : IBillingRoleService
    {
        BioTechContext _db;
        public BillingRoleService(BioTechContext db)
        {
            _db = db;
        }

        public IEnumerable<BillingRole> GetAll()
        {
            return _db.BillingRole.ToList();
        }

        public IEnumerable<SelectListItem> GetBillingRolesSelectListItems()
        {
            List<SelectListItem> billingRoleList = new List<SelectListItem>();        
            billingRoleList.AddRange(_db.BillingRole.ToList().Select(i => new SelectListItem() { Text = i.Name, Value = i.Id.ToString() }));
            return billingRoleList;
        }

        public BillingRole GetBillingRoleById(int billingRoleId)
        {
            BillingRole billingRole = new BillingRole();
            billingRole = _db.BillingRole.FirstOrDefault(br => br.Id == billingRoleId);
            return billingRole;
        }
    }
}

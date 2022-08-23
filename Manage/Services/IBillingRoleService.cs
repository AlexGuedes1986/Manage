using BioTech.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IBillingRoleService
    {
        IEnumerable<BillingRole> GetAll();
        IEnumerable<SelectListItem> GetBillingRolesSelectListItems();
        BillingRole GetBillingRoleById(int billingRoleId);
    }
}

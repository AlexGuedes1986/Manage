using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface IPartialPaymentService
    {
        void Destroy(PartialPayment partialPayment);
    }
}

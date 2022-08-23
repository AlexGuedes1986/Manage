﻿using BioTech.Models;
using Manage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Services
{
    public class ContactClubService : IContactClubService
    {
        BioTechContext _db;
        public ContactClubService(BioTechContext db)
        {
            _db = db;
        }
        public IEnumerable<ContactClub> GetAll()
        {
            return _db.ContactClub;          
        }
    }
}
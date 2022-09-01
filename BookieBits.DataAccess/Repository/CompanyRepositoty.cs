using BookieBits.DataAccess.Repository.IRepository;
using BookieBits.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.DataAccess.Repository
{
    public class CompanyRepositoty : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepositoty(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company obj)
        {
           _db.Companies.Update(obj);
        }
    }
}

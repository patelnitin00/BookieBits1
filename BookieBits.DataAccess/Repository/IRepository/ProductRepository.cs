using BookieBits.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.DataAccess.Repository.IRepository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db; 

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
           //_db.Products.Update(obj);
           //here we do not want to update whole 
            var objFromDB = _db.Products.FirstOrDefault(x=>x.ID == obj.ID);
            if (objFromDB != null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.Description = obj.Description;
                objFromDB.Price = obj.Price;
                objFromDB.ListPrice = obj.ListPrice;
                objFromDB.Price50 = obj.Price50;
                objFromDB.Price100 = obj.Price100;
                objFromDB.CategoryID = obj.CategoryID;
                objFromDB.Author = obj.Author;
                objFromDB.CoverTypeID = obj.CoverTypeID;

                if(obj.ImageUrl != null)
                {
                    objFromDB.ImageUrl = obj.ImageUrl;
                }

            }
        }
    }
}

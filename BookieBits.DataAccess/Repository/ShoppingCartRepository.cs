using BookieBits.DataAccess.Repository.IRepository;
using BookieBits.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepositoty
    {
        private readonly ApplicationDbContext _db;
       
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db= db;
        }

        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }

       
       
    }
}

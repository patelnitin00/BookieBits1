
using BookieBits.DataAccess.Repository.IRepository;
using BookieBits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookieBitsWeb.Controllers;
[Area("Customer")]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        //what we need here
        //we need product, categoty and cover type

        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "category,coverType");


        return View(productList);
    }


    public IActionResult Details(int productID)
    {
        //Product product = _unitOfWork.Product.GetFirstOrDefault(x => x.ID == id);

        //in product we need number of count that are needed for shopping cart
        //so we need to add one count variable 
        //so we will create a viewModel as just product model is not sufficient
        //and once use adds that we will add to shopping cart
        //so we will make shopping cart viewModel and add count property to it
        //so now we will use shopping cart viewModel instead of product

        ShoppingCart shoppingCart = new()
        {
            Count = 1,
            ProductID = productID,
            Product = _unitOfWork.Product.GetFirstOrDefault(x => x.ID == productID, includeProperties: "category,coverType")
        };

        return View(shoppingCart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        //how to get UserID - using claims identity
        var claimsIdentity =(ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserID = claim.Value;

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserID == claim.Value && u.ProductID == shoppingCart.ProductID);

        if(cartFromDb == null)
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }
        else
        {
            //we will add method to update count in shopCartReposit..
            _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
        }

        
        _unitOfWork.Save();

        //return RedirectToAction("Index");
        //we can rewrite as
        return RedirectToAction(nameof(Index));
    }



    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

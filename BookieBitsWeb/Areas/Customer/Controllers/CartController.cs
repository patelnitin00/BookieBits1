using BookieBits.DataAccess.Repository;
using BookieBits.DataAccess.Repository.IRepository;
using BookieBits.Models;
using BookieBits.Models.ViewModel;
using BookieBits.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookieBitsWeb.Areas.Customer.Controllers
{
[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    //public int OrderTotal { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        //get identity bu claims
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartVM = new ShoppingCartVM()
        {
            //get list for particular user
            //for this we will  modify getall to add filter for 
            //particular user
            ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claims.Value,
            includeProperties: "Product"),
            OrderHeader = new() //we have order header in VM
        };

        //we need to invoke the below function for each product
        foreach (var cart in ShoppingCartVM.ListCart)
        {
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                            cart.Product.Price50, cart.Product.Price100);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartVM);
    }


    public IActionResult plus(int cartId)
    {
        //1st we will get cart
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);

        _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult minus(int cartId)
    {
        //1st we will get cart
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);

        if (cart.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Remove(cart);
        }
        else
        {
            _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
        }


        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult remove(int cartId)
    {
        //1st we will get cart
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);

        _unitOfWork.ShoppingCart.Remove(cart);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Summary()
    {
        //get identity bu claims
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartVM = new ShoppingCartVM()
        {
            //get list for particular user
            //for this we will  modify getall to add filter for 
            //particular user
            ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claims.Value,
            includeProperties: "Product"),
            OrderHeader = new()
        };
        //we have some extra properties like address, phone etc.
        //these things we can load through application user

        ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claims.Value);
        //now base on this we can populate all the properties in OrderHeader
        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;



        //we need to invoke the below function for each product
        foreach (var cart in ShoppingCartVM.ListCart)
        {
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                            cart.Product.Price50, cart.Product.Price100);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartVM);

    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]
    public IActionResult SummaryPOST()
    {
        //get identity bu claims
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claims.Value,
            includeProperties: "Product");

        //add order header value
        //initially order and payment status is pending
        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
        ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        ShoppingCartVM.OrderHeader.ApplicationUserID = claims.Value;


        //we need to invoke the below function for each product
        foreach (var cart in ShoppingCartVM.ListCart)
        {
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                            cart.Product.Price50, cart.Product.Price100);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        //now we need to create order details
        foreach (var cart in ShoppingCartVM.ListCart)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = cart.ProductID,
                //now we need orderId in order detial
                //we will get from order header as we have already saved it above
                OrderId = ShoppingCartVM.OrderHeader.Id,
                Price = cart.Price,
                Count = cart.Count
            };
            //the above Order detail will iterate for every item in shopping cart
            //so we will add it every time to the database
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }


        //-- Stripe Code --//


        var domain = "https://localhost:44373/"; //later we will as to IHostEnvironment
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
            CancelUrl = domain+$"customer/cart/index",
        };

            foreach(var item in ShoppingCartVM.ListCart)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title, //we can add more like details etc.
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

        var service = new SessionService();
        Session session = service.Create(options);

            //before we redirect we will get paymentIntentId and sessionID
            //we need to get it with order header for orderconfirmation ckeck
            //ShoppingCartVM.OrderHeader.SessionId = session.Id;
            //ShoppingCartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            //more proper way for avove 2 line is to create method
            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);


        //-- Sripe Code - Ends --//
   

    }


        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            //reason to retrive order header - check stripe status and confirm the payment is made or not
            //we have 2 properties of stripe in our header

            //we will create new session service
            //we will get existing session here
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            //check stripe status - to make sure payment is actually done
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.OrderStatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            //now everything is completed with order
            //so we will clear the shopping cart
            //but 1st we will get our shopping cart
            List<ShoppingCart> shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserID == orderHeader.ApplicationUserID).ToList();    

            //after this we need to clear our shopping cart by passing list to remove
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartList);
            _unitOfWork.Save();

            return View(id);

        }






        //--- Utility method for this controller---//
        private double GetPriceBasedOnQuantity(double quantity, double price,
        double price50, double price100)
    {
        if (quantity <= 50)
        {
            return price;
        }
        else
        {
            if (quantity <= 100)
            {
                return price50;
            }

            return price100;

        }
    }

    //---Utility Methods - END ---//


}
}

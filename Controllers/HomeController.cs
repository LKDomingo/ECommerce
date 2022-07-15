using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For Session
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers;

public class HomeController : Controller
{
    private MyContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        /////////////// Auto Login
        // User? loggedUser = _context.Users.FirstOrDefault(a => a.Email == "l@d.com");
        // HttpContext.Session.SetInt32("User", loggedUser.UserId);
        // return RedirectToAction("Dashboard");
        //////////////////

        return View();
    }
    ////////////////Register User/////////////////
    [HttpPost("user/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(a => a.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");
                return View("Index");
            }
            // Custom Password REGEX
            // if ((!newUser.Password.Any(char.IsDigit)) || !newUser.Password.Any(char.IsLetter) || !newUser.Password.Any(c => !Char.IsLetterOrDigit(c)))
            // {
            //     System.Console.WriteLine("does not contains numbers and letters");
            //     System.Console.WriteLine("no found special character");
            //     ModelState.AddModelError("Password", "*Password must contain at least 1 number, 1 letter, and a special character!");
            //     return View("Index");
            // }

            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);


            _context.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("User", newUser.UserId);
            return RedirectToAction("Dashboard");
        }
        else
        {
            return View("Index");
        }

    }
    /////////////////Login//////////////////////
    [HttpPost("user/login")]
    public IActionResult Login(LogUser loginUser)
    {
        if (ModelState.IsValid)
        {
            // Grab the user from the Db
            User? userInDb = _context.Users.FirstOrDefault(a => a.Email == loginUser.LogEmail);

            // If user is not found in Db
            if (userInDb == null)
            {
                ModelState.AddModelError("LogEmail", "Invalid login");
                return View("Index");
            }

            // Hash the LogUser password and check with the hashed password in the Db
            PasswordHasher<LogUser> Hasher = new PasswordHasher<LogUser>();
            var result = Hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LogPassword);

            // if result == 0, passwords did not match
            if (result == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid login");
                return View("Index");
            }

            HttpContext.Session.SetInt32("User", userInDb.UserId);
            return RedirectToAction("Dashboard");
        }
        return View("Index");
    }
    ////////////////Logout///////////////
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect("/");
    }
    ///////////////Dashboard///////////////////////
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }

        ViewBag.FourProducts = _context.Products.OrderByDescending(a => a.CreatedAt).Take(4).ToList();
        ViewBag.RecentMembers = _context.Users.OrderByDescending(a => a.CreatedAt).Take(3).ToList();
        ViewBag.User = _context.Users.Include(a => a.OrderItems).ThenInclude(a => a.Product).FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User"));
        return View();
    }
    ////////////////////Products///////////////////
    [HttpGet("products")]
    public IActionResult Products()
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }

        ViewBag.AllProducts = _context.Products.ToList();
        ViewBag.User = _context.Users.Include(a => a.OrderItems).ThenInclude(a => a.Product).FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User"));
        return View("Products");
    }
    //////////////////Add Product//////////////////
    [HttpPost("add/product")]
    public IActionResult AddProduct(Product newProduct)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        
        if (ModelState.IsValid)
        {
            if (_context.Products.Any(a => a.Name == newProduct.Name))
            {
                ModelState.AddModelError("Name", "Product ame already in use!");
            }
            _context.Add(newProduct);
            _context.SaveChanges();
            return Redirect("/products");
        }
        else
        {
            return View("Products");
        }
    }
    //////////////////Add Product to Order from Dashboard////////////////////
    [HttpGet("dashboard/AddToCart/{productId}")]
    public IActionResult AddToCart(int productId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        Cart newCartItem = new Cart();
        newCartItem.ProductId = productId;
        newCartItem.UserId = (int)HttpContext.Session.GetInt32("User");

        _context.Carts.Add(newCartItem);
        _context.SaveChanges();
        return Redirect("/dashboard");
    }
    //////////////////Add Product to Order from Products////////////////////
    [HttpGet("products/AddToCart/{productId}")]
    public IActionResult AddToCartProducts(int productId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        
        Cart newCartItem = new Cart();
        newCartItem.ProductId = productId;
        newCartItem.UserId = (int)HttpContext.Session.GetInt32("User");

        _context.Carts.Add(newCartItem);
        _context.SaveChanges();
        return Redirect("/products");
    }
    //////////////////Add Product to Order from OneProduct////////////////////
    [HttpGet("product/AddToCart/{productId}")]
    public IActionResult AddToCartProduct(int productId)
    {
        System.Console.WriteLine("Entered AddToCartProduct");
        System.Console.WriteLine("(((((((((((((((((((((((((");
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        
        Cart newCartItem = new Cart();
        newCartItem.ProductId = productId;
        newCartItem.UserId = (int)HttpContext.Session.GetInt32("User");

        _context.Carts.Add(newCartItem);
        _context.SaveChanges();
        System.Console.WriteLine("HERE");
        System.Console.WriteLine("=========================");
        return Redirect($"/product/{productId}");
    }
    ////////////////Remove Product from Cart from Dashboard////////
    [HttpGet("dashboard/RemoveFromCart/{productId}")]
    public IActionResult RemoveFromCart(int productId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }

        Cart? cartItemToDelete = _context.Carts.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User") && a.ProductId == productId);
        _context.Carts.Remove(cartItemToDelete);
        _context.SaveChanges();
        return Redirect("/dashboard");
    }
    ////////////////Remove Product from Cart from Products////////
    [HttpGet("products/RemoveFromCart/{productId}")]
    public IActionResult RemoveFromCartProducts(int productId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        
        Cart? cartItemToDelete = _context.Carts.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User") && a.ProductId == productId);
        _context.Carts.Remove(cartItemToDelete);
        _context.SaveChanges();
        return Redirect("/products");
    }
    ////////////////Remove Product from Cart from Cart////////
    [HttpGet("Cart/RemoveFromCart/{productId}")]
    public IActionResult RemoveFromCartCart(int productId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        
        Cart? cartItemToDelete = _context.Carts.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User") && a.ProductId == productId);
        _context.Carts.Remove(cartItemToDelete);
        _context.SaveChanges();
        return Redirect($"/Cart/{HttpContext.Session.GetInt32("User")}");
    }
    ////////////////Remove Product from Cart from OneProduct////////
    [HttpGet("product/RemoveFromCart/{productId}")]
    public IActionResult RemoveFromCartProduct(int productId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        
        Cart? cartItemToDelete = _context.Carts.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User") && a.ProductId == productId);
        _context.Carts.Remove(cartItemToDelete);
        _context.SaveChanges();
        return Redirect($"/product/{productId}");
    }
    ///////////////Cart/////////////////
    [HttpGet("Cart/{userId}")]
    public IActionResult Cart(int userId)
    {
        System.Console.WriteLine("entered cart");
        System.Console.WriteLine("777777777777777");
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }

        ViewBag.User = _context.Users.Include(a => a.OrderItems).ThenInclude(a => a.Product).FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User"));
        System.Console.WriteLine("exiting cart");
        System.Console.WriteLine("777777777777777");
        return View();
    }
    ////////////////One Product///////////////
    [HttpGet("product/{productId}")]
    public IActionResult OneProduct(int productId)
    {
        System.Console.WriteLine("entered one product");
        System.Console.WriteLine("******************");
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        
        Product? OneProduct = _context.Products.FirstOrDefault(a => a.ProductId == productId);
        ViewBag.OneProduct = _context.Products.FirstOrDefault(a => a.ProductId == productId);
        ViewBag.User = _context.Users.Include(a => a.OrderItems).ThenInclude(a => a.Product).FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("User"));
        System.Console.WriteLine("rendering one product");
        System.Console.WriteLine("******************");
        return View("OneProduct", OneProduct);
    }
    ///////////Update Product///////////////
    [HttpPost("/update/{productId}")]
    public IActionResult UpdateProduct(Product UpdatedProduct, int productId)
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        if ( ModelState.IsValid )
        {

        Product? ProductToUpdate = _context.Products.FirstOrDefault(a => a.ProductId == UpdatedProduct.ProductId);
        ProductToUpdate.Name = UpdatedProduct.Name;
        ProductToUpdate.ImageURL = UpdatedProduct.ImageURL;
        ProductToUpdate.Description = UpdatedProduct.Description;
        ProductToUpdate.Price = UpdatedProduct.Price;
        ProductToUpdate.Quantity = UpdatedProduct.Quantity;
        ProductToUpdate.UpdatedAt = DateTime.Now;
        _context.SaveChanges();
        return Redirect($"/product/{productId}");
        }
        else
        {
            return View("OneProduct");
        }
    }
    ///////////////Members////////////////
    [HttpGet("Members")]
    public IActionResult Members()
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        ViewBag.AllUsers = _context.Users.Include(a => a.OrderItems).ToList();
        return View();
    }
    //////////////Submit Cart/////////////
    [HttpGet("/SubmitCart")]
    public IActionResult SubmitCart()
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        List<Cart> CartToDelete = _context.Carts.Where(a => a.UserId == HttpContext.Session.GetInt32("User")).ToList();
        foreach(var i in CartToDelete)
        {
        Product? ProductToUpdate = _context.Products.FirstOrDefault(a => a.ProductId == i.ProductId);
        ProductToUpdate.Quantity -= 1;
        _context.Carts.Remove(i);
        }
        _context.SaveChanges();
        return Redirect($"/Cart/{HttpContext.Session.GetInt32("User")}");
    }
    ////////////Settings////////////////
    [HttpGet("Settings")]
    public IActionResult Settings()
    {
        if (HttpContext.Session.GetInt32("User") == null)
        {
            ModelState.AddModelError("LogEmail", "Please login to continue to site");
            return View("Index");
        }
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


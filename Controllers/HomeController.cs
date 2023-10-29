// Using statements
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InicioSesion.Models;
using Microsoft.AspNetCore.Identity;
namespace InicioSesion.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // Add a private variable of type MyContext (or whatever you named your context file)
    private MyContext _context;
    // Here we can "inject" our context service into the constructor 
    // The "logger" was something that was already in our code, we're just adding around it   
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        // When our HomeController is instantiated, it will fill in _context with context
        // Remember that when context is initialized, it brings in everything we need from DbContext
        // which comes from Entity Framework Core
        _context = context;
    }
    [HttpGet("")]
   
    public IActionResult Index()
    {
        // Now any time we want to access our database we use _context   
        return View("Index");
    }
    [HttpGet]
    [Route("success")]
    [SessionCheck]
    public IActionResult Success()
    {
        // Now any time we want to access our database we use _context   
        return View("Success");
    }


    [HttpPost]
    [Route("procesa/login")]
    public IActionResult Login(Login userSubmission)
    {
        if (ModelState.IsValid)
        {
            // If initial ModelState is valid, query for a user with the provided email        
            Usuario? userInDb = _context.Usuarios.FirstOrDefault(u => u.Email == userSubmission.Email);
            // If no user exists with the provided email        
            if (userInDb == null)
            {
                // Add an error to ModelState and return to View!            
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }

            PasswordHasher<Login> hasher = new PasswordHasher<Login>();
            // Verify provided password against hash stored in db        
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);                                    // Result can be compared to 0 for failure        
            if (result == 0)
            {
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");

            }
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return RedirectToAction("Success");
        }
        else
        {
            return View("Index");
        }
    }


 [HttpGet]
    [Route("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return View("Index");
    }
    //POST

    [HttpPost]

    [Route("user/Registrar")]
    public IActionResult UserRegistrar(Usuario user)
    {



        // si todos los datos fueron validados
        if (ModelState.IsValid)
        {
            PasswordHasher<Usuario> Hasher = new PasswordHasher<Usuario>();
            user.Password = Hasher.HashPassword(user,user.Password);

            _context.Usuarios.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Success");
        };
        return View("Index");
    }

    public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?

        string? email = context.HttpContext.Session.GetString("Email");

        // Check to see if we got back null
        if(email == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "", null);
        }
    }
}

}

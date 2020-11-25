using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Design.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Design.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]        
        public ActionResult Register(IFormCollection form)
        {
            User newUser = new User();
            newUser.UserName = form["UserName"];
            //Hacer cifrado:
            newUser.Password = form["Password"];
            newUser._id = Guid.NewGuid().ToString();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62573/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var postTask = client.PostAsJsonAsync<User>("userValidation", newUser);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    ViewBag.UserExists = "A user with this name already exists!";
                    return View("../Home/Create_Account");
                }                
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62573/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var postTask = client.PostAsJsonAsync<User>("addUser", newUser);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("thisUser", JsonSerializer.Serialize(newUser));
                    return View("../Home/Main_Screen");
                }
                else
                {
                    return StatusCode(500);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Login(IFormCollection form)
        {
            User newUser = new User();
            newUser.UserName = form["userName"];
            //Hacer cifrado:
            newUser.Password = form["Password"];
            newUser._id = Guid.NewGuid().ToString();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62573/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var postTask = client.PostAsJsonAsync<User>("userValidation", newUser);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    string resultAux = await result.Content.ReadAsAsync<string>();
                    User actualUser = JsonSerializer.Deserialize<User>(resultAux);

                    if (actualUser.Password == newUser.Password)
                    {
                        HttpContext.Session.SetString("thisUser", JsonSerializer.Serialize(actualUser));
                        return View("../Home/Main_Screen");
                    }
                    else
                    {
                        ViewBag.UserNotFound = "Incorrect password!";
                        return View("../Home/Index");
                    }
                }
                else
                {
                    ViewBag.UserNotFound = "This user does not exist!";
                    return View("../Home/Index");
                }
            }
        }
    }
}
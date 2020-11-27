using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Design.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Structs_Final_Project;
using System.IO;

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
        public async Task<ActionResult> Register(IFormCollection form)
        {
            User newUser = new User();
            newUser.UserName = form["UserName"];
            Cesar cesar = new Cesar();
            string _encryption = cesar.CesarE("CENTRIFUGADO", form["Password"]);
            newUser.Password = _encryption;
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

                Random random = new Random();
                bool insert = false;
                while (!insert)
                {
                    int number = random.Next(1, 1000);
                    newUser.Private_Number = number;

                    var posTask2 = client.PostAsJsonAsync<User>("privateNumberValidation", newUser);
                    posTask2.Wait();

                    var result2 = posTask2.Result;

                    if (!result2.IsSuccessStatusCode)
                    {
                        insert = true;
                    }
                }

                var postTask = client.PostAsJsonAsync<User>("addUser", newUser);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("thisUser", JsonSerializer.Serialize(newUser));

                    var postTask3 = client.GetAsync("allUsers");
                    postTask3.Wait();

                    var result3 = postTask3.Result;

                    if (result3.IsSuccessStatusCode)
                    {
                        string resultAux2 = await result3.Content.ReadAsAsync<string>();
                        User[] allUsers = JsonSerializer.Deserialize<User[]>(resultAux2);
                        List<string> nameAllUsers = new List<string>();
                        for (int i = 0; i < allUsers.Length; i++)
                        {
                            if (allUsers[i].UserName != newUser.UserName)
                            {
                                nameAllUsers.Add(allUsers[i].UserName);
                            }
                        }
                        ViewBag.AllUsers = nameAllUsers;
                    }

                    List<Message> listAux = new List<Message>();
                    ViewBag.actualMessages = listAux;
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
            Cesar cesar = new Cesar();
            string _encryption = cesar.CesarE("CENTRIFUGADO", form["Password"]);
            newUser.Password = _encryption;
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

                        var postTask3 = client.GetAsync("allUsers");
                        postTask3.Wait();

                        var result3 = postTask3.Result;

                        if (result3.IsSuccessStatusCode)
                        {
                            string resultAux2 = await result3.Content.ReadAsAsync<string>();
                            User[] allUsers = JsonSerializer.Deserialize<User[]>(resultAux2);
                            List<string> nameAllUsers = new List<string>();
                            for (int i = 0; i < allUsers.Length; i++)
                            {
                                if (allUsers[i].UserName != newUser.UserName)
                                {
                                    nameAllUsers.Add(allUsers[i].UserName);
                                }
                            }
                            ViewBag.AllUsers = nameAllUsers;
                        }

                        List<Message> listAux = new List<Message>();
                        ViewBag.actualMessages = listAux;
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

        //Listo con actualización de combo box
        [HttpPost]
        public async Task<ActionResult> SendMessage(IFormCollection form)
        {
            Message newMessage = new Message();
            User user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("thisUser"));

            //Nuevo
            Message userAux = new Message();
            userAux.receptor = form["ListUsers"];
            HttpContext.Session.SetString("thisR", JsonSerializer.Serialize(userAux));


            newMessage.emisor = user.UserName;
            newMessage.receptor = form["ListUsers"];
            newMessage.message = form["Message"];
            newMessage._id = Guid.NewGuid().ToString();
            newMessage.date = DateTime.Now;
            newMessage.type = "message";

            if ((newMessage.message != "") && (newMessage.receptor != ""))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:62573/api/");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<Message>("addMessage", newMessage);
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var postTask2 = client.GetAsync("allMessages");
                        postTask2.Wait();

                        var result2 = postTask2.Result;

                        if (result2.IsSuccessStatusCode)
                        {
                            string resultAux2 = await result2.Content.ReadAsAsync<string>();
                            Message[] allMessages = JsonSerializer.Deserialize<Message[]>(resultAux2);
                            List<Message> allMessagesAux = new List<Message>();

                            for (int i = 0; i < allMessages.Length; i++)
                            {
                                if (((allMessages[i].emisor == newMessage.emisor) && (allMessages[i].receptor == newMessage.receptor)) || ((allMessages[i].receptor == newMessage.emisor) && (allMessages[i].emisor == newMessage.receptor)))
                                {
                                    allMessagesAux.Add(allMessages[i]);
                                }
                            }

                            #region "Motrar nuevamente usuario en el combobox"
                            var postTask3 = client.GetAsync("allUsers");
                            postTask3.Wait();

                            var result3 = postTask3.Result;

                            if (result3.IsSuccessStatusCode)
                            {
                                string resultAux3 = await result3.Content.ReadAsAsync<string>();
                                User[] allUsers = JsonSerializer.Deserialize<User[]>(resultAux3);
                                List<string> nameAllUsers = new List<string>();
                                for (int i = 0; i < allUsers.Length; i++)
                                {
                                    //Nuevo
                                    Message mmm = JsonSerializer.Deserialize<Message>(HttpContext.Session.GetString("thisR"));

                                    if(allUsers[i].UserName == mmm.receptor)
                                    {
                                        nameAllUsers.Insert(0, mmm.receptor);
                                    }
                                    else if (allUsers[i].UserName != user.UserName)
                                    {
                                        nameAllUsers.Add(allUsers[i].UserName);
                                    }
                                }

                                string[] arrayAux = nameAllUsers.ToArray();
                                HttpContext.Session.SetString("nameAllUsers", JsonSerializer.Serialize(arrayAux));
                            }
                            #endregion

                            Message[] arrayAux2 = allMessagesAux.ToArray();
                            HttpContext.Session.SetString("nameAllMessages", JsonSerializer.Serialize(arrayAux2));

                            return RedirectToAction("LoginChat");
                        }
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
            }
            else
            {
                //Solo se cargan los mensajes :)
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:62573/api/");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.GetAsync("allMessages");
                    postTask.Wait();

                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        string resultAux2 = await result.Content.ReadAsAsync<string>();
                        Message[] allMessages = JsonSerializer.Deserialize<Message[]>(resultAux2);
                        List<Message> allMessagesAux = new List<Message>();

                        for (int i = 0; i < allMessages.Length; i++)
                        {
                            if (((allMessages[i].emisor == newMessage.emisor) && (allMessages[i].receptor == newMessage.receptor)) || ((allMessages[i].receptor == newMessage.emisor) && (allMessages[i].emisor == newMessage.receptor)))
                            {
                                allMessagesAux.Add(allMessages[i]);
                            }
                        }

                        Message[] arrayAux2 = allMessagesAux.ToArray();
                        HttpContext.Session.SetString("nameAllMessages", JsonSerializer.Serialize(arrayAux2));

                    }
                    #region "Motrar nuevamente usuario en el combobox"
                    var postTask2 = client.GetAsync("allUsers");
                    postTask2.Wait();

                    var result2 = postTask2.Result;

                    if (result2.IsSuccessStatusCode)
                    {
                        string resultAux3 = await result2.Content.ReadAsAsync<string>();
                        User[] allUsers = JsonSerializer.Deserialize<User[]>(resultAux3);
                        List<string> nameAllUsers = new List<string>();
                        for (int i = 0; i < allUsers.Length; i++)
                        {
                            Message mmm = JsonSerializer.Deserialize<Message>(HttpContext.Session.GetString("thisR"));

                            if (allUsers[i].UserName == mmm.receptor)
                            {
                                nameAllUsers.Insert(0, mmm.receptor);
                            }
                            else if (allUsers[i].UserName != user.UserName)
                            {
                                nameAllUsers.Add(allUsers[i].UserName);
                            }
                        }

                        string[] arrayAux = nameAllUsers.ToArray();
                        HttpContext.Session.SetString("nameAllUsers", JsonSerializer.Serialize(arrayAux));
                    }
                    #endregion
                    return RedirectToAction("LoginChat");
                }
            }

            return Ok();
        }

        public ActionResult LoginChat()
        {
            if (HttpContext.Session.GetString("nameAllUsers") != null)
            {
                ViewBag.AllUsers = JsonSerializer.Deserialize<string[]>(HttpContext.Session.GetString("nameAllUsers"));
            }
            if (HttpContext.Session.GetString("nameAllMessages") != null)
            {
                ViewBag.actualMessages = JsonSerializer.Deserialize<Message[]>(HttpContext.Session.GetString("nameAllMessages"));
            }
            return View("../Home/Main_Screen");
        }

        //Listo con actualización de combo box
        [HttpPost]
        public async Task<ActionResult> Archive(IFormFile file, IFormCollection form)
        {
            User userAux = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("thisUser"));
            Message message = new Message();
            message.emisor = userAux.UserName;
            message.receptor = form["receptor"];
            message.message = "attached file";
            message._id = Guid.NewGuid().ToString();
            message.date = DateTime.Now;
            message.type = "archive";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62573/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var postTask = client.PostAsJsonAsync<Message>("addMessage", message);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    byte[] originalByte = null;
                    using (var memory = new MemoryStream())
                    {
                        await file.CopyToAsync(memory);
                        originalByte = memory.ToArray();
                    }
                    Archive archive = new Archive();
                    archive._id = message._id;
                    archive.message = originalByte;

                    var postTaskAux = client.PostAsJsonAsync<Archive>("addArchive", archive);
                    postTaskAux.Wait();
                    //

                    var postTask2 = client.GetAsync("allMessages");
                    postTask2.Wait();

                    var result2 = postTask2.Result;

                    if (result2.IsSuccessStatusCode)
                    {
                        string resultAux2 = await result2.Content.ReadAsAsync<string>();
                        Message[] allMessages = JsonSerializer.Deserialize<Message[]>(resultAux2);
                        List<Message> allMessagesAux = new List<Message>();

                        for (int i = 0; i < allMessages.Length; i++)
                        {
                            if (((allMessages[i].emisor == message.emisor) && (allMessages[i].receptor == message.receptor)) || ((allMessages[i].receptor == message.emisor) && (allMessages[i].emisor == message.receptor)))
                            {
                                allMessagesAux.Add(allMessages[i]);
                            }
                        }

                        #region "Motrar nuevamente usuario en el combobox"
                        var postTask3 = client.GetAsync("allUsers");
                        postTask3.Wait();

                        var result3 = postTask3.Result;

                        if (result3.IsSuccessStatusCode)
                        {
                            string resultAux3 = await result3.Content.ReadAsAsync<string>();
                            User[] allUsers = JsonSerializer.Deserialize<User[]>(resultAux3);
                            List<string> nameAllUsers = new List<string>();
                            for (int i = 0; i < allUsers.Length; i++)
                            {
                                Message mmm = JsonSerializer.Deserialize<Message>(HttpContext.Session.GetString("thisR"));

                                if (allUsers[i].UserName == mmm.receptor)
                                {
                                    nameAllUsers.Insert(0, mmm.receptor);
                                }
                                else if (allUsers[i].UserName != userAux.UserName)
                                {
                                    nameAllUsers.Add(allUsers[i].UserName);
                                }
                            }

                            string[] arrayAux = nameAllUsers.ToArray();
                            HttpContext.Session.SetString("nameAllUsers", JsonSerializer.Serialize(arrayAux));
                        }
                        #endregion

                        Message[] arrayAux2 = allMessagesAux.ToArray();
                        HttpContext.Session.SetString("nameAllMessages", JsonSerializer.Serialize(arrayAux2));

                        return RedirectToAction("LoginChat");
                    }
                }
                else
                {
                    return StatusCode(500);
                }
            }
            return StatusCode(500);
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(string id)
        {
            Archive archiveAux = new Archive();
            archiveAux._id = id;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62573/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var postTask = client.PostAsJsonAsync<Archive>("downloadFile", archiveAux);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    string stringAux = await result.Content.ReadAsAsync<string>();
                    Archive allMessages = JsonSerializer.Deserialize<Archive>(stringAux);
                    return File(allMessages.message, "text/plain", "resultado.txt");
                }
                else
                {
                    return StatusCode(500);
                }
            }           
        }

        [HttpPost]
        public async Task<ActionResult> SearchMessages(IFormCollection form)
        {
            string messageToSearch = form["MessageToSearch"];
            string receptor = form["receptor"];

            Message messageAux = new Message();
            messageAux.message = messageToSearch;
            User userAux = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("thisUser"));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62573/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var postTask = client.PostAsJsonAsync<Message>("searchMessages", messageAux);
                postTask.Wait();

                var result = postTask.Result;

                //Si encontró algún mensaje igual
                if (result.IsSuccessStatusCode)
                {
                    string resultAux = await result.Content.ReadAsAsync<string>();
                    Message[] allMessages = JsonSerializer.Deserialize<Message[]>(resultAux);
                    List<Message> allMessagesAux = new List<Message>();

                    for (int i = 0; i < allMessages.Length; i++)
                    {
                        if (((allMessages[i].emisor == userAux.UserName) && (allMessages[i].receptor == receptor)) || ((allMessages[i].receptor == userAux.UserName) && (allMessages[i].emisor == receptor)))
                        {
                            allMessagesAux.Add(allMessages[i]);
                        }
                    }
                    Message[] arrayAux2 = allMessagesAux.ToArray();
                    HttpContext.Session.SetString("nameAllMessages", JsonSerializer.Serialize(arrayAux2));
                }
                
                #region "Motrar nuevamente usuario en el combobox"
                var postTask3 = client.GetAsync("allUsers");
                postTask3.Wait();

                var result3 = postTask3.Result;

                if (result3.IsSuccessStatusCode)
                {
                    string resultAux3 = await result3.Content.ReadAsAsync<string>();
                    User[] allUsers = JsonSerializer.Deserialize<User[]>(resultAux3);
                    List<string> nameAllUsers = new List<string>();
                    for (int i = 0; i < allUsers.Length; i++)
                    {
                        Message mmm = JsonSerializer.Deserialize<Message>(HttpContext.Session.GetString("thisR"));

                        if (allUsers[i].UserName == mmm.receptor)
                        {
                            nameAllUsers.Insert(0, mmm.receptor);
                        }
                        else if (allUsers[i].UserName != userAux.UserName)
                        {
                            nameAllUsers.Add(allUsers[i].UserName);
                        }
                    }

                    string[] arrayAux = nameAllUsers.ToArray();
                    HttpContext.Session.SetString("nameAllUsers", JsonSerializer.Serialize(arrayAux));
                }
                #endregion
            }
            return LoginChat();            
        }
    }
}
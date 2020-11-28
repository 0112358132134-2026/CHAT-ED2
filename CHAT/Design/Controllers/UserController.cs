using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Design.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Structs_Final_Project_1;
using System.IO;
using System.Numerics;
using Microsoft.AspNetCore.Hosting;

namespace Design.Controllers
{
    public class UserController : Controller
    {
        private IWebHostEnvironment _env;
        public UserController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
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
                        ViewBag.UserNotFound = "Incorrect password / user!";
                        return View("../Home/Index");
                    }
                }
                else
                {
                    ViewBag.UserNotFound = "Incorrect password / user!";
                    return View("../Home/Index");
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage(IFormCollection form)
        {
            string actualMessage = form["Message"];

            Message newMessage = new Message();
            User user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("thisUser"));

            Message userAux = new Message
            {
                receptor = form["ListUsers"]
            };
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
                    
                    var postAllUsers = client.GetAsync("allUsers");
                    postAllUsers.Wait();
                    var fufu = postAllUsers.Result;
                    string asdasda = await fufu.Content.ReadAsAsync<string>();
                    User[] allUsersXD = JsonSerializer.Deserialize<User[]>(asdasda);

                    User newUser1 = new User();
                    User newUser2 = new User();
                    for (int i = 0; i < allUsersXD.Length; i++)
                    {
                        if (allUsersXD[i].UserName == newMessage.emisor)
                        {
                            newUser1 = allUsersXD[i];
                        }
                        if (allUsersXD[i].UserName == newMessage.receptor)
                        {
                            newUser2 = allUsersXD[i];
                        }
                    }
                    
                    #region "Cifrado SDES"
                    Diffie_Hellman _diffie = new Diffie_Hellman();
                    BigInteger publicNumber1 = _diffie.PublicNumberGenerator(33, 101, newUser1.Private_Number);
                    BigInteger publicNumber2 = _diffie.PublicNumberGenerator(33, 101, newUser2.Private_Number);
                    BigInteger kuser1 = _diffie.KGenerator(publicNumber2, newUser1.Private_Number, 101);
                    BigInteger kuser2 = _diffie.KGenerator(publicNumber1, newUser2.Private_Number, 101);

                    string response = "";
                    if (kuser1 == kuser2)
                    {
                        SDES _sdes = new SDES();
                        string keyMaster = _sdes.ToNBase(kuser1, 2);
                        _sdes.KeysGenerator(keyMaster);
                        //Convertir mensaje a arreglo de bytes:
                        char[] charrArray_OriginalText = actualMessage.ToCharArray();
                        byte[] original_Bytes = new byte[charrArray_OriginalText.Length];
                        for (int i = 0; i < charrArray_OriginalText.Length; i++)
                        {
                            original_Bytes[i] = (byte)charrArray_OriginalText[i];
                        }
                        byte[] resultFinalBytes = _sdes.SDES_Encryption(original_Bytes, "cifrado");
                        //Convertir arreglo de bytes a string y agregarlo a Mongo:
                        char[] charrArra_Result = new char[resultFinalBytes.Length];
                        for (int i = 0; i < resultFinalBytes.Length; i++)
                        {
                            charrArra_Result[i] = (char)resultFinalBytes[i];
                        }
                        for (int i = 0; i < charrArra_Result.Length; i++)
                        {
                            response += charrArra_Result[i].ToString();
                        }
                    }
                    newMessage.message = response;
                    #endregion

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
                                    if (allMessages[i].type == "message")
                                    {
                                        #region "Descifrado SDES"

                                        var postAllUsers2 = client.GetAsync("allUsers");
                                        postAllUsers2.Wait();
                                        var fufufu = postAllUsers2.Result;
                                        string asdasdasa = await fufufu.Content.ReadAsAsync<string>();
                                        User[] allUsersXDD = JsonSerializer.Deserialize<User[]>(asdasdasa);

                                        User newUser11 = new User();
                                        User newUser22 = new User();
                                        for (int j = 0; j < allUsersXDD.Length; j++)
                                        {
                                            if (allUsersXDD[j].UserName == newMessage.emisor)
                                            {
                                                newUser11 = allUsersXDD[j];
                                            }
                                            if (allUsersXDD[j].UserName == newMessage.receptor)
                                            {
                                                newUser22 = allUsersXDD[j];
                                            }
                                        }                                        

                                        Diffie_Hellman _diffie1 = new Diffie_Hellman();
                                        BigInteger publicNumber11 = _diffie1.PublicNumberGenerator(33, 101, newUser11.Private_Number);
                                        BigInteger publicNumber22 = _diffie1.PublicNumberGenerator(33, 101, newUser22.Private_Number);
                                        BigInteger kuser11 = _diffie1.KGenerator(publicNumber22, newUser11.Private_Number, 101);
                                        BigInteger kuser22 = _diffie1.KGenerator(publicNumber11, newUser22.Private_Number, 101);

                                        string response2 = "";
                                        if (kuser11 == kuser22)
                                        {
                                            SDES _sdes = new SDES();
                                            string keyMaster = _sdes.ToNBase(kuser11, 2);
                                            _sdes.KeysGenerator(keyMaster);
                                            //Convertir mensaje a arreglo de bytes:
                                            char[] arrayChar = allMessages[i].message.ToCharArray();
                                            byte[] bytes_Encrypted = new byte[arrayChar.Length];
                                            for (int j = 0; j < arrayChar.Length; j++)
                                            {
                                                bytes_Encrypted[j] = (byte)arrayChar[j];
                                            }
                                            byte[] resultFinalBytes = _sdes.SDES_Encryption(bytes_Encrypted, "descifrado");
                                            //Convertir arreglo de bytes a string y agregarlo a Mongo:
                                            char[] charResult = new char[resultFinalBytes.Length];
                                            for (int j = 0; j < resultFinalBytes.Length; j++)
                                            {
                                                charResult[j] = (char)resultFinalBytes[j];
                                            }
                                            for (int j = 0; j < charResult.Length; j++)
                                            {
                                                response2 += charResult[j].ToString();
                                            }
                                        }
                                        allMessages[i].message = response2;
                                        #endregion
                                    }
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
                                if (allMessages[i].type == "message")
                                {
                                    #region "Descifrado SDES"

                                    var postAllUsers2 = client.GetAsync("allUsers");
                                    postAllUsers2.Wait();
                                    var fufufu = postAllUsers2.Result;
                                    string asdasdasa = await fufufu.Content.ReadAsAsync<string>();
                                    User[] allUsersXDD = JsonSerializer.Deserialize<User[]>(asdasdasa);

                                    User newUser11 = new User();
                                    User newUser22 = new User();
                                    for (int j = 0; j < allUsersXDD.Length; j++)
                                    {
                                        if (allUsersXDD[j].UserName == newMessage.emisor)
                                        {
                                            newUser11 = allUsersXDD[j];
                                        }
                                        if (allUsersXDD[j].UserName == newMessage.receptor)
                                        {
                                            newUser22 = allUsersXDD[j];
                                        }
                                    }

                                    Diffie_Hellman _diffie1 = new Diffie_Hellman();
                                    BigInteger publicNumber11 = _diffie1.PublicNumberGenerator(33, 101, newUser11.Private_Number);
                                    BigInteger publicNumber22 = _diffie1.PublicNumberGenerator(33, 101, newUser22.Private_Number);
                                    BigInteger kuser11 = _diffie1.KGenerator(publicNumber22, newUser11.Private_Number, 101);
                                    BigInteger kuser22 = _diffie1.KGenerator(publicNumber11, newUser22.Private_Number, 101);

                                    string response2 = "";
                                    if (kuser11 == kuser22)
                                    {
                                        SDES _sdes = new SDES();
                                        string keyMaster = _sdes.ToNBase(kuser11, 2);
                                        _sdes.KeysGenerator(keyMaster);
                                        //Convertir mensaje a arreglo de bytes:
                                        char[] arrayChar = allMessages[i].message.ToCharArray();
                                        byte[] bytes_Encrypted = new byte[arrayChar.Length];
                                        for (int j = 0; j < arrayChar.Length; j++)
                                        {
                                            bytes_Encrypted[j] = (byte)arrayChar[j];
                                        }
                                        byte[] resultFinalBytes = _sdes.SDES_Encryption(bytes_Encrypted, "descifrado");
                                        //Convertir arreglo de bytes a string y agregarlo a Mongo:
                                        char[] charResult = new char[resultFinalBytes.Length];
                                        for (int j = 0; j < resultFinalBytes.Length; j++)
                                        {
                                            charResult[j] = (char)resultFinalBytes[j];
                                        }
                                        for (int j = 0; j < charResult.Length; j++)
                                        {
                                            response2 += charResult[j].ToString();
                                        }
                                    }
                                    allMessages[i].message = response2;
                                    #endregion
                                }
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
                    #region "LZW Compression"
                    LZW _lzw = new LZW();
                    byte[] resultArchive = null;
                    byte[] originalByte = null;
                   
                    using (var memory = new MemoryStream())
                    {
                        await file.CopyToAsync(memory);
                        originalByte = memory.ToArray();
                        //nuevo
                        using (FileStream fstream = System.IO.File.Create(_env.ContentRootPath + "/Copy/" + file.FileName))
                        {
                            fstream.Write(memory.ToArray());
                            fstream.Close();
                        }
                        resultArchive = _lzw.Compression(_env.ContentRootPath + "/Copy/" + file.FileName, file.FileName);
                    }
                    System.IO.File.Delete(_env.ContentRootPath + "/Copy/" + file.FileName);
                    #endregion

                    Archive archive = new Archive
                    {
                        _id = message._id,
                        message = resultArchive,
                        name = file.FileName
                    };

                    var postTaskAux = client.PostAsJsonAsync<Archive>("addArchive", archive);
                    postTaskAux.Wait();                    

                    //HERE

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
                                if (allMessages[i].type == "message")
                                {
                                    #region "Descifrado SDES"

                                    var postAllUsers2 = client.GetAsync("allUsers");
                                    postAllUsers2.Wait();
                                    var fufufu = postAllUsers2.Result;
                                    string asdasdasa = await fufufu.Content.ReadAsAsync<string>();
                                    User[] allUsersXDD = JsonSerializer.Deserialize<User[]>(asdasdasa);

                                    User newUser11 = new User();
                                    User newUser22 = new User();
                                    for (int j = 0; j < allUsersXDD.Length; j++)
                                    {
                                        if (allUsersXDD[j].UserName == userAux.UserName)
                                        {
                                            newUser11 = allUsersXDD[j];
                                        }
                                        if (allUsersXDD[j].UserName == form["receptor"])
                                        {
                                            newUser22 = allUsersXDD[j];
                                        }
                                    }

                                    Diffie_Hellman _diffie1 = new Diffie_Hellman();
                                    BigInteger publicNumber11 = _diffie1.PublicNumberGenerator(33, 101, newUser11.Private_Number);
                                    BigInteger publicNumber22 = _diffie1.PublicNumberGenerator(33, 101, newUser22.Private_Number);
                                    BigInteger kuser11 = _diffie1.KGenerator(publicNumber22, newUser11.Private_Number, 101);
                                    BigInteger kuser22 = _diffie1.KGenerator(publicNumber11, newUser22.Private_Number, 101);

                                    string response2 = "";
                                    if (kuser11 == kuser22)
                                    {
                                        SDES _sdes = new SDES();
                                        string keyMaster = _sdes.ToNBase(kuser11, 2);
                                        _sdes.KeysGenerator(keyMaster);
                                        //Convertir mensaje a arreglo de bytes:
                                        char[] arrayChar = allMessages[i].message.ToCharArray();
                                        byte[] bytes_Encrypted = new byte[arrayChar.Length];
                                        for (int j = 0; j < arrayChar.Length; j++)
                                        {
                                            bytes_Encrypted[j] = (byte)arrayChar[j];
                                        }
                                        byte[] resultFinalBytes = _sdes.SDES_Encryption(bytes_Encrypted, "descifrado");
                                        //Convertir arreglo de bytes a string y agregarlo a Mongo:
                                        char[] charResult = new char[resultFinalBytes.Length];
                                        for (int j = 0; j < resultFinalBytes.Length; j++)
                                        {
                                            charResult[j] = (char)resultFinalBytes[j];
                                        }
                                        for (int j = 0; j < charResult.Length; j++)
                                        {
                                            response2 += charResult[j].ToString();
                                        }
                                    }
                                    allMessages[i].message = response2;
                                    #endregion
                                }
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

                    #region "LZW Decompression"
                    LZW _lzw = new LZW();
                    
                    using (FileStream fstream = System.IO.File.Create(_env.ContentRootPath + "/Copy2/" + allMessages.name))
                    {
                        fstream.Write(allMessages.message);
                        fstream.Close();
                    }
                    string originalName = _lzw.GetOriginalName(_env.ContentRootPath + "/Copy2/" + allMessages.name);
                    byte[] resultArchive = _lzw.Decompression(_env.ContentRootPath + "/Copy2/" + allMessages.name);                    
                    
                    System.IO.File.Delete(_env.ContentRootPath + "/Copy2/" + allMessages.name);
                    #endregion

                    return File(resultArchive, "text/plain", originalName);
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
            Message mmmm = JsonSerializer.Deserialize<Message>(HttpContext.Session.GetString("thisR"));

            Message messageAux = new Message();
            User userAux = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("thisUser"));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62573/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //Cifrar mensaje
                var postAllUsers = client.GetAsync("allUsers");
                postAllUsers.Wait();
                var fufu = postAllUsers.Result;
                string asdasda = await fufu.Content.ReadAsAsync<string>();
                User[] allUsersXD = JsonSerializer.Deserialize<User[]>(asdasda);

                User newUser1 = new User();
                User newUser2 = new User();
                for (int i = 0; i < allUsersXD.Length; i++)
                {
                    if (userAux.UserName != form["receptor"])
                    {
                        if (allUsersXD[i].UserName == userAux.UserName)
                        {
                            newUser1 = allUsersXD[i];
                        }
                        if (allUsersXD[i].UserName == form["receptor"])
                        {
                            newUser2 = allUsersXD[i];
                        }
                    }
                    else
                    {
                        if (allUsersXD[i].UserName == mmmm.receptor)
                        {
                            newUser1 = allUsersXD[i];
                        }
                        if (allUsersXD[i].UserName == form["receptor"])
                        {
                            newUser2 = allUsersXD[i];
                        }
                    }                   
                }

                #region "Cifrado SDES"
                Diffie_Hellman _diffie = new Diffie_Hellman();
                BigInteger publicNumber1 = _diffie.PublicNumberGenerator(33, 101, newUser1.Private_Number);
                BigInteger publicNumber2 = _diffie.PublicNumberGenerator(33, 101, newUser2.Private_Number);
                BigInteger kuser1 = _diffie.KGenerator(publicNumber2, newUser1.Private_Number, 101);
                BigInteger kuser2 = _diffie.KGenerator(publicNumber1, newUser2.Private_Number, 101);

                string response = "";
                if (kuser1 == kuser2)
                {
                    SDES _sdes = new SDES();
                    string keyMaster = _sdes.ToNBase(kuser1, 2);
                    _sdes.KeysGenerator(keyMaster);
                    //Convertir mensaje a arreglo de bytes:
                    char[] charrArray_OriginalText = messageToSearch.ToCharArray();
                    byte[] original_Bytes = new byte[charrArray_OriginalText.Length];
                    for (int i = 0; i < charrArray_OriginalText.Length; i++)
                    {
                        original_Bytes[i] = (byte)charrArray_OriginalText[i];
                    }
                    byte[] resultFinalBytes = _sdes.SDES_Encryption(original_Bytes, "cifrado");
                    //Convertir arreglo de bytes a string y agregarlo a Mongo:
                    char[] charrArra_Result = new char[resultFinalBytes.Length];
                    for (int i = 0; i < resultFinalBytes.Length; i++)
                    {
                        charrArra_Result[i] = (char)resultFinalBytes[i];
                    }
                    for (int i = 0; i < charrArra_Result.Length; i++)
                    {
                        response += charrArra_Result[i].ToString();
                    }
                }
                messageAux.message = response;
                #endregion

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
                        bool oka = false;
                        if (((allMessages[i].emisor == mmmm.receptor) && (allMessages[i].receptor == receptor)) || ((allMessages[i].receptor == mmmm.receptor) && (allMessages[i].emisor == receptor)))
                        {
                            oka = true;
                        }
                        if (((allMessages[i].emisor == userAux.UserName) && (allMessages[i].receptor == receptor)) || ((allMessages[i].receptor == userAux.UserName) && (allMessages[i].emisor == receptor)) || oka)
                        {
                            if (allMessages[i].type == "message")
                            {
                                #region "Descifrado SDES"

                                var postAllUsers2 = client.GetAsync("allUsers");
                                postAllUsers2.Wait();
                                var fufufu = postAllUsers2.Result;
                                string asdasdasa = await fufufu.Content.ReadAsAsync<string>();
                                User[] allUsersXDD = JsonSerializer.Deserialize<User[]>(asdasdasa);

                                User newUser11 = new User();
                                User newUser22 = new User();
                                for (int j = 0; j < allUsersXDD.Length; j++)
                                {
                                    if (userAux.UserName != form["receptor"])
                                    {
                                        if (allUsersXDD[j].UserName == userAux.UserName)
                                        {
                                            newUser11 = allUsersXDD[j];
                                        }
                                        if (allUsersXDD[j].UserName == receptor)
                                        {
                                            newUser22 = allUsersXDD[j];
                                        }
                                    }
                                    else
                                    {
                                        if (allUsersXD[j].UserName == mmmm.receptor)
                                        {
                                            newUser11 = allUsersXD[j];
                                        }
                                        if (allUsersXD[j].UserName == form["receptor"])
                                        {
                                            newUser22 = allUsersXD[j];
                                        }
                                    }
                                }

                                Diffie_Hellman _diffie1 = new Diffie_Hellman();
                                BigInteger publicNumber11 = _diffie1.PublicNumberGenerator(33, 101, newUser11.Private_Number);
                                BigInteger publicNumber22 = _diffie1.PublicNumberGenerator(33, 101, newUser22.Private_Number);
                                BigInteger kuser11 = _diffie1.KGenerator(publicNumber22, newUser11.Private_Number, 101);
                                BigInteger kuser22 = _diffie1.KGenerator(publicNumber11, newUser22.Private_Number, 101);

                                string response2 = "";
                                if (kuser11 == kuser22)
                                {
                                    SDES _sdes = new SDES();
                                    string keyMaster = _sdes.ToNBase(kuser11, 2);
                                    _sdes.KeysGenerator(keyMaster);
                                    //Convertir mensaje a arreglo de bytes:
                                    char[] arrayChar = allMessages[i].message.ToCharArray();
                                    byte[] bytes_Encrypted = new byte[arrayChar.Length];
                                    for (int j = 0; j < arrayChar.Length; j++)
                                    {
                                        bytes_Encrypted[j] = (byte)arrayChar[j];
                                    }
                                    byte[] resultFinalBytes = _sdes.SDES_Encryption(bytes_Encrypted, "descifrado");
                                    //Convertir arreglo de bytes a string y agregarlo a Mongo:
                                    char[] charResult = new char[resultFinalBytes.Length];
                                    for (int j = 0; j < resultFinalBytes.Length; j++)
                                    {
                                        charResult[j] = (char)resultFinalBytes[j];
                                    }
                                    for (int j = 0; j < charResult.Length; j++)
                                    {
                                        response2 += charResult[j].ToString();
                                    }
                                }
                                allMessages[i].message = response2;
                                #endregion
                            }
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
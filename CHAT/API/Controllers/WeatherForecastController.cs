using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Design.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpPost]
        [Route("privateNumberValidation")]
        public IActionResult nValidation(User user)
        {
            if (!ModelState.IsValid)
                return NotFound();

            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("Private_Number", user.Private_Number);
            var result = collection.Find(filter).FirstOrDefault();

            if (result != null)
            {
                User userAux = BsonSerializer.Deserialize<User>(result);
                var json = JsonSerializer.Serialize(userAux);
                return Ok(json);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("allUsers")]
        public IActionResult allUsers()
        {
            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("users");
            var document = collection.Find(new BsonDocument()).ToList();

            if (document.Count != 0)
            {
                User[] array = new User[document.Count];
                int i = 0;
                foreach (BsonDocument item in document)
                {
                    array[i] = BsonSerializer.Deserialize<User>(item);
                    i++;
                }
                var json = JsonSerializer.Serialize(array);
                return Ok(json);
            }
            else
            {
                return NotFound();
            }            
        }

        [HttpGet]
        [Route("allMessages")]
        public IActionResult allMessages()
        {
            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("messages");
            var document = collection.Find(new BsonDocument()).ToList();

            if (document.Count != 0)
            {
                Message[] array = new Message[document.Count];
                int i = 0;
                foreach (BsonDocument item in document)
                {
                    array[i] = BsonSerializer.Deserialize<Message>(item);
                    i++;
                }
                var json = JsonSerializer.Serialize(array);
                return Ok(json);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("userValidation")]
        public IActionResult userV(User user)
        {
            if (!ModelState.IsValid)
                return NotFound();

            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("UserName", user.UserName);
            var result = collection.Find(filter).FirstOrDefault();

            if (result != null)
            {
                User userAux = BsonSerializer.Deserialize<User>(result);
                var json = JsonSerializer.Serialize(userAux);
                return Ok(json);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("addUser")]
        public IActionResult addUser(User newUser)
        {
            if (!ModelState.IsValid)
                return NotFound();

            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("users");
            var user = new BsonDocument { { "_id", newUser._id }, { "UserName", newUser.UserName }, { "Password", newUser.Password }, { "Private_Number", newUser.Private_Number } };
            collection.InsertOne(user);
            return Ok();
        }

        [HttpPost]
        [Route("addMessage")]
        public IActionResult addMessage(Message newMessage)
        {
            if (!ModelState.IsValid)
                return NotFound();

            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("messages");
            var message = new BsonDocument { { "_id", newMessage._id }, { "emisor", newMessage.emisor }, { "receptor", newMessage.receptor }, { "message", newMessage.message }, { "date", newMessage.date }, { "type", newMessage.type } };
            collection.InsertOne(message);
            return Ok();
        }

        [HttpPost]
        [Route("addArchive")]
        public IActionResult addArchive(Archive archive)
        {
            if (!ModelState.IsValid)
                return NotFound();

            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("records");
            var message = new BsonDocument { { "_id", archive._id }, { "message", archive.message }, { "name", archive.name} };
            collection.InsertOne(message);
            return Ok();
        }

        [HttpPost]
        [Route("downloadFile")]
        public IActionResult downloadFile(Archive archive)
        {
            if (!ModelState.IsValid)
                return NotFound();

            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("records");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", archive._id);
            var result = collection.Find(filter).FirstOrDefault();

            if (result != null)
            {
                Archive archiveAux = BsonSerializer.Deserialize<Archive>(result);
                var json = JsonSerializer.Serialize(archiveAux);
                return Ok(json);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("searchMessages")]
        public IActionResult searchMessages(Message messageAux)
        {
            MongoClient newClient = new MongoClient("mongodb://localhost:27017");
            var db = newClient.GetDatabase("chat");
            var collection = db.GetCollection<BsonDocument>("messages");
            var filter = Builders<BsonDocument>.Filter.Eq("message", messageAux.message);
            var document = collection.Find(filter).ToList();

            if (document.Count != 0)
            {
                Message[] array = new Message[document.Count];
                int i = 0;
                foreach (BsonDocument item in document)
                {
                    array[i] = BsonSerializer.Deserialize<Message>(item);
                    i++;
                }
                var json = JsonSerializer.Serialize(array);
                return Ok(json);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
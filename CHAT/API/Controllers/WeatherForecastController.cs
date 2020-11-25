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
            var user = new BsonDocument {{ "_id", newUser._id }, { "UserName", newUser.UserName }, {"Password", newUser.Password}};
            collection.InsertOne(user);
            return Ok();
        }
    }
}
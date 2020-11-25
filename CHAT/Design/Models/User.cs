using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Design.Models
{
    public class User
    {
        [Required(ErrorMessage = "Please enter a username")]
        [StringLength(15)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter a password")]
        [StringLength(15)]
        public string Password { get; set; }

        public string _id { get; set; }
        public List<Chat> chats { get; set; }

        public User()
        {
            chats = new List<Chat>();
        }
    }
}
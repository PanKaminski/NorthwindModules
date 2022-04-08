using System;
using System.Collections.Generic;

namespace NorthwindApp.FrontEnd.Mvc.Identity.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<User> Users { get; set; }

        public Role()
        {
            this.Users = new List<User>();
        }
    }
}

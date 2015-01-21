using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class UserListViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserPoints { get; set; }
    }
}
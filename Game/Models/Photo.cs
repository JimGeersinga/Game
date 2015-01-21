using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string imgPath { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime PictureTaken { get; set; }
    }
}
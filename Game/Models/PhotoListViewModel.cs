using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class PhotoListViewModel
    {
        public int Id { get; set; }
        public string imgName { get; set; }
        public string imgPath { get; set; }
        public int Orientation { get; set; }
        public DateTime PictureTaken { get; set; }
    }
}
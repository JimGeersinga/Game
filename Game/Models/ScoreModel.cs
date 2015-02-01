using Goheer.EXIF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ImgId { get; set; }
        public int ScorePoints { get; set; }
    }
}
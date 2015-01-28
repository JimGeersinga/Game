using Goheer.EXIF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string imgName { get; set; }
        public string imgPath { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Orientation { get; set; }
        public DateTime PictureTaken { get; set; }

        public static void Rotate(string path)
        {
            Bitmap bmp = new Bitmap(path);
            EXIFextractor exif = new EXIFextractor(ref bmp, "n");

            if (exif["Orientation"] != null)
            {
                RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

                if (flip != RotateFlipType.RotateNoneFlipNone) 
                {
                    bmp.RotateFlip(flip);
                    exif.setTag(0x112, "1"); 
                    bmp.Save(path, ImageFormat.Jpeg);
                    bmp.Dispose();
                }
            }
        }
        private static RotateFlipType OrientationToFlipType(string orientation)
        {
            switch (int.Parse(orientation))
            {
                case 1: return RotateFlipType.RotateNoneFlipNone;
                case 2: return RotateFlipType.RotateNoneFlipX;
                case 3: return RotateFlipType.Rotate180FlipNone;
                case 4: return RotateFlipType.Rotate180FlipX;
                case 5: return RotateFlipType.Rotate90FlipX;
                case 6: return RotateFlipType.Rotate90FlipNone;
                case 7: return RotateFlipType.Rotate270FlipX;
                case 8: return RotateFlipType.Rotate270FlipNone;
                default: return RotateFlipType.RotateNoneFlipNone;
            }
        }
    }
}
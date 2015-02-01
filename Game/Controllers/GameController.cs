using Game.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Game.Controllers
{

    public class GameController : Controller
    {
        GameAppDb _db = new GameAppDb();

        public ActionResult index()
        {
            return View();
        }
        public ActionResult start(int Id = 0)
        {
            // check if user already played this photo

            //---------
            Photo photo = _db.Photos.Find(Id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            Session["photoId"] = photo.Id;
            var id = photo.Id;
            ViewBag.imgPath = "~/Images/uploads/" + photo.imgName;
            ViewBag.Id = photo.Id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Result(int Id )
        {
            double lat = Convert.ToDouble((Request.Form["lat"]).Replace('.',','));
            double lng = Convert.ToDouble((Request.Form["lng"]).Replace('.',',')); 

            Photo photo = _db.Photos.Find(Id);
            if (photo == null){ return HttpNotFound(); }

            double distance = CalcDistance(lat, lng, photo.Latitude, photo.Longitude);
            int points = CalcPoints(distance);

            UserProfile User = _db.UserProfiles.Find(WebSecurity.CurrentUserId);
            User.UserPoints += points;
            _db.SaveChanges();

            ViewBag.Points = points;

            return View();
        }

        private int CalcPoints(double distance) {
            double metre = Math.Round(distance, MidpointRounding.AwayFromZero);
            int km = Convert.ToInt32(metre / 60);
            int points = 0;

            if (km >= 0 && km <= 10) { points = 100;}     
            else if (km > 11 && km <= 20) { points = 90; }  
            else if (km > 21 && km <= 30) { points = 80; }  
            else if (km > 31 && km <= 40) { points = 70; }  
            else if (km > 41 && km <= 50) { points = 60; }  
            else if (km > 51 && km <= 100) { points = 50; }  
            else if (km > 101 && km <= 200) { points = 40; }  
            else if (km > 201 && km <= 300) { points = 30; }  
            else if (km > 301 && km <= 400) { points = 20; } 
            else if (km > 401 && km <= 500) { points = 10; } 
            else if (km > 501 && km <= 600) { points = 8; } 
            else if (km > 601 && km <= 300) { points = 6; } 
            else if (km > 701 && km <= 300) { points = 4; } 
            else if (km > 801 && km <= 300) { points = 2; }
            else if (km > 901 && km <= 1000) { points = 1; } 

            return points;
        }
        private double CalcDistance(double latA, double longA, double latB, double longB) {
            var locA = new GeoCoordinate(latA, longA);
            var locB = new GeoCoordinate(latB, longB);
            double distance = locA.GetDistanceTo(locB);
            return distance;       
        }
    }
}

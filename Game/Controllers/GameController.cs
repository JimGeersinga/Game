using Game.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        public ActionResult start(int Id = -1)
        {
            if (Id == -1)
            {
               Id = GetRandomPictureId();
               if (Id == -999)
               {
                  return Content("No more pictures to play");
               }
            }
            Photo photo = _db.Photos.Find(Id);

            if (photo == null) return HttpNotFound(); 
            if (PhotoPlayed(Id)) return RedirectToAction("index", "Photo");

            Session["photoId"] = photo.Id;
            var id = photo.Id;

            ViewBag.imgPath = "~/Images/uploads/" + photo.imgName;
            ViewBag.Id = photo.Id;
            ViewBag.lat = photo.Latitude;
            ViewBag.lng = photo.Longitude;

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
            if(PhotoPlayed(Id)) return RedirectToAction("index", "Photo");

            double distance = CalcDistance(lat, lng, photo.Latitude, photo.Longitude);
            int points = CalcPoints(distance);

            _db.Scores.Add(new Score
            { 
                UserId = WebSecurity.CurrentUserId,
                ImgId = Id,               
                ScorePoints = points
            });

            UserProfile User = _db.UserProfiles.Find(WebSecurity.CurrentUserId);
            User.UserPoints += points;
            _db.SaveChanges();

            ViewBag.lat1 = lat;
            ViewBag.lon1 = lng;
            ViewBag.lat2 = photo.Latitude;
            ViewBag.lon2 = photo.Longitude;  
            ViewBag.Distance = distance;
            ViewBag.Points = points;

            return View();
        }

        private int CalcPoints(double distance) {

            int points;

            if (distance >= 0 && distance <= 100) { points = 100;}     
            else if (distance > 101 && distance <= 200) { points = 90; }  
            else if (distance > 201 && distance <= 300) { points = 80; }  
            else if (distance > 301 && distance <= 400) { points = 70; }  
            else if (distance > 401 && distance <= 500) { points = 60; }  
            else if (distance > 501 && distance <= 1000) { points = 50; }  
            else if (distance > 1001 && distance <= 2000) { points = 40; }  
            else if (distance > 2001 && distance <= 3000) { points = 30; }  
            else if (distance > 3001 && distance <= 4000) { points = 20; } 
            else if (distance > 4001 && distance <= 5000) { points = 10; } 
            else if (distance > 5001 && distance <= 6000) { points = 8; } 
            else if (distance > 6001 && distance <= 3000) { points = 6; } 
            else if (distance > 7001 && distance <= 3000) { points = 4; } 
            else if (distance > 8001 && distance <= 3000) { points = 2; }
            else if (distance > 9001 && distance <= 10000) { points = 1; }
            else { points = 0; }

            return points;
        }
        private double CalcDistance(double latA, double longA, double latB, double longB) {
            double distance = 0;
            double dLat = (latB - latA) / 180 * Math.PI;
            double dLong = (longB - longA) / 180 * Math.PI;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(latB) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double radiusE = 6378135; 
            double radiusP = 6356750; 
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(latA / 180 * Math.PI), 2);
            double dr = Math.Pow(radiusE * Math.Cos(latA / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(latA / 180 * Math.PI), 2);
            double radius = Math.Sqrt(nr / dr);

            distance = radius * c;
            return distance;       
        }
        private bool PhotoPlayed(int Id)
        {
            var record = from r in _db.Scores
                         where r.UserId == WebSecurity.CurrentUserId && r.ImgId == Id
                         orderby r.Id
                         select r.Id;
            if (record.Count() > 0) { return true; } else { return false; }
        }
        private int GetRandomPictureId()
        {
            List<int> list1 = new List<int>();
            var record1 = from photo in _db.Photos                          
                          orderby photo.Id                          
                          select photo.Id;

            foreach(var item in record1){
               list1.Add(item);
            }

            List<int> list2 = new List<int>();
            var record2 =  from score in _db.Scores
                           orderby score.Id
                           where score.UserId == WebSecurity.CurrentUserId
                           select score.ImgId;

            foreach (var item in record2){
               list2.Add(item);
            }

            int[] terms1 = list1.ToArray();
            int[] terms2 = list2.ToArray();
            var ids = terms1.Except(terms2).ToArray();
            if (ids.Length == 0)
            {
                return -999;
            }
            else
            {
                return ids[new Random().Next(0, ids.Length)];
            }
           
        }
    }
}

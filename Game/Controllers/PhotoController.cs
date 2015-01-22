﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExifLib;
using Game.Models;
using WebMatrix.WebData;


namespace Game.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private GameAppDb _db = new GameAppDb();

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string tempPath = Path.Combine(Server.MapPath("~/App_Data/uploads/temp"), Path.GetFileName(file.FileName));
                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), Path.GetFileName(file.FileName));
                
                file.SaveAs(tempPath);

                try
                {
                    using (ExifReader reader = new ExifReader(tempPath))
                    {
                        Double[] GpsLongArray;
                        Double[] GpsLatArray;
                        Double GpsLongDouble;
                        Double GpsLatDouble;
                        DateTime datePictureTaken;

                        if (reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out GpsLongArray) 
                            && reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out GpsLatArray))
                        {
                            if (reader.GetTagValue<DateTime>
                            (ExifTags.DateTimeDigitized, out datePictureTaken))
                            {
                                GpsLongDouble = GpsLongArray[0] + GpsLongArray[1] / 60 + GpsLongArray[2] / 3600;
                                GpsLatDouble = GpsLatArray[0] + GpsLatArray[1] / 60 + GpsLatArray[2] / 3600;
   
                                var record = from r in _db.Photos
                                             where r.Longitude == GpsLongDouble
                                             && r.Latitude == GpsLatDouble
                                             && r.PictureTaken == datePictureTaken
                                             orderby r.Id
                                             select r.Id;

                                if(record.Count() == 0)
                                { 
                                    _db.Photos.Add(new Photo
                                    {
                                        UserId = WebSecurity.GetUserId(User.Identity.Name),
                                        imgPath = path,
                                        Longitude = GpsLongDouble,
                                        Latitude = GpsLatDouble,
                                        PictureTaken = datePictureTaken
                                    });

                                    _db.SaveChanges();

                                    System.IO.File.Move(tempPath, path);
                                    ViewBag.Message = "Photo uploaded successfully";
                               }
                               else
                               {
                                   throw new Exception("Photo already exists!");
                               }
                            }
                        }
                        else
                        {
                            throw new Exception("The Photo does not contain GPS info!");
                        }
                    }
                }
                catch (Exception)
                {
                    System.IO.File.Delete(tempPath);
                    ViewBag.Message = "The Photo does not contain GPS info!";
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }

    }
}
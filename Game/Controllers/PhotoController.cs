using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExifLib;
using Game.Models;
using WebMatrix.WebData;
using PagedList;
using System.Drawing;
using Goheer.EXIF;
using System.Drawing.Imaging;

namespace Game.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private GameAppDb _db = new GameAppDb();

        public ActionResult Index(int page = 1, string error = "")
        {
            var model =
                _db.Photos
                    .OrderByDescending(r => r.Id)
                    .Select(r => new PhotoListViewModel
                    {
                        Id = r.Id,
                        imgPath = "~/Images/uploads/" + r.imgName,
                    }).ToPagedList(page, 10);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Photos", model);
            }
            ViewBag.head = "All Pictures";
            ViewBag.Message = error;
            return View(model);
        }

        public ActionResult PlayedPictures(int page = 1, string error = "")
        {
            int UserID =  WebSecurity.CurrentUserId;

            var model =
                _db.Photos
                .Join(_db.Scores,
                    photo => photo.Id,
                    Score => Score.ImgId,
                    (photo, Score) => new { photo, Score })
                .Where(m => m.photo.UserId == UserID)
                .OrderBy(m => m.photo.Id)
                .Select(m => new PhotoListViewModel
                {
                    Id = m.photo.Id,
                    imgPath = "~/Images/uploads/" + m.photo.imgName,
                }).ToPagedList(page, 10);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Photos", model);
            }
            ViewBag.head = "Played Pictures";
            ViewBag.Message = error;
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var Photo = _db.Photos.OrderByDescending(p => p.Id).FirstOrDefault();
                string oldFileName = Path.GetFileName(file.FileName);
                string[] name = oldFileName.Split('.');
                int id = (Photo == null) ? 0 : Photo.Id;                   
                string newFileName = (Convert.ToInt16(id) + 1).ToString() + "." + name[1];
                string tempPath = Path.Combine(Server.MapPath("~/Images/uploads/temp"), newFileName);
                string path = Path.Combine(Server.MapPath("~/Images/uploads"), newFileName);
                file.SaveAs(tempPath);
                
                try
                {
                    using (ExifReader reader = new ExifReader(tempPath))
                    {
                        Double[] GpsLongArray;
                        Double[] GpsLatArray;
                        UInt16 Orientation;
                        Double GpsLongDouble;
                        Double GpsLatDouble;
                        DateTime datePictureTaken;

                        if (reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out GpsLongArray)
                            && reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out GpsLatArray)
                            && reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out datePictureTaken)
                            && reader.GetTagValue<UInt16>(ExifTags.Orientation, out Orientation))
                        {                                
                            GpsLongDouble = GpsLongArray[0] + GpsLongArray[1] / 60 + GpsLongArray[2] / 3600;
                            GpsLatDouble = GpsLatArray[0] + GpsLatArray[1] / 60 + GpsLatArray[2] / 3600;

                            var record = from r in _db.Photos
                                            where r.Longitude == GpsLongDouble
                                            && r.Latitude == GpsLatDouble
                                            && r.PictureTaken == datePictureTaken
                                            orderby r.Id
                                            select r.Id;
                                    
                            if (record.Count() == 0) {
                                _db.Photos.Add(new Photo {
                                    UserId = WebSecurity.CurrentUserId,
                                    imgName = newFileName,
                                    imgPath = path,
                                    Longitude = GpsLongDouble,
                                    Latitude = GpsLatDouble,
                                    Orientation = Orientation,
                                    PictureTaken = datePictureTaken
                                });

                                reader.Dispose();
                                System.IO.File.Move(tempPath, path);
                                Photo.Rotate(path);
                                _db.SaveChanges();                                   
                                ViewBag.Message = "Photo uploaded successfully";
                            }
                            else {
                                reader.Dispose();
                                System.IO.File.Delete(tempPath);
                                ViewBag.Message = "Photo already exists!";
                            }
                        }
                        else {
                            reader.Dispose();
                            System.IO.File.Delete(tempPath);
                            ViewBag.Message = "The Photo does not contain GPS info!";
                        }
                    }
                }
                catch (Exception ex) {
                    System.IO.File.Delete(tempPath);
                    ViewBag.Message = ex.Message.ToString();
                }
            }
            else {
                ViewBag.Message = "You have not specified a file.";
            }
            return RedirectToAction("Index", "Photo", new { error = ViewBag.Message });
        } 
    }
}
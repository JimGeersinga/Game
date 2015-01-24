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
                        imgName = r.imgName,
                        imgPath = "~/Images/uploads/" + r.imgName,
                        Orientation = r.Orientation
                    }).ToPagedList(page, 10);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Photos", model);
            }
            ViewBag.Message = error;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string tempPath = Path.Combine(Server.MapPath("~/Images/uploads/temp"), Path.GetFileName(file.FileName));
                string path = Path.Combine(Server.MapPath("~/Images/uploads"), Path.GetFileName(file.FileName));
                
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
                            String Oname;

                            reader.GetTagValue<UInt16>(ExifTags.Orientation, out Orientation);

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
                                    if (Orientation == 1)
                                    {
                                        Oname = "Landscape";
                                    }else{
                                        Oname = "Portrait";   
                                    }
                                    if (record.Count() == 0)
                                    {
                                        _db.Photos.Add(new Photo
                                        {
                                            UserId = WebSecurity.GetUserId(User.Identity.Name),
                                            imgName = file.FileName,
                                            imgPath = path,
                                            Longitude = GpsLongDouble,
                                            Latitude = GpsLatDouble,
                                            Orientation = Oname,
                                            PictureTaken = datePictureTaken
                                        });

                                        _db.SaveChanges();

                                        reader.Dispose();
                                        System.IO.File.Move(tempPath, path);
                                        ViewBag.Message = "Photo uploaded successfully";

                                    }
                                    else
                                    {
                                        reader.Dispose();
                                        System.IO.File.Delete(tempPath);
                                        ViewBag.Message = "Photo already exists!";
                                    }
                                }
                            }
                            else
                            {
                                reader.Dispose();
                                System.IO.File.Delete(tempPath);
                                ViewBag.Message = "The Photo does not contain GPS info!";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.Delete(tempPath);
                        ViewBag.Message = ex.Message.ToString();
                    }
                
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            return RedirectToAction("Index", "Photo", new { error = ViewBag.Message });
        }

    }
}
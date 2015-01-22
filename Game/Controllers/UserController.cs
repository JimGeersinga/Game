using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Game.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        GameAppDb _db = new GameAppDb();

        public ActionResult AutoComplete(string term)
        {
            var model =
                _db.UserProfiles
                    .Where(r => r.UserName.StartsWith(term))
                    .Take(10)
                    .Select(r => new
                    {
                        label = r.UserName
                    });
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index(string searchTerm = null, int page = 1)
        {
            var model =
                _db.UserProfiles
                    .OrderByDescending(r => r.UserPoints)
                    .Where(r => searchTerm == null || r.UserName.StartsWith(searchTerm))
                    .Select(r => new UserListViewModel
                    {
                        UserId = r.UserId,
                        UserName = r.UserName,
                        UserPoints = r.UserPoints
                    }).ToPagedList(page, 10);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Users", model);
            }
            return View(model);
        }
    }
}

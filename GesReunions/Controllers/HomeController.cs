using GesReunions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;


namespace GesReunions.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        private GestionReunionsEntities db = new GestionReunionsEntities();
        public JsonResult GetEvents()
        {
            var events = db.Events.ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [HttpPost]
        public JsonResult SaveEvent(Events e)
        {
            var status = false;
            if (e.EventID > 0)
            {
                var v = db.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                if(v != null)
                {
                    v.Subject = e.Subject;
                    v.Start = e.Start;
                    v.Endd = e.Endd;
                    v.Description = e.Description;
                    v.IsFullDay = e.IsFullDay;
                    v.ThemeColor = e.ThemeColor;
                }
            }
            else
            {
                db.Events.Add(e);
            }
            db.SaveChanges();
            status = true;
            return new JsonResult
            {
                Data = new { status = status } };
        }
        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            var v = db.Events.Where(a => a.EventID == eventID).FirstOrDefault();
            if(v != null)
            {
                db.Events.Remove(v);
                db.SaveChanges();
                status = true;
            }
            return new JsonResult
            {
                Data = new { status = status }
            };


        }
      

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
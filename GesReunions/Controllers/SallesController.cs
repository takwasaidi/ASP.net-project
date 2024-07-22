using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GesReunions.Models;

namespace GesReunions.Controllers
{
    public class SallesController : Controller
    {
        private GestionReunionsEntities db = new GestionReunionsEntities();

        // GET: Salles
        public ActionResult Index()
        {
            return View(db.Salles.ToList());
        }

        // GET: Salles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salles salles = db.Salles.Find(id);
            if (salles == null)
            {
                return HttpNotFound();
            }
            return View(salles);
        }

        // GET: Salles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Salles/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nom,capacite")] Salles salles)
        {
            if (ModelState.IsValid)
            {
                db.Salles.Add(salles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(salles);
        }

        // GET: Salles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salles salles = db.Salles.Find(id);
            if (salles == null)
            {
                return HttpNotFound();
            }
            return View(salles);
        }

        // POST: Salles/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nom,capacite")] Salles salles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salles);
        }

        // GET: Salles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Salles salles = db.Salles.Find(id);
            if (salles == null)
            {
                return HttpNotFound();
            }
            return View(salles);
        }

        // POST: Salles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Salles salles = db.Salles.Find(id);
            db.Salles.Remove(salles);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

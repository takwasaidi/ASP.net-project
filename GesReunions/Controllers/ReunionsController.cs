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
    public class ReunionsController : Controller
    {
        private GestionReunionsEntities db = new GestionReunionsEntities();

        // GET: Reunions
        public ActionResult Index()
        {
            //var reunions = db.Reunions.Include(r => r.Salles);
            //return View(reunions.ToList());
            var reunions = db.Reunions
                    .Include(r => r.Salles)
                    .Include(r => r.reunion_utilisateur.Select(ru => ru.Utilisateurs))
                    .ToList();
            return View(reunions);
        }

        // GET: Reunions/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reunions reunions = db.Reunions.Find(id);
            if (reunions == null)
            {
                return HttpNotFound();
            }
            return View(reunions);
        }

        // GET: Reunions/Create
        public ActionResult Create()
        {    // hedhy ken bech nkhdem methode select 
            //ViewBag.Lieu = new SelectList(db.Salles, "nom", "nom");
            //ViewBag.UtilisateursDisponibles = new MultiSelectList(db.Utilisateurs, "Id", "Nom");
            //return View();
            ViewBag.Lieu = new SelectList(db.Salles, "nom", "nom");
            ViewBag.UtilisateursDisponibles = db.Utilisateurs.ToList();
            return View();
        }

        // POST: Reunions/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Titre,Description,Date,Heures,Lieu")] Reunions reunions, int[] utilisateursSelectionnes)
        {
            if (ModelState.IsValid)
            {
                if (utilisateursSelectionnes != null)
                {
                    foreach (var utilisateurId in utilisateursSelectionnes)
                    {
                        var utilisateur = db.Utilisateurs.Find(utilisateurId);
                        if (utilisateur != null)
                        {
                            var reunionUtilisateur = new reunion_utilisateur
                            {
                                Reunions = reunions,
                                Utilisateurs = utilisateur
                            };
                            db.reunion_utilisateur.Add(reunionUtilisateur);
                        }
                    }
                }

                db.Reunions.Add(reunions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Lieu = new SelectList(db.Salles, "nom", "nom", reunions.Lieu);
            ViewBag.UtilisateursDisponibles = db.Utilisateurs.ToList();
            return View(reunions);
        }

        // GET: Reunions/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reunions reunions = db.Reunions.Find(id);
            if (reunions == null)
            {
                return HttpNotFound();
            }


            // Charger les utilisateurs disponibles
            var utilisateursDisponibles = db.Utilisateurs.ToList();
            ViewBag.UtilisateursDisponibles = utilisateursDisponibles;

            // Charger les utilisateurs sélectionnés pour cette réunion
            var utilisateursSelectionnes = reunions.reunion_utilisateur.Select(r => r.utilisateurId).ToList();
            ViewBag.SelectedUtilisateurs = utilisateursSelectionnes;

            // Charger les lieux pour le DropDownList
            ViewBag.Lieu = new SelectList(db.Salles, "nom", "nom", reunions.Lieu);

            return View(reunions);
        }

        // POST: Reunions/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Titre,Description,Date,Heures,Lieu")] Reunions reunions , string[] utilisateursSelectionnes)
        {

            if (ModelState.IsValid)
            {
                db.Entry(reunions).State = EntityState.Modified;

                // Supprimer les utilisateurs existants associés à la réunion
                var reunionUtilisateurs = db.reunion_utilisateur.Where(r => r.ReunionTitre == reunions.Titre).ToList();
                db.reunion_utilisateur.RemoveRange(reunionUtilisateurs);

                // Ajouter les nouveaux utilisateurs sélectionnés
                if (utilisateursSelectionnes != null)
                {
                    foreach (var utilisateurId in utilisateursSelectionnes)
                    {
                        var reunionUtilisateur = new reunion_utilisateur
                        {
                            utilisateurId = int.Parse(utilisateurId),
                            ReunionTitre = reunions.Titre
                        };
                        db.reunion_utilisateur.Add(reunionUtilisateur);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Lieu = new SelectList(db.Salles, "nom", "nom", reunions.Lieu);
            ViewBag.UtilisateursDisponibles = db.Utilisateurs.ToList();
            ViewBag.SelectedUtilisateurs = utilisateursSelectionnes;
            return View(reunions);
        }

        // GET: Reunions/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reunions reunions = db.Reunions.Find(id);
            if (reunions == null)
            {
                return HttpNotFound();
            }
            return View(reunions);
        }

        // POST: Reunions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Reunions reunions = db.Reunions.Find(id);
            // Supprimer les enregistrements associés dans reunion_utilisateur
            var utilisateursAssocies = db.reunion_utilisateur.Where(r => r.ReunionTitre == id);
            db.reunion_utilisateur.RemoveRange(utilisateursAssocies);

            db.Reunions.Remove(reunions);
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

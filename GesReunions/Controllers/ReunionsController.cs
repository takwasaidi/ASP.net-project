﻿using System;
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
        public ActionResult calendrier()
        {
            ViewBag.Lieu = new SelectList(db.Salles, "nom", "nom");
            ViewBag.UtilisateursDisponibles = db.Utilisateurs.ToList();
            return View();
        }
        // Action pour récupérer les réunions en format JSON
        public JsonResult GetReunions()
        {
            // Charger les réunions depuis la base de données
            var reunions = db.Reunions.ToList();
         
            // Transformer les données en mémoire
            var result = reunions.Select(r => new
            {
                Titre = r.Titre,
                Description = r.Description,
                Date = r.Date.ToString("yyyy-MM-ddTHH:mm:ss"), // Format ISO 8601 pour les dates
                End = r.Date.AddHours(r.Heures).ToString("yyyy-MM-ddTHH:mm:ss"), // Calcul de la fin
                ThemeColor = "#378006", // Couleur par défaut, ajustez si nécessaire
                IsFullDay = false, // Ajustez selon la logique de votre application
                Participants = r.reunion_utilisateur.Select(ru => ru.Utilisateurs.Nom).ToList(),
                Lieu = r.Lieu // Ajouter le lieu
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveMeeting(SaveMeetingViewModel model)
        {
            var status = false;
            Console.WriteLine("Titre: " + model.Titre);
            Console.WriteLine("Participants: " + string.Join(", ", model.Participants));
            if (!string.IsNullOrEmpty(model.Titre))
            {
                var meeting = db.Reunions
                    .Include(r => r.reunion_utilisateur) // Inclure les participants existants
                    .FirstOrDefault(a => a.Titre == model.Titre);

                if (meeting != null)
                {
                    // Mise à jour
                    meeting.Description = model.Description;
                    meeting.Date = model.Date;
                    meeting.Heures = model.Heures;
                    meeting.Lieu = model.Lieu;


                    // Supprimer les anciens participants
                    var existingParticipants = meeting.reunion_utilisateur.ToList();
                    db.reunion_utilisateur.RemoveRange(existingParticipants);

                    // Ajouter les nouveaux participants
                    foreach (var participant in model.Participants)
                    {
                        var utilisateur = db.Utilisateurs.FirstOrDefault(u => u.Nom == participant);
                        if (utilisateur != null)
                        {
                            meeting.reunion_utilisateur.Add(new reunion_utilisateur
                            {
                                utilisateurId = utilisateur.Id,
                                ReunionTitre = meeting.Titre
                            });
                        }
                    }
                }
                else
                {
                    // Nouvelle réunion
                    meeting = new Reunions
                    {
                        Titre = model.Titre,
                        Description = model.Description,
                        Date = model.Date,
                        Heures = model.Heures,
                        Lieu = model.Lieu,
                        reunion_utilisateur = new List<reunion_utilisateur>()
                    };

                    foreach (var participant in model.Participants)
                    {
                        var utilisateur = db.Utilisateurs.FirstOrDefault(u => u.Nom == participant);
                        if (utilisateur != null)
                        {
                            meeting.reunion_utilisateur.Add(new reunion_utilisateur
                            {
                                utilisateurId = utilisateur.Id,
                                ReunionTitre = meeting.Titre
                            });
                        }
                    }
                    db.Reunions.Add(meeting);
                }

                db.SaveChanges();
                status = true;
            }

            return Json(new { status = status });
        }


        // POST: Reunions/DeleteMeeting
        [HttpPost]
        public JsonResult DeleteMeeting(string titre)
        {
            var status = false;
           
            
            var v = db.Reunions.Where(a => a.Titre == titre).FirstOrDefault();
            if (v != null)
            {
                var utilisateursAssocies = db.reunion_utilisateur.Where(r => r.ReunionTitre == titre);
                db.reunion_utilisateur.RemoveRange(utilisateursAssocies);
                db.Reunions.Remove(v);
                db.SaveChanges();
                status = true;
            }

            return new JsonResult { Data = new { status = status } };
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

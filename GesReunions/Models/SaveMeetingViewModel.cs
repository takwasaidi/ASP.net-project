using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GesReunions.Models
{
  
        public class SaveMeetingViewModel
        {
            public string Titre { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public int Heures { get; set; }
            public string Lieu { get; set; }
            public List<string> Participants { get; set; } // Liste des participants
        }

    
}
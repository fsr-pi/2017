using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Firma.Mvc.Models
{
    public partial class Dokument
    {
        public Dokument()
        {
            Stavka = new HashSet<Stavka>();
        }

        public int IdDokumenta { get; set; }
        [Display(Name = "Vrsta dokumenta")]
        public string VrDokumenta { get; set; }
        [Display(Name = "Broj dokumenta")]
        public int BrDokumenta { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy.}", ApplyFormatInEditMode = true)]
        [Display(Name = "Datum")]
        public DateTime DatDokumenta { get; set; }
        [Display(Name = "Partner")]        
        public int IdPartnera { get; set; }
        public int? IdPrethDokumenta { get; set; }
        public decimal PostoPorez { get; set; }
        public decimal IznosDokumenta { get; set; }

        public virtual ICollection<Stavka> Stavka { get; set; }
        public virtual Partner IdPartneraNavigation { get; set; }
        public virtual Dokument IdPrethDokumentaNavigation { get; set; }
        public virtual ICollection<Dokument> InverseIdPrethDokumentaNavigation { get; set; }
    }
}

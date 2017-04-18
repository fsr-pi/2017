using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Firma.Mvc.Models
{
    public partial class Drzava
    {
        public Drzava()
        {
            Mjesto = new HashSet<Mjesto>();
        }

        [Required(ErrorMessage="Oznaka države je obvezno polje")]
        [Display(Name="Oznaka države")]
        public string OznDrzave { get; set; }
        [Required(ErrorMessage = "Naziv države je obvezno polje")]
        [Display(Name = "Naziv države")]
        public string NazDrzave { get; set; }
        [Display(Name = "ISO3 oznaka")]
        [MaxLength(3, ErrorMessage="ISO3 oznaka može sadržavati maksimalno 3 slova")]
        public string Iso3drzave { get; set; }
        [Display(Name = "Šifra države")]
        public int? SifDrzave { get; set; }

        public virtual ICollection<Mjesto> Mjesto { get; set; }
    }
}

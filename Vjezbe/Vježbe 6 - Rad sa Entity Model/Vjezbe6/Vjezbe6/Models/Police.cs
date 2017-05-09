using System;
using System.Collections.Generic;

namespace Vjezbe6.Models
{
    public partial class Police
    {
        public int Id { get; set; }
        public string VrstaPolice { get; set; }
        public int BrojPolice { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumKraja { get; set; }
        public DateTime? DatumIzdavanja { get; set; }
        public decimal Premija { get; set; }
        public decimal Porez { get; set; }
        public int Pribavitelj { get; set; }
        public string Posrednik { get; set; }

        public virtual Pribavitelj PribaviteljNavigation { get; set; }
    }
}

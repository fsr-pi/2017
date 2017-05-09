using System;
using System.Collections.Generic;

namespace Vjezbe6.Models
{
    public partial class Pribavitelj
    {
        public Pribavitelj()
        {
            Police = new HashSet<Police>();
        }

        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public int? Godine { get; set; }

        public virtual ICollection<Police> Police { get; set; }
    }
}

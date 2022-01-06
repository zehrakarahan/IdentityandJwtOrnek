using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.Model
{
    public class Firma
    {
        [Key]
        public int Id { get; set; }

        public string FirmaAdi { get; set; }

        public virtual IEnumerable<Sube> Sube { get; set; }

    }
}

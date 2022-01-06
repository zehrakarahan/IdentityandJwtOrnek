using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.Model
{
    public class Sube
    {
        [Key]
        public int Id { get; set; }

        public string SubeAdi { get; set; }


        public int FirmaId { get; set; }
    }
}

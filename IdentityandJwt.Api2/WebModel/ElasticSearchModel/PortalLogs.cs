using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.WebModel.ElasticSearchModel
{
    public class PortalLogs
    {
        public PortalLogs()
        {
            GuidNoElastic = Guid.NewGuid();
        }
        public Guid? GuidNoElastic { get; set; }
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string UserName { get; set; }

        public List<string> ErrorDescription { get; set; }

        public DateTime Tarih { get; set; }

    }
}

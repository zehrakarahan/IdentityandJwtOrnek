using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.Services
{
    public interface ILogServices
    {
        public void LogAdd(string ControllerName,string ActionName,string UserName,List<string> ErrorDescription,DateTime Tarih);
    }
}

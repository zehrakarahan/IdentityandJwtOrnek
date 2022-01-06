using IdentityandJwt.Api2.Model.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityandJwt.Api2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HomeController : ControllerBase
    {
        public AppIdentityDbContext _context;
        public HomeController(AppIdentityDbContext context)
        {
            this._context = context;
        }
        [HttpGet]
        [Route("CompanyAll")]
        public IActionResult Index()
        {
            var liste = _context.Firma.ToList();
            if (liste!=null)
            {
                return Ok(liste);
            }
            else
            {
                return Ok(null);
            }
        }

    }
}

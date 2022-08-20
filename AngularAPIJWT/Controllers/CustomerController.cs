using AngularAPIJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AngularAPIJWT.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly AngularJWTContext _context;

        public CustomerController(AngularJWTContext context)
        {
             _context = context;
        }

        [HttpGet]
        public IEnumerable<TblCustomer> Get()
        {
            return _context.TblCustomers.ToList();
        }
    }
}

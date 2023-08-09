using Microsoft.AspNetCore.Mvc;
using ModelFactory;
using ModelFactory.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ServerSideKeyVaultSettings _settings;
        protected readonly DebateDbContext _context;
        public BaseController(DebateDbContext context, ServerSideKeyVaultSettings settings)
        {
            _context = context;
            _settings = settings;
        }
    }
}

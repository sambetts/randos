using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelFactory;
using ModelFactory.Config;
using Models;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, DebateDbContext context, ServerSideKeyVaultSettings settings) : base(context, settings)
        {
            this._logger = logger;
        }

        // POST: /Users/EnsureUser
        [HttpPost("[action]")]
        public async Task<Models.DebateUser> EnsureUser()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            _logger.LogInformation($"claims: {claims}", claims);

            var emailsClaim = claims.Where(c => c.Type == "emails").FirstOrDefault();
            var displayNameClaim = claims.Where(c => c.Type == "emails").FirstOrDefault();

            var user = await _context.users.Where(u => u.email == emailsClaim.Value).SingleOrDefaultAsync();

            if (user == null)
            {
                user = new ModelFactory.DebateUser { display_name = displayNameClaim.Value, email = emailsClaim.Value };
                _context.users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user.ToModel();
        }
    }
}

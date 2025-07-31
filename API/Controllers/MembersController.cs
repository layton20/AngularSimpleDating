using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly AppDbContext __Context;

        public MembersController(AppDbContext context)
        {
            this.__Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembersAsync()
        {
            List<AppUser> _Members = await __Context.Users.ToListAsync();

            return _Members;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMemberAsync(string id)
        {
            AppUser? _Member = await __Context.Users.FindAsync(id);

            if (_Member == null)
            {
                return NotFound();
            }

            return _Member;
        }
    }
}

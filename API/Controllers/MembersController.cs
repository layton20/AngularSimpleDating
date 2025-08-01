using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class MembersController : BaseApiController
    {
        private readonly AppDbContext __Context;

        public MembersController(AppDbContext context)
        {
            __Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembersAsync()
        {
            List<AppUser> _Members = await __Context.Users.ToListAsync();

            return _Members;
        }

        [Authorize]
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

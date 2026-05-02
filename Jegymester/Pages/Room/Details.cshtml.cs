using JegyMester.DataContext.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoomEntity = JegyMester.DataContext.Entities.Room;

namespace JegyMester.Pages.Room
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public DetailsModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public RoomEntity Room { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);

            if (room is not null)
            {
                Room = room;

                return Page();
            }

            return NotFound();
        }
    }
}

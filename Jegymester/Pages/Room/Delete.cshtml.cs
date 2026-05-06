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
    public class DeleteModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public DeleteModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RoomEntity Room { get; set; } = default!;
        public string? DeleteErrorMessage { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms
                .Include(r => r.Screenings)
                .Include(r => r.Cinema)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            // Ha van vetítés → nem törölhető
            if (room.Screenings.Any())
            {
                DeleteErrorMessage = "A terem nem törölhető, mert léteznek hozzá vetítések.";
                Room = room; // fontos: visszatöltjük, hogy a Delete oldal meg tudja jeleníteni
                return Page(); // nem redirect
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}

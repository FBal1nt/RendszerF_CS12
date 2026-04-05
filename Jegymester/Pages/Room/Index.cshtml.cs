using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using JegyMester.DataContext.Context;
using RoomEntity = JegyMester.DataContext.Entities.Room;

namespace JegyMester.Pages.Room
{
    public class IndexModel : PageModel
    {
        private readonly JegyMester.DataContext.Context.JegyMesterDbContext _context;

        public IndexModel(JegyMester.DataContext.Context.JegyMesterDbContext context)
        {
            _context = context;
        }

        public IList<RoomEntity> Room { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Room = await _context.Rooms
                .Include(r => r.Cinema).ToListAsync();
        }
    }
}

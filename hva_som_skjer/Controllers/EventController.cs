using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hva_som_skjer.Data;
using hva_som_skjer.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace hva_som_skjer.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EventController(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: Event
        public async Task<IActionResult> Index()
        {
            var events = await _db.Events.ToListAsync();

            return View(events.OrderBy(x => x.StartDate));
        }

        // GET: Event
        public async Task<IActionResult> Calendar()
        {
            var events = await _db.Events.ToListAsync();

            return View(events.OrderBy(x => x.StartDate));
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,StartDate,StartTime,EndTime,Location")] Event @event, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = string.Format(@"{0}.png", Guid.NewGuid());
                    string filePath = "/images/events/"+filename;

                    var localPath = Directory.GetCurrentDirectory();
                    localPath += "/wwwroot/" + filePath;

                    if (file.Length > 0)
                    {
                        using (var stream = new FileStream(localPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    @event.ImagePath = filePath;
                }
                else 
                {
                    @event.ImagePath = "/images/events/EventDefault.PNG";
                }
                
                _db.Add(@event);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,StartDate,StartTime,EndTime,Location,ImagePath")] Event @event, IFormFile file)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = string.Format(@"{0}.png", Guid.NewGuid());
                    string filePath = "/images/events/"+filename;

                    var localPath = Directory.GetCurrentDirectory();
                    localPath += "/wwwroot/" + filePath;

                    if (file.Length > 0)
                    {
                        using (var stream = new FileStream(localPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    @event.ImagePath = filePath;

                    // TODO: Delete old image
                    // TODO: Virker ikke. File er alltid null
                }

                try
                {
                    _db.Update(@event);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);
            _db.Events.Remove(@event);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _db.Events.Any(e => e.Id == id);
        }
    }
}

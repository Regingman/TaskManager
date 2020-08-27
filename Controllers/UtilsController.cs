using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

namespace TaskManager.Controllers
{
    public class UtilsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtilsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Utils
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utils.ToListAsync());
        }

        // GET: Utils/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utils = await _context.Utils
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utils == null)
            {
                return NotFound();
            }

            return View(utils);
        }

        // GET: Utils/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,firstParameter,secondParameter,thirdParameter,CreateDate,UpdateDate")] Utils utils)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utils);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utils);
        }

        // GET: Utils/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utils = await _context.Utils.FindAsync(id);
            if (utils == null)
            {
                return NotFound();
            }
            return View(utils);
        }

        // POST: Utils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,firstParameter,secondParameter,thirdParameter,CreateDate,UpdateDate")] Utils utils)
        {
            if (id != utils.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utils);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilsExists(utils.Id))
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
            return View(utils);
        }

        // GET: Utils/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utils = await _context.Utils
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utils == null)
            {
                return NotFound();
            }

            return View(utils);
        }

        // POST: Utils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utils = await _context.Utils.FindAsync(id);
            _context.Utils.Remove(utils);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilsExists(int id)
        {
            return _context.Utils.Any(e => e.Id == id);
        }
    }
}

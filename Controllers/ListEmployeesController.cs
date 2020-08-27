using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

namespace TaskManager.Controllers
{
    [Authorize(Roles = "PROJECT MANAGER")]
    public class ListEmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListEmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ListEmployees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ListEmployees.Include(l => l.ApplicationUser).Include(l => l.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ListEmployees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listEmployee = await _context.ListEmployees
                .Include(l => l.ApplicationUser)
                .Include(l => l.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listEmployee == null)
            {
                return NotFound();
            }

            return View(listEmployee);
        }

        // GET: ListEmployees/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id");
            return View();
        }

        // POST: ListEmployees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ProjectId")] ListEmployee listEmployee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(listEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", listEmployee.ApplicationUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", listEmployee.ProjectId);
            return View(listEmployee);
        }

        // GET: ListEmployees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listEmployee = await _context.ListEmployees.FindAsync(id);
            if (listEmployee == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", listEmployee.ApplicationUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", listEmployee.ProjectId);
            return View(listEmployee);
        }

        // POST: ListEmployees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,ProjectId")] ListEmployee listEmployee)
        {
            if (id != listEmployee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listEmployee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListEmployeeExists(listEmployee.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", listEmployee.ApplicationUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Id", listEmployee.ProjectId);
            return View(listEmployee);
        }

        // GET: ListEmployees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listEmployee = await _context.ListEmployees
                .Include(l => l.ApplicationUser)
                .Include(l => l.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listEmployee == null)
            {
                return NotFound();
            }

            return View(listEmployee);
        }

        // POST: ListEmployees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listEmployee = await _context.ListEmployees.FindAsync(id);
            _context.ListEmployees.Remove(listEmployee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListEmployeeExists(int id)
        {
            return _context.ListEmployees.Any(e => e.Id == id);
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class AppendixesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;

        public AppendixesController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Appendixes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Appendices.ToListAsync());
        }

        // GET: Appendixes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appendix = await _context.Appendices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appendix == null)
            {
                return NotFound();
            }

            return View(appendix);
        }

        // GET: Appendixes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Appendixes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FileName")] AppendixViewModel appendix)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                if (appendix.FileName != null)
                {
                    string uploadPath = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    fileName = Guid.NewGuid().ToString() + "_" + appendix.FileName.FileName;
                    string FilePath = Path.Combine(uploadPath, fileName);
                    appendix.FileName.CopyTo(new FileStream(FilePath, FileMode.Create));
                    
                }
                Appendix model = new Appendix();
                model.FileName = fileName;
                model.Name = appendix.Name;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appendix);
        }

        // GET: Appendixes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appendix = await _context.Appendices.FindAsync(id);
            if (appendix == null)
            {
                return NotFound();
            }
            return View(appendix);
        }

        // POST: Appendixes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FileName")] Appendix appendix)
        {
            if (id != appendix.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appendix);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppendixExists(appendix.Id))
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
            return View(appendix);
        }

        // GET: Appendixes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appendix = await _context.Appendices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appendix == null)
            {
                return NotFound();
            }

            return View(appendix);
        }

        // POST: Appendixes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appendix = await _context.Appendices.FindAsync(id);
            _context.Appendices.Remove(appendix);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppendixExists(int id)
        {
            return _context.Appendices.Any(e => e.Id == id);
        }
    }
}

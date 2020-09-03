using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize(Roles = "USER")]
    public class TasksController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment hostingEnvironment;



        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tasks.Include(t => t.ApplicationUser).Include(t => t.Module).Include(t => t.Module.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> UserProject()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var model = _context.ListEmployees
                .Include(e => e.Project)
                .Include(e => e.ApplicationUser)
                .Where(e => e.ApplicationUserId == user.Id)
                .Select(c => c.Project)
                .ToList();
            return View(model);
        }

        public async Task<IActionResult> UserModule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userList = _context.ListEmployees
                .Include(e => e.Project)
                .Include(e => e.ApplicationUser)
                .Where(e => e.ApplicationUserId == user.Id)
                .Select(c => c.Project)
                .ToList();

            var model = _context.Modules
                .Include(e => e.Project)
                .Where(e => e.ProjectId == id)
                .ToList();

            if (userList.Count == 0)
            {
                return RedirectToAction("UserProject", "Tasks");
            }
            return View(model);
        }

        public async Task<IActionResult> TaskForModule(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var model = _context.Tasks
                .Include(e => e.Module)
                .Include(e => e.ApplicationUser)
                .Include(e => e.Status)
                .Include(e => e.Appendix)
                .Where(e => e.ApplicationUserId == user.Id && e.ModuleId == id)
                .ToList();

            var userList = _context.ListEmployees
               .Include(e => e.Project)
               .Include(e => e.ApplicationUser)
               .Where(e => e.ApplicationUserId == user.Id)
               .Select(c => c.Project)
               .ToList();
            if (userList.Count == 0)
            {
                return RedirectToAction("UserProject", "Tasks");
            }
            var modules = await _context.Modules
               .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.ModuleId = id;
            ViewBag.ModuleName = modules.Name;
            ViewBag.ProjectId = modules.ProjectId;
            return View(model);
        }

        public class TempModelForTask
        {
            public int Moduleid;
            public List<TaskManager.Data.Task> tasks;
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.ApplicationUser)
                .Include(t => t.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        public async Task<IActionResult> MyTaskDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.ApplicationUser)
                .Include(t => t.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        public IActionResult CreateTaskForModule(int? id)
        {
            if (id != null)
            {
                ViewBag.ModuleId = id;
            }
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTaskForModule(int id, [Bind("Id,Name,ModuleId,ApplicationUserId,DateOfBirth")] TaskManager.Data.Task task)
        {
            task.ModuleId = id;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            task.ApplicationUserId = user.Id;
            task.Id = 0;
            task.StatusId = 1;
            task.CreateDate = DateTime.Now;
            task.UpdateDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("TaskForModule", new { id = id });
            }
            return View(task);
        }


        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Id");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ModuleId,ApplicationUserId,DateOfBirth")] TaskManager.Data.Task task)
        {
            if (ModelState.IsValid)
            {

                task.CreateDate = DateTime.Now;
                task.UpdateDate = DateTime.Now;
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", task.ApplicationUserId);
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Id", task.ModuleId);
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", task.ApplicationUserId);
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Id", task.ModuleId);
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ModuleId,ApplicationUserId,DateOfBirth")] TaskManager.Data.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    task.UpdateDate = DateTime.Now;
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", task.ApplicationUserId);
            ViewData["ModuleId"] = new SelectList(_context.Modules, "Id", "Id", task.ModuleId);
            return View(task);
        }


        public async Task<IActionResult> EditTaskForModule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
                return View(task);
        }

        public async Task<IActionResult> TaskComplete(int id)
        {
            var task = _context.Tasks.Find(id);
            task.StatusId = 2;
            _context.Update(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("TaskForModule", new { id = task.ModuleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTaskForModule(int id, [Bind("Id,Name,Note,Cause,ModuleId,ApplicationUserId,DateOfBirth")] TaskManager.Data.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            var taskTemp = await _context.Tasks
                .Include(t => t.ApplicationUser)
                .Include(t => t.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            taskTemp.DateOfBirth = task.DateOfBirth;
            taskTemp.Name = task.Name;
            taskTemp.Appendix = task.Appendix;
            taskTemp.Note = task.Note;
            taskTemp.UpdateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskTemp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("TaskForModule", new { id = taskTemp.ModuleId });
            }
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.ApplicationUser)
                .Include(t => t.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteTaskForModule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.ApplicationUser)
                .Include(t => t.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("DeleteTaskForModule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTaskForModule(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            var tempId = task.ModuleId;
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("TaskForModule", new { id = tempId });
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DetailsAppendix(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var task = _context.Tasks.FirstOrDefault(e => e.AppendixId == id);
            var appendix = await _context.Appendices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appendix == null)
            {
                return NotFound();
            }
            ViewBag.Id = task.ModuleId;
            return View(appendix);
        }

        // GET: Appendixes/Create
        public IActionResult CreateAppendix(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        // POST: Appendixes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppendix(int id, [Bind("Id,Name,FileName")] AppendixViewModel appendix)
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
                //model.Id = 0;
                model.FileName = fileName;
                model.Name = appendix.Name;
                _context.Add(model);
                var task = _context.Tasks.Find(id);
                await _context.SaveChangesAsync();
                task.AppendixId = model.Id;
                _context.Update(task);



                await _context.SaveChangesAsync();
                return RedirectToAction("TaskForModule", new { id = task.ModuleId });
            }
            return View(appendix);
        }

        // GET: Appendixes/Edit/5
        public async Task<IActionResult> EditAppendix(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var task = _context.Tasks.FirstOrDefault(e => e.AppendixId == id);
            var appendix = await _context.Appendices.FindAsync(id);
            AppendixViewModel returnModel = new AppendixViewModel();
            returnModel.Id = appendix.Id;
            returnModel.Name = appendix.Name;
            ViewBag.Id = task.ModuleId;
            if (returnModel == null)
            {
                return NotFound();
            }
            return View(returnModel);
        }

        // POST: Appendixes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppendix([Bind("Id,Name,FileName")] AppendixViewModel appendix)
        {
            if (ModelState.IsValid)
            {
                var model = _context.Appendices.Find(appendix.Id);

                try
                {
                    string fileName = "";
                    if (appendix.FileName != null)
                    {
                        string uploadPath = Path.Combine(hostingEnvironment.WebRootPath, "images");
                        fileName = Guid.NewGuid().ToString() + "_" + appendix.FileName.FileName;
                        string FilePath = Path.Combine(uploadPath, fileName);
                        appendix.FileName.CopyTo(new FileStream(FilePath, FileMode.Create));
                        System.IO.File.Delete(uploadPath + model.FileName);
                    }
                    model.FileName = fileName;
                    model.Name = appendix.Name;
                    _context.Update(model);
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

                var task = _context.Tasks.FirstOrDefault(e => e.AppendixId == model.Id);
                return RedirectToAction("TaskForModule", new { id = task.ModuleId });
            }
            return View(appendix);
        }

        // GET: Appendixes/Delete/5
        public async Task<IActionResult> DeleteAppendix(int? id)
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
        [HttpPost, ActionName("DeleteAppendix")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAppendix(int id)
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

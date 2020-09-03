using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TemplateEngine.Docx;

namespace TaskManager.Controllers
{
    [Authorize(Roles = "PROJECT MANAGER")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;

        private readonly IHostingEnvironment hostingEnvironment;

        private static readonly string DOCX_FILE_MIME_TYPE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DateOfBirth")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.CreateDate = DateTime.Now;
                project.UpdateDate = DateTime.Now;
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewBag.firstDate = project.DateOfBirth.ToString("yyyy-MM-dd");
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DateOfBirth")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.UpdateDate = DateTime.Now;
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        public IActionResult EditUserForProject(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            var project = _context.Projects.FirstOrDefault(e => e.Id == id);
            ProjectEmployeeModel model = new ProjectEmployeeModel();
            List<ListEmployee> list = _context.ListEmployees.Include(e => e.ApplicationUser).Where(e => e.ProjectId == id).ToList();
            List<string> vs = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                vs.Add(list[i].ApplicationUser.UserName);
            }

            model.projectId = project.Id;
            model.projectName = project.Name;
            model.allUsers = _userManager.Users.ToList();
            model.UserInProject = vs;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserForProject(int projectId, List<string> users)
        {
            if (users.Count == 0)
            {
                return NotFound();
            }
            // получем список пользователей в проекте
            var userProject = _context.ListEmployees
                .Include(e => e.ApplicationUser)
                .Include(e => e.Project)
                .Where(e => e.ProjectId == projectId)
                .Select(e => e.ApplicationUser)
                .ToList();
            List<ApplicationUser> tempUsers = new List<ApplicationUser>();
            for (int i = 0; i < users.Count; i++)
            {
                tempUsers.Add(_context.ApplicationUsers.FirstOrDefault(e => e.UserName == users[i]));
            }
            // получаем всех пользователей
            var allUsers = _userManager.Users.ToList();
            // получаем список пользоваталей, которые были добавлены
            var addedUsers = tempUsers.Except(userProject).ToList();
            // получаем роли, которые были удалены
            var removedUsers = userProject.Except(tempUsers).ToList();

            for (int i = 0; i < addedUsers.Count; i++)
            {
                ListEmployee newList = new ListEmployee();
                newList.ApplicationUserId = addedUsers[i].Id;
                newList.ProjectId = projectId;
                _context.ListEmployees.Add(newList);

            }

            for (int i = 0; i < removedUsers.Count; i++)
            {
                var listempl = _context.ListEmployees
                    .FirstOrDefault(e => e.ApplicationUserId == removedUsers[i].Id && e.ProjectId == projectId);
                _context.ListEmployees.Remove(listempl);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DetailsModuleForProject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .Include(e => e.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }



        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }



        //Модули
        public IActionResult ModuleForProject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _context.Modules
                .Include(e => e.Project)
                .Where(e => e.ProjectId == id)
                .ToList();
            ViewBag.ProjectId = id;
            return View(model);
        }




        public IActionResult CreateModuleForProject(int id)
        {
            ViewBag.ProjectId = id;
            ViewBag.firstDate = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModuleForProject(int id, [Bind("Id,Name,ProjectId,DateOfBirth")] Module @module)
        {
            if (ModelState.IsValid)
            {
                @module.CreateDate = DateTime.Now;
                @module.UpdateDate = DateTime.Now;
                @module.ProjectId = id;
                @module.Id = 0;
                var project = _context.Projects.Find(@module.ProjectId);
                if (!dateScale(false, @module.DateOfBirth, project.DateOfBirth))
                {
                    ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                    ViewBag.ErrorMassage = "Срок модуля не может превышать срок проекта. Дата завершения проекта: " + project.UpdateDate.ToString("D");
                    return View();
                }
                _context.Add(@module);
                await _context.SaveChangesAsync();
                return RedirectToAction("ModuleForProject", new { id = @module.ProjectId });
            }
            ViewBag.ProjectId = @module.ProjectId;
            ViewBag.firstDate = DateTime.Now.ToString("yyyy-MM-dd");

            return View(@module);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> EditModuleForProject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", @module.ProjectId);
            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModuleForProject(int id, [Bind("Id,Name,ProjectId,DateOfBirth")] Module @module)
        {
            if (id != @module.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var tempModule = _context.Modules.Find(id);
                var project = _context.Projects.Find(tempModule.ProjectId);
                if (!dateScale(false, @module.DateOfBirth, project.DateOfBirth))
                {
                    var moduletemp = await _context.Modules.FindAsync(id);
                    ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                    ViewBag.ErrorMassage = "Срок модуля не может превышать срок проекта. Дата завершения проекта: " + project.UpdateDate.ToString("D");
                    return View(moduletemp);
                }
                try
                {
                    tempModule.DateOfBirth = @module.DateOfBirth;
                    tempModule.CreateDate = @module.CreateDate;
                    tempModule.Name = @module.Name;
                    _context.Update(tempModule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ModuleForProject", new { id = module.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", @module.ProjectId);
            return View(@module);
        }

        public IActionResult TaskForModule(int id)
        {
            var model = _context.Tasks
                .Include(e => e.Module)
                .Include(e => e.ApplicationUser)
                .Include(e => e.Status)
                .Where(e => e.ModuleId == id)
                .ToList();
            var modules = _context.Modules
               .FirstOrDefault(m => m.Id == id);
            ViewBag.ModuleId = id;
            ViewBag.ModuleName = modules.Name;
            ViewBag.ProjectId = modules.ProjectId;
            return View(model);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> DeleteModuleForProject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .Include(e => e.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("DeleteModuleForProject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedModuleForProject(int id)
        {
            var @module = await _context.Modules.FindAsync(id);
            _context.Modules.Remove(@module);
            await _context.SaveChangesAsync();
            return RedirectToAction("ModuleForProject", new { id = module.ProjectId });
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }


        [HttpGet]
        public IActionResult ReportSelect()
        {
            ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.firstDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.secondDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.ErrorMassage = "";
            return View();
        }

        [HttpPost]
        public IActionResult ReportSelect(ReportViewModel report)
        {
            if (!dateScale(false, report.FirstDate, report.LastDate))
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                ViewBag.firstDate = startDate.ToString("yyyy-MM-dd");
                ViewBag.secondDate = endDate.ToString("yyyy-MM-dd");
                ViewBag.ErrorMassage = "Задайте верные данные, конечная дата должна быть пожже начальной или они должны быть равны";
                return View();
            }
            string uploadPath = Path.Combine(hostingEnvironment.WebRootPath, "template");
            string path = Path.Combine(uploadPath, "template.docx");
            string templatePath = Path.Combine(uploadPath + "temp.docx");
            System.IO.File.Copy(path, templatePath, true);
            List<ProjectReport> projectReports = new List<ProjectReport>();
            var user = _context.ApplicationUsers.FirstOrDefault(e => e.Id == report.userId);

            var utils = _context.Utils.Find(1);
            if (utils == null)
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "Укажите конфигурационный данные! Этими правами обладает администратор!";
                return View();
            }
            if (utils.firstParameter == "")
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "Задайте имя главному бухгалтеру! Этими правами обладает администратор!";
                return View();
            }
            if (utils.secondParameter == "")
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "Задайте имя Координатору разработки программного обеспечения и тех. задания! Этими правами обладает администратор!";
                return View();
            }
            if (utils.thirdParameter == "")
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "Задайте имя Специалисту по управлению проектной деятельностью! Этими правами обладает администратор!";
                return View();
            }
            var listEmployees = _context.ListEmployees
                .Include(e => e.Project)
                .Where(e => e.ApplicationUserId == report.userId)
                .ToList();
            if (listEmployees.Count == 0)
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "У выбранного вами пользователя нет проектов. Поручите ему проекты!";
                return View();
            }

            List<Project> project = new List<Project>();
            foreach (var listempl in listEmployees)
            {
                var tempProject = listempl.Project;
                project.Add(tempProject);
            }

            int counter = 0;
            user.Position = _context.Positions.FirstOrDefault(e => e.Id == user.PositionId);
            user.QualificationLevel = _context.QualificationLevels.FirstOrDefault(e => e.Id == user.QualificationLevelId);
            if (user.PositionId == null)
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "У выбранного вами пользователя нет должности. Задайте ему должность! На это способен администратор";
                return View();
            }
            if (user.QualificationLevel == null)
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "У выбранного вами пользователя не указан уровень знаний. Укажите ему уровень знаний! На это способен администратор";
                return View();
            }
            foreach (var tempProject in project)
            {
                var module = _context.Modules.Where(e => e.ProjectId == tempProject.Id).ToList();
                List<ModuleReport> moduleReports = new List<ModuleReport>();
                foreach (var tempModule in tempProject.Module)
                {
                    var task = _context.Tasks.Where(e => e.ModuleId == tempModule.Id && e.ApplicationUserId == report.userId).Include(e => e.Appendix).ToList();
                    for (int i = 0; i < task.Count; i++)
                    {
                        if (DataPicker(task[i], report.FirstDate, report.LastDate))
                        {
                            task.Remove(task[i]);
                        }
                        else
                        {
                            counter++;
                        }
                    }
                    ModuleReport moduleReport = new ModuleReport();
                    moduleReport.Module = tempModule;
                    moduleReport.Tasks = task;
                    moduleReports.Add(moduleReport);
                }
                ProjectReport projectReport = new ProjectReport();
                projectReport.Modules = moduleReports;
                projectReport.Project = tempProject;
                projectReports.Add(projectReport);
            }

            if (counter == 0)
            {
                ViewData["userId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName");
                ViewBag.ErrorMassage = "У выбранного вами пользователя нет задач за текущий период!";
                return View();
            }

            var valuesToFill = GetContent(projectReports, user, report.FirstDate, report.LastDate, utils);
            using (var outputDocument = new TemplateProcessor(templatePath)
                 .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }

            var bytes = System.IO.File.ReadAllBytes(templatePath);
            string file_name = "department.docx";
            System.IO.File.Delete(templatePath);
            return File(bytes, DOCX_FILE_MIME_TYPE, file_name);
        }

        public class ProjectReport
        {
            public Project Project;
            public List<ModuleReport> Modules;
        }

        public class ModuleReport
        {
            public Module Module;
            public List<TaskManager.Data.Task> Tasks;
        }

        private bool DataPicker(TaskManager.Data.Task task, DateTime firstDate, DateTime secondDate)
        {
            if (dateScale(true, task.UpdateDate, firstDate) && dateScale(false, task.UpdateDate, secondDate))
            {
                return false;
            }
            return true;
        }

        private bool dateScale(bool flag, DateTime firstDate, DateTime secondDate)
        {
            if (flag)
            {
                if (firstDate.Year > secondDate.Year)
                {
                    return true;
                }
                else if (firstDate.Year == secondDate.Year)
                {
                    if (firstDate.Month > secondDate.Month)
                    {
                        return true;
                    }
                    else if (firstDate.Month == secondDate.Month)
                    {
                        if (firstDate.Day >= secondDate.Day)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (secondDate.Year > firstDate.Year)
                {
                    return true;
                }
                else if (secondDate.Year == firstDate.Year)
                {
                    if (secondDate.Month >= firstDate.Month)
                    {
                        return true;
                    }
                    else if (firstDate.Month == secondDate.Month)
                    {
                        if (secondDate.Day >= firstDate.Day)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private Content GetContent(List<ProjectReport> template, ApplicationUser user, DateTime firstDate, DateTime lastDate, Utils utils)
        {
            List<Appendix> appendices = new List<Appendix>();
            int count = 1;
            var listContentTwo = ListContent.Create("Projects List");
            for (int i = 0; i < template.Count; i++)
            {
                List<TableRowContent> tableRowContent = new List<TableRowContent>();
                for (int j = 0; j < template[i].Modules.Count; j++)
                {
                    List<FieldContent> fieldContents = new List<FieldContent>();
                    fieldContents.Add(new FieldContent("count", (j + 1).ToString()));
                    fieldContents.Add(new FieldContent("Name", template[i].Modules[j].Module.Name));
                    string desc = "";
                    string status = "";
                    string date = "";
                    string cause = "";
                    string note = "";
                    for (int x = 0; x < template[i].Modules[j].Tasks.Count; x++)
                    {
                        if (template[i].Modules[j].Tasks[x].Name is null)
                        {
                            desc += "                                " + (x + 1).ToString() + ". ";
                        }
                        else
                        {
                            desc += "                                " + (x + 1).ToString() + ". " + template[i].Modules[j].Tasks[x].Name;
                        }

                        DateTime firstDate1 = template[i].Modules[j].Tasks[x].CreateDate;
                        DateTime updateDate = template[i].Modules[j].Tasks[x].UpdateDate;

                        DateTime dateTime = DateTime.Now;


                        if (template[i].Modules[j].Tasks[x].StatusId == 1)
                        {
                            status += "                                " + (x + 1).ToString() + ". " + "В процессе";
                            int raznica = (dateTime - firstDate1).Days;
                            date += "                                " + (x + 1).ToString() + ". " + raznica.ToString() + " дней";

                        }
                        else
                        {
                            status += "                                " + (x + 1).ToString() + ". " + "Выполнено";
                            int raznica = (updateDate - firstDate1).Days;
                            date += "                                " + (x + 1).ToString() + ". " + raznica.ToString() + " дней";

                        }




                        if (template[i].Modules[j].Tasks[x].Appendix is null)
                        {
                            cause += "                                " + (x + 1).ToString() + ". ";
                        }
                        else
                        {
                            appendices.Add(template[i].Modules[j].Tasks[x].Appendix);
                            cause += "                                " + (x + 1).ToString() + ". Примечание " + count.ToString();
                            count++;
                        }

                        if (template[i].Modules[j].Tasks[x].Note is null)
                        {
                            note += "                                " + (x + 1).ToString() + ". ";
                        }
                        else
                        {
                            note += "                                " + (x + 1).ToString() + ". " + template[i].Modules[j].Tasks[x].Note;
                        }

                    }

                    fieldContents.Add(new FieldContent("Description", desc));
                    fieldContents.Add(new FieldContent("status", status));
                    fieldContents.Add(new FieldContent("date", date));
                    fieldContents.Add(new FieldContent("cause", note));
                    fieldContents.Add(new FieldContent("note", cause));
                    tableRowContent.Add(new TableRowContent(fieldContents));
                }

                listContentTwo.AddItem(new ListItemContent("Project", "Проект: " + template[i].Project.Name)
                                .AddTable(TableContent.Create("Team members", tableRowContent)));
            }
            ListContent imageList = new ListContent("Scientists List");

            if (appendices.Count > 0)
                for (int i = 0; i < appendices.Count; i++)
                {
                    string uploadPath = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    string fileName = appendices[i].FileName;
                    string FilePath = Path.Combine(uploadPath, fileName);
                    //appendices[i].FileName.CopyTo(new FileStream(FilePath, FileMode.Create));
                    imageList.AddItem(new FieldContent("CountImage", "Примечание " + (i + 1).ToString()),
                      new ImageContent("Photo", System.IO.File.ReadAllBytes(FilePath)),
                         new FieldContent("NameImage", appendices[i].Name));
                }
            if (imageList.Items == null)
            {
                imageList.AddItem(new FieldContent("CountImage", ""),
                        new FieldContent("NameImage", ""));
            }

            var valuesToFill = new Content(
               listContentTwo,
               imageList,
                new FieldContent("reportName", "Отчет о выполненных задачах"),
                new FieldContent("reportDate", "За период с " + firstDate.ToString("D") + " по " + lastDate.ToString("D")),
                new FieldContent("employeePosition", user.Position.Name),
                new FieldContent("employeeQualification", user.QualificationLevel.Name),
            new FieldContent("First Parameter", utils.firstParameter),
            new FieldContent("Second Parameter", utils.secondParameter),
            new FieldContent("Third Parameter", utils.thirdParameter),
            new FieldContent("employeePositionTwo", user.Position.Name),
            new FieldContent("employeeName", user.UserName));
            return valuesToFill;
        }
    }
}

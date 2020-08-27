using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TemplateEngine.Docx;

namespace TaskManager.Controllers
{

   // [Authorize(Roles = "ADMIN")]
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;
        private static readonly string DOCX_FILE_MIME_TYPE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        public RolesController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {

            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index() => View(_roleManager.Roles.ToList());

        // [Authorize(Roles = "ADMIN")]
        public IActionResult Create() => View();


        public IActionResult UserCreate() => View();
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> UserCreate(string name, string password)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
            {

                var user = new ApplicationUser { UserName = name, Email = name };
                IdentityResult result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        public IActionResult UserList()
        {
            var user = _userManager.Users.ToList();
            var returnUsers = new List<ApplicationUser>();
            foreach (var tempuser in user)
            {
                tempuser.Position = _context.Positions.FirstOrDefault(e => e.Id == tempuser.PositionId);
                tempuser.QualificationLevel = _context.QualificationLevels.FirstOrDefault(e => e.Id == tempuser.QualificationLevelId);
                returnUsers.Add(tempuser);
            }
            return View(returnUsers);
        }

        public IActionResult UserListByRole(string Id)
        {
            if (Id is null)
            {
                return NotFound();
            }
            var roleName = _roleManager.FindByIdAsync(Id).Result.Name;
            var user = _userManager.GetUsersInRoleAsync(roleName);
            var returnUsers = new List<ApplicationUser>();
            foreach (var tempuser in user.Result)
            {
                tempuser.Position = _context.Positions.FirstOrDefault(e => e.Id == tempuser.PositionId);
                tempuser.QualificationLevel = _context.QualificationLevels.FirstOrDefault(e => e.Id == tempuser.QualificationLevelId);
                returnUsers.Add(tempuser);
            }
            return View(returnUsers);
        }

        [HttpGet]
        public IActionResult ReportSelect(String userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            ViewBag.Id = user.Id;
            ViewBag.UserName = user.UserName;
            return View();
        }

        [HttpPost]
        public IActionResult ReportSelect(ReportViewModel report)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.docx");
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + "temp.docx";
            System.IO.File.Copy(path, templatePath, true);
            List<ProjectReport> projectReports = new List<ProjectReport>();
            var user = _context.ApplicationUsers.FirstOrDefault(e => e.Id == report.userId);

            var listEmployees = _context.ListEmployees
                .Include(e => e.Project)
                .Where(e => e.ApplicationUserId == report.userId)
                .ToList();

            List<Project> project = new List<Project>();
            foreach (var listempl in listEmployees)
            {
                var tempProject = listempl.Project;
                project.Add(tempProject);
            }


            user.Position = _context.Positions.FirstOrDefault(e => e.Id == user.PositionId);
            user.QualificationLevel = _context.QualificationLevels.FirstOrDefault(e => e.Id == user.QualificationLevelId);
            foreach (var tempProject in project)
            {
                var module = _context.Modules.Where(e => e.ProjectId == tempProject.Id).ToList();
                List<ModuleReport> moduleReports = new List<ModuleReport>();
                foreach (var tempModule in tempProject.Module)
                {
                    var task = _context.Tasks.Where(e => e.ModuleId == tempModule.Id && e.ApplicationUserId == report.userId).ToList();
                    for(int i = 0; i < task.Count; i++)
                    {
                        if(DataPicker(task[i],report.FirstDate, report.LastDate))
                        {
                            task.Remove(task[i]);
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

            var valuesToFill = GetContent(projectReports, user, report.FirstDate, report.LastDate);
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




        [HttpPost]
        public async Task<IActionResult> UserDelete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("UserList");
                else
                    return NotFound();
            }
            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> UserEdit(string userId)
        {
            // получаем пользователя
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {

                ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name");
                ViewData["QualificationLevelId"] = new SelectList(_context.QualificationLevels, "Id", "Name");
                return View(user);
            }


            return NotFound();
        }

        //  [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(string userId, [Bind("Email,PositionId,QualificationLevelId,UserName,PhoneNumber")] ApplicationUser user)
        {
            /* if (userId != user.Id)
             {
                 return NotFound();
             }*/
            if (ModelState.IsValid)
                if (user != null)
                {

                    ApplicationUser userTemp = await _userManager.FindByIdAsync(userId);
                    await _userManager.SetUserNameAsync(userTemp, user.UserName);
                    await _userManager.SetPhoneNumberAsync(userTemp, user.PhoneNumber);
                    await _userManager.SetEmailAsync(userTemp, user.Email);
                    userTemp.QualificationLevelId = user.QualificationLevelId;
                    userTemp.PositionId = user.PositionId;
                    _context.Update(userTemp);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("UserList");
                }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
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

        public IActionResult Word(string userId)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.docx");
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + "temp.docx";
            System.IO.File.Copy(path, templatePath, true);
            List<ProjectReport> projectReports = new List<ProjectReport>();
            var user = _context.ApplicationUsers.FirstOrDefault(e => e.Id == userId);

            var listEmployees = _context.ListEmployees
                .Include(e => e.Project)
                .Where(e => e.ApplicationUserId == userId)
                .ToList();

            List<Project> project = new List<Project>();
            foreach (var listempl in listEmployees)
            {
                var tempProject = listempl.Project;
                project.Add(tempProject);
            }


            user.Position = _context.Positions.FirstOrDefault(e => e.Id == user.PositionId);
            user.QualificationLevel = _context.QualificationLevels.FirstOrDefault(e => e.Id == user.QualificationLevelId);
            foreach (var tempProject in project)
            {
                var module = _context.Modules.Where(e => e.ProjectId == tempProject.Id).ToList();
                List<ModuleReport> moduleReports = new List<ModuleReport>();
                foreach (var tempModule in tempProject.Module)
                {
                    var task = _context.Tasks.Where(e => e.ModuleId == tempModule.Id && e.ApplicationUserId == userId).ToList();
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

            DateTime dateTime = DateTime.Now;
            var valuesToFill = GetContent(projectReports, user, dateTime, dateTime);
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
                if (firstDate.Year >= secondDate.Year)
                {
                    if (firstDate.Month >= secondDate.Month)
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
                if (secondDate.Year >= firstDate.Year)
                {
                    if (secondDate.Month >= firstDate.Month)
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

        private Content GetContent(List<ProjectReport> template, ApplicationUser user, DateTime firstDate, DateTime lastDate)
        {
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
                            desc += "\n";
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
                            cause += "\n";
                        }
                        else
                        {
                            cause += "                                " + (x + 1).ToString() + ". " + template[i].Modules[j].Tasks[x].Appendix.Name;
                        }

                        if (template[i].Modules[j].Tasks[x].Note is null)
                        {
                            note += "\n";
                        }
                        else
                        {
                            note += "                                " + (x + 1).ToString() + ". " + template[i].Modules[j].Tasks[x].Note;
                        }

                    }

                    fieldContents.Add(new FieldContent("Description", desc));
                    fieldContents.Add(new FieldContent("status", status));
                    fieldContents.Add(new FieldContent("date", date));
                    fieldContents.Add(new FieldContent("cause", cause));
                    fieldContents.Add(new FieldContent("note", note));
                    tableRowContent.Add(new TableRowContent(fieldContents));
                }

                listContentTwo.AddItem(new ListItemContent("Project", "Проект: " + template[i].Project.Name)
                                .AddTable(TableContent.Create("Team members", tableRowContent)));
            }

            var valuesToFill = new Content(
               listContentTwo,
                new FieldContent("reportName", "Отчет о выполненных задачах"),
                new FieldContent("reportDate", "За период с " + firstDate.ToString("D") + " по " + lastDate.ToString("D")),
                new FieldContent("employeePosition", user.Position.Name),
                new FieldContent("employeeQualification", user.QualificationLevel.Name),
            new FieldContent("employeePositionTwo", user.Position.Name),
            new FieldContent("employeeName", user.UserName));
            return valuesToFill;
        }


    }
}
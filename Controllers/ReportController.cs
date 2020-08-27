using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TemplateEngine.Docx;

namespace TaskManager.Controllers
{
    [Authorize(Roles = "PROJECT MANAGER")]
    public class ReportController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private static readonly string DOCX_FILE_MIME_TYPE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";


        public ReportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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
            var project = _context.ListEmployees
                .Where(e => e.ApplicationUserId == userId)
                .Select(e => e.Project)
                .ToList();
            user.Position = _context.Positions.FirstOrDefault(e => e.Id == user.PositionId);
            user.QualificationLevel = _context.QualificationLevels.FirstOrDefault(e => e.Id == user.QualificationLevelId);
            foreach (var tempProject in project)
            {
                var module = _context.Modules.Where(e => e.ProjectId == tempProject.Id).ToList();
                List<ModuleReport> moduleReports = new List<ModuleReport>();
                foreach (var tempModule in tempProject.Module)
                {
                    var task = _context.Tasks.Where(e => e.ModuleId == tempModule.Id).ToList();
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

            var valuesToFill = GetContent(projectReports, user);
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

        private Content GetContent(List<ProjectReport> template, ApplicationUser user)
        {
            DateTime dateTime = DateTime.Now;
            var studentsTable = new TableContent("table");
            for (int i = 0; i < template.Count; i++)
            {
                studentsTable.AddRow(new FieldContent("employeeProjectName", template[i].Project.Name));
                for (int j = 0; j < template[i].Modules.Count; j++)
                {
                    studentsTable
                        .AddRow(
                            new FieldContent("employeeCount", j.ToString()),
                            new FieldContent("employeeModule", template[i].Modules[j].Module.Name),
                            new FieldContent("employeeProject", j.ToString()),
                            new FieldContent("employeeDate", j.ToString()),
                            new FieldContent("employeeCause", j.ToString()),
                            new FieldContent("employeeNote", j.ToString()));
                }
            }

            var valuesToFill = new Content(
                new FieldContent("reportName", "Отчет о выполненных задачах"),
                new FieldContent("reportDate", "За период с 01" + dateTime.Month.ToString() + dateTime.Year.ToString() + "по" + dateTime.Day.ToString() + dateTime.Year.ToString()),
                new FieldContent("employeePosition", user.Position.Name),
                new FieldContent("employeeQualification", user.QualificationLevel.Name),
                new FieldContent("date", DateTime.Now.Date.ToString()),
            studentsTable,
            new FieldContent("employeePositionTwo", user.Position.Name),
            new FieldContent("employeeName", user.UserName));
            return valuesToFill;
        }
    }
}

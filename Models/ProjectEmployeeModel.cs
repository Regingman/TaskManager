using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Models
{
    public class ProjectEmployeeModel
    {
        public int projectId { get; set; }
        public string projectName { get; set; }
        public IList<string> UserInProject { get; set; }
        public List<ApplicationUser> allUsers { get; set; }
    }
}

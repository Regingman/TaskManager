using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Controllers
{
    public class FirtInit
    {
        public static async System.Threading.Tasks.Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "te@mail.ru";
            string password = "Test123!";
            if (await roleManager.FindByNameAsync("ADMIN") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
            }
            if (await roleManager.FindByNameAsync("USER") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("USER"));
            }
            if (await roleManager.FindByNameAsync("PROJECT MANAGER") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("PROJECT MANAGER"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                ApplicationUser admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "ADMIN");
                }
            }
        }
    }
}

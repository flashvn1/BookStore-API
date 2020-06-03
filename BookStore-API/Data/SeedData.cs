using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Data
{
    public static class SeedData
    {

        public async static Task seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await seedRoles(roleManager);
            await seedUsers(userManager);
        }

        private async static Task seedUsers(UserManager<IdentityUser> userManager)
        { 
            if(await userManager.FindByEmailAsync("admin@bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("customer@bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer",
                    Email = "customer@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private async static Task seedRoles(RoleManager<IdentityRole> roleManager)
        { 
            if(! await roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                var result = await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole
                {
                    Name = "Customer"
                };
                var result = await roleManager.CreateAsync(role);
            }
        }
    }
}

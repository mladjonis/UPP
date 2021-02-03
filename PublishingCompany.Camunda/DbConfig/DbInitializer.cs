using Camunda.Api.Client.User;
using Microsoft.AspNetCore.Identity;
using PublishingCompany.Camunda.BPMN;
using PublishingCompany.Camunda.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.DbConfig
{
    public class DbInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly CamundaContext _context;
        private readonly BpmnService _bpmnService;

        public DbInitializer(UserManager<User> userManager, RoleManager<Role> roleManager, CamundaContext context, BpmnService bpmnService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _bpmnService = bpmnService;
        }

        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var roles = new List<Role>
                {
                    new Role() {Name = "Admin"},
                    new Role() {Name = "Writer"},
                    new Role() {Name = "Cometee"},
                    new Role() {Name = "Reader"},
                    new Role() {Name = "BetaReader"},
                    new Role() {Name = "Editor"},
                    new Role() {Name = "MainEditor"}
                };

                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                var writer = new User()
                {
                    Name = "Writer",
                    Lastname = "Writer",
                    Email = "writer@camunda.com",
                    UserName = "writer",
                    Files = "https://localhost:44343/docs/writer/doc1.pdf,https://localhost:44343/docs/writer/doc2.pdf"
                };

                IdentityResult writerResult =  _userManager.CreateAsync(writer, "password123").Result;
                if (writerResult.Succeeded)
                {
                    _userManager.AddToRoleAsync(writer, "Writer").Wait();
                    _bpmnService.CreateUser(new UserProfileInfo() { Email = writer.Email, LastName = writer.Lastname, FirstName = writer.Name, Id = writer.UserName }, "password123").Wait();
                }
                

                var admin = new User { UserName = "Admin", Email = "admin@camunda.com", Name="Admin", Lastname="Admin" };
                IdentityResult result = _userManager.CreateAsync(admin, "password123").Result;

                if (result.Succeeded)
                {
                    var adm = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRoleAsync(adm, "Admin").Wait();
                }

                //dodati odbor
                var cometee1 = new User { UserName = "Cometee1", Email = "cometee1@camunda.com", Name = "Cometee1", Lastname = "Cometee1" };
                var cometee2 = new User { UserName = "Cometee2", Email = "cometee2@camunda.com", Name = "Cometee2", Lastname = "Cometee2" };
                var cometees = new List<User>() { cometee1, cometee2 };
                foreach(var user in cometees)
                {
                    IdentityResult resulte = _userManager.CreateAsync(user, "password123").Result;
                    if (resulte.Succeeded)
                    {
                        _userManager.AddToRoleAsync(user, "Cometee").Wait();
                        _bpmnService.CreateUser(new UserProfileInfo() { Email = user.Email, LastName = user.Lastname, FirstName = user.Name, Id = user.UserName }, "password123").Wait();
                    }
                }

            }
        }
    }
}

using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LeDi.Server2.Pages
{
    public partial class Setup
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        // The role that defindes the role for Administrators
        string AdministrationRole = "Role-Administrators";

        // The role that defines the role for Guests
        string GuestRole = "Role-Guests";

        // The role that is used for referees
        string RefereeRole = "Role-Referees";

        // The role that is used for tournament operators
        string OperatorRole = "Role-Operators";

        // The admin name
        string AdminName = "admin@ledi";

        // Setuplog to show on the setup website
        string SetupLog = "";


        protected override async Task OnInitializedAsync()
        {
            var user = await _UserManager.FindByNameAsync(AdminName);
            if (user != null)
            {
                // Is admin@ledi in administrator role?
                var UserResult = await _UserManager.IsInRoleAsync(user, AdministrationRole);
                if (UserResult)
                {
                    SetupLog += "Setup done. Please logoff and logon again.";
                    //await InvokeAsync(() => { StateHasChanged(); });
                    return;
                }
                else
                {
                    SetupLog += "Setting up roles...<br />";
                    //await InvokeAsync(() => { StateHasChanged(); });
                    await SetupRoles();
                    SetupLog += "Roles set up...<br />";
                    //await InvokeAsync(() => { StateHasChanged(); });

                    SetupLog += "Setting up admin...<br />";
                    //await InvokeAsync(() => { StateHasChanged(); });
                    await SetupAdmin();
                    SetupLog += "Admin set up...<br />";
                    //await InvokeAsync(() => { StateHasChanged(); });

                    SetupLog += "Setup done. Please logoff and logon again.";

                }
            }
            else
            {
                SetupLog += "Please create adminuser before calling the setup.";
            }
}

        private async Task SetupRoles()
        {
            // Ensure all attributes are created
            await CreateAttributes();

            // Ensure the Build in Admin role is created
            var RoleResult = await _RoleManager.FindByNameAsync(AdministrationRole);
            if (RoleResult == null)
            {
                // Create AdministrationRole Role
                await _RoleManager.CreateAsync(new IdentityRole(AdministrationRole));

                if ((await DataHandler.GetUserRoleAsync(AdministrationRole)) == null)
                {
                    await DataHandler.SetUserRoleAsync(new TblUserRole()
                    {
                        CanDeviceCommands = true,
                        CanDeviceManage = true,
                        CanMatchAdd = true,
                        CanMatchAdvancedControls = true,
                        CanMatchDelete = true,
                        CanMatchEdit = true,
                        CanMatchEnd = true,
                        CanMatchPenalty = true,
                        CanMatchStart = true,
                        CanMatchStop = true,
                        CanPlayerAdd = true,
                        CanPlayerDelete = true,
                        CanPlayerEdit = true,
                        CanRoleAdd = true,
                        CanRoleDelete = true,
                        CanRoleEdit = true,
                        CanSettingManage = true,
                        CanTeamAdd = true,
                        CanTeamDelete = true,
                        CanTeamEdit = true,
                        CanTemplateManage = true,
                        CanTournamentAdd = true,
                        CanTournamentEdit = true,
                        CanTournamentMatchAdd = true,
                        CanTournamentMatchDelete = true,
                        CanTournamentMatchEdit = true,
                        CanUserAdd = true,
                        CanUserDelete = true,
                        CanUserEdit = true,
                        CanUserPasswordEdit = true,
                        IsAdmin = true,
                        RoleName = AdministrationRole
                    });
                }
            }


            // Ensure the Build in Referee role is created
            RoleResult = await _RoleManager.FindByNameAsync(RefereeRole);
            if (RoleResult == null)
            {
                // Create AdministrationRole Role
                await _RoleManager.CreateAsync(new IdentityRole(RefereeRole));

                if ((await DataHandler.GetUserRoleAsync(RefereeRole)) == null)
                {
                    await DataHandler.SetUserRoleAsync(new TblUserRole()
                    {
                        CanMatchAdvancedControls = true,
                        CanMatchEnd = true,
                        CanMatchPenalty = true,
                        CanMatchStart = true,
                        CanMatchStop = true,
                        RoleName = RefereeRole
                    });
                }
            }


            // Ensure the Build in Operator role is created
            RoleResult = await _RoleManager.FindByNameAsync(OperatorRole);
            if (RoleResult == null)
            {
                // Create AdministrationRole Role
                await _RoleManager.CreateAsync(new IdentityRole(OperatorRole));

                if ((await DataHandler.GetUserRoleAsync(OperatorRole)) == null)
                {
                    await DataHandler.SetUserRoleAsync(new TblUserRole()
                    {
                        CanDeviceCommands = true,
                        CanMatchAdd = true,
                        CanMatchAdvancedControls = true,
                        CanMatchDelete = true,
                        CanMatchEdit = true,
                        CanMatchEnd = true,
                        CanMatchPenalty = true,
                        CanMatchStart = true,
                        CanMatchStop = true,
                        CanPlayerAdd = true,
                        CanPlayerDelete = true,
                        CanPlayerEdit = true,
                        CanRoleAdd = true,
                        CanRoleDelete = true,
                        CanRoleEdit = true,
                        CanTeamAdd = true,
                        CanTeamDelete = true,
                        CanTeamEdit = true,
                        CanTemplateManage = true,
                        CanTournamentAdd = true,
                        CanTournamentEdit = true,
                        CanTournamentMatchAdd = true,
                        CanTournamentMatchDelete = true,
                        CanTournamentMatchEdit = true,
                        CanUserAdd = true,
                        CanUserDelete = true,
                        CanUserEdit = true,
                        IsAdmin = true,
                        RoleName = OperatorRole
                    });
                }
            }

            // Ensure the Build in Guest role is created
            RoleResult = await _RoleManager.FindByNameAsync(GuestRole);
            if (RoleResult == null)
            {
                // Create GuestRole Role
                await _RoleManager.CreateAsync(new IdentityRole(GuestRole));

                if ((await DataHandler.GetUserRoleAsync(GuestRole)) == null)
                {
                    await DataHandler.SetUserRoleAsync(new TblUserRole() { RoleName = GuestRole });
                }
            }

            // Ensure all roles are created
            var roleList = await DataHandler.GetUserRoleListAsync();
            foreach (var aRole in roleList)
            {
                if (await _RoleManager.FindByNameAsync(aRole.RoleName) == null)
                {
                    // Create role
                    await _RoleManager.CreateAsync(new IdentityRole(aRole.RoleName));
                }
            }
        }

        private async Task SetupAdmin()
        { 

            // Ensure a user named admin@ledi is an Administrator
            var user = await _UserManager.FindByNameAsync(AdminName);
            if (user != null)
            {
                // Is admin@ledi in administrator role?
                var UserResult = await _UserManager.IsInRoleAsync(user, AdministrationRole);
                if (!UserResult)
                {
                    // Put admin in Administrator role
                    await _UserManager.AddToRoleAsync(user, AdministrationRole);
                    await SetAdminRoleAttributes(user);
                } 
            }
        }

        /// <summary>
        /// Creates all attributes, if they don't exist.
        /// </summary>
        /// <returns></returns>
        private async Task CreateAttributes()
        {
            try
            {
                if (!(await _RoleManager.RoleExistsAsync("Att-IsAdmin")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-IsAdmin"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchAdd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchDelete"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchStart")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchStart"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchStop")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchStop"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchEnd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchEnd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchPenalty")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchPenalty"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchAdvancedControls")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchAdvancedControls"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentAdd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentMatchAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentMatchAdd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentMatchDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentMatchDelete"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentMatchEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentMatchEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserAdd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserDelete"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserPasswordEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserPasswordEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanRoleAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanRoleAdd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanRoleEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanRoleEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanRoleDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanRoleDelete"));

                if (!(await _RoleManager.RoleExistsAsync("Att-IsAdCanPlayerAddmin")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanPlayerAdd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanPlayerEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanPlayerEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanPlayerDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanPlayerDelete"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTeamAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTeamAdd"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTeamEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTeamEdit"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanSettingManage")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanSettingManage"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTeamDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTeamDelete"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanDeviceManage")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanDeviceManage"));

                if (!(await _RoleManager.RoleExistsAsync("Att-CanDeviceCommands")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanDeviceCommands"));
            }
            catch (Exception ea)
            {
                Console.WriteLine("Failed to set Role attributes. " + ea.ToString());
            }
        }



        /// <summary>
        /// Sets the attributs for the role of the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task SetAdminRoleAttributes(IdentityUser? user)
        {
            // add the attributes as role
            var attributes = await DataHandler.GetUserRoleAsync(AdministrationRole);

            if (user == null || attributes == null)
                return;

            try
            {
                await _UserManager.AddToRoleAsync(user, "Att-IsAdmin");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchAdd");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchDelete");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchStart");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchStop");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchEnd");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchPenalty");
                await _UserManager.AddToRoleAsync(user, "Att-CanMatchAdvancedControls");
                await _UserManager.AddToRoleAsync(user, "Att-CanTournamentAdd");
                await _UserManager.AddToRoleAsync(user, "Att-CanTournamentEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanTournamentMatchAdd");
                await _UserManager.AddToRoleAsync(user, "Att-CanTournamentMatchDelete");
                await _UserManager.AddToRoleAsync(user, "Att-CanTournamentMatchEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanUserAdd");
                await _UserManager.AddToRoleAsync(user, "Att-CanUserEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanUserDelete");
                await _UserManager.AddToRoleAsync(user, "Att-CanUserPasswordEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanRoleAdd");
                await _UserManager.AddToRoleAsync(user, "Att-CanRoleEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanRoleDelete");
                await _UserManager.AddToRoleAsync(user, "Att-CanPlayerAdd");
                await _UserManager.AddToRoleAsync(user, "Att-CanPlayerEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanPlayerDelete");
                await _UserManager.AddToRoleAsync(user, "Att-CanTeamAdd");
                await _UserManager.AddToRoleAsync(user, "Att-CanTeamEdit");
                await _UserManager.AddToRoleAsync(user, "Att-CanSettingManage");
                await _UserManager.AddToRoleAsync(user, "Att-CanTeamDelete");
                await _UserManager.AddToRoleAsync(user, "Att-CanDeviceManage");
                await _UserManager.AddToRoleAsync(user, "Att-CanDeviceCommands");
            }
            catch (Exception ea)
            {
                Console.WriteLine("Failed to set Admin role attributes. " + ea.ToString());
            }
        }
    }
}

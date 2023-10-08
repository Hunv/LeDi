using LeDi.Server2.Data;
using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Runtime.CompilerServices;

namespace LeDi.Server2.Pages
{
    // Credits to https://blazorhelpwebsite.com/ViewBlogPost/21
    // Examples for usage: https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-7.0
    public partial class UserManage
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        // The role that defindes the role for Administrators
        string AdministrationRole = "Role-Administrators";

        // Property used to add or edit the currently selected user
        IdentityUser objUser = new IdentityUser();

        // Tracks the selected role for the currently selected user
        string CurrentUserRole { get; set; } = "Role-Guests";

        // Collection to display the existing users
        List<IdentityUser> ColUsers = new List<IdentityUser>();

        // Options to display in the roles dropdown when editing a user (default values)
        List<string> Options = new List<string>() { "Guests", "Administrators" };

        // To hold any possible errors
        string strError = "";
        
        // To enable showing the Popup
        bool ShowPopup = false;


        protected override async Task OnInitializedAsync()
        {
            var roleList = await DataHandler.GetUserRoleListAsync();

            // Get all roles to show for add/edit dialog
            if (roleList.Count > 0)
            {
                //Remove the Role-Prefix to display it in the Combobox
                var roles = roleList.Select(x => x.RoleName);
                Options = roles.Select(x => x.Replace("Role-", "")).ToList();
            }

            // Get the current logged in user
            //CurrentUser = (await authenticationStateTask).User;

            // Get the users
            await GetUsers();
        }

        /// <summary>
        /// Gets all users from the database
        /// </summary>
        public async Task GetUsers()
        {
            // clear any error messages
            strError = "";

            // Collection to hold users
            ColUsers = new List<IdentityUser>();

            // get users from _UserManager
            var user = _UserManager.Users.Select(x => new IdentityUser
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                PasswordHash = "*****"
            });

            foreach (var item in user)
            {
                var roles = await GetRoles(item);
                roles = roles.Where(x => x.StartsWith("Role")).ToList();

                // Just for visual purposes to show the assigned role(s) in the UI. Abuse the unused PhoneNumber field for this.
                item.PhoneNumber = string.Join(", ", roles); 

                ColUsers.Add(item);
            }
        }

        /// <summary>
        /// Shows the AdduserPopup
        /// </summary>
        void AddNewUser()
        {
            // Make new user
            objUser = new IdentityUser();
            objUser.PasswordHash = "";

            // Set Id to blank so we know it is a new record
            objUser.Id = "";

            // Open the Popup
            ShowPopup = true;
        }

        /// <summary>
        /// Saves changes and new users
        /// </summary>
        /// <returns></returns>
        async Task SaveUser()
        {
            try
            {
                // Is this an existing user?
                if (objUser.Id != "")
                {
                    // Get the user
                    var user = await _UserManager.FindByIdAsync(objUser.Id);
                    if (user == null) { return; }

                    // Check if currently selected Role has the Role Prefix
                    if (!CurrentUserRole.StartsWith("Role-"))
                    {
                        CurrentUserRole = "Role-" + CurrentUserRole;
                    }

                    // Update Email
                    user.Email = objUser.Email;

                    // Update the user
                    await _UserManager.UpdateAsync(user);

                    // Only update password if the current value is not the default value, null or empty
                    if (!string.IsNullOrEmpty(objUser.PasswordHash) && objUser.PasswordHash != "*****")
                    {
                        var resetToken = await _UserManager.GeneratePasswordResetTokenAsync(user);
                        var passworduser = await _UserManager.ResetPasswordAsync(user,resetToken,objUser.PasswordHash);

                        if (!passworduser.Succeeded)
                        {
                            if (passworduser.Errors.FirstOrDefault() != null)
                            {
                                var error = passworduser.Errors.FirstOrDefault();
                                if (error != null)
                                    strError = error.Description;
                            }
                            else
                            {
                                strError = "Password error";
                            }
                            // Keep the popup opened
                            return;
                        }
                    }

                    // Handle Roles

                    // If the role changed
                    if (await _UserManager.IsInRoleAsync(user, CurrentUserRole) == false)
                    {
                        // Remove user from old role(s)
                        var oldRoles = await _UserManager.GetRolesAsync(user);
                        foreach (var oldRole in oldRoles)
                        {
                            await _UserManager.RemoveFromRolesAsync(user, oldRoles);
                        }

                        // add user to new role
                        await _UserManager.AddToRoleAsync(user, CurrentUserRole);
                        await SetRoleAttributes(user, CurrentUserRole.Replace("Role-", ""));
                    }
                }
                else
                {
                    // Insert new user
                    var NewUser =
                        new IdentityUser
                        {
                            UserName = objUser.UserName,
                            Email = objUser.Email
                        };
                    var CreateResult = await _UserManager.CreateAsync(NewUser, objUser.PasswordHash ?? "");
                    
                    if (!CreateResult.Succeeded)
                    {
                        var error = CreateResult.Errors.FirstOrDefault();
                        if (error != null)
                        {
                            strError = error.Description;
                        }
                        else
                        {
                            strError = "Create error";
                        }
                        // Keep the popup opened
                        return;
                    }
                    else
                    {
                        // Handle Roles
                        // put user to given role
                        await _UserManager.AddToRoleAsync(NewUser, "Role-" + CurrentUserRole);
                        await SetRoleAttributes(NewUser, "Role-" + CurrentUserRole);

                        //if (CurrentUserRole == AdministrationRole)
                        //{
                        //    // Put admin in Administrator role
                        //    await _UserManager.AddToRoleAsync(NewUser, AdministrationRole);
                        //}
                    }
                }

                // Close the Popup
                ShowPopup = false;

                // Refresh Users
                await GetUsers();
            }
            catch (Exception ex)
            {
                strError = ex.GetBaseException().Message;
            }
        }

        /// <summary>
        /// Sets the attributs for the role of the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task SetRoleAttributes(IdentityUser? user, string role)
        {
            // add the attributes as role
            var attributes = await DataHandler.GetUserRoleAsync(role);

            if (user == null || attributes == null)
                return;

            try
            {
                if (!(await _RoleManager.RoleExistsAsync("Att-IsAdmin")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-IsAdmin"));
                if (attributes.IsAdmin && !(await _UserManager.IsInRoleAsync(user, "Att-IsAdmin")))
                    await _UserManager.AddToRoleAsync(user, "Att-IsAdmin");
                else if (!attributes.IsAdmin && (await _UserManager.IsInRoleAsync(user, "Att-IsAdmin")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-IsAdmin");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchAdd"));
                if (attributes.CanMatchAdd && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchAdd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchAdd");
                else if (!attributes.CanMatchAdd && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchAdd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchAdd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchDelete"));
                if (attributes.CanMatchDelete && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchDelete")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchDelete");
                else if (!attributes.CanMatchDelete && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchDelete")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchDelete");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchEdit"));
                if (attributes.CanMatchEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchEdit");
                else if (!attributes.CanMatchEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchStart")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchStart"));
                if (attributes.IsAdmin && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchStart")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchStart");
                else if (!attributes.IsAdmin && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchStart")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchStart");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchStop")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchStop"));
                if (attributes.CanMatchStop && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchStop")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchStop");
                else if (!attributes.CanMatchStop && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchStop")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchStop");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchEnd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchEnd"));
                if (attributes.CanMatchEnd && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchEnd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchEnd");
                else if (!attributes.CanMatchEnd && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchEnd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchEnd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchPenalty")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchPenalty"));
                if (attributes.CanMatchPenalty && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchPenalty")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchPenalty");
                else if (!attributes.CanMatchPenalty && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchPenalty")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchPenalty");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanMatchAdvancedControls")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanMatchAdvancedControls"));
                if (attributes.CanMatchAdvancedControls && !(await _UserManager.IsInRoleAsync(user, "Att-CanMatchAdvancedControls")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanMatchAdvancedControls");
                else if (!attributes.CanMatchAdvancedControls && (await _UserManager.IsInRoleAsync(user, "Att-CanMatchAdvancedControls")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanMatchAdvancedControls");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentAdd"));
                if (attributes.CanTournamentAdd && !(await _UserManager.IsInRoleAsync(user, "Att-CanTournamentAdd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTournamentAdd");
                else if (!attributes.CanTournamentAdd && (await _UserManager.IsInRoleAsync(user, "Att-CanTournamentAdd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTournamentAdd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentEdit"));
                if (attributes.CanTournamentEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanTournamentEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTournamentEdit");
                else if (!attributes.CanTournamentEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanTournamentEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTournamentEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentMatchAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentMatchAdd"));
                if (attributes.CanTournamentMatchAdd && !(await _UserManager.IsInRoleAsync(user, "Att-CanTournamentMatchAdd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTournamentMatchAdd");
                else if (!attributes.CanTournamentMatchAdd && (await _UserManager.IsInRoleAsync(user, "Att-CanTournamentMatchAdd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTournamentMatchAdd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentMatchDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentMatchDelete"));
                if (attributes.CanTournamentMatchDelete && !(await _UserManager.IsInRoleAsync(user, "Att-CanTournamentMatchDelete")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTournamentMatchDelete");
                else if (!attributes.CanTournamentMatchDelete && (await _UserManager.IsInRoleAsync(user, "Att-CanTournamentMatchDelete")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTournamentMatchDelete");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTournamentMatchEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTournamentMatchEdit"));
                if (attributes.CanTournamentMatchEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanTournamentMatchEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTournamentMatchEdit");
                else if (!attributes.CanTournamentMatchEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanTournamentMatchEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTournamentMatchEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserAdd"));
                if (attributes.CanUserAdd && !(await _UserManager.IsInRoleAsync(user, "Att-CanUserAdd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanUserAdd");
                else if (!attributes.CanUserAdd && (await _UserManager.IsInRoleAsync(user, "Att-CanUserAdd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanUserAdd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserEdit"));
                if (attributes.CanUserEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanUserEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanUserEdit");
                else if (!attributes.CanUserEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanUserEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanUserEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserDelete"));
                if (attributes.CanUserDelete && !(await _UserManager.IsInRoleAsync(user, "Att-CanUserDelete")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanUserDelete");
                else if (!attributes.CanUserDelete && (await _UserManager.IsInRoleAsync(user, "Att-CanUserDelete")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanUserDelete");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanUserPasswordEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanUserPasswordEdit"));
                if (attributes.CanUserPasswordEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanUserPasswordEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanUserPasswordEdit");
                else if (!attributes.CanUserPasswordEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanUserPasswordEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanUserPasswordEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanRoleAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanRoleAdd"));
                if (attributes.CanRoleAdd && !(await _UserManager.IsInRoleAsync(user, "Att-CanRoleAdd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanRoleAdd");
                else if (!attributes.CanRoleAdd && (await _UserManager.IsInRoleAsync(user, "Att-CanRoleAdd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanRoleAdd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanRoleEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanRoleEdit"));
                if (attributes.CanRoleEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanRoleEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanRoleEdit");
                else if (!attributes.CanRoleEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanRoleEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanRoleEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanRoleDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanRoleDelete"));
                if (attributes.CanRoleDelete && !(await _UserManager.IsInRoleAsync(user, "Att-CanRoleDelete")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanRoleDelete");
                else if (!attributes.CanRoleDelete && (await _UserManager.IsInRoleAsync(user, "Att-CanRoleDelete")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanRoleDelete");

                if (!(await _RoleManager.RoleExistsAsync("Att-IsAdCanPlayerAddmin")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanPlayerAdd"));
                if (attributes.CanPlayerAdd && !(await _UserManager.IsInRoleAsync(user, "Att-CanPlayerAdd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanPlayerAdd");
                else if (!attributes.CanPlayerAdd && (await _UserManager.IsInRoleAsync(user, "Att-CanPlayerAdd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanPlayerAdd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanPlayerEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanPlayerEdit"));
                if (attributes.CanPlayerEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanPlayerEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanPlayerEdit");
                else if (!attributes.CanPlayerEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanPlayerEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanPlayerEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanPlayerDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanPlayerDelete"));
                if (attributes.CanPlayerDelete && !(await _UserManager.IsInRoleAsync(user, "Att-CanPlayerDelete")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanPlayerDelete");
                else if (!attributes.CanPlayerDelete && (await _UserManager.IsInRoleAsync(user, "Att-CanPlayerDelete")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanPlayerDelete");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTeamAdd")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTeamAdd"));
                if (attributes.CanTeamAdd && !(await _UserManager.IsInRoleAsync(user, "Att-CanTeamAdd")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTeamAdd");
                else if (!attributes.CanTeamAdd && (await _UserManager.IsInRoleAsync(user, "Att-CanTeamAdd")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTeamAdd");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTeamEdit")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTeamEdit"));
                if (attributes.CanTeamEdit && !(await _UserManager.IsInRoleAsync(user, "Att-CanTeamEdit")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTeamEdit");
                else if (!attributes.CanTeamEdit && (await _UserManager.IsInRoleAsync(user, "Att-CanTeamEdit")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTeamEdit");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanSettingManage")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanSettingManage"));
                if (attributes.CanSettingManage && !(await _UserManager.IsInRoleAsync(user, "Att-CanSettingManage")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanSettingManage");
                else if (!attributes.CanSettingManage && (await _UserManager.IsInRoleAsync(user, "Att-CanSettingManage")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanSettingManage");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanTeamDelete")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanTeamDelete"));
                if (attributes.CanTeamDelete && !(await _UserManager.IsInRoleAsync(user, "Att-CanTeamDelete")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanTeamDelete");
                else if (!attributes.CanTeamDelete && (await _UserManager.IsInRoleAsync(user, "Att-CanTeamDelete")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanTeamDelete");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanDeviceManage")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanDeviceManage"));
                if (attributes.CanDeviceManage && !(await _UserManager.IsInRoleAsync(user, "Att-CanDeviceManage")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanDeviceManage");
                else if (!attributes.CanDeviceManage && (await _UserManager.IsInRoleAsync(user, "Att-CanDeviceManage")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanDeviceManage");

                if (!(await _RoleManager.RoleExistsAsync("Att-CanDeviceCommands")))
                    await _RoleManager.CreateAsync(new IdentityRole("Att-CanDeviceCommands"));
                if (attributes.CanDeviceCommands && !(await _UserManager.IsInRoleAsync(user, "Att-CanDeviceCommands")))
                    await _UserManager.AddToRoleAsync(user, "Att-CanDeviceCommands");
                else if (!attributes.CanDeviceCommands && (await _UserManager.IsInRoleAsync(user, "Att-CanDeviceCommands")))
                    await _UserManager.RemoveFromRoleAsync(user, "Att-CanDeviceCommands");
            }
            catch(Exception ea)
            {
                Console.WriteLine("Failed to set Role attributes. " + ea.ToString());
            }
        }

        /// <summary>
        /// Edit an user
        /// </summary>
        /// <param name="_IdentityUser"></param>
        /// <returns></returns>
        async Task EditUser(IdentityUser _IdentityUser)
        {
            // Set the selected user as the current user
            objUser = _IdentityUser;

            // Get the user
            var user = await _UserManager.FindByIdAsync(objUser.Id);
            if (user != null)
            {
                // Is user in administrator role?
                //var IsAdminUser = await _UserManager.IsInRoleAsync(user, AdministrationRole);

                foreach(var aRole in await DataHandler.GetUserRoleListAsync())
                {
                    if (await _UserManager.IsInRoleAsync(user, aRole.RoleName))
                    {
                        CurrentUserRole = aRole.RoleName;
                        break;
                    }
                }

                //if (IsAdminUser)
                //{
                //    CurrentUserRole = AdministrationRole;
                //}
                //else
                //{
                //    CurrentUserRole = "Guests";
                //}
            }

            // Open the Popup
            ShowPopup = true;
        }

        /// <summary>
        /// Delete an user
        /// </summary>
        /// <returns></returns>
        async Task DeleteUser()
        {
            // Close the Popup
            ShowPopup = false;

            // Get the user
            var user = await _UserManager.FindByIdAsync(objUser.Id);
            if (user != null)
            {
                // Delete the user
                await _UserManager.DeleteAsync(user);
            }

            // Refresh Users
            await GetUsers();
        }

        /// <summary>
        /// Closes the edit/delete/create popup
        /// </summary>
        void ClosePopup()
        {
            // Close the Popup
            ShowPopup = false;
        }


        /// <summary>
        /// Gets the roles of a user
        /// </summary>
        /// <param name="user"></param>
        async Task<IList<string>> GetRoles(IdentityUser user)
        {
            var roles = await _UserManager.GetRolesAsync(user);                        
            return roles;
        }

        ///// <summary>
        ///// Contains the list of all users
        ///// </summary>
        //public List<TblUser> UserList { get; set; } = new List<TblUser>();

        //public List<TblUserRole> RoleList { get; set; }= new List<TblUserRole>();

        ///// <summary>
        ///// Contains the currently added or edited user
        ///// </summary>
        //public TblUser User { get; set; } = new TblUser();


        //private ApplicationDbContext DbContextIdentity;
        //public IPasswordHasher<IdentityUser> PasswordHasher;

        //public int UserRoleId
        //{
        //    get
        //    {
        //        if (User.Role == null)
        //        {
        //            if (RoleList == null || RoleList.Count == 0)
        //            {
        //                return 0;
        //            }
        //            else
        //            {
        //                return RoleList.First().Id;
        //            }
        //        }
        //        else
        //        {
        //            return User.Role.Id;
        //        }
        //    }
        //    set
        //    {
        //        User.Role = RoleList.Single(x => x.Id == value);
        //    }
        //}

        //protected override async Task OnInitializedAsync()
        //{
        //    //UserList = await DataHandler.GetUserListAsync();
        //    //RoleList = await DataHandler.GetUserRoleListAsync();
        //    //if (RoleList != null)
        //    //    User.Role = RoleList.First();

        //    DbContextIdentity = new ApplicationDbContext();
        //    PasswordHasher = _passwordHasher;

        //    await base.OnInitializedAsync();
        //}
        //private async void EditUser(int userId)
        //{
        //    //var editUser = await DataHandler.GetUserAsync(userId);
        //    //User.LoginName = editUser.LoginName;
        //    //User.DisplayName = editUser.DisplayName;
        //    //User.IsEnabled = editUser.IsEnabled;
        //    //User.Password = editUser.Password;
        //    //User.Role = editUser.Role;
        //    //User.Id = userId;
        //    //await InvokeAsync(() => { StateHasChanged(); });
        //}

        //private void DeleteUser(int userId)
        //{

        //}

        //private async void AddUser(string username, string displayname, string password)
        //{
        //    var user = new IdentityUser { UserName = username, NormalizedUserName = displayname };
        //    var x = new UserManager<IdentityUser>();
        //    UserManager = new UserManager<ApplicationUser, string>(new UserStore<ApplicationUser, CustomRole, string, CustomUserLogin, CustomUserRole, CustomUserClaim>(new myDbContext()));
        //    var result = await UserManager<IdentityUser>
        //}

        //private async void SaveUser()
        //{
        //    //await DataHandler.SetUserAsync(User);
        //    //User = new TblUser() { Role = RoleList.First() };
        //    //UserList = await DataHandler.GetUserListAsync();
        //    //await InvokeAsync(() => { StateHasChanged(); });
        //}
    }
}

using LeDi.Server2.Data;
using LeDi.Server2.DatabaseModel;
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
        string AdministrationRole = "Administrators";
        //System.Security.Claims.ClaimsPrincipal CurrentUser;

        // The role that defindes the role for Guests
        string GuestRole = "Guests";

        // Property used to add or edit the currently selected user
        IdentityUser objUser = new IdentityUser();

        // Tracks the selected role for the currently selected user
        string CurrentUserRole { get; set; } = "Guests";

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
            // Ensure the Build in Admin role is created
            var RoleResult = await _RoleManager.FindByNameAsync(AdministrationRole);
            if (RoleResult == null)
            {
                // Create AdministrationRole Role
                await _RoleManager.CreateAsync(new IdentityRole(AdministrationRole));
            }

            // Ensure the Build in Guest role is created
            RoleResult = await _RoleManager.FindByNameAsync(GuestRole);
            if (RoleResult == null)
            {
                // Create GuestRole Role
                await _RoleManager.CreateAsync(new IdentityRole(GuestRole));
            }

            //ensure all roles are created
            var roleList = await DataHandler.GetUserRoleListAsync();
            foreach(var aRole in roleList)
            {
                if (await _RoleManager.FindByNameAsync(aRole.RoleName) == null)
                {
                    // Create role
                    await _RoleManager.CreateAsync(new IdentityRole(aRole.RoleName));
                }
            }

            // Get all roles to show for add/edit dialog
            if (roleList.Count > 0)
            {
                Options = roleList.Select(x => x.RoleName).ToList();
            }

            // Ensure a user named admin@ledi is an Administrator
            var user = await _UserManager.FindByNameAsync("admin@ledi");
            if (user != null)
            {
                // Is admin@ledi in administrator role?
                var UserResult = await _UserManager.IsInRoleAsync(user, AdministrationRole);
                if (!UserResult)
                {
                    // Put admin in Administrator role
                    await _UserManager.AddToRoleAsync(user, AdministrationRole);
                }
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
                // Just for visual purposes to show the assigned role(s) in the UI. Abuse the unused PhoneNumber field for this.
                item.PhoneNumber = string.Join(", ", await GetRoles(item)); 

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
                    // Is user in administrator role?
                    var UserResult = await _UserManager.IsInRoleAsync(user, AdministrationRole);

                    // Of the role changed
                    if (await _UserManager.IsInRoleAsync(user, CurrentUserRole) == false)
                    {
                        // Remove user from old role(s)
                        var oldRoles = await _UserManager.GetRolesAsync(user);
                        foreach (var oldRole in oldRoles) { await _UserManager.RemoveFromRolesAsync(user, oldRoles); }

                        // add user to new role
                        await _UserManager.AddToRoleAsync(user, CurrentUserRole);
                    }

                    // Is Administrator role selected but user is not an Administrator?
                    //if ((CurrentUserRole == AdministrationRole) & (!UserResult))
                    //{
                    //    // Put admin in Administrator role
                    //    await _UserManager.AddToRoleAsync(user, AdministrationRole);
                    //}
                    //else
                    //{
                    //    // Is Administrator role not selected but user is an Administrator?
                    //    if ((CurrentUserRole != AdministrationRole) & (UserResult))
                    //    {
                    //        // Remove user from Administrator role
                    //        await _UserManager.RemoveFromRoleAsync(user, AdministrationRole);
                    //    }
                    //}
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
                        await _UserManager.AddToRoleAsync(NewUser, CurrentUserRole);

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
                var UserResult = await _UserManager.IsInRoleAsync(user, AdministrationRole);

                if (UserResult)
                {
                    CurrentUserRole = AdministrationRole;
                }
                else
                {
                    CurrentUserRole = "Guests";
                }
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
        async Task<string> GetRoles(IdentityUser user)
        {
            var roles = await _UserManager.GetRolesAsync(user);            
            string roleString = String.Join(",", roles);
            return roleString;
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

using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace LeDi.Server2.Pages
{
    public partial class UserRole
    {
        /// <summary>
        /// Contains all User roles
        /// </summary>
        public List<TblUserRole> RoleList { get; set; }= new List<TblUserRole>();

        /// <summary>
        /// Contains the currently added or edited userrole
        /// </summary>
        public TblUserRole Role { get; set; } = new TblUserRole();



        protected override async Task OnInitializedAsync()
        {
            RoleList = await DataHandler.GetUserRoleListAsync();

            await base.OnInitializedAsync();
        }
        private async void EditRole(int roleId)
        {
            Role = await DataHandler.GetUserRoleAsync(roleId);
            await InvokeAsync(() => { StateHasChanged(); });
        }

        private async void DeleteRole(int roleId)
        {
            await DataHandler.RemoveUserRoleAsync(roleId);
            RoleList = await DataHandler.GetUserRoleListAsync();
            await InvokeAsync(() => { StateHasChanged(); });
        }

        private async void SaveRole()
        {
            if (!Role.RoleName.StartsWith("Role-"))
            {
                Role.RoleName = "Role-" + Role.RoleName;
            }
            await DataHandler.SetUserRoleAsync(Role);

            if (!(await _RoleManager.RoleExistsAsync(Role.RoleName)))
                await _RoleManager.CreateAsync(new IdentityRole(Role.RoleName));

            Role = new TblUserRole();
            RoleList = await DataHandler.GetUserRoleListAsync();

            await InvokeAsync(() => { StateHasChanged(); });
        }
    }
}

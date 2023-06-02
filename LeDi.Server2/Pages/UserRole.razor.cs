using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

        private void DeleteRole(int userId)
        {

        }

        private async void SaveRole()
        {
            Role.RoleName = "Role-" + Role.RoleName;
            await DataHandler.SetUserRoleAsync(Role);
            Role = new TblUserRole();
            RoleList = await DataHandler.GetUserRoleListAsync();
            await InvokeAsync(() => { StateHasChanged(); });
        }
    }
}

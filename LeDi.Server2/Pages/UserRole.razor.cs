using LeDi.Server2.DatabaseModel;
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

        /// <summary>
        /// Contains the roles a user has
        /// </summary>
        private TblUserRole? AuthenticatedUserRole { get; set; }



        protected override async Task OnInitializedAsync()
        {
            // Get the roles of the currently logged in user
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState != null && authState.User.Identity != null && authState.User.Identity.IsAuthenticated)
            {
                // Get the Roles from Identity management. Should only be one always.
                var username = authState.User.Identity.Name;
                if (username != null)
                {
                    var roles = await _UserManager.GetRolesAsync(await _UserManager.FindByNameAsync(username));

                    if (roles != null && roles.Count >= 1)
                    {
                        AuthenticatedUserRole = await DataHandler.GetUserRoleAsync(roles[0]);
                    }
                }
            }
            else
            {
                AuthenticatedUserRole = await DataHandler.GetUserRoleAsync("Guests");
            }

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
            await DataHandler.SetUserRoleAsync(Role);
            Role = new TblUserRole();
            RoleList = await DataHandler.GetUserRoleListAsync();
            await InvokeAsync(() => { StateHasChanged(); });
        }
    }
}

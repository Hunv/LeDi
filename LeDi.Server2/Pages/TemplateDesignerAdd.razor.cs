using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LeDi.Server2.Pages
{
    public partial class TemplateDesignerAdd
    {
        public TblTemplate Template { get; set; } = new TblTemplate();

        //Todo:
        //Parameter to edit template
        //If edit: Load it on start
        //Penalties

        public async void SaveTemplate()
        {
            await DataHandler.AddTemplate(Template);
            NavigationManager.NavigateTo("/TemplateDesigner");
        }

        public void DeleteTemplate(int templateId)
        {
            //Todo
        }

        protected override async Task OnInitializedAsync()
        {

        }
    }
}

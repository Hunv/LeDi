using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LeDi.Server2.Pages
{
    public partial class TemplateAdd
    {
        public TblTemplate Template { get; set; } = new TblTemplate();
        
        [Parameter]
        public int? SelectedTemplateId { get; set; }


        //Todo:
        //Penalties

        public async void SaveTemplate()
        {
            await DataHandler.AddTemplate(Template);
            NavigationManager.NavigateTo("/TemplateEditor");
        }

        public async void DeleteTemplate()
        {
            await DataHandler.DeleteTemplate(Template);
        }

        protected override async Task OnInitializedAsync()
        {
            if (SelectedTemplateId != null)
            {
                var tpl = await DataHandler.GetTemplate(SelectedTemplateId.Value);
                if (tpl != null)
                {
                    Template = tpl;
                }
            }
            await InvokeAsync(() => { StateHasChanged(); });
        }
    }
}

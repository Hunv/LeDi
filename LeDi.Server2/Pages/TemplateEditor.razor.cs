using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LeDi.Server2.Pages
{
    public partial class TemplateEditor
    {
        public TblTemplate Template { get; set; } = new TblTemplate();

        public List<TblTemplate> TemplateList { get; set; } = new List<TblTemplate>();

        //Todo
        //Clone an existing template
        //Build in templates (soccer, UWH, handball,...)


        public void EditTemplate(int templateId)
        {
            NavigationManager.NavigateTo("/TemplateAdd/" + templateId.ToString());
        }

        public async void DeleteTemplate(int templateId)
        {
            await DataHandler.DeleteTemplate(templateId);
            TemplateList = await DataHandler.GetTemplateList();
            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected override async Task OnInitializedAsync()
        {
            TemplateList = await DataHandler.GetTemplateList();
        }
    }
}

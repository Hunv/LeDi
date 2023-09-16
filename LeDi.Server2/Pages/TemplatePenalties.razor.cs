using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LeDi.Server2.Pages
{
    public partial class TemplatePenalties
    {
        [Parameter]
        public int? SelectedTemplateId { get; set; }

        public TblTemplate? SelectedTemplate { get; set; }


        protected override async Task OnInitializedAsync()
        {
            if (SelectedTemplateId.HasValue)
            {
                SelectedTemplate = await DataHandler.GetTemplate(SelectedTemplateId.Value);
            }
        }
    }
}

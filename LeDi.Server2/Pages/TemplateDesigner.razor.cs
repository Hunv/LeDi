using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LeDi.Server2.Pages
{
    public partial class TemplateDesigner
    {
        public TblTemplate Template { get; set; } = new TblTemplate();

        public List<TblTemplate> TemplateList { get; set; } = new List<TblTemplate>();

        //Todo
        //Clone an existing template
        //Build in templates (soccer, UWH, handball,...)

        public void SaveTemplate()
        {

        }

        public void EditTemplate(int templateId)
        {

        }
        public void DeleteTemplate(int templateId)
        {

        }

        protected override async Task OnInitializedAsync()
        {

        }
    }
}

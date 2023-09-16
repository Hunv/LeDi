using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NLog;
using System.IO;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;


namespace LeDi.Server2.Pages
{
    public partial class TemplateEditor
    {
        public TblTemplate Template { get; set; } = new TblTemplate();

        public List<TblTemplate> TemplateList { get; set; } = new List<TblTemplate>();

        private List<IBrowserFile> loadedFiles = new();
        private long maxFileSize = 1024 * 15; // For import template
        private int maxAllowedFiles = 100;// For import template

        //Todo
        //Delete Penalties

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

        public void EditTemplatePenalties(int templateId)
        {
            NavigationManager.NavigateTo("/TemplatePenalties/" + templateId.ToString());
        }

        public async void ExportTemplate(int templateId)
        {
            try
            {
                var template = await DataHandler.GetTemplate(templateId);

                var json = JsonConvert.SerializeObject(template, Formatting.None, new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);
                var fileStream = new MemoryStream(jsonBinary);
                var fileName = template.TemplateName + ".json";

                using var streamRef = new DotNetStreamReference(stream: fileStream);
                await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", Localizer["FailedToExportTemplate"] + Environment.NewLine + ex.Message);
            }
        }
        

        public async void ImportTemplate(InputFileChangeEventArgs e)
        {
            // Load the files selected
            foreach (var file in e.GetMultipleFiles(100))
            {
                try
                {
                    loadedFiles.Add(file);
                }
                catch (Exception ex)
                {
                    await JS.InvokeVoidAsync("alert", Localizer["FailedToImportTemplate"] + Environment.NewLine + ("File: {FileName} Error: {Error}", file.Name, ex.Message));
                }
            }

            // Check and save content
            try
            {
                foreach(var file in loadedFiles)
                {
                    var sR = new StreamReader(file.OpenReadStream());
                    var json = await sR.ReadToEndAsync();
                    
                    if (json == null)
                        continue;

                    try
                    {
                        TblTemplate importedTemplate = (TblTemplate)JsonConvert.DeserializeObject(json, typeof(TblTemplate));
                        
                        if (importedTemplate != null)
                        {
                            if (TemplateList.Any(x => x.TemplateName == importedTemplate.TemplateName))
                            {
                                await JS.InvokeVoidAsync("alert", Localizer["TemplateAlreadyExists"] + importedTemplate.TemplateName);
                                continue;
                            }

                            var penalties = importedTemplate.PenaltyList.ToList(); //create a deepcopy
                            importedTemplate.PenaltyList = new List<TblTemplatePenaltyItem>();

                            await DataHandler.AddTemplate(importedTemplate);

                            TemplateList = await DataHandler.GetTemplateList();
                            var template = TemplateList.Single(x => x.TemplateName == importedTemplate.TemplateName);

                            //add the penalties
                            foreach(var penalty in penalties)
                            {
                                penalty.Template = template;
                                await DataHandler.AddTemplatePenalty(template.Id, penalty);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        await JS.InvokeVoidAsync("alert", Localizer["UnableToImport"] + Environment.NewLine + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", Localizer["FailedToSaveImportedTemplate"] + Environment.NewLine + ex.Message);
            }
            await InvokeAsync(() => { StateHasChanged(); });

        }

        protected override async Task OnInitializedAsync()
        {
            TemplateList = await DataHandler.GetTemplateList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                var module = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/TemplateEditor.razor.js");
            }
            catch (Exception ex)
            {

            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}

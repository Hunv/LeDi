using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace LeDi.Server2.Pages
{
    public partial class TemplatePenaltyAdd
    {
        public TblTemplate Template { get; set; } = new TblTemplate();
        public TblTemplatePenaltyItem Penalty { get; set; } = new TblTemplatePenaltyItem();
        public TblTemplatePenaltyText PenaltyText { get; set; } = new TblTemplatePenaltyText("","");

        /// <summary>
        /// This is set if a Penalty is edited. Otherwise this is null to add a new TemplatePenalty
        /// </summary>
        [Parameter]
        public int? SelectedPenaltyId { get; set; }

        [Parameter]
        public int? SelectedTemplateId { get; set; }

        public async void SaveTemplatePenalty()
        {
            //this is a new one:
            if (SelectedPenaltyId == null)
            {
                await DataHandler.AddTemplatePenalty(SelectedTemplateId.Value, Penalty);
            }
            else //an existing one is edited.
            {
                await DataHandler.SetTemplatePenalty(SelectedTemplateId.Value, Penalty);
            }
            NavigationManager.NavigateTo("/TemplatePenalties/" + SelectedTemplateId.Value);
            
        }

        public async void AddTemplatePenaltyText()
        {
            // Check if settings are set
            if (!string.IsNullOrWhiteSpace(PenaltyText.Language) && !string.IsNullOrWhiteSpace(PenaltyText.Text))
            {
                //Check if language already exists:
                if (!Penalty.Display.Any(x => x.Language == PenaltyText.Language))
                {

                    Penalty.Display.Add(PenaltyText);
                    PenaltyText = new TblTemplatePenaltyText("", "");
                    await InvokeAsync(() => { StateHasChanged(); });
                }
                else
                {
                    await JsRuntime.InvokeVoidAsync("alert", Localizer["LanguageAlreadyExisting"]);
                }
            }
        }

        public void CancelTemplatePenalty()
        {
            NavigationManager.NavigateTo("/TemplatePenalties/" + SelectedTemplateId.Value);
        }

        public async void DeleteTemplatePenaltyText(string language)
        {
            var text = Penalty.Display.SingleOrDefault(x => x.Language == language);
            if (text != null)
            {
                Penalty.Display.Remove(text);
            }
            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected override async Task OnInitializedAsync()
        {
            if (SelectedTemplateId != null)
            {
                var tpl = await DataHandler.GetTemplate(SelectedTemplateId.Value);
                if (tpl != null)
                {
                    Template = tpl;

                    if (SelectedPenaltyId != null)
                    {
                        var pen = tpl.PenaltyList.SingleOrDefault(x => x.Id == SelectedPenaltyId.Value);
                        if (pen == null)
                        {
                            SelectedPenaltyId = null;
                        }
                        else
                        {
                            Penalty = pen;
                        }
                    }
                }
            }

            await InvokeAsync(() => { StateHasChanged(); });
        }
    }
}

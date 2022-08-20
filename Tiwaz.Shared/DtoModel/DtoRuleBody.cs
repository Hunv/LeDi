namespace Tiwaz.Shared.DtoModel
{
    public class DtoRuleBody
    {
        [System.Text.Json.Serialization.JsonPropertyName("rules")]
        public List<DtoRule>? Rules { get; set; }    
    }
}

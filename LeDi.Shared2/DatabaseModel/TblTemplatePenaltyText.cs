using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared2.DatabaseModel
{
    public class TblTemplatePenaltyText
    {
        /// <summary>
        /// Creates a new instance of Display Text
        /// </summary>
        /// <param name="language"></param>
        /// <param name="text"></param>
        public TblTemplatePenaltyText(string language, string text)
        {
            Language = language;
            Text = text;
        }

        /// <summary>
        /// Unique Id of the text
        /// </summary>
        [Key]
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Language of the text (ISO short version. i.e. EN = English, DE = German, ...)
        /// </summary>        
        public string Language { get; set; }


        /// <summary>
        /// The text to show
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The PenaltyItem this Text belongs to
        /// </summary>
        public TblTemplatePenaltyItem TemplatePenaltyItem { get; set; }
    }
}

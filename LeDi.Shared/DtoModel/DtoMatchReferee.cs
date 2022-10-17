using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Shared.DtoModel
{
    public class DtoMatchReferee
    {

        [MaxLength(256, ErrorMessageResourceName = "NameLengthErrorMax", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [MinLength(2, ErrorMessageResourceName = "NameLengthErrorMin", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [RegularExpression(@"^[\w\säüößÄÜÖẞ\s-_\.\+]*$", ErrorMessageResourceName = "NameFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = string.Empty;


        [MaxLength(256, ErrorMessageResourceName = "ClubnameLengthErrorMax", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [MinLength(2, ErrorMessageResourceName = "ClubnameLengthErrorMin", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [RegularExpression(@"^[\w\säüößÄÜÖẞ\s-_\.\+]*$", ErrorMessageResourceName = "ClubnameFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Clubname { get; set; } = string.Empty;


        [MaxLength(256, ErrorMessageResourceName = "RoleLengthErrorMax", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [MinLength(2, ErrorMessageResourceName = "RoleLengthErrorMin", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [RegularExpression(@"^[\w\säüößÄÜÖẞ\s-_\.\+]*$", ErrorMessageResourceName = "RoleFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoMatchReferee))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; } = string.Empty;

    }
}

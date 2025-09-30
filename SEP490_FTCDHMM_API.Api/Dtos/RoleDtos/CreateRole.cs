using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RoleDtos
{
    public class CreateRole
    {
        [Required(ErrorMessage = "Missing Name")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public required string Name { get; set; }
    }
}

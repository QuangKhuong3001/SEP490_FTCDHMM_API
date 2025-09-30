using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Dtos.RoleDtos
{
    public class PermissionToggleDto
    {
        [Required(ErrorMessage = "Missing PermissionActionId")]
        public Guid PermissionActionId { get; set; }
        public bool IsActive { get; set; } = false;
    }
}

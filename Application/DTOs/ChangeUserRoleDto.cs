using Domain.Enums;

namespace Application.DTOs;

public class ChangeUserRoleDto
{
    public int UserId { get; set; }
    public EUserPermissions NewRole { get; set; }
}
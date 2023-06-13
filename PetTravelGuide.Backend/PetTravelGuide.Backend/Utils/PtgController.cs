using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PetTravelGuide.Backend.Utils;

public class PtgController : ControllerBase
{
    private string? _requestUserEmail;
    private string? _requestUserRoles;
    private long? _requestUserId;
    
    protected string? RequestUserEmail
    {
        get
        {
            if (_requestUserEmail is not null) 
            {
                return _requestUserEmail;
            }
            string? payload = ControllerContext.HttpContext.User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(payload)) 
            {
                payload = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            _requestUserEmail = payload;
            return _requestUserEmail;
        }
    }

    protected long? RequestUserId
    {
        get
        {
            if (_requestUserId is not null)
            {
                return _requestUserId;
            }
            string? payload = ControllerContext.HttpContext.User.FindFirst("userId")?.Value;
            if (payload is not null)
            {
                _requestUserId = Convert.ToInt64(payload);
            }

            return _requestUserId;
        }
    }

    protected string? RequestUserRoles
    {
        get
        {
            if (_requestUserRoles is not null) 
            {
                return _requestUserRoles;
            }

            _requestUserRoles =
                (from c in ControllerContext.HttpContext.User.Claims
                    where c.Type == JwtTokenBuilder.Role
                    select c.Value).FirstOrDefault();
            
            return _requestUserRoles;
        }
    }
}
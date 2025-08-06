using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Models.Membership;
using System.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Authorization;

namespace YourNamespace.Controllers
{
    [Route("umbraco/backoffice/api/UserGroupPicker")]
    [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
    public class UserGroupPickerController : UmbracoAuthorizedApiController
    {
        private readonly IUserService _userService;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        public UserGroupPickerController(IUserService userService, IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
        {
            _userService = userService;
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var groups = _userService.GetAllUserGroups();

            var result = groups.Select(g => new
            {
                alias = g.Alias,
                name = g.Name
            });

            return Ok(result);
        }

        [HttpGet("CheckAdminAccess")]
        public IActionResult CheckAdminAccess()
        {
            var currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser == null)
            {
                return Ok(new { isAdmin = false });
            }

            var isAdmin = currentUser.Groups.Any(g => g.Alias.Equals("admin", StringComparison.OrdinalIgnoreCase));

            return Ok(new { isAdmin = isAdmin });
        }
    }
}
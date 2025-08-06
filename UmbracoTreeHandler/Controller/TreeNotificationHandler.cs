using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using System.Linq;
using Newtonsoft.Json;

public class TreeNotificationHandler : INotificationHandler<TreeNodesRenderingNotification>
{
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
    private readonly IContentService _contentService;
    private readonly IMediaService _mediaService;

    public TreeNotificationHandler(
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
        IContentService contentService,
        IMediaService mediaService)
    {
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        _contentService = contentService;
        _mediaService = mediaService;
    }

    public void Handle(TreeNodesRenderingNotification notification)
    {
        try
        {
            var currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser == null) return;

            var currentUserGroupAliases = currentUser.Groups.Select(g => g.Alias).ToList();
            var nodesToRemove = new List<Umbraco.Cms.Core.Trees.TreeNode>();

            switch (notification.TreeAlias)
            {
                case "content":
                    ProcessContentNodes(notification.Nodes, currentUserGroupAliases, nodesToRemove);
                    break;

                case "media":
                    ProcessMediaNodes(notification.Nodes, currentUserGroupAliases, nodesToRemove);
                    break;

                default:
                    // Do nothing for other tree types
                    return;
            }

            // Remove nodes that don't match user groups
            foreach (var nodeToRemove in nodesToRemove)
            {
                notification.Nodes.Remove(nodeToRemove);
            }
        }
        catch (Exception ex)
        {
            // Log the exception if you have logging configured
            // _logger.LogError(ex, "Error in TreeNotificationHandler");

            // Don't throw to prevent breaking the CMS
            return;
        }
    }

    private void ProcessContentNodes(ICollection<Umbraco.Cms.Core.Trees.TreeNode> nodes,
        List<string> currentUserGroupAliases,
        List<Umbraco.Cms.Core.Trees.TreeNode> nodesToRemove)
    {
        foreach (var node in nodes)
        {
            try
            {
                if (int.TryParse(node.Id.ToString(), out int contentId))
                {
                    var content = _contentService.GetById(contentId);
                    if (content != null)
                    {
                        var selectUserGroupsValue = content.GetValue("selectUserGroups");

                        if (ShouldHideNode(selectUserGroupsValue, currentUserGroupAliases))
                        {
                            nodesToRemove.Add(node);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Skip this node if there's an error
                continue;
            }
        }
    }

    private void ProcessMediaNodes(ICollection<Umbraco.Cms.Core.Trees.TreeNode> nodes,
        List<string> currentUserGroupAliases,
        List<Umbraco.Cms.Core.Trees.TreeNode> nodesToRemove)
    {
        foreach (var node in nodes)
        {
            try
            {
                if (int.TryParse(node.Id.ToString(), out int mediaId))
                {
                    var media = _mediaService.GetById(mediaId);
                    if (media != null)
                    {
                        // Check if the media type has the selectUserGroups property
                        if (media.HasProperty("selectUserGroups"))
                        {
                            var selectUserGroupsValue = media.GetValue("selectUserGroups");

                            if (ShouldHideNode(selectUserGroupsValue, currentUserGroupAliases))
                            {
                                nodesToRemove.Add(node);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Skip this node if there's an error
                continue;
            }
        }
    }

    private bool ShouldHideNode(object selectUserGroupsValue, List<string> currentUserGroupAliases)
    {
        if (selectUserGroupsValue != null)
        {
            var selectedUserGroups = ParseUserGroups(selectUserGroupsValue);

            if (selectedUserGroups.Any() &&
                !selectedUserGroups.Any(groupAlias => currentUserGroupAliases.Contains(groupAlias)))
            {
                return true;
            }
        }
        return false;
    }

    private List<string> ParseUserGroups(object selectUserGroupsValue)
    {
        var userGroups = new List<string>();

        if (selectUserGroupsValue == null) return userGroups;

        var valueString = selectUserGroupsValue.ToString();

        // Handle different possible formats of the multiple dropdown values
        try
        {
            // Case 1: JSON array format (common for multiple selection properties)
            if (valueString.StartsWith("[") && valueString.EndsWith("]"))
            {
                var jsonArray = JsonConvert.DeserializeObject<string[]>(valueString);
                if (jsonArray != null)
                {
                    userGroups.AddRange(jsonArray);
                }
            }
            // Case 2: Comma-separated values
            else if (valueString.Contains(","))
            {
                userGroups.AddRange(valueString.Split(',').Select(s => s.Trim()));
            }
            // Case 3: Single value
            else if (!string.IsNullOrWhiteSpace(valueString))
            {
                userGroups.Add(valueString.Trim());
            }
        }
        catch
        {
            // If parsing fails, try simple comma separation as fallback
            if (valueString.Contains(","))
            {
                userGroups.AddRange(valueString.Split(',').Select(s => s.Trim()));
            }
            else if (!string.IsNullOrWhiteSpace(valueString))
            {
                userGroups.Add(valueString.Trim());
            }
        }

        // Remove empty entries and return
        return userGroups.Where(g => !string.IsNullOrWhiteSpace(g)).ToList();
    }
}
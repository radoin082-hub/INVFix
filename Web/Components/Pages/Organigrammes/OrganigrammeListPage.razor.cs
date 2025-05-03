using Microsoft.AspNetCore.Components;
using ORG.App.Orgs;
using ORG.Domain.Entities.Node;
using OrgUis.Models;

namespace INV.Web.Components.Pages.Organigrammes;

public partial class OrganigrammeListPage : ComponentBase
{
   
    [Inject] private IOrgService orgService { set; get; }
    private NodeModel nodeModel = new();
    private List<Node> tree;
    private bool visible;
    private Node Parentcurrent;
    private string keyframes;
    protected override async Task OnInitializedAsync() => await loadTree();

    private async Task loadTree()
    {
        var result = await orgService.GetAllOrgs();
        if (result.IsSuccess)
        {
            tree = BuildTree(result.Value);
            StateHasChanged();
        }
    }

    private List<Node> BuildTree(List<Node> flat)
    {
        var lookup = flat.ToDictionary(n => n.Id.ToString(), n => n);
        var roots = new List<Node>();
        foreach (var n in flat)
        {
            var parentId = n.Id.GetAncestor(1).ToString();
            if (lookup.TryGetValue(parentId, out var p))
                p.Children.Add(n);
            else
                roots.Add(n);
        }

        return roots;
    }

    private async Task AddRoot() => await ShowPopup(null);
    private async Task AddChild(Node p) => await ShowPopup(p);

    private Task ShowPopup(Node parent)
    {
        Parentcurrent = parent;
        nodeModel = new NodeModel();
        visible = true;
        return Task.CompletedTask;
    }

    private async Task AddNode()
    {
        if (!string.IsNullOrWhiteSpace(nodeModel.Name) &&
            !string.IsNullOrWhiteSpace(nodeModel.Description))
        {
            await orgService.AddOrg(new Node
            {
                Name = nodeModel.Name,
                Type = nodeModel.NodeType,
                Description = nodeModel.Description
            }, Parentcurrent?.Id);

            visible = false;
            await loadTree();
        }
    }

    private void CancelAddNode() => visible = false;

    private void DropDownNode(Node n) => n.IsExpanded = !n.IsExpanded;

  
    private async Task NodeDropped((Node draggedNode, Node targetNode) data)
    {
        var (draggedNode, targetNode) = data;

        if (draggedNode is not null && targetNode is not null)
        {
          
            await orgService.UpdateNodeParent(draggedNode.Id, targetNode.Id);

        
            await loadTree();
        }
    }
}
using Microsoft.AspNetCore.Components;
using ORG.App.Orgs;
using ORG.Domain.Entities.Node;
using OrgUis.Models;

namespace INV.Web.Components.Pages.Organigrammes;

public partial class OrganigrammeListPage
{
    [Inject] private IOrgService orgService { set; get; }
    private string keyframes;
    private NodeModel nodeModel = new();
    private List<Node> tree;
    private bool visible;
    private Node Parentcurrent;
    private List<Node> nodelist = new List<Node>();
    protected override async Task OnInitializedAsync() => await loadTree();

    private async Task loadTree()
    {
        var result = await orgService.GetAllOrgs();
        if (result.IsSuccess)
        {
            tree = BuildTree(result.Value);
        }
    }

    private async Task AddRoot() => await ShowPopup(null);
    private async Task AddChild(Node p) => await ShowPopup(p);

    private Task
        ShowPopup(Node parent)
    {
        Parentcurrent = parent;
        nodeModel = new NodeModel();
        visible = true;
        return Task.CompletedTask;
    }

    private async Task AddNode()
    {
        var m = nodeModel;
        if (!string.IsNullOrWhiteSpace(m.Name) &&
            !string.IsNullOrWhiteSpace(m.Description))
        {
            await orgService.AddOrg(new Node
            {
                Name = m.Name,
                Type = m.NodeType,
                Description = m.Description
            }, Parentcurrent?.Id);

            visible = false;
            await loadTree();
        }
    }

    private void CancelAddNode() => visible = false;

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

    private void DropDownNode(Node n) => n.IsExpanded = !n.IsExpanded;
}
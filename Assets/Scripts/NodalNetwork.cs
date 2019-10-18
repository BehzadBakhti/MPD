using System;
using System.Collections;
using System.Collections.Generic;
using Analytics.Nonlinear;
using Mathematics.NL;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class NodalNetwork : MonoBehaviour
{
    private Node _defaultNode = new Node();
    public HashSet<Node> NetworkNodes;
    public HashSet<Link> NetworkLinks;
    private NonlinearSolver _solver;


    [SerializeField] private int _nodeCount = 0, _linkCount = 0;
    public Dictionary<Node, string> NodeEquations;
    public Dictionary<Link, string> LinkEquations;
    private NonlinearSystem _equationSystem;
    [SerializeField] private List<string> _allVariables;
    [SerializeField] private List<string> _allEquations;

    private void Awake()
    {
        NetworkNodes = new HashSet<Node>();
        NetworkLinks = new HashSet<Link>();
        NodeEquations = new Dictionary<Node, string>();
        LinkEquations = new Dictionary<Link, string>();
        
        _solver = new NewtonRaphsonSolver();
    }



    internal void Init(List<BaseTool> initialTools)
    {
        foreach (var tool in initialTools)
        {
            if (tool.CenterNode == null) continue;
            tool.CenterNode.Param = "h" + _nodeCount;
            AddNode(tool.CenterNode);

        }

    }
    public void CreateNode(Connection conn1, Connection conn2)
    {
        var nd = new Node();

        nd.AddLink(conn1.AttachedLink);
        nd.AddLink(conn2.AttachedLink);
        AddNode(nd);

    }


    private void AddNode(Node nd)
    {

        bool isNewNode = NetworkNodes.Add(nd);
        if (!isNewNode) return;

        nd.Param = "h" + _nodeCount;
        if (_nodeCount == 0) _defaultNode = nd;
        _nodeCount++;
        foreach (Link lnk in nd.Links)
        {
            bool isNewLink = NetworkLinks.Add(lnk);
            if (!isNewLink) continue;

            lnk.Param = "q" + _linkCount;
            _linkCount++;
            foreach (var lnkNode in lnk.Nodes)
            {
                AddNode(lnkNode);
            }

        }



    }

    public void RemoveLink(Link lnk)
    {
        foreach (var node in lnk.Nodes)
        {
            node.RemoveLink(lnk);
        }

        lnk.Nodes[0] = null;
        lnk.Nodes[1] = null;
        NetworkLinks.Remove(lnk);
    }

    private void GetEquationTree(Node rootNode)
    {
        if (NodeEquations.ContainsKey(rootNode)) return;

        NodeEquations.Add(rootNode, rootNode.GetEquation());
        _allVariables.Add(rootNode.Param);
        foreach (var lnk in rootNode.Links)
        {
            if (LinkEquations.ContainsKey(lnk)) continue;

            LinkEquations.Add(lnk, lnk.GetEquation());
            _allVariables.Add(lnk.Param);

            foreach (var node in lnk.Nodes)
            {
                if (node == rootNode) continue;
                GetEquationTree(node);
            }
        }


    }



    public void EqMatrix()
    {
        ValidateLinks();
        GetEquationTree(_defaultNode);

        foreach (var eq in LinkEquations)
        {
            _allEquations.Add(eq.Value);
        }
        foreach (var eq in NodeEquations)
        {
            _allEquations.Add(eq.Value);
        }
        
        _equationSystem = new AnalyticalSystem(_allVariables.ToArray(), _allEquations.ToArray());
    }


    private void ValidateLinks()
    {
        foreach (Link lnk in NetworkLinks)
        {
            if (!lnk.Validate())
            {
                CompleteNetwork(lnk);
            }
        }
    }

    void CompleteNetwork(Link lnk)
    {
        Node lastNode = new Node { Param = "h_out", Head = Fluids.BackPressure };
        lnk.AddNode(lastNode);
    }


}

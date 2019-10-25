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

    public Dictionary<string, CalculationParams> SolverInputData;

    [SerializeField] private NlEquationSolver _nlSolver;


    [SerializeField] private int _nodeCount = 0, _linkCount = 0;

   
    [SerializeField] private List<string> _allVariables;
    [SerializeField] private List<string> _allEquations;
    [SerializeField] private List<double> _initGuess;

    private void Awake()
    {
        NetworkNodes = new HashSet<Node>();
        NetworkLinks = new HashSet<Link>();
        SolverInputData= new Dictionary<string, CalculationParams>();

    }

    internal void Init(List<BaseTool> initialTools)
    {
        foreach (var tool in initialTools)
        {
            if (tool.CenterNode == null) continue;
            tool.CenterNode.NodeData.Param = "h" + _nodeCount;
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

        var isNewNode = NetworkNodes.Add(nd);
        if (!isNewNode) return;

        nd.NodeData.Param = "h" + _nodeCount;
        if (_nodeCount == 0) _defaultNode = nd;
        _nodeCount++;
        foreach (var lnk in nd.Links)
        {
            var isNewLink = NetworkLinks.Add(lnk);
            if (!isNewLink) continue;

            lnk.LinkData.Param = "q" + _linkCount;
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


    private void GetParametersTree(Node rootNode)
    {
        if (SolverInputData.ContainsKey(rootNode.NodeData.Param)) return;

        SolverInputData.Add(rootNode.NodeData.Param, rootNode.NodeData);

    
        foreach (var lnk in rootNode.Links)
        {
            if (SolverInputData.ContainsKey(lnk.LinkData.Param)) continue;
            SolverInputData.Add(lnk.LinkData.Param, lnk.LinkData);
            foreach (var node in lnk.Nodes)
            {
                if (node == rootNode) continue;
                GetEquationTree(node);
            }
        }
    }


    private void GetEquationTree(Node rootNode)
    {
        if (SolverInputData.ContainsKey(rootNode.NodeData.Param)) return;

        SolverInputData.Add(rootNode.NodeData.Param, rootNode.NodeData);


        foreach (var lnk in rootNode.Links)
        {
            if (SolverInputData.ContainsKey(lnk.LinkData.Param)) continue;
            SolverInputData.Add(lnk.LinkData.Param, lnk.LinkData);
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

        //foreach (var eq in LinkParams)
        //{
        //    _allEquations.Add(eq.Value);
        //}

        //foreach (var eq in NodeParams)
        //{
        //    _allEquations.Add(eq.Value);
        //}


        var initGuess=new double[_allEquations.Count];

        _nlSolver= new NlEquationSolver(_allVariables.ToArray(),_allEquations.ToArray(), initGuess);
        _nlSolver.Solve();

    }


    private void ValidateLinks()
    {
        foreach (var lnk in NetworkLinks)
        {
            if (!lnk.Validate())
            {
                CompleteNetwork(lnk);
            }
        }
    }

    void CompleteNetwork(Link lnk)
    {
        var lastNode = new Node { NodeData= new CalculationParams("h_out", "", 0) };
        lnk.AddNode(lastNode);
    }

 
}

[Serializable]
public struct CalculationParams
{

    public string Param { get; set; }
    public string Equation { get; set; }
    public double Value { get; set; }

    public CalculationParams(string param, string equation, double value)
    {
        Param = param;
        Equation = equation;
        Value = value;
    }
}

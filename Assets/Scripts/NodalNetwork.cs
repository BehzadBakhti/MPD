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

    public List<INetworkElement> SolverInputData;

    [SerializeField] private NlEquationSolver _nlSolver;


    [SerializeField] private int _nodeCount = 0, _linkCount = 0;


    [SerializeField] private List<string> _allVariables;
    [SerializeField] private List<string> _allEquations;
    [SerializeField] private List<float> _initGuess;

    private void Awake()
    {
        NetworkNodes = new HashSet<Node>();
        NetworkLinks = new HashSet<Link>();
        SolverInputData = new List<INetworkElement>();

    }
    private Dictionary<string, float> initialCondition;
   
    public void Init(List<BaseTool> initialTools)
    {
        foreach (var tool in initialTools)
        {
            if (tool.CenterNode == null) continue;
            // tool.CenterNode.NodeData.Param = "h" + _nodeCount;
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

        nd.ElementData.Param = "h" + _nodeCount;
        if (_nodeCount == 0) _defaultNode = nd;
        _nodeCount++;
        foreach (var lnk in nd.Links)
        {
            var isNewLink = NetworkLinks.Add(lnk);
            if (!isNewLink) continue;

            lnk.ElementData.Param = "q" + _linkCount;
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
        if (SolverInputData.Contains(rootNode)) return;
        SolverInputData.Add(rootNode);
        foreach (var lnk in rootNode.Links)
        {
            if (SolverInputData.Contains(lnk)) continue;
            SolverInputData.Add(lnk);
            foreach (var node in lnk.Nodes)
            {
                if (node == rootNode) continue;
                GetEquationTree(node);
            }
        }
    }

    public void CalculateHeads()
    {
        ValidateLinks();
        SolverInputData.Clear();
        GetEquationTree(_defaultNode);
        var initGuess = new List<float>();
        float[] results= new float[SolverInputData.Count];
        foreach (var item in SolverInputData)
        {
       
            float value = item.ElementData.Param.Contains("h") ? 0 : 100;
            initGuess.Add(value);// should be change with the Initial value of flowrate
            _allEquations.Add(item.GetEquation(value));
            _allVariables.Add(item.ElementData.Param);
        }

        while (!MPDUtility.CompareArrays(results, initGuess.ToArray(), 0.1f)){
         
            _nlSolver = new NlEquationSolver(_allVariables.ToArray(), _allEquations.ToArray(), initGuess.ToArray());
            results= _nlSolver.Solve();
            for (int i = 0; i < SolverInputData.Count; i++)
            {   
                _allEquations.Add(SolverInputData[i].GetEquation(results[i]));
                _allVariables.Add(SolverInputData[i].ElementData.Param);
            }
        }
        foreach (var item in initGuess)
        {
            print(item);
        }
        
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
        var lastNode = new Node { ElementData = new CalculationParams { Param="h_out"} };
        lnk.AddNode(lastNode);
    }


  
}

public static class MPDUtility
{
    public static bool CompareArrays(float[] arr1, float[] arr2, float epsilon)
    {
        if(arr1.Length!=arr2.Length)
        return false;

        float total = 0;
        for (int i = 0; i < arr1.Length; i++)
        {
            total += Mathf.Abs(arr1[i] - arr2[i]);
        }
        if (total < epsilon) return true;

        return false;
    }
}

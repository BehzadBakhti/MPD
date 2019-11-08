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
    private MpdData _data;

    public List<INetworkElement> SolverInputData;

    [SerializeField] private NlEquationSolver _nlSolver;


    [SerializeField] private int _nodeCount = 0, _linkCount = 0;

    [SerializeField] private List<string> _allVariables;
    [SerializeField] private List<string> _allEquations;
    [SerializeField] private float[] _initGuess;

    private void Awake()
    {
        NetworkNodes = new HashSet<Node>();
        NetworkLinks = new HashSet<Link>();
        SolverInputData = new List<INetworkElement>();

    }
    private Dictionary<string, float> _initialCondition;
   
    public void Init(List<BaseTool> initialTools, MpdData data)
    {
        _data = data;
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
                if(node.ElementData.Param=="0") continue;
                GetEquationTree(node);
            }
        }
    }

    public void CalculateHeads()
    {
        _allEquations=new List<string>();
        _allVariables= new List<string>();
        SolverInputData.Clear();
        ValidateLinks();
        
        GetEquationTree(_defaultNode);
       _initGuess = new float[SolverInputData.Count];
        var results= new float[SolverInputData.Count];
        for (var i = 0; i < SolverInputData.Count; i++)
        {
            var item = SolverInputData[i];
            var value = item.ElementData.Param.Contains("h") ? 0 : _data.FlowRate;
            _initGuess[i]=value;
            results[i] = value + 1;
            _allEquations.Add(item.GetEquation(value));
            _allVariables.Add(item.ElementData.Param);
        }

        while (!MPDUtility.CompareArrays(results, _initGuess, 0.1f))
        {
            results = _initGuess;
            _nlSolver = new NlEquationSolver(_allVariables.ToArray(), _allEquations.ToArray(), _initGuess);
            _initGuess= _nlSolver.Solve();
            for (var i = 0; i < SolverInputData.Count; i++)
            {   
                _allEquations[i]=SolverInputData[i].GetEquation(_initGuess[i]);
                _allVariables[i]=SolverInputData[i].ElementData.Param;
            }
        }
        foreach (var t in SolverInputData)
        {
            print(t.ElementData.Param+"="+ t.ElementData.Value);
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
        var lastNode = new Node { ElementData = new CalculationParams { Param="0"} };
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
        for (var i = 0; i < arr1.Length; i++)
        {
            total += Mathf.Abs(arr1[i] - arr2[i]);
        }
        return total < epsilon;
    }
}

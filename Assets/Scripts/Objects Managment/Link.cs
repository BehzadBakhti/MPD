using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Link : MonoBehaviour, INetworkElement
{


    public IPressureDropFunc PressureDrop; // dP= f(Q)

    public List<Node> Nodes { get; set; } = new List<Node>();
    public CalculationParams ElementData { get ; set ; }

    public float Friction;
    public bool IsOpen= true;


    private void OnEnable(){
        ElementData = new CalculationParams();
    }


    public void AddNode(Node n){
        Nodes.Add(n);
    }


    public bool Validate()
    {
        return this.Nodes.Count >= 2;
    }

    public  string GetEquation(float value)
    {
       // print(ElementData.Param+" | "+value);
        var fQ = GetComponentInParent<BaseTool>().HeadLossEq(ElementData.Param, value);
        var equation = Nodes[0].ElementData.Param + "-" + Nodes[1].ElementData.Param + "-" + fQ ;
        ElementData.Equation= equation;

        return equation;
    }
}


public interface  INetworkElement 
{
     CalculationParams ElementData { get; set; }
    

    string GetEquation(float value);
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Link :MonoBehaviour
{


    public CalculationParams LinkData;
    public IPressureDropFunc PressureDrop; // dP= f(Q)

    public List<Node> Nodes { get; set; } = new List<Node>();
   
    public double Friction;
    public bool IsOpen= true;


    private void Awake(){
    LinkData= new CalculationParams();
    }

    public string GetEquation(float flowRate){
    
        string fQ=GetComponentInParent<BaseTool>().HeadLossEq(LinkData.Param, flowRate);
        var equation= Nodes[0].NodeData.Param + "-" + Nodes[1].NodeData.Param + "-" + fQ + "=0";
        LinkData.Equation = equation;

        return equation;

    }

    public void AddNode(Node n){
        Nodes.Add(n);
    }


    public bool Validate()
    {
        return this.Nodes.Count >= 2;
    }

}

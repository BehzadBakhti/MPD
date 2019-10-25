using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node 
{


    public CalculationParams NodeData;
    public List<Link> Links = new List<Link>();

    public Node()
    {
        NodeData = new CalculationParams();
    }


    public string GetEquation(){

        foreach (var link in Links)
        {
           NodeData.Equation+=link.LinkData.Param+"-"; 
        }
        NodeData.Equation += "0=0";

        return NodeData.Equation;

    }

    public void AddLink(Link lnk){
        
        Links.Add(lnk);
        lnk.AddNode(this);

    
    }

    public void RemoveLink(Link lnk)
    {
        Links.Remove(lnk);
    }
}


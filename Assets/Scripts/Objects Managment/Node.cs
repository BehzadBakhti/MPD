using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node :INetworkElement
{



    public List<Link> Links = new List<Link>();

    public CalculationParams ElementData { get ;set; }

    public Node()
    {
        ElementData = new CalculationParams();
    }

    public string GetEquation(float value)
    {

        ElementData.Equation = "";
        foreach (var link in Links)
        {
           ElementData.Equation+=link.ElementData.Param+"-"; 
        }
        ElementData.Equation += "0";

        return ElementData.Equation;

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


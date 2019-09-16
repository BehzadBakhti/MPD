using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node 
{

    public string Param;
    public double Head;
    public string Equation="";
    public List<Link> Links = new List<Link>();


    public string GetEquation(){

        foreach (Link link in Links)
        {
           Equation+=link.Param+"-"; 
        }
        Equation+= "0=0";

        return Equation;

    }

    public void AddLink(Link lnk){
        Links.Add(lnk);
        lnk.AddNode(this);

    
    }

}


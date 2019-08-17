using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node 
{

    public string param;
    public float head;
    public string equation="";
    public List<Link> links = new List<Link>();


    public string GetEquation(){

        foreach (Link link in links)
        {
           equation+=link.param+"+"; 
        }
        equation+= "0=0";

        return equation;

    }

    public void AddLink(Link lnk){
        links.Add(lnk);
        if(lnk.nodes[0]==null){
            lnk.nodes[0]=this;

        }else{
            lnk.nodes[1]=this;
        }
    
    }

}


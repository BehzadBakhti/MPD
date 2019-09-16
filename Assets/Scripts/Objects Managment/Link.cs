using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Link :MonoBehaviour
{

    public string Param;
    public double FlowRate;
    public IPressureDropFunc PressureDrop; // dP= f(Q)

    public List<Node> Nodes { get; set; } = new List<Node>();
    public string Equation;
    public double Friction;
    public bool IsOpen= true;


    void Start(){
    
    }

    public string GetEquation(){
    
        string fQ=GetComponentInParent<BaseTool>().HeadLossEq(Param, FlowRate);
        Equation= Nodes[0].Param + "-" + Nodes[1].Param + "-" + fQ + "=0";

        return Equation;

    }

    public void AddNode(Node n){
        Nodes.Add(n);
    }

    public void Unlink(){
        for(int j = 0; j < Nodes.Count; j++)
        {

            for (int i = 0; i < Nodes[j].Links.Count; i++)
            {
                if (Nodes[j].Links[i]==this)
                {
                    Nodes[j].Links[i]=null;
                }  
            }
            Nodes[j]=null;

        }
    }

    public bool Validate(){
        if(this.Nodes.Count<2)
            return false;

        return true;
        
        }

}

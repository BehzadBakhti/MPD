using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Link :MonoBehaviour
{
    public string param;
    public double flowRate;
    public IPressureDropFunc pressureDrop; // dP= f(Q)
    public Node[] nodes= new Node[2];
    public string equation;
    public double friction;


void Start(){
 
}
    public string GetEquation(){

       string fQ=transform.root.GetComponent<BaseTool>().HeadLossEq(param, flowRate);
        equation= nodes[0].param+"-"+nodes[1].param+"-"+fQ;

        return equation;

    }


}

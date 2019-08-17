using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tee : BaseTool
{
  public Node teeNode;

    public override string HeadLossEq(string param, double flowRate)
    {
        throw new System.NotImplementedException();
    }

    void Start(){
  
     foreach (Connection conn in connections)
     {
        teeNode.links.Add(conn.attachedLink);
        conn.attachedLink.nodes[1]=teeNode;
     }
  }

}

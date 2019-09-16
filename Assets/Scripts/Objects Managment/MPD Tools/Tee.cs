using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tee : BaseTool
{
  public Node TeeNode= new Node();

    public override string HeadLossEq(string param, double flowRate)
    {
        return "";
    }

    void Start(){
  
     foreach (Connection conn in Connections)
     {
        TeeNode.Links.Add(conn.AttachedLink);
        conn.AttachedLink.AddNode(TeeNode);
     }
  }

}

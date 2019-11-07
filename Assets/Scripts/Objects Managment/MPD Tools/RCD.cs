using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rcd : BaseTool
{
    public Node RcdNode;
    
    void Start(){
        
            RcdNode.Links.Add(Connections[3].AttachedLink);
            Connections[3].AttachedLink.AddNode(RcdNode);
            RcdNode.ElementData.Param="h0";
 
    }

    public override string HeadLossEq(string param, float flowRate)
    {
       return "0" ;
    }

}

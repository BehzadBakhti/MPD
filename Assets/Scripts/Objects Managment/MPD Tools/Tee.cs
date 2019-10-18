using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tee : BaseTool
{


    protected override void Awake()
    {
        base.Awake();
        ViewType = typeof(TeePropertiesView);
    }

    public override string HeadLossEq(string param, double flowRate)
    {
        return "0";
    }

    private void Start(){
        CenterNode = new Node();
         foreach (Connection conn in Connections)
         {
            CenterNode.Links.Add(conn.AttachedLink);
            conn.AttachedLink.AddNode(CenterNode);
         }
    }

}

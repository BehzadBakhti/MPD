using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOP: BaseTool
{
    void Start()
    {
        CenterNode = new Node();
        foreach (var conn in Connections)
        {
            CenterNode.Links.Add(conn.AttachedLink);
            conn.AttachedLink.AddNode(CenterNode);
        }
    }

    public override string HeadLossEq(string param, float flowRate)
    {
        return "0";
    }
}



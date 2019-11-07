using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Valve : BaseTool, IClosable
{
    [RangeAttribute(0, 100)]
    public float Openness;

    public float InnerDiameter=9;
    public bool IsOpen;
    

    protected override void Awake()
    {
        base.Awake();
        ToolName = "Valve";
        ViewType = typeof(ValvePropertiesView);
    }

    public void Open()
    {
        if (this.Openness <= 95)
            this.Openness += 5f;
    }


    public void Close()
    {
        if (this.Openness >= 5)
            this.Openness += 5f;
    }

    public override string HeadLossEq(string param, float flowRate)
    {
   
        if (Openness <= 0)
        {
            return ("0");
        }
        //float velocity = 4 * flowRate / (Math.PI * InnerDiameter * InnerDiameter);
        //float rN = Fluids.Reynolds(Fluids.Density, velocity, InnerDiameter, Fluids.Viscosity);
        //float f = Fluids.FrictionFactor(rN, Roughness);
        //return (8 * f * Length / (Math.PI * Math.PI * 9.81 * Math.Pow(InnerDiameter, 5))).ToString("f5") + "*" + param + "^2";
        return "0";
    }
}

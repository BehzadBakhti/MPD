using System;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : BaseTool
{
    public float OuterDiameter=11;
    public float InnerDiameter=9;
    public float Roughness = 0.05f;
    public float Length = 5;

    protected override void Awake()
    {
        base.Awake();
        ViewType = typeof(PipePropertiesView);
    }
    public override string HeadLossEq(string param, float flowRate)
    {
        // H=8fLQ^2/(pi^2 * D^5 * g)
        float velocity = 4 * flowRate / (Mathf.PI * InnerDiameter * InnerDiameter);
        float rN = Fluids.Reynolds(Fluids.Density, velocity, InnerDiameter, Fluids.Viscosity);
        float f = Fluids.FrictionFactor(rN, Roughness);
        return (8 * f * Length / (Math.PI * Math.PI * 9.81 * Math.Pow(InnerDiameter, 5))).ToString("f5") + "*" + param + "^2";
    }
}

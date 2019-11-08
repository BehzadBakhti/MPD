using System;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : BaseTool
{
    public float OuterDiameter { get; set; } = 11;
    public float InnerDiameter { get; set; } = 9;
    public float Roughness { get; set; } = 0.05f;
    public float Length { get; set; } = 5;

    protected override void Awake()
    {
        ToolName = "Pipe";


        base.Awake();
        ViewType = typeof(PipePropertiesView);
    }
    public override string HeadLossEq(string param, float flowRate)
    {
        // H=8fLQ^2/(pi^2 * D^5 * g)
        var velocity = 4 * flowRate / (Mathf.PI * InnerDiameter * InnerDiameter);
        var rN = Fluids.Reynolds(Fluids.Density, velocity, InnerDiameter, Fluids.Viscosity);
        var f = Fluids.FrictionFactor(rN, Roughness);
       
        return (8 * f * Length / (Mathf.PI * Mathf.PI * 9.81 * Mathf.Pow(InnerDiameter, 5))).ToString("F5")+ "*" + param + "^2";
    }
}

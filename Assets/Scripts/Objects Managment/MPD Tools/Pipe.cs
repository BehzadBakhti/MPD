using System;
using System.Collections.Generic;
using UnityEngine;

public class Pipe: BaseTool
{
   
   public double InnerDiameter;
   public double Roughness;
   public double Length;

   protected override void Awake()
   {
       base.Awake();
       ViewType = typeof(PipePropertiesView);
   }
   public override string HeadLossEq(string param, double flowRate)
    {
      // H=8fLQ^2/(pi^2 * D^5 * g)
      double velocity=4*flowRate/(Math.PI*InnerDiameter*InnerDiameter);
      double rN= Fluids.Reynolds(Fluids.Density,velocity, InnerDiameter,Fluids.Viscosity );
      double f=Fluids.FrictionFactor(rN,Roughness);
      return (8*f*Length/(Math.PI*Math.PI*9.81*Math.Pow(InnerDiameter,5))).ToString("f5")+"*"+param+"^2";
    }
}

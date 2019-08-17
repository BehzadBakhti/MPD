using System;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : BaseTool
{
   public double innerDiameter;
   public double rughness;
   public double length;

    public override string HeadLossEq(string param, double flowRate)
    {
      // H=8fLQ^2/(pi^2 * D^5 * g)
      double velocity=4*flowRate/(Math.PI*innerDiameter*innerDiameter);
      double rN= Fluids.Reynolds(Fluids.density,velocity, innerDiameter,Fluids.viscosity );
      double f=Fluids.FrictionFactor(rN,rughness);
      return (8*f*length/(Math.PI*Math.PI*9.81*Math.Pow(innerDiameter,5))).ToString("f5")+"*"+param+"^2";
    }
}

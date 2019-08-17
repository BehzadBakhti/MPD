using System;
using System.Collections;
using System.Collections.Generic;

public static class Fluids
{
    public static double density=0.2;
    public static double viscosity=0.2;

        public static double FrictionFactor(double reynoldsN, double rughness){

        if(reynoldsN<2000){
           return reynoldsN/64;
        }

        double f_up=0.1f;
        double f_low=0.001f;
        double avg;
            while(f_up-f_low>0.000001f){
             avg= (f_up+ f_low)/2;
             double A= rughness/3.7;
             double B= reynoldsN*Math.Pow(avg, 0.5f);
            
             double E=1/Math.Pow(avg, 0.5);
             if(E+2*Math.Log10(rughness/3.71 +2.51/B)>0){
                 f_low=avg;
             }else{
                 f_up=avg;
             }
            
            }
        return f_up;
    }

        public static double Reynolds(double density, double velocity, double diameter, double viscosity){
            return density*velocity*diameter/viscosity;
        }
}
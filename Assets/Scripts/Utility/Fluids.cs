using System;
using System.Collections;
using System.Collections.Generic;

public static class Fluids
{
    public static double Density=0.2;
    public static double Viscosity=0.2;
    public static double BackPressure=0;

        public static double FrictionFactor(double reynoldsN, double roughness){

        if(reynoldsN<2000){
           return reynoldsN/64;
        }

        double fUp=0.1f;
        double fLow=0.001f;
        double avg;
            while(fUp-fLow>0.000001f){
             avg= (fUp+ fLow)/2;
             double a= roughness/3.71;
             double b= reynoldsN*Math.Pow(avg, 0.5f);
            
             double e=1/Math.Pow(avg, 0.5);
             if(e+2*Math.Log10(a +2.51/b)>0){
                 fLow=avg;
             }else{
                 fUp=avg;
             }
            
            }
        return fUp;
    }

        public static double Reynolds(double density, double velocity, double diameter, double viscosity){
            return density*velocity*diameter/viscosity;
        }
}
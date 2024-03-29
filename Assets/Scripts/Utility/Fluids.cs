using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Fluids
{
    public static float Density=0.2f;
    public static float Viscosity=0.2f;
    public static float BackPressure=0;

        public static float FrictionFactor(float reynoldsN, float roughness){

        if(reynoldsN<2000){
           return reynoldsN/64;
        }

        var fUp=0.1f;
        var fLow=0.001f;
        while(fUp-fLow>0.000001f){
             var avg = (fUp+ fLow)/2;
             var a= roughness/3.71f;
             var b= reynoldsN*Mathf.Pow(avg, 0.5f);
            
             var e=1/Mathf.Pow(avg, 0.5f);
             if(e+2*Math.Log10(a +2.51/b)>0){
                 fLow=avg;
             }else{
                 fUp=avg;
             }
            
        }
        return fUp;
    }

        public static float Reynolds(float density, float velocity, float diameter, float viscosity){
            //Debug.Log(density * velocity * diameter);
            return density*velocity*diameter/viscosity;
        }
}
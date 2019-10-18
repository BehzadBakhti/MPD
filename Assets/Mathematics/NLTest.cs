using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Reflection;
using Analytics.Nonlinear;
using Mathematics.NL;
using UnityEngine;

// namespace TestRunner
// {
//     /// <summary>
//     /// Interaction logic for App.xaml
//     /// </summary>
    public  class NLTest : MonoBehaviour
    {

// 			// variables of the analytical system
		public	string[] Variables = { "x", "y", "z" };
			// analytical expressions of the system's equations
		public	string[] Functions = { "x^2+y^2+z^2-1", // sphere
								   "x^2+y^2",     // paraboloid along the z axis
								   "-x+2*y^2+z^2"   // paraboloid along the x axis
								 };

		public	double[] X0;
			SolverOptions _options;
		public	double[] Result;
		public	double[] Expected;
			SolutionResult _actual;

			// creating nonlinear solver instance - Newton-Raphson solver.
			NonlinearSolver _solver = new NewtonRaphsonSolver();

			// initial guess for variable values
			



void Start(){
    
			NonlinearSystem system = new AnalyticalSystem(Variables, Functions);
          //  x0 = new double[] { 0.0, 5.0};
            
             _options = new SolverOptions()
			{
				MaxIterationCount = 100,
				SolutionPrecision = 1e-5
			};



            Result = null;
			// solving the system
			_actual = _solver.Solve(system, X0, _options, ref Result);

			 // expected values
			// printing solution result into console out
			
        }


 }

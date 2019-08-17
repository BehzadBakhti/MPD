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
		public	string[] variables = { "x", "y", "z" };
			// analytical expressions of the system's equations
		public	string[] functions = { "x^2+y^2+z^2-1", // sphere
								   "x^2+y^2-z",     // paraboloid along the z axis
								   "-x+2*y^2+z^2"   // paraboloid along the x axis
								 };

		public	double[] x0;
			SolverOptions options;
		public	double[] result;
		public	double[] expected;
			SolutionResult actual;

			// creating nonlinear solver instance - Newton-Raphson solver.
			NonlinearSolver solver = new NewtonRaphsonSolver();

			// initial guess for variable values
			



void Start(){
    
			NonlinearSystem system = new AnalyticalSystem(variables, functions);
          //  x0 = new double[] { 0.0, 5.0};
            
             options = new SolverOptions()
			{
				MaxIterationCount = 100,
				SolutionPrecision = 1e-5
			};



            result = null;
			// solving the system
			actual = solver.Solve(system, x0, options, ref result);

			expected = new double[] { -4d/3d, -5d/3d }; // expected values
			// printing solution result into console out
			
        }


 }

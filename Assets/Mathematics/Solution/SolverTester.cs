using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mathematics.NL;
using Analytics.Nonlinear;

namespace Analytics.Tests
{
    /// <summary>
    /// Tester for nonlinear solver.
    /// </summary>
    public static class SolverTester
    {
        /// <summary>
        /// Runs all tests.
        /// </summary>
        public static void RunTests()
        {
            SolverTests();
        }

        private static NonlinearSolver CreateNewton()
        {
            NewtonRaphsonSolver s = new NewtonRaphsonSolver();
            return s;
        }

        private static SolverOptions CreateOptions()
        {
            SolverOptions opt = new SolverOptions();

            opt.MaxIterationCount = 100;
            opt.SolutionPrecision = 1e-5;
            //opt.Norm = new EuclidianNorm();

            return opt;
        }

        private static void PrintNlResult(SolutionResult r, double[] result, double[] expected, double prec)
        {
            string s = "ANALYTIC NL Solution:";

            s = s + Environment.NewLine + "Converged: " + r.Converged.ToString();

            string s1 = "RESULT:";
            string s2 = "EXPECT:";
            int l = result.Length;

            bool b = true;
            for (int i = 0; i < l; i++)
            {
                s1 = s1 + " " + result[i].ToString();
                s2 = s2 + " " + expected[i].ToString();
                if (Math.Abs(result[i] - expected[i]) > prec)
                {
                    b = false;
                }
            }

            s = s + Environment.NewLine + s1 + Environment.NewLine + s2 + Environment.NewLine;

            if (b)
            {
                s = s + "SUCCESS";
            }
            else
            {
                s = s + "ERROR: " + r.Message;
            }

            Console.Out.WriteLine(Environment.NewLine + s);
        }

        private static string Circle()
        { 
            return "x^2+y^2-1";
        }

        private static string Parabola()
        {
            return "x^2-y";
        }

        private static string Sphere()
        {
            return "x^2+y^2+z^2-1";
        }

        private static string ParaboloidZ()
        {
            return "x^2+y^2-z";
        }

        private static string ParaboloidX()
        {
            return "-x+2*y^2+z^2";
        }

        private static void SolverTests()
        {
            NonlinearSolver solver;
            string f1;
            string f2;
            string f3;
            string v1;
            string v2;
            string v3;

            string[] functions;
            string[] variables;
            
            NonlinearSystem system;
            double[] x0;
            SolverOptions options;
            double[] result;
            double[] expected;
            SolutionResult actual;

            f1 = Circle();
            f2 = Parabola();
            v1 = "x";
            v2 = "y";
            solver = CreateNewton();
            variables = new string[] { v1, v2 };
            functions = new string[] { f1, f2 };
            system = new AnalyticalSystem(variables, functions);
            expected = new double[] { 0.7861513778, 0.6180339887 };
            x0 = new double[] { 0.2, 0.2 };
            options = CreateOptions();
            result = null;
            actual = solver.Solve(system, x0, options, ref result);
            PrintNlResult(actual, result, expected, options.SolutionPrecision);

            f1 = Sphere();
            f2 = ParaboloidZ();
            f3 = ParaboloidX();
            v1 = "x";
            v2 = "y";
            v3 = "z";
            solver = CreateNewton();
            variables = new string[] { v1, v2, v3 };
            functions = new string[] { f1, f2, f3 };
            system = new AnalyticalSystem(variables, functions);
            expected = new double[] { 0.6835507455, 0.3883199288, 0.6180339887 };
            x0 = new double[] { 1.0, 1.0, 1.0 };
            options = CreateOptions();
            result = null;
            actual = solver.Solve(system, x0, options, ref result);
            PrintNlResult(actual, result, expected, options.SolutionPrecision);
        }
    }
}

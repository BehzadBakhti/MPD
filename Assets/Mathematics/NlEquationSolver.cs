using Analytics.Nonlinear;
using Mathematics.NL;
using System;

public class NlEquationSolver
{

    private readonly string[] _variables;// = { "x", "y", "z" };
    private readonly string[] _functions;// = { "x^2+y^2+z^2-1", "x^2+y^2", "-x+2*y^2+z^2" };

    private readonly float[] _initGuess;

    private double[] _result;
    private float[] _expected;
    private SolverOptions _options;
    private SolutionResult _actual;
    // creating nonlinear solver instance - Newton-Raphson solver.
    private readonly NonlinearSolver _solver = new NewtonRaphsonSolver();

    public NlEquationSolver(string[] variables, string[] functions, float[] initGuess)
    {
        _initGuess = initGuess;
        _functions = functions;
        _variables = variables;
    }


    public float[] Solve()
    {
        NonlinearSystem system = new AnalyticalSystem(_variables, _functions);

        _options = new SolverOptions()
        {
            MaxIterationCount = 100,
            SolutionPrecision = 1e-5
        };

        _result = null;
        // solving the system
        double[] guess= Array.ConvertAll(_initGuess, x => (double)x);

        _actual = _solver.Solve(system, guess, _options, ref _result);
        return Array.ConvertAll(_result, x => (float)x); 
        // expected values
        // printing solution result into console out

    }


}

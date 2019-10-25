using Analytics.Nonlinear;
using Mathematics.NL;

public class NlEquationSolver
{

    private readonly string[] _variables;// = { "x", "y", "z" };
    private readonly string[] _functions;// = { "x^2+y^2+z^2-1", "x^2+y^2", "-x+2*y^2+z^2" };

    private readonly double[] _initGuess;

    private double[] _result;
    private double[] _expected;
    private SolverOptions _options;
    private SolutionResult _actual;
    // creating nonlinear solver instance - Newton-Raphson solver.
    private readonly NonlinearSolver _solver = new NewtonRaphsonSolver();

    public NlEquationSolver(string[] variables, string[] functions, double[] initGuess)
    {
        _initGuess = initGuess;
        _functions = functions;
        _variables = variables;
    }


    public void Solve()
    {
        NonlinearSystem system = new AnalyticalSystem(_variables, _functions);

        _options = new SolverOptions()
        {
            MaxIterationCount = 100,
            SolutionPrecision = 1e-5
        };

        _result = null;
        // solving the system
        _actual = _solver.Solve(system, _initGuess, _options, ref _result);

        // expected values
        // printing solution result into console out

    }


}

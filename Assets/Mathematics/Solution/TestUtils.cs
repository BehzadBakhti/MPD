using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using Mathematics.Fractions;

namespace Analytics.Tests
{
    public delegate bool CompareEqual<T>(T expected, T actual);

    public static class TestUtils
    {
        private static double precision;
        private static double bigpositive;
        private static double bignegative;

        static TestUtils()
        {
            Precision = 1e-12;
        }

        private static bool IsBigPositive(double expected)
        {
            return expected >= bigpositive;
        }

        private static bool IsBigNegative(double expected)
        {
            return expected <= bignegative;
        }

        /// <summary>
        /// Comparison precision
        /// </summary>
        public static double Precision
        {
            get
            {
                return precision;
            }
            set
            {
                if (value > 0.0)
                {
                    precision = value;
                    bigpositive = 1.0 / precision;
                    bignegative = -1.0 / precision;
                }
                else
                {
                    throw new ArgumentException("Precision must be positive.");
                }
            }
        }

        /// <summary>
        /// Compares two doubles with given precision.
        /// Takes into account infinities (negative and positive) and NaNs.
        /// WARNING: Do not confuse expected and actual arguments.
        ///          Expected is really what is expected.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool CompareReal(double expected, double actual)
        { 
            if (double.IsNaN(expected) && double.IsNaN(actual)) return true;
            if (double.IsNaN(expected) || double.IsNaN(actual)) return false;

            if (double.IsPositiveInfinity(expected))
            {
                if (double.IsPositiveInfinity(actual)) return true;

                return IsBigPositive(actual);
            }

            if (double.IsNegativeInfinity(expected))
            {
                if (double.IsNegativeInfinity(actual)) return true;

                return IsBigNegative(actual);
            }

            return Math.Abs(expected - actual) <= precision;
        }

        /// <summary>
        /// Compares two Complex with given precision.
        /// Takes into account infinities (negative and positive) and NaNs.
        /// WARNING: Do not confuse expected and actual arguments.
        ///          Expected is really what is expected.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool CompareComplex(Complex expected, Complex actual)
        {
            return ((CompareReal(expected.Real, actual.Real) && CompareReal(expected.Imaginary, actual.Imaginary)));
        }

        /// <summary>
        /// Compares two fractions
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool CompareFraction(Fraction expected, Fraction actual)
        {
            return expected == actual;
        }
    }
}

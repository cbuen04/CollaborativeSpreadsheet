
using System;
using FormulaEvaluator;

namespace FormulaEvaluatorTest
{
    public class Program
    {
        public static int simpleLookup(string s)
        {
            if (s == "a1")
                return 5;
            if (s == "A2")
                return 8;
            else
                throw new ArgumentException();
        }

        public static void TestSimpleExpression()
        {
            Evaluator.Evaluate("3 + 5 + 7", simpleLookup);
        }
        static void Main(string[] args)
        {
            Console.WriteLine(Evaluator.Evaluate("3 + 5 + 7", simpleLookup));
        }
    }
}

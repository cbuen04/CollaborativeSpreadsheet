using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{

    /// <summary>
    /// This class is an expression evaluator which evaluates integer arithmetic expressions using standard infix notation. The algorithm is implemented and allows the user to apply their own lookup function to assign variables to values. 
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(String v);
        private static Stack<String> Operators = new Stack<String>();
        private static Stack<int> Values = new Stack<int>();


        /// <summary>
        /// This method takes in the expression to be evaluated and returns an integer representing the computed result.
        /// </summary>
        /// <param name="exp">The input user's expression</param>
        /// <param name="variableEvaluator">The lookup function which assigns values to assigned variables</param>
        /// <returns>Computed integer result</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            String[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"); 
            foreach (String substr in substrings)
            {
                String type = DetermineType(substr);
                // Case of an operator: (+,-,/,*)
                if (type == "operator")
                {
                    if (substr == "+" || substr == "-")
                    {
                        if (Operators.Count > 0 && Operators.Peek() == "+")
                        {
                            Operate("+");
                        }
                        else if (Operators.Count > 0 && Operators.Peek() == "-")
                        {
                            Operate("-");
                        }
                        Operators.Push(substr);
                    }
                    else if (substr == "*" || substr == "/")
                    {
                        Operators.Push(substr);
                    }
                    else if (substr == "(")
                    {
                        Operators.Push(substr);
                    }
                    else if (substr == ")")
                    {
                        if (Operators.Count > 0 && Operators.Peek() == "+")
                        {
                            Operate("+");
                        }
                        else if (Operators.Count > 0 && Operators.Peek() == "-")
                        {
                            Operate("-");
                        }
                        else if (Operators.Count > 0 && Operators.Peek() == "/")
                        {
                            Operate("/");
                        }
                        else if (Operators.Count > 0 && Operators.Peek() == "*")
                        {
                            Operate("*");
                        }    
                        // Tests if this is a valid parenthesis expression
                        if ((Operators.Count > 0 && !(Operators.Peek() == "(")) || Operators.Count == 0)
                            throw new ArgumentException("Invalid expression. No opening parenthesis detected/invalid expression");
                        Operators.Pop();
                        if (Operators.Count > 0 && (Operators.Peek() == "/" || Operators.Peek() == "*"))
                        {
                            if (Operators.Peek() == "/")
                            {
                                Operate("/");
                            }
                            else if (Operators.Peek() == "*")
                            {
                                Operate("*");
                            }
                        }
                    }
                }

                // Case of integer or variable token 
                if (type == "integer" || type == "variable")
                {
                    // NumericValue will store either the searched variable value expression or the integer value 
                    int NumericValue = 0;
                    if (type == "integer")
                        NumericValue = int.Parse(substr);
                    else
                    {
                        try
                        {
                            string trimmedSubstr = substr.Replace(" ", "");
                            NumericValue = variableEvaluator(trimmedSubstr);
                        }
                        catch (ArgumentException)
                        {
                            throw new ArgumentException("No value was found for the variable detected");
                        }
                    }
                    if (Operators.Count > 0 && Operators.Peek() == "/")
                    {
                        int second = NumericValue;
                        if (Values.Count == 0)
                            throw new ArgumentException();
                        int first = Values.Pop();
                        if (second == 0)
                            throw new ArgumentException("Illegal division by zero attempted");
                        Operators.Pop();
                        Values.Push(first / second);
                    }
                    else if (Operators.Count > 0 && Operators.Peek() == "*")
                    {
                        int second = NumericValue;
                        if (Values.Count == 0)
                            throw new ArgumentException();
                        int first = Values.Pop();
                        Operators.Pop();
                        Values.Push(first * second);
                    }
                    else
                        Values.Push(NumericValue);
                }
            }

            // Once all tokens have been processed
            if (Values.Count == 1 && Operators.Count == 0)
            {
                return Values.Pop();
            }
            else if (Operators.Count == 1)
            {
                if (Operators.Peek() == "+")
                {
                    Operate("+");
                }
                else if (Operators.Peek() == "-")
                {
                    Operate("-");
                }
            }
            else
                throw new ArgumentException("Invalid expression");
            return Values.Pop();
        }

        /// <summary>
        /// Private helper method which returns what type an individual term in the expression is
        /// </summary>
        /// <param name="str">The substring which represents an individual term in the total input expression</param>
        /// <returns>The term type: either an integer, operator, variable, or skip</returns>
        private static string DetermineType(String str)
        {
            if (int.TryParse(str, out _))
            {
                return "integer";
            }
            if (str == "+" || str == "-" || str == "*" || str == "/" || str == "(" || str == ")")
            {
                return "operator";
            }
            else if (!(str == "" || str == " "))
            {
                Boolean CheckingLetters = true;
                Boolean CheckingNumbers = false;
                // removing valid empty spaces, empty spaces before and after variable token
                string trimmedStr = str.Trim();
                if (!Char.IsLetter(trimmedStr[0]))
                {
                    throw new ArgumentException("An illegal input value was used in the expression");
                }
                foreach (char c in trimmedStr)
                {
                    if (CheckingLetters && !Char.IsLetter(c))
                    {
                        if (!Char.IsDigit(c))
                            throw new ArgumentException("An illegal input value was used in the expression");
                        if (Char.IsDigit(c))
                            CheckingLetters = false;
                    }
                    if (!CheckingLetters)
                    {
                        if (Char.IsDigit(c))
                            CheckingNumbers = true; 
                        if (!Char.IsDigit(c))
                            throw new ArgumentException("An illegal input value was used in the expression");
                    } 
                }
                if(!CheckingNumbers)
                    throw new ArgumentException("An illegal input value was used in the expression");
                return "variable";
            }
            //These tokens can be ignored so long as they are not sandwiched improperly.
            if (str == "" || str == " ")
                return "skip";
            else
                throw new ArgumentException("An illegal input value was used in the expression");
        }


        /// <summary>
        /// This private helper method computes the "operation" step, popping 2 values in the value stack and computing the specified operation on them, returning them to the stack when finished
        /// </summary>
        /// <param name="operation">he string that has been previously determined to be an operation of type +,-,*,/. This string will determine the respective operation</param>
        private static void Operate(string operation)
        {
            if (Values.Count < 2)
                throw new ArgumentException("Invalid expression, illegal number of values provided");
            int second = Values.Pop();
            int first = Values.Pop();
            Operators.Pop();
            if (operation == "+")
                Values.Push(first + second);
            if (operation == "-")
                Values.Push(first - second);
            if (operation == "/")
            {
                if (second == 0)
                    throw new ArgumentException("Illegal division by zero attempted");
                Values.Push(first / second);
            }
            if (operation == "*")
                Values.Push(first * second);
        }
    }
}

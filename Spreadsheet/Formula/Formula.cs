// Skeleton written by Joe Zachary for CS 3500, September 2013

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens
// Authors: Daniel Kopta, Joe Zachary, Emma Kerr



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<string> tokens;
        private Func<string, string> normalizeFunc;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            tokens = new List<String>(GetTokens(formula));
            try
            {
                CheckStandardValidity(tokens);

            }
            catch
            {
                throw new FormulaFormatException("Improper formula format detected");
            }

            normalizeFunc = normalize;
            foreach (string token in tokens)
            {
                // only check variables, ignores numerical values and opperators
                if (StandardValidType(token) == "variable")
                {

                    // Checking validity after tokens are normalized
                    string normalizedToken = normalizeFunc(token);
                    if (StandardValidType(normalizedToken) != "variable")
                    {
                        throw new FormulaFormatException("Invalid token type detected after normalization to fit standards of the spreadsheet");
                    }
                    if (!isValid(normalizedToken))
                    {
                        throw new FormulaFormatException("Invalid token type detected after normalization to fit standards of the spreadsheet and input validity specifications");
                    }
                }
            }

        }



        /// <summary>
        /// This is a private helper method to check validity based on the standard and rules outlined in the assignment. 
        /// </summary>
        /// <param name="tokens"></param>
        private static void CheckStandardValidity(List<String> tokens)
        {
            // (2) One token rule
            if (tokens.Count() == 0)
            {
                FormulaFormatException exception = new FormulaFormatException("Improper formula structure: No tokens detected"); // write comment here?
                throw exception;
            }
            Boolean atStartingToken = true;
            int openingParenthCount = 0;
            int closingParenthCount = 0;
            Boolean lastTokenSymbol = false; // Flag to indicate the previous token was a symbol
            Boolean extraFollowing = false;
            foreach (string token in tokens)
            {
                string tokenType = StandardValidType(token);
                // (5) Starting token rule
                if (atStartingToken)
                {
                    if (!((tokenType == "number") || (tokenType == "variable" || token == "(")))
                        throw new FormulaFormatException("Improper formula structure: No tokens detected : did not start with a variable or number");
                    atStartingToken = false;
                }
                // (1) Parsing check
                if (tokenType == "invalid")
                    throw new FormulaFormatException("Invalid token detected");
                // (7) Parenthesis/Operator following rule
                if (lastTokenSymbol)
                {
                    if (!(tokenType == "variable" || tokenType == "number" || token == "("))
                    {
                        throw new FormatException("Improper formula sequence detected");
                    }
                    lastTokenSymbol = false;
                }
                // (8) Extra following rule
                if (extraFollowing)
                {
                    if (!(tokenType == "operator" || token == ")")) // helper? 
                    {
                        throw new FormatException("Invalid formula sequence detected");
                    }
                    extraFollowing = false;
                }

                if (token == "(")
                {
                    openingParenthCount++;
                }
                if (token == ")")
                {
                    closingParenthCount++;
                }
                // (3) Right parenthesis rule
                if (closingParenthCount > openingParenthCount)
                    throw new FormulaFormatException("Improper formula structure: too many closing parenthesis");

                if (tokenType == "operator" || token == "(")
                {
                    lastTokenSymbol = true;
                }
                if (tokenType == "number" || tokenType == "variable" || token == ")")
                {
                    extraFollowing = true;
                }
            }
            // (4) Balanced parenthesis rule
            if (!(closingParenthCount == openingParenthCount))
                throw new FormulaFormatException("Mismatch of closing/opening parenthesis");
            // (6) Ending token rule
            String lastToken = tokens.Last<string>();
            if (!((StandardValidType(lastToken) == "number") || ((StandardValidType(lastToken) == "variable")) || lastToken == ")")) // logic??? lmfao head empty just vibes
            {
                throw new FormulaFormatException("Improper final token");
            }
        }

        /// <summary>
        /// Private helper method which returns what type an individual term in the expression is
        /// </summary>
        /// <param name="str">The substring which represents an individual term in the total input expression</param>
        /// <returns>The term type: either an integer, operator, variable, or skip</returns>
        private static String StandardValidType(String str)
        {
            if (Double.TryParse(str, out _))
            {
                return "number";
            }
            if (str == "+" || str == "-" || str == "*" || str == "/")
            {
                return "operator";
            }
            if (str == "(" || str == ")")
            {
                return "parenthesis";
            }
            else
            {
                // variables that consist of a letter or underscore followed by zero or more letters, underscores, or digits
                String validVarPattern = "^[a-zA-Z_][a-zA-Z0-9_]*$";
                if (Regex.IsMatch(str, validVarPattern))
                    return "variable";
                return "invalid";
            }

        }



        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<String> Operators = new Stack<string>();
            Stack<double> Values = new Stack<double>();
            foreach (String token in tokens)
            {
                String type = StandardValidType(token);
                // Case of an operator: (+,-,/,*)
                if (type == "operator" || type == "parenthesis")
                {
                    if (token == "+" || token == "-")
                    {
                        if (Operators.Count > 0 && Operators.Peek() == "+")
                        {
                            Operate("+", Operators, Values);
                        }
                        else if (Operators.Count > 0 && Operators.Peek() == "-")
                        {
                            Operate("-", Operators, Values);
                        }
                        Operators.Push(token);
                    }
                    else if (token == "*" || token == "/")
                    {
                        Operators.Push(token);
                    }
                    else if (token == "(")
                    {
                        Operators.Push(token);
                    }
                    else if (token == ")")
                    {
                        if (Operators.Count > 0 && Operators.Peek() == "+")
                        {
                            Operate("+", Operators, Values);
                        }
                        else if (Operators.Count > 0 && Operators.Peek() == "-")
                        {
                            Operate("-", Operators, Values);
                        }
                        Operators.Pop();
                        if (Operators.Count > 0 && Operators.Peek() == "/")
                        {
                            try
                            {
                                Operate("/", Operators, Values);
                            }
                            catch
                            {
                                return new FormulaError("Illegal Attempt to divide by zero detected");
                            }
                        }
                        else if (Operators.Count > 0 && Operators.Peek() == "*")
                        {
                            Operate("*", Operators, Values);
                        }
                    }
                }

                // Case of integer or variable token 
                if (type == "number" || type == "variable")
                {
                    // NumericValue will store either the searched variable value expression or the integer value 
                    double NumericValue = 0.0;
                    if (type == "number")
                        NumericValue = Double.Parse(token);
                    else
                    {
                        try
                        {
                            NumericValue = lookup(normalizeFunc(token));
                        }
                        catch (ArgumentException)
                        {
                            return new FormulaError("No variable found in lookup function provided");
                        }
                    }
                    if (Operators.Count > 0 && Operators.Peek() == "/")
                    {
                        double second = NumericValue;
                        double first = Values.Pop();
                        if (second == 0)
                            return new FormulaError("Illegal Attempt to divide by zero detected");
                        Operators.Pop();
                        Values.Push(first / second);
                    }
                    else if (Operators.Count > 0 && Operators.Peek() == "*")
                    {
                        double second = NumericValue;
                        double first = Values.Pop();
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
                    Operate("+", Operators, Values);
                }
                else if (Operators.Peek() == "-")
                {
                    Operate("-", Operators, Values);
                }
            }
            return Values.Pop();
        }

        /// <summary>
        /// This private helper method computes the "operation" step, popping 2 values in the value stack and computing the specified operation on them, returning them to the stack when finished
        /// </summary>
        /// <param name="operation">he string that has been previously determined to be an operation of type +,-,*,/. This string will determine the respective operation</param>
        private static void Operate(string operation, Stack<String> Operators, Stack<double> Values)
        {
            double second = Values.Pop();
            double first = Values.Pop();
            Operators.Pop();
            if (operation == "+")
                Values.Push(first + second);
            if (operation == "-")
                Values.Push(first - second);
            if (operation == "/")
            {
                if (second == 0)
                    throw new ArgumentException();
                Values.Push(first / second);
            }
            if (operation == "*")
                Values.Push(first * second);
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<String> variablesHash = new HashSet<string>();
            foreach (string token in tokens)
            {
                if (StandardValidType(token) == "variable")
                {
                    string normalizedToken = normalizeFunc(token);
                    _ = variablesHash.Add(normalizedToken);
                }
            }
            return variablesHash;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            List<string> normalizedString = new List<string>();
            foreach (string token in tokens)
            {
                if (StandardValidType(token) == "variable")
                {
                    string normalizedToken = normalizeFunc(token);
                    normalizedString.Add(normalizedToken);
                }
                else if (StandardValidType(token) == "number")
                {
                    // Normalize the number to be standard with decimals
                    string numberToken = double.Parse(token).ToString();
                    normalizedString.Add(numberToken);
                }
                else
                    normalizedString.Add(token);
            }
            return string.Join("", normalizedString);
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null) || !(obj is Formula))
                return false;
            return ToString() == obj.ToString();
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null))
                return ReferenceEquals(f2, null);
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            // Case where one or both are null
            if (ReferenceEquals(f1, null))
                return !(ReferenceEquals(f2, null));
            return !(f1.Equals(f2));
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
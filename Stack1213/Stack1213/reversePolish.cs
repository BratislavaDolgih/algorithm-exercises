using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

public class RPNCalculator
{
    private Dictionary<string, double> variables;

    public RPNCalculator()
    {
        variables = new Dictionary<string, double>();
    }

    public void SetVariable(string name, double value)
    {
        variables[name] = value;
    }

    private enum TokenType
    {
        Number,
        Variable,
        Operator,
        Function,
        LeftParen,
        RightParen
    }

    private class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    private int GetPrecedence(string op)
    {
        switch (op)
        {
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
            case "%":
            case "div":
                return 2;
            case "^":
                return 3;
            case "sqrt":
            case "abs":
            case "sign":
            case "sin":
            case "cos":
            case "tan":
            case "ln":
            case "lg":
            case "exp":
            case "floor":
                return 4;
            case "min":
            case "max":
                return 4;
            default:
                return 0;
        }
    }

    private bool IsRightAssociative(string op)
    {
        return op == "^";
    }

    private bool IsFunction(string token)
    {
        return token == "sqrt" || token == "abs" || token == "sign" ||
               token == "sin" || token == "cos" || token == "tan" ||
               token == "ln" || token == "lg" || token == "exp" ||
               token == "floor" || token == "min" || token == "max";
    }

    private bool IsBinaryFunction(string token)
    {
        return token == "min" || token == "max";
    }

    private MyStack<Token> Tokenize(string expression)
    {
        MyStack<Token> tokens = new MyStack<Token>();
        MyStack<Token> tempStack = new MyStack<Token>();

        int i = 0;
        while (i < expression.Length)
        {
            char c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c) || c == '.')
            {
                StringBuilder number = new StringBuilder();
                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                {
                    number.Append(expression[i]);
                    i++;
                }
                tempStack.Push(new Token(TokenType.Number, number.ToString()));
            }
            else if (char.IsLetter(c))
            {
                StringBuilder word = new StringBuilder();
                while (i < expression.Length && char.IsLetterOrDigit(expression[i]))
                {
                    word.Append(expression[i]);
                    i++;
                }
                string w = word.ToString();

                if (IsFunction(w))
                {
                    tempStack.Push(new Token(TokenType.Function, w));
                }
                else
                {
                    tempStack.Push(new Token(TokenType.Variable, w));
                }
            }
            else if (c == '(')
            {
                tempStack.Push(new Token(TokenType.LeftParen, "("));
                i++;
            }
            else if (c == ')')
            {
                tempStack.Push(new Token(TokenType.RightParen, ")"));
                i++;
            }
            else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '%')
            {
                tempStack.Push(new Token(TokenType.Operator, c.ToString()));
                i++;
            }
            else
            {
                throw new ArgumentException($"Unknown character: {c}");
            }
        }

        while (!tempStack.Empty())
        {
            tokens.Push(tempStack.Pop());
        }

        return tokens;
    }

    public string ConvertToRPN(string expression)
    {
        MyStack<Token> tokens = Tokenize(expression);
        MyStack<string> output = new MyStack<string>();
        MyStack<string> operators = new MyStack<string>();

        while (!tokens.Empty())
        {
            Token token = tokens.Pop();

            if (token.Type == TokenType.Number)
            {
                output.Push(token.Value);
            }
            else if (token.Type == TokenType.Variable)
            {
                if (!variables.ContainsKey(token.Value))
                {
                    throw new ArgumentException($"Variable '{token.Value}' is not defined");
                }
                output.Push(variables[token.Value].ToString(CultureInfo.InvariantCulture));
            }
            else if (token.Type == TokenType.Function)
            {
                operators.Push(token.Value);
            }
            else if (token.Type == TokenType.Operator)
            {
                while (!operators.Empty() && operators.Peek() != "(")
                {
                    string top = operators.Peek();
                    int topPrec = GetPrecedence(top);
                    int currPrec = GetPrecedence(token.Value);

                    if (topPrec > currPrec || (topPrec == currPrec && !IsRightAssociative(token.Value)))
                    {
                        output.Push(operators.Pop());
                    }
                    else
                    {
                        break;
                    }
                }
                operators.Push(token.Value);
            }
            else if (token.Type == TokenType.LeftParen)
            {
                operators.Push("(");
            }
            else if (token.Type == TokenType.RightParen)
            {
                while (!operators.Empty() && operators.Peek() != "(")
                {
                    output.Push(operators.Pop());
                }

                if (operators.Empty())
                {
                    throw new ArgumentException("Mismatched parentheses");
                }

                operators.Pop();

                if (!operators.Empty() && IsFunction(operators.Peek()))
                {
                    output.Push(operators.Pop());
                }
            }
        }

        while (!operators.Empty())
        {
            string op = operators.Pop();
            if (op == "(" || op == ")")
            {
                throw new ArgumentException("Mismatched parentheses");
            }
            output.Push(op);
        }

        MyStack<string> result = new MyStack<string>();
        while (!output.Empty())
        {
            result.Push(output.Pop());
        }

        StringBuilder rpn = new StringBuilder();
        bool first = true;
        while (!result.Empty())
        {
            if (!first) rpn.Append(" ");
            rpn.Append(result.Pop());
            first = false;
        }

        return rpn.ToString();
    }

    public double EvaluateRPN(string rpn)
    {
        string[] tokens = rpn.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        MyStack<double> stack = new MyStack<double>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
            {
                stack.Push(num);
            }
            else if (token == "+")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                stack.Push(a + b);
            }
            else if (token == "-")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                stack.Push(a - b);
            }
            else if (token == "*")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                stack.Push(a * b);
            }
            else if (token == "/")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                if (Math.Abs(b) < 1e-10)
                    throw new DivideByZeroException("Division by zero");
                stack.Push(a / b);
            }
            else if (token == "^")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                stack.Push(Math.Pow(a, b));
            }
            else if (token == "%")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                if (Math.Abs(b) < 1e-10)
                    throw new DivideByZeroException("Division by zero");
                stack.Push(a % b);
            }
            else if (token == "div")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                if (Math.Abs(b) < 1e-10)
                    throw new DivideByZeroException("Division by zero");
                stack.Push(Math.Floor(a / b));
            }
            else if (token == "sqrt")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                double a = stack.Pop();
                if (a < 0)
                    throw new ArgumentException("Square root of negative number");
                stack.Push(Math.Sqrt(a));
            }
            else if (token == "abs")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                stack.Push(Math.Abs(stack.Pop()));
            }
            else if (token == "sign")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                stack.Push(Math.Sign(stack.Pop()));
            }
            else if (token == "sin")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                stack.Push(Math.Sin(stack.Pop()));
            }
            else if (token == "cos")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                stack.Push(Math.Cos(stack.Pop()));
            }
            else if (token == "tan")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                stack.Push(Math.Tan(stack.Pop()));
            }
            else if (token == "ln")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                double a = stack.Pop();
                if (a <= 0)
                    throw new ArgumentException("Logarithm of non-positive number");
                stack.Push(Math.Log(a));
            }
            else if (token == "lg")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                double a = stack.Pop();
                if (a <= 0)
                    throw new ArgumentException("Logarithm of non-positive number");
                stack.Push(Math.Log10(a));
            }
            else if (token == "exp")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                stack.Push(Math.Exp(stack.Pop()));
            }
            else if (token == "floor")
            {
                if (stack.Size() < 1) throw new ArgumentException("Invalid expression");
                stack.Push(Math.Floor(stack.Pop()));
            }
            else if (token == "min")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                stack.Push(Math.Min(a, b));
            }
            else if (token == "max")
            {
                if (stack.Size() < 2) throw new ArgumentException("Invalid expression");
                double b = stack.Pop();
                double a = stack.Pop();
                stack.Push(Math.Max(a, b));
            }
            else
            {
                throw new ArgumentException($"Unknown operator: {token}");
            }
        }

        if (stack.Size() != 1)
        {
            throw new ArgumentException("Invalid expression");
        }

        return stack.Pop();
    }

    public double Evaluate(string expression)
    {
        string rpn = ConvertToRPN(expression);
        return EvaluateRPN(rpn);
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            RPNCalculator calc = new RPNCalculator();
            string expression;

            if (args.Length > 0)
            {
                expression = args[0];

                for (int i = 1; i < args.Length; i++)
                {
                    string[] parts = args[i].Split('=');
                    if (parts.Length == 2)
                    {
                        string varName = parts[0].Trim();
                        if (double.TryParse(parts[1].Trim(), NumberStyles.Any,
                            CultureInfo.InvariantCulture, out double value))
                        {
                            calc.SetVariable(varName, value);
                            Console.WriteLine($"Variable {varName} = {value}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Enter expression:");
                expression = Console.ReadLine();
            }

            Console.WriteLine($"\nOriginal: {expression}");
            string rpn = calc.ConvertToRPN(expression);
            Console.WriteLine($"RPN: {rpn}");
            double result = calc.EvaluateRPN(rpn);
            Console.WriteLine($"Result: {result}");

            Console.WriteLine("\n=== Additional Tests ===\n");

            TestExpression(calc, "3 + 4 * 2 / (1 - 5) ^ 2");
            TestExpression(calc, "sqrt(16) + abs(-10)");
            TestExpression(calc, "sin(0) + cos(0)");
            TestExpression(calc, "max(10, 20) + min(5, 3)");
            TestExpression(calc, "2 ^ 3 ^ 2");
            TestExpression(calc, "10 % 3");

            calc.SetVariable("x", 5);
            calc.SetVariable("y", 10);
            TestExpression(calc, "x * 2 + y");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void TestExpression(RPNCalculator calc, string expr)
    {
        try
        {
            Console.WriteLine($"Expression: {expr}");
            string rpn = calc.ConvertToRPN(expr);
            Console.WriteLine($"RPN: {rpn}");
            double result = calc.EvaluateRPN(rpn);
            Console.WriteLine($"Result: {result}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\n");
        }
    }
}

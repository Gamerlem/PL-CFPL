using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace PL
{
    class Parser
    { 
        private List<List<Token>> lines;
        private List<Variable> variables;
        private List<Token> line;
        private Token current;
        private int line_number = -1;
        private List<string> output;

        public Parser(List<List<Token>> lines)
        {
            this.lines = lines;
            variables = new List<Variable>();
            next_list();
            output = new List<string>();
        }
        public List<Variable> getVariables()
        {
            return variables;
        }   
        public void startParse()
        {
            parseDeclaration();
            if(line!=null)
                parseBlock();

        }
        public List<string> Output
        {
            get => output;
        }

      
        //LINE FUNCTIONS
        public void varDeclaration()
        {
            //state diagram of Variable declaration DEAD: 7 FINAL: 4
             int[,] variableDeclaration = new int[8, 8] {
                { 1, 7, 7, 7, 7, 7, 7, 7},
                { 7, 7, 2, 7, 7, 7, 7, 7},
                { 7, 3, 7, 1, 5, 7, 7, 7},
                { 7, 7, 7, 7, 7, 7, 4, 7},
                { 1, 7, 7, 7, 7, 7, 7, 7},
                { 7, 7, 6, 7, 7, 2, 7, 7},
                { 7, 3, 7, 1, 5, 7, 7, 7},
                { 7, 7, 7, 7, 7, 7, 7, 7}
            };

            List<Token> tokens = line;
            int state = 0;
            int input;

            Token tokenError = new Token();
            Token prevToken = new Token();

            bool equalFlag = false;
            List<Variable> temp = new List<Variable>();
            List<Variable> list = new List<Variable>();

            foreach (Token token in tokens)
            {
                tokenError = token;
                //tokenError.Lexeme = replaceLOGICAL(tokenError.Lexeme);

                if (token.Type == "keyword")
                {
                    if (token.Lexeme == "VAR") input = 0;
                    else if (token.Lexeme == "AS")
                    {
                        input = 1;
                        if (prevToken.Type == "identifier" && equalFlag)
                        {
                            equalFlag = false;
                            Variable t = variables.Find(item => item.Name == prevToken.Lexeme);
                            if (t != null)
                            {
                                foreach (Variable var in temp)
                                {
                                    var.Type = t.Type;
                                    var.Value = t.Value;
                                    list.Add(var);
                                }
                                temp.Clear();
                            }
                            else
                            {
                                //undeclared variable
                                throw new VariableErrorException("" + prevToken.Lineno + ":error: Undeclared Variable Found \"" + prevToken.Lexeme + "\"");
                            }
                                
                        }
                        else
                        {
                            foreach (Variable var in temp)
                            {
                                list.Add(var);
                            }
                            temp.Clear();
                        }
                    }
                    else input = 7;
                }
                else if (token.Type == "identifier")
                {
                    input = 2;
                    //find if this identifier is already declared 
                    Variable t = variables.Find(item => item.Name == token.Lexeme);
                    if(t == null)
                    {
                        Variable var = new Variable(token.Lexeme);
                        temp.Add(var);
                    }
                    else
                    {
                        //this if statement assumes that this identifier is not for assignment for other variables
                        if (!equalFlag)
                        {
                            throw new VariableErrorException("" + tokenError.Lineno + ":error: Variable \"" + tokenError.Lexeme + "\" is already declared");
                        }
                            
                    }
                }
                else if (token.Type == "comma")
                {
                    input = 3;
                    if (prevToken.Type == "identifier" && equalFlag)
                    {
                        equalFlag = false;
                        Variable t = variables.Find(item => item.Name == prevToken.Lexeme);
                        if (t != null)
                        {
                            foreach (Variable var in temp)
                            {
                                var.Type = t.Type;
                                var.Value = t.Value;
                                list.Add(var);
                            }
                            temp.Clear();
                        }
                        else
                        {
                            //throw new VariableErrorException("Undeclared Variable Found");
                            throw new VariableErrorException("" + tokenError.Lineno + ":error: Variable \""+tokenError.Lexeme+"\" is undeclared");
                        }
                    }
                    else
                    {
                        foreach (Variable var in temp)
                            list.Add(var);
                        temp.Clear();
                    }
                }
                else if (token.Type == "equal")
                {
                    input = 4;
                    equalFlag = true;
                }
                else if (token.Type == "int_lit" || token.Type == "float_lit" || token.Type == "bool_lit" || token.Type == "char_lit" || token.Type == "expression")
                {
                    input = 5;
                    equalFlag = false;
                    string value = token.Lexeme;
                    
                    //expressions
                    if (token.Type == "expression")
                    {
                        value = evaluateExpression(token);
                    }
                    //literals
                    foreach (Variable var in temp)
                    {
                        if (Pattern.int_lit.IsMatch(value))
                        {
                            var.Type = "int";
                            var.Value = value;
                        }
                        else if (Pattern.float_lit.IsMatch(value))
                        {
                            var.Type = "float";
                            var.Value = value;
                        }
                        else if (Pattern.bool_lit.IsMatch(value))
                        {
                            var.Type = "bool";
                            var.Value = value;
                        }
                        else if(Pattern.char_lit.IsMatch(value))
                        {
                            var.Type = "char";
                            var.Value = value;
                        }
                        list.Add(var);
                    }
                    temp.Clear();
                }
                else if (token.Type == "int" || token.Type == "float" || token.Type == "bool" || token.Type == "char")
                {
                    input = 6;

                    foreach (Variable var in list)
                    {
                        if (var.Type == "null")
                        {
                            var.Type = token.Type;
                        }
                        else if (var.Type != token.Type)
                        {
                            //consider float cases
                            if(var.Type == "int" && token.Type == "float")
                            {
                                var.Type = token.Type;
                            }
                            else
                            {
                                //unmatched data assignment to datatype
                                //throw new VariableErrorException("Unmatched data assignment to datatype");
                                throw new VariableErrorException("" + tokenError.Lineno + ":error: Unmatched data assignment to datatype " + tokenError.Lexeme);
                            }
                        }
                    }
                }
                else
                {
                    input = 7;
                }

                state = variableDeclaration[state, input];
                prevToken = token;
                if (state == 7)
                {
                    break;
                }
            }
            //if it reaches the final state
            if (state == 4)
            {
                foreach (Variable var in list)
                {
                    variables.Add(var);
                }
                list.Clear();
            }
            else
            {
                tokenError.Lexeme = replaceLOGICAL(tokenError.Lexeme);
                throw new SyntaxErrorException("" + tokenError.Lineno + ":error: Syntax Error in Variable Declaration: " + tokenError.Lexeme);
            }
        }
        public void varAssignment()
        {
            //state diagram of variable assignment DEAD: 4 FINAL: 3
            int[,] variableAssignment = new int[5, 4]
            {
                { 1, 4, 4, 4},
                { 4, 2, 4, 4},
                { 1, 4, 3, 4},
                { 1, 4, 4, 4},
                { 4, 4, 4, 4}
            };
            
            List<Token> tokens = this.line;
            int state = 0;
            int input;
            bool equalFlag = false;
            List<Variable> temp = new List<Variable>();
            Token tokenError = new Token();


            foreach (Token token in tokens)
            {
                tokenError = token;

                if (token.Type == "identifier")
                {
                    Variable t = variables.Find(item => item.Name == token.Lexeme);
                    if (t != null)
                    {
                        input = 0;
                        temp.Add(t);
                    }
                    else
                        //input = 3;
                        //error undeclared variable found 
                        throw new VariableErrorException("" + tokenError.Lineno + ":error: Undeclared Variable Found in variable assignment: " + tokenError.Lexeme);
                }
                else if (token.Type == "equal")
                {
                    input = 1;
                    equalFlag = true;
                }
                else if (token.Type == "int_lit" || token.Type == "float_lit" || token.Type == "bool_lit" || token.Type == "char_lit" || token.Type == "expression")
                {
                    input = 2;
                    equalFlag = false;
                    string value = token.Lexeme;

                    //expressions
                    if (token.Type == "expression")
                    {
                        value = evaluateExpression(token);
                    }

                    //literals
                    if (Pattern.int_lit.IsMatch(value))
                    {
                        foreach (Variable var in temp)
                        {
                            if (var.Type != "int")
                            {
                                //unmatched datatype
                                throw new VariableErrorException("" + tokenError.Lineno + ":error: unmatched datatype found: " + tokenError.Lexeme + " cannot converted into "+var.Type);
                            }
                            else
                            {
                                var.Value = value;
                            }
                        }
                    }
                    else if (Pattern.float_lit.IsMatch(value))
                    {
                        foreach (Variable var in temp)
                        {
                            if (var.Type != "float")
                            {
                                //unmatched datatype
                                throw new VariableErrorException("" + tokenError.Lineno + ":error: unmatched datatype found: " + tokenError.Lexeme + " cannot converted into " + var.Type);
                            }
                            else
                            {
                                var.Value = value;
                            }
                        }
                    }
                    else if (Pattern.bool_lit.IsMatch(value))
                    {
                        foreach (Variable var in temp)
                        {
                            if (var.Type != "bool")
                            {
                                //unmatched datatype
                                throw new VariableErrorException("" + tokenError.Lineno + ":error: unmatched datatype found: " + tokenError.Lexeme + " cannot converted into " + var.Type);
                            }
                            else
                            {
                                var.Value = value;
                            }
                        }
                    }
                    else if (Pattern.char_lit.IsMatch(value))
                    {
                        foreach (Variable var in temp)
                        {
                            if (var.Type != "char")
                            {
                                //unmatched datatype
                                throw new VariableErrorException("" + tokenError.Lineno + ":error: unmatched datatype found: " + tokenError.Lexeme + " cannot converted into " + var.Type);
                            }
                            else
                            {
                                var.Value = value;
                            }
                        }
                    }
                }
                else
                {
                    input = 3;
                }

                state = variableAssignment[state, input];
                if (state == 4)
                {
                    break;
                }
            }
            //special case, variable to variable assignment e.g. a = b = c
            if(state == 1 && equalFlag)
            {
                for(int i = 0; i<temp.Count -1; i++)
                {
                    if(temp[i].Type == temp[temp.Count - 1].Type)
                    {
                        temp[i].Value = temp[temp.Count - 1].Value;
                    }
                    else
                    {
                        throw new VariableErrorException("" + tokenError.Lineno + ":error: unmatched datatype found: "+ temp[i].Type + " cannot be converted to "+ temp[temp.Count - 1].Type);
                    }
                }
                state = 3;
            }

            if (state != 3)
            {
                tokenError.Lexeme = replaceLOGICAL(tokenError.Lexeme);
                throw new SyntaxErrorException("" + tokenError.Lineno + ":error: Syntax Error in Variable Assignment: " + tokenError.Lexeme);
            }
                
        }
        private void outputStatement()
        {
            StringBuilder sb = new StringBuilder();
            int state = 0;
            int[,] m = new int[6, 6]
            {
                { 1, 5, 5, 5, 5, 5 },
                { 5, 2, 5, 5, 5, 5 },
                { 5, 5, 3, 5, 3, 5 },
                { 5, 5, 3, 5, 3, 5 },
                { 5, 5, 5, 4, 5, 5 },
                { 5, 5, 5, 5, 5, 5 }
            };
            int type;
            foreach (Token token in line)
            {
                switch (token.Type)
                {
                    case "keyword":
                        type = token.Lexeme == "OUTPUT" ? 0 : 5;
                        break;
                    case "colon":
                        type = 1;
                        break;
                    case "string":
                        type = 2;
                        string s = token.Lexeme;
                        s = s.Replace("\"", "");
                        s = s.Replace("[[]", "[");
                        s = s.Replace("[]]", "]");
                        s = s.Replace("[#]", "[hashtag]");
                        s = s.Replace("#", "\n");
                        s = s.Replace("[hashtag]", "#");
                        sb.Append(s);
                        break;
                    case "identifier":
                        type = 3;
                        bool find = false;
                        foreach (Variable v in variables)
                        {
                            if (v.Name == token.Lexeme)
                            {
                                if(v.Type == "char")
                                {
                                    sb.Append(v.Value[1]);
                                }else if(v.Type == "bool")
                                {
                                    sb.Append(v.Value.Replace("\"", ""));
                                }
                                else
                                {
                                    sb.Append(v.Value);
                                }
                                find = true;
                                break;
                            }
                        }
                        if (find == false) throw new VariableErrorException("Unknown variable " + token.Lexeme);
                        break;
                    case "ampersand":
                        type = 4;
                        break;
                    default:
                        type = 5;
                        break;
                }
                state = m[type, state];
            }
            if (state == 3)
            {
                output.Add(sb.ToString());
            }
            else
            {
                throw new SyntaxErrorException("Invalid OUTPUT statement.");
            }
        }

        
        public void inputStatement()
        {
            //state diagram of Input Keyword DEAD: 4 FINAL: 3
            int[,] tableInput = new int[5, 5]
            {
                { 1, 4, 4, 4, 4},
                { 4, 2, 4, 4, 4},
                { 4, 4, 3, 4, 4},
                { 4, 4, 4, 2, 4},
                { 4, 4, 4, 4, 4}
            };

            int state = 0;
            int input;
            List<Variable> inp = new List<Variable>();

            foreach (Token token in line)
            {
                if (token.Type == "keyword")
                {
                    if (token.Lexeme == "INPUT")
                    {
                        input = 0;
                    }
                    else
                    {
                        input = 4;
                    }
                }
                else if (token.Type == "colon")
                {
                    input = 1;
                }
                else if (token.Type == "identifier")
                {
                    Variable t = variables.Find(item => item.Name == token.Lexeme);
                    if (t != null)
                    {
                        input = 2;
                        inp.Add(t);
                    }
                    else
                        //error undeclared variable found 
                        throw new VariableErrorException("Undeclared Variable Found");
                }
                else if (token.Type == "comma")
                {
                    input = 3;
                }
                else
                {
                    input = 4;
                }

                state = tableInput[state, input];
                if (state == 4)
                {
                    break;
                }
            }

            if (state == 3)
            {
                Form2 frm2 = new Form2();
                DialogResult dr = frm2.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string[] values = frm2.getText().Split(',');
                    readInputLine(inp, values);
                }
            }
            else
            {
                throw new SyntaxErrorException("Syntax Error in INPUT");
            }

        }

        private void readInputLine(List<Variable> inp, string[] values)
        {
            if(inp.Count != values.Length)
                throw new SyntaxErrorException("Lacking inputted values");

            int index = 0;

            foreach(string str in values)
            {
                Variable t = variables.Find(item => item == inp[index]);
                if (Pattern.int_lit.IsMatch(str))
                {
                    if (t.Type != "int")
                    {
                        throw new VariableErrorException("unmatched datatype found 1");
                    }else if (t.Type == "float")
                    {
                        t.Value = str;
                    }
                    else
                    {
                        t.Value = str;
                    }
                }
                else if (Pattern.float_lit.IsMatch(str))
                {
                    if (t.Type != "float")
                    {
                        throw new VariableErrorException("unmatched datatype found 2");
                    }
                    else
                    {
                        t.Value = str;
                    }
                }
                else if (Pattern.char_lit.IsMatch(str))
                {
                    if (t.Type != "char")
                    {
                        throw new VariableErrorException("unmatched datatype found 3");
                    }
                    else
                    {
                        t.Value = str;
                    }
                }
                else if (Pattern.bool_lit.IsMatch(str))
                {
                    if (t.Type != "bool")
                    {
                        throw new VariableErrorException("unmatched datatype found 4");
                    }
                    else
                    {
                        t.Value = str;
                    }
                }
                else
                {
                    throw new VariableErrorException("input error");
                }
                index++;
            }
        }

        //HELPER FUNCTIONS
        private string evaluateExpression(Token token)
        {
            //state diagram of valid bool expressions DEAD: 5 FINAL: 3
            int[,] boolExpression = new int[6, 6]
            {
            {1, 5, 5, 3, 4, 5 },
            {5, 2, 5, 5, 5, 5 },
            {3, 5, 5, 5, 5, 5 },
            {5, 5, 0, 5, 5, 5 },
            {1, 5, 5, 3, 4, 5 },
            {5, 5, 5, 5, 5, 5 },
            };

            //get the string to process
            string expression = token.Lexeme;

            //prepare the string error
            Token err = token;
            err.Lexeme = replaceLOGICAL(err.Lexeme);

            //check if expression has a variable then replace the variable to its value
            if (Pattern.identifierInLine.IsMatch(expression))
            {
                var matches = Pattern.identifierInLine.Matches(expression);
                foreach (Match m in matches)
                {
                    //throw new VariableErrorException(m.Value);
                    Variable t = variables.Find(item => item.Name == m.Value);
                    if (t != null)
                    {
                        if (t.Value != "null")
                            expression = expression.Replace(m.Value, t.Value);
                        else
                            //null value
                            throw new VariableErrorException("Trying to use uninitialized variable");
                    }
                    else
                    {
                        //undeclared variable found
                        Regex b = new Regex(@"TRUE|FALSE");
                        if(!b.IsMatch(m.Value))
                            throw new VariableErrorException("Undeclared Variable Found in expression: "+m.Value.ToString());
                    }
                }
            }

            //arithmetic expressions
            if (Pattern.arithmetic.IsMatch(expression))
            {
                //solve expression
                Evaluate evaluate = new Evaluate(expression);
                expression = evaluate.getResult();
            }
            //else if boolean expressions
            else if (Pattern.Strictexpression.IsMatch(expression) || Pattern.StrictNOTexpressions.IsMatch(expression) || Pattern.BOOLsingle.IsMatch(expression))
            {
                //solve all arithmetic found in the line
                foreach (Match match in Pattern.arithmeticInLine.Matches(expression))
                {
                    String temp = match.Value.ToString();
                    Evaluate evaluate = new Evaluate(temp);
                    temp = evaluate.getResult();
                    expression = expression.Replace(match.Value.ToString(), temp);
                }
                //evaluate boolean arguments

                //prepare the expression for splitting
                expression = expression.Replace(">", " > ");
                expression = expression.Replace("<", " < ");
                expression = expression.Replace(" < =", " <= ");
                expression = expression.Replace(" > =", " >= ");
                expression = expression.Replace("==", " == ");
                expression = expression.Replace(" <  > ", " <> ");
                expression = expression.Replace("(", "");
                expression = expression.Replace(")", "");

                expression = expression.Replace("&&", " && ");
                expression = expression.Replace("||", " || ");
                expression = expression.Replace("!", " ! ");

                string[] tokens = expression.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                int state = 0;
                int input;

                string operand1 = "";
                string operand2 = ""; 
                string opt = "";
                string res = "";
                bool flag = false;
                Queue<string> queue = new Queue<string>();


                foreach (string str in tokens)
                {
                    //skip parenthesis
                    if (str == "(" || str == ")")
                        continue;

                    if (Pattern.int_lit.IsMatch(str) || Pattern.float_lit.IsMatch(str))
                    {
                        input = 0;
                        if (!flag)
                            operand1 = str;
                        else
                        {
                            flag = false;
                            operand2 = str;

                            if (opt == ">")
                            {
                                float num1 = float.Parse(operand1);
                                float num2 = float.Parse(operand2);
                                res = (num1 > num2) ? "true" : "false";
                            }
                            else if (opt == "<")
                            {
                                float num1 = float.Parse(operand1);
                                float num2 = float.Parse(operand2);
                                res = (num1 < num2) ? "true" : "false";
                            }
                            else if (opt == ">=")
                            {
                                float num1 = float.Parse(operand1);
                                float num2 = float.Parse(operand2);
                                res = (num1 >= num2) ? "true" : "false";
                            }
                            else if (opt == "<=")
                            {
                                float num1 = float.Parse(operand1);
                                float num2 = float.Parse(operand2);
                                res = (num1 <= num2) ? "true" : "false";
                            }
                            else if (opt == "==")
                            {
                                float num1 = float.Parse(operand1);
                                float num2 = float.Parse(operand2);
                                res = (num1 == num2) ? "true" : "false";
                            }
                            else if (opt == "<>")
                            {
                                float num1 = float.Parse(operand1);
                                float num2 = float.Parse(operand2);
                                res = (num1 != num2) ? "true" : "false";
                            }
                            queue.Enqueue(res);
                        }
                    }
                    else if(Pattern.boolOpt.IsMatch(str))
                    {
                        input = 1;
                        opt = str;
                        flag = true;
                    }
                    else if(Pattern.logOpt.IsMatch(str))
                    {
                        input = 2;
                        flag = false;

                        if (str == "&&")
                            queue.Enqueue("AND");
                        else if (str == "||")
                            queue.Enqueue("OR");
                    }
                    else if(str == "\"TRUE\""|| str == "\"FALSE\"")
                    {
                        input = 3;
                        if (str == "\"TRUE\"")
                            queue.Enqueue("true");
                        else
                            queue.Enqueue("false");
                    }
                    else if(str == "!")
                    {
                        input = 4;
                        queue.Enqueue("NOT");
                    }
                    else
                    {
                        input = 5;
                    }

                    state = boolExpression[state, input];

                    if (state == 5)
                    {
                        break;
                    }
                        
                }

                //evaluate the boolean formed
                if (state == 3)
                {
                    string temp = "";
                    foreach (string str in queue)
                        temp += str;
                    
                    temp = temp.Replace("AND", " AND ");
                    temp = temp.Replace("OR", " OR ");
                    temp = temp.Replace("NOT", " NOT ");

                    DataTable dt = new DataTable();
                    bool b = (bool)dt.Compute(temp, "");

                    if (b)
                    {
                        expression = "\"TRUE\"";
                    }
                    else
                    {
                        expression = "\"FALSE\"";
                    }
                }
                else
                {
                    throw new SyntaxErrorException(""+err.Lineno+": error: Invalid expression: " + err.Lexeme);
                }

            }
            else
            {
                throw new SyntaxErrorException("" + err.Lineno + ": error: Invalid expression: " + err.Lexeme);
            }
            
            return expression;
        }
        private String replaceLOGICAL(string statement)
        {
            statement = statement.Replace("&&", " AND ");
            statement = statement.Replace("||", " OR ");
            statement = statement.Replace("!", " NOT ");

            return statement;
        }
        private void next_list()
        {
            //throw new SyntaxErrorException(lines.Count.ToString());
            line_number += 1;
            if (line_number < lines.Count)
                line = lines[line_number];
            else
                line = null;
            getFirstToken();
        }
        private String peekNext()
        {
            string temp = "";
            if (line_number + 1 < lines.Count)
            {
                temp = lines[line_number + 1][0].Lexeme;
            }

            return temp;
        }
        private void check(string word)
        {
            if(current != null)
            {
                if (current.Lexeme != word)
                    throw new SyntaxErrorException("Syntax Error 1 " + word + " " + current.Lineno + " " + current.Lexeme);
                if (current.Lexeme == "START" || current.Lexeme == "STOP")
                    if (line.Count != 1)
                        throw new SyntaxErrorException("Syntax Error 2");
            }
            else
            {
                throw new SyntaxErrorException("Syntax Error 3");
            }
        }
        private void getFirstToken()
        {
            if (line != null)
                current = line[0];
            else
                current = null;
        }
        private void parseDeclaration()
        {
            while (line != null && current.Lexeme == "VAR")
            {
                varDeclaration();
                next_list();
            }
        }
        private void parseBlock()
        {
            check("START");
            parseBlockHelper();
            check("STOP");
        }
        private void parseBlockHelper()
        {
            next_list();
            while (line != null && isStatement())
            {
                if (current.Type == "identifier")
                {
                    varAssignment();
                }
                else if (current.Lexeme == "IF")
                {
                    ifStatement();
                }
                else if (current.Lexeme == "ELSE")
                {
                    throw new SyntaxErrorException("ELSE without IF");
                }
                else if (current.Lexeme == "WHILE")
                {
                    whileStatement();
                }
                else if (current.Lexeme == "OUTPUT")
                {
                    outputStatement();
                }
                else if (current.Lexeme == "INPUT")
                {
                    inputStatement();
                }
                next_list();
            }
        }
        private bool isStatement()
        {
            bool flag = false;
            if (current.Type == "identifier")
                flag = true;
            else if (current.Lexeme == "IF")
                flag = true;
            else if (current.Lexeme == "ELSE")
                flag = true;
            else if (current.Lexeme == "WHILE")
                flag = true;
            else if (current.Lexeme == "OUTPUT")
                flag = true;
            else if (current.Lexeme == "INPUT")
                flag = true;
            return flag;
        }
        private void skipBlock()
        {
            check("START");
            skipBlockHelper();
            check("STOP");
        }
        private void skipBlockHelper()
        {
            next_list();
            while (line != null && isStatement())
            {
                if(current.Lexeme == "IF")
                {
                    if (line.Count != 2)
                        throw new SyntaxErrorException("Syntax Error in IF Statement skip");

                    next_list();
                    skipBlock();

                    if(peekNext() == "ELSE")
                    {
                        next_list();

                        if (line.Count != 1)
                            throw new SyntaxErrorException("Syntax Error in ELSE Statement skip");

                        if(line != null)
                        {
                            next_list();
                            skipBlock();
                        }
                    }
                }else if (current.Lexeme == "ELSE")
                {
                    throw new SyntaxErrorException("ELSE without IF skip");
                }
                else if (current.Lexeme == "WHILE")
                {
                    if (line.Count != 2)
                        throw new SyntaxErrorException("Syntax Error in WHILE Statement skip");

                    next_list();
                    skipBlock();
                }
                next_list();
            }
        }
        private void ifStatement()
        {
            if (line.Count != 2)
                throw new SyntaxErrorException("Syntax Error in IF Statement");

            string expression = line[1].Lexeme;

            if (Pattern.BOOLexpressions.IsMatch(expression) || Pattern.BOOLsingle.IsMatch(expression))
            {
                expression = evaluateExpression(line[1]);

                if (expression == "\"TRUE\"")
                {
                    //execute first block
                    next_list();
                    parseBlock();
                    //skip the else block if it exist
                    if (peekNext() == "ELSE")
                    {
                        next_list();
                        if (line.Count != 1)
                            throw new SyntaxErrorException("Syntax Error in ELSE Statement");

                        if (line != null)
                        {
                            next_list();
                            skipBlock();
                        }
                    }
                }
                else if (expression == "\"FALSE\"")
                {
                    //skip the first block
                    next_list();
                    skipBlock();
                    //execute else block if it exist
                    if (peekNext() == "ELSE")
                    {
                        next_list();
                        if (line.Count != 1)
                            throw new SyntaxErrorException("Syntax Error in ELSE Statement");
                        if (line != null)
                        {
                            next_list();
                            parseBlock();
                        }
                    }
                }
                else
                {
                    throw new SyntaxErrorException("Invalid argument in IF<expression>");
                }
            }
            else
            {
                throw new SyntaxErrorException("Syntax Error in IF expression");
            }
        }
        private void whileStatement()
        {
            if (line.Count != 2)
                throw new SyntaxErrorException("Syntax Error in WHILE Statement");

            string expression = line[1].Lexeme;

            if (Pattern.BOOLexpressions.IsMatch(expression) || Pattern.BOOLsingle.IsMatch(expression))
            {
                
                expression = evaluateExpression(line[1]);
                int startIndex = line_number;
                
                while (expression == "\"TRUE\"")
                {
                    next_list();
                    parseBlock();
                    line_number = startIndex;
                    line = lines[line_number];
                    expression = evaluateExpression(line[1]);
                }
                next_list();
                skipBlock();
            }
            else
            {
                throw new SyntaxErrorException("Syntax Error in WHILE expression");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    class Evaluate
    {
        private String data;
        private float result;
        private int flag;

        private Stack<float> values = new Stack<float>();
        private Stack<String> operations = new Stack<String>();

        public Evaluate()
        {
            data = "";
            result = 0;
            flag = 0;
        }

        public Evaluate(String str)
        {
            data = str;
            result = 0;
            flag = 0;
        }

        public string Data { get => data; set => data = value; }

        
        public String getResult()
        {
            evaluate();
            bool ok = false;
            string temp = "";

            switch (flag)
            {
                case 1: temp = "Error: Lacking operand"; break;
                case 2: temp = "Error: Missing operand"; break;
                case 3: temp = "Error: Missing operator"; break;
                case 4: temp = "Error: Invalid Symbol"; break;
                case 5: temp = "Error: Math Error"; break;
                case 6: temp = "Error: Missing ( "; break;
                case 7: temp = "Error: Missing ) "; break;
                case 8: temp = "Error: Not Infix Equation"; break;
                case 9: temp = "Error: Modulo operations are applicable only to integeral values"; break;
                default: temp = result.ToString();ok = true; break;
            }
            //throw new SyntaxErrorException(temp);
            if (ok)
                return temp;
            else
                throw new SyntaxErrorException(temp);
        }
        

        private void evaluate()
        {
            int count = 0, groupFlag = 0;
            String val = "";
            String prev = "";
            bool negativeFlag = false;

            //preprocess the string
            data = data.Replace("+", " + ");
            data = data.Replace("-", " - ");
            data = data.Replace("*", " * ");
            data = data.Replace("/", " / ");
            data = data.Replace("%", " % ");
            data = data.Replace("(", " ( ");
            data = data.Replace(")", " ) ");

            string[] strings = data.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i<strings.Length; i++)
            {
                val = strings[i];

                if (checkInput(val))
                {
                    if (operation(val))
                    {
                        if(prev == "" && val == "+" || val == "+" && operation(prev) || prev == "(" && val == "+")
                        {
                            continue;
                        }else if (prev == "" && val == "-" || val == "-" && operation(prev) || prev =="(" && val == "-")
                        {
                            negativeFlag = true;
                        }
                        else
                        {
                            
                            if (operations.Count != 0 && precedence(operations.Peek()) >= precedence(val))
                            {
                                performOpt();
                                if (flag != 0)
                                {
                                    break;
                                }
                                count--;
                            }
                            if (count == 2 && operations.Count == 0)
                            {
                                flag = 8;
                                break;
                            }
                            operations.Push(val);
                        }
                    }
                    else if (val.Equals("("))
                    {
                        operations.Push(val);
                        groupFlag++; //group evaluation is present
                    }
                    else if (val.Equals(")"))
                    {
                        if (groupFlag > 0)
                        {
                            while (operations.Count!= 0 && !(operations.Peek()).Equals("("))
                            {
                                performOpt();
                                if (flag != 0)
                                {
                                    break;
                                }
                                count--;
                            }
                            operations.Pop(); //popping (
                            groupFlag--; //done performing group equation
                        }
                        else
                        {
                            flag = 6; // missing (
                            break;
                        }
                    }
                    else
                    {
                        float temp = float.Parse(val);
                        if (negativeFlag)
                        {
                            temp *= -1;
                            negativeFlag = false;
                        }
                        values.Push(temp);
                        count++;
                    }
                }
                else
                {
                    flag = 4;
                    break;
                }

                prev = val;
            }

            //performing remaining operations: flag==0, checking if previous parching was no error
            if (flag == 0 && groupFlag == 0)
            {
                while (operations.Count != 0)
                {
                    performOpt();
                    if (flag != 0)
                    {
                        break;
                    }
                    count--;
                }
            }
            else
            {
                if (flag == 0)
                {
                    flag = 7; //missing )
                }
            }

            //checking stack for values
            if (values.Count != 0 && count > 1 && flag == 0)
            {
                flag = 3; //no operator
            }
            else
            {
                if (flag == 0)
                {
                    result = values.Peek();
                }
            }
        }


        //helper
        private Boolean operation(String str)
        {
            return str.Equals("+") || str.Equals("-") || str.Equals("*") || str.Equals("/") || str.Equals("%");
        }

        private Boolean symbols(String str)
        {
            return str.Equals(" ") || str.Equals(".") || str.Equals(")") || str.Equals("(");
        }

        private Boolean checkInput(String str)
        {
            Boolean state = true;

            if (!operation(str))
            {
                if (!symbols(str))
                {
                    try
                    {
                        float f = float.Parse(str);
                    }catch(Exception e)
                    {
                        state = false;
                    }
                }
            }
            return state;
        }

        private int precedence(String str)
        {
            //higher p value means higher precedence
            int p = 0;
            if (str.Equals("+") || str.Equals("-"))
            {
                p = 1;
            }
            if (str.Equals("*") || str.Equals("/") || str.Equals("%"))
            {
                p = 2;
            }
            return p;
        }


        private float applyOpt(float num1, float num2, String opt)
        {
            float result = 0;
            switch (opt)
            {
                case "+": result = num1 + num2; break;
                case "-": result = num1 - num2; break;
                case "*": result = num1 * num2; break;
                case "/": result = num1 / num2; break;
                case "%": 
                    if (Pattern.int_lit.IsMatch(num1.ToString()) && Pattern.int_lit.IsMatch(num2.ToString()))
                        result = num1 % num2;
                    else
                        flag = 9;
                    break;
            }
            return result;
        }

        private void performOpt()
        {
            float num1 = 0, num2 = 0;

            if (values.Count == 0)
            {
                flag = 2; // missing operands
            }
            else
            {
                num2 = values.Peek();
                values.Pop();
            }

            if (values.Count == 0)
            {
                if (flag == 0)
                {
                    flag = 1; // lacking operands
                }
            }
            else
            {
                num1 = values.Peek();
                values.Pop();
            }

            if ((operations.Peek()).Equals("/") && num2 == 0)
            {
                flag = 5; //math error
            }
            else
            {
                values.Push(applyOpt(num1, num2, operations.Peek()));
                operations.Pop();
            }
        }

    }
}

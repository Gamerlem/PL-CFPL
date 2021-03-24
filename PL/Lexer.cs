using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL
{
    class Lexer
    {
        private string source;

        public Lexer(string source)
        {
            this.source = source;
        }

        public List<List<Token>> scan() 
        {
            TypeAnalyzer typeAnalyzer = new TypeAnalyzer();
            List<List<Token>> lines = new List<List<Token>>();
            String[] strlines = source.Split('\n');

            for(int i=0; i<strlines.Length; i++)
            {
                //if (strlines[i].StartsWith("*")) continue;
                if(Pattern.comment.IsMatch(strlines[i])) continue;


                string[] lexemes = GetTokens(strlines[i]);
                List<Token> line = new List<Token>();
                foreach (string lexeme in lexemes)
                {
                    line.Add(new Token(typeAnalyzer.GetTokenType(lexeme), lexeme, i + 1));
                }
                if(line.Count >= 1)
                    lines.Add(line);
            }
            return lines;
        }

        //tokenizer
        private string[] GetTokens(string statement)
        {
            //change keyword AND to special symbol
            statement = statement.Replace("AND", "&&");
            statement = statement.Replace("OR", "||");
            statement = statement.Replace("NOT", "!");

            
            //NOT expressions e.g. NOT "TRUE", NOT NOT "TRUE"
            foreach (Match match in Pattern.NOTexpressions.Matches(statement))
            {
                //MessageBox.Show("NOT: "+match.Value);
                String temp = match.Value.ToString();
                temp = temp.Replace(" ", "");
                statement = statement.Replace(match.Value.ToString(), temp);
            }
            //boolean NOT expressions e.g ( NOT "TRUE"), (NOT flag)
            foreach (Match match in Pattern.BOOLsingle.Matches(statement))
            {
                //MessageBox.Show("NOT: "+match.Value);
                String temp = match.Value.ToString();
                temp = temp.Replace(" ", "");
                statement = statement.Replace(match.Value.ToString(), temp);
            }

            //arithmetic and boolean expressions
            foreach (Match match in Pattern.expression.Matches(statement))
            {
                //MessageBox.Show("EXP: " + match.Value);
                String temp = match.Value.ToString();
                temp = temp.Replace(" ", "");
                statement = statement.Replace(match.Value.ToString(), temp);
            }

            //add important whitespaces for tokenizing
            statement = AddWhitespaces(statement);

            //pre-process the expressions to remove whitespace to avoid individual tokenizing
            foreach (Match match in Pattern.string_lit_inline.Matches(statement))
            {
                String temp = match.Value.ToString();
                temp = temp.Replace(" ", "[SPACE]");
                statement = statement.Replace(match.Value.ToString(), temp);
            }

            string[] tokens = statement.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i<tokens.Length; i++)
            {
                tokens[i] = tokens[i].Replace("[SPACE]", " ");
            }

            return tokens;
        }

        private string AddWhitespaces(string statement)
        {
            statement = statement.Replace(",", " , ");
            statement = statement.Replace("=", " = ");
            statement = statement.Replace(":", " : ");
            statement = statement.Replace("“", "\"");
            statement = statement.Replace("”", "\"");
            statement = statement.Replace("‘", "'");
            statement = statement.Replace("’", "'");
            statement = statement.Replace("VAR", " VAR ");
            statement = statement.Replace("AS", " AS ");

            statement = statement.Replace("IF", " IF ");
            statement = statement.Replace("ELSE", " ELSE ");
            statement = statement.Replace("WHILE", " WHILE ");

            statement = statement.Replace(" =  = ", "==");
            statement = statement.Replace("> = ", ">=");
            statement = statement.Replace("< = ", "<=");
            return statement;
        }
    }
}

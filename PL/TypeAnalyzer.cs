using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PL
{
    class TypeAnalyzer
    {
        private IReadOnlyCollection<string> KEYWORDS = new List<string>()
        {
            "VAR", "AS",
            "START", "OUTPUT", "STOP", "INPUT",
            "IF","ELSE", "WHILE",
        };
        
        public TypeAnalyzer() { }

        public string GetTokenType(string token)
        {
            if (KEYWORDS.Contains(token))
                return "keyword";
            else if (token == ",")
                return "comma";
            else if (token == "=")
                return "equal";
            else if (token == ":")
                return "colon";
            else if (token == "&")
                return "ampersand";
            //////////
            else if (Pattern.expression.IsMatch(token))
                return "expression";
            else if (Pattern.int_lit.IsMatch(token))
                return "int_lit";
            else if (Pattern.float_lit.IsMatch(token))
                return "float_lit";
            else if (Pattern.char_lit.IsMatch(token))
                return "char_lit";
            else if (Pattern.bool_lit.IsMatch(token))
                return "bool_lit";
            else if (Pattern.NOTexpressions.IsMatch(token) || Pattern.BOOLsingle.IsMatch(token))
                return "expression";
            //////////
            else if (Pattern.INT.IsMatch(token))
                return "int";
            else if (Pattern.CHAR.IsMatch(token))
                return "char";
            else if (Pattern.BOOL.IsMatch(token))
                return "bool";
            else if (Pattern.FLOAT.IsMatch(token))
                return "float";
            ///////////
            else if (Pattern.string_lit.IsMatch(token))
                return "string";
            else if (Pattern.identifier.IsMatch(token))
                return "identifier";
            return "unknown";
        }
    }
}

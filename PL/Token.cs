using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    class Token
    {
        private string type;
        private string lexeme;
        private int lineno;

        public Token() { }
        public Token(string type, string lexeme, int lineno)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.lineno = lineno;
        }

        public string Type { get => type; set => type = value; }
        public string Lexeme { get => lexeme; set => lexeme = value; }
        public int Lineno { get => lineno;  }
    }
}

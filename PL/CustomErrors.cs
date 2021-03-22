using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException() : base() { }
        public SyntaxErrorException(string message) : base(message) { }
    }

    public class VariableErrorException : Exception
    {
        public VariableErrorException() : base() { }
        public VariableErrorException(string message) : base(message) { }
    }
}

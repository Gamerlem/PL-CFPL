using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    class Variable
    {
        private string name;
        private string type;
        private string value;


        public Variable(string name, string type = null)
        {
            this.name = name;
            this.type = "null";
            this.value = "null";
        }

        public string Name { get => name; }
        public string Type { get => type; set => type = value; }
        public string Value { get => value; set => this.value = value; }
    }
}

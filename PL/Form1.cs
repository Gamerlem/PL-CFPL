using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Lexer lexer = new Lexer(richTextBox1.Text);
            List<List<Token>> lines = lexer.scan();
            Parser p = new Parser(lines);

            try
            {
                p.startParse();
                /*
                List<Variable> variables = p.getVariables();

                foreach (Variable var in variables)
                {
                    richTextBox3.Text += "NAME: " + var.Name + " TYPE: " + var.Type + " VALUE: " + var.Value + "\n";
                }
                */

                foreach (string s in p.Output)
                {
                    richTextBox3.Text += s + "\n";
                }

            }
            catch(Exception error)
            {
                richTextBox3.Text = error.Message;
            }

            foreach(List<Token> line in lines)
            {
                foreach(Token t in line)
                {
                    richTextBox2.Text += t.Lineno + " : " + t.Type + " : " + t.Lexeme + Environment.NewLine;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            richTextBox3.Text = "";
        }
    }
}

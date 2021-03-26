using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PL
{
    static class Pattern
    {
        /// <summary>
        /// Regex Patterns Class
        /// </summary>
       
        public static Regex expression = new Regex(@"(?x)
                (!|(\s))*
                (?> (?<p> (\s)* \( (\s)*)* (\s)* (?> (!|(\s))* (-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (\s)* (?<-p> (\s)* \) (\s)* )* )
                (?> (?:
                    (\s)* (-|\+|\*|/|%|>|<|(<>)|(==)|(>=)|(<=)|(&&)|(\|\|))+ (\s)* 
                    (?> (?<p> (\s)* \( (\s)*)* (\s)* (?> (!|(\s))* (-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (\s)* (?<-p> (\s)* \) (\s)* )* )
                )+)
                (?(p)(?!))
             ");

        public static Regex NOTexpressions = new Regex(@"(?x)
                !(\s)*(!|(\s))*([a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))
             ");

        public static Regex BOOLexpressions = new Regex(@"(?x)
                \((\s)* (!|(\s))*
                (?> (?<p> (\s)* \( (\s)*)* (\s)* (?> (!|(\s))* (-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (\s)* (?<-p> (\s)* \) (\s)* )* )
                (?> (?:
                    (\s)* (-|\+|\*|/|%|>|<|(<>)|(==)|(>=)|(<=)|(&&)|(\|\|))+ (\s)* 
                    (?> (?<p> (\s)* \( (\s)*)* (\s)* (?> (!|(\s))* (-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))) (\s)* (?<-p> (\s)* \) (\s)* )* )
                )+ (\s)* \))
                (?(p)(?!))
             ");

        public static Regex BOOLsingle = new Regex(@"(?x)
                \( (\s)* (!|(\s))* ([a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE"")) (\s)* \)
             ");

        public static Regex Strictexpression = new Regex(@"(?x)
                (!|(\s))*
                (?> (?<p> (\s)* \( (\s)*)* (\s)* (?> (!|(\s))* (-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE"")|\'.\'|\'\'.\'\')) (\s)* (?<-p> (\s)* \) (\s)* )* )
                (?> (?:
                    (\s)* (-|\+|\*|/|%|>|<|(<>)|(==)|(>=)|(<=)|(&&)|(\|\|)) (\s)* 
                    (?> (?<p> (\s)* \( (\s)*)* (\s)* (?> (!|(\s))* (-|\+)? (\s)* (\d+(?:\.\d+)?|[a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE"")|\'.\'|\'\'.\'\')) (\s)* (?<-p> (\s)* \) (\s)* )* )
                )+)
                (?(p)(?!))
                $
             ");

        public static Regex StrictNOTexpressions = new Regex(@"(?x)
                !(\s)*(!|(\s))*([a-zA-Z_$][a-zA-Z_$0-9]*|(""TRUE""|""FALSE""))$
             ");

        public static Regex arithmetic = new Regex(@"(?x)
                (?> (?<p> \( )* (?>-?(\+|-)?\d+(?:\.\d+)?) (?<-p> \) )* )
                (?>(?:
                    [-+*%/]
                    (?> (?<p> \( )* (?>(\+|-)?\d+(?:\.\d+)?) (?<-p> \) )* )
                )+)
                (?(p)(?!))
                $
            ");

        public static Regex arithmeticInLine = new Regex(@"(?x)
                (?> (?<p> \( )* (?>-?(\+|-)?\d+(?:\.\d+)?) (?<-p> \) )* )
                (?>(?:
                    [-+*%/]
                    (?> (?<p> \( )* (?>(\+|-)?\d+(?:\.\d+)?) (?<-p> \) )* )
                )+)
                (?(p)(?!))
            ");

        public static Regex int_lit = new Regex(@"^(\+|-)?[0-9]+$");
        public static Regex float_lit = new Regex(@"^(\+|-)?[0-9]*[.][0-9]*$");
        public static Regex char_lit = new Regex(@"'.'");
        public static Regex bool_lit = new Regex(@"(^""TRUE""$)|(^""FALSE""$)");
        public static Regex string_lit = new Regex(@"^"".*?""$");
        public static Regex string_lit_inline = new Regex(@""".*?""");

        public static Regex INT = new Regex("^INT$"); 
        public static Regex FLOAT = new Regex("^FLOAT$");
        public static Regex CHAR = new Regex("^CHAR$");
        public static Regex BOOL = new Regex("^BOOL$");
        public static Regex identifier = new Regex(@"^[a-zA-Z_$][a-zA-Z_$0-9]*$");
        public static Regex identifierInLine = new Regex(@"[a-zA-Z_$][a-zA-Z_$0-9]*");

        public static Regex boolOpt = new Regex(@"^(>|<|(>=)|(<=)|(==)|(<>))$");
        public static Regex logOpt = new Regex(@"^((\&\&)|(\|\|))$");

        public static Regex comment = new Regex(@"^(\s)*(\*)(.*)");
        
    }
}

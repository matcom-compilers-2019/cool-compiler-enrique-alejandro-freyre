using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using System.IO;
using Core.AST;
using Core.AST.Program;
using Core.SemanticCheck.Scopes;
using Antlr4.Runtime.Tree;
using Core.Parser;
using Core.SemanticCheck;
using Core.SemanticCheck.Errors;
using Core.CodeGen;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Invalid params count");
            }
            Console.WriteLine("Cool Compiler");
            var input = args[0];
            var output = args[1];
            //var input = "C:\\Users\\Kike-PC\\Desktop\\CoolCompiler\\TestCases\\CodeGeneration\\hello_world.cl";
            //var output = "C:\\Users\\Kike-PC\\Desktop\\CoolCompiler\\TestCases\\CodeGeneration\\hello.s";
            DirectoryInfo dirInfo = new DirectoryInfo(input);
            //FileInfo files = dirInfo.GetFiles();
            
            FileInfo file = new FileInfo(input);
            ASTNode root = ParseInput(file.FullName);
            Scope scope = new Scope();
            List<SemanticError> errors = new List<SemanticError>();
            ProgramNode rootProgram = root as ProgramNode;
            SemanticCheck.CheckAST(rootProgram, errors, scope);
            if (errors.Count > 0)
                foreach (SemanticError error in errors)
                {
                    Console.WriteLine(error.Type);
                    Console.WriteLine(error.Line + ":" + error.Column);
                }
            else
            {
                var bc = new BuildICCode(rootProgram, scope);
                var s = WMIPS.Generate(bc.GetIntCode());
                File.WriteAllText(output, s);
            }
            Console.WriteLine("Finished:");

        }

        private static ASTNode ParseInput(string inputPath)
        {
            var input = new AntlrFileStream(inputPath);
            var lexer = new CoolLexer(input);


            var errors = new List<string>();
            lexer.RemoveErrorListeners();
            //lexer.AddErrorListener(new LexerErrorListener(errors));

            var tokens = new CommonTokenStream(lexer);

            var parser = new CoolParser(tokens);

            //parser.RemoveErrorListeners();
            //parser.AddErrorListener(new ParserErrorListener(errors));

            IParseTree tree = parser.program();

            if (errors.Any())
            {
                Console.WriteLine();
                foreach (var item in errors)
                    Console.WriteLine(item);
                return null;
            }

            var astBuilder = new ASTGenerator();
            ASTNode ast = astBuilder.Visit(tree);
            return ast;
        }
    }
}

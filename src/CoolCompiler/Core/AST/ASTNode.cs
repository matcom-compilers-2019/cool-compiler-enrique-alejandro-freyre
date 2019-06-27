using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Core.SemanticCheck;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.CodeGen.IC;

namespace Core.AST
{
    public abstract class ASTNode : ISemanticCheck, IGenIntCode
    {
        public int Line { get; }

        public int Column { get; }

        public ASTNode(int line, int column)
        {
            Line = line;
            Column = column;
        }
        public ASTNode(ParserRuleContext context)
        {
            Line = context.Start.Line;
            Column = context.Start.Column;
        }

        public abstract void FirstSemanticCheck(List<SemanticError> errors, Scope scope);

        public abstract void SecondSemanticCheck(List<SemanticError> errors, Scope scope);

        public abstract void GetIntCode(Core.CodeGen.BuildICCode codeManager);
    }
}

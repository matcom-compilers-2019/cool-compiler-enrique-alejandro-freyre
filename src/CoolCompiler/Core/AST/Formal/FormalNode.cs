using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.AST.Type;
using Core.CodeGen;

namespace Core.AST.Formal
{
    public class FormalNode : Expressions.ExpressionNode
    {
        public Expressions.Atomics.IdNode Id { get; set; }

        public TypeNode Type { get; set; }

        public FormalNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!scope.IsDefinedType(Type.Text, out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Operators.Unarys
{
    public class NegationNode : UnaryOperatorNode
    {
        public override string Symbol => "~";

        public NegationNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            Operand.SecondSemanticCheck(errors, scope);
            if (Operand.StaticType.Text != "Int")
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));
            if (!scope.IsDefinedType("Int", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.UnaryOperationCheck(this);
            if (codeManager.special_object_return_type)
                codeManager.SetReturnType("Int");
        }
    }
}

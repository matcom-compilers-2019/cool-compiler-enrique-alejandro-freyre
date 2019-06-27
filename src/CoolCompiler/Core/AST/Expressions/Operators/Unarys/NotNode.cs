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
    public class NotNode : UnaryOperatorNode
    {
        public override string Symbol => "not";

        public NotNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            Operand.SecondSemanticCheck(errors, scope);
            if (Operand.StaticType.Text != "Bool")
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));
            if (!scope.IsDefinedType("Bool", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.UnaryOperationCheck(this);
            if (codeManager.special_object_return_type)
                codeManager.SetReturnType("Bool");
        }
    }
}

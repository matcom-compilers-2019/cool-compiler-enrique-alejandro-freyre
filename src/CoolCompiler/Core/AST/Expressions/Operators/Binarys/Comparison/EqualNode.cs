using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Operators.Binarys.Comparison
{
    public class EqualNode : ComparisonOperatorNode
    {
        public override string Symbol => "=";

        public EqualNode(int line, int column) : base(line, column)
        {
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            RightOperand.SecondSemanticCheck(errors, scope);
            LeftOperand.SecondSemanticCheck(errors, scope);
            if (LeftOperand.StaticType.Text != RightOperand.StaticType.Text)
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));
            if (!scope.IsDefinedType("Bool", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.BinaryOperationCheck(this);
            if (codeManager.special_object_return_type)
                codeManager.SetReturnType("Bool");
        }
    }
}

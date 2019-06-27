using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Operators.Binarys.Comparison
{
    public abstract class ComparisonOperatorNode : BinaryOperatorNode
    {
        public ComparisonOperatorNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.BinaryOperationCheck(this);
            if (codeManager.special_object_return_type)
                codeManager.codeLines.Add(new ICAssignStrToVar(BuildICCode.return_type_variable, "Bool"));
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            RightOperand.SecondSemanticCheck(errors, scope);
            LeftOperand.SecondSemanticCheck(errors, scope);
            if (LeftOperand.StaticType.Text != "Int" || RightOperand.StaticType.Text != "Int")
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));
            if (!scope.IsDefinedType("Bool", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }
    }
}

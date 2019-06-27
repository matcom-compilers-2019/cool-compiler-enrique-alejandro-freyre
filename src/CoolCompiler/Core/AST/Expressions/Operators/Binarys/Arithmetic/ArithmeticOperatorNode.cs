using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.AST.Expressions.Atomics;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Operators.Binarys.Arithmetic
{
    public abstract class ArithmeticOperatorNode : BinaryOperatorNode
    {
        public ArithmeticOperatorNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            if (LeftOperand is IntegerNode && RightOperand is IntegerNode)
            {
                int v = 0;
                switch (Symbol)
                {
                    case "+":
                        v = ((IntegerNode)LeftOperand).Value + ((IntegerNode)RightOperand).Value;
                        break;
                    case "-":
                        v = ((IntegerNode)LeftOperand).Value - ((IntegerNode)RightOperand).Value;
                        break;
                    case "*":
                        v = ((IntegerNode)LeftOperand).Value * ((IntegerNode)RightOperand).Value;
                        break;
                    case "/":
                        v = ((IntegerNode)LeftOperand).Value / ((IntegerNode)RightOperand).Value;
                        break;
                }
                codeManager.codeLines.Add(new ICAssignConstToVar(codeManager.variableManager.PeekCounter(), v));
            }
            else codeManager.BinaryOperationCheck(this);
            if (codeManager.special_object_return_type)
                codeManager.SetReturnType("Int");
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            RightOperand.SecondSemanticCheck(errors, scope);
            LeftOperand.SecondSemanticCheck(errors, scope);
            if (LeftOperand.StaticType.Text != RightOperand.StaticType.Text)
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));
            else if (LeftOperand.StaticType.Text != "Int" || RightOperand.StaticType.Text != "Int")
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));
            else if (!scope.IsDefinedType("Int", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.CodeGen.IC;

namespace Core.AST.Feature
{
    public class AttributeNode : FeatureNode
    {
        public Formal.FormalNode Formal { get; set; }

        public Expressions.ExpressionNode DessignateExpression { get; set; }

        public AttributeNode(int line, int column) : base(line, column)
        {
        }

        public AttributeNode(ParserRuleContext context) : base(context)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!scope.IsDefinedType(Formal.Type.Text, out SemanticCheck.Type type))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));

            if (scope.IsDefined(Formal.Id.Text, out SemanticCheck.Type t))
                errors.Add(new SemanticError(Line,Column,TypeError.RepeatVariableName));

            scope.Define(Formal.Id.Text, type);
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            DessignateExpression.SecondSemanticCheck(errors, scope);
            var typeAssignExp = DessignateExpression.StaticType;

            if (!scope.IsDefinedType(Formal.Type.Text, out SemanticCheck.Type typeDeclared))
                errors.Add(new SemanticError(Line, Column, TypeError.RepeatMethod));

            if (!(typeAssignExp <= typeDeclared))
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));

            scope.Define(Formal.Id.Text, typeDeclared);
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            DessignateExpression.GetIntCode(codeManager);
            if ((DessignateExpression.StaticType.Text == "Int" ||
                DessignateExpression.StaticType.Text == "Bool" ||
                DessignateExpression.StaticType.Text == "String") &&
                Formal.Type.Text == "Object")
            {
                codeManager.codeLines.Add(new ICPushParams(codeManager.variableManager.PeekCounter()));
                codeManager.codeLines.Add(new ICCallLabel(new ICLabel("_wrapper", DessignateExpression.StaticType.Text),
                    codeManager.variableManager.PeekCounter()));
                codeManager.codeLines.Add(new ICPopParams(1));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Keyword
{
    public class LoopNode : KeywordNode
    {
        public Expressions.ExpressionNode Condition { get; set; }

        public Expressions.ExpressionNode Body { get; set; }

        public LoopNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            Condition.SecondSemanticCheck(errors, scope);
            Body.SecondSemanticCheck(errors, scope);

            if (Condition.StaticType.Text != "Bool")
                errors.Add(new SemanticError(Condition.Line, Condition.Column, TypeError.InconsistentType));
            if (!scope.IsDefinedType("Object", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            string tag = codeManager.codeLines.Count.ToString();
            codeManager.codeLines.Add(new ICLabel("_whilecond", tag));
            Condition.GetIntCode(codeManager);
            codeManager.codeLines.Add(new ICCondJump(codeManager.variableManager.PeekCounter(),
                new ICLabel("_endwhile", tag)));
            Body.GetIntCode(codeManager);
            codeManager.codeLines.Add(new ICJump(new ICLabel("_whilecond", tag)));
            codeManager.codeLines.Add(new ICLabel("_endwhile", tag));
        }
    }
}

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
    public class ConditionalNode : KeywordNode
    {
        public Expressions.ExpressionNode Condition { get; set; }

        public Expressions.ExpressionNode IfBody { get; set; }

        public Expressions.ExpressionNode ElseBody { get; set; }

        public ConditionalNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            Condition.SecondSemanticCheck(errors, scope);
            IfBody.SecondSemanticCheck(errors, scope);
            ElseBody.SecondSemanticCheck(errors, scope);

            if (Condition.StaticType.Text != "Bool") // AKI AKI AKI
                errors.Add(new SemanticError(Condition.Line, Condition.Column, TypeError.InconsistentType));
            StaticType = SemanticCheck.SemanticCheckAlgoritms.LowerCommonAncestor(IfBody.StaticType, ElseBody.StaticType);
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            var tag = codeManager.codeLines.Count.ToString();
            Condition.GetIntCode(codeManager);

            codeManager.codeLines.Add(new ICCondJump(codeManager.variableManager.PeekCounter(), new ICLabel("_else", tag)));

            IfBody.GetIntCode(codeManager);

            codeManager.codeLines.Add(new ICJump(new ICLabel("_endif", tag)));

            codeManager.codeLines.Add(new ICLabel("_else", tag));

            ElseBody.GetIntCode(codeManager);

            codeManager.codeLines.Add(new ICLabel("_endif", tag));
        }
    }
}

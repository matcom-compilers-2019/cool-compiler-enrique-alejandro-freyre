using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Keyword
{
    public class LetNode : KeywordNode
    {
        public List<Feature.AttributeNode> Sentences { get; set; }

        public Expressions.ExpressionNode Body { get; set; }

        public LetNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            var nscope = scope.CreateChild();
            foreach (var item in Sentences)
            {
                item.SecondSemanticCheck(errors, nscope);
                var t = item.DessignateExpression.StaticType;
                if (!nscope.IsDefinedType(item.Formal.Type.Text, out SemanticCheck.Type type))
                    errors.Add(new SemanticError(item.Formal.Line, item.Formal.Column, TypeError.ClassNotDefine));
                if (!(t <= type)) errors.Add(new SemanticError(item.Formal.Line, item.Formal.Column, TypeError.InconsistentType));
                if (nscope.IsDefined(item.Formal.Id.Text, out SemanticCheck.Type oldt))
                    nscope.Change(item.Formal.Id.Text, type);
                else nscope.Define(item.Formal.Id.Text, type);
            }
            Body.SecondSemanticCheck(errors, nscope);
            StaticType = Body.StaticType;
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.variableManager.PushCounter();

            foreach (var attr in this.Sentences)
            {
                codeManager.variableManager.IncreaseCounter();
                codeManager.variableManager.Push(attr.Formal.Id.Text, attr.Formal.Type.Text);
                codeManager.variableManager.PushCounter();
                attr.GetIntCode(codeManager);
            }
            codeManager.variableManager.IncreaseCounter();

            Body.GetIntCode(codeManager);

            foreach (var attr in Sentences)
                codeManager.variableManager.Pop(attr.Formal.Id.Text);

            codeManager.variableManager.PopCounter();

            if (codeManager.special_object_return_type)
                codeManager.SetReturnType(StaticType.Text);
        }
    }
}

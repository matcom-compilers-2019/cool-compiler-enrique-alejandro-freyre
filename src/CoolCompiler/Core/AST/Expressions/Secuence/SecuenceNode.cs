using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Secuence
{
    public class SecuenceNode : ExpressionNode
    {
        public List<ExpressionNode> Secuence { get; set; }

        public SecuenceNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            foreach (var sec in Secuence)
                sec.SecondSemanticCheck(errors, scope);

            var retType = Secuence[Secuence.Count - 1].StaticType.Text;

            if (!scope.IsDefinedType(retType, out StaticType))
                errors.Add(new SemanticError(Secuence[Secuence.Count - 1].Line, Secuence[Secuence.Count - 1].Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            foreach (var e in Secuence)
                e.GetIntCode(codeManager);
        }
    }
}

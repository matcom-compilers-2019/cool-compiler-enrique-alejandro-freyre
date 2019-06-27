using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.SemanticCheck
{
    public interface ISemanticCheck
    {
        void FirstSemanticCheck(List<SemanticError> errors, Scope scope);
        void SecondSemanticCheck(List<SemanticError> errors, Scope scope);
    }

    public static class SemanticCheck
    {
        public static void CheckAST(ISemanticCheck node, List<SemanticError> errors, Scope scope)
        {
            node.FirstSemanticCheck(errors, scope);
            node.SecondSemanticCheck(errors, scope);
        }
    }
}

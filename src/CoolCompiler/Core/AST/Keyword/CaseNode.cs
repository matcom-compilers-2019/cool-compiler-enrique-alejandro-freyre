using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.SemanticCheck;

namespace Core.AST.Keyword
{
    public class CaseNode : KeywordNode
    {
        public Expressions.ExpressionNode Sentence { get; set; }

        public List<Feature.AttributeNode> Cases { get; set; }

        public int BranchSelected { get; set; }

        public CaseNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            this.Sentence.SecondSemanticCheck(errors, scope);

            int branchSel = -1;
            var typeExpr0 = Sentence.StaticType;
            var typeExprK = scope.GetType(Cases[0].Formal.Type.Text);

            for (int i = 0; i < Cases.Count; i++)
            {
                if (!scope.IsDefinedType(Cases[i].Formal.Type.Text, out SemanticCheck.Type info))
                    errors.Add(new SemanticError(Cases[i].Line, Cases[i].Column, TypeError.ClassNotDefine));

                var typeK = scope.GetType(Cases[i].Formal.Type.Text);

                var scopeBranch = scope.CreateChild();
                scopeBranch.Define(Cases[i].Formal.Id.Text, typeK);

                Cases[i].DessignateExpression.SecondSemanticCheck(errors, scopeBranch);

                typeExprK = Cases[i].DessignateExpression.StaticType;

                if (branchSel == -1 && typeExpr0 <= typeK)
                    branchSel = i;

                if (i == 0)
                    StaticType = Cases[0].DessignateExpression.StaticType;
                StaticType = SemanticCheck.SemanticCheckAlgoritms.LowerCommonAncestor(StaticType, typeExprK);
            }
            BranchSelected = branchSel;
            if (BranchSelected == -1)
                errors.Add(new SemanticError(Line, Column, TypeError.InvalidCase));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            var stype = StaticType.Text;
            int res = codeManager.variableManager.PeekCounter();
            int expr = codeManager.variableManager.IncreaseCounter();

            codeManager.variableManager.PushCounter();
            Sentence.GetIntCode(codeManager);
            codeManager.variableManager.PopCounter();

            if (stype == "String" || stype == "Int" || stype == "Bool")
            {
                int idx = Cases.FindIndex((x) => x.Formal.Type.Text == stype);
                var v = Cases[idx].Formal.Id.Text;

                codeManager.variableManager.Push(v, Cases[idx].Formal.Type.Text);

                int t = codeManager.variableManager.IncreaseCounter();
                codeManager.variableManager.PushCounter();

                Cases[idx].DessignateExpression.GetIntCode(codeManager);
                codeManager.variableManager.PopCounter();
                codeManager.variableManager.Pop(v);

                codeManager.codeLines.Add(new ICAssignVarToVar(codeManager.variableManager.PeekCounter(), t));
            }
            else
            {
                var tag = codeManager.codeLines.Count.ToString();
                var l = new List<Feature.AttributeNode>();
                l.AddRange(Cases);
                l.Sort((x, y) => (codeManager.scope.GetType(x.Formal.Type.Text) <= codeManager.scope.GetType(y.Formal.Type.Text)? -1 : 1));
                for (int i = 0; i < l.Count; i++)
                {
                    codeManager.variableManager.Push(l[i].Formal.Id.Text, l[i].Formal.Type.Text);

                    var caseType = l[i].Formal.Type.Text;
                    codeManager.variableManager.PushCounter();
                    codeManager.variableManager.IncreaseCounter();

                    codeManager.codeLines.Add(new ICLabel("_case", tag + "." + i));
                    codeManager.codeLines.Add(new ICAssignStrToVar(codeManager.variableManager.VarCount, caseType));
                    codeManager.codeLines.Add(new ICArith(codeManager.variableManager.VarCount, expr, codeManager.variableManager.VarCount, "inherit"));
                    codeManager.codeLines.Add(new ICCondJump(codeManager.variableManager.VarCount, new ICLabel("_case", tag + "." + (i + 1))));

                    if (caseType == "Int" || caseType == "Bool" || caseType == "String")
                    {
                        if (stype == "Object")
                        {
                            codeManager.codeLines.Add(new ICAssignMemToVar(expr, expr, codeManager.virtualTable.SizeOf(caseType)));

                            codeManager.variableManager.PopCounter();
                            l[i].DessignateExpression.GetIntCode(codeManager);

                            codeManager.codeLines.Add(new ICAssignVarToVar(res, codeManager.variableManager.PeekCounter()));
                            codeManager.codeLines.Add(new ICJump(new ICLabel("_endcase", tag)));
                        }
                    }
                    else
                    {
                        codeManager.variableManager.PushCounter();
                        l[i].DessignateExpression.GetIntCode(codeManager);
                        codeManager.variableManager.PopCounter();

                        codeManager.codeLines.Add(new ICAssignVarToVar(res, codeManager.variableManager.PeekCounter()));
                        codeManager.codeLines.Add(new ICJump(new ICLabel("_endcase", tag)));
                    }
                    codeManager.variableManager.PopCounter();
                    codeManager.variableManager.Pop(l[i].Formal.Id.Text);
                }

                codeManager.codeLines.Add(new ICLabel("_case", tag + "." + l.Count));
                codeManager.codeLines.Add(new ICJump(new ICLabel("_caseselectionexception")));

                codeManager.codeLines.Add(new ICLabel("_endcase", tag));
            }
        }
    }
}

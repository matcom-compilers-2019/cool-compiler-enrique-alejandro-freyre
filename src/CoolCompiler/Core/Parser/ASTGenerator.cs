using System.Linq;
using Antlr4.Runtime.Misc;
using Core.AST;
using Core.AST.Program;
using Core.AST.Class;
using Core.AST.Type;
using Core.AST.Feature;
using Core.AST.Expressions;
using Core.AST.Expressions.Atomics;
using Core.AST.Expressions.Operators;
using Core.AST.Expressions.Operators.Binarys;
using Core.AST.Expressions.Operators.Binarys.Arithmetic;
using Core.AST.Expressions.Operators.Binarys.Comparison;
using Core.AST.Expressions.Operators.Unarys;
using Core.AST.Expressions.Secuence;
using Core.AST.Formal;
using Core.AST.Keyword;
using Core.AST.Assigment;
using Core.AST.Dispatch;
using System.Collections.Generic;

namespace Core.Parser
{
    public class ASTGenerator : CoolBaseVisitor<ASTNode>
    {
        public override ASTNode VisitProgram([NotNull] CoolParser.ProgramContext context)
        {
            return new ProgramNode(context){
                ClassNodes = context.classdef().Select(x => Visit(x) as ClassNode).ToList()
            };
        }

        public override ASTNode VisitClassdef([NotNull] CoolParser.ClassdefContext context)
        {
            var classNode = new ClassNode(context);

            var typeClass = new TypeNode(context.TYPE(0).Symbol.Line,
                context.TYPE(0).Symbol.Column, context.TYPE(0).GetText());
            classNode.TypeClass = typeClass;

            var inheritType = context.TYPE(1) == null ? new TypeNode(0, 0, "Object") :
                    new TypeNode(context.TYPE(1).Symbol.Line, context.TYPE(1).Symbol.Column, context.TYPE(1).GetText());
            classNode.TypeClassInherit = inheritType;
            classNode.Features = (from x in context.feature() select Visit(x) as FeatureNode).ToList(); 
            return classNode;
        }

        public override ASTNode VisitFeature([NotNull] CoolParser.FeatureContext context)
        {
            return VisitChildren(context);
        }

        public override ASTNode VisitMethod([NotNull] CoolParser.MethodContext context)
        {
            var methodNode = new MethodNode(context);

            methodNode.Id = new IdNode(context.ID().Symbol.Line, context.ID().Symbol.Column, context.ID().GetText());

            var formalNodes = new List<FormalNode>();
            foreach (var x in context.formal())
                formalNodes.Add(Visit(x) as FormalNode);
            methodNode.Arguments = formalNodes;

            methodNode.TypeReturn = new TypeNode(context.TYPE().Symbol.Line, context.TYPE().Symbol.Column, context.TYPE().GetText());

            methodNode.Body = Visit(context.expr()) as ExpressionNode;

            return methodNode;
        }

        public override ASTNode VisitProperty([NotNull] CoolParser.PropertyContext context)
        {
            var attributeNode = new AttributeNode(context);

            attributeNode.Formal = Visit(context.formal()) as FormalNode;

            if (context.expr() != null)
                attributeNode.DessignateExpression = Visit(context.expr()) as ExpressionNode;
            else
            {
                if (attributeNode.Formal.Type.Text == "Int")
                    attributeNode.DessignateExpression = new IntegerNode(context.formal().Start.Line,
                        context.formal().Start.Column)
                    { Value = 0 };
                else if (attributeNode.Formal.Type.Text == "Bool")
                    attributeNode.DessignateExpression = new BoolNode(context.formal().Start.Line,
                        context.formal().Start.Column)
                    { Value = false };
                else if (attributeNode.Formal.Type.Text == "String")
                    attributeNode.DessignateExpression = new StringNode(context.formal().Start.Line,
                        context.formal().Start.Column)
                    { Value = "" };
                else
                    attributeNode.DessignateExpression = new VoidNode(attributeNode.Formal.Type.Text);
            }

            return attributeNode;
        }

        public override ASTNode VisitFormal([NotNull] CoolParser.FormalContext context)
        {
            return new FormalNode(context.Start.Line, context.Start.Column)
            {
                Id = new IdNode(context.ID().Symbol.Line, context.ID().Symbol.Column, context.ID().GetText()),
                Type = new TypeNode(context.TYPE().Symbol.Line, context.TYPE().Symbol.Column, context.TYPE().GetText())
            };
        }

        public override ASTNode VisitNew([NotNull] CoolParser.NewContext context)
        {
            return new NewNode(context.Start.Line, context.Start.Column)
            {
                Type = new TypeNode(context.TYPE().Symbol.Line, context.TYPE().Symbol.Column, context.TYPE().GetText())
            };
        }

        public override ASTNode VisitParentheses([NotNull] CoolParser.ParenthesesContext context)
        { return Visit(context.expr()); }

        public override ASTNode VisitLetIn([NotNull] CoolParser.LetInContext context)
        {
            return new LetNode(context.Start.Line, context.Start.Column)
            {
                Sentences = (from p in context.property() select Visit(p) as AttributeNode).ToList(),
                Body = Visit(context.expr()) as ExpressionNode
            };
        }

        public override ASTNode VisitString([NotNull] CoolParser.StringContext context)
        {
            return new StringNode(context.STRING().Symbol.Line, context.STRING().Symbol.Column)
            {
                Value = context.STRING().GetText()
            };
        }

        public override ASTNode VisitIsvoid([NotNull] CoolParser.IsvoidContext context)
        {
            return new IsVoidNode(context.ISVOID().Symbol.Line, context.ISVOID().Symbol.Column)
            {
                Operand = Visit(context.expr()) as ExpressionNode
            };
        }

        public override ASTNode VisitAssignment([NotNull] CoolParser.AssignmentContext context)
        {
            return new AssigmentNode(context.Start.Line, context.Start.Column)
            {
                Id = new IdNode(context.ID().Symbol.Line, context.ID().Symbol.Column, context.ID().GetText()),
                RightExpression = Visit(context.expr()) as ExpressionNode
            };
        }

        public override ASTNode VisitArithmetic([NotNull] CoolParser.ArithmeticContext context)
        {
            BinaryOperatorNode boNode = null;

            if (context.op.Text == "+")
                boNode = new SumNode(context.Start.Line, context.Start.Column);
            if(context.op.Text == "-")
                boNode = new SubNode(context.Start.Line, context.Start.Column);
            if(context.op.Text == "*")
                boNode = new MulNode(context.Start.Line, context.Start.Column);
            if (context.op.Text == "/")
                boNode = new DivNode(context.Start.Line, context.Start.Column);

            boNode.LeftOperand = Visit(context.expr(0)) as ExpressionNode;
            boNode.RightOperand = Visit(context.expr(1)) as ExpressionNode;

            return boNode;
        }

        public override ASTNode VisitWhile([NotNull] CoolParser.WhileContext context)
        {
            return new LoopNode(context.Start.Line, context.Start.Column)
            {
                Condition = Visit(context.expr(0)) as ExpressionNode,
                Body = Visit(context.expr(1)) as ExpressionNode
            };
        }

        public override ASTNode VisitDispatchImplicit([NotNull] CoolParser.DispatchImplicitContext context)
        {
            return new ImplicitDispatch(context.Start.Line, context.Start.Column)
            {
                IdMethod = new IdNode(context.ID().Symbol.Line, context.ID().Symbol.Column, context.ID().GetText()),
                Arguments = (from arg in context.expr() select Visit(arg) as ExpressionNode).ToList()
            };
        }

        public override ASTNode VisitInt([NotNull] CoolParser.IntContext context)
        {
            return new IntegerNode(context.Start.Line, context.Start.Column)
            {
                Value = int.Parse(context.INT().GetText())
            };
        }

        public override ASTNode VisitNegative([NotNull] CoolParser.NegativeContext context)
        {
            return new NegationNode(context.Start.Line, context.Start.Column)
            {
                Operand = Visit(context.expr()) as ExpressionNode
            };
        }

        public override ASTNode VisitBoolNot([NotNull] CoolParser.BoolNotContext context)
        {
            return new NotNode(context.Start.Line, context.Start.Column)
            {
                Operand = Visit(context.expr()) as ExpressionNode
            };
        }

        public override ASTNode VisitBoolean([NotNull] CoolParser.BooleanContext context)
        {
            return new BoolNode(context.Start.Line, context.Start.Column)
            {
                Value = context.value.Text == "False" ? false : true
            };
        }

        public override ASTNode VisitBlock([NotNull] CoolParser.BlockContext context)
        {
            return new SecuenceNode(context.Start.Line, context.Start.Column)
            {
                Secuence = (from e in context.expr() select Visit(e) as ExpressionNode).ToList()
            };
        }

        public override ASTNode VisitComparisson([NotNull] CoolParser.ComparissonContext context)
        {
            BinaryOperatorNode coNode = null;

            if (context.op.Text == "<")
                coNode = new LessNode(context.Start.Line, context.Start.Column);
            if (context.op.Text == "=")
                coNode = new EqualNode(context.Start.Line, context.Start.Column);
            if (context.op.Text == "<=")
                coNode = new LessEqualNode(context.Start.Line, context.Start.Column);

            coNode.LeftOperand = Visit(context.expr(0)) as ExpressionNode;
            coNode.RightOperand = Visit(context.expr(1)) as ExpressionNode;

            return coNode;
        }

        public override ASTNode VisitId([NotNull] CoolParser.IdContext context)
        {
            return (context.ID().GetText() == "self") ? new SelfNode(context.Start.Line, context.Start.Column) as ASTNode :
                new IdNode(context.Start.Line, context.Start.Column, context.ID().GetText()) as ASTNode;
        }
        
        public override ASTNode VisitIf([NotNull] CoolParser.IfContext context)
        {
            return new ConditionalNode(context.Start.Line, context.Start.Column)
            {
                Condition = Visit(context.expr(0)) as ExpressionNode,
                IfBody = Visit(context.expr(1)) as ExpressionNode,
                ElseBody = Visit(context.expr(2)) as ExpressionNode
            };
        }

        public override ASTNode VisitCase([NotNull] CoolParser.CaseContext context)
        {
            var caseNode = new CaseNode(context.Start.Line, context.Start.Column) { Cases = new List<AttributeNode>()};

            caseNode.Sentence = Visit(context.expr(0)) as ExpressionNode;

            var formals = (from f in context.formal() select Visit(f) as FormalNode).ToList();
            var dexprs = (from de in context.expr().Skip(1) select Visit(de) as ExpressionNode).ToList();
            for (int i = 0; i < formals.Count; i++)
                caseNode.Cases.Add( 
                    new AttributeNode(formals[i].Line, formals[i].Column)
                    {
                        Formal = formals[i],
                        DessignateExpression = dexprs[i]
                    });

            return caseNode;
        }
        
        public override ASTNode VisitDispatchExplicit([NotNull] CoolParser.DispatchExplicitContext context)
        {
            var _base = Visit(context.expr(0)) as ExpressionNode;
            var idMethod = new IdNode(context.ID().Symbol.Line, context.ID().Symbol.Column, context.ID().GetText());
            var args = (from x in context.expr().Skip(1) select Visit(x) as ExpressionNode).ToList();

            return context.TYPE() == null ? 
                new ExplicitDispatch(context.Start.Line, context.Start.Column)
                { Base = _base, IdMethod = idMethod, Arguments = args } as DispatchNode : 
                new BaseDispatch(context.Start.Line, context.Start.Column)
                {
                    Base = _base,
                    IdMethod = idMethod,
                    Arguments = args,
                    TypeBase = new TypeNode(context.TYPE().Symbol.Line, context.TYPE().Symbol.Column, context.TYPE().GetText())
                };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public abstract class ICUnary : ICCode
    {
        public int Destination { get; }
        public int Variable { get; }

        protected string read_op, save_res;
        public ICUnary(int d, int v)
        {
            Destination = d;
            Variable = v;
            read_op = $"lw $a0, {-Variable * 4}($sp)";
            save_res = $"sw $a0, {-Destination * 4}($sp)";
        }
    }

    public class ICNot : ICUnary
    {
        public ICNot(int d, int v) : base(d, v) { }

        public override List<string> Generate()
        {
            return new List<string>()
            {
                read_op,
                WMIPS.li("$a1", 1),
                WMIPS.sub("$a0", "$a1", "$a0"),
                save_res
            };
        }
    }

    public class ICIsVoid : ICUnary
    {
        public ICIsVoid(int d, int v) : base(d, v) { }

        public override List<string> Generate()
        {
            return new List<string>()
            {
                read_op,
                WMIPS.seq("$a0", "$a0", "$zero"),
                save_res
            };
        }
    }

    public class ICNegate : ICUnary
    {
        public ICNegate(int d, int v) : base(d, v) { }

        public override List<string> Generate()
        {
            return new List<string>()
            {
                read_op,
                WMIPS.not("$a0", "$a0"),
                save_res
            };
        }
    }

    public abstract class ICBinary : ICCode
    {
        public int Destination { get; }
        public int Variable1 { get; }
        public int Variable2 { get; }

        protected string read_op1, read_op2, save_res;
        public ICBinary(int d, int v1, int v2)
        {
            Destination = d;
            Variable1 = v1;
            Variable2 = v2;
            read_op1 = $"lw $a0, {-Variable1 * 4}($sp)";
            read_op2 = $"lw $a1, {-Variable2 * 4}($sp)";
            save_res = $"sw $a0, {-Destination * 4}($sp)";
        }
    }

    public class ICArith : ICBinary
    {
        string _operator;
        public ICArith(int d, int v1, int v2, string s) : base(d, v1, v2) { _operator = s; }

        public override List<string> Generate()
        {
            var lines = new List<string>() { read_op1, read_op2 };
            switch (_operator)
            {
                case "+":
                    lines.Add(WMIPS.add("$a0", "$a0", "$a1"));
                    break;
                case "-":
                    lines.Add(WMIPS.sub("$a0", "$a0", "$a1"));
                    break;
                case "*":
                    lines.Add(WMIPS.mult("$a0", "$a1"));
                    lines.Add(WMIPS.mflo("$a0"));
                    break;
                case "/":
                    lines.Add(WMIPS.div("$a0", "$a1"));
                    lines.Add(WMIPS.mflo("$a0"));
                    break;
                case "<":
                    lines.Add(WMIPS.sge("$a0", "$a0", "$a1"));
                    lines.Add(WMIPS.li("$a1", 1));
                    lines.Add(WMIPS.sub("$a0", "$a1", "$a0"));
                    break;
                case "<=":
                    lines.Add(WMIPS.sle("$a0", "$a0", "$a1"));
                    break;
                case "=":
                    lines.Add(WMIPS.seq("$a0", "$a0", "$a1"));
                    break;
                case "=:=":
                    break;
                case "inherit":
                    lines.Add(WMIPS.move("$v1", "$ra"));
                    lines.Add(WMIPS.jal("_inherit"));
                    lines.Add(WMIPS.move("$ra", "$v1"));
                    lines.Add(WMIPS.move("$a0", "$v0"));
                    break;

            }
            lines.Add(save_res);
            return lines;
        }
    }

}

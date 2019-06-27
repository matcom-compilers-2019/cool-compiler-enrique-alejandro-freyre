using System;
using System.Collections.Generic;
using Core.CodeGen.IC;


namespace Core.CodeGen
{
    public static class WMIPS
    {
        public const string SYSCALL = "syscall";
        public const string NEWLINE = "\n";
        public const string FINISH = "li $v0, 10\nsyscall\n";
        static int size, pcount;
        static string f;

        #region Operaciones Aritmeticas

        internal static string add(string r1, string r2, string r3) => "add " + r1 + ", " + r2 + ", " + r3;

        internal static string sub(string r1, string r2, string r3) => "sub " + r1 + ", " + r2 + ", " + r3;

        // Añadir un valor constante
        internal static string addi(string r1, string r2, int number) => "addi " + r1 + ", " + r2 + ", " + number.ToString();

        // Sumar sin signo
        internal static string addu(string r1, string r2, string r3) => "addu " + r1 + ", " + r2 + ", " + r3;

        // Restar sin signo
        internal static string subu(string r1, string r2, string r3) => "subu " + r1 + ", " + r2 + ", " + r3;

        // Añadir un valor constante sin signo
        internal static string addiu(string r1, string r2, int number) => "addiu " + r1 + ", " + r2 + ", " + number.ToString();

        // Multiplicar sin overflow (32 bits)
        internal static string mul(string r1, string r2, string r3) => "mul " + r1 + ", " + r2 + ", " + r3;

        // Guarda los 32 bits + significativos en hi y los 32 bits menos significativos en lo
        internal static string mult(string r2, string r3) =>
            "mult " + r2 + ", " + r3;

        // Dividir. Resto en hi. Cociente en lo
        internal static string div(string r2, string r3) =>
            "div " + r2 + ", " + r3;

        #endregion

        #region Operaciones logicas
        internal static string and(string r1, string r2, string r3) => "and " + r1 + ", " + r2 + ", " + r3;

        // r3 puede ser una constante numerica
        internal static string or(string r1, string r2, string r3) => "or " + r1 + ", " + r2 + ", " + r3;

        internal static string andi(string r1, string r2, string number) => "andi " + r1 + ", " + r2 + ", " + number;

        // Shift Left(Right - srl) logico por una cantidad r3 de bits
        internal static string sll(string r1, string r2, string r3) => "sll " + r1 + ", " + r2 + ", " + r3;

        internal static string not(string r1, string r2) => "not " + r1 + ", " + r2;

        internal static string srl(string r1, string r2, string r3) => "srl " + r1 + ", " + r2 + ", " + r3;
        #endregion

        #region Transferencia de datos
        // Load word
        internal static string lw(string r1, string r2, string r3) => "lw " + r1 + ", " + r2 + "(" + r3 + ")";

        // Store word
        internal static string sw(string r1, string r2, string r3) => "sw " + r1 + ", " + r2 + "(" + r3 + ")";

        // Load byte
        internal static string lb(string r1, string r2, string r3) => "lb " + r1 + ", " + r2 + "(" + r3 + ")";

        // Store byte
        internal static string sb(string r1, string r2, string r3) => "sb " + r1 + ", " + r2 + "(" + r3 + ")";

        // Load address
        internal static string la(string r1, string label) => "la " + r1 + ", " + label;

        // Move from hi/lo
        internal static string mfhi(string r1) => "mfhi " + r1;

        internal static string mflo(string r1) => "mflo " + r1;

        // Mover
        internal static string move(string r1, string r2) => "move " + r1 + ", " + r2;

        internal static string li(string r1, int v) => "li " + r1 + ", " + v;
        #endregion

        #region Branch
        // =
        internal static string beq(string r1, string r2, string n) => "beq " + r1 + ", " + r2 + ", " + n;

        // !=
        internal static string bne(string r1, string r2, string n) => "bne " + r1 + ", " + r2 + ", " + n;

        // >
        internal static string bgt(string r1, string r2, string n) => "bgt " + r1 + ", " + r2 + ", " + n;

        // >=
        internal static string bge(string r1, string r2, string n) => "bge " + r1 + ", " + r2 + ", " + n;

        // <
        internal static string blt(string r1, string r2, string n) => "blt " + r1 + ", " + r2 + ", " + n;

        // <=
        internal static string ble(string r1, string r2, string n) => "ble " + r1 + ", " + r2 + ", " + n;

        internal static string beqz(string r1, string p) => "beqz " + r1 + ", " + p;
        #endregion

        #region Comparacion
        // set on less than - r2 < r3 ? r1 = 1:0
        internal static string slt(string r1, string r2, string r3) => "slt " + r1 + ", " + r2 + ", " + r3;

        internal static string seq(string r1, string r2, string r3) => "seq " + r1 + ", " + r2 + ", " + r3;

        internal static string sge(string r1, string r2, string r3) => "sge " + r1 + ", " + r2 + ", " + r3;

        internal static string sle(string r1, string r2, string r3) => "sle " + r1 + ", " + r2 + ", " + r3;

        // slt con una constante
        internal static string slti(string r1, string r2, string n) => "slti " + r1 + ", " + r2 + ", " + n;
        #endregion

        #region Saltos incondicionales
        // Jump a la direccion n
        internal static string j(string n) => "j " + n;

        // Jump register
        internal static string jr(string r1) => "jr " + r1;

        // Jump and Link. Usando para llamada de metodos.
        // Salva la direccion de retorno en $ra
        internal static string jal(string n) => "jal " + n;

        internal static string jalr(string r1, string r2) => "jalr " + r1 + ", " + r2;
        #endregion

        internal static string Data(List<string> data)
        {
            var s = ".data\nbuff: .space 65536\n" +
                "substrexep: .asciiz \"Substring index exception\"\n"; ;
            foreach (var item in data)
                s += item + NEWLINE;
            return s;
        }

        internal static string Calls()
        {
            #region Herencia
            var s = "\n.globl main\n.text\n";
            s += "_inherit:\n";
            s += lw("$a0", "8", "$a0");
            s += "\n_inherit.cycle:\n" +
                lw("$a2", "0", "$a0") + NEWLINE +
                beq("$a1", "$a2", "_inherit.ok") + NEWLINE +
                beq("$a2", "$zero", "_inherit.wrong") + NEWLINE +
                addiu("$a0", "$a0", 4) + NEWLINE +
                j("_inherit.cycle");
            s += "\n_inherit.wrong:\n" +
                li("$v0", 0) + NEWLINE +
                jr("$ra");
            s += "\n_inherit.ok:\n" +
                li("$v0", 0) + NEWLINE +
                jr("$ra") + NEWLINE;
            #endregion

            #region Copiar
            s += "\n_copy:\n";
            s += lw("$a1", "0", "$sp") + NEWLINE +
                lw("$a0", "-4", "$sp") + NEWLINE +
                li("$v0", 9) + NEWLINE +
                SYSCALL + NEWLINE +
                lw("$a1", "0", "$sp") + NEWLINE +
                lw("$a0", "4", "$sp") + NEWLINE +
                move("$a3", "$v0");
            s += "\n_copy.cycle:\n";
            s += lw("$a2", "0", "$a1") + NEWLINE +
                sw("$a2", "0", "$a3") + NEWLINE +
                addiu("$a0", "$a0", -1) + NEWLINE +
                addiu("$a1", "$a1", 4) + NEWLINE +
                addiu("$a3", "$a3", 4) + NEWLINE +
                beq("$a0", "$zero", "_copy.finish") + NEWLINE +
                j("_copy.cycle");
            s += "\n_copy.finish:\n" + jr("$ra") + "\n\n";
            #endregion

            #region Abortar
            s += "_abort:\n" + li("$v0", 10) + NEWLINE + SYSCALL + NEWLINE;
            #endregion

            #region String de salida
            s += "\n_out_string:\n";
            s += li("$v0", 4) + NEWLINE +
                lw("$a0", "0", "$sp") + NEWLINE +
                SYSCALL + NEWLINE +
                jr("$ra") + NEWLINE;
            #endregion

            #region Entero de salida
            s += "\n_out_int:\n";
            s += li("$v0", 1) + NEWLINE +
                lw("$a0", "0", "$sp") + NEWLINE +
                SYSCALL + NEWLINE +
                jr("$ra") + NEWLINE;
            #endregion

            #region String de entrada
            s += "\n_in_string:\n";
            s += move("$a3", "$ra") + NEWLINE +
                la("$a0", "buff") + NEWLINE +
                li("$a1", 65536) + NEWLINE +
                li("$v0", 8) + NEWLINE + SYSCALL + NEWLINE +
                addiu("$sp", "$sp", -4) + NEWLINE +
                sw("$a0", "0", "$sp") + NEWLINE +
                jal("_strlen") + NEWLINE +
                addiu("$sp", "$sp", 4) + NEWLINE +
                move("$a2", "$v0") + NEWLINE +
                addiu("$a2", "$a2", -1) + NEWLINE +
                move("$a0", "$v0") + NEWLINE +
                li("$v0", 9) + NEWLINE + SYSCALL + NEWLINE +
                move("$v1", "$v0") + NEWLINE +
                la("$a0", "buff") + NEWLINE;
            s += "_in_string.cycle:\n";
            s += beqz("$a2", "_in_string.finish") + NEWLINE +
                lb("$a1", "0", "$a0") + NEWLINE +
                sb("$a1", "0", "$v1") + NEWLINE +
                addiu("$a0", "$a0", 1) + NEWLINE +
                addiu("$v1", "$v1", 1) + NEWLINE +
                addiu("$a2", "$a2", -1) + NEWLINE +
                j("_in_string.cycle") + NEWLINE +
                "_in_string.finish:\n" +
                sb("$zero", "0", "$v1") + NEWLINE +
                move("$ra", "$a3") + NEWLINE + jr("$ra") + NEWLINE;
            #endregion

            #region Entero de entrada
            s += "\n_in_int:\n" + li("$v0", 5) + NEWLINE +
                SYSCALL + NEWLINE + jr("$ra") + NEWLINE;
            #endregion

            #region Largo de un String
            s += "\n_strlen:\n";
            s += lw("$a0", "0", "$sp") + NEWLINE +
                "_strlen.cycle:\n" +
                lb("$a1", "0", "$sp") + NEWLINE +
                beqz("$a0", "_strlen.finish") + NEWLINE +
                addiu("$a0", "$a0", 1) + NEWLINE +
                j("_strlen.cycle") + NEWLINE +
                "_strlen.finish:\n" +
                lw("$a1", "0", "$sp") + NEWLINE +
                subu("$v0", "$a0", "$a1") + NEWLINE +
                jr("$ra") + NEWLINE;
            #endregion

            #region Concatenar Strings
            s += "\n_strcat:\n";
            s += move("$a2", "$ra") + NEWLINE +
                jal("_strlen") + NEWLINE +
                move("$v1", "$v0") + NEWLINE +
                addiu("$sp", "$sp", -4) + NEWLINE +
                jal("_strlen") + NEWLINE +
                addiu("$sp", "$sp", 4) + NEWLINE +
                add("$v1", "$v1", "$v0") + NEWLINE +
                addi("$v1", "$v1", 1) + NEWLINE +
                li("$v0", 9) + NEWLINE +
                move("$a0", "$v1") + NEWLINE + SYSCALL + NEWLINE +
                move("$v1", "$v0") + NEWLINE +
                lw("$a0", "0", "$sp") + NEWLINE +
                "_strcat.firstcycle:\n" +
                lb("$a1", "0", "$a0") + NEWLINE +
                beqz("$a1", "_strcat.firstexit") + NEWLINE +
                sb("$a1", "0", "$v1") + NEWLINE +
                addiu("$a0", "$a0", 1) + NEWLINE +
                addiu("$v1", "$v1", 1) + NEWLINE +
                j("_strcat.firstcycle") + NEWLINE +
                "_strcat.firstexit:\n" +
                lw("$a0", "-4", "$sp") + NEWLINE +
                "_strcat.sndcycle:\n" +
                lb("$a1", "0", "$a0") + NEWLINE +
                beqz("$a1", "_strcat.sndexit") + NEWLINE +
                sb("$a1", "0", "$v1") + NEWLINE +
                addiu("$a0", "$a0", 1) + NEWLINE +
                addiu("$v1", "$v1", 1) + NEWLINE +
                j("_strcat.sndcycle") + NEWLINE +
                "_strcat.sndexit:\n" +
                sb("$zero", "0", "$v1") + NEWLINE +
                move("$ra", "$a2") + NEWLINE +
                jr("$ra") + NEWLINE;
            #endregion

            #region Substring
            s += "\n_substr:\n";
            s += lw("$a0", "-8", "$sp") + NEWLINE +
                addiu("$a0", "$a0", 1) + NEWLINE +
                li("$v0", 9) + NEWLINE + SYSCALL + NEWLINE +
                move("$v1", "$v0") + NEWLINE +
                lw("$a0", "0", "$sp") + NEWLINE +
                lw("$a1", "-4", "$sp") + NEWLINE +
                add("$a0", "$a0", "$a1") + NEWLINE +
                lw("$a2", "-8", "$sp") + NEWLINE +
                "_substr.cycle:\n" +
                beqz("$a2", "_substr.finish") + NEWLINE +
                lb("$a1", "0", "$v1") + NEWLINE +
                beqz("$a1", "_substrE") + NEWLINE +
                sb("$a1", "0", "$v1") + NEWLINE +
                addiu("$a0", "$a0", 1) + NEWLINE +
                addiu("$v1", "$v1", 1) + NEWLINE +
                addiu("$a2", "$a2", -1) + NEWLINE +
                j("_substr.cycle") + NEWLINE +
                "_substr.finish:\n" +
                sb("$zero", "0", "$v1") + NEWLINE +
                jr("$ra") + NEWLINE;

            s += "\n_substrE:\n";
            s += la("$a0", "substrexep") + NEWLINE +
                li("$v0", 4) + NEWLINE + SYSCALL + NEWLINE +
                li("$v0", 10) + NEWLINE + SYSCALL + NEWLINE;
            #endregion

            #region Comparar dos strings
            s += "_strcmp:\n";
            s += li("$v0", 1) + NEWLINE +
                "_strcmp.cycle:\n" +
                lb("$a2", "0", "$a0") + NEWLINE +
                lb("$a3", "0", "$a1") + NEWLINE +
                beqz("$a2", "_strcmp.finish") + NEWLINE +
                beq("$a2", "$zero", "_strcmp.finish") + NEWLINE +
                beq("$a3", "$zero", "_strcmp.finish") + NEWLINE +
                bne("$a2", "$a3", "_strcmp.neq") + NEWLINE +
                addiu("$a0", "$a0", 1) + NEWLINE +
                addiu("$a1", "$a1", 1) + NEWLINE +
                j("_strcmp.cycle") + NEWLINE +
                "_strcmp.finish:\n" +
                beq("$a2", "$a3", "_strcmp.eq") + NEWLINE +
                "_strcmp.neq:\n" +
                li("$v0", 0) + NEWLINE +
                jr("$ra") + NEWLINE +
                "_strcmp.eq:\n" +
                li("$v0", 1) + NEWLINE +
                jr("$ra") + NEWLINE;
            #endregion

            s += "\nmain:\n";
            return s;
        }

        public static string Generate(List<ICCode> lines)
        {
            var data = new List<string>();
            Generals g = new Generals(lines);
            foreach (var item in g.StrCount)
                data.Add("string" + item.Value + ": .asciiz \"" + (item.Key[0] == '"' ? item.Key.Substring(1, item.Key.Length - 2) : item.Key) + "\"");
            foreach (var item in g.Inherit)
            {
                var tmp = "_class." + item.Key + ": .word string" + g.StrCount[item.Key] + ", ";
                var parent = item.Value;
                while (parent != "Object")
                {
                    tmp += "string" + g.StrCount[parent] + ", ";
                    parent = g.Inherit[parent];
                }
                tmp += "string" + g.StrCount["Object"] + ", 0";
                data.Add(tmp);
            }
            var s = Data(data);
            s += Calls();

            foreach (var item in lines)
            {
                Review(item, g);
                foreach (var line in item.Generate())
                    s += line + NEWLINE;
            }
            s += FINISH;
            return s;
        }

        static void Review(ICCode line, Generals g)
        {
            if (line is ICLabel)
            {
                var l = line as ICLabel;
                if (l.LHead[0] != '_')
                {
                    f = l.LFull;
                    size = g.FVarSize[f];
                }
            }
            if (line is ICAssignStrToVar)
            {
                ((ICAssignStrToVar)line).R = "string" + g.StrCount[((ICAssignStrToVar)line).R];
            }
            if (line is ICPopParams)
            {
                pcount = 0;
            }
            if (line is ICPushParams)
            {
                ++pcount;
                ((ICPushParams)line).Set(pcount, size);
            }
            if (line is ICAssignStrToMem)
            {
                ((ICAssignStrToMem)line).R = "string" + g.StrCount[((ICAssignStrToMem)line).R];
            }
            if (line is ICCallLabel)
            {
                ((ICCallLabel)line).Set(size);
            }
            if (line is ICCallAddress)
            {
                ((ICCallAddress)line).Set(size);
            }
        }
    }
}

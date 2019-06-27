using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.AST.Class;

namespace Core.SemanticCheck
{
    public class Type
    {
        public string Text { get; set; }
        public Type Parent { get; set; }
        public ClassNode ClassReference { get; set; }
        public int Level { get; set; }

        public Type()
        {
            Text = "Object";
            Parent = null;
            ClassReference = null;
            Level = 0;
        }

        public Type(string text, Type parent, ClassNode classReference)
        {
            Text = text;
            Parent = parent;
            ClassReference = classReference;
            Level = parent.Level + 1;
        }

        /// <summary>
        /// Check if a type inherit of other type in the hierarchy of the program.
        /// </summary>
        /// <param name="other">Represent the second type to check</param>
        /// <returns>True if the first type inherit of the second</returns>
        public virtual bool Inherit(Type other)
        {
            if (this == other) return true;
            return Parent.Inherit(other);
        }

        public static bool operator <=(Type a, Type b)
        {
            return a.Inherit(b);
        }

        public static bool operator >=(Type a, Type b)
        {
            return b.Inherit(a);
        }

        public static bool operator ==(Type a, Type b)
        {
            return a.Text == b.Text;
        }

        public static bool operator !=(Type a, Type b)
        {
            return !(a == b);
        }

        #region OBJECT
        private static ObjectType objectType = new ObjectType();

        public static ObjectType OBJECT => objectType;

        public class ObjectType : Type
        {

            public override bool Inherit(Type other)
            {
                return this == other;
            }
        }
        #endregion

        public override string ToString()
        {
            return Text + " : " + Parent.Text;
        }
    }
}

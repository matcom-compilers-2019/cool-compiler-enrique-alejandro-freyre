using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public abstract class ICCode
    {
        public abstract List<string> Generate();
    }
}

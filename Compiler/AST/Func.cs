using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public class Func
    {
        public List<Var> Params { get; set; } = new();
        public IExpr Body { get; set; }

        public Func(IExpr body)
        {
            Body = body;
        }
    }
}

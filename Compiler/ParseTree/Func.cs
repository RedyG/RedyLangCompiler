using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Func
    {
        public Proto Proto { get; }
        public IExpr Body { get; }

        public Func(Proto proto, IExpr body)
        {
            Proto = proto;
            Body = body;
        }
    }
}



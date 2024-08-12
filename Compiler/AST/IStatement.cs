using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.AST
{
    public interface IStatement
    {
        public void CodeGen(ByteCode.Func func, CodeGenContext ctx);
    }
}

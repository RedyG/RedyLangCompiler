using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ByteCode
{
    public enum Intrinsic : UInt16
    {
        Exit,

        Print,
        Println,
        ReadLine,

        ReadFile,
        WriteFile,

        StringConcat,
    }
}

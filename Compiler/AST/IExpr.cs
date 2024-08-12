﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.ByteCode;

namespace Compiler.AST
{
    public interface IExpr
    {
        public void CodeGen(ByteCode.Func func);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public struct TextRange
    {
        public TextPos Start;
        public TextPos End;

        public TextRange(TextPos start, TextPos end)
        {
            Start = start;
            End = end;
        }

        public TextRange() : this(new TextPos(), new TextPos())
        {
        }
    }
}

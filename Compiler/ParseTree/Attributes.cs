using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class Attributes : INode
    {
        public TextRange Range { get; }
        public List<Identifier> Identifiers { get; }

        public Attributes(TextRange range, List<Identifier> identifiers)
        {
            Range = range;
            Identifiers = identifiers;
        }
    }
}

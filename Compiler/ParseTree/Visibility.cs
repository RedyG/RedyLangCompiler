using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public enum Visibility
    {
        Pub,
        Priv,
    }

    public class VisibilityNode : INode
    {
        public TextRange Range { get; }
        public Visibility Visibility { get; }

        public VisibilityNode(TextRange range, Visibility visibility)
        {
            Range = range;
            Visibility = visibility;
        }
    }

}
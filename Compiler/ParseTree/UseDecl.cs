using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.ParseTree
{
    public class UseDecl
    {
        public VisibilityNode VisibilityNode { get; }
        public ModuleFile ModuleFile { get; }

        public List<Identifier> Path;
        public List<Identifier> Imported;

        public UseDecl(VisibilityNode visibilityNode, ModuleFile moduleFile, List<Identifier> path, List<Identifier> imported)
        {
            VisibilityNode = visibilityNode;
            ModuleFile = moduleFile;
            Path = path;
            Imported = imported;
        }
    }
}

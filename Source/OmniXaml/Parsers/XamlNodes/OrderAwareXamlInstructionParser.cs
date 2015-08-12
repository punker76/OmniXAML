namespace OmniXaml.Parsers.XamlNodes
{
    using System.Collections.Generic;
    using Tests;

    public class OrderAwareXamlInstructionParser : IXamlInstructionParser
    {
        private readonly IXamlInstructionParser parser;

        public OrderAwareXamlInstructionParser(IXamlInstructionParser parser)
        {
            this.parser = parser;
        }

        public IEnumerable<XamlInstruction> Parse(IEnumerable<ProtoXamlInstruction> protoNodes)
        {
            var nodeSorter = new MemberDependencyNodeSorter();
            var originalNodes = parser.Parse(protoNodes);
            var enumerator = originalNodes.GetEnumerator();
            return nodeSorter.Sort(enumerator);
        }
    }
}
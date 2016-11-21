﻿namespace OmniXaml.Tests.XmlParserTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using DefaultLoader;
    using Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Model;

    [TestClass]
    public class AssigmentExtractorTests
    {
        [TestMethod]
        public void ContentPropertyWithChildren()
        {
            var assigments = Parse(@"<ItemsControl xmlns=""root""><TextBlock/><TextBlock/><TextBlock/></ItemsControl>", (e, a) => new ConstructionNode(typeof(TextBlock)));

            var expected = new[]
            {
                new MemberAssignment
                {
                    Member = Member.FromStandard<ItemsControl>(collection => collection.Items),
                    Children = new[]
                    {
                        new ConstructionNode(typeof(TextBlock)),
                        new ConstructionNode(typeof(TextBlock)),
                        new ConstructionNode(typeof(TextBlock)),
                    }
                }
            };

            CollectionAssert.AreEqual(expected, assigments);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void ElementOrdering_InvalidPropertyElementOrder()
        {
            Parse(@"<ItemsControl xmlns=""root"">
<TextBlock/>
<ItemsControl.HeaderText>Hola</ItemsControl.HeaderText>
<TextBlock/>
</ItemsControl>", (element, annotator) => new ConstructionNode(typeof(TextBlock)));
           
        }

        [TestMethod]
        public void ElementOrdering_PropertyAfterDirectContent()
        {
            Parse(@"<ItemsControl xmlns=""root"">
<TextBlock/>
<TextBlock/>
<ItemsControl.HeaderText>Hola</ItemsControl.HeaderText>
</ItemsControl>", (element, annotator) => new ConstructionNode(typeof(TextBlock)));

        }

        [TestMethod]
        public void ElementOrdering_PropertyBeforeDirectContent()
        {
            Parse(@"<ItemsControl xmlns=""root"">
<ItemsControl.HeaderText>Hola</ItemsControl.HeaderText>
<TextBlock/>
<TextBlock/>
</ItemsControl>", (element, annotator) => new ConstructionNode(typeof(TextBlock)));

        }

        private static List<MemberAssignment> Parse(string xaml, Func<XElement, IPrefixAnnotator, ConstructionNode> parser)
        {
            var typeDirectory = new AttributeBasedTypeDirectory(new List<Assembly>() { Assembly.GetExecutingAssembly() });
            var sut = new AssignmentExtractor(new AttributeBasedMetadataProvider(), new InlineParser[0], new Resolver(typeDirectory), parser);

            var assigments = sut.GetAssignments(typeof(ItemsControl), XElement.Parse(xaml), new PrefixAnnotator()).ToList();
            return assigments;
        }

        [TestMethod]
        public void PropertyElement()
        {
            var xaml =
@"<ItemsControl xmlns=""root"">
    <ItemsControl.HeaderText>Hola</ItemsControl.HeaderText>
</ItemsControl>";

            var constructionNode = new ConstructionNode(typeof(string));
            var assigments = Parse(xaml, (element, annotator) => constructionNode);

            var expected = new[]
            {
                new MemberAssignment
                {
                    Member = Member.FromStandard<ItemsControl>(collection => collection.HeaderText),
                    SourceValue = "Hola",
                }
            };

            CollectionAssert.AreEqual(expected, assigments);
        }
    }    
}
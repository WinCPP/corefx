// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;

namespace XPathTests.XPathNavigatorTests
{
    public class XPathNavigatorReaderTests
    {
        XmlDocument _document;
        XPathNavigator _nav;
        XPathDocument _xpathDocument;

        public XPathNavigatorReaderTests()
        {
            _document = new XmlDocument();
        }

        private XPathNavigator GetXmlDocumentNavigator(string xml)
        {
            _document.LoadXml(xml);
            return _document.CreateNavigator();
        }

        private XPathNavigator GetXPathDocumentNavigator(XmlNode node)
        {
            XmlNodeReader xr = new XmlNodeReader(node);
            _xpathDocument = new XPathDocument(xr);
            return _xpathDocument.CreateNavigator();
        }

        [Fact]
        public void ReadSubtree1()
        {
            string xml = "<root/>";

            _nav = GetXmlDocumentNavigator(xml);
            ReadSubtree1(_nav, "#1.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            ReadSubtree1(_nav, "#2.");

            _nav = GetXPathDocumentNavigator(_document);
            ReadSubtree1(_nav, "#3.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            ReadSubtree1(_nav, "#4.");
        }

        void ReadSubtree1(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();

            XmlAssert.AssertNode(label + "#1", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.None, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.True(r.Read(), label + "#2");
            XmlAssert.AssertNode(label + "#3", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 0, true,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.False(r.Read(), label + "#4");
        }

        [Fact]
        public void ReadSubtree2()
        {
            string xml = "<root></root>";

            _nav = GetXmlDocumentNavigator(xml);
            ReadSubtree2(_nav, "#1.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            ReadSubtree2(_nav, "#2.");

            _nav = GetXPathDocumentNavigator(_document);
            ReadSubtree2(_nav, "#3.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            ReadSubtree2(_nav, "#4.");
        }

        void ReadSubtree2(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();

            XmlAssert.AssertNode(label + "#1", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.None, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.True(r.Read(), label + "#2");
            XmlAssert.AssertNode(label + "#3", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.True(r.Read(), label + "#4");
            XmlAssert.AssertNode(label + "#5", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.EndElement, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.False(r.Read(), label + "#6");
        }

        [Fact]
        public void ReadSubtree3()
        {
            string xml = "<root attr='value'/>";

            _nav = GetXmlDocumentNavigator(xml);
            ReadSubtree3(_nav, "#1.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            ReadSubtree3(_nav, "#2.");

            _nav = GetXPathDocumentNavigator(_document);
            ReadSubtree3(_nav, "#3.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            ReadSubtree3(_nav, "#4.");
        }

        void ReadSubtree3(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();

            XmlAssert.AssertNode(label + "#1", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.None, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.True(r.Read(), label + "#2");
            XmlAssert.AssertNode(label + "#3", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 0, true,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 1, true);

            Assert.True(r.MoveToFirstAttribute(), label + "#4");
            XmlAssert.AssertNode(label + "#5", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Attribute, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "attr", String.Empty, "attr", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "value", true, 1, true);

            Assert.True(r.ReadAttributeValue(), label + "#6");
            XmlAssert.AssertNode(label + "#7", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "value", true, 1, true);

            Assert.False(r.ReadAttributeValue(), label + "#8");
            Assert.False(r.MoveToNextAttribute(), label + "#9");
            Assert.True(r.MoveToElement(), label + "#10");

            Assert.False(r.Read(), label + "#11");
        }

        [Fact]
        public void DocElem_OpenClose_Attribute()
        {
            string xml = "<root attr='value'></root>";

            _nav = GetXmlDocumentNavigator(xml);
            DocElem_OpenClose_Attribute(_nav, "#1.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            DocElem_OpenClose_Attribute(_nav, "#2.");

            _nav = GetXPathDocumentNavigator(_document);
            DocElem_OpenClose_Attribute(_nav, "#3.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            DocElem_OpenClose_Attribute(_nav, "#4.");
        }

        void DocElem_OpenClose_Attribute(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();

            XmlAssert.AssertNode(label + "#1", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.None, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.True(r.Read(), label + "#2");
            XmlAssert.AssertNode(label + "#3", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 1, true);

            Assert.True(r.MoveToFirstAttribute(), label + "#4");
            XmlAssert.AssertNode(label + "#5", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Attribute, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "attr", String.Empty, "attr", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "value", true, 1, true);

            Assert.True(r.ReadAttributeValue(), label + "#6");
            XmlAssert.AssertNode(label + "#7", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "value", true, 1, true);

            Assert.False(r.ReadAttributeValue(), label + "#8");
            Assert.False(r.MoveToNextAttribute(), label + "#9");
            Assert.True(r.MoveToElement(), label + "#10");

            Assert.True(r.Read(), label + "#11");
            XmlAssert.AssertNode(label + "#12", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.EndElement, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.False(r.Read(), label + "#13");
        }

        [Fact]
        public void FromChildElement()
        {
            string xml = "<root><foo attr='value'>test</foo><bar/></root>";

            _nav = GetXmlDocumentNavigator(xml);
            _nav.MoveToFirstChild();
            _nav.MoveToFirstChild(); // foo
            FromChildElement(_nav, "#1.");

            _nav = GetXPathDocumentNavigator(_document);
            _nav.MoveToFirstChild();
            _nav.MoveToFirstChild(); // foo
            FromChildElement(_nav, "#2.");
        }

        void FromChildElement(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();

            XmlAssert.AssertNode(label + "#1", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.None, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.True(r.Read(), label + "#2");
            XmlAssert.AssertNode(label + "#3", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "foo", String.Empty, "foo", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 1, true);

            Assert.True(r.Read(), label + "#4");
            XmlAssert.AssertNode(label + "#5", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "test", true, 0, false);

            Assert.True(r.Read(), label + "#6");
            XmlAssert.AssertNode(label + "#7", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.EndElement, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "foo", String.Empty, "foo", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            // end at </foo>, without moving toward <bar>.
            Assert.False(r.Read(), label + "#8");
        }

        [Fact]
        public void AttributesAndNamespaces()
        {
            string xml = "<root attr='value' x:a2='v2' xmlns:x='urn:foo' xmlns='urn:default'></root>";

            _nav = GetXmlDocumentNavigator(xml);
            AttributesAndNamespaces(_nav, "#1.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            AttributesAndNamespaces(_nav, "#2.");

            _nav = GetXPathDocumentNavigator(_document);
            AttributesAndNamespaces(_nav, "#3.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            AttributesAndNamespaces(_nav, "#4.");
        }

        void AttributesAndNamespaces(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();

            XmlAssert.AssertNode(label + "#1", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.None, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.True(r.Read(), label + "#2");
            XmlAssert.AssertNode(label + "#3", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", "urn:default",
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 4, true);

            // Namespaces

            Assert.True(r.MoveToAttribute("xmlns:x"), label + "#4");
            XmlAssert.AssertNode(label + "#5", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Attribute, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "xmlns:x", "xmlns", "x", "http://www.w3.org/2000/xmlns/",
                // Value, HasValue, AttributeCount, HasAttributes
                "urn:foo", true, 4, true);

            Assert.True(r.ReadAttributeValue(), label + "#6");
            ///* MS.NET has a bug here
            XmlAssert.AssertNode(label + "#7", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                "xmlns:", "xmlns", String.Empty, "http://www.w3.org/2000/xmlns/",
                // Value, HasValue, AttributeCount, HasAttributes
                "urn:foo", true, 4, true);
            //*/

            Assert.False(r.ReadAttributeValue(), label + "#8");

            Assert.True(r.MoveToAttribute("xmlns"), label + "#9");
            XmlAssert.AssertNode(label + "#10", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Attribute, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "xmlns", String.Empty, "xmlns", "http://www.w3.org/2000/xmlns/",
                // Value, HasValue, AttributeCount, HasAttributes
                "urn:default", true, 4, true);

            Assert.True(r.ReadAttributeValue(), label + "#11");
            ///* MS.NET has a bug here
            XmlAssert.AssertNode(label + "#12", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                "xmlns", String.Empty, "xmlns", "http://www.w3.org/2000/xmlns/",
                // Value, HasValue, AttributeCount, HasAttributes
                "urn:default", true, 4, true);
            //*/

            Assert.False(r.ReadAttributeValue(), label + "#13");

            // Attributes

            Assert.True(r.MoveToAttribute("attr"), label + "#14");
            XmlAssert.AssertNode(label + "#15", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Attribute, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "attr", String.Empty, "attr", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "value", true, 4, true);

            Assert.True(r.ReadAttributeValue(), label + "#16");
            XmlAssert.AssertNode(label + "#17", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "value", true, 4, true);

            Assert.False(r.ReadAttributeValue(), label + "#18");

            Assert.True(r.MoveToAttribute("x:a2"), label + "#19");
            XmlAssert.AssertNode(label + "#20", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Attribute, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "x:a2", "x", "a2", "urn:foo",
                // Value, HasValue, AttributeCount, HasAttributes
                "v2", true, 4, true);

            Assert.True(r.ReadAttributeValue(), label + "#21");
            XmlAssert.AssertNode(label + "#22", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "v2", true, 4, true);

            Assert.True(r.MoveToElement(), label + "#24");

            Assert.True(r.Read(), label + "#25");
            XmlAssert.AssertNode(label + "#26", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.EndElement, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "root", String.Empty, "root", "urn:default",
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.False(r.Read(), label + "#27");
        }

        [Fact]
        public void MixedContentAndDepth()
        {
            string xml = @"<one>  <two>Some data.<three>more</three> done.</two>  </one>";

            _nav = GetXmlDocumentNavigator(xml);
            MixedContentAndDepth(_nav, "#1.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            MixedContentAndDepth(_nav, "#2.");

            _nav = GetXPathDocumentNavigator(_document);
            MixedContentAndDepth(_nav, "#3.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            MixedContentAndDepth(_nav, "#4.");
        }

        void MixedContentAndDepth(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();
            r.Read();
            XmlAssert.AssertNode(label + "#1", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "one", String.Empty, "one", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#2", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "two", String.Empty, "two", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#3", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "Some data.", true, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#4", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Element, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                "three", String.Empty, "three", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#5", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 3, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                "more", true, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#6", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.EndElement, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                "three", String.Empty, "three", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#7", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.Text, 2, false,
                // Name, Prefix, LocalName, NamespaceURI
                String.Empty, String.Empty, String.Empty, String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                " done.", true, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#8", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.EndElement, 1, false,
                // Name, Prefix, LocalName, NamespaceURI
                "two", String.Empty, "two", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            r.Read();
            XmlAssert.AssertNode(label + "#9", r,
                // NodeType, Depth, IsEmptyElement
                XmlNodeType.EndElement, 0, false,
                // Name, Prefix, LocalName, NamespaceURI
                "one", String.Empty, "one", String.Empty,
                // Value, HasValue, AttributeCount, HasAttributes
                String.Empty, false, 0, false);

            Assert.False(r.Read(), label + "#10");
        }

        [Fact]
        public void MoveToFirstAttributeFromAttribute()
        {
            string xml = @"<one xmlns:foo='urn:foo' a='v' />";

            _nav = GetXmlDocumentNavigator(xml);
            MoveToFirstAttributeFromAttribute(_nav, "#1.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            MoveToFirstAttributeFromAttribute(_nav, "#2.");

            _nav = GetXPathDocumentNavigator(_document);
            MoveToFirstAttributeFromAttribute(_nav, "#3.");

            _nav.MoveToRoot();
            _nav.MoveToFirstChild();
            MoveToFirstAttributeFromAttribute(_nav, "#4.");
        }

        void MoveToFirstAttributeFromAttribute(XPathNavigator nav, string label)
        {
            XmlReader r = nav.ReadSubtree();
            r.MoveToContent();
            Assert.True(r.MoveToFirstAttribute(), label + "#1");
            Assert.True(r.MoveToFirstAttribute(), label + "#2");
            r.ReadAttributeValue();
            Assert.True(r.MoveToFirstAttribute(), label + "#3");
            Assert.True(r.MoveToNextAttribute(), label + "#4");
            Assert.True(r.MoveToFirstAttribute(), label + "#5");
        }

        [Fact]
        public void ReadSubtreeAttribute()
        {
            string xml = "<root a='b' />";
            _nav = GetXmlDocumentNavigator(xml);
            _nav.MoveToFirstChild();
            _nav.MoveToFirstAttribute();
            Assert.Throws<InvalidOperationException>(() => _nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreeNamespace()
        {
            string xml = "<root xmlns='urn:foo' />";
            _nav = GetXmlDocumentNavigator(xml);
            _nav.MoveToFirstChild();
            _nav.MoveToFirstNamespace();
            Assert.Throws<InvalidOperationException>(() => _nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreePI()
        {
            string xml = "<?pi ?><root xmlns='urn:foo' />";
            _nav = GetXmlDocumentNavigator(xml);
            _nav.MoveToFirstChild();
            Assert.Throws<InvalidOperationException>(() => _nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreeComment()
        {
            string xml = "<!-- comment --><root xmlns='urn:foo' />";
            _nav = GetXmlDocumentNavigator(xml);
            _nav.MoveToFirstChild();
            Assert.Throws<InvalidOperationException>(() => _nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreeAttributesByIndex()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<u:Timestamp u:Id='ID1' xmlns:u='urn:foo'></u:Timestamp>");
            XmlReader r = doc.CreateNavigator().ReadSubtree();
            r.Read();
            r.MoveToAttribute(0);
            if (r.LocalName != "Id")
                r.MoveToAttribute(1);
            Assert.Equal("Id", r.LocalName);
        }
    }
}

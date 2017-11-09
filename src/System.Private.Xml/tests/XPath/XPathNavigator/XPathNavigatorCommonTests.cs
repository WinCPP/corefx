// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class XPathNavigatorCommonTests
    {
        XmlDocument _document;
        XPathNavigator _nav;
        XPathDocument _xpathDocument;

        private XPathNavigator GetXmlDocumentNavigator(string xml)
        {
            _document = new XmlDocument();
            _document.LoadXml(xml);
            return _document.CreateNavigator();
        }

        private XPathNavigator GetXPathDocumentNavigator(XmlNode node)
        {
            XmlNodeReader xr = new XmlNodeReader(node);
            _xpathDocument = new XPathDocument(xr);
            return _xpathDocument.CreateNavigator();
        }

        private XPathNavigator GetXPathDocumentNavigator(XmlNode node, XmlSpace space)
        {
            XmlNodeReader xr = new XmlNodeReader(node);
            _xpathDocument = new XPathDocument(xr, space);
            return _xpathDocument.CreateNavigator();
        }

        private void AssertNavigator(XPathNavigator nav, XPathNodeType type, string prefix, string localName, string ns, string name, string value, bool hasAttributes, bool hasChildren, bool isEmptyElement)
        {
            Assert.Equal(type, nav.NodeType);
            Assert.Equal(prefix, nav.Prefix);
            Assert.Equal(localName, nav.LocalName);
            Assert.Equal(ns, nav.NamespaceURI);
            Assert.Equal(name, nav.Name);
            Assert.Equal(value, nav.Value);
            Assert.Equal(hasAttributes, nav.HasAttributes);
            Assert.Equal(hasChildren, nav.HasChildren);
            Assert.Equal(isEmptyElement, nav.IsEmptyElement);
        }

        [Fact]
        public void DocumentWithXmlDeclaration()
        {
            string xml = "<?xml version=\"1.0\" standalone=\"yes\"?><foo>bar</foo>";

            _nav = GetXmlDocumentNavigator(xml);
            DocumentWithXmlDeclaration(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            DocumentWithXmlDeclaration(_nav);
        }

        public void DocumentWithXmlDeclaration(XPathNavigator nav)
        {
            nav.MoveToFirstChild();
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "bar", false, true, false);
        }

        [Fact]
        public void DocumentWithProcessingInstruction()
        {
            string xml = "<?xml-stylesheet href='foo.xsl' type='text/xsl' ?><foo />";

            _nav = GetXmlDocumentNavigator(xml);
            DocumentWithProcessingInstruction(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            DocumentWithProcessingInstruction(_nav);
        }

        public void DocumentWithProcessingInstruction(XPathNavigator nav)
        {
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.ProcessingInstruction, "", "xml-stylesheet", "", "xml-stylesheet", "href='foo.xsl' type='text/xsl' ", false, false, false);
            Assert.False(nav.MoveToFirstChild());
        }

        [Fact]
        public void XmlRootElementOnly()
        {
            string xml = "<foo />";

            _nav = GetXmlDocumentNavigator(xml);
            XmlRootElementOnly(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            XmlRootElementOnly(_nav);
        }

        private void XmlRootElementOnly(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, false, true);
            Assert.False(nav.MoveToFirstChild());
            Assert.False(nav.MoveToNext());
            Assert.False(nav.MoveToPrevious());
            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.False(nav.MoveToNext());
        }

        [Fact]
        public void XmlSimpleTextContent()
        {
            string xml = "<foo>Test.</foo>";

            _nav = GetXmlDocumentNavigator(xml);
            XmlSimpleTextContent(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            XmlSimpleTextContent(_nav);
        }

        private void XmlSimpleTextContent(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "Test.", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "Test.", false, true, false);
            Assert.False(nav.MoveToNext());
            Assert.False(nav.MoveToPrevious());
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Text, "", "", "", "", "Test.", false, false, false);

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "Test.", false, true, false);

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "Test.", false, true, false);

            nav.MoveToFirstChild();
            nav.MoveToFirstChild();
            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "Test.", false, true, false);
            Assert.False(nav.MoveToNext());
        }

        [Fact]
        public void XmlSimpleElementContent()
        {
            string xml = "<foo><bar /></foo>";

            _nav = GetXmlDocumentNavigator(xml);
            XmlSimpleElementContent(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            XmlSimpleElementContent(_nav);
        }

        private void XmlSimpleElementContent(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.False(nav.MoveToNext());
            Assert.False(nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.False(nav.MoveToNext());
        }

        [Fact]
        public void XmlTwoElementsContent()
        {
            string xml = "<foo><bar /><baz /></foo>";

            _nav = GetXmlDocumentNavigator(xml);
            XmlTwoElementsContent(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            XmlTwoElementsContent(_nav);
        }

        private void XmlTwoElementsContent(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.False(nav.MoveToNext());
            Assert.False(nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);
            Assert.False(nav.MoveToFirstChild());

            Assert.True(nav.MoveToNext());
            AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
            Assert.False(nav.MoveToFirstChild());

            Assert.True(nav.MoveToPrevious());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.False(nav.MoveToNext());
        }

        [Fact]
        public void XmlElementWithAttributes()
        {
            string xml = "<img src='foo.png' alt='image Fooooooo!' />";

            _nav = GetXmlDocumentNavigator(xml);
            XmlElementWithAttributes(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            XmlElementWithAttributes(_nav);
        }

        private void XmlElementWithAttributes(XPathNavigator nav)
        {
            nav.MoveToFirstChild();
            AssertNavigator(nav, XPathNodeType.Element, "", "img", "", "img", "", true, false, true);
            Assert.False(nav.MoveToNext());
            Assert.False(nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstAttribute());
            AssertNavigator(nav, XPathNodeType.Attribute, "", "src", "", "src", "foo.png", false, false, false);
            Assert.False(nav.MoveToFirstAttribute()); // On attributes, it fails.

            Assert.True(nav.MoveToNextAttribute());
            AssertNavigator(nav, XPathNodeType.Attribute, "", "alt", "", "alt", "image Fooooooo!", false, false, false);
            Assert.False(nav.MoveToNextAttribute());

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "img", "", "img", "", true, false, true);

            Assert.True(nav.MoveToAttribute("alt", ""));
            AssertNavigator(nav, XPathNodeType.Attribute, "", "alt", "", "alt", "image Fooooooo!", false, false, false);
            Assert.False(nav.MoveToAttribute("src", "")); // On attributes, it fails.
            Assert.True(nav.MoveToParent());
            Assert.True(nav.MoveToAttribute("src", ""));
            AssertNavigator(nav, XPathNodeType.Attribute, "", "src", "", "src", "foo.png", false, false, false);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
        }

        [Fact]
        public void XmlNamespaceNode()
        {
            string xml = "<html xmlns='http://www.w3.org/1999/xhtml'><body>test.</body></html>";

            _nav = GetXmlDocumentNavigator(xml);
            XmlNamespaceNode(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            XmlNamespaceNode(_nav);
        }

        private void XmlNamespaceNode(XPathNavigator nav)
        {
            string xhtml = "http://www.w3.org/1999/xhtml";
            string xmlNS = "http://www.w3.org/XML/1998/namespace";

            nav.MoveToFirstChild();
            AssertNavigator(nav, XPathNodeType.Element, "", "html", xhtml, "html", "test.", false, true, false);

            Assert.True(nav.MoveToFirstNamespace(XPathNamespaceScope.Local));
            AssertNavigator(nav, XPathNodeType.Namespace, "", "", "", "", xhtml, false, false, false);

            // Test difference between Local, ExcludeXml and All.
            Assert.False(nav.MoveToNextNamespace(XPathNamespaceScope.Local));
            Assert.False(nav.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));

            // see http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316808
            Assert.True(nav.MoveToNextNamespace(XPathNamespaceScope.All));
            AssertNavigator(nav, XPathNodeType.Namespace, "", "xml", "", "xml", xmlNS, false, false, false);
            Assert.False(nav.MoveToNextNamespace(XPathNamespaceScope.All));

            // Test to check if MoveToRoot() resets Namespace node status.
            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "test.", false, true, false);
            nav.MoveToFirstChild();

            // Test without XPathNamespaceScope argument.
            Assert.True(nav.MoveToFirstNamespace());
            Assert.True(nav.MoveToNextNamespace());
            AssertNavigator(nav, XPathNodeType.Namespace, "", "xml", "", "xml", xmlNS, false, false, false);

            // Test MoveToParent()
            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "html", xhtml, "html", "test.", false, true, false);

            nav.MoveToFirstChild(); // body
                                    // Test difference between Local and ExcludeXml
            Assert.False(nav.MoveToFirstNamespace(XPathNamespaceScope.Local), "Local should fail");
            Assert.True(nav.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml), "ExcludeXml should succeed");
            AssertNavigator(nav, XPathNodeType.Namespace, "", "", "", "", xhtml, false, false, false);

            Assert.True(nav.MoveToNextNamespace(XPathNamespaceScope.All));
            AssertNavigator(nav, XPathNodeType.Namespace, "", "xml", "", "xml", xmlNS, false, false, false);
            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "body", xhtml, "body", "test.", false, true, false);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "test.", false, true, false);
        }

        [Fact]
        public void MoveToNamespaces()
        {
            string xml = "<a xmlns:x='urn:x'><b xmlns:y='urn:y'/><c/><d><e attr='a'/></d></a>";

            _nav = GetXmlDocumentNavigator(xml);
            MoveToNamespaces(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            MoveToNamespaces(_nav);
        }

        private void MoveToNamespaces(XPathNavigator nav)
        {
            XPathNodeIterator iter = nav.Select("//e");
            iter.MoveNext();
            nav.MoveTo(iter.Current);
            Assert.Equal("e", nav.Name);
            nav.MoveToFirstNamespace();
            Assert.Equal("x", nav.Name);
            nav.MoveToNextNamespace();
            Assert.Equal("xml", nav.Name);
        }

        [Fact]
        public void IsDescendant()
        {
            string xml = "<a><b/><c/><d><e attr='a'/></d></a>";

            _nav = GetXmlDocumentNavigator(xml);
            IsDescendant(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            IsDescendant(_nav);
        }

        private void IsDescendant(XPathNavigator nav)
        {
            XPathNavigator tmp = nav.Clone();
            XPathNodeIterator iter = nav.Select("//e");
            iter.MoveNext();
            Assert.True(nav.MoveTo(iter.Current));
            Assert.True(nav.MoveToFirstAttribute());
            Assert.Equal("attr", nav.Name);
            Assert.Equal("", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.False(nav.IsDescendant(tmp));
            tmp.MoveToFirstChild();
            Assert.Equal("a", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.False(nav.IsDescendant(tmp));
            tmp.MoveTo(iter.Current);
            Assert.Equal("e", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.False(nav.IsDescendant(tmp));
        }

        [Fact]
        public void LiterallySplittedText()
        {
            string xml = "<root><![CDATA[Fact]]> string</root>";

            _nav = GetXmlDocumentNavigator(xml);
            LiterallySplittedText(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            LiterallySplittedText(_nav);
        }

        private void LiterallySplittedText(XPathNavigator nav)
        {
            nav.MoveToFirstChild();
            nav.MoveToFirstChild();
            Assert.Equal(XPathNodeType.Text, nav.NodeType);
            Assert.Equal("Fact string", nav.Value);
        }

        [Fact]
        public void SelectChildren()
        {
            string xml = "<root><foo xmlns='urn:foo' /><ns:foo xmlns:ns='urn:foo' /></root>";

            _nav = GetXmlDocumentNavigator(xml);
            SelectChildrenNS(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            SelectChildrenNS(_nav);
        }

        private void SelectChildrenNS(XPathNavigator nav)
        {
            nav.MoveToFirstChild(); // root
            XPathNodeIterator iter = nav.SelectChildren("foo", "urn:foo");
            Assert.Equal(2, iter.Count);
        }


        [Fact]
        public void OuterXml()
        {
            string xml = @"<?xml version=""1.0""?><one><two>Some data.</two></one>";

            _nav = GetXmlDocumentNavigator(xml);
            OuterXml(_nav);
            _nav = GetXPathDocumentNavigator(_document);
            OuterXml(_nav);
        }

        private void OuterXml(XPathNavigator nav)
        {
            string ret = @"<one>
  <two>Some data.</two>
</one>";
            Assert.Equal(ret.Replace("\r\n", "\n"), nav.OuterXml.Replace("\r\n", "\n"));
        }

        [Fact]
        public void ReadSubtreeLookupNamespace()
        {
            string xml = "<x:foo xmlns:x='urn:x'><bar>x:val</bar></x:foo>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.LastChild.LastChild.CreateNavigator();
            XmlReader xr = nav.ReadSubtree();
            xr.MoveToContent();
            xr.Read(); // should be at x:val
            Assert.Equal("urn:x", xr.LookupNamespace("x"));
        }

        [Fact]
        public void GetNamespaceConsistentTree()
        {
            string xml = "<x:root xmlns:x='urn:x'>  <x:foo xmlns='ns1'> <x:bar /> </x:foo>  <x:foo xmlns:y='ns2'> <x:baz /> </x:foo></x:root>";
            _nav = GetXmlDocumentNavigator(xml);
            _document.PreserveWhitespace = true;

            GetNamespaceConsistentTree(_nav);
            _nav = GetXPathDocumentNavigator(_document, XmlSpace.Preserve);
            GetNamespaceConsistentTree(_nav);
        }

        private void GetNamespaceConsistentTree(XPathNavigator nav)
        {
            nav.MoveToFirstChild();
            nav.MoveToFirstChild();
            Assert.Equal("ns1", nav.GetNamespace(""));
            nav.MoveToNext();
            nav.MoveToNext();
            Assert.Equal("", nav.GetNamespace(""));
        }
    }
}

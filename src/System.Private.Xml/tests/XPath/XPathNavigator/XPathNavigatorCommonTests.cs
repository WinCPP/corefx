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
        XmlDocument document;
        XPathNavigator nav;
        XPathDocument xpathDocument;

        private XPathNavigator GetXmlDocumentNavigator(string xml)
        {
            document = new XmlDocument();
            document.LoadXml(xml);
            return document.CreateNavigator();
        }

        private XPathNavigator GetXPathDocumentNavigator(XmlNode node)
        {
            XmlNodeReader xr = new XmlNodeReader(node);
            xpathDocument = new XPathDocument(xr);
            return xpathDocument.CreateNavigator();
        }

        private XPathNavigator GetXPathDocumentNavigator(XmlNode node, XmlSpace space)
        {
            XmlNodeReader xr = new XmlNodeReader(node);
            xpathDocument = new XPathDocument(xr, space);
            return xpathDocument.CreateNavigator();
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

            nav = GetXmlDocumentNavigator(xml);
            DocumentWithXmlDeclaration(nav);
            nav = GetXPathDocumentNavigator(document);
            DocumentWithXmlDeclaration(nav);
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

            nav = GetXmlDocumentNavigator(xml);
            DocumentWithProcessingInstruction(nav);
            nav = GetXPathDocumentNavigator(document);
            DocumentWithProcessingInstruction(nav);
        }

        public void DocumentWithProcessingInstruction(XPathNavigator nav)
        {
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.ProcessingInstruction, "", "xml-stylesheet", "", "xml-stylesheet", "href='foo.xsl' type='text/xsl' ", false, false, false);
            Assert.True(!nav.MoveToFirstChild());
        }

        [Fact]
        public void XmlRootElementOnly()
        {
            string xml = "<foo />";

            nav = GetXmlDocumentNavigator(xml);
            XmlRootElementOnly(nav);
            nav = GetXPathDocumentNavigator(document);
            XmlRootElementOnly(nav);
        }

        private void XmlRootElementOnly(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, false, true);
            Assert.True(!nav.MoveToFirstChild());
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());
            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlSimpleTextContent()
        {
            string xml = "<foo>Test.</foo>";

            nav = GetXmlDocumentNavigator(xml);
            XmlSimpleTextContent(nav);
            nav = GetXPathDocumentNavigator(document);
            XmlSimpleTextContent(nav);
        }

        private void XmlSimpleTextContent(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "Test.", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "Test.", false, true, false);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());
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
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlSimpleElementContent()
        {
            string xml = "<foo><bar /></foo>";

            nav = GetXmlDocumentNavigator(xml);
            XmlSimpleElementContent(nav);
            nav = GetXPathDocumentNavigator(document);
            XmlSimpleElementContent(nav);
        }

        private void XmlSimpleElementContent(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlTwoElementsContent()
        {
            string xml = "<foo><bar /><baz /></foo>";

            nav = GetXmlDocumentNavigator(xml);
            XmlTwoElementsContent(nav);
            nav = GetXPathDocumentNavigator(document);
            XmlTwoElementsContent(nav);
        }

        private void XmlTwoElementsContent(XPathNavigator nav)
        {
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);
            Assert.True(!nav.MoveToFirstChild());

            Assert.True(nav.MoveToNext());
            AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
            Assert.True(!nav.MoveToFirstChild());

            Assert.True(nav.MoveToPrevious());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlElementWithAttributes()
        {
            string xml = "<img src='foo.png' alt='image Fooooooo!' />";

            nav = GetXmlDocumentNavigator(xml);
            XmlElementWithAttributes(nav);
            nav = GetXPathDocumentNavigator(document);
            XmlElementWithAttributes(nav);
        }

        private void XmlElementWithAttributes(XPathNavigator nav)
        {
            nav.MoveToFirstChild();
            AssertNavigator(nav, XPathNodeType.Element, "", "img", "", "img", "", true, false, true);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstAttribute());
            AssertNavigator(nav, XPathNodeType.Attribute, "", "src", "", "src", "foo.png", false, false, false);
            Assert.True(!nav.MoveToFirstAttribute()); // On attributes, it fails.

            Assert.True(nav.MoveToNextAttribute());
            AssertNavigator(nav, XPathNodeType.Attribute, "", "alt", "", "alt", "image Fooooooo!", false, false, false);
            Assert.True(!nav.MoveToNextAttribute());

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "img", "", "img", "", true, false, true);

            Assert.True(nav.MoveToAttribute("alt", ""));
            AssertNavigator(nav, XPathNodeType.Attribute, "", "alt", "", "alt", "image Fooooooo!", false, false, false);
            Assert.True(!nav.MoveToAttribute("src", "")); // On attributes, it fails.
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

            nav = GetXmlDocumentNavigator(xml);
            XmlNamespaceNode(nav);
            nav = GetXPathDocumentNavigator(document);
            XmlNamespaceNode(nav);
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
            Assert.True(!nav.MoveToNextNamespace(XPathNamespaceScope.Local));
            Assert.True(!nav.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));

            // see http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316808
            Assert.True(nav.MoveToNextNamespace(XPathNamespaceScope.All));
            AssertNavigator(nav, XPathNodeType.Namespace, "", "xml", "", "xml", xmlNS, false, false, false);
            Assert.True(!nav.MoveToNextNamespace(XPathNamespaceScope.All));

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
            Assert.True(!nav.MoveToFirstNamespace(XPathNamespaceScope.Local), "Local should fail");
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

            nav = GetXmlDocumentNavigator(xml);
            MoveToNamespaces(nav);
            nav = GetXPathDocumentNavigator(document);
            MoveToNamespaces(nav);
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

            nav = GetXmlDocumentNavigator(xml);
            IsDescendant(nav);
            nav = GetXPathDocumentNavigator(document);
            IsDescendant(nav);
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
            Assert.True(!nav.IsDescendant(tmp));
            tmp.MoveToFirstChild();
            Assert.Equal("a", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.True(!nav.IsDescendant(tmp));
            tmp.MoveTo(iter.Current);
            Assert.Equal("e", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.True(!nav.IsDescendant(tmp));
        }

        [Fact]
        public void LiterallySplittedText()
        {
            string xml = "<root><![CDATA[Fact]]> string</root>";

            nav = GetXmlDocumentNavigator(xml);
            LiterallySplittedText(nav);
            nav = GetXPathDocumentNavigator(document);
            LiterallySplittedText(nav);
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

            nav = GetXmlDocumentNavigator(xml);
            SelectChildrenNS(nav);
            nav = GetXPathDocumentNavigator(document);
            SelectChildrenNS(nav);
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

            nav = GetXmlDocumentNavigator(xml);
            OuterXml(nav);
            nav = GetXPathDocumentNavigator(document);
            OuterXml(nav);
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
            var xr = nav.ReadSubtree();
            xr.MoveToContent();
            xr.Read(); // should be at x:val
            Assert.Equal("urn:x", xr.LookupNamespace("x"));
        }

        [Fact]
        public void GetNamespaceConsistentTree()
        {
            string xml = "<x:root xmlns:x='urn:x'>  <x:foo xmlns='ns1'> <x:bar /> </x:foo>  <x:foo xmlns:y='ns2'> <x:baz /> </x:foo></x:root>";
            nav = GetXmlDocumentNavigator(xml);
            document.PreserveWhitespace = true;

            GetNamespaceConsistentTree(nav);
            nav = GetXPathDocumentNavigator(document, XmlSpace.Preserve);
            GetNamespaceConsistentTree(nav);
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

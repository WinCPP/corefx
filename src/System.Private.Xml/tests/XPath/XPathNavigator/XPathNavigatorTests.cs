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
    public class XPathNavigatorTests
    {
        [Fact]
        public void CreateNavigator()
        {
            var document = new XmlDocument();
            document.LoadXml("<foo />");
            var navigator = document.CreateNavigator();
            Assert.NotNull(navigator);
        }

        [Fact]
        public void PropertiesOnDocument()
        {
            var document = new XmlDocument();

            document.LoadXml("<foo:bar xmlns:foo='#foo' />");
            var navigator = document.CreateNavigator();

            Assert.Equal(XPathNodeType.Root, navigator.NodeType);
            Assert.Equal(String.Empty, navigator.Name);
            Assert.Equal(String.Empty, navigator.LocalName);
            Assert.Equal(String.Empty, navigator.NamespaceURI);
            Assert.Equal(String.Empty, navigator.Prefix);
            Assert.True(!navigator.HasAttributes);
            Assert.True(navigator.HasChildren);
            Assert.True(!navigator.IsEmptyElement);
        }

        [Fact]
        public void PropertiesOnElement()
        {
            var document = new XmlDocument();

            document.LoadXml("<foo:bar xmlns:foo='#foo' />");
            var navigator = document.DocumentElement.CreateNavigator();

            Assert.Equal(XPathNodeType.Element, navigator.NodeType);
            Assert.Equal("foo:bar", navigator.Name);
            Assert.Equal("bar", navigator.LocalName);
            Assert.Equal("#foo", navigator.NamespaceURI);
            Assert.Equal("foo", navigator.Prefix);
            Assert.True(!navigator.HasAttributes);
            Assert.True(!navigator.HasChildren);
            Assert.True(navigator.IsEmptyElement);
        }

        [Fact]
        public void PropertiesOnAttribute()
        {
            var document = new XmlDocument();
            document.LoadXml("<foo bar:baz='quux' xmlns:bar='#bar' />");

            XPathNavigator navigator = document.DocumentElement.GetAttributeNode("baz", "#bar").CreateNavigator();

            Assert.Equal(XPathNodeType.Attribute, navigator.NodeType);
            Assert.Equal("bar:baz", navigator.Name);
            Assert.Equal("baz", navigator.LocalName);
            Assert.Equal("#bar", navigator.NamespaceURI);
            Assert.Equal("bar", navigator.Prefix);
            Assert.True(!navigator.HasAttributes);
            Assert.True(!navigator.HasChildren);
            Assert.True(!navigator.IsEmptyElement);
        }

        [Fact]
        public void PropertiesOnNamespace()
        {
            var document = new XmlDocument();
            document.LoadXml("<root xmlns='urn:foo' />");

            var navigator = document.DocumentElement.Attributes[0].CreateNavigator();
            Assert.Equal(XPathNodeType.Namespace, navigator.NodeType);
        }

        [Fact]
        public void Navigation()
        {
            var document = new XmlDocument();
            document.LoadXml("<foo><bar /><baz /></foo>");

            var navigator = document.DocumentElement.CreateNavigator();

            Assert.Equal("foo", navigator.Name);
            Assert.True(navigator.MoveToFirstChild());
            Assert.Equal("bar", navigator.Name);
            Assert.True(navigator.MoveToNext());
            Assert.Equal("baz", navigator.Name);
            Assert.True(!navigator.MoveToNext());
            Assert.Equal("baz", navigator.Name);
            Assert.True(navigator.MoveToPrevious());
            Assert.Equal("bar", navigator.Name);
            Assert.True(!navigator.MoveToPrevious());
            Assert.True(navigator.MoveToParent());
            Assert.Equal("foo", navigator.Name);
            navigator.MoveToRoot();
            Assert.Equal(XPathNodeType.Root, navigator.NodeType);
            Assert.True(!navigator.MoveToParent());
            Assert.Equal(XPathNodeType.Root, navigator.NodeType);
            Assert.True(navigator.MoveToFirstChild());
            Assert.Equal("foo", navigator.Name);
            Assert.True(navigator.MoveToFirst());
            Assert.Equal("foo", navigator.Name);
            Assert.True(navigator.MoveToFirstChild());
            Assert.Equal("bar", navigator.Name);
            Assert.True(navigator.MoveToNext());
            Assert.Equal("baz", navigator.Name);
            Assert.True(navigator.MoveToFirst());
            Assert.Equal("bar", navigator.Name);
        }

        [Fact]
        public void MoveToAndIsSamePosition()
        {
            XmlDocument document1 = new XmlDocument();
            document1.LoadXml("<foo><bar /></foo>");
            XPathNavigator navigator1a = document1.DocumentElement.CreateNavigator();
            XPathNavigator navigator1b = document1.DocumentElement.CreateNavigator();

            XmlDocument document2 = new XmlDocument();
            document2.LoadXml("<foo><bar /></foo>");
            XPathNavigator navigator2 = document2.DocumentElement.CreateNavigator();

            Assert.Equal("foo", navigator1a.Name);
            Assert.True(navigator1a.MoveToFirstChild());
            Assert.Equal("bar", navigator1a.Name);

            Assert.True(!navigator1b.IsSamePosition(navigator1a));
            Assert.Equal("foo", navigator1b.Name);
            Assert.True(navigator1b.MoveTo(navigator1a));
            Assert.True(navigator1b.IsSamePosition(navigator1a));
            Assert.Equal("bar", navigator1b.Name);

            Assert.True(!navigator2.IsSamePosition(navigator1a));
            Assert.Equal("foo", navigator2.Name);
            Assert.True(!navigator2.MoveTo(navigator1a));
            Assert.Equal("foo", navigator2.Name);
        }

        [Fact]
        public void AttributeNavigation()
        {
            var document = new XmlDocument();
            document.LoadXml("<foo bar='baz' quux='quuux' />");

            XPathNavigator navigator = document.DocumentElement.CreateNavigator();

            Assert.Equal(XPathNodeType.Element, navigator.NodeType);
            Assert.Equal("foo", navigator.Name);
            Assert.True(navigator.MoveToFirstAttribute());
            Assert.Equal(XPathNodeType.Attribute, navigator.NodeType);
            Assert.Equal("bar", navigator.Name);
            Assert.Equal("baz", navigator.Value);
            Assert.True(navigator.MoveToNextAttribute());
            Assert.Equal(XPathNodeType.Attribute, navigator.NodeType);
            Assert.Equal("quux", navigator.Name);
            Assert.Equal("quuux", navigator.Value);
        }

        [Fact]
        public void ElementAndRootValues()
        {
            var document = new XmlDocument();
            document.LoadXml("<foo><bar>baz</bar><quux>quuux</quux></foo>");

            XPathNavigator navigator = document.DocumentElement.CreateNavigator();

            Assert.Equal(XPathNodeType.Element, navigator.NodeType);
            Assert.Equal("foo", navigator.Name);
            //Assert.Equal ("bazquuux", navigator.Value);

            navigator.MoveToRoot();
            //Assert.Equal ("bazquuux", navigator.Value);
        }

        [Fact]
        public void DocumentWithXmlDeclaration()
        {
            var document = new XmlDocument();
            document.LoadXml("<?xml version=\"1.0\" standalone=\"yes\"?><Root><foo>bar</foo></Root>");

            XPathNavigator navigator = document.CreateNavigator();

            navigator.MoveToRoot();
            navigator.MoveToFirstChild();
            Assert.Equal(XPathNodeType.Element, navigator.NodeType);
            Assert.Equal("Root", navigator.Name);
        }

        [Fact]
        public void DocumentWithProcessingInstruction()
        {
            var document = new XmlDocument();
            document.LoadXml("<?xml-stylesheet href='foo.xsl' type='text/xsl' ?><foo />");

            XPathNavigator navigator = document.CreateNavigator();

            Assert.True(navigator.MoveToFirstChild());
            Assert.Equal(XPathNodeType.ProcessingInstruction, navigator.NodeType);
            Assert.Equal("xml-stylesheet", navigator.Name);

            XPathNodeIterator iter = navigator.SelectChildren(XPathNodeType.Element);
            Assert.Equal(0, iter.Count);
        }

        [Fact]
        public void SelectFromOrphan()
        {
            // SelectSingleNode () from node without parent.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<foo><include id='original' /></foo>");

            XmlNode node = doc.CreateElement("child");
            node.InnerXml = "<include id='new' />";

            XmlNode new_include = node.SelectSingleNode("//include");
            Assert.Equal("<include id=\"new\" />", new_include.OuterXml);

            // In this case 'node2' has parent 'node'
            doc = new XmlDocument();
            doc.LoadXml("<foo><include id='original' /></foo>");

            node = doc.CreateElement("child");
            XmlNode node2 = doc.CreateElement("grandchild");
            node.AppendChild(node2);
            node2.InnerXml = "<include id='new' />";

            new_include = node2.SelectSingleNode("/");
            Assert.Equal("<child><grandchild><include id=\"new\" /></grandchild></child>",
                new_include.OuterXml);
        }

        [Fact]
        public void XPathDocumentMoveToId()
        {
            string dtd = "<!DOCTYPE root [<!ELEMENT root EMPTY><!ATTLIST root id ID #REQUIRED>]>";
            string xml = dtd + "<root id='aaa'/>";
            StringReader sr = new StringReader(xml);
            XPathNavigator nav = new XPathDocument(sr).CreateNavigator();
            Assert.True(nav.MoveToId("aaa"), "ctor() from TextReader");

            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.DTD
            };
            XmlReader xvr = XmlReader.Create(new StringReader(xml), settings);
            nav = new XPathDocument(xvr).CreateNavigator();
            Assert.True(nav.MoveToId("aaa"), "ctor() from XmlReader");

            // FIXME: it seems to result in different in .NET 2.0.
        }

        [Fact]
        public void SignificantWhitespaceConstruction()
        {
            string xml = @"<root>
        <child xml:space='preserve'>    <!-- -->   </child>
        <child xml:space='preserve'>    </child>
</root>";
            XPathNavigator nav = new XPathDocument(
                new XmlTextReader(xml, XmlNodeType.Document, null),
                XmlSpace.Preserve).CreateNavigator();
            nav.MoveToFirstChild();
            nav.MoveToFirstChild();
            Assert.Equal(XPathNodeType.Whitespace, nav.NodeType);
            nav.MoveToNext();
            nav.MoveToFirstChild();
            Assert.Equal(XPathNodeType.SignificantWhitespace,
                nav.NodeType);
        }

        [Fact]
        public void VariableReference()
        {
            XPathDocument xpd = new XPathDocument(
                new StringReader("<root>sample text</root>"));
            XPathNavigator nav = xpd.CreateNavigator();

            XPathExpression expr = nav.Compile("foo(string(.),$idx)");
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("idx", "", 5);
            MyContext ctx = new MyContext(nav.NameTable as NameTable, args);
            ctx.AddNamespace("x", "urn:foo");

            expr.SetContext(ctx);

            XPathNodeIterator iter = nav.Select("/root");
            iter.MoveNext();
            Assert.Equal("e", iter.Current.Evaluate(expr));
        }

        class MyContext : XsltContext
        {
            XsltArgumentList args;

            public MyContext(NameTable nt, XsltArgumentList args)
                : base(nt)
            {
                this.args = args;
            }

            public override IXsltContextFunction ResolveFunction(
                string prefix, string name, XPathResultType[] argtypes)
            {
                if (name == "foo")
                    return new MyFunction(argtypes);
                return null;
            }

            public override IXsltContextVariable ResolveVariable(string prefix, string name)
            {
                return new MyVariable(name);
            }

            public override bool PreserveWhitespace(XPathNavigator nav)
            {
                return false;
            }

            public override int CompareDocument(string uri1, string uri2)
            {
                return String.CompareOrdinal(uri1, uri2);
            }

            public override bool Whitespace
            {
                get { return false; }
            }

            public object GetParam(string name, string ns)
            {
                return args.GetParam(name, ns);
            }
        }

        public class MyFunction : IXsltContextFunction
        {
            XPathResultType[] argtypes;

            public MyFunction(XPathResultType[] argtypes)
            {
                this.argtypes = argtypes;
            }

            public XPathResultType[] ArgTypes
            {
                get { return argtypes; }
            }

            public int Maxargs
            {
                get { return 2; }
            }

            public int Minargs
            {
                get { return 2; }
            }

            public XPathResultType ReturnType
            {
                get { return XPathResultType.String; }
            }

            public object Invoke(XsltContext xsltContext,
                object[] args, XPathNavigator instanceContext)
            {
                return ((string)args[0])[(int)(double)args[1]].ToString();
            }
        }

        public class MyVariable : IXsltContextVariable
        {
            string name;

            public MyVariable(string name)
            {
                this.name = name;
            }

            public object Evaluate(XsltContext ctx)
            {
                return ((MyContext)ctx).GetParam(name, String.Empty);
            }

            public bool IsLocal
            {
                get { return false; }
            }

            public bool IsParam
            {
                get { return false; }
            }

            public XPathResultType VariableType
            {
                get { return XPathResultType.Any; }
            }
        }

        [Fact]
        public void TextMatchesWhitespace()
        {
            string xml = "<root><ws>   </ws><sws xml:space='preserve'> </sws></root>";
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild(); // root
            nav.MoveToFirstChild(); // ws
            nav.MoveToFirstChild(); // '   '
            Assert.Equal(true, nav.Matches("text()"));
            nav.MoveToParent();
            nav.MoveToNext(); // sws
            nav.MoveToFirstChild(); // ' '
            Assert.Equal(true, nav.Matches("text()"));
        }

        [Fact]
        public void Bug456103()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><X/></root>");

            XPathNavigator nav = doc.DocumentElement.CreateNavigator();
            // ".//*" does not reproduce the bug.
            var i = nav.Select("descendant::*");

            // without this call to get_Count() the bug does not reproduce.
            Assert.Equal(1, i.Count);

            Assert.True(i.MoveNext());
        }

        [Fact]
        public void ValueAsBoolean()
        {
            string xml = "<root>1</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Equal(true, nav.ValueAsBoolean);
            nav.MoveToFirstChild();
            Assert.Equal(true, nav.ValueAsBoolean);
        }

        [Fact]
        public void ValueAsBooleanFail()
        {
            string xml = "<root>1.0</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Throws<FormatException>(() => nav.ValueAsBoolean);
        }

        [Fact]
        public void ValueAsDateTime()
        {
            DateTime time = new DateTime(2005, 12, 13);
            string xml = "<root>2005-12-13</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Equal(time, nav.ValueAsDateTime);
            nav.MoveToFirstChild();
            Assert.Equal(time, nav.ValueAsDateTime);
        }

        [Fact]
        public void ValueAsDateTimeFail()
        {
            string xml = "<root>dating time</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Throws<FormatException>(() => nav.ValueAsDateTime);
        }

        [Fact]
        public void ValueAsDouble()
        {
            string xml = "<root>3.14159265359</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Equal(3.14159265359, nav.ValueAsDouble);
            nav.MoveToFirstChild();
            Assert.Equal(3.14159265359, nav.ValueAsDouble);
        }

        [Fact]
        public void ValueAsDoubleFail()
        {
            string xml = "<root>Double Dealer</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Throws<FormatException>(() => nav.ValueAsDouble);
        }

        [Fact]
        public void ValueAsInt()
        {
            string xml = "<root>1</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Equal(1, nav.ValueAsInt);
            nav.MoveToFirstChild();
            Assert.Equal(1, nav.ValueAsInt);
        }

        [Fact]
        // Here, it seems to be using XQueryConvert (whatever was called)
        public void ValueAsIntFail()
        {
            string xml = "<root>1.0</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Throws<FormatException>(() => nav.ValueAsInt);
        }

        [Fact]
        public void ValueAsLong()
        {
            string xml = "<root>10000000000000000</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Equal(10000000000000000, nav.ValueAsLong);
            nav.MoveToFirstChild();
            Assert.Equal(10000000000000000, nav.ValueAsLong);
        }

        [Fact]
        // Here, it seems to be using XQueryConvert (whatever was called)
        public void ValueAsLongFail()
        {
            string xml = "<root>0x10000000000000000</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToFirstChild();
            Assert.Throws<FormatException>(() => nav.ValueAsLong);
        }

        [Fact]
        public void InnerXmlText()
        {
            StringReader sr = new StringReader("<Abc><Foo>Hello</Foo></Abc>");
            XPathDocument doc = new XPathDocument(sr);
            XPathNavigator nav = doc.CreateNavigator();
            XPathNodeIterator iter = nav.Select("/Abc/Foo");
            iter.MoveNext();
            Assert.Equal("Hello", iter.Current.InnerXml);
            Assert.Equal("<Foo>Hello</Foo>", iter.Current.OuterXml);
            iter = nav.Select("/Abc/Foo/text()");
            iter.MoveNext();
            Assert.Equal(String.Empty, iter.Current.InnerXml);
            Assert.Equal("Hello", iter.Current.OuterXml);
        }

        [Fact]
        public void InnerXmlAttribute()
        {
            StringReader sr = new StringReader("<Abc><Foo attr='val1'/></Abc>");
            XPathDocument doc = new XPathDocument(sr);
            XPathNavigator nav = doc.CreateNavigator();

            XPathNodeIterator iter = nav.Select("/Abc/Foo/@attr");
            iter.MoveNext();
            Assert.Equal("val1", iter.Current.InnerXml);
        }

        string AlterNewLine(string s)
        {
            return s.Replace("\r\n", Environment.NewLine);
        }

        [Fact]
        public void InnerXmlTextEscape()
        {
            StringReader sr = new StringReader("<Abc><Foo>Hello&lt;\r\nInnerXml</Foo></Abc>");
            XPathDocument doc = new XPathDocument(sr);
            XPathNavigator nav = doc.CreateNavigator();
            XPathNodeIterator iter = nav.Select("/Abc/Foo");
            iter.MoveNext();
            Assert.Equal(AlterNewLine("Hello&lt;\r\nInnerXml"), iter.Current.InnerXml);
            Assert.Equal(AlterNewLine("<Foo>Hello&lt;\r\nInnerXml</Foo>"), iter.Current.OuterXml);
            iter = nav.Select("/Abc/Foo/text()");
            iter.MoveNext();
            Assert.Equal(String.Empty, iter.Current.InnerXml);
            Assert.Equal(AlterNewLine("Hello&lt;\r\nInnerXml"), iter.Current.OuterXml);
        }

        [Fact]
        public void InnerXmlAttributeEscape()
        {
            StringReader sr = new StringReader("<Abc><Foo attr='val&quot;1&#13;&#10;&gt;'/></Abc>");
            XPathDocument doc = new XPathDocument(sr);
            XPathNavigator nav = doc.CreateNavigator();

            XPathNodeIterator iter = nav.Select("/Abc/Foo/@attr");
            iter.MoveNext();
            Assert.Equal("val\"1\r\n>", iter.Current.InnerXml);
        }

        [Fact]
        public void WriterAttributePrefix()
        {
            XmlDocument doc = new XmlDocument();
            XmlWriter w = doc.CreateNavigator().AppendChild();
            w.WriteStartElement("foo");
            w.WriteAttributeString("xmlns", "x", "http://www.w3.org/2000/xmlns/", "urn:foo");
            Assert.Equal("x", w.LookupPrefix("urn:foo"));
            w.WriteStartElement(null, "bar", "urn:foo");
            w.WriteAttributeString(null, "ext", "urn:foo", "bah");
            w.WriteEndElement();
            w.WriteEndElement();
            w.Close();
            Assert.Equal("x", doc.FirstChild.FirstChild.Prefix);
            Assert.Equal("x", doc.FirstChild.FirstChild.Attributes[0].Prefix);
        }

        [Fact]
        public void ValueAs()
        {
            string xml = "<root>1</root>";
            XPathNavigator nav = new XPathDocument(XmlReader.Create(new StringReader(xml))).CreateNavigator();
            nav.MoveToFirstChild();
            nav.MoveToFirstChild();
            Assert.Equal("1", nav.ValueAs(typeof(string), null));
            Assert.Equal(1, nav.ValueAs(typeof(int), null));
        }

        [Fact]
        public void MoveToFollowingNodeTypeAll()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><child/><child2/></root>");
            XPathNavigator nav = doc.CreateNavigator();
            Assert.True(nav.MoveToFollowing(XPathNodeType.All));
            Assert.True(nav.MoveToFollowing(XPathNodeType.All));
            Assert.Equal("child", nav.LocalName);
            Assert.True(nav.MoveToNext(XPathNodeType.All));
            Assert.Equal("child2", nav.LocalName);
        }

        [Fact]
        public void XPathDocumentFromSubtreeNodes()
        {
            string xml = "<root><child1><nest1><nest2>hello!</nest2></nest1></child1><child2/><child3/></root>";
            XmlReader r = new XmlTextReader(new StringReader(xml));
            while (r.Read())
            {
                if (r.Name == "child1")
                    break;
            }
            XPathDocument d = new XPathDocument(r);
            XPathNavigator n = d.CreateNavigator();
            string result = @"<child1>
  <nest1>
    <nest2>hello!</nest2>
  </nest1>
</child1>
<child2 />
<child3 />";
            Assert.Equal(result.Replace("\r\n", "\n"), n.OuterXml.Replace("\r\n", "\n"));
        }

        [Fact]
        public void InnerXmlOnRoot()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(@"<test>
			<node>z</node>
			<node>a</node>
			<node>b</node>
			<node>q</node>
			</test>");
            XPathNavigator navigator = document.CreateNavigator();
            Assert.Equal(navigator.OuterXml, navigator.InnerXml);
        }

        [Fact]
        public void SelectChildrenEmpty()
        {
            string s = "<root> <foo> </foo> </root>";
            XPathDocument doc = new XPathDocument(new StringReader(s));
            XPathNavigator nav = doc.CreateNavigator();
            XPathNodeIterator it = nav.SelectChildren(String.Empty, String.Empty);
            foreach (XPathNavigator xpn in it)
            {
                Assert.Empty(xpn.Value);
            }
        }
    }
}

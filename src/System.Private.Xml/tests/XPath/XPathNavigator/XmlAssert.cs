// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Xml.XPath;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class XmlAssert
    {
        // copy from XmlTextReaderTests
        public static void AssertStartDocument(XmlReader xmlReader,
            string label)
        {
            Assert.Equal(ReadState.Initial, xmlReader.ReadState);
            Assert.Equal(XmlNodeType.None, xmlReader.NodeType);
            Assert.Equal(0, xmlReader.Depth);
            Assert.False(xmlReader.EOF);
        }

        public static void AssertNode(
            string label,
            XmlReader xmlReader,
            XmlNodeType nodeType,
            int depth,
            bool isEmptyElement,
            string name,
            string prefix,
            string localName,
            string namespaceURI,
            string value,
            bool hasValue,
            int attributeCount,
            bool hasAttributes)
        {
            Assert.Equal(nodeType, xmlReader.NodeType);

            Assert.Equal(isEmptyElement, xmlReader.IsEmptyElement);

            Assert.Equal(name, xmlReader.Name);

            Assert.Equal(prefix, xmlReader.Prefix);

            Assert.Equal(localName, xmlReader.LocalName);

            Assert.Equal(namespaceURI, xmlReader.NamespaceURI);

            Assert.Equal(depth, xmlReader.Depth);

            Assert.Equal(hasValue, xmlReader.HasValue);

            Assert.Equal(value, xmlReader.Value);

            Assert.Equal(hasAttributes, xmlReader.HasAttributes);

            Assert.Equal(attributeCount, xmlReader.AttributeCount);
        }

        public static void AssertAttribute(
            string label,
            XmlReader xmlReader,
            XmlNodeType nodeType,
            int depth,
            bool isEmptyElement,
            string name,
            string prefix,
            string localName,
            string namespaceURI,
            string value)
        {
            Assert.Equal(nodeType, xmlReader.NodeType);

            Assert.Equal(depth, xmlReader.Depth);

            Assert.Equal(isEmptyElement, xmlReader.IsEmptyElement);

            Assert.Equal(value, xmlReader[name]);

            Assert.Equal(value, xmlReader.GetAttribute(name));

            if (namespaceURI != string.Empty)
            {
                Assert.Equal(value, xmlReader[localName, namespaceURI]);
                Assert.Equal(value, xmlReader.GetAttribute(localName, namespaceURI));
            }
        }

        public static void AssertEndDocument(XmlReader xmlReader, string label)
        {
            Assert.False(!xmlReader.Read());
            Assert.Equal(XmlNodeType.None, xmlReader.NodeType);
            Assert.Equal(0, xmlReader.Depth);
            Assert.Equal(ReadState.EndOfFile, xmlReader.ReadState);
            Assert.True(xmlReader.EOF);

            xmlReader.Close();
            Assert.Equal(ReadState.Closed, xmlReader.ReadState);
        }
    }
}

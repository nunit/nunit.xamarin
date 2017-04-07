// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.Data.Xml.Dom;
using Windows.Data.Xml.Xsl;
using Windows.Storage;
#else
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
#endif
using NUnit.Runner.Helpers;
using NUnit.Runner.Messages;
using Xamarin.Forms;

namespace NUnit.Runner.Services
{
    internal class XmlFileTransformer : TestResultProcessor
    {
        public XmlFileTransformer(TestOptions options)
            : base(options) { }

        public override async Task Process(ResultSummary testResult)
        {
            if (!string.IsNullOrEmpty(Options.XmlTransformFile))
            {
                try
                {
#if WINDOWS_UWP
                    var resultFile = await StorageFile.GetFileFromPathAsync(Options.ResultFilePath);
                    var doc = await XmlDocument.LoadFromFileAsync(resultFile);

                    var xslt = await StorageFile.GetFileFromPathAsync(Options.XmlTransformFile);
                    var xsltDoc = await XmlDocument.LoadFromFileAsync(xslt);

                    var processor = new XsltProcessor(xsltDoc);
                    var transformed = processor.TransformToDocument(doc.FirstChild);
                    await transformed.SaveToFileAsync(resultFile);
#else
                    var xpathDocument = new XPathDocument(Options.ResultFilePath);
                    var transform = new XslCompiledTransform();
                    transform.Load(Options.XmlTransformFile);
                    var writer = new XmlTextWriter(Options.ResultFilePath, null);
                    transform.Transform(xpathDocument, null, writer);
#endif
                }
                catch (Exception ex)
                {
                    var message = $"Fatal error while trying to transform xml result: {ex.Message}";
                    MessagingCenter.Send(new ErrorMessage(message), ErrorMessage.Name);
                }
            }

            if (Successor != null)
            {
                await Successor.Process(testResult);
            }
        }
    }
}

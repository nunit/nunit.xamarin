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
using System.IO;
using System.Threading.Tasks;
using NUnit.Runner.Helpers;

namespace NUnit.Runner.Services
{
    class XmlFileProcessor : TestResultProcessor
    {
        public XmlFileProcessor(TestOptions options)
            : base(options) { }

        public override async Task Process(ResultSummary result)
        {
            if (Options.CreateXmlResultFile == false)
                return;

            try
            {
                WriteXmlResultFile(result);
            }
            catch (Exception ex)
            {
                RealConsole.WriteLine("Fatal error while trying to write xml result file! " + ex.Message);
                throw;
            }

            if (Successor != null)
            {
                await Successor.Process(result).ConfigureAwait(false);
            }
        }

        void WriteXmlResultFile(ResultSummary result)
        {
            var outputFolderName = Path.GetDirectoryName(Options.ResultFilePath);

            Directory.CreateDirectory(outputFolderName);
            var xml = result.GetTestXml().ToString();
            File.WriteAllText(Options.ResultFilePath, xml);
        }
    }
}
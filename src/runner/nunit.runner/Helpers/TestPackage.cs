// ***********************************************************************
// Copyright (c) 2017 Charlie Poole
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Runner.Helpers
{
    /// <summary>
    /// Contains all assemblies for a test run, and controls execution of tests and collection of results
    /// </summary>
    internal class TestPackage
    {
        private readonly NUnitTestAssemblyRunner _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

        public int TestsCount => _runner.IsTestLoaded ? _runner.CountTestCases(TestFilter.Empty) : 0;

        public IEnumerable<ITest> LoadedTests
        {
            get
            {
                var tests = _runner.LoadedTest.Tests;

                while (tests?.Count == 1)
                {
                    tests = tests.Single().Tests;
                }

                return tests;
            }
        }

        public void AddAssembly(Assembly testAssembly)
        {
            _runner.Load(testAssembly, new Dictionary<string, object>());
        }

        public async Task<TestRunResult> ExecuteTests(IEnumerable<ITest> tests = null, bool force = false)
        {
            var resultPackage = new TestRunResult();

            var filter = new CustomTestFilter(tests, force);
            var result = await Task.Run(() => _runner.Run(TestListener.NULL, filter)).ConfigureAwait(false);
            resultPackage.AddResult(result);
            resultPackage.CompleteTestRun();
            return resultPackage;
        }

        private class CustomTestFilter : TestFilter
        {
            private readonly HashSet<string> _testNames;
            private readonly bool _force;

            public CustomTestFilter(IEnumerable<ITest> tests, bool force)
            {
                if (tests != null)
                {
                    var names = tests.Flatten()
                                     .Select(t => t.FullName);

                    _testNames = new HashSet<string>(names);
                }

                _force = force;
            }

            public override TNode AddToXml(TNode parentNode, bool recursive)
            {
                return parentNode.AddElement("filter");
            }

            public override bool Match(ITest test)
            {
                // We don't want to run explicit tests
                if (!_force)
                {
                    var parent = test.Parent;
                    while (parent != null)
                    {
                        if (parent.RunState != RunState.Runnable)
                        {
                            return false;
                        }

                        parent = parent.Parent;
                    }
                }

                // If filter was created with null tests collection, we assume we want to run all tests
                return _testNames?.Contains(test.FullName) ?? true;
            }
        }
    }
}

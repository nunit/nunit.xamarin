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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NUnit.Framework.Interfaces;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class EnumerableExtensions
{
    internal static IEnumerable<ITest> Flatten(this IEnumerable<ITest> tests)
    {
        foreach (var test in tests)
        {
            if (test.Tests.Any())
            {
                foreach (var child in test.Tests.Flatten())
                {
                    yield return child;
                }
            }
            else
            {
                yield return test;
            }
        }
    }

    internal static IEnumerable<ITestResult> Flatten(this IEnumerable<ITestResult> results)
    {
        foreach (var result in results)
        {
            if (result.Children.Any())
            {
                foreach (var child in result.Children.Flatten())
                {
                    yield return child;
                }
            }

            yield return result;
        }
    }
}

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

using NUnit.Framework.Interfaces;
using System.Linq;

using XFColor = Xamarin.Forms.Color;

namespace NUnit.Runner.Extensions
{
    internal static class XamarinExtensions
    {
        /// <summary>
        /// Gets the color to display for the test status
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static XFColor Color(this ResultState result)
        {
            switch (result.Status)
            {
                case TestStatus.Passed:
                    return XFColor.Green;
                case TestStatus.Skipped:
                    return XFColor.FromRgb(206, 172, 0);    // Dark Yellow
                case TestStatus.Failed:
                    if (result == ResultState.Failure)
                        return XFColor.Red;
                    if (result == ResultState.NotRunnable)
                        return XFColor.FromRgb(255, 106, 0);  // Orange

                    return XFColor.FromRgb(170, 0, 0); // Dark Red
                default:
                    return XFColor.Gray;
            }
        }

        public static ResultState OverallResultState(this ITestResult result)
        {
            if (result.ResultState.Status == TestStatus.Skipped &&
                result.Test.IsSuite &&
                result.Children.Flatten().Any(c => c.ResultState.Status == TestStatus.Passed))
            {
                return new ResultState(TestStatus.Passed);
            }

            return result.ResultState;
        }
    }
}
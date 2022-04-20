// ***********************************************************************
// Copyright (c) 2015 NUnit Project
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
using NUnit.Runner.Extensions;
using NUnit.Runner.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Runner.ViewModel
{
    class TestDescriptionViewModel : BaseViewModel
    {
        public TestDescriptionViewModel(ITest test, string assembly)
        {
            _test = test;
            Name = StringOrUnknown(_test.Name);
            MethodName = StringOrUnknown(_test.MethodName);
            ClassName = StringOrUnknown(_test.ClassName);
            FullName = StringOrUnknown(_test.FullName);
            Assembly = assembly;
            Categories = _test.PropertyValues("Category").ToArray();
        }

        private readonly ITest _test;
        public string Name { get; }
        public string MethodName { get; }
        public string ClassName { get; }
        public string FullName { get; }
        public string Assembly { get; }
        public string[] Categories { get; }

        private bool _selected;
        
        /// <summary>
        /// True if test is selected
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value.Equals(_selected)) return;
                _selected = value;
                OnPropertyChanged();
            }
        }

        private string StringOrUnknown(string str) => string.IsNullOrWhiteSpace(str) ? "<unknown>" : str;        
    }
}

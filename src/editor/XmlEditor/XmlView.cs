﻿// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using System.ComponentModel;
using System.Windows.Forms;
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor
{
    public partial class XmlView : UserControl, IXmlView
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public XmlView()
        {
            InitializeComponent();

            Xml = new TextElement(richTextBox1);
        }

        #endregion

        #region IXmlView Members


        /// <summary>
        /// Gets or sets the XML text
        /// </summary>
        public ITextElement Xml { get; private set; }

        public void DisplayError(string message)
        {
            errorMessageLabel.Visible = true;
            errorMessageLabel.Text = message;

            richTextBox1.Dock = DockStyle.Top;
            richTextBox1.Height = this.ClientSize.Height - errorMessageLabel.Height;
        }

        public void DisplayError(string message, int lineNumber, int linePosition)
        {
            DisplayError(message);

            int offset = richTextBox1.GetFirstCharIndexFromLine(lineNumber) + linePosition - 1;
            richTextBox1.Select(offset, 3); // TODO: Determine true length of area to highlight
        }

        public void RemoveError()
        {
            errorMessageLabel.Visible = false;
            richTextBox1.Dock = DockStyle.Fill;
        }

        public IMessageDisplay MessageDisplay { get; private set; }

        #endregion

        #region Event Handlers

        //private void richTextBox1_Validated(object sender, EventArgs e)
        //{
        //    if (XmlChanged != null)
        //        XmlChanged();
        //}

        #endregion
    }
}

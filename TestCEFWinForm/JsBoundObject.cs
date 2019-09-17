using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace TestCEFWinForm
{
    public class AsyncJavascriptBindingClass
    {
        private Form1 _form;
        public Form1 form { get => _form; set => _form = value; }
        public AsyncJavascriptBindingClass(Form1 inputForm)
        {
            _form = inputForm;
        }        
        public void Close()
        {
            
            Application.Exit();
        }
        public void Maximize()
        {
            //if (System.Windows.Forms.Form.ActiveForm.InvokeRequired)
            //{
            //    System.Windows.Forms.Form.ActiveForm.Invoke(new Action(() =>
            //    {
            //        System.Windows.Forms.Form.ActiveForm.WindowState = FormWindowState.Maximized;
            //    }));
            //}
            _form.BeginInvoke(new Action(() =>
            {
                _form.Maximize();
            }));
        }
        public void Minimize()
        {   
            _form.Invoke(new Action(() =>
            {
                _form.Minimize();
            }));
        }
    }

}

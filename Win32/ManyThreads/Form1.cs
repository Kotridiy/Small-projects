using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManyThreads
{
    public partial class Form1 : Form
    {
        object book = new object();

        public Form1()
        {
            InitializeComponent();
            ParameterizedThreadStart start = new ParameterizedThreadStart(EnterLib);
            for (int i = 1; i <= 10; i++)
            {
                Thread thread = new Thread(start);
                thread.Priority = ThreadPriority.Lowest;
                thread.Start(i);
            }
        }

        void EnterLib(object obj)
        {
            while (Form1.ActiveForm == null) { }
            int num = (int)obj;
            textBox1.Invoke((MethodInvoker)(() => textBox1.Text = string.Format("Reader{0} enter lib", num)));
            Thread.Sleep(300 + num * 1000);
            lock (book)
            {
                textBox1.Invoke((MethodInvoker)(() => textBox1.Text = string.Format("Reader{0} read book", num)));
                Thread.Sleep(1000 + num * 200);
            }
            textBox1.Invoke((MethodInvoker)(() => textBox1.Text = string.Format("Reader{0} leave lib", num)));
            Thread.Sleep(300 + num * 100);
            return;
        }
    }
}

using System;
using System.Diagnostics;
using System.Windows.Forms;
using AutoIt;
using Win32;
using System.Threading;

namespace PressF
{
    public partial class Form1 : Form
    {
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_SETFOCUS = 0x0007;
        const int VK_A = 0x41;
        const int VK_D = 0x44;
        const int VK_S = 0x53;
        const int VK_W = 0x57;
        const int VK_F = 0x46;
        const int VK_RIGHT = 0x27;
        const int VK_Space = 0x20;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcesses();

            foreach (var proc in processes)
            {
                if (proc.ProcessName == "PressMe" || proc.ProcessName == "isaac-ng")
                    //continue;
                listBox1.Items.Add(proc.ProcessName);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process process = Process.GetProcessesByName(listBox1.SelectedItem.ToString())[0];
            //Process process = Process.GetProcessesByName("PressMe")[0];
            User32.SetForegroundWindow(process.MainWindowHandle);
            //User32.PostMessage(process.MainWindowHandle, WM_SETFOCUS, 0, 0);
            User32.PostMessage(process.MainWindowHandle, WM_KEYDOWN, VK_Space, 0);
            Thread.Sleep(100);
            User32.PostMessage(process.MainWindowHandle, WM_KEYUP, VK_Space, 0);
            Thread.Sleep(500);
            User32.PostMessage(process.MainWindowHandle, WM_KEYDOWN, VK_A, 0);
            Thread.Sleep(500);
            User32.PostMessage(process.MainWindowHandle, WM_KEYUP, VK_A, 0);
            User32.PostMessage(process.MainWindowHandle, WM_KEYDOWN, VK_D, 0);
            Thread.Sleep(500);
            User32.PostMessage(process.MainWindowHandle, WM_KEYUP, VK_D, 0);
            User32.PostMessage(process.MainWindowHandle, WM_KEYDOWN, VK_S, 0);
            Thread.Sleep(500);
            User32.PostMessage(process.MainWindowHandle, WM_KEYUP, VK_S, 0);
            User32.PostMessage(process.MainWindowHandle, WM_KEYDOWN, VK_W, 0);
            Thread.Sleep(500);
            User32.PostMessage(process.MainWindowHandle, WM_KEYUP, VK_W, 0);
        }
    }
}

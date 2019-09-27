#region License_Do_Not_Remove
/* 
*  Made by TheDarkJoker94. 
*  Check http://thedarkjoker94.cer33.com/ for more C# Tutorials 
*  and also SUBSCRIBE to my Youtube Channel http://www.youtube.com/user/TheDarkJoker094  
*  GlobalKeyboardHook is licensed under a Creative Commons Attribution 3.0 Unported License.(http://creativecommons.org/licenses/by/3.0/)
*  This means you can use this Code for whatever you want as long as you credit me! That means...
*  DO NOT REMOVE THE LINES ABOVE !!!
*  
*  Supplemented by Kotridiy
*/
#endregion
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Win32;

public class GlobalKeyboardHook
{
    [DllImport("user32.dll")]
    static extern int CallNextHookEx(IntPtr hhk, int code, int wParam, ref keyBoardHookStruct lParam);
    [DllImport("user32.dll")]
    static extern int CallNextHookEx(IntPtr hhk, int code, int wParam, ref mouseHookStruct lParam);
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowsHookEx(int idHook, LLKeyboardHook callback, IntPtr hInstance, uint theardID);
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowsHookEx(int idHook, LLMouseHook callback, IntPtr hInstance, uint theardID);
    
    public delegate int LLKeyboardHook(int Code, int wParam, ref keyBoardHookStruct lParam);
    public delegate int LLMouseHook(int Code, int wParam, ref mouseHookStruct lParam);

    public struct keyBoardHookStruct
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
    public struct mouseHookStruct
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    const int WH_KEYBOARD_LL = 13;
    const int WM_KEYDOWN = 0x0100;
    const int WM_KEYUP = 0x0101;
    const int WM_SYSKEYDOWN = 0x0104;
    const int WM_SYSKEYUP = 0x0105;

    const int WH_MOUSE_LL = 14;
    const int WM_MOUSEMOVE = 0x200;
    const int WM_LBUTTONDOWN = 0x201;
    const int WM_RBUTTONDOWN = 0x204;
    const int WM_MBUTTONDOWN = 0x207;
    const int WM_LBUTTONUP = 0x202;
    const int WM_RBUTTONUP = 0x205;
    const int WM_MBUTTONUP = 0x208;
    const int WM_LBUTTONDBLCLK = 0x203;
    const int WM_RBUTTONDBLCLK = 0x206;
    const int WM_MBUTTONDBLCLK = 0x209;
    const int WM_MOUSEWHEEL = 0x020A;

    LLKeyboardHook llkh;
    LLMouseHook llmh;
    public List<Keys> HookedKeys = new List<Keys>();

    IntPtr Hook = IntPtr.Zero;
    IntPtr MouseHook = IntPtr.Zero;

    public event KeyEventHandler KeyDown;
    public event KeyEventHandler KeyUp;
    public event MouseEventHandler MouseDown;
    public event MouseEventHandler MouseUp;
    public event MouseEventHandler MouseMove;
    public event MouseEventHandler MouseWheel;
    public event MouseEventHandler MouseDouble;


    public GlobalKeyboardHook()
    {
        llkh = new LLKeyboardHook(HookProc);
        llmh = new LLMouseHook(mouseHookProc);
    }

    ~GlobalKeyboardHook()
    { unhook(); }

    public void hook()
    {
        IntPtr hInstance = (IntPtr)Kernel32.LoadLibrary("User32");
        Hook = SetWindowsHookEx(WH_KEYBOARD_LL, llkh, hInstance, 0);
        MouseHook = SetWindowsHookEx(WH_MOUSE_LL, llmh, hInstance, 0);
    }

    public void unhook()
    {
        User32.UnhookWindowsHookEx(Hook);
        User32.UnhookWindowsHookEx(MouseHook);
    }

    public int HookProc(int Code, int wParam, ref keyBoardHookStruct lParam)
    {
        if (Code >= 0)
        {
            Keys key = (Keys)lParam.vkCode;
            if (HookedKeys.Contains(key))
            {
                KeyEventArgs kArg = new KeyEventArgs(key);
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    KeyDown(this, kArg);
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    KeyUp(this, kArg);
                if (kArg.Handled)
                    return 1;
            }
        }
        return CallNextHookEx(Hook, Code, wParam, ref lParam);
    }

    public int mouseHookProc(int Code, int wParam, ref mouseHookStruct lParam)
    {
        if (Code >= 0)
        {
            switch (wParam)
            {
                case WM_MBUTTONDOWN:
                    {
                        if (MouseDown != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Middle, 1, lParam.pt.x, lParam.pt.y, 0);
                            MouseDown(this, mArg);
                        }
                        break;
                    }
                case WM_LBUTTONDOWN:
                    {
                        if (MouseDown != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Left, 1, lParam.pt.x, lParam.pt.y, 0);
                            MouseDown(this, mArg);
                        }
                        break;
                    }
                case WM_RBUTTONDOWN:
                    {
                        if (MouseDown != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Right, 1, lParam.pt.x, lParam.pt.y, 0);
                            MouseDown(this, mArg);
                        }
                        break;
                    }
                case WM_MBUTTONUP:
                    {
                        if (MouseUp != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Middle, 1, lParam.pt.x, lParam.pt.y, 0);
                            MouseUp(this, mArg);
                        }
                        break;
                    }
                case WM_LBUTTONUP:
                    {
                        if (MouseUp != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Left, 1, lParam.pt.x, lParam.pt.y, 0);
                            MouseUp(this, mArg);
                        }
                        break;
                    }
                case WM_RBUTTONUP:
                    {
                        if (MouseUp != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Right, 1, lParam.pt.x, lParam.pt.y, 0);
                            MouseUp(this, mArg);
                        }
                        break;
                    }
                case WM_MOUSEMOVE:
                    {
                        if (MouseMove != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.None, 0, lParam.pt.x, lParam.pt.y, 0);
                            MouseMove(this, mArg);
                        }
                        break;
                    }
                case WM_MOUSEWHEEL:
                    {
                        if (MouseWheel != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.None, 0, lParam.pt.x, lParam.pt.y, lParam.mouseData / 0xFFFF);
                            MouseWheel(this, mArg);
                        }
                        break;
                    }
                case WM_MBUTTONDBLCLK:
                    {
                        if (MouseDown != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Middle, 2, lParam.pt.x, lParam.pt.y, 0);
                            MouseDouble(this, mArg);
                        }
                        break;
                    }
                case WM_LBUTTONDBLCLK:
                    {
                        if (MouseDown != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Left, 2, lParam.pt.x, lParam.pt.y, 0);
                            MouseDouble(this, mArg);
                        }
                        break;
                    }
                case WM_RBUTTONDBLCLK:
                    {
                        if (MouseDown != null)
                        {
                            MouseEventArgs mArg = new MouseEventArgs(MouseButtons.Right, 2, lParam.pt.x, lParam.pt.y, 0);
                            MouseDouble(this, mArg);
                        }
                        break;
                    }
            }

        }
        return CallNextHookEx(Hook, Code, wParam, ref lParam);
    }
}
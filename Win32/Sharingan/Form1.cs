using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using Win32;

namespace Sharingan
{
    public partial class Form1 : Form
    {
        const int WM_MOUSEWHEEL = 0x020A;
        const int MOUSEEVENTF_WHEEL = 0x800;

        public enum Event
        {
            KeyDown = User32.WM_KEYDOWN,
            KeyUp = User32.WM_KEYUP,
            LMouseDown = User32.WM_LBUTTONDOWN,
            RMouseDown = User32.WM_RBUTTONDOWN,
            MMouseDown = User32.WM_MBUTTONDOWN,
            LMouseUp = User32.WM_LBUTTONUP,
            RMouseUp = User32.WM_RBUTTONUP,
            MMouseUp = User32.WM_MBUTTONUP,
            MouseMove = User32.WM_MOUSEMOVE,
            MouseWheel = WM_MOUSEWHEEL
        }

        GlobalKeyboardHook gHook;
        Queue record = new Queue();
        DateTime lastTime = new DateTime();
        DateTime startTime = new DateTime();
        PressInfo lastPress = new PressInfo();
        Thread thread;
        State state = State.Wait;
        Keys recordKey = Keys.F2;
        Keys playKey = Keys.F3;

        delegate void Updater();

        struct PressInfo
        {
            public Keys key;
            public POINT point;
            public int wheelCount;
            public Event eventType;
        }

        enum State
        {
            Wait,
            Recording,
            Playing
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelRecord.Text = "";
            gHook = new GlobalKeyboardHook(); 
            gHook.KeyDown += gHook_WaitCommand;
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                gHook.HookedKeys.Add(key);

            gHook.hook();
        }

        public void gHook_WaitCommand(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == recordKey)
            {
                if (state == State.Wait)
                {
                    state = State.Recording;
                    labelStatus.Text = "Запись";
                    LabelUpdate();
                    lastTime = DateTime.Now;
                    startTime = DateTime.Now;
                    record.Clear();
                    gHook.KeyDown += gHook_KeyDown;
                    gHook.KeyUp += gHook_KeyUp;
                    gHook.MouseDown += gHook_MouseDown;
                    gHook.MouseUp += gHook_MouseUp;
                    gHook.MouseWheel += gHook_MouseWheel;
                    if (checkBoxMouse.Checked)
                        gHook.MouseMove += gHook_MouseMove;
                    listBox1.Items.Clear();
                    return;
                }
                if (state == State.Recording)
                {
                    state = State.Wait;
                    labelStatus.Text = "Ожидание";
                    LabelUpdate();
                    labelRecord.Text = string.Format("Время записи: {0} сек\nКоличество действий: {1}",
                        Math.Round((DateTime.Now - startTime).TotalSeconds), record.Count / 4);
                    gHook.KeyDown -= gHook_KeyDown;
                    gHook.KeyUp -= gHook_KeyUp;
                    gHook.MouseDown -= gHook_MouseDown;
                    gHook.MouseUp -= gHook_MouseUp;
                    gHook.MouseWheel -= gHook_MouseWheel;
                    gHook.MouseMove -= gHook_MouseMove;
                    return;
                }
            }
            if (e.KeyCode == playKey)
            {
                if (state == State.Wait && record.Count != 0)
                {
                    state = State.Playing;
                    labelStatus.Text = "Воспроизведение";
                    LabelUpdate();
                    ThreadStart threadStart = new ThreadStart(RecordPlay);
                    thread = new Thread(threadStart);
                    thread.Start();
                    return;
                }
                if (state == State.Playing)
                {
                    state = State.Wait;
                    labelStatus.Text = "Ожидание";
                    LabelUpdate();
                    thread.Abort();
                    return;
                }
            }
        }

        public void gHook_KeyDown(object sender, KeyEventArgs e)
        {
            PressInfo press = new PressInfo() { eventType = Event.KeyDown, key = e.KeyCode };
            if (checkBoxText.Checked || !press.Equals(lastPress))
            {
                if (!press.Equals(lastPress))
                    listBox1.Items.Add(e.KeyCode);
                    //listBox1.Items.Add(string.Format("{0} down, {1}", e.KeyCode, Math.Round(time)));
                double time = (DateTime.Now - lastTime).TotalMilliseconds;
                lastTime = DateTime.Now;
                record.Enqueue(time);
                record.Enqueue(press);
                lastPress = press;
            }
        }

        public void gHook_KeyUp(object sender, KeyEventArgs e)
        {
            PressInfo press = new PressInfo() { eventType = Event.KeyUp, key = e.KeyCode };
            double time = (DateTime.Now - lastTime).TotalMilliseconds;
            lastTime = DateTime.Now;
            record.Enqueue(time);
            record.Enqueue(press);
            lastPress = press;
            //listBox1.Items.Add(string.Format("{0} up, {1}", e.KeyCode, Math.Round(time))); 
        }

        public void gHook_MouseDown(object sender, MouseEventArgs e)
        {
            PressInfo press = new PressInfo() { point = new POINT() };
            press.point.x = e.X;
            press.point.y = e.Y;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        press.eventType = Event.LMouseDown;
                        break;
                    }
                case MouseButtons.Right:
                    {
                        press.eventType = Event.RMouseDown;
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        press.eventType = Event.MMouseDown;
                        break;
                    }
            }
            double time = (DateTime.Now - lastTime).TotalMilliseconds;
            lastTime = DateTime.Now;
            record.Enqueue(time);
            record.Enqueue(press);
            listBox1.Items.Add(string.Format("{2} button ({0}; {1})", e.X, e.Y, e.Button));
        }

        public void gHook_MouseUp(object sender, MouseEventArgs e)
        {
            PressInfo press = new PressInfo() { point = new POINT() };
            press.point.x = e.X;
            press.point.y = e.Y;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        press.eventType = Event.LMouseUp;
                        break;
                    }
                case MouseButtons.Right:
                    {
                        press.eventType = Event.RMouseUp;
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        press.eventType = Event.MMouseUp;
                        break;
                    }
            }
            double time = (DateTime.Now - lastTime).TotalMilliseconds;
            lastTime = DateTime.Now;
            record.Enqueue(time);
            record.Enqueue(press);
        }

        public void gHook_MouseWheel(object sender, MouseEventArgs e)
        {
            PressInfo press = new PressInfo() { point = new POINT(), eventType = Event.MouseWheel };
            press.point.x = e.X;
            press.point.y = e.Y;
            press.wheelCount = e.Delta;
            double time = (DateTime.Now - lastTime).TotalMilliseconds;
            lastTime = DateTime.Now;
            record.Enqueue(time);
            record.Enqueue(press);
            listBox1.Items.Add(string.Format("Wheel {0}", (e.Delta > 0) ? "up" : "down"));
        }

        public void gHook_MouseMove(object sender, MouseEventArgs e)
        {
            PressInfo press = new PressInfo() { point = new POINT(), eventType = Event.MouseWheel };
            press.point.x = e.X;
            press.point.y = e.Y;
            double time = (DateTime.Now - lastTime).TotalMilliseconds;
            lastTime = DateTime.Now;
            record.Enqueue(time);
            record.Enqueue(press);
        }

        public void RecordPlay()
        {
            Queue recordTape = new Queue(record);
            recordTape.Dequeue();
            recordTape.Dequeue();
            while (recordTape.Count > 2)
            {
                int time = (int)((double)recordTape.Dequeue());
                Thread.Sleep(time);
                PressInfo press = (PressInfo)recordTape.Dequeue();
                switch (press.eventType)
                {
                    case Event.KeyDown:
                        {
                            IntPtr ptr = (IntPtr)User32.GetForegroundWindow();
                            User32.PostMessage(ptr, (int)Event.KeyDown, (int)press.key, 0);
                            break;
                        }
                    case Event.KeyUp:
                        {
                            if (checkBoxText.Checked) break;
                            IntPtr ptr = (IntPtr)User32.GetForegroundWindow();
                            User32.PostMessage(ptr, (int)Event.KeyUp, (int)press.key, 0);
                            break;
                        }
                    case Event.LMouseDown:
                        {
                            Cursor.Position = new System.Drawing.Point(press.point.x, press.point.y);
                            User32.mouse_event(User32.MOUSEEVENTF_LEFTDOWN, press.point.x, press.point.y, 0, 0);
                            break;
                        }
                    case Event.RMouseDown:
                        {
                            Cursor.Position = new System.Drawing.Point(press.point.x, press.point.y);
                            User32.mouse_event(User32.MOUSEEVENTF_RIGHTDOWN, press.point.x, press.point.y, 0, 0);
                            break;
                        }
                    case Event.MMouseDown:
                        {
                            Cursor.Position = new System.Drawing.Point(press.point.x, press.point.y);
                            User32.mouse_event(User32.MOUSEEVENTF_MIDDLEDOWN, press.point.x, press.point.y, 0, 0);
                            break;
                        }
                    case Event.LMouseUp:
                        {
                            Cursor.Position = new System.Drawing.Point(press.point.x, press.point.y);
                            User32.mouse_event(User32.MOUSEEVENTF_LEFTUP, press.point.x, press.point.y, 0, 0);
                            break;
                        }
                    case Event.RMouseUp:
                        {
                            Cursor.Position = new System.Drawing.Point(press.point.x, press.point.y);
                            User32.mouse_event(User32.MOUSEEVENTF_RIGHTUP, press.point.x, press.point.y, 0, 0);
                            break;
                        }
                    case Event.MMouseUp:
                        {
                            Cursor.Position = new System.Drawing.Point(press.point.x, press.point.y);
                            User32.mouse_event(User32.MOUSEEVENTF_MIDDLEUP, press.point.x, press.point.y, 0, 0);
                            break;
                        }
                    case Event.MouseWheel:
                        {
                            Cursor.Position = new System.Drawing.Point(press.point.x, press.point.y);
                            User32.mouse_event(MOUSEEVENTF_WHEEL, press.point.x, press.point.y, press.wheelCount, 0);
                            break;
                        }
                    case Event.MouseMove:
                        {
                            User32.mouse_event(User32.MOUSEEVENTF_MOVE, press.point.x, press.point.y, press.wheelCount, 0);
                            break;
                        }
                }
            }
            state = State.Wait;
            labelStatus.Invoke((MethodInvoker)(() => labelStatus.Text = "Ожидание"));
            Invoke((Updater)LabelUpdate);
            return;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            gHook.unhook();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2(recordKey, playKey);
            this.Enabled = false;
            form.FormClosed += Form2_Closed;
            form.SettingsSet += Form2_SettingsSet;
            form.Show();
        }

        private void Form2_SettingsSet(Keys first, Keys second)
        {
            recordKey = first;
            playKey = second;
            LabelUpdate();
        }

        public void Form2_Closed(object sender, EventArgs e)
        {
            this.Enabled = true;
        }

        void LabelUpdate()
        {
            switch (state)
            {
                case State.Wait:
                    {
                        if (record.Count == 0)
                            labelHelp.Text = "Нажмите " + recordKey.ToString() + ", чтобы начать новую запись.";
                        else
                            labelHelp.Text = "Нажмите " + recordKey.ToString() + ", чтобы начать новую запись.\n" +
                                "Нажмите " + playKey.ToString() + ", чтобы воспроизвести запись.";
                        break;
                    }
                case State.Recording:
                    {
                        labelHelp.Text = "Нажмите " + recordKey.ToString() + ", чтобы закончить запись.";
                        break;
                    }
                case State.Playing:
                    {
                        labelHelp.Text = "Нажмите " + playKey.ToString() + ", чтобы прервать воспроизведение.";
                        break;
                    }
            }
        }
    }
}
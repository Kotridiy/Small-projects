using System;
using System.Windows.Forms;

namespace Sharingan
{
    public partial class Form2 : Form
    {
        Keys first;
        Keys second;
        public delegate void SettingDelegate(Keys first, Keys second);
        public event SettingDelegate SettingsSet;

        public Form2(Keys first, Keys second)
        {
            this.first = first;
            this.second = second;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            foreach (var key in Enum.GetValues(typeof(Keys)))
            {
                comboBox1.Items.Add(key);
                comboBox2.Items.Add(key);
            }
            comboBox1.SelectedItem = first;
            comboBox2.SelectedItem = second;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                comboBox1.SelectedItem = first;
            }
            if (comboBox2.SelectedItem == null)
            {
                comboBox2.SelectedItem = second;
            }
            SettingsSet((Keys)comboBox1.SelectedItem, (Keys)comboBox2.SelectedItem);
            this.Close();
        }
    }
}
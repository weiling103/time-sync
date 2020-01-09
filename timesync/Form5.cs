﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
namespace timesync {
    public partial class Form5 : Form {
        private Form1 parentForm;
        public Form5 (Form1 parentForm) {
            this.parentForm = parentForm;
            InitializeComponent ();
            renderView ();
        }
        private void openAutoStart (object sender = null, System.Timers.ElapsedEventArgs e = null) {
            string appPath = Application.ExecutablePath;
            string appName = System.IO.Path.GetFileName (appPath);
            try {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey (@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue (appName, appPath + " -s");
                rk2.Close ();
                rk.Close ();
            } catch { }
        }
        private bool checkAutorunStatus () {
            string appPath = Application.ExecutablePath;
            string appName = System.IO.Path.GetFileName (appPath);
            Object obj = Registry.GetValue (@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", appName, null);
            if (obj != null) {
                return true;
            } else {
                return false;
            }
        }
        private void closeAutoStart () {
            string appPath = Application.ExecutablePath;
            string appName = System.IO.Path.GetFileName (appPath);
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey (@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.DeleteValue (appName, false);
            rk2.Close ();
            rk.Close ();
        }
        private string INIReader () {
            string stmp = Assembly.GetExecutingAssembly ().Location;
            stmp = stmp.Substring (0, stmp.LastIndexOf ('\\'));
            INIClass ini = new INIClass (stmp + @"\config.ini");
            string isConfirm = ini.IniReadValue ("EXIT", "confirm", "1");
            return isConfirm;
        }
        private string INIReader2 () {
            string stmp = Assembly.GetExecutingAssembly ().Location;
            stmp = stmp.Substring (0, stmp.LastIndexOf ('\\'));
            INIClass ini = new INIClass (stmp + @"\config.ini");
            string isConfirm = ini.IniReadValue ("AUTOSYNC", "startsync", "0");
            return isConfirm;
        }

        private void readSyncConfig()
        {
            string stmp = Assembly.GetExecutingAssembly().Location;
            stmp = stmp.Substring(0, stmp.LastIndexOf('\\'));
            INIClass ini = new INIClass(stmp + @"\config.ini");
            Boolean isAutoSync = ini.IniReadValue("AUTOSYNC", "enable", "1") == "1" ? true : false;
            decimal interval = int.Parse(ini.IniReadValue("AUTOSYNC", "interval", "5"));
            checkBox4.Checked = isAutoSync;
            decimal max = numericUpDown1.Maximum;
            decimal min = numericUpDown1.Minimum;
            numericUpDown1.Enabled = isAutoSync;
            if (interval >= min && interval <= max)
            {
                numericUpDown1.Value = interval;
            }
        }
        private void renderView () {
            this.checkBox1.Checked = checkAutorunStatus();
            this.checkBox2.Checked = INIReader() == "1" ? true : false;
            this.checkBox3.Checked = INIReader2() == "1" ? true : false;
            readSyncConfig();
        }
        private void button1_Click (object sender, EventArgs e) {
            bool isSaved = true;
            if (checkBox1.Checked) {
                if (!checkAutorunStatus ()) {
                    openAutoStart ();
                    if (!checkAutorunStatus ()) {
                        isSaved = false;
                    }
                }
            } else {
                if (checkAutorunStatus ()) {
                    closeAutoStart ();
                    if (checkAutorunStatus ()) {
                        isSaved = false;
                    }
                }
            }
            string stmp = Assembly.GetExecutingAssembly ().Location;
            stmp = stmp.Substring (0, stmp.LastIndexOf ('\\'));
            INIClass ini = new INIClass (stmp + @"\config.ini");
            bool confirm = checkBox2.Checked;
            bool confirmStartSync = checkBox3.Checked;
            if (confirm) {
                ini.IniWriteValue ("EXIT", "confirm", "1");
            } else {
                ini.IniWriteValue ("EXIT", "confirm", "0");
            }
            if (confirmStartSync) {
                ini.IniWriteValue ("AUTOSYNC", "startsync", "1");
            } else {
                ini.IniWriteValue ("AUTOSYNC", "startsync", "0");
            }
            saveSyncConfig();
            saveIntervalConfig();
            parentForm.runTaskTimer(numericUpDown1.Value, checkBox4.Checked);
            if (!isSaved) {
                renderView ();
                MessageBox.Show ("部分设置保存失败！", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                MessageBox.Show ("设置保存成功！", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close ();
                this.Dispose ();
            }
        }
        private void saveSyncConfig()
        {
            string stmp = Assembly.GetExecutingAssembly().Location;
            stmp = stmp.Substring(0, stmp.LastIndexOf('\\'));
            INIClass ini = new INIClass(stmp + @"\config.ini");
            bool confirm = checkBox4.Checked;
            if (confirm)
            {
                ini.IniWriteValue("AUTOSYNC", "enable", "1");
            }
            else
            {
                ini.IniWriteValue("AUTOSYNC", "enable", "0");
            }
        }
        private void saveIntervalConfig()
        {
            string stmp = Assembly.GetExecutingAssembly().Location;
            stmp = stmp.Substring(0, stmp.LastIndexOf('\\'));
            INIClass ini = new INIClass(stmp + @"\config.ini");
            string interval = numericUpDown1.Value.ToString();
            ini.IniWriteValue("AUTOSYNC", "interval", interval);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox1.Checked;
        }
        private void pictureBox2_Click (object sender, EventArgs e) {
            this.Close ();
            this.Dispose ();
        }
        private void checkBox1_Click (object sender, EventArgs e) {
            if (checkBox1.Checked == false) {
                MessageBox.Show ("取消开机启动将不能在开机时同步时间！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private bool isEnterMenuPanel = false;
        private bool isMouseDown = false;
        private int MouseX, MouseY, LocationX, LocationY;
        private void panel3_MouseEnter (object sender, EventArgs e) {
            isEnterMenuPanel = true;
        }
        private void panel3_MouseLeave (object sender, EventArgs e) {
            isEnterMenuPanel = false;
        }
        private void panel3_MouseDown (object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isMouseDown = true;
                Point MousePos = Control.MousePosition;
                MouseX = MousePos.X;
                MouseY = MousePos.Y;
                LocationX = this.Location.X;
                LocationY = this.Location.Y;
            }
        }
        private void panel3_MouseUp (object sender, MouseEventArgs e) {
            isMouseDown = false;
        }
        private void panel3_MouseMove (object sender, MouseEventArgs e) {
            if (isMouseDown && isEnterMenuPanel) {
                Point MousePos = Control.MousePosition;
                this.Location = new Point (LocationX + MousePos.X - MouseX, LocationY + MousePos.Y - MouseY);
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace Clock
{
	public partial class MainForm : Form
	{
		ChooseFontForm fontDialog = null;
		AlarmsForm alarms = null;
		public MainForm()
		{
			InitializeComponent();
			labelTime.BackColor = Color.Black;
			labelTime.ForeColor = Color.Red;

			this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, 50);
			SetVisibility(false);

			cmShowConsole.Checked = true;
			LoadSettings();
			//fontDialog = new ChooseFontForm();								
			alarms = new AlarmsForm();
		}
		void SetVisibility(bool visible)
		{
			cbShowDate.Visible = visible;
			cbShowWeekDay.Visible = visible;
			btnHideControls.Visible = visible;
			this.TransparencyKey = visible ? Color.Empty : this.BackColor;
			this.FormBorderStyle = visible ? FormBorderStyle.FixedToolWindow : FormBorderStyle.None;
			this.ShowInTaskbar = visible;
		}
		void SaveSettings()
		{
			StreamWriter sw = new StreamWriter("Settings.ini");
			sw.WriteLine($"{cmTopmost.Checked}");
			sw.WriteLine($"{cmShowControls.Checked}");
			sw.WriteLine($"{cmShowDate.Checked}");
			sw.WriteLine($"{cmShowWeekDay.Checked}");
			sw.WriteLine($"{cmShowConsole.Checked}");
			sw.WriteLine($"{labelTime.BackColor.ToArgb()}");
			sw.WriteLine($"{labelTime.ForeColor.ToArgb()}");
			sw.WriteLine($"{fontDialog.Filename}");
			sw.WriteLine($"{labelTime.Font.Size}");
			sw.Close();
			Process.Start("notepad", "Settings.ini");

		}
		void LoadSettings()
		{
			string execution_path = Path.GetDirectoryName(Application.ExecutablePath);
			Directory.SetCurrentDirectory($"{execution_path}\\..\\..\\Fonts");
			StreamReader sr = new StreamReader("Settings.ini");
			cmTopmost.Checked= bool.Parse( sr.ReadLine());
			cmShowControls.Checked= bool.Parse( sr.ReadLine());
			cmShowDate.Checked= bool.Parse( sr.ReadLine());
			cmShowWeekDay.Checked= bool.Parse( sr.ReadLine());
			cmShowConsole.Checked= bool.Parse( sr.ReadLine());
			labelTime.BackColor= Color.FromArgb(Convert.ToInt32(sr.ReadLine()));
			labelTime.ForeColor= Color.FromArgb(Convert.ToInt32(sr.ReadLine()));
			string font_name = sr.ReadLine();
			int font_size = (int)Convert.ToDouble(sr.ReadLine());
			sr.Close();
			fontDialog = new ChooseFontForm(this, font_name, font_size); 
			labelTime.Font = fontDialog.Font;
		}
		private void timer_Tick(object sender, EventArgs e)
		{

			labelTime.Text = DateTime.Now.ToString
				(
				"hh:mm:ss tt",
				System.Globalization.CultureInfo.InvariantCulture
				);
			if (cbShowDate.Checked)
			{
				labelTime.Text += "\n";
				labelTime.Text += DateTime.Now.ToString("yyyy.MM.dd");
			}
			if (cbShowWeekDay.Checked)
			{
				labelTime.Text += "\n";
				labelTime.Text += DateTime.Now.DayOfWeek;
			}
			notifyIcon.Text = labelTime.Text;
		}

			private void btnHideControls_Click(object sender, EventArgs e)
		{
			//cbShowDate.Visible = false;
			//btnHideControls.Visible = false;
			//this.TransparencyKey = this.BackColor;
			//this.FormBorderStyle = FormBorderStyle.None;
			//labelTime.BackColor = Color.AliceBlue;
			//this.ShowInTaskbar = false;

			SetVisibility(cmShowControls.Checked = false);
		}

		private void labelTime_DoubleClick(object sender, EventArgs e)
		{

			//MessageBox.Show
			//(
			//this,
			//"Вы два раза щелкнули по времени и теперь вы управляете временем",
			//"info",
			//MessageBoxButtons.OK,
			//MessageBoxIcon.Information
			//);
			//cbShowDate.Visible = true;
			//btnHideControls.Visible = true;
			//this.TransparencyKey = Color.Empty;
			//this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			//labelTime.BackColor = Color.AliceBlue;
			//this.ShowInTaskbar = true;
			SetVisibility(cmShowControls.Checked = true);
		}

	

		
		private void cmExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cmTopmost_CheckedChanged(object sender, EventArgs e)
		{
			this.TopMost=cmTopmost.Checked;
		}

		private void cmShowDate_CheckedChanged(object sender, EventArgs e)
		{
			cbShowDate.Checked=cmShowDate.Checked;
		}

		private void cbShowDate_CheckedChanged(object sender, EventArgs e)
		{
			cmShowDate.Checked=cbShowDate.Checked;
		}

		private void cbShowWeekDay_CheckedChanged(object sender, EventArgs e)
		{
			cmShowWeekDay.Checked=cbShowWeekDay.Checked;
		}

		private void cmShowweekday_CheckedChanged(object sender, EventArgs e)
		{
			cbShowWeekDay.Checked=cmShowWeekDay.Checked;
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			if (!this.TopMost)
			{
				this.TopMost = true;
				this.TopMost = false;
			}
		}

		private void cmShowControls_CheckedChanged(object sender, EventArgs e)
		{
			SetVisibility(cmShowControls.Checked);
		}

		private void SetColor (object sender, EventArgs e)
		{
			ColorDialog dialog = new ColorDialog();
			//dialog.Color=labelTime.BackColor;
				switch((sender as ToolStripMenuItem).Text) // as - это оператор преобразования типа
				{										   // оператор 'as' значение слева приводит к типу справа   
					case "Background color": dialog.Color = labelTime.BackColor; break;
					case "Foreground color": dialog.Color = labelTime.ForeColor; break;
				}
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				switch((sender as ToolStripMenuItem).Text)
				{
					case "Background color":labelTime.BackColor=dialog.Color; break;
					case "Foreground color":labelTime.ForeColor=dialog.Color; break;
				}
			}
		}

		private void cmChooseFont_Click(object sender, EventArgs e)
		{
			if(fontDialog.ShowDialog()==DialogResult.OK)
				labelTime.Font= fontDialog.Font;
		}

		private void cmShowConsole_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as ToolStripMenuItem).Checked)
				AllocConsole();
			else
				FreeConsole();
		}

		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
		}

		private void cmLoadOnWinStartup_CheckedChanged(object sender, EventArgs e)
		{
			string key_name = "ClockPV_319";
			RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//true откроет ветку на запись
			if (cmLoadOnWinStartup.Checked) rk.SetValue(key_name, Application.ExecutablePath);
			else rk.DeleteValue(key_name, false);
			rk.Dispose();
		}

		private void cmAlarm_Click(object sender, EventArgs e)
		{
			alarms.StartPosition = FormStartPosition.Manual;
			alarms.Location = new Point(this.Location.X - alarms.Width, this.Location.Y*2);
			alarms.ShowDialog();
		}
	}
}

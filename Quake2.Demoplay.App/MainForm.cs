using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace Quake2.Demoplay.App
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		DemoPlayer _demoplay;
		bool _schanged;
        readonly string _playDemo;
        ParsedDemosBindingList _demosParsed;
        Thread _loadDemosThread = null;

        // for cleaning demos list from a different thread
        private delegate void ClearDemos();
        private ClearDemos clearDemos;

        // for adding demos to the datagridview binding source from a different thread
        private delegate void AddDemo(ParsedDemo demo);
        private AddDemo addDemo;

        // for sizing datagridview columns from a different thread
        private delegate void AdjustColumnWidth();
        private AdjustColumnWidth adjustColumnWidth;
		
		public MainForm()
		{
			InitializeComponent();
			System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
			_playDemo = null;
		}
		
		public MainForm(string file)
		{
			InitializeComponent();
			System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
			_playDemo = file;
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			this.Text = "Demoplay by neveride (ver. " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0,3)+")";
			_demoplay = new DemoPlayer();
			
			textBox1.Text = _demoplay.Q2exe;
			textBox2.Text = _demoplay.FFButton;
			textBox3.Text = _demoplay.PauseButton;
			textBox4.Text = _demoplay.SlowmoButton;
			checkBox1.Checked = _demoplay.closeafter;
			_schanged = false;
						
			if (_playDemo != null)
			{
				if (_demoplay.PlayDemo(_playDemo))
					this.Dispose();
			}

            //_demosParsed = new BindingSource();
            _demosParsed = new ParsedDemosBindingList();
            dataGridView1.DataSource = _demosParsed;
            clearDemos = new ClearDemos(_demosParsed.Clear);
            addDemo = new AddDemo(_demosParsed.Add);
            adjustColumnWidth = new AdjustColumnWidth(dataGridView1.AutoResizeColumns);
		}
		
		void Panel1Paint(object sender, PaintEventArgs e)
		{
            var sformat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            e.Graphics.DrawString("Drop demo file here...", new Font(new FontFamily("Microsoft Sans Serif"), 8.25f), Brushes.Black, (RectangleF)e.ClipRectangle, sformat);
		}
		
		void Panel1DragEnter(object sender, DragEventArgs e)
		{
			    if (e.Data.GetDataPresent(DataFormats.FileDrop))
			    {
			        e.Effect = DragDropEffects.Copy;
			    }
			    else
			        e.Effect = DragDropEffects.None;

		}
		
		void Panel1DragDrop(object sender, DragEventArgs e)
		{
			string [] fileList = (string []) e.Data.GetData(DataFormats.FileDrop, false);
			string file = fileList[0];
			if (!file.EndsWith("dm2"))
			{
				MessageBox.Show("File type not supported!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (_demoplay.PlayDemo(file))
			{
				MainFormFormClosing(this, new FormClosingEventArgs(CloseReason.None, false));
				this.Dispose();
			}
		}
		
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			_demoplay.closeafter = checkBox1.Checked;
			_schanged = true;
		}
		
		void TextBox1TextChanged(object sender, EventArgs e)
		{
			_demoplay.Q2exe = textBox1.Text;
			_schanged = true;
		}
				
		void TextBox2TextChanged(object sender, EventArgs e)
		{
			_demoplay.FFButton = textBox2.Text;
			_schanged = true;
		}
		
		void TextBox3TextChanged(object sender, EventArgs e)
		{
			_demoplay.PauseButton = textBox3.Text;
			_schanged = true;
		}
				
		void TextBox4TextChanged(object sender, EventArgs e)
		{
			_demoplay.SlowmoButton = textBox4.Text;
			_schanged = true;
		}
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (_schanged)
			{
				DialogResult result = MessageBox.Show("The settings have changed. Do you want to save them?", "Settings changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				
				switch (result)
				{
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.Yes:
						_demoplay.SaveSettings();
						break;
					case DialogResult.No:
						break;
				}
			}

            if (_loadDemosThread != null && _loadDemosThread.ThreadState == ThreadState.Running)
                _loadDemosThread.Abort();
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				textBox1.Text = openFileDialog1.FileName;
			}
		}

        #region Tab Parse

        private void ButtonDemosDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxDemosDir.Text = folderBrowserDialog1.SelectedPath;
                //ParseDemos(folderBrowserDialog1.SelectedPath);

                _loadDemosThread = new Thread(new ParameterizedThreadStart(ParseDemos));
                //t.IsBackground = true;
                _loadDemosThread.Start(folderBrowserDialog1.SelectedPath);                            
            }
        }

        private void ParseDemos(object selectedPath)
        {
            dataGridView1.Invoke(clearDemos);

            DirectoryInfo di = new DirectoryInfo((string)selectedPath);

            foreach (FileInfo fi in di.GetFiles("*.dm2", SearchOption.TopDirectoryOnly))
            {
                ParsedDemo demo = new ParsedDemo(fi);
                dataGridView1.Invoke(addDemo, demo);
            }

            dataGridView1.Invoke(adjustColumnWidth);
        }

        #endregion

        private void DataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            ParsedDemo demo = dataGridView1.Rows[e.RowIndex].DataBoundItem as ParsedDemo;
            _demoplay.PlayDemo(demo.GetFullPath());
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            return;
        }

        private void DataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < dataGridView1.Rows.Count; ++i)
                dataGridView1.Rows[i].ContextMenuStrip = rowClickMenuStrip;
        }

        private void ContextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ParsedDemo demo = dataGridView1.SelectedRows[0].DataBoundItem as ParsedDemo;

            switch (e.ClickedItem.Name)
            {
                case "toolStripMenuItemPlay":
                    _demoplay.PlayDemo(demo.GetFullPath());
                    break;
            }
        }
    }
}

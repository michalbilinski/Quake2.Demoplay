/*
 * Created by SharpDevelop.
 * User: michal
 * Date: 2010-04-06
 * Time: 09:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Quake2.Demoplay.App
{
	public class DemoPlayer
	{
		//ConfigReader ConfigReader;
		
		public DemoPlayer()
		{
			//reader = new ConfigReader();
		}
		
		public bool PlayDemo(string file)
		{
			if (!File.Exists(ConfigReader.Q2exe))
			{
				MessageBox.Show("Cannot find '"+ConfigReader.Q2exe + "'. Provide a path to a Quake 2 executable file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);	
				return false;
            }
			
			CreateExecFile();
            try
            {
                File.Copy(file, ConfigReader.demosdir + "\\tempdemo.dm2", true);
            }
            catch (IOException exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
			System.IO.Directory.SetCurrentDirectory(ConfigReader.Q2dir);
			Process q2 = new Process();
            q2.StartInfo.FileName = ConfigReader.Q2exe;

            q2.StartInfo.Arguments = $" +exec demoplay.txt +{ConfigReader.DemoCommand} tempdemo.dm2";
			q2.Start();
			if (ConfigReader.closeafter)
			{
				return true; // Close application
			}
			
			return false;
		}

		public void CreateExecFile()
		{
			// create a writer and open the file
	        TextWriter tw = new StreamWriter(ConfigReader.baseq2dir + "\\demoplay.txt");
			// write a line of text to the file
		
			tw.WriteLine("alias +ff \"timescale 100\"");
			tw.WriteLine("alias -ff \"timescale 1\"");
			tw.WriteLine("bind "+ConfigReader.FFButton+" +ff");
			tw.WriteLine("bind "+ConfigReader.PauseButton+" \"pause\"");
			tw.WriteLine("alias +slowmo \"timescale 0.1\"");
			tw.WriteLine("alias -slowmo \"timescale 1\"");
			tw.WriteLine("bind "+ConfigReader.SlowmoButton+" +slowmo");
					
			// close the stream
			tw.Close();
		}
		
		public void SaveSettings()
		{
			ConfigReader.ConfigSave();
		}
		
		public bool closeafter
		{
			get
			{
				return ConfigReader.closeafter;
			}
			set
			{
				ConfigReader.closeafter = value;
			}
		}
		
		public string Q2exe
		{
			get
			{
				return ConfigReader.Q2exe;
			}
			set
			{
				ConfigReader.Q2exe = value;
			}
		}
		
		public string PauseButton
		{
			get
			{
				return ConfigReader.PauseButton;
			}
			set
			{
				ConfigReader.PauseButton = value;
			}
		}
		
		public string FFButton
		{
			get
			{
				return ConfigReader.FFButton;
			}
			set
			{
				ConfigReader.FFButton = value;
			}
		}
		
		public string SlowmoButton
		{
			get
			{
				return ConfigReader.SlowmoButton;
			}
			set
			{
				ConfigReader.SlowmoButton = value;
			}
		}
	}
}

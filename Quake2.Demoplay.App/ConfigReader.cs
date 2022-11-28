/*
 * Created by SharpDevelop.
 * User: michal
 * Date: 2010-04-06
 * Time: 09:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Quake2.Demoplay.App
{
	/// <summary>
	/// Description of ConfigReader.
	/// </summary>
	public static class ConfigReader
	{
		static string configfile;
		static Dictionary<string, string> settings;
		
		static ConfigReader()
		{
			configfile = "demoplay.ini";
			settings = ReadFromFile();
		}
		
		public static void ConfigSave()
		{
			System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath));
	    	// create a writer and open the file
	        TextWriter tw = new StreamWriter(configfile);
			// write a line of text to the file
			if (!settings.ContainsKey("q2exe") || settings["q2exe"].Length == 0)
			{
				settings["q2exe"] = "quake2.exe";
			}
			tw.WriteLine("q2exe="+settings["q2exe"]);
			
			if (!settings.ContainsKey("closeafter"))
			{
				settings["closeafter"] = "false";
			}
			tw.WriteLine("closeafter="+settings["closeafter"]);
			tw.WriteLine("pause="+PauseButton);
			tw.WriteLine("ff="+FFButton);
			tw.WriteLine("slowmo="+SlowmoButton);
			
			// close the stream
			tw.Close();
		}

        static Dictionary<string, string> ReadFromFile()
		{
			Dictionary<string, string> psettings = new Dictionary<string, string>();
			
			try
			{
				using (StreamReader sr = new StreamReader(configfile, System.Text.Encoding.Default))
				{
					string wiersz;
					int nrwiersza = 0;
					
					while ((wiersz = sr.ReadLine()) != null)
					{
						if (wiersz.Length > 0 && wiersz.Substring(0,1) != "#")
						{
							Match m = Regex.Match(wiersz, @"\s*(.+)\s*=\s*(.+)$");
							
							if (!m.Success)
								throw new Exception("(Line "+nrwiersza+"): "+wiersz);
							
							if (m.Success)
							{
								psettings.Add(m.Groups[1].Value, m.Groups[2].Value);
								System.Diagnostics.Trace.WriteLine("ADD: "+m.Groups[1].Value+" "+m.Groups[2].Value);
							}
						}
						
						++nrwiersza;
					}
				}
			}
			catch (FileNotFoundException exc)
			{
			//	System.Windows.Forms.MessageBox.Show(exc.Message, "File not found.", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
			catch (Exception exc)
			{
				System.Windows.Forms.MessageBox.Show("Invalid expression:\n"+exc.Message+"\nCheck the syntax!", "Invalid expression.", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
			
			return psettings;
		}
		
		#region properties
		public static string ConfigFile
		{
			get
			{
				return configfile;
			}
		}
		
		public static string Q2exe
		{
			set
			{
					settings["q2exe"] = value;
					System.Diagnostics.Trace.WriteLine("q2exe = " + settings["q2exe"]);
			}
			get
			{
				try
				{
					return settings["q2exe"];
				}
				catch
				{
					return "quake2.exe";
				}
			}
		}

		public static string DemoCommand
		{
			get
			{
				// by default demomap
				var info = FileVersionInfo.GetVersionInfo(Q2exe);
				Trace.WriteLine(info.FileDescription);
				if (info.FileDescription.Contains("Q2PRO"))
                {
					return "demo";
                }
                else
                {
					return "demomap";
                }
			}
		}

        public static string baseq2dir
		{
			get
			{
				try
				{
					return settings["q2exe"].Substring(0, settings["q2exe"].LastIndexOf("\\"))+"\\baseq2";
				}
				catch
				{
					return "";
				}
			}
		}
        public static string Q2dir
		{
			get
			{
				try
				{
					return settings["q2exe"].Substring(0, settings["q2exe"].LastIndexOf("\\"));
				}
				catch
				{
					return "";
				}
			}
		}

        public static string demosdir
		{
			get
			{
				try
				{
					if (!File.Exists(ConfigReader.Q2exe))
                    {
                        return "";
                    }

                    var demosdirPath = settings["q2exe"].Substring(0, settings["q2exe"].LastIndexOf("\\")) + "\\baseq2\\demos";

					// Creates all directories and subdirectories in the specified path unless they already exist.
					var di = new DirectoryInfo(demosdirPath);
					di.Create();

					return demosdirPath;
				}
				catch
				{
					return "";
				}
			}
		}

        public static bool closeafter
		{
			get
			{
				try
				{
					if (settings["closeafter"] == "true")
						return true;
					else
						return false;
				}
				catch
				{
					return false;
				}

			}
			set
			{
				if (value)
					settings["closeafter"] = "true";
				else
					settings["closeafter"] = "false";
				
				System.Diagnostics.Trace.WriteLine("closeafter = " + value);
			}
		}

        public static string PauseButton
		{
			get
			{
				try
				{
					return settings["pause"];
					
				}
				catch
				{
					return "F6";
				}

			}
			set
			{
				settings["pause"] = value;
				System.Diagnostics.Trace.WriteLine("PauseButton = " + value);
			}
		}

        public static string FFButton
		{
			get
			{
				try
				{
					return settings["ff"];
					
				}
				catch
				{
					return "F5";
				}

			}
			set
			{
				settings["ff"] = value;
				System.Diagnostics.Trace.WriteLine("FFButton = " + value);
			}
		}

        public static string SlowmoButton
		{
			get
			{
				try
				{
					return settings["slowmo"];
					
				}
				catch
				{
					return "F7";
				}

			}
			set
			{
				settings["slowmo"] = value;
				System.Diagnostics.Trace.WriteLine("SlowmoButton = " + value);
			}
		}
		#endregion
	}
}

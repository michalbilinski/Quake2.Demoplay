/*
 * Created by SharpDevelop.
 * User: michal
 * Date: 2010-04-06
 * Time: 09:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.IO;

namespace Quake2.Demoplay.App
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
			
			if (args.Length > 0 && File.Exists("demoplay.ini"))
			{
				Application.Run(new MainForm(args[0]));
			}
			else
			{
				Application.Run(new MainForm());
			}		
		}
		
	}
}

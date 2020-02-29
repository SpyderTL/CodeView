using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodeView
{
	static class Program
	{
		public static string Path = null;
		public static string File = null;
		public static byte[] Data = null;
		public static Stack<Change> Undo = new Stack<Change>();
		public static Stack<Change> Redo = new Stack<Change>();

		[STAThread]
		static void Main()
		{
			Path = "../../Examples";
			File = "StarFoxUsa12.cvproj";

			ProjectFile.Load(System.IO.Path.Combine(Path, File));
			ProgramProject.Load();

			ProgramForm.Load();

			ProgramForm.Refresh();

			Application.Run(ProgramForm.Form);
		}
	}
}

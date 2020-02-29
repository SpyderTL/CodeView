using System;
using System.IO;

namespace CodeView
{
	public class ProgramProject
	{
		public static void Load()
		{
			Program.Data = new byte[Project.Length];

			foreach (var file in Project.Files)
			{
				using (var reader = File.OpenRead(Path.Combine(Program.Path, file.Path)))
				{
					foreach (var segment in file.Segments)
					{
						reader.Position = (long)segment.Offset;

						reader.Read(Program.Data, (int)segment.Address, (int)segment.Length);
					}
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeView
{
	public static class Project
	{
		public static string Name;
		public static string Description;
		public static string Notes;
		public static ulong Length;

		public static List<File> Files = new List<File>();
		public static List<Function> Functions = new List<Function>();
		public static List<Table> Tables = new List<Table>();
		public static List<Variable> Variables = new List<Variable>();

		public class File
		{
			public string Path;
			public List<Segment> Segments = new List<Segment>();

			public class Segment
			{
				public ulong Offset;
				public ulong Address;
				public ulong Length;
			}
		}

		public class Function
		{
			public ulong Address;
			public string Name;
			public string Description;
			public string Notes;
			public Dictionary<string, string> Properties = new Dictionary<string, string>();
		}

		public class Variable
		{
			public ulong Address;
			public string Name;
			public string Description;
			public string Notes;
			public Dictionary<string, string> Properties = new Dictionary<string, string>();
		}

		public class Table
		{
			public ulong Address;
			public string Name;
			public string Description;
			public string Notes;
			public Dictionary<string, string> Properties = new Dictionary<string, string>();
		}
	}
}

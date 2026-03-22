//////////////////////////////////////////////////////////////////////////////////////////////////
//	Projects		: DHUtilsSol
//	Author			: CyberKDH(cyberkdh@gmail.com, cyberkdh@hotmail.com)
//	Module			: Changes File's DateTime
//	History			: 
//	Copyrights		: Copyright ⓒCYBERKDH Co., Ltd. All Rights Reserved.
//////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHChangeFileDateTime {
    internal class Program {
		public static bool m_bChange_CreatedDateTime = false;
		public static bool m_bChange_ModifiedDateTime = false;
		public static bool m_bChange_AccessedDateTime = false;

		static void Main(string[] args) {

			// Show help if not a required argument count
			if (args.Length < 3) {
				PrintHelp();
				return;
			}

			// Check Argument
			string strcma = args[0];
			m_bChange_CreatedDateTime = strcma.IndexOf('c') != -1 ? true : false;
			m_bChange_ModifiedDateTime = strcma.IndexOf('m') != -1 ? true : false;
			m_bChange_AccessedDateTime = strcma.IndexOf('a') != -1 ? true : false;

			// Converts input datetime string to DateTime Object
			string strDateTime = args[1];
			DateTime dtNew = DateTime.Now;
			if (!ConvertToDateTime(strDateTime, out dtNew) == true) {
				PrintHelp();
				return;
			}

			// sets File(s)'s date(create, modify, access) as new datetime
			List<string> lstFileOrPath = args.ToList().GetRange(2, args.Length - 2);
			int i, j;
			for (i = 0; i < lstFileOrPath.Count; i++) {
				Console.WriteLine("Path: " + lstFileOrPath[i]);
				List<string> lst = GetFiles(lstFileOrPath[i]);
				for (j = 0; j < lst.Count; j++) {
					try {
						if (m_bChange_CreatedDateTime == true) {
							File.SetCreationTime(lst[j], dtNew);
						}

						if (m_bChange_ModifiedDateTime == true) {
							File.SetLastWriteTime(lst[j], dtNew);
						}

						if (m_bChange_AccessedDateTime == true) {
							File.SetLastAccessTime(lst[j], dtNew);
						}
						Console.WriteLine($"  [success] File: {lst[j]}");
					}
					catch (Exception ex) {
						Console.WriteLine($"  [failed] File: {lst[j]}: {ex.ToString()}");
					}
				}
			}
		}

		// If input argument is folder, enumerates files with input pattern
		public static List<string> GetFiles(string strFileOrPath) {
			List<string> lst = new List<string>();

			try {
				FileAttributes attr = File.GetAttributes(strFileOrPath);
				if (attr.HasFlag(FileAttributes.Directory) == true) {
					try {
						string strfilepath = strFileOrPath;
						int nidx = strfilepath.LastIndexOf("\\");
						if (nidx != -1) {
							string[] arrfiles = Directory.GetFiles(strfilepath.Substring(0, nidx), strfilepath.Substring(nidx + 1));
							lst = arrfiles.ToList();
						}
					}
					catch (Exception ex) {
						Console.WriteLine("[ERR] " + ex.Message);
					}
				}
				else {
					lst.Add(strFileOrPath);
				}
			}
			catch(Exception ex) {
				Console.WriteLine("[ERR] " + ex.Message);

			}
			return lst;
		}

		// Converts string type datetime to DateTime object
		static bool ConvertToDateTime(string str, out DateTime dt) {
			bool bret = false;
			dt = DateTime.Now;

			try {
				List<string> lstformat = GetSupportFormatter();
				int i;
				for (i = 0; i < lstformat.Count; i++) {
					if (str.Length == lstformat[i].Length) {
						DateTime dtConv = DateTime.ParseExact(str, lstformat[i], null);
						dt = dtConv;
						bret = true;
						break;
					}
				}
			}
			catch (Exception ex) {
				bret = false;
				Console.WriteLine("Failed to parse datetime from input data: " + ex.Message);
			}

			return bret;
		}

		// Show supported datetime format
		static List<string> GetSupportFormatter() {
			List<string> lstformatter = new List<string>() {
				"yyyyMMddHHmmss",
				"MMddHHmmss",
				"ddHHmmss",
				"HHmmss",
				"mmss",
				"ss",
			};

			return lstformatter;
		}

		// Show help
		static void PrintHelp() {
			Console.WriteLine();
			Console.WriteLine("Help for DHChangeFileDateTime");
			Console.WriteLine("DHChangeFileDateTime {cma} {datetime} [file or pathwithexts] .... ");
			Console.WriteLine();
			Console.WriteLine(" cmd: c=>createdatetime, m=>modifieddatetime, a=>accessdatetime");
			Console.WriteLine("DateTime Format");
			List<string> lstformat = GetSupportFormatter();
			int i;
			for (i = 0; i < lstformat.Count; i++) {
				Console.WriteLine($"  {lstformat[i]}");
			}

			Console.WriteLine();

			Console.WriteLine("example: ");			
			Console.WriteLine(@"  DHChangeFileDateTime cma 20260322000000 h:\temp\*.*");
			Console.WriteLine(@"  DHChangeFileDateTime cma 20260322000000 h:\temp\a.txt");

			Console.WriteLine();
			Console.WriteLine();
		}
	}
}

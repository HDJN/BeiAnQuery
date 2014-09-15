using System;
using System.Runtime.InteropServices;
using System.Threading;
namespace ImgOcr
{
	public class Package
	{
		private static object objlock = new object();
		public static int id = -1;
		[DllImport("baidu.dll")]
		private static extern int loadcode(int a, int b, string c, string d);
		[DllImport("baidu.dll")]
		private static extern int Recognitiond(int a, int b, int c, string d, string e);
		public static void Init(String runtimedatPath)
		{
			object obj;
			Monitor.Enter(obj = Package.objlock);
			try
			{
				if (Package.id == -1)
				{
                    string c = runtimedatPath;
					string str = "AvX";
					string str2 = "v5d4";
					string str3 = ",m3";
					string d = str + str2 + str3 + "N";
					int num2;
					try
					{
						int num = Package.loadcode(0, 0, c, d);
						num2 = num;
					}
					catch (Exception)
					{
						num2 = -1;
					}
					Package.id = num2;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static string Recognitondimg(string loclpath)
		{
			string text = "";
			string result;
			try
			{
				string text2 = "";
				object obj;
				Monitor.Enter(obj = Package.objlock);
				try
				{
					text2 = Marshal.PtrToStringAnsi((IntPtr)Package.Recognitiond(Package.id, 0, 0, "", loclpath), 4);
				}
				finally
				{
					Monitor.Exit(obj);
				}
				text = text2;
				if (text2.Length == 4)
				{
					string text3 = text2;
					for (int i = 0; i < text3.Length; i++)
					{
						char c = text3[i];
						if (c == '?')
						{
							result = "";
							return result;
						}
					}
					result = text;
					return result;
				}
			}
			catch
			{
				text = "";
			}
			result = text;
			return result;
		}
	}
}

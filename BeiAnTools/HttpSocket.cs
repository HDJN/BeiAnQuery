using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
public class HttpSocket
{
	private struct UrlInfo
	{
		public string Host;
		public int Port;
		public string File;
		public string Body;
	}
	private string sCookies = "";
	private string sReferer = "";
	private string sUserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; Tablet PC 2.0; .NET4.0C; .NET4.0E)";
	private string sAccept = "*/*";
	private string sEncoding;
	private string sContentType = "application/x-www-form-urlencoded";
	public string Cookies
	{
		get
		{
			return this.sCookies;
		}
		set
		{
			this.sCookies = value;
		}
	}
	public string Referer
	{
		get
		{
			return this.sReferer;
		}
		set
		{
			this.sReferer = value;
		}
	}
	public string UserAgent
	{
		get
		{
			return this.sUserAgent;
		}
		set
		{
			this.sUserAgent = value;
		}
	}
	public string Accept
	{
		get
		{
			return this.sAccept;
		}
		set
		{
			this.sAccept = value;
		}
	}
	public string Encoding
	{
		get
		{
			return this.sEncoding;
		}
		set
		{
			this.sEncoding = value;
		}
	}
	public string ContentType
	{
		get
		{
			return this.sContentType;
		}
		set
		{
			this.sContentType = value;
		}
	}
	~HttpSocket()
	{
	}
	private void SetCookies(string sHtml)
	{
		if (!this.sCookies.EndsWith(";") && this.sCookies != "")
		{
			this.sCookies += ";";
		}
		Regex regex = new Regex("Set-Cookie:\\s*(?<sName>.*?)=(?<sValue>.*?);", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
		MatchCollection matchCollection = regex.Matches(sHtml);
		for (int i = 0; i < matchCollection.Count; i++)
		{
			string text = matchCollection[i].Groups["sName"].Value.Trim();
			string text2 = matchCollection[i].Groups["sValue"].Value.Trim();
			regex = new Regex(text + "\\s*=\\s*.*?;", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
			Match match = regex.Match(this.sCookies);
			if (match.Success)
			{
				this.sCookies = this.sCookies.Replace(match.Value, text + "=" + text2 + ";");
			}
			else
			{
				string text3 = this.sCookies;
				this.sCookies = string.Concat(new string[]
				{
					text3,
					text,
					"=",
					text2,
					";"
				});
			}
		}
		try
		{
			if (this.sCookies.StartsWith(";"))
			{
				this.sCookies = this.sCookies.Substring(1, this.sCookies.Length - 1);
			}
		}
		catch
		{
		}
	}
	private HttpSocket.UrlInfo ParseURL(string url)
	{
		if (!url.ToLower().StartsWith("http://"))
		{
			url = "http://" + url;
		}
		HttpSocket.UrlInfo result = default(HttpSocket.UrlInfo);
		result.Host = "";
		result.Port = 80;
		result.File = "/";
		result.Body = "";
		int num = url.ToLower().IndexOf("http://");
		if (num != -1)
		{
			url = url.Substring(7);
			num = url.IndexOf("/");
			if (num == -1)
			{
				result.Host = url;
			}
			else
			{
				result.Host = url.Substring(0, num);
				url = url.Substring(num);
				num = result.Host.IndexOf(":");
				if (num != -1)
				{
					string[] array = result.Host.Split(new char[]
					{
						':'
					});
					result.Host = array[0];
					int.TryParse(array[1], out result.Port);
				}
				num = url.IndexOf("?");
				if (num == -1)
				{
					result.File = url;
				}
				else
				{
					string[] array = url.Split(new char[]
					{
						'?'
					});
					result.File = array[0];
					result.Body = array[1];
				}
			}
		}
		return result;
	}
	private string GetResponse(string host, int port, string body, string sCode, out string sHeaders)
	{
		Encoding encoding = System.Text.Encoding.GetEncoding(sCode);
		sHeaders = string.Empty;
		string text = string.Empty;
		byte[] bytes = System.Text.Encoding.ASCII.GetBytes(body);
		byte[] array = new byte[4096];
		using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
		{
			Stream stream = new MemoryStream();
			try
			{
				socket.Connect(host, port);
				if (socket.Connected)
				{
					socket.Send(bytes, bytes.Length, SocketFlags.None);
					int count;
					while ((count = socket.Receive(array, array.Length, SocketFlags.None)) > 0)
					{
						stream.Write(array, 0, count);
					}
				}
				stream.Position = 0L;
				StreamReader streamReader = new StreamReader(stream, encoding);
				text = streamReader.ReadToEnd();
				streamReader.Close();
				streamReader.Dispose();
				stream.Close();
				stream.Dispose();
				socket.Close();
			}
			catch
			{
			}
		}
		this.SetCookies(text);
		Regex regex = new Regex("^(.*?)\r\n\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
		Match match = regex.Match(text);
		if (match.Success)
		{
			sHeaders = match.Value;
			text = text.Substring(sHeaders.Length, text.Length - sHeaders.Length);
			sHeaders = sHeaders.Trim();
		}
		return text;
	}
	public bool DownloadFile2(string sFileUrl, string sFilePath, out string sMsg)
	{
		sMsg = "";
		bool result;
		if (sFileUrl == "" || sFilePath == "")
		{
			sMsg = "文件URL和文件保存路径都不能为空";
			result = false;
		}
		else
		{
			string path = sFilePath.Remove(sFilePath.LastIndexOf('\\'));
			if (Directory.Exists(path))
			{
				if (File.Exists(sFilePath))
				{
					File.Delete(sFilePath);
				}
			}
			else
			{
				Directory.CreateDirectory(path);
			}
			HttpSocket.UrlInfo urlInfo = this.ParseURL(sFileUrl);
			string text;
			if (urlInfo.Body != "")
			{
				text = string.Format("GET {0}?{1} HTTP/1.1\r\n", urlInfo.File, urlInfo.Body);
			}
			else
			{
				text = string.Format("GET {0} HTTP/1.1\r\n", urlInfo.File);
			}
			if (urlInfo.Port == 80)
			{
				text += string.Format("Host:{0}\r\n", urlInfo.Host, urlInfo.ToString());
			}
			else
			{
				text += string.Format("Host:{0}:{1}\r\n", urlInfo.Host, urlInfo.Port.ToString());
			}
			text += string.Format("Referer:{0}\r\n", this.sReferer);
			text += string.Format("User-Agent:{0}\r\n", this.sUserAgent);
			text += string.Format("Connection:Close\r\n", new object[0]);
			text += string.Format("Cookie:{0}", this.sCookies);
			text += "\r\n\r\n";
			text += "OK";
			string text2 = string.Empty;
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(text);
			byte[] array = new byte[1];
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				try
				{
					socket.Connect(urlInfo.Host, urlInfo.Port);
					if (socket.Connected)
					{
						socket.Send(bytes, bytes.Length, SocketFlags.None);
						int count;
						while ((count = socket.Receive(array, array.Length, SocketFlags.None)) > 0)
						{
							text2 += System.Text.Encoding.ASCII.GetString(array, 0, count);
							if (text2.IndexOf("\r\n\r\n") > -1)
							{
								break;
							}
						}
						Stream stream = new FileStream(sFilePath, FileMode.Create);
						array = new byte[1];
						while ((count = socket.Receive(array, array.Length, SocketFlags.None)) > 0)
						{
							if (array[0] == 255)
							{
								stream.Write(array, 0, count);
								break;
							}
						}
						array = new byte[1024];
						while ((count = socket.Receive(array, array.Length, SocketFlags.None)) > 0)
						{
							stream.Write(array, 0, count);
						}
						stream.Close();
						stream.Dispose();
					}
					socket.Close();
				}
				catch
				{
				}
			}
			this.SetCookies(text2);
			result = true;
		}
		return result;
	}
	public bool DownloadFile1(string sFileUrl, string sFilePath, out string sMsg)
	{
		sMsg = "";
		bool result;
		if (sFileUrl == "" || sFilePath == "")
		{
			sMsg = "文件URL和文件保存路径都不能为空";
			result = false;
		}
		else
		{
			string path = sFilePath.Remove(sFilePath.LastIndexOf('\\'));
			if (Directory.Exists(path))
			{
				if (File.Exists(sFilePath))
				{
					File.Delete(sFilePath);
				}
			}
			else
			{
				Directory.CreateDirectory(path);
			}
			WebClient webClient = new WebClient();
			webClient.Headers.Add("Referer", this.sReferer);
			webClient.Headers.Add("User-Agent", this.sUserAgent);
			webClient.Headers.Add("Cookie", this.sCookies);
			try
			{
				webClient.DownloadFile(sFileUrl, sFilePath);
			}
			catch
			{
				result = false;
				return result;
			}
			webClient.Dispose();
			result = true;
		}
		return result;
	}
	public bool GetImg(string sFileUrl, string sFilePath, out string msg)
	{
		bool result;
		try
		{
			this.DownloadFile1(sFileUrl, sFilePath, out msg);
			result = true;
		}
		catch (Exception ex)
		{
			msg = ex.Message;
			result = false;
		}
		return result;
	}
	public string UrlEncode(string str, string CharSet)
	{
		string result;
		if (CharSet.Trim() == "")
		{
			result = str;
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte[] bytes = System.Text.Encoding.GetEncoding(CharSet).GetBytes(str);
			for (int i = 0; i < bytes.Length; i++)
			{
				if (Convert.ToInt32(bytes[i]) < 128)
				{
					stringBuilder.Append((char)bytes[i]);
				}
				else
				{
					stringBuilder.Append("%" + Convert.ToString(bytes[i], 16).ToUpper());
				}
			}
			result = stringBuilder.ToString();
		}
		return result;
	}
	public string Get(string url, string encode)
	{
		string text;
		return this.Get(url, encode, out text);
	}
	public string Get(string url, string encode, out string sHeaders)
	{
		HttpSocket.UrlInfo urlInfo = this.ParseURL(url);
		string text = string.Format("GET {0}?{1} HTTP/1.1\r\n", urlInfo.File, urlInfo.Body);
		text += string.Format("Host:{0}:{1}\r\n", urlInfo.Host, urlInfo.Port.ToString());
		text += string.Format("Content-Type:{0}\r\n", this.ContentType);
		text += string.Format("Referer:{0}\r\n", this.sReferer);
		text += "Accept: image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/msword, */*\r\n";
		text += string.Format("User-Agent:{0}\r\n", this.sUserAgent);
		text += string.Format("Connection:Close\r\n", new object[0]);
		text += "Cache-Control: no-cache\r\n";
		text += string.Format("Cookie:{0}", this.sCookies);
		text += "\r\n\r\n";
		return this.GetResponse(urlInfo.Host, urlInfo.Port, text, encode, out sHeaders);
	}
	public string Post(string url, string sPostString, string encode)
	{
		sPostString = this.UrlEncode(sPostString, encode);
		string text;
		return this.Post(url, sPostString, encode, out text);
	}
	public string Post(string url, string sPostString, string encode, out string sHeaders)
	{
		HttpSocket.UrlInfo urlInfo = this.ParseURL(url);
		string text = "";
		if (urlInfo.Body != "")
		{
			text += string.Format("POST {0}?{1} HTTP/1.1\r\n", urlInfo.File, urlInfo.Body);
		}
		else
		{
			text += string.Format("POST {0} HTTP/1.1\r\n", urlInfo.File);
		}
		text += string.Format("Host:{0}:{1}\r\n", urlInfo.Host, urlInfo.Port.ToString());
		text += string.Format("Content-Length:{0}\r\n", sPostString.Length.ToString());
		text += string.Format("Content-Type:{0}\r\n", this.ContentType);
		text += string.Format("Referer:{0}\r\n", this.sReferer);
		text += "Accept: image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/msword, */*\r\n";
		text += "Accept-Language:zh-CN\r\n";
		text += string.Format("User-Agent:{0}\r\n", this.sUserAgent);
		text += string.Format("Connection:Close\r\n", new object[0]);
		text += "Cache-Control: no-cache\r\n";
		text += string.Format("Cookie:{0}", this.sCookies);
		text += string.Format("\r\n\r\n", new object[0]);
		text += string.Format("{0}", sPostString);
		return this.GetResponse(urlInfo.Host, urlInfo.Port, text, encode, out sHeaders);
	}
}

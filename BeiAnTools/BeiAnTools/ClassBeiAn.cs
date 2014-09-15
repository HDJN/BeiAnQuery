using ImgOcr;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace BeiAnTools
{
	public class ClassBeiAn
	{
		public enum ServerName
		{
			工信部,
			北京,
			天津,
			河北,
			山西,
			内蒙古,
			辽宁,
			吉林,
			黑龙江,
			上海,
			江苏,
			浙江,
			安徽,
			福建,
			江西,
			山东,
			河南,
			湖北,
			湖南,
			广东,
			广西,
			海南,
			重庆,
			四川,
			贵州,
			云南,
			西藏,
			陕西,
			甘肃,
			青海,
			宁夏,
			新疆
		}
		public string sID = "Temp";
		private string sBeiAnUrl = "";
		private string GetUrl = "{BeiAnUrl}/icp/publish/query/icpMemoInfo_searchExecute.action";
		private string sChapchaUrl = "{BeiAnUrl}/getVerifyCode";
		private int sChapchaLen = 4;
		public string sCheckType = "0";
		private string PostDate0 = "siteName=&condition=1&siteDomain={DOMAIN}&siteUrl=&mainLicense=&siteIp=&unitName=&mainUnitNature=-1&certType=-1&mainUnitCertNo=&verifyCode={CHAPCHA}";
		private string PostDate1 = "siteName=&siteDomain=&siteUrl=&condition=3&mainLicense={DOMAIN}&siteIp=&unitName=&mainUnitNature=-1&certType=-1&mainUnitCertNo=&verifyCode={CHAPCHA}";
		private string sRealPostDate = "";
		private string GetExp0 = "<tr id=\"\\d+\" align=\"center\">\\S*<td align=\"center\" class=\"by1\">.{1,21}</td>\\S*<td align=\"center\" class=\"bxy\">(?<OWNERNAME>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<TYPE>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<ICPNUM>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<SITENAME>.+?)</td>\\S*<td align=\"center\" class=\"bxy\"><div> <a href=\"http://(?<SITEURL>.+?)\" target=\"_blank\">(?<DOMAIN>.+?)</a></div>&nbsp;</td>\\S*<td align=\"center\" class=\"bxy\">(?<ADDTIME>.+?)</td>\\S*<td align=\"center\" class=\"by2\">&nbsp;\\S*<span onclick=\"doDetail\\('(?<BEIANID>\\d+)'\\)";
		private string GetExp1 = "<tr id=\"\\d+\" align=\"center\">\\S*<td align=\"center\" class=\"by1\">.{1,21}</td>\\S*<td align=\"center\" class=\"bxy\">(?<OWNERNAME>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<TYPE>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<ICPNUM>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<SITENAME>.+?)</td>\\S*<td align=\"center\" class=\"bxy\"><div>\\s*<a\\s*href=\"http://(?<SITEURL>.+?)\" target=\"_blank\">(?<DOMAIN>.+?)</a></div>&nbsp;</td>\\S*<td align=\"center\" class=\"bxy\">(?<ADDTIME>.{1,20})</td>\\S*<td align=\"center\" class=\"by2\">&nbsp;\\S*<span onclick=\"doDetail\\('(?<BEIANID>\\d+)'\\)|<tr id=\"\\d+\" align=\"center\">\\S*<td align=\"center\" class=\"by1\">.{1,21}</td>\\S*<td align=\"center\" class=\"bxy\">(?<OWNERNAME>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<TYPE>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<ICPNUM>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">(?<SITENAME>.+?)</td>\\S*<td align=\"center\" class=\"bxy\">&nbsp;</td>\\S*<td align=\"center\" class=\"bxy\">(?<ADDTIME>.{1,20})</td>\\S*<td align=\"center\" class=\"by2\">&nbsp;\\S*<span onclick=\"doDetail\\('(?<BEIANID>\\d+)'\\)";
		private string UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
		private string ErrCode = "<title>备案信息查询</title>";
		private string NoBeiAnCode = "没有符合条件的记录";
		private string sCode = "GBK";
		public bool BeErr = false;
		private string sInitUrl = "{BeiAnUrl}/icp/publish/query/icpMemoInfo_showPage.action";
		private HttpSocket HC = new HttpSocket();
		private string msg = "";
		public string sChapchaValue = "";
		public string sDomain = "";
		public string sOwnerName = "";
		public string sType = "";
		public string sICPNum = "";
		public string sSiteName = "";
		public string sSiteUrl = "";
		public string sAddTime = "";
		public string sBeiAn = "";
		public string sBeiAnID = "";
		public ClassBeiAn(string ServerName,string RuntimePath)
		{
            try
            {
                if (string.IsNullOrEmpty(RuntimePath))
                {
                    RuntimePath = Environment.CurrentDirectory;
                }
                Package.Init(RuntimePath);
            }
            catch
            {

            }

            switch (ServerName)
			{
			case "工信部":
				this.sBeiAnUrl = "http://www.miitbeian.gov.cn";
				goto IL_599;
			case "北京":
				this.sBeiAnUrl = "http://bcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "天津":
				this.sBeiAnUrl = "http://tjcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "河北":
				this.sBeiAnUrl = "http://hbcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "山西":
				this.sBeiAnUrl = "http://sxcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "内蒙古":
				this.sBeiAnUrl = "http://nmcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "辽宁":
				this.sBeiAnUrl = "http://lncainfo.miitbeian.gov.cn";
				goto IL_599;
			case "吉林":
				this.sBeiAnUrl = "http://jlcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "黑龙江":
				this.sBeiAnUrl = "http://hlcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "上海":
				this.sBeiAnUrl = "http://shcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "江苏":
				this.sBeiAnUrl = "http://jscainfo.miitbeian.gov.cn";
				goto IL_599;
			case "浙江":
				this.sBeiAnUrl = "http://zcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "安徽":
				this.sBeiAnUrl = "http://ahcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "福建":
				this.sBeiAnUrl = "http://fjcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "江西":
				this.sBeiAnUrl = "http://jxcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "山东":
				this.sBeiAnUrl = "http://sdcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "河南":
				this.sBeiAnUrl = "http://hcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "湖北":
				this.sBeiAnUrl = "http://ecainfo.miitbeian.gov.cn";
				goto IL_599;
			case "湖南":
				this.sBeiAnUrl = "http://xcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "广东":
				this.sBeiAnUrl = "http://gdcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "广西":
				this.sBeiAnUrl = "http://gxcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "海南":
				this.sBeiAnUrl = "http://hncainfo.miitbeian.gov.cn";
				goto IL_599;
			case "重庆":
				this.sBeiAnUrl = "http://cqcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "四川":
				this.sBeiAnUrl = "http://sccainfo.miitbeian.gov.cn";
				goto IL_599;
			case "贵州":
				this.sBeiAnUrl = "http://gzcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "云南":
				this.sBeiAnUrl = "http://yncainfo.miitbeian.gov.cn";
				goto IL_599;
			case "西藏":
				this.sBeiAnUrl = "http://xzcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "陕西":
				this.sBeiAnUrl = "http://shxcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "甘肃":
				this.sBeiAnUrl = "http://gscainfo.miitbeian.gov.cn";
				goto IL_599;
			case "青海":
				this.sBeiAnUrl = "http://qhcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "宁夏":
				this.sBeiAnUrl = "http://nxcainfo.miitbeian.gov.cn";
				goto IL_599;
			case "新疆":
				this.sBeiAnUrl = "http://xjcainfo.miitbeian.gov.cn";
				goto IL_599;
			}
			this.sBeiAnUrl = "http://jscainfo.miitbeian.gov.cn";
			IL_599:
			this.GetUrl = this.GetUrl.Replace("{BeiAnUrl}", this.sBeiAnUrl);
			this.sChapchaUrl = this.sChapchaUrl.Replace("{BeiAnUrl}", this.sBeiAnUrl);
			this.sInitUrl = this.sInitUrl.Replace("{BeiAnUrl}", this.sBeiAnUrl);
			this.InitHttpContent();
		}
		~ClassBeiAn()
		{
		}
		public string KeyReplace(string sStr)
		{
			string result;
			if (sStr == null || Convert.ToString(sStr) == "")
			{
				result = "";
			}
			else
			{
				string text = "";
				ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
				byte[] bytes = aSCIIEncoding.GetBytes(sStr);
				byte[] array = bytes;
				for (int i = 0; i < array.Length; i++)
				{
					byte b = array[i];
					if ((b > 47 && b < 58) || (b > 64 && b < 91) || (b > 96 && b < 123))
					{
						string arg_87_0 = text;
						char c = (char)b;
						text = arg_87_0 + c.ToString();
					}
				}
				result = text;
			}
			return result;
		}
		public void InitHttpContent()
		{
			if (this.sInitUrl != "")
			{
				try
				{
					this.HC.Get(this.sInitUrl, this.sCode);
				}
				catch
				{
				}
			}
			this.HC.UserAgent = this.UserAgent;
			this.HC.Referer = this.sInitUrl;
			this.HC.Accept = "*.*"; 
		}
		public string GetChapcha(String ChapchaPath)
		{
			string result;
			if (this.sChapchaUrl != "")
			{
				string text = "";
                string text2 = ChapchaPath + this.sID + "_Chapcha.jpg";
				while (text == "")
				{
					try
					{
						int i = 0;
						this.InitHttpContent();
						while (i < 5)
						{
							this.HC.Referer = this.sChapchaUrl;
							if (this.HC.GetImg(this.sChapchaUrl + "?" + DateTime.Now.Millisecond.ToString(), text2, out this.msg))
							{
								break;
							}
							i++;
						}
						if (i == 5)
						{
							result = "";
							return result;
						}
						if (File.ReadAllBytes(text2).Length <= 500)
						{
							this.HC.Cookies = "";
							result = "验证码下载失败：文件过小";
							return result;
						}
						text = Package.Recognitondimg(text2);
						text = Regex.Replace(text, "\\W", "");
						if (text.Length != this.sChapchaLen)
						{
							text = "";
						}
					}
					catch (Exception ex)
					{
						result = ex.Message;
						return result;
					}
				}
				this.sChapchaValue = text;
				result = text;
			}
			else
			{
				result = "";
			}
			return result;
		}
		public void GetBeinAnInfo(string sValue)
		{
			this.sDomain = "";
			this.sOwnerName = "";
			this.sICPNum = "";
			this.sSiteName = "";
			this.sSiteUrl = "";
			this.sAddTime = "";
			this.sType = "";
			this.sBeiAn = "未匹配";
			this.BeErr = false;
			this.sBeiAnID = "";
			string text = this.GetUrl;
			this.sDomain = sValue;
			string text2 = this.sCheckType;
			string pattern;
			if (text2 != null)
			{
				if (text2 == "0")
				{
					this.sRealPostDate = this.PostDate0;
					pattern = this.GetExp0;
					goto IL_EE;
				}
				if (text2 == "1")
				{
					this.sRealPostDate = this.PostDate1;
					pattern = this.GetExp1;
					goto IL_EE;
				}
			}
			this.sRealPostDate = this.PostDate0;
			pattern = this.GetExp0;
			IL_EE:
			this.sChapchaUrl = this.sChapchaUrl.Replace("{DOMAIN}", sValue);
			text = text.Replace("{DOMAIN}", sValue);
			this.sRealPostDate = this.sRealPostDate.Replace("{DOMAIN}", sValue);
			text = text.Replace("{DOMAIN}", sValue);
			this.sRealPostDate = this.sRealPostDate.Replace("{ICPNUM}", sValue);
			text = text.Replace("{ICPNUM}", sValue);
			this.sRealPostDate = this.sRealPostDate.Replace("{BEIANID}", sValue);
			text = text.Replace("{BEIANID}", sValue);
			this.sRealPostDate = this.sRealPostDate.Replace("{CHAPCHA}", this.sChapchaValue);
			string text3 = this.HC.Post(text, this.sRealPostDate, this.sCode);
			text3 = text3.Replace("\t", "");
			text3 = text3.Replace("\r\n", "");
			text3 = text3.Replace("\r", "");
			text3 = text3.Replace("\n", "");
			if (text3.Length > 0)
			{
				Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
				MatchCollection matchCollection = regex.Matches(text3);
				if (matchCollection.Count > 0)
				{
					try
					{
						this.sDomain = matchCollection[0].Groups["DOMAIN"].Value;
					}
					catch
					{
					}
					try
					{
						this.sOwnerName = matchCollection[0].Groups["OWNERNAME"].Value;
					}
					catch
					{
					}
					try
					{
						this.sType = matchCollection[0].Groups["TYPE"].Value;
					}
					catch
					{
					}
					try
					{
						this.sICPNum = matchCollection[0].Groups["ICPNUM"].Value;
					}
					catch
					{
					}
					try
					{
						this.sSiteName = matchCollection[0].Groups["SITENAME"].Value;
					}
					catch
					{
					}
					try
					{
						this.sSiteUrl = matchCollection[0].Groups["SITEURL"].Value;
					}
					catch
					{
					}
					try
					{
						this.sAddTime = matchCollection[0].Groups["ADDTIME"].Value;
					}
					catch
					{
					}
					try
					{
						this.sBeiAnID = matchCollection[0].Groups["BEIANID"].Value;
					}
					catch
					{
					}
					this.sBeiAn = "已备案";
				}
				else
				{
					string[] array = this.NoBeiAnCode.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						if (text3.IndexOf(array[i]) > 0)
						{
							this.sBeiAn = "未备案";
							return;
						}
					}
					string[] array2 = this.ErrCode.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array2.Length; i++)
					{
						if (text3.IndexOf(array2[i]) > 0)
						{
							this.BeErr = true;
							break;
						}
					}
				}
			}
			else
			{
				if (this.ErrCode == "" && text3.Trim() == "")
				{
					this.BeErr = true;
				}
			}
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
				byte[] bytes = Encoding.GetEncoding(CharSet).GetBytes(str);
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
	}
}

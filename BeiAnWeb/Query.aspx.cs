using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Query : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            Response.ContentType = "text/plain";
            Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            HttpContext.Current.Response.Clear();
            string sValue = HttpContext.Current.Request["sValue"];
            string sType = HttpContext.Current.Request["sType"];
            string sName = HttpContext.Current.Request["sName"];
            if (!string.IsNullOrEmpty(sValue)
    && !string.IsNullOrEmpty(sType)
    && !string.IsNullOrEmpty(sName))
            {

                GetBeiAnResult(sName, sType, sValue);
            }
            else
            {
                Response.Write("参数不能为空");
                HttpContext.Current.Response.End();
            }
        }
        else {
            
        }
    }

    public static void GetBeiAnResult(string sName,string sType,string sValue) 
    {
        //Init(@"C:\c#codes\BeiAnWeb\Bin\runtime.dat");
        BeiAnTools.ClassBeiAn CBA = new BeiAnTools.ClassBeiAn(sName,System.Configuration.ConfigurationManager.AppSettings["RuntimePath"]);//初始化备案接口
        CBA.sID = DateTime.Now.ToFileTime().ToString();//接口ID，需要赋予一个与其它接口不重复的值，用来区别下载的验证码文件

            ReGetChapcha://错误后返回这里重新查询
        //Response.Write("获取验证码...");
        string sChapcha = CBA.GetChapcha(System.Configuration.ConfigurationManager.AppSettings["ChapchaPath"]);//首先获取验证码

        if (sChapcha == "")//验证码为空
        {
            HttpContext.Current.Response.Write("可能网络错误，验证码识别为空");
            HttpContext.Current.Response.End();
            return;
        }

        CBA.sCheckType = sType;//设置查询类型 0=域名模式,1=备案号模式
        //CBA.sCheckType = "1";
        CBA.GetBeinAnInfo(sValue);//开始查询
        if (CBA.BeErr)//获取后报错,重试
        {
            goto ReGetChapcha;
        }
        else//结果正常，输出前最好对结果进行HTML过滤操作
        {

            HttpContext.Current.Response.Write(RemoveHtml(CBA.sBeiAn, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sDomain, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sICPNum, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sOwnerName, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sType, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sSiteName, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sSiteUrl, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sAddTime, false));
            HttpContext.Current.Response.Write(RemoveHtml(CBA.sBeiAnID, false));
            HttpContext.Current.Response.End();
        }
        
    }

    [DllImport("baidu.dll")]
    private static extern int loadcode(int a, int b, string c, string d);
    [DllImport("baidu.dll")]
    private static extern int Recognitiond(int a, int b, int c, string d, string e);
    public static void Init(String runtimedatPath)
    {
        string c = runtimedatPath;
        string str = "AvX";
        string str2 = "v5d4";
        string str3 = ",m3";
        string d = str + str2 + str3 + "N";
        int num2;
        try
        {
            int num = loadcode(0, 0, c, d);
            num2 = num;
        }
        catch (Exception ex)
        {
            num2 = -1;
            HttpContext.Current.Response.Write(ex.Message);
        }
    }

    public static string RemoveHtml(string sValue,bool isEnd)
    {
        sValue = sValue.Replace("&nbsp;", "");
        if (isEnd)
        {
            return sValue;
        }
        else
        {
            return sValue + "\r\n";
        }
    }
}
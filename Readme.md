###代码改动

客户端程序和Web程序在调用C++库baidu.dll的底层机制上,有些差异,因此修改了ClassBeiAn的部分代码,将其作为解决方案的一个项目.

###接口调用方式

使用POST请求Query.aspx,获取纯文本文件的输出

```
POST http://localhost:8090/Query.aspx HTTP/1.1
Accept: text/plain
Content-Type: application/x-www-form-urlencoded
Host: localhost:8090

sValue=126.com&sType=0&sName=江苏
```

###调试注意事项
1. 确保web.config中RuntimePath路径和文件的存在
2. 确保web进程的用户对web.config中ChapchaPath路径有读写权限
3. 相关的dll,exe,dat,CLL以及授权加密文件均放置在站点的Bin目录下,并在系统环境变量PATH中添加该Bin目录


###服务器部署注意事项
1. 设置应用程序池的账号为本地管理员
2. 和调试的1,2,3注意事项相同
3. 完成以上事项后,如果访问接口仍然有问题(比如无响应),尝试将相关的CLL文件复制到%windir%\System32\inetsrv目录下(32bit os),或者%windir%\SysWOW64\inetsrv目录下(64bit os)
4. IIS的其它设置：
  - 由于验证码识别库是32bit的dll,因此需确保64bit的os运行32bit v2.0的asp.net,方法如下:
	1. 单击“开始”，单击“运行”，键入 cmd，然后单击“确定”。
	2. 键入以下命令启用 32 位模式：
	
	```
	cscript %SYSTEMDRIVE%/inetpub/adminscripts/adsutil.vbs SET W3SVC/AppPools/Enable32bitAppOnWin64 1
	```
	
	3. 键入以下命令，安装 ASP.NET 2.0（32 bit）版本并在 IIS 根目录下安装脚本映射：
	
	```
	%SYSTEMROOT%/Microsoft.NET/Framework/v2.0.50727/aspnet_regiis.exe -i
	```
	
	4. 确保在 Internet 信息服务管理器的 Web 服务扩展列表中，将 ASP.NET 版本 2.0.40607（32 bit）的状态设置为允许。

  - 如果是32bit的os,上面的步骤可以省略

  - 确认IIS中,aspx的isapi扩展对应的文件为C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\aspnet_isapi.dll,默认为v4.0的版本(如果安装了.net framework4.0之后)

###服务错误调试工具
使用Procmon.exe查看相关的dll,CLL文件是否可以被loadimage顺利加载

###相关的参考
http://blog.csdn.net/wildboy2001/article/details/5792804
http://wenku.baidu.com/view/af35cd07eff9aef8941e0606.html
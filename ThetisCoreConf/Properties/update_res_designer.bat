' https://msdn.microsoft.com/ja-jp/library/ccec7sz1(v=vs.110).aspx#Strong

set PATH=C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools
resgen.exe .\Resources.en-US.resx /str:c#,ThetisCore.Conf.Properties,Resources,Resources.en-US.Designer.cs /publicClass

@echo *******************************************************
@echo *   Replace 'internal Resources()' with 'public ..'   *
@echo *******************************************************

@echo off
pause

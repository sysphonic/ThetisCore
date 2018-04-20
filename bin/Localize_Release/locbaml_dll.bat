REM http://msdn.microsoft.com/en-us/library/ms788718.aspx

REM For Resourcefiles + XAML Localization
REM http://www.codeproject.com/KB/WPF/Localization_in_WPF.aspx

REM **** USE SDK commandline. ****

copy LocBaml.exe ..\Release\ja-JP
copy ..\Release\ThetisCoreNavi.exe ..\Release\ja-JP
copy ..\Release\ThetisCoreConf.exe ..\Release\ja-JP
copy ..\Release\ThetisBrowser.exe ..\Release\ja-JP
copy ..\Release\ThetisExport.exe ..\Release\ja-JP
copy ..\Release\ZeptairClientConf.exe ..\Release\ja-JP
REM copy ..\Release\en-US\ThetisCoreNavi.resources.dll ..\Release\ja-JP
REM copy ..\Release\en-US\ThetisCoreConf.resources.dll ..\Release\ja-JP
REM copy ..\Release\en-US\ThetisBrowser.resources.dll ..\Release\ja-JP
REM copy ..\Release\en-US\ThetisExport.resources.dll ..\Release\ja-JP
REM copy ..\Release\en-US\ZeptairClientConf.resources.dll ..\Release\ja-JP
cd ..\Release\ja-JP

REM resgen ..\..\..\ThetisCoreNavi\Properties\Resources.ja-JP.resx
REM copy ..\..\..\ThetisCoreNavi\Properties\Resources.ja-JP.resources .\ThetisCore.Navi.Properties.Resources.ja-JP.resources

REM ************ ThetisCoreTask ************
copy ..\..\..\ThetisCoreTask\obj\Release\ThetisCore.Task.Properties.Resources.ja-JP.resources .
al /template:"ThetisCoreTask.exe" /embed:ThetisCore.Task.Properties.Resources.ja-JP.resources /culture:ja-JP /out:.\ThetisCoreTask.resources.dll
del ThetisCore.Task.Properties.Resources.ja-JP.resources

REM ************ ThetisCoreNavi ************
copy ..\..\..\ThetisCoreNavi\obj\Release\ThetisCoreNavi.g.en-US.resources .
LocBAML /generate ThetisCoreNavi.g.en-US.resources /trans:ThetisCoreNavi.csv /out:. /cul:ja-JP
copy ..\..\..\ThetisCoreNavi\obj\Release\ThetisCore.Navi.Properties.Resources.ja-JP.resources .
al /template:"ThetisCoreNavi.exe" /embed:ThetisCoreNavi.g.ja-JP.resources /embed:ThetisCore.Navi.Properties.Resources.ja-JP.resources /culture:ja-JP /out:.\ThetisCoreNavi.resources.dll
del ThetisCoreNavi.g.en-US.resources
del ThetisCoreNavi.g.ja-JP.resources
del ThetisCore.Navi.Properties.Resources.ja-JP.resources

REM ************ ThetisCoreConf ************
copy ..\..\..\ThetisCoreConf\obj\Release\ThetisCoreConf.g.en-US.resources .
LocBAML /generate ThetisCoreConf.g.en-US.resources /trans:ThetisCoreConf.csv /out:. /cul:ja-JP
copy ..\..\..\ThetisCoreConf\obj\Release\ThetisCore.Conf.Properties.Resources.ja-JP.resources .
al /template:"ThetisCoreConf.exe" /embed:ThetisCoreConf.g.ja-JP.resources /embed:ThetisCore.Conf.Properties.Resources.ja-JP.resources /culture:ja-JP /out:.\ThetisCoreConf.resources.dll
del ThetisCoreConf.g.en-US.resources
del ThetisCoreConf.g.ja-JP.resources
del ThetisCore.Conf.Properties.Resources.ja-JP.resources

REM ************ ThetisBrowser ************
copy ..\..\..\ThetisBrowser\obj\Release\ThetisBrowser.g.en-US.resources .
LocBAML /generate ThetisBrowser.g.en-US.resources /trans:ThetisBrowser.csv /out:. /cul:ja-JP
copy ..\..\..\ThetisBrowser\obj\Release\ThetisBrowser.Properties.Resources.ja-JP.resources .
REM resgen ..\..\..\ThetisBrowser\Properties\Resources.ja-JP.resx
REM copy ..\..\..\ThetisBrowser\Properties\Resources.ja-JP.resources .\ThetisBrowser.Properties.Resources.ja-JP.resources
al /template:"ThetisBrowser.exe" /embed:ThetisBrowser.g.ja-JP.resources /embed:ThetisBrowser.Properties.Resources.ja-JP.resources /culture:ja-JP /out:.\ThetisBrowser.resources.dll
del ThetisBrowser.g.en-US.resources
del ThetisBrowser.g.ja-JP.resources
del ThetisBrowser.Properties.Resources.ja-JP.resources

REM ************ ThetisExport ************
copy ..\..\..\ThetisExport\obj\Release\ThetisExport.g.en-US.resources .
LocBAML /generate ThetisExport.g.en-US.resources /trans:ThetisExport.csv /out:. /cul:ja-JP
copy ..\..\..\ThetisExport\obj\Release\ThetisExport.Properties.Resources.ja-JP.resources .
al /template:"ThetisExport.exe" /embed:ThetisExport.g.ja-JP.resources /embed:ThetisExport.Properties.Resources.ja-JP.resources /culture:ja-JP /out:.\ThetisExport.resources.dll
del ThetisExport.g.en-US.resources
del ThetisExport.g.ja-JP.resources
del ThetisExport.Properties.Resources.ja-JP.resources

REM ************ ZeptairClientConf ************
copy ..\..\..\ZeptairClientConf\obj\Release\ZeptairClientConf.g.en-US.resources .
LocBAML /generate ZeptairClientConf.g.en-US.resources /trans:ZeptairClientConf.csv /out:. /cul:ja-JP
copy ..\..\..\ZeptairClientConf\obj\Release\Zeptair.ClientConf.Properties.Resources.ja-JP.resources .
al /template:"ZeptairClientConf.exe" /embed:ZeptairClientConf.g.ja-JP.resources /embed:Zeptair.ClientConf.Properties.Resources.ja-JP.resources /culture:ja-JP /out:.\ZeptairClientConf.resources.dll
del ZeptairClientConf.g.en-US.resources
del ZeptairClientConf.g.ja-JP.resources
del Zeptair.ClientConf.Properties.Resources.ja-JP.resources

del LocBaml.exe
del ThetisCoreNavi.exe
del ThetisCoreConf.exe
del ThetisBrowser.exe
del ThetisExport.exe
del ZeptairClientConf.exe

cd "..\..\Localize_Release"

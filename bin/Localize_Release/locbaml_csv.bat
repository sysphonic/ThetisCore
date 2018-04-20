REM http://msdn.microsoft.com/en-us/library/ms788718.aspx

copy LocBaml.exe ..\Release\en-US
copy ..\Release\ThetisCoreNavi.exe ..\Release\en-US
copy ..\Release\ThetisCoreConf.exe ..\Release\en-US
copy ..\Release\ThetisBrowser.exe ..\Release\en-US
copy ..\Release\ThetisExport.exe ..\Release\en-US
copy ..\Release\ZeptairClientConf.exe ..\Release\en-US
cd ..\Release\en-US
LocBaml /parse ThetisCoreNavi.resources.dll /out:ThetisCoreNavi.csv
LocBaml /parse ThetisCoreConf.resources.dll /out:ThetisCoreConf.csv
LocBaml /parse ThetisBrowser.resources.dll /out:ThetisBrowser.csv
LocBaml /parse ThetisExport.resources.dll /out:ThetisExport.csv
LocBaml /parse ZeptairClientConf.resources.dll /out:ZeptairClientConf.csv
del LocBaml.exe
del ThetisCoreNavi.exe
del ThetisCoreConf.exe
del ThetisBrowser.exe
del ThetisExport.exe
del ZeptairClientConf.exe

cd "..\..\Localize_Release"

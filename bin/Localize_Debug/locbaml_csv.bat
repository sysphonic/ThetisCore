REM http://msdn.microsoft.com/en-us/library/ms788718.aspx

copy LocBaml.exe ..\Debug\en-US
copy ..\Debug\ThetisCoreNavi.exe ..\Debug\en-US
copy ..\Debug\ThetisCoreConf.exe ..\Debug\en-US
copy ..\Debug\ThetisBrowser.exe ..\Debug\en-US
copy ..\Debug\ThetisExport.exe ..\Debug\en-US
copy ..\Debug\ZeptairClientConf.exe ..\Debug\en-US
cd ..\Debug\en-US
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

cd "..\..\Localize_Debug"

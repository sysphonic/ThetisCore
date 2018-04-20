REM http://msdn.microsoft.com/en-us/library/ms788718.aspx

cd ..\..\ThetisCoreNavi
msbuild /t:updateuid ThetisCoreNavi.csproj
msbuild /t:checkuid ThetisCoreNavi.csproj
cd ..\ThetisCoreConf
msbuild /t:updateuid ThetisCoreConf.csproj
msbuild /t:checkuid ThetisCoreConf.csproj
cd ..\ThetisBrowser
msbuild /t:updateuid ThetisBrowser.csproj
msbuild /t:checkuid ThetisBrowser.csproj
cd ..\ThetisExport
msbuild /t:updateuid ThetisExport.csproj
msbuild /t:checkuid ThetisExport.csproj
cd ..\ZeptairClientConf
msbuild /t:updateuid ZeptairClientConf.csproj
msbuild /t:checkuid ZeptairClientConf.csproj

cd ..\bin\Localize_Release

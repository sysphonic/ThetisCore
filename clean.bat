@echo off

rem del /S /Q /F .\*.csproj.user
del /S /Q /F .\*.suo
rd /S /Q .\.vs
rd /S /Q .\HTMLtoXAMLConverter\bin
rd /S /Q .\HTMLtoXAMLConverter\obj
rd /S /Q .\RSS.NET\bin
rd /S /Q .\RSS.NET\obj
rd /S /Q .\ThetisCoreConf\obj
rd /S /Q .\ThetisCoreLib\obj
rd /S /Q .\ThetisCoreNavi\obj
rd /S /Q .\ThetisCoreTask\obj

cd .\bin\Release
del *.exe.config *.pdb *.exe.manifest log4net.xml *.vshost.exe
cd ..\..
cd .\bin\Debug
del *.exe.config *.pdb *.exe.manifest log4net.xml *.vshost.exe
cd ..\..


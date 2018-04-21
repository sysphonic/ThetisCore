@echo off
set TARGET="C:\ThetisCore"

if not exist "%TARGET%\" (
  mkdir "%TARGET%"
)

copy *.exe "%TARGET%"
copy *.dll "%TARGET%"
copy LICENSE* "%TARGET%"
copy START* "%TARGET%"

if not exist "%TARGET%\data\" (
  xcopy /Y /E ".\data" "%TARGET%\data\"
)
if not exist "%TARGET%\data_common\" (
  xcopy /Y /E ".\data_common" "%TARGET%\data_common\"
)
xcopy /Y /E ".\en-US" "%TARGET%\en-US\"
xcopy /Y /E ".\ja-JP" "%TARGET%\ja-JP\"

del "%TARGET%\en-US\*.csv"
del "%TARGET%\ja-JP\*.csv"


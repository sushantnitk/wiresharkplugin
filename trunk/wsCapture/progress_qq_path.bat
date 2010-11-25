@echo on
rem 使用WMIC获取进程信息中的QQ路径
for /f "tokens=2 delims==" %%a in ('wmic process where "name='tm.exe'" get executablepath /value') do (
  set QQPath=%%a
)
echo %QQPath%
pause
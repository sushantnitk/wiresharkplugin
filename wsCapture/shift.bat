:loop
@echo off&echo\
set num=
set num=%num%
echo 没移位的参数为 : 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20
echo\&echo 参数 1234 始终没有变化，但后面的却在逐位变化，前移。
echo\
:: 作者：随风   @bbs.bathome.cn   2007-11-10
::
call :lis 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20
color 0b
echo\&echo 测试完毕,按任意键退出 ……
echo\&pause>nul&exit
:lis
set /p=%num%<nul
if "%5"=="" goto :eof
shift /5
set /p=移位后的参数为 : %1 %2 %3 %4 %5 %6 %7 %8 %9<nul
set /p=     按任意键继续......<nul
pause>nul
goto lis


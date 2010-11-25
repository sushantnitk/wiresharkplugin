@echo off
:begin
cls
set input=
set /p input=  请输入一个字符串：
if "%input%"=="" goto begin
echo.
echo %input%|findstr "^[0-9]*$">nul && echo   你输入的字符串是纯数字||(
    echo %input%|findstr "^[a-zA-Z]*$">nul && echo   你输入的字符串是纯字母||echo   你输入的字符串既不是纯数字也不是纯字母
)
echo.
pause
goto begin



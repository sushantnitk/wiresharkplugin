@echo off
echo *******************************************************************************
@echo 2010.10.20 数据包Gi采集
echo *******************************************************************************
if exist e:\Progra~1\Wireshark\tshark.exe goto e
if exist f:\Progra~1\Wireshark\tshark.exe goto f
if exist c:\Progra~1\Wireshark\tshark.exe goto c
if exist d:\Progra~1\Wireshark\tshark.exe goto d
:c
c:
cd c:\Program Files\Wireshark 
goto tshark
:d
d:
cd d:\Program Files\Wireshark 
goto tshark
:e
e:
cd e:\Program Files\Wireshark 
goto tshark
:f
f:
cd f:\Program Files\Wireshark 
goto tshark
:tshark
tshark -D
echo ******************************************************************************* 
@echo 指定采集端口,有线端口用数字表示，无线端口用数字 -p表示
@echo 例如：3、2 -p
set/p port=
echo *******************************************************************************
echo 指定了采集端口*** %port% ***
echo 指定每个文件大小1.024G
echo 采集文件目录f盘
echo Ctrl+C停止
echo *******************************************************************************
tshark -i %port% -w f:\t.pcap -b filesize:1024000 
pause




　　
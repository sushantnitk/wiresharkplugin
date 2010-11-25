@echo off
@echo *******************************************************************************
@echo 2010.10.21 移动qq数据包Gi采集
@echo *******************************************************************************
if exist c:\Progra~1\Wireshark\tshark.exe goto c
if exist d:\Progra~1\Wireshark\tshark.exe goto d
if exist e:\Progra~1\Wireshark\tshark.exe goto e
if exist f:\Progra~1\Wireshark\tshark.exe goto f
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

:loop

@echo *******************************************************************************
@echo 指定采集端口
@echo 有线端口用数字表示，例如：3
@echo 无线端口用数字 -p表示，例如：2 -p
set port= 
set/p port=请指定采集端口:


@echo *******************************************************************************
@echo 指定了采集端口*** %port% ***
@echo 指定每个文件大小1.024G,采集文件目录f盘
@echo 过滤：TCP端口14000，UDP端口8040
@echo 过滤：TM 8000和4000
@echo 过滤：oicq协议过滤方法暂时不能使用
@echo Ctrl+C停止
@echo *******************************************************************************
tshark -i %port%  -w f:\t.pcap -b filesize:1024000 -f "tcp port 14000 or udp port 8040 or udp port 8000 or udp port 4000" 

pause
goto loop




　　
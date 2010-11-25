@echo off
echo *******************************************************************************
@echo 2010.10.19 移动qq数据包Gi采集
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
echo 指定采集端口3,观察端口是否指定正确，否则修改脚本指定正确的采集端口
echo 指定每个文件大小1.024G
echo 采集文件目录f盘
echo 过滤：TCP端口14000，UDP端口8040，
echo 过滤：TM 8000和4000
echo 过滤：oicq协议过滤方法暂时不能使用
echo *******************************************************************************
tshark -i 3 -w f:\t.pcap -b filesize:1024000 -f "tcp port 14000 or udp port 8040 or udp port 8000 or udp port 4000" 
pause




　　
@echo off
tshark     -i  3  -w  f:\t.pcap     -b  filesize:2048     -f  "src net 192.168" 

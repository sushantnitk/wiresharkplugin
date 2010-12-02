@echo on 

@echo 0.进入目录

cd K:\寻呼项目—范茂鑫\SZ_1201_08A

@echo 1.多台电脑上的pcap文件按照时间合并......

mergecap -v -T ether -w  mc.cap CE*.cap

@echo 2.分割成大小相同的文件......

editcap mc.cap -c 2000000 mc_mergefile.cap

@echo continue......

pause

@echo 3.重新组包......

D:\ScapyProject\mc_split_v2.py mc_mergefile_00000_20101129195854.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00001_20101129200721.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00002_20101129201534.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00003_20101129202352.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00004_20101129203212.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00005_20101129204028.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00006_20101129204846.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00007_20101129205705.cap

@echo 4.过滤h.248等不需要的文件......

tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00000_20101129195854.cap -w mc_merge_split_filter_0000.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00001_20101129200721.cap -w mc_merge_split_filter_0001.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00002_20101129201534.cap -w mc_merge_split_filter_0002.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00003_20101129202352.cap -w mc_merge_split_filter_0003.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00004_20101129203212.cap -w mc_merge_split_filter_0004.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00005_20101129204028.cap -w mc_merge_split_filter_0005.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00006_20101129204846.cap -w mc_merge_split_filter_0006.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00007_20101129205705.cap -w mc_merge_split_filter_0007.pcap

@echo convert finished!

pause



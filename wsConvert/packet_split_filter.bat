@echo on 


@echo 3.重新组包......

D:\ScapyProject\mc_split_v2.py mc_mergefile_00000_20101201195902.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00001_20101201200810.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00002_20101201201707.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00003_20101201202610.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00004_20101201203510.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00005_20101201204413.cap
D:\ScapyProject\mc_split_v2.py mc_mergefile_00006_20101201205317.cap
@echo D:\ScapyProject\mc_split_v2.py mc_mergefile_00007_20101129205705.cap

@echo 4.过滤h.248等不需要的文件......

tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00000_20101201195902.cap -w mc_merge_split_filter_0000.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00001_20101201200810.cap -w mc_merge_split_filter_0001.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00002_20101201201707.cap -w mc_merge_split_filter_0002.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00003_20101201202610.cap -w mc_merge_split_filter_0003.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00004_20101201203510.cap -w mc_merge_split_filter_0004.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00005_20101201204413.cap -w mc_merge_split_filter_0005.pcap
tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00006_20101201205317.cap -w mc_merge_split_filter_0006.pcap
@echo tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm_mc_mergefile_00007_20101129205705.cap -w mc_merge_split_filter_0007.pcap

@echo convert finished!

pause



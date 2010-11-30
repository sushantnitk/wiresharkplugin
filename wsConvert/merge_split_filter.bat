mergecap -v -T ether -w  e:\mc.cap e:\mc*.cap
e:
cd e:\
D:\ScapyProject\mc_split_v1.py mc.pcap

tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm.pcap -w mmm.pcap
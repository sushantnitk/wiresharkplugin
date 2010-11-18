# IP Address of the VM sending the upgrade
import scapy
from scapy import *

src="192.168.44.128"
f = "captured_firmware_upgrade.pcap"
pcap = rdpcap(f)


data = ""
for packet in pcap:
  il = packet.getlayer("IP")
  if il.src != src:
    continue
  tl = packet.getlayer("TCP")
  # check for data in the payload, if not skip the packet
  if isinstance(tl.payload,scapy.NoPayload):
    continue
  data += str(tl.payload)
# write our raw data file
f = open("raw_data.dat", 'w')
f.write(data)
f.close()

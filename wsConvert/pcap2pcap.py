import struct
fpcap = open('aa.pcap','rb')
ftxt = open('result.pcap','wb') #wb rb.....
string_data = fpcap.read()
pcap_header = {}
pcap_header['magic_number'] = string_data[0:4]
pcap_header['version_major'] = string_data[4:6]
pcap_header['version_minor'] = string_data[6:8]
pcap_header['thiszone'] = string_data[8:12]
pcap_header['sigfigs'] = string_data[12:16]
pcap_header['snaplen'] = string_data[16:20]
pcap_header['network'] = string_data[20:24]
for key in ['magic_number','version_major','version_minor','thiszone','sigfigs','snaplen','network']:
    ftxt.write(pcap_header[key])
i =24
step=0
while(i<len(string_data)):
      pcap_packet_header = {}
      pcap_packet_header['ts_sec'] = string_data[i:(i+4)]
      pcap_packet_header['ts_usec'] = string_data[(i+4):(i+8)]
      pcap_packet_header['incl_len'] = string_data[(i+8):(i+12)]
      pcap_packet_header['orig_len'] = string_data[(i+12):(i+16)]
      packet_len = struct.unpack('I',pcap_packet_header['incl_len'])[0]
      step = i+16+packet_len
      packet_data=string_data[(i+16):step]
      i=step
      for key in ['ts_sec','ts_usec','incl_len','orig_len']:
          ftxt.write(pcap_packet_header[key])
      ftxt.write(packet_data)
ftxt.close()
fpcap.close()

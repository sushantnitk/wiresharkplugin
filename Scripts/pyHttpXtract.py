#!/usr/bin/env python 
 
from optparse import OptionParser 
import sys 
import shutil 
import pcapy 
import impacket.ImpactDecoder as Decoders 
import impacket.ImpactPacket as Packets 
import os.path 
import os 
import chardet 
import string 
import gzip 

class pcapGzip: 
    def __init__(self, pcapfile, reportpath="./report"): 
        assert pcapfile 
        if not os.path.exists(pcapfile): 
            raise TypeError("Pcap file not found. Please check location.") 
        self.reportpath = reportpath 
        if not os.path.exists(self.reportpath): 
            os.makedirs(self.reportpath) 
        self.pcapfile = pcapfile 
    
    def uncompressGzip(self, file): 
        """Gunzip a gz file 
        """ 
        try: 
            r_file = gzip.GzipFile(file, 'r') 
            write_file = string.rstrip(file, '.gz') 
            w_file = open(write_file, 'w') 
            w_file.write(r_file.read()) 
            w_file.close() 
            r_file.close() 
            os.unlink(file) 
            print "Successfully uncompressed %s" % (file) 
        except: 
            print "***Error: Failed to uncompress %s" % (file) 

    def tagFiles(self): 
        """ Browses a given dir and tries to uncompress gz files 
        """ 
        listDir = os.listdir("report") 
        for f in listDir: 
            fullpath = os.path.join(self.reportpath, f) # full path without gz extension 
            if open(fullpath, 'r').read(2)=='\037\213': # magic number for application/x-gzip 
                os.rename(fullpath, fullpath+".gz")     # first give gz extension to gz files 
                self.uncompressGzip(fullpath+".gz")     # then uncompress gz files 
    
    def decodePayload(self, payload): 
        """Decode a payload from the parser and returns an array of lines 
        """ 
        decoder = Decoders.EthDecoder() 
        eth = decoder.decode(payload) 
        ip = eth.child() 
        tcp = ip.child() 
        try: 
            if tcp.get_RST()!=1: 
                data = tcp.get_data_as_string()                 # raw data 
                data = data.replace('\r\n', '\r\n###~~~###') 
                arrline = data.split('\r\n') 
                return arrline 
            else: 
                return None 
        except: 
            return None 

    def writeFile(self, f, content): 
        """Dump content in a file 
        """ 
        obFile = open(os.path.join(self.reportpath, f), 'a') 
        obFile.write(content) 
        obFile.close() 

    def decodeMac(self, mac): 
        """Decode mac address 
        """ 
        m = '' 
        for i in mac: 
            t = "%x" % i 
            if len(t)==1: 
                t = '0'+t 
            m=m+":"+t 
        return m[1:] 

    def createFlows(self): 
        """Create necessary flows based on pcap file 
        """ 
        print "running..." 
        self.writeFile("report.html", '<html>' 
            + '<head><style>td { font-size:8pt; }</style></head>' 
            + '<body><table border="1" style="width:1000px"><tr>' 
            + '<th style="width:100px">Num.</th>' 
            + '<th style="width:200px">Flow</th>' 
            + '<th style="width:600px;word-wrap:true">Request/Response</th>' 
            + '<th style="width:100px">Attachment</th>' 
            + '</tr>') 
        reader = pcapy.open_offline(self.pcapfile) 
        eth_decoder = Decoders.EthDecoder() 
        ip_decoder = Decoders.IPDecoder() 
        tcp_decoder = Decoders.TCPDecoder() 
        countPacket = 0 
        lastAttach = '' 
        ext = '' 
        (header, payload) = reader.next() 

        while payload!='':                  # no other way to stop pcapy loop? 
            countPacket+=1 
            try: 
                if countPacket%100==0: 
                    print "(%d packets already processed)" % countPacket 
                arrline = self.decodePayload(payload) 
                # If TCP flag RST, we skip the packet 
                if arrline: 
                    ethernet = eth_decoder.decode(payload) 
                    smac = self.decodeMac(ethernet.get_ether_shost()) 
                    dmac = self.decodeMac(ethernet.get_ether_dhost()) 
                    if ethernet.get_ether_type() == Packets.IP.ethertype:   # if IP packet 
                        ip = ip_decoder.decode(payload[ethernet.get_header_size():]) 
                        if ip.get_ip_p() == Packets.TCP.protocol:           # if TCP packet 
                            tcp = tcp_decoder.decode( 
                                payload[ethernet.get_header_size()+ip.get_header_size():]) 
                            ipsrc = ip.get_ip_src() 
                            ipdst = ip.get_ip_dst() 
                            sport = tcp.get_th_sport() 
                            dport = tcp.get_th_dport() 
                            sessionFile = "session-"+ipsrc+"."+str(sport)+"-"+ipdst+"."+str(dport) 
                            flow = ipsrc + ':' + str(sport) + '<br />(' + smac + ')' + '<br />-><br />' + ipdst + ':' + str(dport) + '<br />(' + dmac + ')' 
                            for line in arrline: 
                                if line.strip() != "": 
                                    if chardet.detect(line)['encoding'] == 'ascii': 
                                        line = line.replace('###~~~###', '') 
                                        if line.startswith("GET ") or line.startswith("HTTP/"): 
                                            if line.startswith("HTTP/"): # new file 
                                                packetnum = countPacket 
                                                self.writeFile("report.html", '<td>&nbsp</td>') 
                                            self.writeFile("report.html", '<tr><td>'+str(countPacket)+'</td>') 
                                            self.writeFile("report.html", '<td>'+flow+'</td><td>') 
                                        if line.startswith("Content-Type"): 
                                            style = ' style="background:#ffff00"' 
                                            ext = '.'+line.split("/")[1].split(";")[0] 
                                            if ext == '.gzip': 
                                                ext = '.gz' 
                                        else: 
                                            style = '' 
                                        self.writeFile("report.html", '<div'+style+'>'+line+'</div>') 
                                    else: # raw data 

                                        if sessionFile + "-" + str(packetnum) + ext != lastAttach: 
                                            # New file 
                                            line = line.replace('###~~~###', '') 
                                            lastAttach = sessionFile + "-" + str(packetnum) + ext 
                                            self.writeFile("report.html",'</td><td align="center"><a href="' 
                                                + sessionFile + "-" + str(packetnum) + ext + '">') 
                                            if ext==".jpeg" or ext==".gif": 
                                                self.writeFile("report.html",'<img src="' 
                                                    + sessionFile + "-" + str(packetnum) + ext 
                                                    + '" border="2" style="width:100px;" />') 
                                            else: 
                                                self.writeFile("report.html",'<div style="background:#ff0000;color:#fff;font-weight:bold;width:50px;text-align:center">' 
                                                    + ext[1:] + '</div>') 
                                            self.writeFile("report.html", '</a></td></tr>') 
                                        else: 
                                            line = line.replace('###~~~###', '\r\n') 
                                        # Content of the file 
                                        self.writeFile(sessionFile + "-" + str(packetnum) + ext, line)       # raw data 
                (header, payload) = reader.next() 
            except: 
                break 

        print "\n%d have been detected in this pcap file" % countPacket 
        self.writeFile("report.html", "</table>\n%d have been detected in this pcap file</body></html>" % countPacket) 

if __name__ == '__main__': 
    usage = "usage: %prog -r <file> [options]" 
    parser = OptionParser(usage) 
    parser.add_option("-r", "--read-file", dest="pcapfile", 
        help="Capture file to process (pcap format)") 
    parser.add_option("-o", '--output', dest="output_directory", default="./report", 
        help="Reporting directory (default: ./report/)") 
    parser.add_option("-f", '--force', dest="force", default=False, action="store_true", 
        help="Force overwriting of files") 
    (options, args) = parser.parse_args(sys.argv) 
    if not options.pcapfile: 
        parser.error("Capture file is missing. Use -r <file>.") 
    if options.output_directory and os.path.isfile(options.output_directory): 
        parser.error("Use a different name for output directory since it is already used for a file") 
    if options.output_directory and os.path.isdir(options.output_directory) and not options.force: 
        if os.listdir(options.output_directory): 
            parser.error("Output directory is not empty. Use -f to overwrite content") 
    if options.force: 
        shutil.rmtree(options.output_directory, ignore_errors=True) 
    
    p = pcapGzip(options.pcapfile, options.output_directory) 
    p.createFlows() 
    p.tagFiles() 
    del p

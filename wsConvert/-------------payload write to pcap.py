


参考mc 文件分割，

读取每个消息包，

把payload写入 tcp.data



    start = time.time()
    for i in xrange(cnt):
        ip = ImpactPacket.IP()
        ip.set_ip_src('1.2.3.4')
        ip.set_ip_dst('5.6.7.8')
        udp = ImpactPacket.UDP()
        udp.set_uh_sport(111)
        udp.set_uh_dport(222)
        udp.contains(ImpactPacket.Data(data))
        ip.contains(udp)
        ip.get_packet()
    print 'impacket:', cnt / (time.time() - start), 'pps'

    start = time.time()
    for i in xrange(cnt):
        p = packet.createPacket(packet.IP, packet.UDP)
        p['ip'].src = '1.2.3.4'
        p['ip'].dst = '5.6.7.8'
        p['udp'].sport = 111
        p['udp'].dport = 22
        p['udp'].payload = data
        p.finalise()
        p.getRaw()
    print 'openbsd.packet:', cnt / (time.time() - start), 'pps'
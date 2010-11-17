-- The following exaple inverts the src and dst columns in wireshark extracting the values with FieldExtractor
do
    local ip_src = FieldExtractor.new("ip.src")
    local ip_dst = FieldExtractor.new("ip.dst")
    local stupid_joke_tap = Tap.new("stupid_joke")
    stupid_joke_tap:use_fields(ip_src,ip_dst)
    stupid_joke_tap:register()
        function per_packet.stupid_joke(pinfo)
                col_src = pinfo:col.src
                col_dst = pinfo:col.dst
                col_src:set(ip_dst:get())
                col_dst:set(ip_src:get())
        end
end
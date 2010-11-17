-- register http to handle ports 4888-4891
do
        local tcp_port_table = DissectorTable.get("tcp.port")
        local http_dissector = tcp_port_table:get_dissector(80)
        for port in {4888,4889,4890,4891} do
                tcp_port_table:add(port,http_dissector)
        end
end
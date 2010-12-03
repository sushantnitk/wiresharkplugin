-- from http://www.wireshark.org/lists/wireshark-users/200611/msg00011.html


--  Lua Response Time Monitor
--
--  Logs slow response times for HTTP and MS-SQL RPCs
--  Turn off TCP reassembly of pdu for the TDS tap

trigger = 20   -- log requests that take more that trigger seconds
logfile = "wb"..os.date("%Y%m%d")..".csv"
io.output(logfile)
io.write("Timestamp,Protocol,Session,Request,Info,Duration\n")


-- Define the field extractors that will be used
ipsrc=Field.new("ip.src")
ipdst=Field.new("ip.dst")
tcpsrc=Field.new("tcp.srcport")
tcpdst=Field.new("tcp.dstport")
http_request_extractor = Field.new("http.host")
http_uri_extractor = Field.new("http.request.uri")
http_method_extractor = Field.new("http.request.method")
http_code_extractor = Field.new("http.response.code")

-- HTTP Processing
http = Listener.new("http");
http_reqs = {} -- outstanding http requests
http_start = {} -- http request timestamp

function http.reset()
	http_reqs = {}
	http_start = {}
end

function http.packet(pinfo)
    local ip_src, ip_dst = ipsrc(), ipdst()
	local tcp_src, tcp_dst = tcpsrc(), tcpdst()
	local http_request = http_request_extractor()
	local http_method = http_method_extractor()
	local http_uri = http_uri_extractor()
	local http_code = http_code_extractor()
	local conv_key, timestamp

	if http_request then
		conv_key =  tostring(ip_dst) .. ":" .. tostring(tcp_dst) .. " " ..  tostring(ip_src) .. ":" .. tostring(tcp_src)
		http_reqs[conv_key] = tostring(http_method).." "..tostring(http_uri)
		http_start[conv_key] = pinfo.abs_ts
	else
		conv_key =  tostring(ip_src) .. ":" .. tostring(tcp_src) .. " " ..  tostring(ip_dst) .. ":" .. tostring(tcp_dst)
		if http_reqs[conv_key] then
			if pinfo.abs_ts - http_start[conv_key]>trigger then
				timestamp = os.date("%c",http_start[conv_key]) .. "." ..  string.sub(tostring(http_start[conv_key] - math.floor(http_start[conv_key])),3,5)

				io.write(timestamp,",HTTP,",conv_key,",",http_reqs[conv_key],",",tostring(http_code),",",tostring(pinfo.abs_ts - http_start[conv_key]),"\n")
			end
			--if tostring(http_code) ~= "100" then
			--	http_reqs[conv_key] = nil
			--	http_start[conv_key] = nil
			--end
		end
	end
	return true
end

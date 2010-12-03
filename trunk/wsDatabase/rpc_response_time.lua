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
ip_addr_extractor = Field.new("ip.addr")
tcp_port_extractor = Field.new("tcp.port")
http_request_extractor = Field.new("http.request")
http_uri_extractor = Field.new("http.request.uri")
http_method_extractor = Field.new("http.request.method")
http_code_extractor = Field.new("http.response.code")


-- HTTP Processing
http = Tap.new("http","http.request || http.response");
http_reqs = {} -- outstanding http requests
http_start = {} -- http request timestamp

function http.reset()
	http_reqs = {}
	http_start = {}
end

function http.packet(pinfo)
	local ip_src, ip_dst = ip_addr_extractor()
	local tcp_src, tcp_dst = tcp_port_extractor()
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
			if tostring(http_code) ~= "100" then
				http_reqs[conv_key] = nil
				http_start[conv_key] = nil
			end
		end
	end
	return true
end


-- TDS (MS-SQL RPC) Processing
tds = Tap.new("tds","tds.type == 0x03 || tds.type == 0x04");
tds_reqs = {} -- outstanding tds requests
tds_start = {} -- tds request timestamp

function tds.reset()
	tds_reqs = {}
	tds_start = {}
end

function tds.packet(pinfo,tvb)
	local ip_src, ip_dst = ip_addr_extractor()
	local tcp_src, tcp_dst = tcp_port_extractor()
	local conv_key, timestamp
	local tds_rpclen, tds_rpcname
	local i
	tds_type = tvb(54,1):uint()

	if tds_type == 3 then
		tds_rpclen = 64 + 2 * tvb(62,2):le_uint()
		tds_rpcname = ""
		for i = 64, tds_rpclen, 2 do
			tds_rpcname = tds_rpcname .. tvb(i,1):string()
		end
		conv_key =  tostring(ip_dst) .. ":" .. tostring(tcp_dst) .. " " ..  tostring(ip_src) .. ":" .. tostring(tcp_src)
		tds_reqs[conv_key] = tds_rpcname
		tds_start[conv_key] = pinfo.abs_ts
	else
		conv_key =  tostring(ip_src) .. ":" .. tostring(tcp_src) .. " " ..  tostring(ip_dst) .. ":" .. tostring(tcp_dst)
		if tds_reqs[conv_key] then
			if pinfo.abs_ts - tds_start[conv_key]>trigger then
				timestamp = os.date("%c",tds_start[conv_key]) .. "." ..  string.sub(tostring(tds_start[conv_key] - math.floor(tds_start[conv_key])),3,5)

				io.write(timestamp,",TDS,",conv_key,",",tds_reqs[conv_key],",Parms,",tostring(pinfo.abs_ts - tds_start[conv_key]),"\n")
			end
			tds_reqs[conv_key] = nil
			tds_start[conv_key] = nil
		end
	end
	return true
end

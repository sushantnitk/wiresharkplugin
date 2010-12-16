-- time to resolve host, time to ping host, time to get host/page
print ("\"page name\",\"host latency ms\",\"dns resolve ms\",\"page receive ms\"")
-- fromCSV from http://www.lua.org/pil/20.4.html
function fromCSV (s)
	s = s .. ','        -- ending comma
	local t = {}        -- table to collect fields
	local fieldstart = 1
	repeat
		-- next field is quoted? (start with `"'?)
		if string.find(s, '^"', fieldstart) then
			local a, c
			local i  = fieldstart
			repeat
				-- find closing quote
				a, i, c = string.find(s, '"("?)', i+1)
			until c ~= '"'    -- quote not followed by quote?
			if not i then error('unmatched "') end
			local f = string.sub(s, fieldstart+1, i-1)
			table.insert(t, (string.gsub(f, '""', '"')))
			fieldstart = string.find(s, ',', i) + 1
		else                -- unquoted; find next comma
			local nexti = string.find(s, ',', fieldstart)
			table.insert(t, string.sub(s, fieldstart, nexti-1))
			fieldstart = nexti + 1
		end
	until fieldstart > string.len(s)
	return t
end

require("cURL")
require("socket")
while (6 < 9) do
file_config = assert(io.open("config.csv", "r"))
for line in file_config:lines() do
	t = fromCSV(line)
	os.execute("ping -c 1 "..t[2].." | grep round > ping_result.txt")
	file_ping = assert(io.open("ping_result.txt", "r"))
	line=file_ping:read()
	if line == nil then
		ping_time="no response"
	else
		start=string.find(line,"/",32)
		ping_time=string.sub(line,33,start-5)
		file_ping:close()
	end
	begin_time=socket.gettime()
	master= socket.dns.toip(t[2])
	if master ==nil then
		dns_time=("can't resolve")
	else
		dns_time=(socket.gettime()-begin_time)
		file_page = io.open("mcjk", "w")
		c = cURL.easy_init()
		c:setopt_url("http://"..t[2]..t[3])
		begin_time=socket.gettime()
		c:perform({writefunction = function(str)
			file_page:write(str)
		end})
	page_retrieve_time=(socket.gettime()-begin_time)
	file_page:close()
	end
	print (string.format ("\"%s\",\"%.0f\",\"%.4f\",\"%.3f\""
	,t[1],ping_time,dns_time,page_retrieve_time))
end
file_config:close()
end

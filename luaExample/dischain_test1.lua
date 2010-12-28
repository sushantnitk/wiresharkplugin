require('dischain')

-- Test Protocol
do
	--! Declaration of "Test Protocol"
	ptest1 = Proto("ptest1","Test Protocol 1")

	--! Preferences for "Test Protocol"
	ptest1.prefs.port = Pref.uint("Port", 30000, "UDP port to monitor")

	--! Current port
	local curr_port = -1
	
	--! @brief Initializes the Test Protocol dissector (called at startup and upon change in preferences)
	function ptest1.init ()
		
		local port = ptest1.prefs.port
		
		-- only register if the values are different than current
		if (port >= 0 and port ~= curr_port) then
		
			local distab = DissectorTable.get("udp.port")
			
			-- unregister the current port before registering the new one
			dischain_unreg_ports(ptest1, distab, curr_port)
			dischain_reg_ports(ptest1, distab, port)
			
			curr_port = port
		end
	end

	--! @brief Determines if a buffer contains a packet for this Test Protocol
	--!
	--! @param tvb		Frame buffer to evaluate
	--!
	--! @return true or false
	local function is_ptest1 (tvb)
		local buflen = tvb:len()
		if buflen < 2 then 
			return false 
		else
			local ver = tvb(0,1):uint()
			if ver ~= 30 then return false end
		end
		
		return true
	end
	
	--! @brief Test Protocol dissector
	--!
	--! @param tvb		Frame buffer to be dissected
	--! @param pinfo	Packet information
	--! @param tree		Protocol tree to display dissected values
	function ptest1.dissector (tvb, pinfo, root) 
		if not is_ptest1(tvb) then
			debug("ptest1 #" .. pinfo.number .. ", dropped")
		else
			dischain_consume_packet(ptest1, pinfo.number)
			
			pinfo.cols.protocol = ptest1.name
			root:add(ptest1,tvb())
			pinfo.cols.info:set(ptest1.name .. " *** ")
		end
	end
end

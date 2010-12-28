require('dischain')

-- Test Protocol
do
	--! Declaration of "Test Protocol"
	ptest2 = Proto("ptest2","Test Protocol 2")

	--! Preferences for "Test Protocol"
	ptest2.prefs.port = Pref.uint("Port", 30000, "UDP port to monitor")

	--! Current port
	local curr_port = -1
	
	--! @brief Initializes the Test Protocol dissector (called at startup and upon change in preferences)
	function ptest2.init ()
		
		local port = ptest2.prefs.port
		
		-- only register if the values are different than current
		if (port >= 0 and port ~= curr_port) then
		
			local distab = DissectorTable.get("udp.port")
			
			-- unregister the current port before registering the new one
			dischain_unreg_ports(ptest2, distab, curr_port)
			dischain_reg_ports(ptest2, distab, port)
			
			curr_port = port
		end
	end

	--! @brief Determines if a buffer contains a packet for this Test Protocol
	--!
	--! @param tvb		Frame buffer to evaluate
	--!
	--! @return true or false
	local function is_ptest2 (tvb)
		local buflen = tvb:len()
		if buflen < 2 then 
			return false 
		else
			local ver = tvb(0,1):uint()
			if ver ~= 5 then return false end
		end
		
		return true
	end
	
	--! @brief Test Protocol dissector
	--!
	--! @param tvb		Frame buffer to be dissected
	--! @param pinfo	Packet information
	--! @param tree		Protocol tree to display dissected values
	function ptest2.dissector (tvb, pinfo, root) 
		if not is_ptest2(tvb) then
			debug("ptest2 #" .. pinfo.number .. ", dropped")
		else
			dischain_consume_packet(ptest2, pinfo.number)
			
			pinfo.cols.protocol = ptest2.name
			root:add(ptest2,tvb())
			pinfo.cols.info:set(ptest2.name .. " *** ")
		end
	end
end

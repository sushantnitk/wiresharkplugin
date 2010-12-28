-------------------------------------------------------------------
--! @file	dischain.lua
--! @author	Anthony K. Trinh
--! @see	http://dischain.googlecode.com
--! @brief	Lua Sub-dissector Chain. Allows multiple sub-dissectors 
--! 		to be registered with a UDP port.
--! @note	New BSD License:
--!
--! Copyright (c) 2010, Anthony K. Trinh
--! All rights reserved.
--! 
--! Redistribution and use in source and binary forms, with or without 
--! modification, are permitted provided that the following conditions 
--! are met:
--!
--!     * Redistributions of source code must retain the above copyright 
--!       notice, this list of conditions and the following disclaimer.
--!
--!     * Redistributions in binary form must reproduce the above copyright 
--!       notice, this list of conditions and the following disclaimer in 
--!       the documentation and/or other materials provided with the 
--!       distribution.
--!
--!     * The names of its contributors may not be used to endorse or 
--!       promote products derived from this software without specific prior
--!       written permission.
--! 
--! THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
--! "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
--! TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
--! PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
--! CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
--! EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
--! PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
--! OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
--! WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
--! OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
--! ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
--!
-------------------------------------------------------------------

do
	--! Declaration of Lua Sub-dissector Chain
	dischain = Proto("dischain", "Lua Sub-dissector Chain")
	
	--! Protocol Preferences
	dischain.prefs.data_dsctr_fallback = Pref.bool("Use Data dissector for unknown packets", false, "Pass unrecognized packets to Wireshark's default Data dissector")
	dischain.prefs.priodlist = Pref.string("Prioritized dissectors", "ptest1,ptest2", "Comma-separated short names of protocol dissectors that get \"first dibs\" on each packet")
	
	--! Results from sub-dissector for each packet, list index corresponds to the packet number
	local dischain_results = {}
	
	--! Array of stack lists, each list contains dissectors (of type Proto), list index corresponds to port number
	--! Example code: lopri_subdis[port][proto]
	local lopri_subdis = {}
	local hipri_subdis = {}
	
	--! List of sub-dissectors to start dissection of each packet (listed in order of priority)
	local prio_dlist = {}
	
	--! @brief	Finds the list index of an element
	--!
	--! @param lst	List to search
	--! @param val	Value to be found
	--! @return nil if not found; otherwise, non-zero list index of element
	local function list_indexof (lst, val)
		if lst then
			for i,v in ipairs(lst) do
				if v == val then
					-- non-zero index of found element
					return i
				end
			end
		end
		return nil
	end
	
	--! @brief Counts the elements in a list
	--!
	--! @param lst	List to evaluate
	--! @return number of elements in the list
	local function list_count (lst)
		local c = 0
		if lst then
			for d in pairs(lst) do
				if d then 
					c = c + 1	
				end
			end
		end
		return c
	end

	--! @brief	Looks up a Proto or sub-dissector
	--!
	--! @param proto	Protocol dissector (of type 'Proto' or 'Dissector')
	--! @return dissector that corresponds to @paramref proto, or nil if not found
	local function subdis_lookup (proto)
		local dis = nil
		if proto.name then
			-- wrap Dissector.get() to catch any errors, which occur if the specified
			-- dissector name is unknown
			local ok, err = pcall(
				function() 
					dis = Dissector.get(string.lower(proto.name)) 
				end
			)
			
			if not ok then
				debug("Couldn't find dissector: " .. proto.name .. " (E=" .. err .. ")")
			end
		else
			dis = proto
		end
		return dis
	end
	
	--! @brief	Adds a sub-dissector to the head of the subdis array for the specified sub-dissector and port
	--!
	--! @param proto	Protocol dissector (of type 'Proto' or 'Dissector')
	--! @param distab	Dissector table to which the port is registered (of type 'DissectorTable')
	--! @param port		Port number
	local function subdis_add_dis (proto, distab, port)
		if not proto or not distab or port < 0 or port > 65535 then return end
		
		debug("DISCHAIN: Adding " .. tostring(proto) .. " to port " .. port)
		
		local dis = subdis_lookup(proto)
		if not dis then 
			debug("        dis not found")
			return 
		end
		
		local disname = tostring(dis)		
		local is_hipri = list_indexof(prio_dlist, string.lower(disname))

		local d_list = nil
		if is_hipri then
			d_list = hipri_subdis[port]
		else
			d_list = lopri_subdis[port]
		end

		--[[
			If this port has no protocols or sub-dissectors registered,
			create a list for this port. Otherwise, if the dissector is
			not already in the list, put it at the top of the list.
		]]--
		if not d_list then 
			d_list = {proto} 
		elseif not list_indexof(d_list, proto) then
			table.insert(d_list, 1, proto)
		end

		if is_hipri then
			hipri_subdis[port] = d_list
		else
			lopri_subdis[port] = d_list
		end
		
		-- XXX: Register the sub-dissector outside of this function
		--distab:add(port, proto)
	end
	
	--! @brief	Removes a sub-dissector from the subdis array for the specified sub-dissector and port
	--! 		If no more sub-dissectors remain after the specified sub-dissector is removed,
	--!			the Lua Sub-dissector Chain is unregistered from the specified port.
	--!
	--! @param proto	Protocol dissector (of type Proto)
	--! @param distab	Dissector table to which ports are currently registered (of type DissectorTable).
	--! @param port		Port number
	local function subdis_rem_dis (proto, distab, port)
		debug("DISCHAIN: Removing " .. tostring(proto) .. " from port " .. port)
		
		local function rem_dis(sdis, proto, distab, port)
			if not sdis then return end
			
			-- find the subdissector in this port's subdissector list, and remove it
			local d_list = sdis[port]
			if not d_list then
				return 
			end

			local idx = list_indexof(d_list, proto)
			if idx then 
				table.remove(d_list, idx) 
				
				-- if this port has no more sub-dissectors, then unregister the Lua Sub-dissector
				-- Chain from this port
				if 0 == list_count(d_list) then
					d_list = nil
					distab:remove(port, dischain)
				end
				
				sdis[port] = d_list
			end
		end

		rem_dis(hipri_subdis, proto, distab, port)
		rem_dis(lopri_subdis, proto, distab, port)
	end
	
	--! @brief		Iteratively tries dissecting a buffer until the list
	--! 			is exhausted or a dissector parses it successfully
	--!
	--! @param dislist	List of sub-dissectors (of type 'Proto' or 'Dissector')
	--! @param tvb		Frame buffer to be dissected
	--! @param pinfo	Packet information
	--! @param tree		Protocol tree to display dissected values
	--! @return true	if a subdissector parses the tvb; otherwise, false
	local function try_dissector_list (dislist, tvb, pinfo, tree)
		if dislist then
			for _,dis in ipairs(dislist) do
				dis = subdis_lookup(dis)
				if dis then
					local disname = string.lower(tostring(dis))
					if disname ~= "dischain" then 
					
						--debug("DISCHAIN #" .. pinfo.number .. ": pass to " .. disname)
						
						--[[
							FIXME: The only way (I know of) to determine the result of the
							sub-dissector call is for the sub-dissector to put the result
							into dischain_results[] (by calling dischain_consume_packet()). 
							We index this array by the packet number (which is assumed to 
							always be unique for all packets).
						]]--
						dischain_results[pinfo.number] = nil
						dis:call(tvb,pinfo,tree)
						local res = dischain_results[pinfo.number]
						dischain_results[pinfo.number] = nil

						if res then
							--debug("DISCHAIN #" .. pinfo.number .. ": ".. disname .. " consumed")
							return true -- parsed!
						end
					end
				end
			end
		end
		return false -- not parsed!
	end
	
	--! @brief		Resorts subdissectors based on the list of prioritized 
	--! 			dissectors from preferences; dissectors whose names are
	--!				in the list in prefs are given high priority, which means
	--!				packets are passed to those dissectors first.
	local function init_prioritized_subdis()
		prio_dlist = {}
		-- string.gmatch() returns a closure, so we have to collect all the matches into an array for list_indexof()
		for name in string.gmatch(dischain.prefs.priodlist, "([^,]*),?") do
			if name and name ~= "" and name ~= "dischain" then
				table.insert(prio_dlist, string.lower(name))
			end
		end

		if 0 == list_count(prio_dlist) then 
			return
		end
		
		local function resort_subdissectors(sdis, hipri_list, lopri_list, init_dlists)
			for port,d_list in pairs(sdis) do
				local h_list = nil
				local l_list = nil
				if init_dlists then
					h_list = hipri_list[port]
					l_list = lopri_list[port]
				end
				
				for _,dis in pairs(d_list) do
					local d = subdis_lookup(dis)
					if d then
						local disname = string.lower(tostring(d))
						local idx = list_indexof(prio_dlist, disname)
						if idx then
							if not h_list then 
								h_list = {dis} 
							else
								table.insert(h_list, 1, dis)
							end
						else
							if not l_list then
								l_list = {dis}
							else
								table.insert(l_list, 1, dis)
							end
						end
					end
				end
				
				if h_list then
					hipri_list[port] = h_list
				end
				if l_list then
					lopri_list[port] = l_list
				end
			end
		end
		
		local hipri = {}
		local lopri = {}
		resort_subdissectors(hipri_subdis, hipri, lopri)
		resort_subdissectors(lopri_subdis, hipri, lopri, true)
		hipri_subdis = hipri
		lopri_subdis = lopri
	end

	--! @brief Initializes the Lua Sub-dissector Chain dissector (called at startup and upon change in preferences)
	function dischain.init ()
		-- resort dissector lists based on priority
		init_prioritized_subdis()
	end
	
	--! @brief 	Lua Sub-dissector Chain dissector (called for every packet at registered ports)
	--! 		Looks up the sub-dissectors for the packet's port, and passes the packet to the found
	--! 		sub-dissector. If that dissection fails, the next sub-dissector is called. This repeats
	--! 		until sub-dissectors for the packet's port are exhausted, in which case this dissector
	--! 		returns nil. Otherwise, if the dissection succeeds (indicated by non-nil return), this
	--! 		dissector returns the sub-dissector's rval.
	--!
	--! @param tvb		Frame buffer to be dissected
	--! @param pinfo	Packet information
	--! @param tree		Protocol tree to display dissected values
	function dischain.dissector (tvb, pinfo, tree)
		
		-- Look up sub-dissector on source port. If none, try dest port. 
		-- Try the high-priority sub-dissectors first.
		local parsed = false
		if hipri_subdis[pinfo.src_port] then
			parsed = try_dissector_list(hipri_subdis[pinfo.src_port], tvb, pinfo, tree)
		end
		if not parsed and hipri_subdis[pinfo.dst_port] then
			parsed = try_dissector_list(hipri_subdis[pinfo.dst_port], tvb, pinfo, tree)
		end
		if not parsed and lopri_subdis[pinfo.src_port] then
			parsed = try_dissector_list(lopri_subdis[pinfo.src_port], tvb, pinfo, tree)
		end
		if not parsed and lopri_subdis[pinfo.dst_port] then
			parsed = try_dissector_list(lopri_subdis[pinfo.dst_port], tvb, pinfo, tree)
		end

		-- if none of the sub-dissectors parsed this packet, fall back to Wireshark's
		-- default Data dissector (if pref enabled) by simply returning 0
		if not parsed and dischain.prefs.data_dsctr_fallback then
			return 0
		end
	end
	
	--! @brief	Removes a sub-dissector from the dissector table of one or more ports
	--!
	--! @param proto	Protocol dissector (of type Proto)
	--! @param distab	dissector table from which ports are to be unregistered (of type DissectorTable)
	--! @param lo		min value of port range
	--! @param hi		max value of port range (or nil if max N/A)
	function dischain_unreg_ports (proto, distab, lo, hi)
		if 	not proto or 
			not distab or 
			not lo or (lo < 0 or lo > 65535) or 
			(hi and (hi < 0 or hi > 65535)) then 
			return
		end
	
		for i=lo,(hi or lo) do 
			subdis_rem_dis(proto, distab, i)
			--distab:remove(i, proto)
		end
		return true
	end
	
	--! @brief	Adds a sub-dissector to the dissector table of one or more ports.
	--! 		If a previous range is also specified, that range is unregistered
	--!			beforehand.
	--!
	--! @param proto	Protocol dissector (of type Proto)
	--! @param distab	dissector table from which ports are to be registered (of type DissectorTable)
	--! @param lo		min value of port range
	--! @param hi		max value of port range (or nil if max N/A)
	--! @param old_lo	previous min value of port range
	--! @param old_hi	previous max value of port range (or nil if max N/A)
	function dischain_reg_ports (proto, distab, lo, hi, old_lo, old_hi)
		if 	not proto or 
			not distab or 
			not lo or 
			(lo < 0 or lo > 65535) or 
			(hi and (hi < 0 or hi > 65535))	then 
			return
		end
		
		--[[ Test cases
		
		 new outside of old:
		
		 		      a1---a2
		 	b1----b2
		 
		 	|------|  |----|
		 	  new	    old
		 
		 	  
		 new hangs off left end of old:
		
		 		   a1------a2
		 	b1-------------b2
		 
		 	|------|--------|
		 	  new	   old
		 
		 
		 new hangs off right end of old:
		 
		 	a1---------a2
		    b1--------------b2
		 
		    |----------|-----|
		   	    old     new
		 
		 
		 new covers all of old:
		
		 		a1---------a2
		 	b1-------------------b2
		 
		 	|---|----------|-----|
		 	 new	 old     new
		 
		   
		 new inside old:
		
		 	a1---------------------------a2
		 		b1------b2
		 
		 	|---|-------|----------------|
		   	 new   old         new	
		]]--
		if old_lo and old_hi and (old_lo > -1) and (old_hi > -1) then
			
			-- check for overlap
			if 	(hi > old_lo and hi <= old_hi) or (lo >= old_lo and lo < old_hi) or
				(old_hi > lo and old_hi <= hi) or (old_lo >= lo and old_lo < hi) then
				
				if lo < old_lo then
					debug("new lo ports: " .. tostring(lo) .. "-" .. tostring(old_lo-1))
					-- new ports
					dischain_reg_ports(proto, distab, lo, old_lo-1)
					
				elseif lo > old_lo then
					debug("old lo ports: " .. tostring(old_lo) .. "-" .. tostring(lo-1))
					-- expired ports
					dischain_unreg_ports(proto, distab, old_lo, lo-1)
				end
				
				if hi < old_hi then
					debug("old hi ports: " .. tostring(hi+1) .. "-" .. tostring(old_hi))
					-- expired ports
					dischain_unreg_ports(proto, distab, hi+1, old_hi)
					
				elseif hi > old_hi then
					debug("new hi ports: " .. tostring(old_hi+1) .. "-" .. tostring(hi))
					-- new ports
					dischain_reg_ports(proto, distab, old_hi+1, hi)
				end
			
				-- FIXME: For some reason, Dissector.get() cannot find any dissectors if we're
				-- dealing with more than 1000 ports in one call (it works okay, if we do each
				-- call one at a time). That is, Dissector.get() returns an error, which results
				-- in the packet list not being dissected. For now, just force the user to restart 
				-- Wireshark for the change to take effect.
				if (math.abs(hi-old_hi) > 1000) or (math.abs(lo-old_lo) > 1000) then
					report_failure("Chaining more than 1000 ports. Please restart Wireshark to allow proper dissector chaining.");
				end
				
			-- otherwise, the new values are outside the current range
			else
				debug("brand new ports: " .. tostring(lo) .. "-" .. tostring(hi))
				
				dischain_unreg_ports(proto, distab, old_lo, old_hi)
				dischain_reg_ports(proto, distab, lo, hi)
				
				-- FIXME: For some reason, Dissector.get() cannot find any dissectors if we're
				-- dealing with more than 1000 ports in one call (it works okay, if we do each
				-- call one at a time). That is, Dissector.get() returns an error, which results
				-- in the packet list not being dissected. For now, just force the user to restart 
				-- Wireshark for the change to take effect.
				if (math.abs(old_hi-old_lo) > 1000) or (math.abs(hi-lo) > 1000) then
					report_failure("Chaining more than 1000 ports. Please restart Wireshark to allow proper dissector chaining.");
				end
			end
			return true
		end
		
		for i=lo,(hi or lo) do 

			-- if a sub-dissector already exists, add it to the dissector chain
			local dis = distab:get_dissector(i)
			if dis and string.lower(tostring(dis)) ~= "dischain" then
				subdis_add_dis(dis, distab, i)
			end

			subdis_add_dis(proto, distab, i) 
			distab:add(i, dischain)
			
			--distab:add(i, proto)
		end
		return true
	end
	
	--! @brief 	Marks a packet as consumed to break out of the dissector
	--! 		chain for the packet
	--!
	--! @param proto	Proto/sub-dissector that is consuming the packet
	--! @param pktnum	Number of packet to be marked as consumed (from pinfo.number)
	function dischain_consume_packet (proto, pktnum)
		if not pktnum then return end
		dischain_results[pktnum] = true
	end
end

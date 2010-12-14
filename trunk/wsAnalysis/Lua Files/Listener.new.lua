--  This Example will add a menu "Lua Dialog Test" under the Tools menu, which when selected will pop a dialog prompting the user for input that when accepted will pop a window with a result.


function dialog_menu(pinfo)
local function init_listener()
do

   local tap = Listener.new("ip")
   function tap.packet(pinfo,tvb,ip)
    --function dialog_func(pinfo,person,eyes,hair)
        --local win = TextWindow.new("The Person");
        --win:set(person)
        --win:append(" with " .. eyes .." eyes and")
        --win:append(" " .. hair .. " hair.");
		--set_filter(" hair."..pinfo.len)
		--apply_filter()
		set_filter(" hair."..tostring(ip.ip_src))
    end
end
end
init_listener()
    --dialog_func
	
    --new_dialog("Dialog Test",init_listener(),"A Person","Eyes","Hair")
end
-- put this script in your init.lua file, and click on the Tools menu 
-- to copy the desired string in the clipboard
function stringipaddr()       copy_to_clipboard("sccp.dlr== || sccp.dlr==|| ") end
function stringnotipaddr()    copy_to_clipboard("sccp.slr== || sccp.slr==|| ") end

register_menu("Copy string \"ip.addr == \"", stringipaddr,   MENU_TOOLS_UNSORTED)
register_menu("Copy string \"!ip.addr == \"",stringnotipaddr,MENU_TOOLS_UNSORTED)

-- optional 3rd parameter to register_menu. See http://www.wireshark.org/docs/wsug_html_chunked/wsluarm_modules.html 
-- If omitted, defaults to MENU_STAT_GENERIC. Other options include:
-- MENU_STAT_UNSORTED (Statistics), MENU_STAT_GENERIC (Statistics, first section), 
-- MENU_STAT_CONVERSATION (Statistics/Conversation List), MENU_STAT_ENDPOINT (Statistics/Endpoint List), 
-- MENU_STAT_RESPONSE (Statistics/Service Response Time), MENU_STAT_TELEPHONY (Telephony), 
-- MENU_ANALYZE (Analyze), MENU_ANALYZE_CONVERSATION (Analyze/Conversation Filter), 
-- MENU_TOOLS_UNSORTED (Tools)
register_menu("Lua Dialog Test",dialog_menu,MENU_TOOLS_UNSORTED)
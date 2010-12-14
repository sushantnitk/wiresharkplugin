--  This Example will add a menu "Lua Dialog Test" under the Tools menu, which when selected will pop a dialog prompting the user for input that when accepted will pop a window with a result.

if gui_enabled() then
   --local splash = TextWindow.new("Hello!");
   --splash:set("This time wireshark has been enhanced with an useles feature.\n")
   --splash:append("Go to Statistics->Useless Feature and check it out!")
end
function dialog_menu()
    function dialog_func(slr,dlr)
        local win = TextWindow.new("Follow SCCP Stream");
		slr=0
		dlr=0
        win:set("")
		--sccp.dlr==0xee089d || sccp.slr==0xee089d || sccp.dlr==0xdd002f || sccp.slr==0xdd002f
        win:append(" sccp.slr==" .. slr .." || sccp.dlr==".. slr.." || ")
        win:append(" sccp.slr==" .. dlr .." || sccp.dlr==".. dlr);
		set_filter(tostring(win))
    end
    new_dialog("Dialog Test",dialog_func,"slr","dlr")
end
-- optional 3rd parameter to register_menu. See http://www.wireshark.org/docs/wsug_html_chunked/wsluarm_modules.html 
-- If omitted, defaults to MENU_STAT_GENERIC. Other options include:
-- MENU_STAT_UNSORTED (Statistics), MENU_STAT_GENERIC (Statistics, first section), 
-- MENU_STAT_CONVERSATION (Statistics/Conversation List), MENU_STAT_ENDPOINT (Statistics/Endpoint List), 
-- MENU_STAT_RESPONSE (Statistics/Service Response Time), MENU_STAT_TELEPHONY (Telephony), 
-- MENU_ANALYZE (Analyze), MENU_ANALYZE_CONVERSATION (Analyze/Conversation Filter), 
-- MENU_TOOLS_UNSORTED (Tools)
register_menu("Follow SCCP Stream",dialog_menu,MENU_TOOLS_UNSORTED)
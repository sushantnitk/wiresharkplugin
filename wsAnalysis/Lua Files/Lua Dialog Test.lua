--  This Example will add a menu "Lua Dialog Test" under the Tools menu, which when selected will pop a dialog prompting the user for input that when accepted will pop a window with a result.

if gui_enabled() then
   local splash = TextWindow.new("Hello!");
   splash:set("This time wireshark has been enhanced with an useles feature.\n")
   splash:append("Go to Statistics->Useless Feature and check it out!")
end
function dialog_menu()
    function dialog_func(person,eyes,hair)
        local win = TextWindow.new("The Person");
        win:set(person)
        win:append(" with " .. eyes .." eyes and")
        win:append(" " .. hair .. " hair.");
    end

    new_dialog("Dialog Test",dialog_func,"A Person","Eyes","Hair")
end
-- optional 3rd parameter to register_menu. See http://www.wireshark.org/docs/wsug_html_chunked/wsluarm_modules.html 
-- If omitted, defaults to MENU_STAT_GENERIC. Other options include:
-- MENU_STAT_UNSORTED (Statistics), MENU_STAT_GENERIC (Statistics, first section), 
-- MENU_STAT_CONVERSATION (Statistics/Conversation List), MENU_STAT_ENDPOINT (Statistics/Endpoint List), 
-- MENU_STAT_RESPONSE (Statistics/Service Response Time), MENU_STAT_TELEPHONY (Telephony), 
-- MENU_ANALYZE (Analyze), MENU_ANALYZE_CONVERSATION (Analyze/Conversation Filter), 
-- MENU_TOOLS_UNSORTED (Tools)
register_menu("Lua Dialog Test",dialog_menu,MENU_TOOLS_UNSORTED)
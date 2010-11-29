Wireshark 1.0.13 and Lua (post)dissectors From: Oleg <oleg.khr () gmail com>
Date: Mon, 17 May 2010 17:52:53 -0500

I am trying to implement postdissectors using Lua and Wireshark 1.0.13.
And even a basic example doesn't work for me.

Field extractor always returns nil:

udp_len_f = Field.new("udp.length")
test_proto = Proto ("test", "Test Protocol")

function test_proto.dissector (buffer, pinfo, tree)
   pinfo.cols.info = "Test"

   local udp_len = udp_len_f()
   if udp_len then
     pinfo.cols.info = "UDP length: " .. udp_len.value
else
     pinfo.cols.info = "UDP length: nil"
   end
end

register_postdissector(test_proto)

Such postdissector always produces "UDP length: nil" for all UDP messages.

It works fine on the newer versions of Wireshark but I have to use 1.0.x
series for my task.
Was anyone able to get it working with 1.0.13?

Thanks,
Oleg

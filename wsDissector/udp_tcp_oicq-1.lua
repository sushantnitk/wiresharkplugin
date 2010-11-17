-- 定义协议，可以在wireshark中使用trivial过滤
--http://blog.csdn.net/someonea/archive/2008/04/27/2336195.aspx

trivial_proto = Proto("trivial","TRIVIAL","Trivial Protocol")

 

-- dissector函数

function trivial_proto.dissector(buffer,pinfo,tree)

   

    --pinfo的成员可以参考用户手册

    pinfo.cols.protocol = "TRIVIAL"

    pinfo.cols.info = "TRIVIAL data"

   

    local subtree = tree:add(trivial_proto,buffer(),"Trivial Protocol")

          

    --不对应任何数据

    subtree:add(buffer(0,0),"Message Header: ")

   

    --版本号对应于第一个字节

    subtree:add(buffer(0,1),"Version: " .. buffer(0,1):uint())

   

    --类型对应于第二个字节

    type = buffer(1,1):uint()

    type_str = "Unknown"

    if type == 1 then

        type_str = "REQUEST"

    elseif type == 2 then

        type_str = "RESPONSE"

    end

    subtree:add(buffer(1,1), "Type: " .. type_str)

 

    --从第三个字节开始是数据

    size = buffer:len()

    subtree:add(buffer(2,size-2), "Data: ")

      

end

 

tcp_table = DissectorTable.get("tcp.port")

--注册到tcp的8888端口

tcp_table:add(14000,trivial_proto)
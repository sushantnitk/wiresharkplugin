-- value-string maps for the protocol fields
local qqcommand  = {
		Logout=1,
		HeartMessage=2,
		UpdateUserinformation=4,
		Searchuser=5,
		GetUserinformationBroadcast=6,
		Addfriendnoauth=9,
		Deleteuser=10,
		Addfriendbyauth=11,
		Setstatus=13,
		Confirmationofreceivingmessagefromserver=18,
		Sendmessage=22,
		Receivemessage=23,
		Retrieveinformation=24,
		HeartMessage=26,
		DeleteMe=28,
		RequestKEY=29,
		CellPhone=33,
		Login=34,
		Getfriendlist=38,
		Getfriendonline=39,
		CellPHONE=41,
		Operationongroup=48,
		Logintest=49,
		Groupnameoperation=60,
		Uploadgroupfriend=61,
		MEMOOperation=62,
		Downloadgroupfriend=88,
		Getlevel=92,
		Requestlogin=98,
		Requestextrainformation=101,
		Signatureoperation=103,
		Receivesystemmessage=128,
		Getstatusoffriend=129,
		Getfriendsstatusofgroup=181
}

-- Declare protocol
proto_msqq = Proto("msqq","msqq Protocol")
-- Declare its fields
local msqq = proto_msqq.fields
msqq.Flag = ProtoField.string("msqq.Flag","Flag")
msqq.Length = ProtoField.string("msqq.Length","Length")
msqq.Version = ProtoField.string("msqq.Version","Version")
msqq.Command = ProtoField.string("msqq.Command","Command")
msqq.Sequence = ProtoField.string("msqq.Sequence","Sequence")
msqq.qqNumber = ProtoField.string("msqq.qqNumber","qqNumber")
-- msqq.Data = ProtoField.string("msqq.Data","Data",)
-- The next step is to populate the fields table for our new protocol
-- msqq.fields = {msqq.Flag,msqq.Length,msqq.Version,msqq.Command,msqq.Sequence,msqq.qqNumber,msqq.Data}
-- Next we generate a dissector function
function proto_msqq.dissector(buffer,pinfo,tree)
    --qqcommand

    -- get data from raw packet data and convert to uint
    size = buffer:len()
	local flag= buffer(0,1):uint()--Flag对应1字节
	local length=buffer(1,2):uint()--Length对应2,3字节
	local version=buffer(3,2):uint()--Version对应4,5字节
	local command=buffer(5,2):uint()--Command对应6,7字节
	local sequence=buffer(7,2):uint()--Sequence对应8,9字节
	local qqnumber=buffer(9,4):uint()--qqNumber对应10,11,12,13字节
	-- local data=buffer(13,size-13):uint()--14字节开始是数据
	local type_str=""
	for key, value in pairs(qqcommand) do 
		if command == value then
		   type_str = key
		end
	end
	
    -- set the info column in top frame to include new field information
	pinfo.cols.protocol = "msqq"
	pinfo.cols.info:set("qqNumber: "..qqnumber)
    -- pinfo.cols.info:set("Flag: "..flag..", Length: ".. length..", Version: " ..version..", Command："..command..", Sequence: " ..sequence..", qqNumber: "..qqnumber)					 
	-- pinfo.cols.info:set("Field1: "..field1..", Field2: "..field2..", Field3: "..field3)
	
	-- generate a new tree item
    local subtree = tree:add(proto_msqq,buffer(),"msqq Protocol")
    subtree:add(msqq.Flag,buffer(0,1),flag)
    subtree:add(msqq.Length,buffer(1,2),length)
    subtree:add(msqq.Version,buffer(3,2),version)
	if type_str == "" then
       subtree:add(msqq.Command,buffer(5,2),command)
	else
	   subtree:add(msqq.Command,buffer(5,2),type_str)
	end
	subtree:add(msqq.Sequence,buffer(7,2),sequence)
	subtree:add(msqq.qqNumber,buffer(9,4),qqnumber)
	subtree:add(buffer(13,size-13), "Data: ".. buffer(13,size-13))
    --subtree:add(msqq.Data,buffer(13,size-13),data)

end
-- Finally we register our protoco
tcp_table = DissectorTable.get("tcp.port")
tcp_table:add(14000,proto_msqq)
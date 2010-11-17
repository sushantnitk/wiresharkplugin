-- value-string maps for the protocol fields
local VALS_FOO  = {[0x333333] = "New York", [0x303030] = "Los Angeles",[0x313233] = "Chicago"}
local VALS_BAR  = {[0x11] = "Whiskey", [0x12] = "Rum", [0x13] ="Vodka", [0x14] = "Gin"}
local VALS_BOOL = {[0] = "False", [1] = "True"}

-- Declare protocol
proto_toy = Proto("toy", "Toy Protocol")

-- Declare its fields
local toy       = proto_toy.fields
toy.ver = ProtoField.uint32("toy.ver"   , "Version")
toy.bf  = ProtoField.uint64("toy.bf"    , "Bitfield"            , base.HEX)
toy.bfhi        = ProtoField.uint32("toy.bfhi"  , "Upper 32 bits"       , base.HEX)
toy.bflo        = ProtoField.uint32("toy.bflo"  , "Lower 32 bits"       , base.HEX)

-- (the tree is more readable when all bit fields are aligned, so make them all the same bit length)
toy.bf_foo      = ProtoField.uint32("toy.bf.foo", "Foo"         , base.DEC,VALS_FOO , 0x00FFFFFF)
toy.bf_bar      = ProtoField.uint32("toy.bf.bar", "Bar"         , base.DEC,VALS_BAR , 0x1F000000)
toy.bf_st       = ProtoField.uint32("toy.bf.st" , "Sticky"      , base.DEC,VALS_BOOL, 0x00000001)
toy.bf_rd       = ProtoField.uint32("toy.bf.rd" , "Read"        , base.DEC,VALS_BOOL, 0x00000002)
toy.bf_wr       = ProtoField.uint32("toy.bf.wr" , "Write"       , base.DEC,VALS_BOOL, 0x00000004)
toy.bf_ex       = ProtoField.uint32("toy.bf.ex" , "Execute"     , base.DEC,VALS_BOOL, 0x00000008)

-- Define the dissector
function proto_toy.dissector(buf, pinfo, tree)

        -- 1 byte for version and 8 for 64-bit field
        local EXPECTED_LENGTH = 1+8

        if (buf:len() < EXPECTED_LENGTH) then
                -- not ours, let it go to default Data dissector
                return 0
        end

        pinfo.cols.protocol = "toy"

        -- add our packet to the tree root...we'll add fields to its subtree
        local t = tree:add( proto_toy, buf(0, EXPECTED_LENGTH) )

        t:add( toy.ver, buf(0,1) )                      -- version
        local t_bf = t:add( toy.bf, buf(1,8) )          -- bitfield

        local t_hi = t_bf:add( toy.bfhi, buf(1,4) )     -- Upper 32 bits
        t_hi:add( toy.bf_foo    , buf(1,4) )            -- Foo
        t_hi:add( toy.bf_bar    , buf(1,4) )            -- Bar

        local t_lo = t_bf:add( toy.bflo, buf(5,4) )     -- Lower 32 bits
        t_lo:add( toy.bf_st     , buf(5,4) )            -- Sticky
        t_lo:add( toy.bf_rd     , buf(5,4) )            -- Read
        t_lo:add( toy.bf_wr     , buf(5,4) )            -- Write
        t_lo:add( toy.bf_ex     , buf(5,4) )            -- Execute

end

-- Register toy protocol on UDP port 22222
local tab = DissectorTable.get("udp.port")
tab:add(22222, proto_toy)

--[[

# Start capture on UDP port 22222, and enter netcat commands to test:

# New York, Rum, Write
echo "-r333---4" | nc -w 0 -u 1.1.1.1 22222

# Los Angeles, Vodka,  Read, Execute
echo "-s000---j" | nc -w 0 -u 1.1.1.1 22222

# Chicago, Gin, Sticky, Read
echo "-t123---3" | nc -w 0 -u 1.1.1.1 22222

]]--
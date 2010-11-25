tshark.exe  -T pdml -r "MCNew.cap"  | perl -ane ' <at> flist=qw(m3ua.protocol_data_opc m3ua.protocol_data_dpc h248.transactionId);\
foreach $f ( <at> flist) {\
 if(/field name=\"$f\".*show=\"(.*?)\".*/){print "$f:$1,";}}'
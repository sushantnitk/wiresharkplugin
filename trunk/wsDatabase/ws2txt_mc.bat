@echo gsm_a.dtap_msg_cc_type 	Unsigned 8-bit integer 	DTAP Call Control Message Type 	1.2.0 to 1.4.2
@echo gsm_a.dtap_msg_mm_type 	Unsigned 8-bit integer 	DTAP Mobility Management Message Type 	1.2.0 to 1.4.2
@echo gsm_a.dtap_msg_sms_type 	Unsigned 8-bit integer 	DTAP Short Message Service Message Type 	1.2.0 to 1.4.2
@echo gsm_a.dtap_msg_ss_type 	Unsigned 8-bit integer 	DTAP Non call Supplementary Service Message Type 	1.2.0 to 1.4.2
@echo gsm_a.dtap_msg_tp_type 	Unsigned 8-bit integer 	DTAP Tests Procedures Message Type 	1.2.0 to 1.4.2
@echo expert.group 	Unsigned 32-bit integer 	Group 	1.2.0 to 1.4.2
@echo expert.message 	String 	Message 	1.2.0 to 1.4.2
@echo expert.severity 	Unsigned 32-bit integer 	Severity level 	1.2.0 to 1.4.2
@echo tshark -r  mc.cap -V -t  e  -T fields -e frame.number -e frame.time -e frame.time_relative -e sccp.message_type -Ttext
@echo tshark -r mc.cap -o column.format:'"No.", "%m", "Info", "%i"'
@echo tshark -r mc.cap  -o column.format:'"Source", "%s","Destination", "%d"' -Ttext

@echo on
set port= 
set/p port=ÇëÊäÈëÎÄ¼þ:

tshark -r  %port% -V -t  e  -T fields -e frame.number -e frame.time -e frame.time_relative -e sccp.message_type -e isup.message_type  -e gsm_a.bssmap_msgtype -e gsm_a.dtap_msg_mm_type -e gsm_a.dtap_msg_cc_type -e gsm_a.dtap_msg_rr_type -e gsm_a.imsi -e gsm_a.tmsi -e ip.src -e ip.dst -e sccp.slr -e sccp.dlr -e ip.version  -E  separator=;   >  %port%.csv

@echo Done.....
pause
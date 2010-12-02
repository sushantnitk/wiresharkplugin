@echo 0.进入目录

cd K:\寻呼项目—范茂鑫\SZ_1201_08A

@echo 1.多台电脑上的pcap文件按照时间合并......

mergecap -v -T ether -w  mc.cap CE*.cap

@echo 2.分割成大小相同的文件......

editcap mc.cap -c 2000000 mc_mergefile.cap

@echo continue......

pause
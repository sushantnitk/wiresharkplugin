
#!/usr/bin/perl

CAP_DIR="/export/pktcap/"
NEW_CAP_DIR="$CAP_DIR/dst"

for CAP_FILE in `ls $CAP_DIR/*.cap`; do
    # get the filename from the full path
    CAP_FILE=`basename $CAP_FILE`
    NEW_CAP_FILE="$NEW_CAP_DIR/$CAP_FILE"
    mergecap -aw- $_ | tcpdump -nqpr- -w $NEW_CAP_FILE dst 192.168.0.78
    echo "Filtered Results in $NEW_CAP_FILE"
done

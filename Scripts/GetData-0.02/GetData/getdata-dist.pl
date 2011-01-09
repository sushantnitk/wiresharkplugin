#getdata.pl ver 0.02
#written by AJSPINA@GMAIL.COM
#released 07/25/06
#http://simquery.sourceforge.net/
#
# 0.01 Initial Release
# 0.02 added config file

#Import Modules, both are "BASE" modules on ActiveStates ActivePerl
#Upgrade to latest ActivePerl(Build 817) to avoid TK bugs
use Time::Local;
use Tk;
use GetData::Config;

#Declare scalars
my $srcip;
my $srcport;
my $dstip;
my $dstport;
my $endtime;

my $password = "";#these values will 
my $sensorip = $GetData::Config{sensorip};#prepopulate the TK pop-up box

my $username = $GetData::Config{username};#username on sensor
my $hostname = $GetData::Config{hostname};#must match hostname used in logpackets.pl

#Arguements passed via 
my $sshexe = "c:\\windows\\system32\\plink.exe";
my $scpexe = "c:\\windows\\system32\\pscp.exe";
my $tcpdump = "/usr/local/sbin/tcpdump";#path TO tcpdump ON sensor
my $dumppath = "/packetdumps/$hostname/dailylogs";
my $tempholding = "/export/home/$username/";
my $localpcappath = "C:\\";
#my $ethereal = "\"C:\\Program Files\\Ethereal\\ethereal.exe\"";
my $ethereal = "\"C:\\Program Files\\Wireshark\\wireshark.exe\"";


#Grab agruments passed from arcsight 
foreach $arg (@ARGV) {
if ($arg =~m/srcIP/) {$srcip=$arg; $srcip=~s/srcIP://g;}
elsif ($arg =~m/destIP/) {$dstip=$arg; $dstip=~s/destIP://g;}
elsif ($arg =~m/srcPORT/) {$srcport=$arg; $srcport=~s/srcPORT://g;}
elsif ($arg =~m/destPORT/) {$dstport=$arg; $dstport=~s/destPORT://;}
elsif ($arg =~m/endTime/) {$endtime=$arg; $endtime=~s/endTime://;$endtime=~s/"//g;}
}

#Print Arguments to stdout
print "Source IP: $srcip:$srcport \n";
print "Dest IP: $dstip:$dstport \n";
print "End Time: $endtime \n";

#Launch a gui window to grab username/password/sensorip
#If your choose to hardcode the username/password/sensorip in this script comment out the following lines

$mw = MainWindow->new;
$hello = $mw->title("Sensor Requestor");
$hello = $mw->Label(-text => "Please login to the sensor")->pack;

$hello = $mw->Label(-text => "Username:")->pack;
$hello = $mw->Entry(-width => '20',-textvariable => \$username)->pack;

$hello = $mw->Label(-text => "Password:")->pack;
$hello = $mw->Entry(-show => '*',-width => '20',-textvariable => \$password)->pack;

$hello = $mw->Label(-text => "SensorIP:")->pack;
$hello = $mw->Entry(-width => '20',-textvariable => \$sensorip)->pack;

$hello = $mw->Button( -text    => 'Login', -command => sub {$mw->destroy()})->pack;
MainLoop;
##################################################################################################

print "Logging in as: $username\n";

#The following lines convert the date/time to epoch time
#TIME MUST BE PASSED AS "DD Mon YYYY hh:mm:ss TZ"
#IF IT DOESNT, MODIFY THE PARSING CODE BELOW
#
@datetime = split/ /,$endtime; $mday = $datetime[0]; $month = $datetime[1]; $year = $datetime[2]; $hhmmss = $datetime[3];
#print "Month: $month\n";
if ($month eq "Jan") {$month = "00";$smonth = "01";}
if ($month eq "Feb") {$month = "01";$smonth = "02";}
if ($month eq "Mar") {$month = "02";$smonth = "03";}
if ($month eq "Apr") {$month = "03";$smonth = "04";}
if ($month eq "May") {$month = "04";$smonth = "05";}
if ($month eq "Jun") {$month = "05";$smonth = "06";}
if ($month eq "Jul") {$month = "06";$smonth = "07";}
if ($month eq "Aug") {$month = "07";$smonth = "08";}
if ($month eq "Sep") {$month = "08";$smonth = "09";}
if ($month eq "Oct") {$month = "09";$smonth = "10";}
if ($month eq "Nov") {$month = "10";$smonth = "11";}
if ($month eq "Dec") {$month = "11";$smonth = "12";}
#print "hhmmss: |$hhmmss|\n";
@realtime = split ':',$hhmmss; 

$hour = $realtime[0]; $min = $realtime[1]; $sec = $realtime[2]; $tz = $datetime[4];
print "Time Zone:$tz\n";
$etime = timelocal($sec,$min,$hour,$mday,$month,$year);
if ($mday < 10) {$mday = "0" . $mday;}
#just a little debug to stdout to make sure all worked well before start the real work...
#uncomment to test

print "Checking what is available on sensor \n";

#########################################################

#grab list of files for date of request
@filelist = `plink $username\@$sensorip -pw $password ls -r $dumppath/$year-$smonth-$mday/ `;

@revlist = reverse @filelist;
$timeranged = substr($revlist[0],10,10);
print "\nEarliest Packet Trace File Available:" . scalar localtime $timeranged . "\n";

foreach $line (@filelist){ 
	unless ($line =~m/Using/) {
		chomp $line;	
		$filetime = substr($line,10,10);
		#print "\n==============================================================\n";
		#print "\nTime on  File:$filetime " . scalar localtime $filetime . "\n";
		#print "\nTime of Event:$etime " . scalar localtime $etime . "\n"; 
		$diff = $etime - $filetime;
		#print "\nDifference: $diff\n";
		#print "\n==============================================================\n";
		$outfile = $line . $etime . ".cap";
		$outfile =~s/ //g;
		$offset = 900;#seconds
			if ($filetime lt $etime && $diff < $offset && $diff > 1) {	
				print "\nNow Doing Search using: $line\n";
				#Arguements must change if using different ssh executable, below arguements are for plink(putty)
				$args2 = `$sshexe -T $username\@$sensorip -pw $password $tcpdump -n -r $dumppath/$year-$smonth-$mday/$line -w $tempholding$outfile host $srcip and host $dstip 2>&1`;	
		
				print "Now doing Secure Copy:$args2\n";
				$args3 = `$scpexe -pw $password $username\@$sensorip\:$tempholding/$outfile $localpcappath 2>&1`;
				print "Secure Copy Status: $args3\n";

				print "Now opening Wireshark\n";
				$args4 = `$ethereal -r $localpcappath$outfile 2>&1`;
				print "$args4\n"
				}
			else {
			#print "\n skipping $line (out of range)\n";
			}
	}
	}
print "Tool is now exiting, Query complete......\n";






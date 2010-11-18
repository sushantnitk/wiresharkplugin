#!/usr/bin/python
#pcap2msc ./avses-tosip.cap "sip" | mscgen -T png -o avses-tosip.png

import sys
import subprocess
import re

def usage():
  print >>sys.stderr, '%s: <.pcap> [tshark additional options] "wireshark display filter"' % sys.argv[0]
  sys.exit(1)

n = len(sys.argv)
if n < 3:
  usage()

capture = sys.argv[1]
dfilter = sys.argv[-1]

tshark_cmd = [ 'tshark', '-n' ]
tshark_cmd.extend(sys.argv[2:-1])
tshark_cmd.append('-r')
tshark_cmd.append(sys.argv[1])
tshark_cmd.append(sys.argv[-1])

# start tshark subprocess and prepare a pipe to which it will write stdout
shark = subprocess.Popen(tshark_cmd, stdout=subprocess.PIPE)
sharkout = shark.stdout

# list of messages displayed by tshark
messages = []

while True:
  line = sharkout.readline()
  # eof encountered
  if len(line) == 0:
    break

  regex = re.compile('^ *(\d+) +(\d+\.\d+) +(\d+\.\d+\.\d+\.\d+) -> (\d+\.\d+\.\d+\.\d+) (.*?)$')

  ret = regex.match(line)
  if ret != None:
    msg = {}
    msg['num'] = ret.group(1)
    msg['date'] = ret.group(2)
    msg['src'] = ret.group(3)
    msg['dst'] = ret.group(4)
    msg['msg'] = ret.group(5)
    messages.append(msg)
  else:
    print >>sys.stderr, "line '%s' not handled by regex !" % line
    break

# synchronously wait for tshark termination
shark.wait()
if shark.returncode != 0:
  print >>sys.stderr, "tshark returned error code %d" % shark.returncode
  sys.exit(1)

# list of entity
# contains IP addresses used IP datagrams exchanged in this capture
entities = []
for msg in messages:
  if msg['src'] not in entities:
    entities.append(msg['src'])
  if msg['dst'] not in entities:
    entities.append(msg['dst'])

if len(entities) == 0:
  sys.exit(1)

# print msc generated file on stdout
print("msc {")

# dots are not allowed in entity grammar (see mscgen grammar)
# thus, name IP address by u%d, where %d is replaced by their index in the list
line = ''
for i in range(0, len(entities)):
  line += 'u%d[label=\"%s\"]' % (i,entities[i])
  if i < len(entities)-1:
    line += ','
print("  %s;" % line)

# add messages
# a message is an arrow between src and dst (IP addresses)
# and a label which is the line used by tshark to describe packet content
for msg in messages:
  src = entities.index(msg['src'])
  dst = entities.index(msg['dst'])
  if src < dst:
    print("  u%d=>u%d [ label = \"%s\" ] ;" % (src, dst, msg['msg']))
  else:
    print("  u%d<=u%d [ label = \"%s\" ] ;" % (dst, src, msg['msg']))

print("}") 

import urllib.request
import json
import os
import sys
import subprocess
import signal
import shutil

def remove_temp():
    print("remove temp")
    if os.path.exists("./publishlinux-arm.tar.xz"):
        os.unlink("./publishlinux-arm.tar.xz")
    if os.path.exists("./publishlinux-arm"):
        shutil.rmtree("./publishlinux-arm")
    if os.path.exists("./logs"):
        shutil.rmtree("./logs")
    if os.path.exists('./events.db'):
        os.unlink('./events.db')
    print("done")

def dlProgress(count, blockSize, totalSize):
    percent = int(count*blockSize*100/totalSize)
    sys.stdout.write("\rdownloading...%d%%" % percent)
    sys.stdout.flush()

def getpid(process_name):
    return [int(item.split()[1]) for item in os.popen('ps -Af').read().splitlines() if process_name in item]

def make_executable(path):
    mode = os.stat(path).st_mode
    mode |= (mode & 0o444) >> 2    # copy R bits to X
    os.chmod(path, mode)

hyper_version_path = "/var/inhaus/hyper/version.txt"
hyper_version_remote_url = 'https://api.github.com/repos/jimzrt/hyper/releases/latest'
hyper_latest_url = 'https://github.com/jimzrt/hyper/releases/latest/download/publishlinux-arm.tar.xz'
default_com = "/dev/ttyUSB_ZStickGen5"

if len(sys.argv) > 1:
    default_com = sys.argv[1]

remove_temp()

#get remote version
res = urllib.request.urlopen(hyper_version_remote_url)
res_body = res.read()
j = json.loads(res_body.decode("utf-8"))
remote_version = j["tag_name"]
print("remote version: " + remote_version)

#get local version
local_version = "N/A"
if os.path.exists(hyper_version_path):
        with open(hyper_version_path, 'r') as f:
                local_version = f.read()
print("local version: " + local_version)

print("u sure? (y/n)")
sure = input()
if sure != "y":
    print("ok bye")
    sys.exit(0)

#download latest release
print("downloading latest version")
urllib.request.urlretrieve(hyper_latest_url, 'publishlinux-arm.tar.xz', reporthook=dlProgress)
print("\ndone!")

#extract
print("extracting")
subprocess.call('tar xf publishlinux-arm.tar.xz'.split(' '))
print("done!")

#stop hyper
print("stopping hyper")
subprocess.call("/etc/init.d/hyper stop".split(" "))
# hyper_pids = getpid('hyper')
# if hyper_pids:
#     hyper_pids.sort()
#     os.kill(hyper_pids[0], signal.SIGKILL)
# else:
#     print("hyper not running")
print("done")

#backup logs and events
print("backup")
shutil.copytree('/var/inhaus/hyper/logs', './logs')
shutil.copyfile('/var/inhaus/hyper/events.db', './events.db')
print("done")

#delete hyper folder
print("delete old")
shutil.rmtree('/var/inhaus/hyper', True)
print("done")

#copy downloaded hyper folder and backups
print("copy new")
shutil.copytree('./publishlinux-arm', '/var/inhaus/hyper')
shutil.copytree('./logs', '/var/inhaus/hyper/logs')
shutil.copyfile('./events.db', '/var/inhaus/hyper/events.db')
print("done")

remove_temp()

#make executable
make_executable('/var/inhaus/hyper/hyper')

print("starting hyper in background")
subprocess.call("/etc/init.d/hyper start".split(" "))
#subprocess.Popen(['nohup', './hyper', default_com], stdout=open('/dev/null', 'w'), stderr=open('logfile.log', 'a'), preexec_fn=os.setpgrp, cwd="/var/inhaus/hyper")
print("done")

print("starting client to verify")
subprocess.call("./ClientTCP", cwd="/var/inhaus/hyper")
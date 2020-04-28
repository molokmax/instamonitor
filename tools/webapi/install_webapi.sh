chmod +x /opt/instamonitor/InstaMonitor.Api

echo "/opt/instamonitor/InstaMonitor.Api urls=http://192.168.1.20:80" > /opt/instamonitor/run-instamonitor.sh
chmod +x /opt/instamonitor/run-instamonitor.sh

# add to /etc/rc.local:
# /opt/instamonitor/run-instamonitor.sh&

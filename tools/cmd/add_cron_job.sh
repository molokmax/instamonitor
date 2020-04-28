echo root >> /etc/cron.allow
# echo molokmax >> /etc/cron.allow

touch /var/spool/cron/crontabs/root
/usr/bin/crontab /var/spool/cron/crontabs/root

# touch /var/spool/cron/molokmax
# /usr/bin/crontab /var/spool/cron/molokmax

echo "0 0 * * * /opt/instamonitor/InstaMonitor.Run" >> /var/spool/cron/crontabs/root


service cron restart

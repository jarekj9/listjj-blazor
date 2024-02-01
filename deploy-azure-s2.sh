#!/bin/bash

rsync -avh --exclude '*.env' Listjj/ machinejj:/home/jarek/docker/Listjj-s2/Listjj/ --delete
rsync -avh --exclude '*.env' --exclude 'appsettings*json' ListjjFrontEnd/ machinejj:/home/jarek/docker/Listjj-s2/ListjjFrontEnd/ --delete
rsync -avh --exclude '*.env' Listjj.Infrastructure/ machinejj:/home/jarek/docker/Listjj-s2/Listjj.Infrastructure/ --delete


ansible machinejj -a 'sudo docker-compose -f /home/jarek/docker/Listjj-s2/docker-compose.yml up -d --build'
ansible machinejj -a 'docker image prune -f'



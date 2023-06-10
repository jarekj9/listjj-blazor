#!/bin/bash

rsync -avh --exclude '*.env' Listjj/ machinejj:/home/jarek/docker/Listjj-stg/Listjj/ --delete
rsync -avh --exclude '*.env' Listjj_frontend/ machinejj:/home/jarek/docker/Listjj-stg/Listjj_frontend/ --delete
rsync -avh --exclude '*.env' Listjj.Infrastructure/ machinejj:/home/jarek/docker/Listjj-stg/Listjj.Infrastructure/ --delete


ansible machinejj -a 'docker rm -f listjj_frontend_stg'
ansible machinejj -a 'docker image rm listjj_frontend_blazor_stg'
ansible machinejj -a 'docker-compose -f /home/jarek/docker/Listjj-stg/docker-compose.yml up -d'

#!/bin/bash

rsync -avh --exclude '*.env' Listjj/ machinejj:/home/jarek/docker/Listjj-stg/Listjj/ --delete
rsync -avh --exclude '*.env' --exclude 'appsettings*json' ListjjFrontEnd/ machinejj:/home/jarek/docker/Listjj-stg/ListjjFrontEnd/ --delete
rsync -avh --exclude '*.env' Listjj.Infrastructure/ machinejj:/home/jarek/docker/Listjj-stg/Listjj.Infrastructure/ --delete


ansible machinejj -a 'docker rm -f listjj_frontend_stg'
ansible machinejj -a 'docker image rm listjj_frontend_blazor_stg'
ansible machinejj -a 'docker rm -f listjj_api_stg'
ansible machinejj -a 'docker image rm listjj_blazor_api_stg'
ansible machinejj -a 'docker-compose -f /home/jarek/docker/Listjj-stg/docker-compose.yml up -d'



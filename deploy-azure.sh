#!/bin/bash

rsync -avh Listjj/ machinejj:/home/jarek/docker/Listjj/Listjj --delete
ansible machinejj -a 'docker rm -f listjj'
ansible machinejj -a 'docker image rm listjj_blazor'
ansible machinejj -a 'docker-compose -f /home/jarek/docker/Listjj/docker-compose.yml up -d'

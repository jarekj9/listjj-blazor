#!/bin/bash

rsync -avh --exclude '*.env' Listjj/ machinejj:/home/jarek/docker/Listjj/Listjj --delete
ansible machinejj -a 'docker rm -f listjj'
ansible machinejj -a 'docker rm -f listjj_migrations_temp'
ansible machinejj -a 'docker image rm listjj_blazor'
ansible machinejj -a 'docker image rm listjj_migrations'
ansible machinejj -a 'docker-compose -f /home/jarek/docker/Listjj/docker-compose.yml up -d'

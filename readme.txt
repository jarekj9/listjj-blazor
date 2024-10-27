
sudo docker build -f Dockerfile -t jarekj9/listjj_blazor_api:latest .
docker push jarekj9/listjj_blazor_api:latest
sudo docker build -f Dockerfile_frontend -t jarekj9/listjj_frontend_blazor:latest .
docker push jarekj9/listjj_frontend_blazor:latest


ansible machinejj -a 'sudo docker-compose -f /home/jarek/docker/Listjj-s1/docker-compose.yml pull'
ansible machinejj -a 'docker service rm listjj-blazor_listjj_api_s1'
ansible machinejj -a 'docker service rm listjj-blazor_listjj_frontend_s1'
ansible machinejj -a 'docker stack deploy -c /home/jarek/docker/Listjj-s1/docker-compose.yml listjj-blazor'

ansible machinejj -a 'docker image prune -af'
docker image prune -af



#arm build:

docker buildx build -f Dockerfile_multiarch --platform linux/arm64 --no-cache -t jarekj9/listjj_blazor_api:arm --push .
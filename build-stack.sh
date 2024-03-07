sudo docker build -f Dockerfile -t jarekj9/listjj_blazor_api:latest .
docker push jarekj9/listjj_blazor_api:latest
sudo docker build -f Dockerfile_frontend -t jarekj9/listjj_frontend_blazor:latest .
docker push jarekj9/listjj_frontend_blazor:latest


docker-compose pull
docker service rm listjj-blazor_listjj_api_s1
docker service rm listjj-blazor_listjj_frontend_s1
docker stack deploy -c docker-compose.yml  listjj-blazor

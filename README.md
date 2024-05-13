Simple docker build (no need docker-compose)

Since docker container runs in its own network, it can' connect to localhost MSSQL and have to make a network bridge, or have to 
include have a mssql server container runs in the same network. (This branch just do with a localhost db)

"DefaultConnection": "Server=host.docker.internal;uid=sa;pwd=12345;database=SecretsSharing;TrustServerCertificate=True"
docker run --network bridge -p 8080:8080 -p 1433:1433 -e "ASPNETCORE_ENVIRONMENT=Development" secretssharing


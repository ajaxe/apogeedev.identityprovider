.PHONY: all

all: docker-backend docker-frontend

docker-backend:
	@echo "Docker build backend"
	@docker build . -f build/Dockerfile --network=host --tag apogee-dev/identity-provider:local

docker-frontend:
	@echo "Docker build frontend"
	@docker build . -f build/Dockerfile.spa --network=host --tag apogee-dev/identity-provider-spa:local

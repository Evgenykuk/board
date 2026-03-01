.PHONY: build up down db-up logs test

build:
	docker-compose build

up:
	docker-compose up -d

down:
	docker-compose down

db-up:
	docker-compose up -d board_db

logs:
	docker-compose logs -f board

test:
	@echo "Тесты пока не реализованы. Запуск в контейнере будет позже."

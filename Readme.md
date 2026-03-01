# 🛫 Board Service (Борт)

Полностью рабочая реализация сервиса управления бортом.

---

## ✅ Возможности
- REST API для UI
- Подключение к PostgreSQL 16
- Приём Kafka-событий (`handling.task.completed`, `flight.taxi.start`)
- Идемпотентность, дедупликация
- Health checks
- Docker + Makefile

---

## 🚀 Как запустить

```bash
make build
make up

# MiddleTask

Микросервисное приложение, которое:
- забирает данные из нестабильного внешнего API (WeakApp),
- прокидывает их через очередь сообщений,
- сохраняет в MongoDB,
- отдаёт данные через **GraphQL + REST**,
- и отправляет **real-time** обновления через **SignalR**.

## Состав

- **WeakApp** (external): нестабильный API (`GET /meters`, header `X-Api-Key: supersecret`)
- **DataIngestorService**: Quartz → HTTP (Polly) → RabbitMQ (MassTransit)
- **DataProccesor**: RabbitMQ (MassTransit) → MongoDB
- **API**: GraphQL (Hot Chocolate) + REST поверх чтения из MongoDB
- **NotificationService**: RabbitMQ consumer → SignalR hub (`/hubs/metrics`)
- **frontend**: React (Vite + TypeScript) + Apollo (GraphQL) + SignalR (реальное время)

## Быстрый старт (Docker Compose)

### Требования

- Docker Desktop / Docker Engine (запущенный демон Docker)
- Git

### 1) Клонировать репозиторий

```bash
git clone <your-repo-url> MiddleTask
cd MiddleTask
```

### 2) Добавить внешний WeakApp

В корне репозитория:

```bash
git clone https://github.com/nantonov/WeakApp WeakApp
```

### 3) Запустить всё одной командой

```bash
docker compose up --build
```

## Полезные адреса

- **Frontend (React)**: `http://localhost:3000`
- **GraphQL API**: `http://localhost:5050/graphql`
- **REST API**:
  - `GET http://localhost:5050/api/metrics`
  - `GET http://localhost:5050/api/metrics/{id}`
  - `GET http://localhost:5050/api/metrics/aggregations/by-type`
- **SignalR hub**: `http://localhost:5001/hubs/metrics`
- **WeakApp**: `http://localhost:8081` (например `GET /meters` с `X-Api-Key`)
- **RabbitMQ UI**: `http://localhost:15672` (guest/guest)
- **MongoDB**: `mongodb://localhost:27017`

## Frontend: как общается с backend

- **GraphQL (Apollo Client)**:
  - URL по умолчанию: `http://localhost:5050/graphql`
  - Query для агрегации: `getMetricsByTypeAggregations`
- **SignalR**:
  - URL по умолчанию: `http://localhost:5001/hubs/metrics`
  - Событие: `MetricReceived` (пока логируется в консоль браузера)

Переменные окружения (опционально):
- `VITE_GRAPHQL_URL`
- `VITE_SIGNALR_URL`
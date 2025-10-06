# InvLabTask

Test task for Invention Laboratory LLC. Разработка системы обработки XML-файлов на основе микросервисной архитектуры (C#, RabbitMQ, SQLite)

# Task

* [Полное описание задачи](Task.md)

# Solution

Этот проект состоит из двух микросервисов на [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0), которые взаимодействуют между собой через [RabbitMQ](https://www.rabbitmq.com/):
* `FileParserService` — парсит входные файлы, преобразует их в JSON и публикует сообщения в очередь RabbitMQ.
* `DataProcessorService` — подписывается на очередь и обрабатывает полученные сообщения.
* `RabbitMQ` — брокер сообщений с веб-интерфейсом для мониторинга.

Все сервисы запускаются и работают внутри Docker-контейнеров с помощью [docker-compose](https://docs.docker.com/compose/).

## Структура проекта

```
InvLabTask/
│
├─ FileParserService/
│   ├─ Dockerfile
│   ├─ FileParserService.csproj
│   └─ ...
│
├─ DataProcessorService/
│   ├─ Dockerfile
│   ├─ DataProcessorService.csproj
│   └─ ...
│
├─ ModelLayer/
│   ├─ ModelLayer.csproj
│   └─ ...
│
├─ docker-compose.yml
└─ InvLabTask.sln

```

## PreRequirements

Перед запуском убедись, что установлены:
* [Docker](https://www.docker.com/get-started/);
* [Docker Compose](https://docs.docker.com/compose/install/);
* [Git](https://git-scm.com/downloads) (если проект клонируется из репозитория).

Проверить версии:

```bash
docker --version
docker compose version
```

## Quick Start

1. Клонируй репозиторий:

```bash
git clone https://github.com/lpestl/InvLabTask.git
cd InvLabTask
```

2. Собери и запусти все контейнеры

```bash
docker compose up --build
```

> [!NOTE] 
> Первый запуск может занять несколько минут, пока Docker скачивает образы и собирает .NET-проекты.

3. ...
4. ProfIT!

## Что произойдёт при запуске

* `RabbitMQ` запустится на `localhost:5672`
   * (веб-интерфейс управления доступен на [http://localhost:15672](http://localhost:15672))
* Логин: user
* Пароль: password
* `FileParserService` подключится к `RabbitMQ` и начнёт публиковать JSON-сообщения в очередь `instrument_status_queue`.
* `DataProcessorService` подключится к той же очереди и начнёт принимать и обрабатывать сообщения.

> [!NOTE] 
> 
> Все изменяемые параметры, такие как Порт для RabbitMQ, имя очереди, логин, пароль и пр. можно задавать в файлах `appsetings.json` (в случае запуска в Docker контейнерах - `appsettings.Docker.json`)

## Сервисы в Docker Compose

| Сервис          |	Назначение                                |	Порты                   |	Зависимости |
|-----------------|--------------------------------------------|--------------------------|--------------|
| rabbitmq        |	брокер сообщений                          |	5672 (AMQP), 15672 (UI)	| —            |
| fileparser      |	публикует JSON в очередь                  |	—                       |	rabbitmq    |
| data-processor  |	слушает очередь и обрабатывает сообщения  |	—                       |	rabbitmq    |

## Проверка работы

1. Открой [RabbitMQ Management UI](http://localhost:15672/)
* В разделе Queues появится очередь instrument_status_queue.
2. Посмотри логи контейнеров:

```bash
docker compose logs -f fileparser
docker compose logs -f data-processor
```

Ты должен увидеть, как первый сервис публикует сообщения, а второй — получает, записывает в SQLite базу данных и выводит их в консоль.

3. В корне проекта должна появится папка `output`, а в ней файл базы данных `InstrumentStatus.sqlite`. Посмотреть содержимое БД и обновлённые данные можно любой удобной тулой, например, [DB Browser for SQLite](https://sqlitebrowser.org/)

## Остановка контейнеров

Для остановки и удаления контейнеров:

```bash
docker compose down
```

Если хочешь очистить тома `RabbitMQ` (удалить данные очередей):

```bash
docker compose down -v
```

## Полезные команды

| Команда                                    |	Описание                                  |
|--------------------------------------------|--------------------------------------------|
| `docker compose ps`                        |	Проверить, какие контейнеры запущены      |
| `docker compose logs -f`                   |	Смотреть логи всех сервисов               |
| `docker exec -it rabbitmq bash`            |	Подключиться внутрь контейнера RabbitMQ   |
| `docker images / docker container ls -a`   |	Просмотр образов и контейнеров            |


## FAQ

Если нужно пересобрать всё заново:

```bash
docker compose down -v --rmi all --remove-orphans
docker compose up --build
```

Если возникли проблемы с запуском или `RabbitMQ` не подключается:
* Проверь, что в коде хост указан как `rabbitmq`, а не `localhost`
* `RabbitMQ` должен быть **“Healthy”** (в `docker ps` будет `healthy`)

Перезапусти сервисы:

```bash
docker compose restart
```
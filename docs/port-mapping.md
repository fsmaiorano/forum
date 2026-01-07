# Port Mapping Documentation

## Microservices

| Microservice | Local HTTP | Local HTTPS | Docker HTTP | Docker HTTPS | Container Internal HTTP |
|--------------|------------|-------------|-------------|--------------|-------------------------|
| Forum        | 5000       | 5050        | 8000        | 8443         | 8080                    |
| Notification | 5001       | 5051        | 8001        | 8444         | 8080                    |
| User         | 5002       | 5052        | 8002        | 8445         | 8080                    |

## Infrastructure Services

| Service         | Local Port | Docker Port | Container Internal Port | Management UI |
|-----------------|------------|-------------|------------------------|---------------|
| ForumDb         | 5433       | 5433        | 5432                   | -             |
| NotificationDb  | 5434       | 5434        | 5432                   | -             |
| UserDb          | 5435       | 5435        | 5432                   | -             |
| RabbitMQ        | 5672       | 5672        | 5672                   | 15672         |

## Usage

### Local Development (Debug)
- Use `http://localhost:5000`, `http://localhost:5001`, `http://localhost:5002` for APIs
- HTTPS available at `https://localhost:5050`, `https://localhost:5051`, `https://localhost:5052`
- Connect to databases using `localhost` and ports 5433, 5434, 5435
- RabbitMQ connection: `amqp://guest:guest@localhost:5672/`
- Configuration files: `appsettings.json` and `appsettings.Development.json`

### Docker Environment
- Use `http://localhost:8000`, `http://localhost:8001`, `http://localhost:8002` for APIs from host machine
- HTTPS available at `https://localhost:8443`, `https://localhost:8444`, `https://localhost:8445`
- **Safe ports used** (8000-8002 for HTTP, 8443-8445 for HTTPS) - no browser blocking
- Inside Docker network, services communicate using container names (e.g., `http://forumapi:8080`, `http://userapi:8080`)
- Database connection from host: `localhost:5433/5434/5435`
- Database connection inside Docker: `forumdb:5432`, `notificationdb:5432`, `userdb:5432`
- RabbitMQ connection inside Docker: `amqp://guest:guest@messagebroker:5672/`
- Configuration files: `appsettings.Docker.json`

### Swagger UI Access
- **Local**: 
  - Forum: http://localhost:5000/swagger
  - Notification: http://localhost:5001/swagger
  - User: http://localhost:5002/swagger

- **Docker**: 
  - Forum: http://localhost:8000/swagger
  - Notification: http://localhost:8001/swagger
  - User: http://localhost:8002/swagger

### RabbitMQ Management UI
- **Local/Docker**: http://localhost:15672
- Default credentials: `guest` / `guest`

## Port Safety Notes

**Why ports 8000-8002 instead of 6000-6002?**
- Chrome and other browsers block certain ports for security reasons
- Ports 6000-6007 are considered "unsafe" by browsers
- Ports 8000+ are safe and won't be blocked
- HTTPS ports 8443-8445 are also safe alternatives to standard 443


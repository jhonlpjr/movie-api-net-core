# 🎬 Movie Recommender API

A RESTful microservice that delivers movie recommendations, built with **.NET Core 8**, **MongoDB Atlas**, and **Redis** for caching, following **Clean Architecture** principles.  
This project is containerized with **Docker**, configured for **multi-environment deployments** (`local`, `dev`, `qa`, `prod`), and ready to run on **AWS EC2**, ECS, or any container-based platform.

---

## 📌 Features

- **Clean Architecture** separation (Domain, Application, Infrastructure, API)
- **MongoDB Atlas** as the primary NoSQL database
- **Redis caching** for popular/recommended queries
- Multi-environment support with `.env` configuration
- Dockerized and ready for cloud deployment
- RESTful API with Swagger documentation

---

## 🛠️ Tech Stack

**Backend**
- .NET Core 9 (Web API)
- MongoDB Atlas
- Redis

**Infrastructure**
- Docker & Docker Compose
- Multi-environment configuration
- AWS-ready (EC2, ECS, or Fargate)

---

## 📁 Project Structure

```
src/
├── MovieApi/                  # API Layer (Controllers, DTOs)
├── Application/               # Use Cases
├── Domain/                    # Entities & Interfaces
├── Infrastructure/            # Mongo & Redis implementations
docker/
├── local/                     # Local compose files
└── prod/                      # Production Dockerfile
env/
├── .env.local
├── .env.dev
├── .env.qa
└── .env.prod
```

---

## 🔌 API Endpoints

| Method | Endpoint                       | Description                                  |
|--------|---------------------------------|----------------------------------------------|
| GET    | `/api/movies`                   | List all movies                              |
| GET    | `/api/movies/{id}`              | Get movie details                            |
| GET    | `/api/movies/search?query=...`  | Search movies by title or genre              |
| GET    | `/api/movies/popular`           | Get popular movies (cached in Redis)         |
| GET    | `/api/movies/recommendations`   | Get recommended movies (cached in Redis)     |
| POST   | `/api/movies`                   | Add a new movie (demo purpose)               |

---

## 🚀 Running Locally

### **Option 1: Using Make (recommended)**

```bash
make up ENV=local
```

Other commands:
```bash
make logs ENV=local     # View API logs
make down ENV=local     # Stop containers
```

---

### **Option 2: Using Docker Compose directly**

```bash
ENV=local docker-compose -f docker/docker-compose.yml up --build -d
```

---

## 🔧 Environment Variables

Located in the `/env` folder:

```
ENVIRONMENT=local
PORT=80
MONGO_URI=mongodb://mongo:27017
REDIS_HOST=redis
LOG_LEVEL=debug
```

Change `ENV=dev` or `ENV=prod` to switch environments.

---

## 📦 Deployment

### AWS EC2
1. Build the Docker image:
   ```bash
   docker build -t movie-api -f docker/prod/Dockerfile .
   ```
2. Run the container on EC2:
   ```bash
   docker run -d -p 80:80 --env-file env/.env.prod movie-api
   ```

### AWS ECS (Fargate)
- Push the image to ECR
- Deploy with ECS task definitions
- Configure env variables per environment

---

## 📜 License
This project is open-source and available under the [MIT License](LICENSE).

---

## 👨‍💻 Author
**Jonathan Reyna**  
Software Engineer | Architecture Specialist  
[Portfolio Website](https://jhonlpjr.github.io/) | [GitHub](https://github.com/jhonlpjr) | [LinkedIn](https://www.linkedin.com/in/jonathan-reyna-rossel-889195168/)

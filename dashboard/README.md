# Vue 3 + Vite + TypeScript + Docker

A modern Vue 3 application with Vite build tool, TypeScript support, and complete Docker setup for both development and production environments.

## 🚀 Features

- **Vue 3** - Latest Vue framework with Composition API
- **Vite** - Lightning-fast build tool with instant HMR
- **TypeScript** - Full type safety and better developer experience
- **Docker** - Development and production containerization
- **Docker Compose** - Easy orchestration of services
- **Nginx** - Production-ready web server
- **Multi-stage Builds** - Optimized production images (~25MB)

## 📋 Prerequisites

- Docker & Docker Compose installed
- Node.js 18+ (for local development without Docker)
- npm or yarn package manager

## 🏗️ Project Structure

```
vue-docker-app/
├── src/                          # Vue application source
│   ├── assets/                  # Static assets
│   ├── components/              # Vue components
│   ├── App.vue                  # Root component
│   └── main.ts                  # Application entry point
├── public/                       # Public static files
├── docker/                       # Docker configuration
│   ├── Dockerfile               # Production Dockerfile
│   ├── Dockerfile.dev           # Development Dockerfile
│   └── nginx/
│       └── nginx.conf           # Nginx configuration
├── docker-compose.yml           # Docker Compose configuration
├── .dockerignore                # Docker build exclusions
├── .env.example                 # Environment variables template
├── vite.config.ts               # Vite configuration
├── tsconfig.json                # TypeScript configuration
└── package.json                 # Project dependencies
```

## 🐳 Docker Setup

### Development Environment

Run the development server with hot reload inside Docker:

```bash
docker-compose up dev
```

The application will be available at `http://localhost:5173`

**Features:**
- Hot Module Replacement (HMR) enabled
- Volume mounting for real-time code synchronization
- Automatic server restart on file changes
- Full TypeScript support

### Production Environment

Build and run the production-optimized application:

```bash
docker-compose up prod --build
```

The application will be available at `http://localhost`

**Features:**
- Multi-stage build (Node.js → Nginx)
- Optimized bundle (~25MB image)
- Gzip compression enabled
- Static asset caching
- SPA routing configured

### Stop Services

```bash
docker-compose down
```

## 💻 Local Development (Without Docker)

### Installation

```bash
cd vue-docker-app
npm install
```

### Development Server

```bash
npm run dev
```

Access the application at `http://localhost:5173`

### Build for Production

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

## 📦 Available Scripts

```bash
# Development
npm run dev              # Start Vite dev server

# Production
npm run build            # Build optimized production bundle
npm run preview          # Preview production build locally

# Type checking
npm run type-check       # Check TypeScript types

# Linting (if configured)
npm run lint             # Run ESLint
npm run format           # Format code with Prettier
```

## 🔧 Configuration Files

### vite.config.ts

Vite configuration with Docker-specific settings:
- Host binding to `0.0.0.0` for Docker access
- File watching with polling for Docker compatibility
- Vue 3 plugin enabled

### docker-compose.yml

Orchestrates two services:

**dev service:**
- Uses `Dockerfile.dev`
- Mounts source code as volume
- Port: 5173
- Enables hot reload

**prod service:**
- Uses production `Dockerfile`
- Multi-stage build
- Port: 80
- Optimized for deployment

### docker/nginx/nginx.conf

Nginx configuration for production:
- SPA routing (all routes serve index.html)
- Gzip compression
- Static asset caching (1 year)
- Security headers

### .env.example

Environment variables template. Copy to `.env` and customize:

```bash
cp .env.example .env
```

## 🚢 Deployment

### Docker Hub

Build and push to Docker Hub:

```bash
# Build production image
docker build -f docker/Dockerfile -t your-username/vue-docker-app:latest .

# Push to Docker Hub
docker push your-username/vue-docker-app:latest
```

### Kubernetes

The Docker image is compatible with Kubernetes:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: vue-docker-app
spec:
  replicas: 3
  selector:
    matchLabels:
      app: vue-docker-app
  template:
    metadata:
      labels:
        app: vue-docker-app
    spec:
      containers:
      - name: vue-docker-app
        image: your-username/vue-docker-app:latest
        ports:
        - containerPort: 80
```

### Cloud Platforms

The application can be deployed to:
- **AWS ECS/Fargate** - Container orchestration
- **Google Cloud Run** - Serverless containers
- **Azure Container Instances** - Managed containers
- **DigitalOcean App Platform** - Simple deployment
- **Heroku** - With Docker support

## 📊 Performance

### Development
- **HMR Speed**: <100ms
- **Initial Startup**: 2-3 seconds
- **Memory Usage**: ~200-300MB

### Production
- **Build Time**: 30-60 seconds
- **Image Size**: ~25-30MB
- **Runtime Memory**: ~10-20MB
- **Startup Time**: <1 second

## 🔒 Security Best Practices

- ✅ Non-root user in Nginx
- ✅ Minimal Alpine base images
- ✅ No build tools in production
- ✅ Environment variables for sensitive data
- ✅ Regular dependency updates

## 🐛 Troubleshooting

### Hot reload not working in Docker

If HMR is not working, ensure `usePolling: true` is set in `vite.config.ts`:

```typescript
server: {
  watch: {
    usePolling: true,
  },
}
```

### Port already in use

Change the port in `docker-compose.yml`:

```yaml
ports:
  - "5174:5173"  # Use 5174 instead of 5173
```

### Build fails in Docker

Clear Docker cache and rebuild:

```bash
docker-compose down
docker system prune -a
docker-compose up prod --build
```

### Node modules issues

Rebuild node_modules in container:

```bash
docker-compose down
docker volume prune
docker-compose up dev
```

## 📚 Resources

- [Vue 3 Documentation](https://vuejs.org/)
- [Vite Documentation](https://vitejs.dev/)
- [Docker Documentation](https://docs.docker.com/)
- [TypeScript Documentation](https://www.typescriptlang.org/)
- [Nginx Documentation](https://nginx.org/en/docs/)

## 📝 License

MIT License - feel free to use this project as a template

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Support

For issues and questions, please open an issue on GitHub.

---

**Happy coding! 🎉**

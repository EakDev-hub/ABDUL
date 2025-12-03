# Multi-stage production Dockerfile
# Stage 1: Build
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Stage 2: Serve with Nginx + Node.js env server
FROM nginx:alpine

# Install Node.js and npm in nginx image
RUN apk add --no-cache nodejs npm

# Create app directory for env server
WORKDIR /app

# Copy package.json and install only crypto-js for env server
RUN npm init -y && npm install crypto-js

# Copy env server script
COPY ./docker/env-server.js /app/env-server.js

# Copy nginx configuration
COPY ./docker/nginx/nginx.conf /etc/nginx/conf.d/default.conf

# Copy built application from builder stage
COPY --from=builder /app/dist /usr/share/nginx/html

# Copy startup script
COPY ./docker/start.sh /start.sh
RUN chmod +x /start.sh

# Expose port 80
EXPOSE 80

# Start both nginx and env server
CMD ["/start.sh"]
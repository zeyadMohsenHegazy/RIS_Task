# ---- Build stage ----
FROM node:20-alpine AS build

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

# ---- Runtime stage ----
FROM nginx:1.27-alpine AS final

RUN apk add --no-cache sed

COPY nginx.conf /etc/nginx/nginx.conf
COPY public/env.js.template /etc/nginx/templates/env.js.template
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN sed -i 's/\r$//' /docker-entrypoint.sh && chmod +x /docker-entrypoint.sh

COPY --from=build /app/dist/smart-inventory-ui/browser /usr/share/nginx/html

ENV API_URL=http://localhost:8080/api
ENV APP_NAME="Smart Inventory"

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget -qO- http://127.0.0.1/ >/dev/null || exit 1

ENTRYPOINT ["/docker-entrypoint.sh"]

FROM node:20-alpine AS build
WORKDIR /app

COPY frontend/package.json frontend/tsconfig.json frontend/vite.config.ts frontend/index.html ./
COPY frontend/src ./src

RUN npm install
RUN npm run build

FROM nginx:alpine AS final

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80


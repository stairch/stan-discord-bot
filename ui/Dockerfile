FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:stable-alpine AS production
COPY --from=build /app/dist /usr/share/nginx/html
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf:ro
COPY ./nginx/conf.d /etc/nginx/conf.d:ro
VOLUME /var/www/common:ro
EXPOSE 80
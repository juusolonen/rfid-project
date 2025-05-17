FROM node:14.16.0-slim AS builder
#RUN npm install -g react-scripts --silent
COPY package.json yarn.lock ./
RUN yarn install --frozen-lockfile
COPY . .

ARG REACT_APP_BE_HOST
ENV REACT_APP_BE_HOST=$REACT_APP_BE_HOST


#RUN yarn install
RUN yarn run build

FROM nginx:stable

COPY --from=builder /build /usr/share/nginx/html
COPY nginx.default.conf /etc/nginx/conf.d/default.conf


RUN chown -R nginx:nginx /usr/share/nginx/html && chmod -R 755 /usr/share/nginx/html && \
        chown -R nginx:nginx /var/cache/nginx && \
        chown -R nginx:nginx /var/log/nginx && \
        chown -R nginx:nginx /etc/nginx/conf.d
RUN touch /var/run/nginx.pid && \
        chown -R nginx:nginx /var/run/nginx.pid

USER nginx
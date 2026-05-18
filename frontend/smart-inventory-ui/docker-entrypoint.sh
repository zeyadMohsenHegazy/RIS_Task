#!/bin/sh
set -eu

API_URL="${API_URL:-http://localhost:8080/api}"
APP_NAME="${APP_NAME:-Smart Inventory}"

# Escape sed replacement delimiters in values
escape_sed() {
  printf '%s' "$1" | sed -e 's/[&/\]/\\&/g'
}

API_URL_ESCAPED=$(escape_sed "$API_URL")
APP_NAME_ESCAPED=$(escape_sed "$APP_NAME")

sed \
  -e "s|\${API_URL}|${API_URL_ESCAPED}|g" \
  -e "s|\${APP_NAME}|${APP_NAME_ESCAPED}|g" \
  /etc/nginx/templates/env.js.template \
  > /usr/share/nginx/html/env.js

echo "Runtime config: API_URL=${API_URL} APP_NAME=${APP_NAME}"

exec nginx -g 'daemon off;'

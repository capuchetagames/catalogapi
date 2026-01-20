#!/bin/bash

PATH_FILE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${PATH_FILE}/env.sh"

echo "Build image..."
docker build -t ${APPNAME}-api:latest ${PATH_FILE}/../

echo "Apply secrets and configmap..."
kubectl apply -f ${PATH_FILE}/${APPNAME}-secret.yaml
kubectl apply -f ${PATH_FILE}/${APPNAME}-configmap.yaml

echo "Deploy ${APPNAME}..."
kubectl apply -f ${PATH_FILE}/${APPNAME}-pod.yaml
kubectl apply -f ${PATH_FILE}/${APPNAME}-service.yaml

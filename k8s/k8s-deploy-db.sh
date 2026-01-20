#!/bin/bash

PATH_FILE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${PATH_FILE}/env.sh"

echo "Deploy ${APPNAME} DB..."

kubectl apply -f ${PATH_FILE}/${APPNAME}-secret.yaml
kubectl apply -f ${PATH_FILE}/${APPNAME}-configmap.yaml
kubectl apply -f ${PATH_FILE}/sql-deployment.yaml
kubectl apply -f ${PATH_FILE}/sql-service.yaml
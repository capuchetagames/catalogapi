#!/bin/bash

source env.sh

echo "Deploy ${APPNAME} DB..."

kubectl apply -f ${APPNAME}-secret.yaml
kubectl apply -f ${APPNAME}-configmap.yaml
kubectl apply -f sql-deployment.yaml
kubectl apply -f sql-service.yaml
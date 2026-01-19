#!/bin/bash

source env.sh


echo "Build image..."
docker build -t ${APPNAME}-api:latest ../

echo "Apply secrets and configmap..."
kubectl apply -f ${APPNAME}-secret.yaml
kubectl apply -f ${APPNAME}-configmap.yaml

echo "Deploy ${APPNAME}..."
kubectl apply -f ${APPNAME}-deployment.yaml
kubectl apply -f ${APPNAME}-service.yaml
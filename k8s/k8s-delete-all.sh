#!/bin/bash

PATH_FILE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${PATH_FILE}/env.sh"

kubectl delete deployment ${APPNAME}-db
kubectl delete service ${APPNAME}-db

kubectl delete pod ${APPNAME}-api
kubectl delete deployment ${APPNAME}-api
kubectl delete service ${APPNAME}-api

kubectl delete configmap ${APPNAME}-config

kubectl delete secret ${APPNAME}-secret
#!/bin/bash

PATH_FILE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

source "${PATH_FILE}/k8s-deploy-db.sh"

source "${PATH_FILE}/k8s-deploy-api.sh"
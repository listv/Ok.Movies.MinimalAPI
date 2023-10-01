#!/bin/bash

# Fetch GitHub Secrets
DATABASE__CONNECTIONSTRING="$1"
POSTGRES_PASSWORD="$2"
POSTGRES_DB="$3"
#SECRET_2="$2"
# Add more secrets as needed

# Generate the environment file
echo "DATABASE__CONNECTIONSTRING=$DATABASE__CONNECTIONSTRING" > .env
echo "POSTGRES_PASSWORD=$POSTGRES_PASSWORD" >> .env
echo "POSTGRES_DB=$POSTGRES_DB" >> .env
#echo "SECRET_2=$SECRET_2" >> .env
# Add more secrets as needed

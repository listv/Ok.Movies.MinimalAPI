#!/bin/bash

# Fetch GitHub Secrets
DATABASE__CONNECTIONSTRING="$1"
#SECRET_2="$2"
# Add more secrets as needed

# Generate the environment file
echo "DATABASE__CONNECTIONSTRING=$DATABASE__CONNECTIONSTRING" > .env
#echo "SECRET_2=$SECRET_2" >> .env
# Add more secrets as needed

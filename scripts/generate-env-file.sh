#!/bin/bash

# Fetch GitHub Secrets
DATABASE__CONNECTIONSTRING="${{ secrets.DB_CONNECTION_STRING }}"
#SECRET_2="${{ secrets.SECRET_2 }}"
# Add more secrets as needed

# Generate the environment file
echo "DATABASE__CONNECTIONSTRING=$DATABASE__CONNECTIONSTRING" > generated-env-file.env
#echo "SECRET_2=$SECRET_2" >> generated-env-file.env
# Add more secrets as needed

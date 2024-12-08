using './listjj-api-webapp-docker.bicep'

param appName = 'listjj-api'
param location = 'westeurope'
param mssqlConnStringSecret = $(mssqlConnString) 

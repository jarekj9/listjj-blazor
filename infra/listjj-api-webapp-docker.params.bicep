using './listjj-api-webapp-docker.bicep'

param appName = 'listjj-api'
param location = 'esteurope'
param mssqlConnStringSecret = $(mssqlConnString) 

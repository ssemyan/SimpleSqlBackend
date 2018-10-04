# SimpleSqlBackend
A simple SQL backend to store items in a SQL database

This WebAPI backend can be used with a simple website like [SimpleFrontEnd](https://github.com/ssemyan/SimpleFrontEnd)

To use, change the setting for *DbConnectionString* in *Web.config* to match your local or remote DB connection string. In Azure App Service, you can also create an application setting called *DbConnectionString* to override the value in *Web.config* so you do not have to 
store the connection string in a config file. 

To use in Azure, create an App Service and SQL DB. Then create the user and table in the DB using the *schema.sql* file. Finally use the connection string for the DB in *Web.config* or the application settings.

To use Managed Service Identity, enable it in the Web App, then add the database connection string as a secret in a new or existing KeyVault. Set an access policy to enable *GET* access to secrets for your web app's identity (usually given the name of the web app), and add the URI to the secret
to the *Web.config* or an application setting called *DbConnStrKeyVaultSecretUri*.

# Saritasa Code Exercise Round

This is a 2 weeks code exercise about a sharing secrets ASP.NET API application using MSSQL for storage. 
Users can upload message or file, and an anonymous user can get the URL to the resource to access it.

## Run the app
### Docker
Remember to login to your Docker account on console.
```bash
docker pull miterai/saritasa:latest
```
You can customize the SQL Server database connection string or your own AWS key secrets through the command line. The ASPNETCORE_ENVIRONMENT=Development is for the Swagger UI, can be set to Production if you don't need it.
```bash
docker run -d --name your-app-name -p {your-port}:8080 -e DATABASE_URL="Server=host.docker.internal;uid=sa;pwd=12345;database=SecretsSharing;TrustServerCertificate=True" -e AWS_ACCESS_KEY=your-access-key -e AWS_SECRET_KEY=your-secret-key -e BUCKET_NAME=your-bucket-name -e ASPNETCORE_ENVIRONMENT=Development miterai/saritasa:latest
```
You can also exclude the AWS environment variables to use the default config of the image which can still connect to an S3 bucket instance. 


Or you can config and build your own Docker image using the provided Dockerfile.

Access http://localhost:{your-port} for your app, include **/swagger** if you enable development mode.
### Code
Open the app on your IDE of choice, change the settings in appsettings.json then build the project and **dotnet run**

### Login to use API
The app has on seeded user with email: admin@gmail.com and password: admin123. So you can login using this account or register a new one.

## Approach for the requirements 
- The resource URL should not be simple like upload/1, upload/2 -> Used GUID for upload Id and return the resource Url on the response header in Swagger UI
- The user can upload either a message or a file on the same API -> Check if file is present then change upload type to FILE and MESSAGE accordingly
- When auto-delete is set to true, must be able to prevent 2+ users from accessing the resource at the same time before it is deleted -> Used isolation level when DELETE to prevent row being read while on DELETE
- Must be able to handle duplicate file name on AWS S3 bucket -> Turn on versioning on S3, when upload file to S3 then also save the versionId to database




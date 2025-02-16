# RateLimiting
 Rate limiting web API with Basic Authentication and Jwt Authorization (Cybercrypt interview project).

* The app registers new users in an in-memory database at the 
 `"POST /random/register"` endpoint, uses HTTP Basic Authentication to verify log-in requests of registered users then generates and assigns JWTs to each user at the `"GET /random/authenticate"` endpoint. 

* The user can then use Bearer Token Authentication to access the `"GET /random"` (or `GET "/random?len=xx"`) endpoint to request a variable length Base64 encoded random string (default length 32). 

* Each user is assigned a byte rate limit and every request by a specific user reduces that user's bytes by the corresponding request's length. Users can have different rate limits and every user's rate is reset every 10 seconds.

* An admin can modify the rate limit for a specified user from the `"POST /random/modify"` endpoint 

* The requests are executed asynchronously.

* The app uses HSTS headers and HTTPS redirection to supplement Basic Authentication, but a port must be available for the middleware to redirect an insecure request to HTTPS. If no port is available redirection to HTTPS doesn't occur.

## Execution
 This is a C# .Net Core 3.1 project, it can be built & run instantly by importing it in Visual Studio (Code), or simply using `dotnet run` from the project's root directory.

## Testing
 There's a `testing` branch for unit/integration testing but didn't have time to implement working tests

 During development I tested request functionality using Postman, I have included 7 example requests (register, authenticate and random for 2 different users and the admin modify request) in a Postman collection file.

 Example test with Postman:
 1. Send a request to `"POST /random/register"`, with a json object including at least `"Username"` and `"Password"` parameters. The user should be registered to the database and the response will include an Id for that user. ("Register User" request)
 2. Use the credentials from 1. and Basic Authentication to access `"GET /random/authenticate"`, log-in and get a JWT in the response. ("Authenticate User" request)
 3. Use the JWT from 2. and Bearer Token Authentication to access `"GET /random"`. The response will be a message ("Get random" request)
 4. Use the Id of a registered user from 1. in the body of a request to `"POST /random/modify"` to change the rate limit for that user. ("Modify Rate" request)  

## Assumptions
* Using an in memory database to store users, let's assume we don't care about duplicate usernames/passwords. Basic Authentication succeeds as long as the provided credentials are valid.

* No specified authentication method for admin endpoint requests, will use a preset admin user and the endpoint will require Basic Authentication.

* Followed the specifications for the requests' messages and headers as close as I could, but for unspecified stuff I used arbitrary headers and responses.

* No regard for JWT amount per User.

* We don't care about padding of the Base64 encoded random string

## Inspiration 
* https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api#app-settings-json
* https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio-code#scaffold-a-controller
* https://codeburst.io/adding-basic-authentication-to-an-asp-net-core-web-api-project-5439c4cf78ee
* https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/basic-authentication
# RateLimiting
 Rate limiting web API with Basic Authentication and Jwt Authorization (Cybercrypt interview project).

 <para>The app registers new Users in an in-memory database at the 
 `"POST /random/register"` endpoint, uses HTTP Basic Authentication to verify log-in requests of registered users then generates and assigns JWTs to each user at the `"POST /random/authenticate"` endpoint. </para> 

 <para>The user can then use Bearer Token Authentication to access the `"GET /random"` (or `GET "/random?len=xx"`) endpoint to request a variable length Base64 encoded random string (default length 32). </para> 

## Execution
 This is a C# .Net Core project, it can be built & run instantly by importing it in Visual Studio (Code), or simply using `dotnet run` from the project's root directory.

## Testing
During development I tested requests using Postman, I have included relevant requests in a .txt file.

## Assumptions
* Using an in memory database to store users, let's assume we don't care about duplicate usernames/passwords. Basic Authentication succeeds as long as the provided credentials are valid.

* Basic Authentication is done over HTTPS

* Followed the specifications for the requests' messages and headers as close as I could, but for unspecified stuff I used arbitrary headers and responses.

* No regard for JWT amount per User.

* We don't care about padding of the Base64 encoded random string

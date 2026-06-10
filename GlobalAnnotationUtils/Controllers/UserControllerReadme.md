## API Response

`
GET /api/users/1
{
  "id": 1,
  "name": "Sunny"
}
`

`
GET /api/users/10
{
  "code": "User.NotFound",
  "description": "User not found."
}
`

## Flow

```
Controller
     │
     ▼
UserService
     │
     ▼
UserRepository
     │
     ▼
Database

Repository returns User? (or null)
            │
            ▼
Service converts to Result.Success() or Result.Failure(UserErrors.NotFound)
            │
            ▼
Controller maps Result to HTTP 200 or 404
```
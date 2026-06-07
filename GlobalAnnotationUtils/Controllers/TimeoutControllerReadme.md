## TO Check the Timeout Controller, then run project in without debugginh mode

Call:

```
GET https://localhost:7184/api/Timeout/timeout
```

Expected result after ~2 seconds:

408 Request Timeout

The middleware cancels the request and returns a timeout response.
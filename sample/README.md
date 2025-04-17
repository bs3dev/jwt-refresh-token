# JWT Refresh Token – Sample API

This sample demonstrates how to use the `Jwt.Refresh.Token.Cosmos` NuGet package in a minimal API setup using .NET 9.

It includes:
- ✅ JWT token creation, refresh and revoke endpoints
- 🔐 Full antiforgery protection (XSRF) support
- 🔄 Postman collection for testing the full flow

---

## 🚀 How to Run Locally

Make sure you have the [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) installed.

```bash
cd sample/Jwt.Refresh.Token.Sample.Ui.Api

dotnet run
```

By default, the API runs on:

```
https://localhost:5001
http://localhost:5000
```

---

## 📬 Testing with Postman

We’ve included a full Postman Collection to test the authentication flow:

📄 [`JwtRefreshToken_Antiforgery.postman_collection.json`](./JwtRefreshToken_Antiforgery.postman_collection.json)

### 🧪 Test Flow (via Postman)

1. **Create Token**  → `POST /token`
   - Uses form-data with `userId` and `password`
   - Returns: `accessToken`, `tokenId`, antiforgery cookie and header

2. **Refresh Token** → `PATCH /token`
   - Requires: `Authorization` header (Bearer token)
   - Requires: `X-XSRF-TOKEN` header and valid cookie

3. **Revoke Token** → `PATCH /revoke`
   - Same requirements as Refresh

### ⚠️ Tips

- Clear Postman cookies **before generating a new token**, or the antiforgery validation may fail.
- Confirm the `X-XSRF-TOKEN` header and cookie `.AspNetCore.Antiforgery.*` are both present in refresh and revoke requests.
- Use the Postman Console to debug headers and cookies easily.

### 👤 Test Credentials

Use the following credentials to test the authentication flow:

```
userId: test@bs3.dev
password: test-1234
```

---

## 🧱 Folder Structure
```
sample/
├── Jwt.Refresh.Token.Sample.Ui.Api/      // Minimal API project
├── JwtRefreshToken_Antiforgery.postman_collection.json
└── README.md                              // You're here 😉
```

---

## 🔗 Main Repository
Go back to the main project here:  
👉 [github.com/bs3dev/jwt-refresh-token](https://github.com/bs3dev/jwt-refresh-token)

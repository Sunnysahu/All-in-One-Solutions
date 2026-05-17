
# WebSocket Testing in Postman (Quick Guide)

## Important Note
WebSocket testing will work in Postman. Do NOT rely on Swagger or browser for WebSocket testing.

---

# 1. Open Postman WebSocket Client

### Steps:
1. Open **Postman**
2. Click **New** (top left)
3. Select **WebSocket Request**

---

# 2. Connect to WebSocket Server

### Steps:
1. Paste your WebSocket URL
2. Replace `https://` with `wss://` (secure WebSocket)

### Example:

```
wss://localhost:5001/connect
```

3. Click **Connect / Run**

---

# 3. Sending Messages

After connection is successful:

- Type message in the input box
- Click **Send**
- You will see responses in the same window

---

# 4. Using Subprotocol (If Required)

If your server uses a **WebSocket subprotocol**, you must add it manually.

### Steps:

1. Go to **Headers** tab in Postman
2. Add the following header:

```
Sec-WebSocket-Protocol: protocol-name
```

---

### Example:

```
Sec-WebSocket-Protocol: chat
```

---

# 5. What Happens During Connection

### Step 1: HTTP Upgrade Request
```
GET /connect HTTP/1.1
Upgrade: websocket
Connection: Upgrade
```

### Step 2: WebSocket Established
- Connection becomes persistent
- No more HTTP request/response cycle

---

# 6. Key Points

- WebSocket is NOT HTTP after connection upgrade
- Messages are sent as frames, not requests
- Each message is not stored in HTTP history
- Postman WebSocket tab handles live stream only

---

# 7. Common Mistakes

❌ Using `https://` instead of `wss://`  
❌ Trying Swagger for WebSocket testing  
❌ Forgetting subprotocol header when required  
❌ Expecting request/response like REST API  

---

# 8. Summary

```
Postman → New → WebSocket Request → Connect → Send Messages
```

If subprotocol is required:

```
Add Header → Sec-WebSocket-Protocol → protocol-name
```

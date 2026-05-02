# 🚀 Large File Upload API (.NET 8)

This project implements **resumable large file upload (2GB+)** using:

* Chunk-based upload
* SQL Server (manual DB, no migrations)
* Repository + Service pattern
* Supports unordered & parallel upload (V2)

---

# 📁 Project Features

* Upload large `.mp4` files in chunks
* Resume upload if interrupted
* Prevent duplicate chunks
* Merge chunks into final file
* Optimized DB calls
* Clean architecture (Repo + Service)

---

# 📦 Folder Structure

```
uploads/
   ├── finalFile.mp4        # merged file
   └── {fileId}/            # chunk folder
        ├── {fileId}_0
        ├── {fileId}_1
        ├── ...
```

---

# ⚙️ API Endpoints

---

## 1️⃣ Initialize Upload

### 📌 Endpoint

```
POST /api/file/init
```

### 📥 Params (form-data OR query)

```
fileName: video.mp4
fileSize: 2000000000
totalChunks: 10
```

### 📤 Response

```
{
  "fileId": 1
}
```

---

## 2️⃣ Upload Chunk (V1 - Sequential Only)

### 📌 Endpoint

```
POST /api/file/upload
```

### ⚠️ NOTE

* Requires chunks in order (0 → N)
* Uses append method

### 📥 Body (form-data)

```
fileId: 1
chunkIndex: 0
chunk: (file chunk)
```

---

## 3️⃣ Upload Chunk (V2 - Recommended)

### 📌 Endpoint

```
POST /api/file/upload-chunk-v2
```

### ✅ Supports

* Unordered upload
* Parallel upload
* Retry safe

### 📥 Body (form-data)

```
fileId: 1
chunkIndex: 0
chunk: (file chunk)
```

---

## 4️⃣ Merge File (V2)

### 📌 Endpoint

```
POST /api/file/merge-v2?fileId=1
```

### 🔥 What it does

* Reads all chunks in order
* Merges into final file
* Deletes chunks after processing

### 📤 Response

```
{
  "status": "File merged successfully"
}
```

---

# 🧪 Postman Testing Flow

---

## ✅ Step 1 — Init Upload

```
POST /api/file/init
```

Get:

```
fileId
```

---

## ✅ Step 2 — Upload Chunks

```
POST /api/file/upload-chunk-v2
```

Repeat for all chunks:

```
chunkIndex = 0 → N
```

---

## ✅ Step 3 — Merge File

```
POST /api/file/merge-v2?fileId=1
```

---

# ⚠️ Important Rules

* Chunk index must start from **0**
* Total chunks must match exactly
* Only `.mp4` files allowed
* Duplicate chunks are ignored
* Merge fails if any chunk is missing

---

# 🚀 Parallel Upload (Example JS)

```javascript
const uploadChunk = async (fileId, chunk, index) => {
    const formData = new FormData();
    formData.append("fileId", fileId);
    formData.append("chunkIndex", index);
    formData.append("chunk", chunk);

    return fetch("/api/file/upload-chunk-v2", {
        method: "POST",
        body: formData
    });
};

await Promise.all(chunks.map((chunk, i) => uploadChunk(1, chunk, i)));
```

---

# ⚡ Optimizations Implemented

* Reduced DB calls (5 → 2 per chunk)
* Removed unnecessary queries
* Atomic updates (recommended)
* Unique constraint prevents duplicate chunks

---

# 🧠 Future Improvements (Planned)

* Auto merge after last chunk
* Missing chunk detection API
* Resume upload API
* Progress tracking endpoint
* Distributed upload support (multi-server)

---

# 🔥 Summary

| Version | Type        | Supports             |
| ------- | ----------- | -------------------- |
| V1      | Append      | Ordered only         |
| V2      | Chunk-based | Parallel + Resumable |

---

# 👨‍💻 Author

Sunny Sahu

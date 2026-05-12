
# RabbitMQ Producer & Consumer Console App (Important Notes)

## 1. Start RabbitMQ using Docker

Run this command:

```bash
docker run -d --name rabbitmqOG `
-p 5672:5672 `
-p 15672:15672 `
rabbitmq:3-management
````

### RabbitMQ UI

Open:
[http://localhost:15672](http://localhost:15672)

Login:

* Username: guest
* Password: guest

---

## 2. Key Concept

* **Producer** → Sends messages to queue
* **Consumer** → Reads messages from queue
* **Queue** → Stores messages until consumed

---

## 3. Important Behavior

### Case 1: Producer runs first

If you run the producer first and consumer is NOT running:

* All messages are stored in RabbitMQ queue
* Nothing is lost

---

### Case 2: Consumer starts later

When you start the consumer later:

* It first consumes all previously stored messages
* Then continues consuming new messages in real-time

---

## 4. Producer Flow

* Sends messages to `demo-queue`
* Sends 10 messages
* Delay: 2 seconds between each message

---

## 5. Consumer Flow

* Listens to `demo-queue`
* Prints received messages instantly
* Continuously waits for new messages

---

## 6. Core Idea

RabbitMQ guarantees:

> Messages stay in the queue until a consumer processes them.

This enables:

* Async communication
* Reliable message delivery
* Decoupled systems

---

## 7. Test Summary

1. Run Producer only → messages go to queue
2. Run Consumer later → old messages are consumed first
3. Then live messages are consumed instantly

---

## 8. Conclusion

This setup demonstrates the **basic Producer–Consumer pattern using RabbitMQ**, which is widely used in microservices and distributed systems.

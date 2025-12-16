# Technical Challenge: The Inbox Zero Engine

## 1. Context & Goal

Users are overwhelmed by incoming messages and need help identifying what is truly important. Your task is to build a "Smart Inbox" engine that accepts a stream of raw emails, calculates an **Urgency Score** for each, and displays them to the user sorted by priority.

This challenge evaluates your ability to:

* Design and implement business logic in **.NET**.

* Expose that logic via a RESTful **Web API**.

* Consume that API using a **JavaScript** frontend.

## 2. Scope & Requirements

### Part A: The Backend (.NET)

Create a .NET 9 Web API that adheres to the following:

**1. Data Model**

The input for an email will contain:

* `Sender` (string)

* `Subject` (string)

* `Body` (string)

* `ReceivedAt` (DateTime)

* `IsVIP` (boolean)

**2. The Scoring Logic**

Implement a service that assigns a Priority Score (Integer, 0-100) based on these rules:

* **VIP Status:** +50 points if `IsVIP` is true.

* **Urgency Keywords:** +30 points if the Subject contains "Urgent", "ASAP", or "Error" (case-insensitive).

* **Time Decay:** +1 point for every hour passed since `ReceivedAt` (older messages become more urgent).

* **Spam Filter:** -20 points if the Body contains "Unsubscribe" or "Newsletter".

* **Clamping:** The final score must not exceed 100 or drop below 0.

**3. API Endpoint**

* **Route:** `POST /api/inbox/sort`

* **Input:** A JSON array of email objects.

* **Output:** The same list of objects, but with an added `PriorityScore` field, sorted by score (Highest to Lowest).

**4. Architecture Constraint**

* **Do not** place the scoring logic directly inside the Controller.

* Use **Dependency Injection** to manage your scoring service.

---

### Part B: The Frontend (JavaScript)

Create a simple web interface (Vanilla JS, React, or Vue) that connects to your API.

1.  **Mock Data:** Include a button or mechanism to load a list of mock emails (see JSON Payload below).

2.  **Integration:** Send the raw data to your .NET backend for processing.

3.  **Display:** Render the sorted results returned by the API.

    * Show **Subject**, **Sender**, and **Priority Score**.

    * **Visual Cues:**

        * High Priority (> 70): Highlight in **Red**.

        * Low Priority (< 30): Highlight in **Green**.

---

## 3. JSON Payload (Mock Data)

Use this data to test your application:

```json
[
  {
    "sender": "boss@company.com",
    "subject": "URGENT: Production DB Down",
    "body": "Please fix this immediately.",
    "receivedAt": "2023-10-27T10:00:00Z",
    "isVIP": true
  },
  {
    "sender": "marketing@store.com",
    "subject": "Weekly Newsletter",
    "body": "Click here to unsubscribe.",
    "receivedAt": "2023-10-27T09:00:00Z",
    "isVIP": false
  },
  {
    "sender": "client@example.com",
    "subject": "Question about invoice",
    "body": "Can we meet later?",
    "receivedAt": "2023-10-26T10:00:00Z", 
    "isVIP": false
  }
]
```

## 4. Setup Instructions

Please follow these steps to structure your submission:

**Folder Structure:**

Create a root folder named `submission`.

Inside, create two sub-directories: `/server` (for .NET) and `/client` (for JS).

**Backend Configuration:**

Initialize your Web API inside `/server`.

Crucial: Configure CORS in your Program.cs to allow requests from your frontend (usually http://localhost:3000 or http://127.0.0.1:5500).

**Documentation:**

Include a README.md at the root level.

Provide the specific commands needed to run both the server and the client (e.g., "Run dotnet run in server, then open index.html").

## 5. Evaluation Criteria

We will review your code based on the following rubric:

**Functional Correctness (Pass/Fail)**

[ ] Does the solution build and run?

[ ] Does the frontend successfully talk to the backend (CORS handled)?

[ ] Is the scoring math correct?

[ ] Is the output list sorted correctly (Highest score first)?

**Code Quality & Design**

* Separation of Concerns: Is the business logic isolated in a Service class (e.g., PriorityScorer) rather than the Controller?

* Defensive Coding: Did you handle the score clamping (0-100) correctly? Did you handle potential null inputs?

* Readability: Are variable names clear? Is the code easy to understand?

**Bonus**

* Unit Tests: Inclusion of a unit test for the calculation logic.

* UI Polish: A clean, user-friendly interface.

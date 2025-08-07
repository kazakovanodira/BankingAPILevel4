# Simple Banking Application (with External API Integration)

A simple **RESTful Banking API** built with **ASP.NET Core** and **MySQL**, designed for learning purposes.

---

##  Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-username/RESTAPIBankingApplication.git
```

### 2. Ensure MySQL is Running

Make sure you have MySQL server running on your machine.

### 3. Update the `appsettings.json` File

Update the connection string with your MySQL credentials:

```json
{
  "ConnectionString": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=accounts;Uid=root;Pwd=your_password;"
  }
}
```

### 4. Apply Migrations

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

### 6. Open Swagger UI

Visit:

```
http://localhost:5283/swagger/index.html
```

---

##  API Usage

All endpoints follow REST principles and return data in **JSON** (or **CSV** where applicable).

---

##  Authentication

All endpoints are secured using **JWT tokens**.  
You must authenticate first to receive a token.

---

##  How to Use the API

###  Register a New User

**POST** `/api/users/register`

```json
{
  "name": "string",
  "username": "string",
  "password": "string",
  "role": "string" // Either "User" or "Admin"
}
```

---

###  Login to Get JWT Token

**POST** `/api/authentication/authenticate`

```json
{
  "username": "string",
  "password": "string"
}
```

---

###  Get All Accounts (Admin Only)

**GET** `/api/v1/accounts/all`

**Optional Query Parameters**:

- `name` – filter by name  
- `pageNumber` – pagination (default: 1)  
- `pageSize` – pagination size (default: 10)  
- `orderBy` – default: `Name`  
- `descending` – default: `false`  
- Accept: `application/json` or `text/csv`

---

###  Get Account Info

**GET** `/api/v1/account/{accountNumber}`

---

###  Make a Deposit

**POST** `/api/v1/account/{accountNumber}/deposits`

```json
{
  "amount": "100"
}
```

---

###  Withdraw Funds

**POST** `/api/v1/account/{accountNumber}/withdrawals`

```json
{
  "amount": "100"
}
```

---

###  Transfer Between Accounts

**POST** `/api/v1/account/transfer`

```json
{
  "senderAccId": "GUID",
  "receiverAccId": "GUID",
  "amount": "100"
}
```

---

###  Get Balance in Different Currencies

**GET** `/api/Account/{accountNumber}/balances?currency=EUR,CAD,RUB`

Example:

```
http://localhost:5283/api/Account/{accountNumber}/balances?currency=EUR,CAD,RUB
```

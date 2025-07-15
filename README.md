# Simple Banking Application that consumes external API
A simple RESTful banking API built with **ASP.NET Core** and **MySQL** for learning purposes.

## Getting Started
1. Clone the repository:
   `git clone https://github.com/your-username/RESTAPIBankingApplication.git`

2. Make sure you have MySQL server running on your machine.

3. Update the appsettings.json file with your MySQL connection:
  ` {
     "ConnectionString": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=accounts;Uid=root;Pwd=your_password;"
     }
   }`

4. Apply the migrations to create the database:
   `dotnet ef database update`

5. Run the application:
   `dotnet run`

6. Navigate to the following URL to access the API:
   `http://localhost:5283/swagger/index.html`

## Using the application
- To create a new account:
  Send a POST request to /api/account with a JSON body like:
  `{
    "name": "Jane Doe"
  }`

- To get account info:
  Send a GET request to /api/account/{accountNumber}

- To make a deposit:
  Send a POST request to /api/account/{accountNumber}/deposits with:
  `{
    "amount": "100"
  }
`
- To withdraw:
  POST to /api/account/{accountNumber}/withdrawals with:
  `{
    "amount": "100"
  }`

- To transfer between accounts:
  POST to /api/account/transfer with:
  `{
    "senderAccId": "GUID",
    "receiverAccId": "GUID",
    "amount": "100"
  }`

- To get your balance in different currencies:
  Send a Get request as follows:
  ```GET http://localhost:5283/api/Account/{accountNumber}/balances?currency=EUR,CAD,RUB```



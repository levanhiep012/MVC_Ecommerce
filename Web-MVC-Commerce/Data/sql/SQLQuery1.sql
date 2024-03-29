CREATE DATABASE Ecommerce;
USE Ecommerce;
/*
	USER
*/
CREATE TABLE Users (
    UserId INT IDENTITY(1000,1) PRIMARY KEY ,
    UserName NVARCHAR(50) UNIQUE,
    PassWord VARCHAR(MAX),
    Email VARCHAR(100) UNIQUE,
    FullName NVARCHAR(100),
    Address NVARCHAR(255),
    PhoneNumber VARCHAR(20) UNIQUE
);

CREATE TABLE Addresses(
	AddressId INT IDENTITY PRIMARY KEY ,
	UserId INT,
	AddressLine1 NVARCHAR(MAX),
	City NVARCHAR(50),
	State NVARCHAR(50),
	Country NVARCHAR(50),
	PostalCode VARCHAR(20)
	FOREIGN KEY (UserId) REFERENCES Users(UserId)
)

CREATE TABLE PaymentMethods(
	PaymentMethodId INT IDENTITY PRIMARY KEY,
	UserId INT,
	CardNumber VARCHAR(20),
	ExpiryDate DATE,
	CVV char(5),
	CardHolderName VARCHAR(100),
	BillingAddressId INT,
	FOREIGN KEY (UserId) REFERENCES Users(UserId),
	FOREIGN KEY (BillingAddressId) REFERENCES Addresses(AddressId)
)
/* 
	Account
*/
	CREATE TABLE Accounts(
		AccountId INT IDENTITY PRIMARY KEY,
		UserId INT,
		AccountName VARCHAR(50),
		Balance MONEY,
		FOREIGN KEY (UserId) REFERENCES Users(UserId)

	)
	CREATE TABLE AccountTypes(
		AccountTypeId INT IDENTITY PRIMARY KEY,
		TypeName NVARCHAR(20),
	)
/* 
	Product
*/
CREATE TABLE Categories(
	CategoryId INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(100),
	Description TEXT
)

CREATE TABLE Products (
    ProductId INT PRIMARY KEY,
    Name NVARCHAR(100),
    Description NVARCHAR(MAX),
    Price MONEY,
    Quantity INT,
    Category INT,
	ImageUrl NVARCHAR(255),
	FOREIGN KEY (Category) REFERENCES Categories(CategoryId)
)


/* 
	Cart
*/

CREATE TABLE Carts(
	CartId INT IDENTITY PRIMARY KEY,
	UserId INT,
	ProductId INT,
	Quantity INT,
	FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
)

CREATE TABLE Orders(
	OrderId INT IDENTITY PRIMARY KEY,
	UserId INT,
	OrderDate DATE,
	TotalAmount INT,
	Status NVARCHAR(20),
	FOREIGN KEY (UserId) REFERENCES Users(UserId)
)
CREATE TABLE OrderItems(
	OrderItemId INT IDENTITY PRIMARY KEY,
	OrderId INT,
	ProductId INT,
	Quantity INT,
	Price MONEY,
	FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
	FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
)



-- Mẫu dữ liệu cho các sản phẩm iPhone từ iPhone 13 đến iPhone 15 với các biến thể mã sản phẩm đầy đủ
INSERT INTO Products (ProductId, Name, Description, Price, Quantity, Category, ImageUrl) 
VALUES 
('IP13B1MB16', 'iPhone 13 (Black, Mini, 16GB)', 'Description of iPhone 13 (Black, Mini, 16GB)', 999.99, 100, 'SmartPhone', 'image_url_of_iphone13_black_mini'),
('IP13B1MB32', 'iPhone 13 (Black, Mini, 32GB)', 'Description of iPhone 13 (Black, Mini, 32GB)', 1099.99, 150, 'SmartPhone', 'image_url_of_iphone13_black_mini'),
('IP13B1TB16', 'iPhone 13 (Black, Thường, 16GB)', 'Description of iPhone 13 (Black, Thường, 16GB)', 999.99, 100, 'SmartPhone', 'image_url_of_iphone13_black_thường'),
('IP13B1TB32', 'iPhone 13 (Black, Thường, 32GB)', 'Description of iPhone 13 (Black, Thường, 32GB)', 1099.99, 150, 'SmartPhone', 'image_url_of_iphone13_black_thường'),
('IP13B1PMB16', 'iPhone 13 (Black, Pro Max, 16GB)', 'Description of iPhone 13 (Black, Pro Max, 16GB)', 999.99, 100, 'SmartPhone', 'image_url_of_iphone13_black_pro_max'),
('IP13B1PMB32', 'iPhone 13 (Black, Pro Max, 32GB)', 'Description of iPhone 13 (Black, Pro Max, 32GB)', 1099.99, 150, 'SmartPhone', 'image_url_of_iphone13_black_pro_max'),

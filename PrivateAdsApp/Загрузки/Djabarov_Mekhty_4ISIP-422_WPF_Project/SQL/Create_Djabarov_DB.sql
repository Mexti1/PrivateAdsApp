-- SQL script to create database and tables for the assignment
-- Run in SQL Server Management Studio (SSMS) or via sqlcmd.
CREATE DATABASE Djabarov_DB_Payment;
GO
USE Djabarov_DB_Payment;
GO

CREATE TABLE [User] (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Login NVARCHAR(100) NOT NULL,
    Password NVARCHAR(200) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    FIO NVARCHAR(250) NOT NULL,
    Photo NVARCHAR(500) NULL
);

CREATE TABLE Category (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL
);

CREATE TABLE Payment (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID INT NOT NULL FOREIGN KEY REFERENCES Category(ID),
    UserID INT NOT NULL FOREIGN KEY REFERENCES [User](ID),
    Date DATE NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Num INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL
);

-- Sample data (you can register real users through the app)
INSERT INTO Category (Name) VALUES ('Транспорт'), ('Коммунальные услуги'), ('Развлечения');
INSERT INTO [User] (Login, Password, Role, FIO) VALUES ('admin', 'ADMIN_HASH_PLACEHOLDER', 'Admin', 'Админ Админов');
INSERT INTO [User] (Login, Password, Role, FIO) VALUES ('user', 'USER_HASH_PLACEHOLDER', 'User', 'Пользователь Пользов');
INSERT INTO Payment (CategoryID, UserID, Date, Name, Num, Price) VALUES (1, 2, GETDATE(), 'Билет', 2, 50.00);

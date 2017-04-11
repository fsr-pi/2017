USE master ;  
GO  
CREATE DATABASE InsuranceSales  
ON   
( NAME = Insurance_Sales_dat,  
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\insurancesalesdat.mdf',  
    SIZE = 10MB,  
    MAXSIZE = 50MB,  
    FILEGROWTH = 5MB )  
LOG ON  
( NAME = Insurance_Sales_log,  
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\insurancesaleslog.ldf',  
    SIZE = 5MB,  
    MAXSIZE = 25MB,  
    FILEGROWTH = 5MB ) ;  
GO 
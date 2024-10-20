# SQL QUERIES USED
```sql
CREATE TABLE Categories(Id INT PRIMARY KEY IDENTITY(1,1), CategoryName NVARCHAR(255) NOT NULL);
CREATE TABLE Users(Id INT PRIMARY KEY IDENTITY(1,1), LastName NVARCHAR(255) NOT NULL, FirstName NVARCHAR(255) NOT NULL, DateOfBirth DATE NOT NULL);
CREATE TABLE Banks(Id Int PRIMARY KEY IDENTITY(1,1), BankName NVARCHAR(255) NOT NULL, BankAddress NVARCHAR(255) NOT NULL);
CREATE TABLE Transactions(Id INT PRIMARY KEY IDENTITY(1,1), Amount DECIMAL(18,2) NOT NULL, UserId INT NOT NULL, TransactionDate DATE NOT NULL, CategoryId INT NOT NULL, BankId INT NOT NULL, FOREIGN KEY (CategoryId) REFERENCES Categories(Id), FOREIGN KEY (UserId) REFERENCES Users(Id), FOREIGN KEY (BankId) REFERENCES Banks(Id));
CREATE TABLE UserBanks(UserId INT, BankId INT, FOREIGN KEY (UserId) REFERENCES Users(Id), FOREIGN KEY (BankId) REFERENCES Banks(Id));

INSERT INTO Banks (BankName, BankAddress) VALUES 
('BNP Paribas', '16 Boulevard des Italiens, 75009 Paris, France'),
('Soci�t� G�n�rale', '29 Boulevard Haussmann, 75009 Paris, France'),
('Cr�dit Agricole', '12 Place des �tats-Unis, 92127 Montrouge, France'),
('La Banque Postale', '115 Rue de S�vres, 75275 Paris, France'),
('Cr�dit Mutuel', '88 Rue Cardinet, 75017 Paris, France'),
('Caisse d''�pargne', '50 Avenue Pierre Mend�s-France, 75013 Paris, France'),
('Banque Populaire', '76-78 Avenue de France, 75013 Paris, France'),
('HSBC France', '103 Avenue des Champs-�lys�es, 75008 Paris, France');
```
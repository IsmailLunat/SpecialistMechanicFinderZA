PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Provinces (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Code TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Cities (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    ProvinceId INTEGER NOT NULL,
    Aliases TEXT,
    LocalityType TEXT,
    FOREIGN KEY (ProvinceId) REFERENCES Provinces(Id) ON DELETE RESTRICT,
    UNIQUE (ProvinceId, Name)
);

INSERT OR IGNORE INTO Provinces (Name, Code) VALUES
    ('Eastern Cape', 'EC'),
    ('Free State', 'FS'),
    ('Gauteng', 'GP'),
    ('KwaZulu-Natal', 'KZN'),
    ('Limpopo', 'LP'),
    ('Mpumalanga', 'MP'),
    ('Northern Cape', 'NC'),
    ('North West', 'NW'),
    ('Western Cape', 'WC');

INSERT OR IGNORE INTO Cities (Name, ProvinceId, LocalityType) VALUES
    ('Gqeberha', (SELECT Id FROM Provinces WHERE Name = 'Eastern Cape'), 'City'),
    ('East London', (SELECT Id FROM Provinces WHERE Name = 'Eastern Cape'), 'City'),
    ('Port Alfred', (SELECT Id FROM Provinces WHERE Name = 'Eastern Cape'), 'Town'),
    ('Mthatha', (SELECT Id FROM Provinces WHERE Name = 'Eastern Cape'), 'City'),
    ('Bloemfontein', (SELECT Id FROM Provinces WHERE Name = 'Free State'), 'City'),
    ('Welkom', (SELECT Id FROM Provinces WHERE Name = 'Free State'), 'City'),
    ('Parys', (SELECT Id FROM Provinces WHERE Name = 'Free State'), 'Town'),
    ('Sasolburg', (SELECT Id FROM Provinces WHERE Name = 'Free State'), 'Town'),
    ('Johannesburg', (SELECT Id FROM Provinces WHERE Name = 'Gauteng'), 'City'),
    ('Pretoria', (SELECT Id FROM Provinces WHERE Name = 'Gauteng'), 'City'),
    ('Soweto', (SELECT Id FROM Provinces WHERE Name = 'Gauteng'), 'Township'),
    ('Sandton', (SELECT Id FROM Provinces WHERE Name = 'Gauteng'), 'Suburb'),
    ('Durban', (SELECT Id FROM Provinces WHERE Name = 'KwaZulu-Natal'), 'City'),
    ('Pietermaritzburg', (SELECT Id FROM Provinces WHERE Name = 'KwaZulu-Natal'), 'City'),
    ('Richards Bay', (SELECT Id FROM Provinces WHERE Name = 'KwaZulu-Natal'), 'Town'),
    ('Ballito', (SELECT Id FROM Provinces WHERE Name = 'KwaZulu-Natal'), 'Town'),
    ('Polokwane', (SELECT Id FROM Provinces WHERE Name = 'Limpopo'), 'City'),
    ('Tzaneen', (SELECT Id FROM Provinces WHERE Name = 'Limpopo'), 'Town'),
    ('Musina', (SELECT Id FROM Provinces WHERE Name = 'Limpopo'), 'Town'),
    ('Thohoyandou', (SELECT Id FROM Provinces WHERE Name = 'Limpopo'), 'Town'),
    ('Mbombela', (SELECT Id FROM Provinces WHERE Name = 'Mpumalanga'), 'City'),
    ('Nelspruit', (SELECT Id FROM Provinces WHERE Name = 'Mpumalanga'), 'City'),
    ('Witbank', (SELECT Id FROM Provinces WHERE Name = 'Mpumalanga'), 'Town'),
    ('Secunda', (SELECT Id FROM Provinces WHERE Name = 'Mpumalanga'), 'Town'),
    ('Kimberley', (SELECT Id FROM Provinces WHERE Name = 'Northern Cape'), 'City'),
    ('Upington', (SELECT Id FROM Provinces WHERE Name = 'Northern Cape'), 'Town'),
    ('Kuruman', (SELECT Id FROM Provinces WHERE Name = 'Northern Cape'), 'Town'),
    ('Mahikeng', (SELECT Id FROM Provinces WHERE Name = 'North West'), 'City'),
    ('Rustenburg', (SELECT Id FROM Provinces WHERE Name = 'North West'), 'City'),
    ('Potchefstroom', (SELECT Id FROM Provinces WHERE Name = 'North West'), 'City'),
    ('Cape Town', (SELECT Id FROM Provinces WHERE Name = 'Western Cape'), 'City'),
    ('Stellenbosch', (SELECT Id FROM Provinces WHERE Name = 'Western Cape'), 'Town'),
    ('George', (SELECT Id FROM Provinces WHERE Name = 'Western Cape'), 'Town'),
    ('Paarl', (SELECT Id FROM Provinces WHERE Name = 'Western Cape'), 'Town');

SELECT 'Provinces and cities imported successfully.' AS status;

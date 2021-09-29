SELECT 
    Id,
    Price,
    NetAream2,
    Typology,
    NumberOfBathrooms,
    Country,
    City,
    Latitude,
    Longitude
FROM TempProperty
WHERE Id = @id
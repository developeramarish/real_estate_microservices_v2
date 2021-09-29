SELECT
     Users.ID,
     Users.NAME,
     Users.SURNAME,
     Users.EMAIL,
     Users.CITY,
     Users.COUNTRY,
     Users.ADDRESS,
     Users.TAXNUMBER
FROM Users
WHERE Users.ID = @Id
AND DELETED = 0
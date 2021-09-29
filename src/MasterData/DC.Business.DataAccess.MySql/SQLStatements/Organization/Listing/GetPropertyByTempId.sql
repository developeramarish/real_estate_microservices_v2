SELECT tp.*, tc.*, ti.*
    FROM TempProperty tp
    LEFT JOIN TempCharacteristics tc ON tp.Id = tc.PropertyId
    LEFT JOIN TempImages ti ON tp.Id = ti.PropertyId
    WHERE tp.TempId = @id
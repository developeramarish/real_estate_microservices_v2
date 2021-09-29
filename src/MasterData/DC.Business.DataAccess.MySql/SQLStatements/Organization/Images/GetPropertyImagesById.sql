SELECT 
    Id,
    PropertyId,
    ImageName,
    ImageUrl,
    CreationDate,
    UpdateDate,
    Deleted
FROM Images
WHERE PropertyId = @id
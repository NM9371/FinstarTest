--2.1
SELECT 
    c.ClientName, 
    COUNT(cc.Id) AS ContactsCount
FROM Clients c
LEFT JOIN ClientContacts cc ON cc.ClientId = c.Id
GROUP BY c.ClientName
--2.2
SELECT 
    c.ClientName
FROM Clients c
LEFT JOIN ClientContacts cc ON cc.ClientId = c.Id
GROUP BY c.ClientName
HAVING COUNT(cc.Id)>2
--3.1
WITH Intervals AS (
    SELECT 
        Id,
        Dt AS StartDate,
        LEAD(Dt) OVER (PARTITION BY Id ORDER BY Dt) AS EndDate
    FROM Dates
)
SELECT 
    Id,
    StartDate AS Sd,
    EndDate AS Ed
FROM Intervals
WHERE EndDate IS NOT NULL;
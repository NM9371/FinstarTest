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
SELECT * FROM
(
    SELECT 
        Id,
        Dt AS Sd,
        LEAD(Dt) OVER (PARTITION BY Id ORDER BY Dt) AS Ed
    FROM Dates
) as Intervals 
WHERE Ed IS NOT NULL
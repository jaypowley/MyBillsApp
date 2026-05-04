RESTORE DATABASE MyBills_Core
FROM DISK = '/var/opt/mssql/data/MyBills_Core.bak'
WITH MOVE 'MyBills_Core' TO '/var/opt/mssql/data/MyBills_Core.mdf'
MOVE 'MyBills_Core_log' TO '/var/opt/mssql/data/MyBills_Core_log.ldf'
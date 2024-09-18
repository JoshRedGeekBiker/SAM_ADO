USE telematics;
DELETE FROM codigo WHERE enviado = 1;
DELETE FROM codigo WHERE enviado = 2;
DELETE FROM codigo WHERE Type_Codigo_id = 4;

SET @newid=0;
UPDATE codigo SET PK_ID=(@newid:=@newid+1) ORDER BY PK_ID;

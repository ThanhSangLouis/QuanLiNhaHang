CREATE DATABASE QuanLyQuanCafe
GO

USE QuanLyQuanCafe
GO
-- Food
-- Table
-- FoodCategory
-- Account
-- Bill
-- BillInfo

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Bàn chưa đặt tên', 
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống' --Trống || Có người
)
GO

CREATE TABLE Account
(
	
	DisplayName NVARCHAR(100) NOT NULL  DEFAULT N'SangShark',
	UserName NVARCHAR(100) PRIMARY KEY,
	Password NVARCHAR(1000) NOT NULL DEFAULT 0,
	Type INT NOT NULL DEFAULT 0 --1:Admin and 0: Staff
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',


)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',  	
	idCategory INT NOT NULL,
	price FLOAT NOT NULL,
	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0, --1: đã thanh toán && 0: chưa thanh toán
	FOREIGN KEY (idTable) REFERENCES TableFood(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0,
	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO
INSERT INTO dbo.Account
			( UserName,
			  DisplayName,
			  Password,
			  Type
			)
VALUES (N'K9', --Username - nvarchar(100)
		N'RongK9', --Displayname - nvarchar(100)
		N'1', --Password - nvarchar(1000)
		1 --Type - int
		)
INSERT INTO dbo.Account
			( UserName,
			  DisplayName,
			  Password,
			  Type
			)
VALUES (N'staff', --Username - nvarchar(100)
		N'staff', --Displayname - nvarchar(100)
		N'1', --Password - nvarchar(1000)
		0 --Type - int
		)
GO
CREATE PROC USP_GetAccountByUserName
@userName NVARCHAR(100)
AS
BEGIN
	SELECT * from dbo.Account where userName = @userName
END
GO
EXECUTE dbo.USP_GetAccountByUserName @username = N'K9' --nvarchar(100)
GO
CREATE PROC USP_Login
@userName NVARCHAR(100), @passWord nvarchar(100)
AS
BEGIN
	SELECT * from dbo.Account where UserName = @userName AND Password = @passWord
END
GO
--select * from dbo.Account where UserName = '' AND Password = N'' or 1=1--'
--THÊM BÀN
DECLARE @i int  = 0
WHILE @i <= 10
BEGIN
	INSERT dbo.TableFood (name) VALUEs (N'Bàn ' + CAST(@i as nvarchar(100)))
	SET @i = @i + 1	
END
GO
CREATE PROC USP_GetTableList
AS SELECT * from dbo.TableFood
GO

UPDATE dbo.TableFood SET [status] = N'CÓ NGƯỜI' WHERE id = 9
EXEC dbo.USP_GetTableList
GO

--THÊM CATEGORY
SELECT * from FoodCategory
INSERT dbo.FoodCategory
		(name)
VALUES (N'Hải sản')
INSERT dbo.FoodCategory
		(name)
VALUES (N'Nông sản')
INSERT dbo.FoodCategory
		(name)
VALUES (N'Lâm sản sản')
INSERT dbo.FoodCategory
		(name)
VALUES (N'Khoáng sản')
INSERT dbo.FoodCategory
		(name)
VALUES (N'Nước')
--THÊM MÓN ĂN
INSERT dbo.Food
		(name, idCategory, price)
VALUES (N'Mực một nắng nướng sa tế', 1, 250000)
INSERT dbo.Food
		(name, idCategory, price)
VALUES (N'Cá lóc nướng chui', 1, 150000)
INSERT dbo.Food
		(name, idCategory, price)
VALUES (N'Dú heo nướng muối ớt', 2, 60000)
INSERT dbo.Food
		(name, idCategory, price)
VALUES (N'Heo rừng nướng muối ớt', 3, 450000)
INSERT dbo.Food
		(name, idCategory, price)
VALUES (N'Lúa mạch lên men', 4, 460000)
INSERT dbo.Food
		(name, idCategory, price)
VALUES (N'7up', 5, 15000)
INSERT dbo.Food
		(name, idCategory, price)
VALUES (N'Pepsi', 5, 12000)

GO

--THÊM BILL
select * from dbo.Bill
INSERT dbo.Bill
		(DateCheckIn, DateCheckOut, idTable, [status])
VALUES (GETDATE(), null, 1,0)
INSERT dbo.Bill
		(DateCheckIn, DateCheckOut, idTable, [status])
VALUES (GETDATE(), null, 2,0)
INSERT dbo.Bill
		(DateCheckIn, DateCheckOut, idTable, [status])
VALUES (GETDATE(), GETDATE(), 2,1)
--THÊM BILLINFO
INSERT dbo.BillInfo
		(idBill, idFood, [count])
VALUES (1,1,2)
INSERT dbo.BillInfo
		(idBill, idFood, [count])
VALUES (1,3,4)
INSERT dbo.BillInfo
		(idBill, idFood, [count])
VALUES (1,5,1)
INSERT dbo.BillInfo
		(idBill, idFood, [count])
VALUES (2,1,2)
INSERT dbo.BillInfo
		(idBill, idFood, [count])
VALUES (2,5,2)
INSERT dbo.BillInfo
		(idBill, idFood, [count])
VALUES (3,5,2)

SELECT * from dbo.Bill	
SELECT * from dbo.BillInfo
SELECT * from dbo.Food
SELECT * from dbo.FoodCategory
GO

SELECT * from TableFood
select id from dbo.Bill where idTable = 2 and status = 0
select * from dbo.Bill where idTable = 2 and status = 0

SELECT * from dbo.Bill
SELECT * from dbo.BillInfo
SELECT * from dbo.TableFood

GO
create PROC USP_InsertBill
@idTable INT
AS
BEGIN
	INSERT dbo.Bill
			(DateCheckIn,
			 DateCheckOut,
			 idTable,
			 [status],
			 discount)
	VALUES (GETDATE(),
			NULL,
			@idTable,
			0,
			0
			)
END
GO
create PROC USP_InsertBillInfo
@idBill INT, @idFood INT, @count INT
AS
BEGIN
    DECLARE @isExitsBillInfo INT
    DECLARE @foodCount INT
    DECLARE @newCount INT

    -- Kiểm tra sự tồn tại của bản ghi trong BillInfo và lấy số lượng hiện tại
    SELECT @isExitsBillInfo = COUNT(*), @foodCount = MAX([count])
    FROM dbo.BillInfo 
    WHERE idBill = @idBill AND idFood = @idFood

    -- Nếu bản ghi đã tồn tại
    IF (@isExitsBillInfo > 0)
    BEGIN
        -- Tính số lượng mới
        SET @newCount = @foodCount + @count

        -- Nếu số lượng mới > 0, cập nhật; nếu không xóa bản ghi
        IF (@newCount > 0)
        BEGIN
            UPDATE dbo.BillInfo
            SET [count] = @newCount
            WHERE idBill = @idBill AND idFood = @idFood
        END
        ELSE
        BEGIN
            DELETE dbo.BillInfo 
            WHERE idBill = @idBill AND idFood = @idFood
        END
    END
    -- Nếu bản ghi chưa tồn tại, thêm mới
    ELSE
    BEGIN
        INSERT dbo.BillInfo (idBill, idFood, [count])
        VALUES (@idBill, @idFood, @count)
    END
END
GO

delete dbo.BillInfo
delete dbo.Bill
go

select * from dbo.BillInfo
go
create TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	select @idBill = idBill from inserted
	DECLARE @idTable INT
	SELECT @idTable = idTable from dbo.Bill where id = @idBill and [status] = 0
	declare @count int
	select @count = count(*) from dbo.BillInfo where idBill = @idBill
	if(@count > 0)
		update dbo.TableFood SET [status] = N'Có người' where id = @idTable
	else
		update dbo.TableFood SET [status] = N'Trống' where id = @idTable

END
GO


CREATE TRIGGER UTG_UpdateBill
ON dbo.Bill FOR UPDATE
as
BEGIN
	DECLARE @idBill INT
	select @idBill = id from inserted
	DECLARE @idTable INT
	SELECT @idTable = idTable from dbo.Bill where id = @idBill
	DECLARE @count int = 0
	SELECT @count = count(*) from dbo.Bill where idTable = @idTable and status = 0
	if(@count = 0)
		UPDATE dbo.TableFood SET [status] = N'Trống' where id = @idTable
END
go	

ALTER TABLE dbo.Bill
ADD discount INT
Update dbo.Bill set discount = 0

go
alter PROC USP_SwitchTable
@idTable1 INT, @idTable2 INT
AS
BEGIN
    DECLARE @idFirstBill INT, @idSecondBill INT;
	declare @isFirstTableEmpty int = 1
	declare @isSecondTableEmpty int = 1

    -- Lấy hóa đơn của 2 bàn cần đổi
    SELECT @idSecondBill = id FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0;
    SELECT @idFirstBill = id FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0;

    -- Nếu bàn 1 chưa có hóa đơn, tạo mới
    IF (@idFirstBill IS NULL)
    BEGIN
        INSERT INTO dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
        VALUES (GETDATE(), NULL, @idTable1, 0);

        SELECT @idFirstBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0;
    END
	select @isFirstTableEmpty = count(*) from dbo.BillInfo where idBill = @idFirstBill
    -- Nếu bàn 2 chưa có hóa đơn, tạo mới
    IF (@idSecondBill IS NULL)
    BEGIN
        INSERT INTO dbo.Bill (DateCheckIn, DateCheckOut, idTable, status)
        VALUES (GETDATE(), NULL, @idTable2, 0);

        SELECT @idSecondBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0;
    END
	select @isSecondTableEmpty = count(*) from dbo.BillInfo where idBill = @idSecondBill

    -- Lưu ID của các BillInfo liên quan đến @idSecondBill vào bảng tạm
    DECLARE @IDBillInfoTable TABLE (id INT);
    INSERT INTO @IDBillInfoTable SELECT id FROM dbo.BillInfo WHERE idBill = @idSecondBill;

    -- Cập nhật BillInfo: Đổi @idFirstBill thành @idSecondBill
    UPDATE dbo.BillInfo SET idBill = @idSecondBill WHERE idBill = @idFirstBill;

    -- Cập nhật BillInfo: Đổi @idSecondBill thành @idFirstBill
    UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id IN (SELECT id FROM @IDBillInfoTable);

	if(@isFirstTableEmpty = 0)
		update dbo.TableFood set status = N'Trống'where id = @idTable2
	if(@isSecondTableEmpty = 0)
		update dbo.TableFood set status = N'Trống'where id = @idTable1

END;
GO
create PROC USP_GetListBillByDate
@checkIn date, @checkOut date
as
begin
	select t.name as [Table Name],b.totalPrice as [Total Price], DateCheckIn , DateCheckOut, discount as [Discount]
	from dbo.Bill as b, dbo.TableFood as t
	where DateCheckIn >= @checkIn and DateCheckOut <= @checkOut and b.status = 1
	and t.id = b.idTable
end
GO

CREATE PROC USP_UpdateAccount
@userName NVARCHAR(100), @displayName NVARCHAR(100), @password NVARCHAR(100), @newPassword NVARCHAR(100)
AS
BEGIN
	declare @isRightPass INT = 0
	select @isRightPass = COUNT(*)  from dbo.Account where UserName = @userName and Password = @password
	if(@isRightPass = 1)
	begin
		if(@newPassword = null or @newPassword = '')
		begin
			update dbo.Account set DisplayName = @displayName where UserName = @userName
		end
		else
			update dbo.Account set DisplayName = @displayName, Password = @newPassword where UserName = @userName 
	end
END
GO

INSERT dbo.Food ( name, idCategory, price ) VALUES (N'', 0 , 0.0)
go
 
CREATE TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS	
BEGIN
	DECLARE @idBillInfo INT
	DECLARE @idBill INT
	SELECT @idBillInfo = id, @idBill = deleted.idBill FROM deleted

	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill where id = @idBill

	DECLARE @count INT = 0
	SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi, dbo.Bill AS b where b.id = bi.idBill and b.id = @idBill and status = 0
	 
	if(@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' where id = @idTable
END
GO

CREATE FUNCTION [dbo].[fnConvertToUnsign1] (@str NVARCHAR(MAX))  
RETURNS NVARCHAR(MAX)  
AS  
BEGIN  
    DECLARE @Result NVARCHAR(MAX) = @str  

    DECLARE @AccentChars NVARCHAR(4000) = N'áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ'  
    DECLARE @NoAccentChars NVARCHAR(4000) = N'aaaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyd'  
    DECLARE @i INT = 1  

    WHILE @i <= LEN(@AccentChars)  
    BEGIN  
        SET @Result = REPLACE(@Result, SUBSTRING(@AccentChars, @i, 1), SUBSTRING(@NoAccentChars, @i, 1))  
        SET @i = @i + 1  
    END  

    RETURN @Result  
END  
GO

SELECT userName, displayName, type FROM Account
GO

create PROC USP_GetListBillByDateAndPage
@checkIn date, @checkOut date, @page int
as
begin
	Declare @pageRows int = 10
	declare @selectRows	int  = @pageRows
	Declare @exceptRows int = (@page - 1) * @pageRows

	;with BillShow as (select b.id, t.name as [Table Name],b.totalPrice as [Total Price], DateCheckIn , DateCheckOut, discount as [Discount]
	from dbo.Bill as b, dbo.TableFood as t
	where DateCheckIn >= @checkIn and DateCheckOut <= @checkOut and b.status = 1
	and t.id = b.idTable)

	select TOP (@selectRows) * from BillShow where id NOT IN(select TOP(@exceptRows) id from BillShow)
end
GO

create PROC USP_GetNumBillByDate
@checkIn date, @checkOut date
as
begin
	select COUNT(*)
	from dbo.Bill as b, dbo.TableFood as t
	where DateCheckIn >= @checkIn and DateCheckOut <= @checkOut and b.status = 1
	and t.id = b.idTable
end
GO

ALTER TABLE TableFood ADD reservationStatus INT DEFAULT 0;
GO
CREATE TABLE Reservation (
    id INT IDENTITY PRIMARY KEY,
    idTable INT NOT NULL, --Bàn được đặt trước
    customerName NVARCHAR(100) NOT NULL, --Tên khách đặt bàn
    phone NVARCHAR(15) NOT NULL, --Số điện thoại khách
    reservationTime DATETIME NOT NULL, --Thời gian khách đặt bàn
    status INT DEFAULT 0, -- 0: Chờ xác nhận, 1: Đã xác nhận, 2: Đã hủy
    FOREIGN KEY (idTable) REFERENCES TableFood(id)
);
GO

CREATE PROCEDURE USP_BookTable
    @idTable INT,
    @customerName NVARCHAR(100),
    @phone NVARCHAR(15),
    @reservationTime DATETIME
AS
BEGIN
    -- Kiểm tra xem bàn đã được đặt trước chưa
    IF EXISTS (SELECT * FROM Reservation WHERE idTable = @idTable AND reservationTime = @reservationTime AND status = 0)
    BEGIN
        PRINT 'Bàn này đã có người đặt trước vào thời gian này!';
        RETURN;
    END

    -- Thêm thông tin đặt bàn
    INSERT INTO Reservation (idTable, customerName, phone, reservationTime, status)
    VALUES (@idTable, @customerName, @phone, @reservationTime, 0);

    -- Cập nhật trạng thái bàn
    UPDATE TableFood SET reservationStatus = 1 WHERE id = @idTable;
END
GO
EXEC USP_BookTable 5, 'Nguyễn Văn A', '0987654321', '2025-03-10 18:00:00';
GO

CREATE PROCEDURE USP_CancelReservation
    @idTable INT
AS
BEGIN
    -- Cập nhật trạng thái bàn về "Trống"
    UPDATE TableFood 
    SET status = N'Trống', reservationStatus = 0
    WHERE id = @idTable;

    -- Xóa thông tin đặt bàn trong bảng Reservation
    DELETE FROM Reservation WHERE idTable = @idTable;
END
GO

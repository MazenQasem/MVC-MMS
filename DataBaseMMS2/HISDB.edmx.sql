
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/09/2023 19:35:36
-- Generated from EDMX file: D:\MMS-Rebuild\MMS\Main\DataBaseMMS2\Models\HISDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [HIS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[HISModelStoreContainer].[AlterMRP]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[AlterMRP];
GO
IF OBJECT_ID(N'[dbo].[Bank]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Bank];
GO
IF OBJECT_ID(N'[dbo].[Batch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Batch];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[BatchLocator]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[BatchLocator];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[BatchStore]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[BatchStore];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[Category]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[Category];
GO
IF OBJECT_ID(N'[dbo].[Company]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Company];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[Department]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[Department];
GO
IF OBJECT_ID(N'[dbo].[Employee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employee];
GO
IF OBJECT_ID(N'[dbo].[Grade]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Grade];
GO
IF OBJECT_ID(N'[dbo].[Item]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Item];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[ItemAvgSale]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[ItemAvgSale];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[ItemGroup]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[ItemGroup];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[ItemStore]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[ItemStore];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[Manufacturer]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[Manufacturer];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[OPBService]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[OPBService];
GO
IF OBJECT_ID(N'[dbo].[OPLOAOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OPLOAOrder];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[OPLOAOrderModify]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[OPLOAOrderModify];
GO
IF OBJECT_ID(N'[dbo].[OPLOAService]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OPLOAService];
GO
IF OBJECT_ID(N'[dbo].[Packing]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Packing];
GO
IF OBJECT_ID(N'[dbo].[Patient]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Patient];
GO
IF OBJECT_ID(N'[dbo].[PurchaseReceipt]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PurchaseReceipt];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[PurchaseReceiptDetail]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[PurchaseReceiptDetail];
GO
IF OBJECT_ID(N'[dbo].[Rack]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Rack];
GO
IF OBJECT_ID(N'[dbo].[Shelf]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Shelf];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[supplier]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[supplier];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[TransactionType]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[TransactionType];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[Doctor]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[Doctor];
GO
IF OBJECT_ID(N'[HISModelStoreContainer].[MMS_ItemMaster]', 'U') IS NOT NULL
    DROP TABLE [HISModelStoreContainer].[MMS_ItemMaster];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Banks'
CREATE TABLE [dbo].[Banks] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NAME] varchar(50)  NOT NULL,
    [CITY] varchar(50)  NULL,
    [PIN] varchar(6)  NULL,
    [DELETED] smallint  NULL,
    [State] varchar(50)  NULL,
    [branch] varchar(50)  NULL,
    [operatorid] int  NULL,
    [adress] varchar(510)  NULL,
    [BankCode] varchar(5)  NULL,
    [HospitalAccNo] varchar(24)  NULL,
    [DivisionId] int  NULL,
    [site] varchar(4)  NULL,
    [credit_card_type] varchar(20)  NULL
);
GO

-- Creating table 'Batches'
CREATE TABLE [dbo].[Batches] (
    [ItemID] int  NOT NULL,
    [BatchID] int IDENTITY(1,1) NOT NULL,
    [BatchNo] varchar(25)  NOT NULL,
    [ExpiryDate] datetime  NULL,
    [Supplierid] int  NULL,
    [CostPrice] decimal(20,8)  NULL,
    [SellingPrice] decimal(18,6)  NULL,
    [Tax] decimal(12,2)  NULL,
    [MRP] decimal(18,6)  NULL,
    [Quantity] int  NOT NULL,
    [ItemType] bit  NULL,
    [PurRate] decimal(24,4)  NULL,
    [UnitEpr] decimal(14,5)  NULL,
    [StartDate] datetime  NOT NULL,
    [MIDDLEWARE] int  NULL
);
GO

-- Creating table 'BatchStores'
CREATE TABLE [dbo].[BatchStores] (
    [StationID] smallint  NOT NULL,
    [ItemID] int  NOT NULL,
    [BatchID] int  NULL,
    [BatchNo] varchar(25)  NOT NULL,
    [Quantity] int  NOT NULL,
    [Tax] smallint  NULL,
    [ItemType] bit  NULL,
    [MIDDLEWARE] int  NULL
);
GO

-- Creating table 'ItemGroups'
CREATE TABLE [dbo].[ItemGroups] (
    [ID] int  NOT NULL,
    [Name] varchar(45)  NULL,
    [Parent] int  NULL,
    [MaxID] int  NULL,
    [Fixed] int  NULL,
    [Medical] int  NULL,
    [MaintDept] bit  NULL,
    [Equipment] tinyint  NULL,
    [OverSea] tinyint  NULL,
    [Deleted] bit  NULL
);
GO

-- Creating table 'ItemStores'
CREATE TABLE [dbo].[ItemStores] (
    [StationID] int  NOT NULL,
    [ItemID] int  NOT NULL,
    [UnitID] int  NULL,
    [MaxLevel] int  NULL,
    [MinLevel] int  NULL,
    [QOH] int  NULL,
    [ROL] int  NULL,
    [ROQ] int  NULL,
    [ABC] tinyint  NULL,
    [FSN] tinyint  NULL,
    [ved] tinyint  NULL,
    [Tax] int  NULL,
    [ConversionQty] int  NULL,
    [Deleted] bit  NULL,
    [StartDateTime] datetime  NULL,
    [EndDateTime] datetime  NULL,
    [TimeStamp] binary(8)  NULL
);
GO

-- Creating table 'Manufacturers'
CREATE TABLE [dbo].[Manufacturers] (
    [id] int  NOT NULL,
    [Code] varchar(15)  NULL,
    [Name] varchar(50)  NULL,
    [Grade] varchar(20)  NULL,
    [ContactPerson] varchar(50)  NULL,
    [Address1] varchar(30)  NULL,
    [Address2] varchar(30)  NULL,
    [City] varchar(30)  NULL,
    [Country] varchar(30)  NULL,
    [POBox] int  NULL,
    [Phone1] varchar(30)  NULL,
    [Phone2] varchar(30)  NULL,
    [Page] varchar(30)  NULL,
    [Cell] varchar(30)  NULL,
    [Fax] varchar(30)  NULL,
    [Email] varchar(50)  NULL,
    [web_site] varchar(50)  NULL,
    [PaymentType] tinyint  NULL,
    [PaymentDays] int  NULL,
    [NetDueDays] smallint  NULL,
    [DiscountDays] smallint  NULL,
    [Percentage] tinyint  NULL,
    [CreditLimit] real  NULL,
    [VendorStatus] bit  NULL,
    [OverSeas] bit  NULL,
    [Active] bit  NULL,
    [Deleted] bit  NULL,
    [StartdateTime] datetime  NULL,
    [enddatetime] datetime  NULL
);
GO

-- Creating table 'Packings'
CREATE TABLE [dbo].[Packings] (
    [ID] int  NOT NULL,
    [Name] varchar(255)  NULL,
    [Deleted] bit  NULL,
    [UOMCode] varchar(5)  NULL
);
GO

-- Creating table 'PurchaseReceipts'
CREATE TABLE [dbo].[PurchaseReceipts] (
    [ID] int  NOT NULL,
    [POrderId] int  NULL,
    [ReceiptNo] char(15)  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [ReceiptDate] datetime  NOT NULL,
    [OperatorId] int  NOT NULL,
    [Supplierid] int  NULL,
    [OrderAmount] decimal(14,2)  NULL,
    [stationid] int  NOT NULL,
    [stationslno] int  NOT NULL,
    [posted] bit  NULL,
    [Discount] real  NULL,
    [other_deductions] real  NULL,
    [Freight] real  NULL,
    [Excise] real  NULL,
    [CST] real  NULL,
    [LST] real  NULL,
    [Octroi] real  NULL,
    [RecBayId] int  NULL,
    [invNo] char(15)  NULL,
    [type] tinyint  NULL,
    [gatepassno] int  NULL,
    [PRType] tinyint  NULL,
    [disamount] decimal(12,2)  NULL,
    [reference] varchar(25)  NULL,
    [categoryid] int  NULL,
    [InvConfirmed] bit  NULL,
    [InvConfirmedDatetime] datetime  NULL,
    [InvConfirmedOperatorId] int  NULL,
    [InvDate] datetime  NULL,
    [LocatorStatus] tinyint  NULL
);
GO

-- Creating table 'PurchaseReceiptDetails'
CREATE TABLE [dbo].[PurchaseReceiptDetails] (
    [ReceiptID] int  NOT NULL,
    [ItemId] int  NOT NULL,
    [Quantity] int  NOT NULL,
    [Packid] int  NOT NULL,
    [FreeQuantity] int  NOT NULL,
    [FreePackId] int  NOT NULL,
    [MRP] decimal(18,2)  NOT NULL,
    [BatchNo] varchar(20)  NULL,
    [ExpiryDate] datetime  NULL,
    [PurchaseRate] decimal(20,4)  NULL,
    [epr] decimal(17,5)  NULL,
    [discount] real  NULL,
    [slNo] int  NULL,
    [Full_Part] bit  NOT NULL,
    [remarks] varchar(90)  NULL,
    [Deleted] bit  NULL,
    [Excise] real  NULL,
    [CST] real  NULL,
    [LST] real  NULL,
    [Tax1] real  NULL,
    [DisAmt] real  NULL,
    [TaxAmt] real  NULL,
    [BatchId] int  NULL,
    [pono] int  NULL,
    [unitepr] decimal(9,2)  NULL,
    [adddiscount] real  NULL,
    [PartNo] varchar(20)  NULL,
    [Manufacturerid] int  NULL
);
GO

-- Creating table 'suppliers'
CREATE TABLE [dbo].[suppliers] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Code] varchar(15)  NULL,
    [Name] varchar(50)  NULL,
    [ContactPerson] varchar(50)  NULL,
    [Grade] varchar(20)  NULL,
    [Address1] varchar(30)  NULL,
    [Address2] varchar(30)  NULL,
    [City] varchar(30)  NULL,
    [Country] varchar(30)  NULL,
    [POBox] varchar(30)  NULL,
    [Phone1] varchar(30)  NULL,
    [Phone2] varchar(30)  NULL,
    [Page] varchar(30)  NULL,
    [Cell] varchar(30)  NULL,
    [Fax] varchar(30)  NULL,
    [Telex] varchar(20)  NULL,
    [Email] varchar(50)  NULL,
    [web_site] varchar(50)  NULL,
    [PaymentType] tinyint  NULL,
    [PaymentDays] int  NULL,
    [NetDueDays] smallint  NULL,
    [DiscountDays] smallint  NULL,
    [Percentage] tinyint  NULL,
    [CreditLimit] real  NULL,
    [VendorStatus] bit  NULL,
    [OverSeas] bit  NULL,
    [Active] bit  NULL,
    [BankID] int  NULL,
    [AccountNo] varchar(20)  NULL,
    [Deleted] bit  NULL,
    [StartdateTime] datetime  NULL,
    [EndDatetime] datetime  NULL
);
GO

-- Creating table 'AlterMRPs'
CREATE TABLE [dbo].[AlterMRPs] (
    [itemid] int  NOT NULL,
    [batchid] int  NOT NULL,
    [oldcp] decimal(18,4)  NULL,
    [oldmrp] decimal(18,4)  NULL,
    [newcp] decimal(18,4)  NULL,
    [newmrp] decimal(18,4)  NULL,
    [datetime] datetime  NULL,
    [operatorid] int  NOT NULL,
    [OldExpDate] datetime  NULL,
    [NewExpDate] datetime  NULL
);
GO

-- Creating table 'Employees'
CREATE TABLE [dbo].[Employees] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [EmployeeID] varchar(50)  NULL,
    [EmpCode] varchar(10)  NOT NULL,
    [Title] int  NULL,
    [FirstName] varchar(50)  NOT NULL,
    [MiddleName] varchar(50)  NULL,
    [LastName] varchar(50)  NULL,
    [Sex] int  NULL,
    [DOB] datetime  NULL,
    [Age] smallint  NULL,
    [HAdd1] varchar(250)  NULL,
    [HCity] varchar(50)  NULL,
    [HState] varchar(50)  NULL,
    [HCountry] varchar(50)  NULL,
    [HPINCode] varchar(50)  NULL,
    [HPhoneNo] varchar(50)  NULL,
    [WAdd1] varchar(250)  NULL,
    [WCity] varchar(50)  NULL,
    [WState] varchar(50)  NULL,
    [WCountry] varchar(50)  NULL,
    [WPINCode] varchar(50)  NULL,
    [WPhoneNo] varchar(50)  NULL,
    [FaxNo] varchar(50)  NULL,
    [PagerNo] varchar(50)  NULL,
    [CellNo] varchar(50)  NULL,
    [EMail] varchar(50)  NULL,
    [ECity] varchar(50)  NULL,
    [EAdd1] varchar(250)  NULL,
    [EState] varchar(50)  NULL,
    [ECountry] varchar(50)  NULL,
    [EPINcode] varchar(50)  NULL,
    [EPhoneNo] varchar(50)  NULL,
    [Qualification] varchar(75)  NULL,
    [PlaceOfContact] varchar(50)  NULL,
    [EContactPerson] varchar(50)  NULL,
    [ContactTime] varchar(50)  NULL,
    [Timings] varchar(50)  NULL,
    [Remarks] varchar(250)  NULL,
    [EmployeeType] tinyint  NULL,
    [VisitingProf] tinyint  NULL,
    [IsPractisingDoctor] tinyint  NULL,
    [DivisionID] int  NULL,
    [DepartmentID] int  NULL,
    [DesignationID] int  NULL,
    [CategoryID] int  NULL,
    [SubCategoryID] int  NULL,
    [Medical] int  NULL,
    [Supervisor] tinyint  NULL,
    [Name] varchar(122)  NULL,
    [ArabicName] nvarchar(100)  NULL,
    [NationID] int  NULL,
    [IACode] varchar(6)  NULL,
    [RegNo] int  NULL,
    [OPMarkUpPercent] int  NULL,
    [Password] varchar(100)  NULL,
    [GLCode] varchar(20)  NULL,
    [OperatorID] int  NULL,
    [LastUpdated] datetime  NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL,
    [Deleted] int  NOT NULL,
    [Indent] bit  NULL,
    [SystemName] varchar(50)  NULL,
    [LoggedYN] bit  NULL,
    [LoggedIPAddress] varchar(50)  NULL,
    [Locked_YN] char(1)  NULL,
    [PW_SET_DATE] datetime  NULL,
    [PWD_SET_DATE] datetime  NULL,
    [PWD_EXPIRED_YN] char(1)  NULL,
    [USER_START_TIME] datetime  NULL,
    [USER_END_TIME] datetime  NULL,
    [Insert_Update] char(1)  NULL,
    [IsUploaded] bit  NULL,
    [TempRegNo] varchar(30)  NULL,
    [Arabiccode] nvarchar(50)  NULL,
    [OldEmpCode] varchar(15)  NULL,
    [ADUserName] varchar(50)  NULL,
    [InsuranceNumber] varchar(20)  NULL,
    [WorkHours] int  NULL,
    [BranchCode] varchar(5)  NULL,
    [IsHalfDayDuty] bit  NULL
);
GO

-- Creating table 'Doctors'
CREATE TABLE [dbo].[Doctors] (
    [Arabiccode] nvarchar(50)  NULL,
    [ID] int  NOT NULL,
    [EmployeeID] varchar(50)  NULL,
    [EmpCode] varchar(10)  NOT NULL,
    [FirstName] varchar(50)  NOT NULL,
    [MiddleName] varchar(50)  NULL,
    [LastName] varchar(50)  NULL,
    [Sex] int  NULL,
    [DesignationID] int  NULL,
    [DOB] datetime  NULL,
    [Age] smallint  NULL,
    [CellNo] varchar(50)  NULL,
    [EMail] varchar(50)  NULL,
    [Qualification] varchar(75)  NULL,
    [PlaceOfContact] varchar(50)  NULL,
    [ContactTime] varchar(50)  NULL,
    [Deleted] int  NOT NULL,
    [Title] int  NULL,
    [CategoryID] int  NULL,
    [Password] varchar(100)  NULL,
    [EmployeeType] tinyint  NULL,
    [Name] varchar(122)  NULL,
    [RegNo] int  NULL,
    [IACode] varchar(6)  NULL,
    [ArabicName] nvarchar(100)  NULL,
    [OPMarkUpPercent] int  NULL,
    [SubCategoryID] int  NULL,
    [DepartmentID] int  NULL,
    [SystemName] varchar(50)  NULL,
    [LoggedYN] bit  NULL,
    [LoggedIPAddress] varchar(50)  NULL,
    [Locked_YN] char(1)  NULL,
    [PW_SET_DATE] datetime  NULL,
    [PWD_SET_DATE] datetime  NULL,
    [PWD_EXPIRED_YN] char(1)  NULL,
    [USER_START_TIME] datetime  NULL,
    [USER_END_TIME] datetime  NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL,
    [VISITINGPROF] tinyint  NOT NULL,
    [IsUploaded] bit  NULL
);
GO

-- Creating table 'ItemAvgSales'
CREATE TABLE [dbo].[ItemAvgSales] (
    [Itemid] int  NOT NULL,
    [Month1] int  NULL,
    [Month2] int  NULL,
    [Month3] int  NULL,
    [Stationid] smallint  NULL,
    [Average] real  NULL
);
GO

-- Creating table 'BatchLocators'
CREATE TABLE [dbo].[BatchLocators] (
    [ItemID] int  NULL,
    [BatchID] int  NOT NULL,
    [BatchNo] varchar(50)  NULL,
    [StationID] int  NOT NULL,
    [Quantity] int  NOT NULL,
    [RackID] int  NOT NULL,
    [ShelfID] int  NOT NULL,
    [OperatorID] int  NOT NULL,
    [StartDateTime] datetime  NOT NULL
);
GO

-- Creating table 'Racks'
CREATE TABLE [dbo].[Racks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(20)  NOT NULL,
    [Deleted] bit  NOT NULL,
    [StationId] int  NULL
);
GO

-- Creating table 'Shelves'
CREATE TABLE [dbo].[Shelves] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(20)  NOT NULL,
    [Deleted] bit  NOT NULL,
    [Stationid] smallint  NULL
);
GO

-- Creating table 'TransactionTypes'
CREATE TABLE [dbo].[TransactionTypes] (
    [id] int  NOT NULL,
    [description] varchar(50)  NULL,
    [Type] char(1)  NULL
);
GO

-- Creating table 'MMS_ItemMaster'
CREATE TABLE [dbo].[MMS_ItemMaster] (
    [Id] int  NOT NULL,
    [Name] varchar(100)  NULL,
    [Name1] varchar(100)  NULL,
    [Strength] varchar(35)  NULL,
    [CategoryID] int  NULL,
    [ManufacturerId] int  NOT NULL,
    [MaxLevel] int  NULL,
    [MinLevel] int  NULL,
    [ROL] int  NULL,
    [ROQ] int  NULL,
    [QOH] int  NULL,
    [ABC] tinyint  NULL,
    [FSN] tinyint  NULL,
    [VED] tinyint  NULL,
    [SellingPrice] decimal(19,4)  NULL,
    [StartDateTime] datetime  NOT NULL,
    [Enddatetime] datetime  NULL,
    [ItemCode] varchar(50)  NULL,
    [Deleted] bit  NULL,
    [EUB] bit  NULL,
    [ProfitCenter] varchar(50)  NULL,
    [UnitID] int  NULL,
    [Tax] int  NULL,
    [Schedule] bit  NULL,
    [MRPItem] bit  NULL,
    [ConversionQty] int  NULL,
    [DrugType] int  NOT NULL,
    [DeletedBY] int  NULL,
    [CssdItem] bit  NULL,
    [BatchStatus] bit  NULL,
    [Strength_no] decimal(12,2)  NULL,
    [Narcotic] bit  NULL,
    [NonStocked] bit  NULL,
    [ItemPrefix] varchar(6)  NULL,
    [Consignment] bit  NULL,
    [Approval] bit  NULL,
    [ProfitCentreID] smallint  NULL,
    [FixedAsset] bit  NULL,
    [IssueType] tinyint  NULL,
    [DrugState] tinyint  NULL,
    [Strength_Unit] varchar(10)  NULL,
    [DuplicateLabel] bit  NULL,
    [PartNumber] varchar(30)  NULL,
    [CSSDApp] int  NULL,
    [DRUGCONTROL] bit  NULL,
    [ModelNo] varchar(20)  NULL,
    [CriticalItem] bit  NULL,
    [Feasibility] bit  NULL,
    [iscocktail] tinyint  NULL,
    [catalogueno] varchar(50)  NULL,
    [stationid] int  NOT NULL
);
GO

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [ID] int  NOT NULL,
    [Code] varchar(20)  NOT NULL,
    [Name] varchar(100)  NOT NULL,
    [CategoryType] smallint  NOT NULL,
    [PayAfter] varchar(50)  NOT NULL,
    [InsuranceCard] bit  NOT NULL,
    [IquamaID] bit  NOT NULL,
    [RefLetter] bit  NOT NULL,
    [Address] varchar(300)  NULL,
    [TelephoneNo] varchar(25)  NULL,
    [FaxNo] varchar(25)  NULL,
    [EmailID] varchar(50)  NULL,
    [PoBox] varchar(250)  NULL,
    [ZipCode] varchar(25)  NULL,
    [City] varchar(25)  NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL,
    [Active] bit  NOT NULL,
    [OperatorID] int  NOT NULL,
    [DateTime] datetime  NULL,
    [Deleted] bit  NOT NULL,
    [arabicname] nvarchar(100)  NULL,
    [arabiccode] nvarchar(100)  NULL,
    [ValidFrom] datetime  NULL,
    [ValidTill] datetime  NULL,
    [ArabicInvoice] bit  NULL,
    [Remarks] varchar(5000)  NULL,
    [AccountType] int  NULL,
    [ReferralBasis] int  NULL,
    [OPConsultations] int  NULL,
    [WFCLS] int  NULL,
    [WFRS] int  NULL,
    [WFRC] int  NULL,
    [PrintAddress] int  NULL,
    [OPPD] int  NULL,
    [IPPD] int  NULL,
    [Coverage] tinyint  NULL,
    [CONTACTPERSONNAME] varchar(50)  NOT NULL,
    [CONTACTPERSONDESIG] varchar(50)  NOT NULL,
    [TelNo2] varchar(25)  NOT NULL,
    [FaxNo2] varchar(25)  NOT NULL,
    [ProviderCode] varchar(50)  NOT NULL,
    [FixedConCharges] decimal(13,2)  NOT NULL,
    [RegCharges] decimal(13,2)  NOT NULL,
    [InvoiceConFee] decimal(13,2)  NOT NULL,
    [BlockReason] varchar(1000)  NOT NULL,
    [PharmacyCSTHeader] bit  NOT NULL,
    [RegfeePaidby] smallint  NULL,
    [PolicyRules] varchar(500)  NOT NULL,
    [ApprovalDays] int  NULL,
    [ByPassExclusionsStatus] bit  NOT NULL,
    [TariffID] int  NULL,
    [BillingCollectorId] int  NULL,
    [BillingOfficerId] int  NULL,
    [LOAConsultation] bit  NULL,
    [UCF] bit  NULL,
    [CatGroup] varchar(25)  NULL,
    [Insert_Update] varchar(1)  NULL,
    [ModifiedDateTime] datetime  NULL,
    [Ora_Code] varchar(20)  NULL,
    [RevisitDays] tinyint  NULL,
    [speccons] int  NULL,
    [Attribute4] varchar(20)  NULL
);
GO

-- Creating table 'Companies'
CREATE TABLE [dbo].[Companies] (
    [ID] int  NOT NULL,
    [Code] varchar(15)  NOT NULL,
    [Name] varchar(70)  NOT NULL,
    [CategoryID] int  NOT NULL,
    [TariffID] int  NULL,
    [PolicyNo] varchar(30)  NULL,
    [Coverage] smallint  NOT NULL,
    [OPPD] int  NOT NULL,
    [IPPD] int  NOT NULL,
    [Address] varchar(300)  NULL,
    [TelephoneNo] varchar(25)  NOT NULL,
    [FaxNo] varchar(25)  NOT NULL,
    [EMailID] varchar(50)  NOT NULL,
    [PoBox] varchar(25)  NOT NULL,
    [ZipCode] varchar(25)  NOT NULL,
    [City] varchar(25)  NOT NULL,
    [OperatorID] int  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL,
    [Active] bit  NOT NULL,
    [DateTime] datetime  NULL,
    [Deleted] bit  NOT NULL,
    [ArabicName] nvarchar(100)  NULL,
    [ArabicCode] nvarchar(100)  NULL,
    [TPAId] int  NULL,
    [UCF] bit  NULL,
    [BillingCollectorId] int  NULL,
    [BillingOfficerId] int  NULL,
    [ValidFrom] datetime  NULL,
    [ValidTill] datetime  NULL,
    [ArabicInvoice] bit  NULL,
    [Remarks] varchar(max)  NULL,
    [AccountType] int  NULL,
    [ReferralBasis] int  NULL,
    [OPConsultations] int  NULL,
    [WFCLS] int  NULL,
    [WFRS] int  NULL,
    [WFRC] int  NULL,
    [PrintAddress] int  NULL,
    [FollowRules] tinyint  NOT NULL,
    [LOAConsultation] bit  NULL,
    [EmpInfo] smallint  NULL,
    [CONTACTPERSONNAME] varchar(50)  NULL,
    [CONTACTPERSONDESIG] varchar(50)  NULL,
    [TelNo2] varchar(25)  NOT NULL,
    [FaxNo2] varchar(25)  NOT NULL,
    [ProviderCode] varchar(50)  NOT NULL,
    [RelationDetails] bit  NULL,
    [FixedConCharges] decimal(13,2)  NOT NULL,
    [RegCharges] decimal(13,2)  NOT NULL,
    [InvoiceConFee] decimal(13,2)  NOT NULL,
    [BlockReason] varchar(2000)  NOT NULL,
    [PharmacyCSTHeader] bit  NOT NULL,
    [Aramco] bit  NULL,
    [NoofVisits] int  NULL,
    [RegFeePaidBy] smallint  NULL,
    [PolicyRules] varchar(500)  NOT NULL,
    [CONSULTATIONLIMIT] bit  NOT NULL,
    [BlockReasonId] int  NULL,
    [SubCategoryId] int  NULL,
    [ApprovalDays] int  NULL,
    [DeductableStatus] smallint  NULL,
    [DiscountToPrint] tinyint  NULL,
    [ByPassExclusionsStatus] bit  NULL,
    [PerAdvancetoPay] decimal(18,0)  NULL,
    [DisForAdvancePay] decimal(18,0)  NULL,
    [HostName] varchar(50)  NULL,
    [CheckMedId] bit  NULL,
    [TariffLevel] int  NULL,
    [Insert_Update] varchar(1)  NULL,
    [ModifiedDateTime] datetime  NULL,
    [RevisitDays] tinyint  NULL,
    [speccons] int  NULL,
    [UPLOADED] bit  NULL,
    [Attribute4] varchar(20)  NULL,
    [Staff_Attribute4] varchar(20)  NULL
);
GO

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DeptCode] varchar(10)  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [AccountCode] varchar(20)  NULL,
    [DeptClassID] varchar(5)  NULL,
    [RecordID] varchar(5)  NULL,
    [StartDateTime] datetime  NOT NULL,
    [Deleted] bit  NOT NULL,
    [EndDateTime] datetime  NULL,
    [OperatorID] int  NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDateTime] datetime  NULL,
    [DivisionId] int  NULL,
    [ArabicName] nvarchar(100)  NULL,
    [ArabicCode] nvarchar(100)  NULL,
    [OLDID] int  NULL,
    [NonSGHDept] tinyint  NULL,
    [Ora_Code] varchar(5)  NULL,
    [UPLOADED] bit  NULL,
    [UDATETIME] datetime  NULL
);
GO

-- Creating table 'Grades'
CREATE TABLE [dbo].[Grades] (
    [ID] int  NOT NULL,
    [Name] char(10)  NULL,
    [GradeName] varchar(50)  NULL,
    [ArabicName] nvarchar(100)  NULL,
    [PolicyNo] varchar(30)  NULL,
    [CategoryID] int  NULL,
    [CompanyID] int  NOT NULL,
    [RoomCharges] decimal(9,4)  NULL,
    [FixedConCharges] decimal(9,2)  NULL,
    [InvoiceConFee] decimal(9,2)  NULL,
    [IPCreditLimit] decimal(14,2)  NULL,
    [BedTypeID] int  NULL,
    [Blocked] bit  NULL,
    [Deleted] bit  NOT NULL,
    [OperatorID] int  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL,
    [TariffID] int  NULL,
    [OPConsultations] int  NULL
);
GO

-- Creating table 'OPBServices'
CREATE TABLE [dbo].[OPBServices] (
    [Id] int  NOT NULL,
    [Name] char(60)  NOT NULL,
    [MasterTable] char(60)  NOT NULL,
    [OrderTable] char(60)  NOT NULL,
    [DetaiTtable] char(60)  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL,
    [Deleted] bit  NOT NULL,
    [ServiceCode] varchar(10)  NULL,
    [PriceTable] varchar(30)  NULL,
    [DisplayServiceId] int  NULL,
    [SupportTable] varchar(20)  NULL,
    [MarkUp] int  NULL,
    [Ora_Code] varchar(5)  NULL
);
GO

-- Creating table 'OPLOAOrders'
CREATE TABLE [dbo].[OPLOAOrders] (
    [IssueAuthoritycode] varchar(6)  NULL,
    [RegistrationNo] int  NULL,
    [Name] varchar(200)  NULL,
    [LOAamount] decimal(19,4)  NULL,
    [LOAbalance] decimal(19,4)  NULL,
    [NoDays] int  NULL,
    [Letterno] varchar(50)  NULL,
    [Approval] bit  NOT NULL,
    [AuthorityId] int  NOT NULL,
    [AuthorityBillNo] varchar(15)  NULL,
    [LoaDateTime] datetime  NULL,
    [CategoryId] int  NULL,
    [CompanyId] int  NULL,
    [GradeId] int  NULL,
    [DoctorId] int  NULL,
    [MedIDNumber] varchar(30)  NULL,
    [LOAExpiryDate] datetime  NULL,
    [Checked] bit  NULL,
    [TPAId] int  NULL,
    [ServiceId] int  NULL,
    [IsLOA] smallint  NULL,
    [PharmacyAmount] decimal(19,4)  NULL,
    [LOAType] smallint  NULL,
    [ICDId] int  NULL,
    [ICDCode] varchar(15)  NULL,
    [ICDDescription] varchar(2000)  NULL,
    [SGHAuthorityID] varchar(25)  NULL,
    [Deleted] bit  NULL,
    [EndDatetime] datetime  NULL,
    [OperatorId] int  NULL,
    [PolicyNo] varchar(30)  NULL
);
GO

-- Creating table 'OPLOAOrderModifies'
CREATE TABLE [dbo].[OPLOAOrderModifies] (
    [AutorityId] int  NULL,
    [Registrationno] int  NULL,
    [Amount] decimal(10,2)  NULL,
    [NoDays] int  NULL,
    [Datetime] datetime  NULL,
    [OperatorId] int  NULL,
    [ApprovalNo] varchar(20)  NULL,
    [Notes] varchar(50)  NULL,
    [PharmacyAmount] decimal(18,2)  NULL,
    [ID] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'OPLOAServices'
CREATE TABLE [dbo].[OPLOAServices] (
    [CategoryId] int  NOT NULL,
    [CompanyId] int  NOT NULL,
    [GradeId] int  NOT NULL,
    [ServiceId] int  NOT NULL,
    [LoaAmount] decimal(10,2)  NULL,
    [LoaDays] int  NULL,
    [StartDateTime] datetime  NULL
);
GO

-- Creating table 'Patients'
CREATE TABLE [dbo].[Patients] (
    [RegDateTime] datetime  NOT NULL,
    [IssueAuthorityCode] varchar(6)  NOT NULL,
    [Registrationno] int  NOT NULL,
    [Title] varchar(30)  NOT NULL,
    [FamilyName] varchar(50)  NULL,
    [Firstname] varchar(50)  NOT NULL,
    [MiddleName] varchar(50)  NULL,
    [LastName] varchar(50)  NULL,
    [MothersMaidenName] varchar(20)  NULL,
    [FathersName] varchar(50)  NULL,
    [DateOfBirth] datetime  NOT NULL,
    [Age] smallint  NOT NULL,
    [Agetype] smallint  NOT NULL,
    [Sex] smallint  NOT NULL,
    [MaritalStatus] smallint  NOT NULL,
    [Occupation] varchar(50)  NULL,
    [Guardian] varchar(50)  NULL,
    [GRelationship] varchar(50)  NULL,
    [PCity] int  NOT NULL,
    [PPhone] varchar(25)  NULL,
    [PEMail] varchar(50)  NULL,
    [WrkAddress] varchar(250)  NULL,
    [WrkPhone] varchar(25)  NULL,
    [WrkEMail] varchar(50)  NULL,
    [OtherAllergies] varchar(100)  NULL,
    [Caution] bit  NOT NULL,
    [LastModifiedDateTime] datetime  NULL,
    [OperatorID] int  NOT NULL,
    [Country] int  NULL,
    [PassportNo] varchar(25)  NULL,
    [IssueDate] datetime  NULL,
    [ExpiryDate] datetime  NULL,
    [CCurrency] varchar(20)  NULL,
    [ReferredDocName] varchar(50)  NULL,
    [ReferredDocAddress] varchar(250)  NULL,
    [ReferredDocPhone] varchar(25)  NULL,
    [ReferredDocEmail] varchar(50)  NULL,
    [ReferredDocCellNo] varchar(25)  NULL,
    [Religion] smallint  NULL,
    [ModifiedOperator] int  NULL,
    [Deleted] bit  NOT NULL,
    [Vip] bit  NULL,
    [CityName] varchar(50)  NULL,
    [CountryName] varchar(50)  NULL,
    [Password] varchar(14)  NULL,
    [ReferredDocSpecialisation] varchar(50)  NULL,
    [PCellno] varchar(25)  NULL,
    [Gphone] varchar(25)  NULL,
    [Gcellno] varchar(25)  NULL,
    [Gaddress] varchar(250)  NULL,
    [Gemail] varchar(50)  NULL,
    [BloodGroup] varchar(10)  NULL,
    [WrkFax] varchar(25)  NULL,
    [Ppagerno] varchar(25)  NULL,
    [Cpagerno] varchar(25)  NULL,
    [Rpagerno] varchar(25)  NULL,
    [ChequeBounce] bit  NULL,
    [Address1] varchar(50)  NULL,
    [Address2] varchar(50)  NULL,
    [Address3] varchar(50)  NULL,
    [NonSaudi] bit  NOT NULL,
    [pZipCode] varchar(15)  NULL,
    [Nationality] int  NULL,
    [Billtype] tinyint  NULL,
    [WrkCompanyName] varchar(50)  NULL,
    [CompanyId] int  NULL,
    [SidIssueDate] datetime  NULL,
    [SidIssuedAt] varchar(30)  NULL,
    [SaudiIqamaId] varchar(20)  NULL,
    [SidExpiryDate] datetime  NULL,
    [PassportIssuedAt] varchar(30)  NULL,
    [Sexothers] varchar(20)  NULL,
    [Messages] bit  NULL,
    [BilledBy] bit  NULL,
    [DoctorId] int  NULL,
    [EmployeeId] varchar(20)  NULL,
    [Embose] tinyint  NULL,
    [AFirstName] nvarchar(50)  NULL,
    [AMiddleName] nvarchar(50)  NULL,
    [ALastName] nvarchar(50)  NULL,
    [AFamilyName] nvarchar(50)  NULL,
    [AAddress1] nvarchar(50)  NULL,
    [AAddress2] nvarchar(50)  NULL,
    [CategoryId] int  NULL,
    [GradeId] int  NULL,
    [PolicyNo] varchar(30)  NULL,
    [IDExpiryDate] datetime  NULL,
    [MedIDNumber] varchar(30)  NULL,
    [Billed] bit  NULL,
    [ValidFrom] datetime  NULL,
    [ValidTo] datetime  NULL,
    [MRBlocked] smallint  NULL,
    [IsInvoiced] tinyint  NULL,
    [InvoiceDateTime] datetime  NULL,
    [CompanyLetterId] int  NULL,
    [SGHRegNO] varchar(25)  NULL,
    [SGHDateTime] datetime  NULL,
    [EmboseCharged] bit  NULL,
    [AramcoRegDateTime] datetime  NULL,
    [SGHName] varchar(100)  NULL,
    [UPLOADTAG] bit  NULL,
    [BirthTime] datetime  NULL,
    [Insert_Update] varchar(1)  NULL
);
GO

-- Creating table 'Items'
CREATE TABLE [dbo].[Items] (
    [ID] int  NOT NULL,
    [ItemCode] varchar(50)  NULL,
    [Name] varchar(100)  NULL,
    [Name1] varchar(100)  NULL,
    [ArabicCode] nvarchar(50)  NULL,
    [ArabicName] nvarchar(100)  NULL,
    [SellingPrice] decimal(19,4)  NULL,
    [Strength] varchar(35)  NULL,
    [CategoryID] int  NULL,
    [ManufacturerID] int  NOT NULL,
    [EUB] bit  NULL,
    [ProfitCenter] varchar(50)  NULL,
    [UnitID] int  NULL,
    [Tax] real  NULL,
    [Schedule] bit  NULL,
    [MRPItem] bit  NULL,
    [ConversionQty] int  NOT NULL,
    [DrugType] int  NOT NULL,
    [TempID] int  NULL,
    [CSSDItem] bit  NULL,
    [BatchStatus] bit  NULL,
    [Strength_No] decimal(12,2)  NULL,
    [CapitalRevenue] tinyint  NULL,
    [Narcotic] bit  NULL,
    [NonStocked] bit  NULL,
    [ItemPrefix] varchar(6)  NULL,
    [DrugControl] bit  NULL,
    [Consignment] bit  NULL,
    [Approval] bit  NULL,
    [ProfitCentreID] smallint  NULL,
    [FixedAsset] bit  NULL,
    [IssueType] tinyint  NULL,
    [DrugState] tinyint  NULL,
    [Strength_Unit] varchar(10)  NULL,
    [CriticalItem] bit  NULL,
    [DuplicateLabel] bit  NULL,
    [PartNumber] varchar(30)  NULL,
    [CSSDApp] int  NULL,
    [ModelNo] varchar(20)  NULL,
    [OperatorId] int  NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDatetime] datetime  NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NULL,
    [Deleted] bit  NULL,
    [DeletedBy] int  NULL,
    [Feasibility] bit  NULL,
    [Uploaded] int  NULL,
    [OrcItemCode] varchar(50)  NULL,
    [CatalogueNo] varchar(50)  NULL,
    [ItemForm] int  NULL,
    [Description] varchar(200)  NULL,
    [IsCocktail] tinyint  NULL,
    [Ora_code] varchar(50)  NULL,
    [IsUnified] bit  NULL,
    [Date_Unified] datetime  NULL,
    [Prev_Ora_Code] varchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Banks'
ALTER TABLE [dbo].[Banks]
ADD CONSTRAINT [PK_Banks]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [BatchID], [BatchNo] in table 'Batches'
ALTER TABLE [dbo].[Batches]
ADD CONSTRAINT [PK_Batches]
    PRIMARY KEY CLUSTERED ([BatchID], [BatchNo] ASC);
GO

-- Creating primary key on [StationID], [ItemID], [BatchNo], [Quantity] in table 'BatchStores'
ALTER TABLE [dbo].[BatchStores]
ADD CONSTRAINT [PK_BatchStores]
    PRIMARY KEY CLUSTERED ([StationID], [ItemID], [BatchNo], [Quantity] ASC);
GO

-- Creating primary key on [ID] in table 'ItemGroups'
ALTER TABLE [dbo].[ItemGroups]
ADD CONSTRAINT [PK_ItemGroups]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [StationID], [ItemID] in table 'ItemStores'
ALTER TABLE [dbo].[ItemStores]
ADD CONSTRAINT [PK_ItemStores]
    PRIMARY KEY CLUSTERED ([StationID], [ItemID] ASC);
GO

-- Creating primary key on [id] in table 'Manufacturers'
ALTER TABLE [dbo].[Manufacturers]
ADD CONSTRAINT [PK_Manufacturers]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [ID] in table 'Packings'
ALTER TABLE [dbo].[Packings]
ADD CONSTRAINT [PK_Packings]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PurchaseReceipts'
ALTER TABLE [dbo].[PurchaseReceipts]
ADD CONSTRAINT [PK_PurchaseReceipts]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ReceiptID], [ItemId], [Quantity], [Packid], [FreeQuantity], [FreePackId], [MRP], [Full_Part] in table 'PurchaseReceiptDetails'
ALTER TABLE [dbo].[PurchaseReceiptDetails]
ADD CONSTRAINT [PK_PurchaseReceiptDetails]
    PRIMARY KEY CLUSTERED ([ReceiptID], [ItemId], [Quantity], [Packid], [FreeQuantity], [FreePackId], [MRP], [Full_Part] ASC);
GO

-- Creating primary key on [ID] in table 'suppliers'
ALTER TABLE [dbo].[suppliers]
ADD CONSTRAINT [PK_suppliers]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [itemid], [batchid], [operatorid] in table 'AlterMRPs'
ALTER TABLE [dbo].[AlterMRPs]
ADD CONSTRAINT [PK_AlterMRPs]
    PRIMARY KEY CLUSTERED ([itemid], [batchid], [operatorid] ASC);
GO

-- Creating primary key on [ID] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [PK_Employees]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID], [EmpCode], [FirstName], [Deleted], [StartDateTime], [VISITINGPROF] in table 'Doctors'
ALTER TABLE [dbo].[Doctors]
ADD CONSTRAINT [PK_Doctors]
    PRIMARY KEY CLUSTERED ([ID], [EmpCode], [FirstName], [Deleted], [StartDateTime], [VISITINGPROF] ASC);
GO

-- Creating primary key on [Itemid] in table 'ItemAvgSales'
ALTER TABLE [dbo].[ItemAvgSales]
ADD CONSTRAINT [PK_ItemAvgSales]
    PRIMARY KEY CLUSTERED ([Itemid] ASC);
GO

-- Creating primary key on [BatchID], [StationID], [Quantity], [RackID], [ShelfID], [OperatorID], [StartDateTime] in table 'BatchLocators'
ALTER TABLE [dbo].[BatchLocators]
ADD CONSTRAINT [PK_BatchLocators]
    PRIMARY KEY CLUSTERED ([BatchID], [StationID], [Quantity], [RackID], [ShelfID], [OperatorID], [StartDateTime] ASC);
GO

-- Creating primary key on [Id] in table 'Racks'
ALTER TABLE [dbo].[Racks]
ADD CONSTRAINT [PK_Racks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Shelves'
ALTER TABLE [dbo].[Shelves]
ADD CONSTRAINT [PK_Shelves]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [id] in table 'TransactionTypes'
ALTER TABLE [dbo].[TransactionTypes]
ADD CONSTRAINT [PK_TransactionTypes]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id], [ManufacturerId], [StartDateTime], [DrugType], [stationid] in table 'MMS_ItemMaster'
ALTER TABLE [dbo].[MMS_ItemMaster]
ADD CONSTRAINT [PK_MMS_ItemMaster]
    PRIMARY KEY CLUSTERED ([Id], [ManufacturerId], [StartDateTime], [DrugType], [stationid] ASC);
GO

-- Creating primary key on [ID], [Code], [Name], [CategoryType], [PayAfter], [InsuranceCard], [IquamaID], [RefLetter], [StartDateTime], [Active], [OperatorID], [Deleted], [CONTACTPERSONNAME], [CONTACTPERSONDESIG], [TelNo2], [FaxNo2], [ProviderCode], [FixedConCharges], [RegCharges], [InvoiceConFee], [BlockReason], [PharmacyCSTHeader], [PolicyRules], [ByPassExclusionsStatus] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([ID], [Code], [Name], [CategoryType], [PayAfter], [InsuranceCard], [IquamaID], [RefLetter], [StartDateTime], [Active], [OperatorID], [Deleted], [CONTACTPERSONNAME], [CONTACTPERSONDESIG], [TelNo2], [FaxNo2], [ProviderCode], [FixedConCharges], [RegCharges], [InvoiceConFee], [BlockReason], [PharmacyCSTHeader], [PolicyRules], [ByPassExclusionsStatus] ASC);
GO

-- Creating primary key on [ID] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [PK_Companies]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID], [DeptCode], [Name], [StartDateTime], [Deleted] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([ID], [DeptCode], [Name], [StartDateTime], [Deleted] ASC);
GO

-- Creating primary key on [ID] in table 'Grades'
ALTER TABLE [dbo].[Grades]
ADD CONSTRAINT [PK_Grades]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id], [Name], [MasterTable], [OrderTable], [DetaiTtable], [StartDateTime], [Deleted] in table 'OPBServices'
ALTER TABLE [dbo].[OPBServices]
ADD CONSTRAINT [PK_OPBServices]
    PRIMARY KEY CLUSTERED ([Id], [Name], [MasterTable], [OrderTable], [DetaiTtable], [StartDateTime], [Deleted] ASC);
GO

-- Creating primary key on [AuthorityId] in table 'OPLOAOrders'
ALTER TABLE [dbo].[OPLOAOrders]
ADD CONSTRAINT [PK_OPLOAOrders]
    PRIMARY KEY CLUSTERED ([AuthorityId] ASC);
GO

-- Creating primary key on [ID] in table 'OPLOAOrderModifies'
ALTER TABLE [dbo].[OPLOAOrderModifies]
ADD CONSTRAINT [PK_OPLOAOrderModifies]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [CategoryId], [CompanyId], [GradeId], [ServiceId] in table 'OPLOAServices'
ALTER TABLE [dbo].[OPLOAServices]
ADD CONSTRAINT [PK_OPLOAServices]
    PRIMARY KEY CLUSTERED ([CategoryId], [CompanyId], [GradeId], [ServiceId] ASC);
GO

-- Creating primary key on [IssueAuthorityCode], [Registrationno] in table 'Patients'
ALTER TABLE [dbo].[Patients]
ADD CONSTRAINT [PK_Patients]
    PRIMARY KEY CLUSTERED ([IssueAuthorityCode], [Registrationno] ASC);
GO

-- Creating primary key on [ID] in table 'Items'
ALTER TABLE [dbo].[Items]
ADD CONSTRAINT [PK_Items]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
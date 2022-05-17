/*
SQLyog Ultimate
MySQL - 5.7.21-log : Database - agrishare
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
/*Table structure for table `Adverts` */

CREATE TABLE `Adverts` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(256) DEFAULT NULL,
  `PhotoPath` varchar(1024) DEFAULT NULL,
  `LinkUrl` varchar(1024) DEFAULT NULL,
  `ImpressionCount` int(11) NOT NULL DEFAULT '0',
  `ClickCount` int(11) NOT NULL DEFAULT '0',
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `RegionId` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `Title` (`Title`),
  KEY `StartDate` (`StartDate`),
  KEY `Deleted` (`Deleted`),
  KEY `RegionId` (`RegionId`),
  CONSTRAINT `adverts_ibfk_1` FOREIGN KEY (`RegionId`) REFERENCES `Regions` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Agents` */

CREATE TABLE `Agents` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(256) DEFAULT NULL,
  `Commission` decimal(10,4) NOT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Telephone` varchar(32) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Blogs` */

CREATE TABLE `Blogs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(1024) DEFAULT NULL,
  `Photo` varchar(1024) DEFAULT NULL,
  `Content` text,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Table structure for table `BookingComments` */

CREATE TABLE `BookingComments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BookingId` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `Text` varchar(4096) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `BookingId` (`BookingId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `bookingcomments_ibfk_1` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `bookingcomments_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `BookingProducts` */

CREATE TABLE `BookingProducts` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BookingId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `BookingId` (`BookingId`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `bookingproducts_ibfk_1` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`),
  CONSTRAINT `bookingproducts_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `BookingTags` */

CREATE TABLE `BookingTags` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BookingId` int(11) NOT NULL,
  `TagId` int(11) NOT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `BookingId` (`BookingId`),
  KEY `TagId` (`TagId`),
  CONSTRAINT `bookingtags_ibfk_1` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `bookingtags_ibfk_2` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `BookingUsers` */

CREATE TABLE `BookingUsers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BookingId` int(11) NOT NULL,
  `UserId` int(11) DEFAULT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `Telephone` varchar(16) DEFAULT NULL,
  `Ratio` decimal(10,3) NOT NULL,
  `VerificationCode` varchar(8) DEFAULT NULL,
  `VerificationCodeExpiry` datetime NOT NULL,
  `StatusId` smallint(6) NOT NULL COMMENT 'Enum: Pending, Verified, Paid',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `bookingusers_ibfk_1` (`BookingId`),
  CONSTRAINT `bookingusers_ibfk_1` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `bookingusers_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=latin1;

/*Table structure for table `Bookings` */

CREATE TABLE `Bookings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ForId` smallint(6) NOT NULL COMMENT 'Enum: Me, Friend, Group',
  `UserId` int(11) NOT NULL,
  `ListingId` int(11) DEFAULT NULL,
  `ServiceId` int(11) DEFAULT NULL,
  `Location` varchar(256) DEFAULT NULL,
  `Latitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Longitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Destination` varchar(256) DEFAULT NULL,
  `DestinationLatitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `DestinationLongitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Quantity` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Distance` decimal(10,3) NOT NULL,
  `IncludeFuel` tinyint(1) NOT NULL DEFAULT '0',
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `Price` decimal(10,3) NOT NULL DEFAULT '0.000',
  `HireCost` decimal(10,3) NOT NULL DEFAULT '0.000',
  `FuelCost` decimal(10,3) NOT NULL DEFAULT '0.000',
  `TransportCost` decimal(10,3) NOT NULL DEFAULT '0.000',
  `TransportDistance` decimal(10,3) NOT NULL DEFAULT '0.000',
  `AdditionalInformation` varchar(4096) DEFAULT NULL,
  `Commission` decimal(10,3) NOT NULL DEFAULT '0.000',
  `AgentCommission` decimal(10,3) NOT NULL DEFAULT '0.000',
  `TotalVolume` decimal(10,3) NOT NULL DEFAULT '0.000',
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Approved, Declined, In Progress, Complete',
  `PaidOut` tinyint(1) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `SMSCount` int(11) NOT NULL DEFAULT '0',
  `SMSCost` decimal(10,3) NOT NULL DEFAULT '0.000',
  `TransactionFee` decimal(10,3) NOT NULL DEFAULT '0.000',
  `IMTT` decimal(10,3) NOT NULL DEFAULT '0.000',
  `VoucherId` int(11) DEFAULT NULL,
  `VoucherTotal` decimal(10,3) NOT NULL DEFAULT '0.000',
  `PaymentStatus` smallint(6) NOT NULL DEFAULT '0',
  `Tags` varchar(1024) DEFAULT NULL,
  `ReceiptPhoto` varchar(1024) DEFAULT NULL,
  `PaymentMethodId` smallint(6) NOT NULL DEFAULT '0',
  `ProductListJson` varchar(4096) DEFAULT NULL,
  `SupplierId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `VoucherId` (`VoucherId`),
  KEY `bookings_ibfk_2` (`ListingId`),
  KEY `bookings_ibfk_3` (`ServiceId`),
  KEY `SupplierId` (`SupplierId`),
  KEY `Deleted` (`Deleted`),
  CONSTRAINT `bookings_ibfk_2` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `bookings_ibfk_3` FOREIGN KEY (`ServiceId`) REFERENCES `Services` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `bookings_ibfk_4` FOREIGN KEY (`VoucherId`) REFERENCES `Vouchers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `bookings_ibfk_5` FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5024 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Categories` */

CREATE TABLE `Categories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ParentId` int(11) DEFAULT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `Translations` text,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=71 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Config` */

CREATE TABLE `Config` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Key` varchar(256) DEFAULT NULL,
  `Value` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=54 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Counters` */

CREATE TABLE `Counters` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) DEFAULT NULL,
  `ServiceId` int(11) DEFAULT NULL,
  `CategoryId` int(11) DEFAULT NULL,
  `BookingId` int(11) DEFAULT NULL,
  `Event` varchar(32) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ServiceId` (`ServiceId`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `counters_ibfk_1` FOREIGN KEY (`ServiceId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `counters_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `counters_ibfk_3` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `counters_ibfk_4` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=271 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Devices` */

CREATE TABLE `Devices` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `Token` varchar(1024) DEFAULT NULL,
  `EndpointARN` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `SnsDeviceUser` (`UserId`),
  CONSTRAINT `devices_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Faqs` */

CREATE TABLE `Faqs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `LanguageId` int(11) NOT NULL DEFAULT '1',
  `Question` varchar(4096) DEFAULT NULL,
  `Answer` text,
  `SortOrder` int(11) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Journals` */

CREATE TABLE `Journals` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RegionId` int(11) NOT NULL DEFAULT '1',
  `Title` varchar(256) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  `BookingId` int(11) DEFAULT NULL,
  `Amount` decimal(10,3) NOT NULL DEFAULT '0.000',
  `CurrencyAmount` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Currency` smallint(6) NOT NULL DEFAULT '1',
  `Reconciled` tinyint(1) NOT NULL DEFAULT '0',
  `EcoCashReference` varchar(128) DEFAULT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Date` datetime NOT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  KEY `RegionId` (`RegionId`),
  CONSTRAINT `journals_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `journals_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `journals_ibfk_3` FOREIGN KEY (`RegionId`) REFERENCES `Regions` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;

/*Table structure for table `Listings` */

CREATE TABLE `Listings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RegionId` int(11) NOT NULL DEFAULT '1',
  `UserId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `Description` varchar(2048) DEFAULT NULL,
  `Location` varchar(256) DEFAULT NULL,
  `Latitude` decimal(10,6) NOT NULL DEFAULT '0.000000',
  `Longitude` decimal(10,6) NOT NULL DEFAULT '0.000000',
  `Brand` varchar(256) DEFAULT NULL,
  `HorsePower` int(11) DEFAULT NULL,
  `Year` int(11) DEFAULT NULL,
  `ConditionId` smallint(6) NOT NULL DEFAULT '0',
  `GroupServices` tinyint(1) NOT NULL DEFAULT '0',
  `Photos` text,
  `AvailableWithoutFuel` tinyint(1) NOT NULL DEFAULT '0',
  `AvailableWithFuel` tinyint(1) NOT NULL DEFAULT '0',
  `AverageRating` decimal(10,3) NOT NULL DEFAULT '0.000',
  `RatingCount` int(11) NOT NULL DEFAULT '0',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `Trending` tinyint(1) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `Sublocality` varchar(256) DEFAULT NULL,
  `Locality` varchar(256) DEFAULT NULL,
  `ColloquialArea` varchar(256) DEFAULT NULL,
  `AdministrativeAreaLevel1` varchar(256) DEFAULT NULL,
  `Country` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `CategoryId` (`CategoryId`),
  KEY `RegionId` (`RegionId`),
  CONSTRAINT `listings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `listings_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `listings_ibfk_3` FOREIGN KEY (`RegionId`) REFERENCES `Regions` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Log` */

CREATE TABLE `Log` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `User` varchar(256) DEFAULT NULL,
  `Title` varchar(1024) DEFAULT NULL,
  `Description` longtext,
  `LevelId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1599 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Messages` */

CREATE TABLE `Messages` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `GUID` varchar(128) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  `ParentId` int(11) DEFAULT NULL,
  `Name` varchar(32) DEFAULT NULL,
  `EmailAddress` varchar(128) DEFAULT NULL,
  `Telephone` varchar(32) DEFAULT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `Message` varchar(4096) DEFAULT NULL,
  `Status` smallint(6) NOT NULL DEFAULT '0',
  `ReplyCount` int(11) NOT NULL DEFAULT '0',
  `Date` datetime NOT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `Deleted` (`Deleted`),
  KEY `Date` (`Date`),
  CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Notifications` */

CREATE TABLE `Notifications` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `BookingId` int(11) DEFAULT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `GroupId` smallint(6) NOT NULL DEFAULT '0',
  `Message` varchar(4096) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `notifications_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=65 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Pages` */

CREATE TABLE `Pages` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ParentId` int(11) DEFAULT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Title` varchar(256) DEFAULT NULL,
  `Description` varchar(160) DEFAULT NULL,
  `Content` longtext,
  `Icon` varchar(64) DEFAULT NULL,
  `UrlPath` varchar(256) DEFAULT NULL,
  `Greedy` tinyint(1) NOT NULL DEFAULT '0',
  `TemplatePath` varchar(1024) DEFAULT NULL,
  `ShowInMenu` tinyint(1) NOT NULL DEFAULT '1',
  `SortOrder` int(11) NOT NULL DEFAULT '1',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `cmspages_ibfk_1` (`ParentId`),
  CONSTRAINT `pages_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `Pages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Products` */

CREATE TABLE `Products` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `SupplierId` int(11) NOT NULL,
  `Title` varchar(128) DEFAULT NULL,
  `Description` varchar(1024) DEFAULT NULL,
  `DayRate` decimal(10,3) NOT NULL,
  `Stock` int(11) NOT NULL DEFAULT '0',
  `Photo` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `SupplierId` (`SupplierId`),
  KEY `Deleted` (`Deleted`),
  KEY `Title` (`Title`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Ratings` */

CREATE TABLE `Ratings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ListingId` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `BookingId` int(11) NOT NULL,
  `Comments` varchar(1024) DEFAULT NULL,
  `Rating` decimal(10,1) NOT NULL DEFAULT '0.0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ListingId` (`ListingId`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  CONSTRAINT `ratings_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `ratings_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `ratings_ibfk_3` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Regions` */

CREATE TABLE `Regions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(64) DEFAULT NULL,
  `Currency` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `Title` (`Title`),
  KEY `Deleted` (`Deleted`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Services` */

CREATE TABLE `Services` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ListingId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `Mobile` tinyint(1) NOT NULL DEFAULT '0',
  `TotalVolume` decimal(10,3) NOT NULL DEFAULT '0.000',
  `QuantityUnitId` smallint(6) NOT NULL DEFAULT '0',
  `TimeUnitId` smallint(6) NOT NULL DEFAULT '0',
  `DistanceUnitId` smallint(6) NOT NULL DEFAULT '0',
  `MinimumQuantity` decimal(10,3) NOT NULL DEFAULT '0.000',
  `MaximumDistance` decimal(10,3) NOT NULL DEFAULT '0.000',
  `PricePerQuantityUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `FuelPerQuantityUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `TimePerQuantityUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `PricePerDistanceUnit` decimal(10,3) NOT NULL DEFAULT '0.000',
  `FuelPrice` decimal(10,3) NOT NULL DEFAULT '0.000',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `MaximumDistanceToWaterSource` decimal(10,3) NOT NULL DEFAULT '0.000',
  `MaximumDepthOfWaterSource` decimal(10,3) NOT NULL DEFAULT '0.000',
  `UnclearedLand` tinyint(1) NOT NULL DEFAULT '0',
  `ClearedLand` tinyint(1) NOT NULL DEFAULT '0',
  `NearWaterSource` tinyint(1) NOT NULL DEFAULT '0',
  `FertileSoil` tinyint(1) NOT NULL DEFAULT '0',
  `LandRegion` smallint(6) NOT NULL DEFAULT '0',
  `MaxRentalYears` int(11) NOT NULL DEFAULT '0',
  `AvailableAcres` decimal(10,3) NOT NULL DEFAULT '0.000',
  `MinimumAcres` decimal(10,3) NOT NULL DEFAULT '0.000',
  `LabourServices` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ListingId` (`ListingId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `services_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `services_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Suppliers` */

CREATE TABLE `Suppliers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RegionId` int(11) NOT NULL DEFAULT '1',
  `Title` varchar(128) DEFAULT NULL,
  `Logo` varchar(1024) DEFAULT NULL,
  `Location` varchar(256) DEFAULT NULL,
  `Latitude` decimal(10,6) NOT NULL,
  `Longitude` decimal(10,6) NOT NULL,
  `TransportCostPerKm` decimal(10,3) NOT NULL DEFAULT '0.000',
  `MaximumDistance` int(11) NOT NULL DEFAULT '150',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `RegionId` (`RegionId`),
  KEY `Title` (`Title`),
  KEY `Deleted` (`Deleted`),
  CONSTRAINT `suppliers_ibfk_1` FOREIGN KEY (`RegionId`) REFERENCES `Regions` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Tags` */

CREATE TABLE `Tags` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(64) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Templates` */

CREATE TABLE `Templates` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Title` varchar(256) DEFAULT NULL,
  `HTML` longtext,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `TransactionFees` */

CREATE TABLE `TransactionFees` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MinimumValue` decimal(10,3) NOT NULL DEFAULT '0.000',
  `MaximumValue` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Fee` decimal(10,3) NOT NULL DEFAULT '0.000',
  `FeeType` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Transactions` */

CREATE TABLE `Transactions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClientCorrelator` varchar(64) NOT NULL,
  `BookingId` int(11) NOT NULL,
  `BookingUserId` int(11) NOT NULL,
  `ServerReference` varchar(256) DEFAULT NULL,
  `EcoCashReference` varchar(256) DEFAULT NULL,
  `Amount` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Currency` smallint(6) NOT NULL DEFAULT '1',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `Log` text,
  `Error` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `Gateway` smallint(6) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `BookingId` (`BookingId`),
  KEY `BookingUserId` (`BookingUserId`),
  CONSTRAINT `transactions_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `transactions_ibfk_3` FOREIGN KEY (`BookingUserId`) REFERENCES `BookingUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `UserVouchers` */

CREATE TABLE `UserVouchers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `VoucherId` int(11) NOT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `VoucherId` (`VoucherId`),
  CONSTRAINT `uservouchers_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `uservouchers_ibfk_2` FOREIGN KEY (`VoucherId`) REFERENCES `Vouchers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `Users` */

CREATE TABLE `Users` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RegionId` int(11) NOT NULL DEFAULT '1',
  `FirstName` varchar(256) DEFAULT NULL,
  `LastName` varchar(256) DEFAULT NULL,
  `EmailAddress` varchar(1024) DEFAULT NULL,
  `Telephone` varchar(16) DEFAULT NULL,
  `GenderId` smallint(6) NOT NULL DEFAULT '0',
  `DateOfBirth` date DEFAULT NULL,
  `Password` varchar(1024) DEFAULT NULL,
  `Salt` varchar(512) DEFAULT NULL,
  `AuthToken` varchar(1024) DEFAULT NULL,
  `FailedLoginAttempts` int(11) NOT NULL DEFAULT '0',
  `FailedVoucherAttempts` int(11) NOT NULL DEFAULT '0',
  `VerificationCode` varchar(8) DEFAULT NULL,
  `VerificationCodeExpiry` datetime DEFAULT NULL,
  `NotificationPreferences` smallint(6) NOT NULL DEFAULT '0',
  `InterestId` smallint(6) NOT NULL COMMENT 'Enum: Seeking, Offering',
  `LanguageId` smallint(6) NOT NULL DEFAULT '1',
  `AgentId` int(11) DEFAULT NULL,
  `AgentTypeId` smallint(6) NOT NULL DEFAULT '0',
  `SignalRConnectionId` varchar(256) DEFAULT NULL,
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Verified',
  `RoleList` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `PaymentMethod` smallint(6) NOT NULL DEFAULT '0',
  `BankAccount` varchar(4096) DEFAULT NULL,
  `SupplierId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `AgentId` (`AgentId`),
  KEY `RegionId` (`RegionId`),
  KEY `SupplierId` (`SupplierId`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`AgentId`) REFERENCES `Agents` (`Id`),
  CONSTRAINT `users_ibfk_2` FOREIGN KEY (`RegionId`) REFERENCES `Regions` (`Id`),
  CONSTRAINT `users_ibfk_3` FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10011 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Vouchers` */

CREATE TABLE `Vouchers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Title` varchar(128) DEFAULT NULL,
  `Code` varchar(16) DEFAULT NULL,
  `Amount` decimal(10,3) NOT NULL,
  `RedeemCount` int(11) NOT NULL DEFAULT '0',
  `MaxRedeemCount` int(11) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

/* Function  structure for function  `GetDistance` */

DELIMITER $$

/*!50003 CREATE DEFINER=`root`@`localhost` FUNCTION `GetDistance`(a DOUBLE, b DOUBLE, c DOUBLE, d DOUBLE) RETURNS double
BEGIN
	RETURN ST_Distance(POINT(a, b), POINT(c, d));
    END */$$
DELIMITER ;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

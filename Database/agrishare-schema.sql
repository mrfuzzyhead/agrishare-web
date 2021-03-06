/*
SQLyog Ultimate
MySQL - 5.7.21-log : Database - agrishare_live
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
/*Table structure for table `Agents` */

DROP TABLE IF EXISTS `Agents`;

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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Blogs` */

DROP TABLE IF EXISTS `Blogs`;

CREATE TABLE `Blogs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(1024) DEFAULT NULL,
  `Photo` varchar(1024) DEFAULT NULL,
  `Content` text,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

/*Table structure for table `BookingUsers` */

DROP TABLE IF EXISTS `BookingUsers`;

CREATE TABLE `BookingUsers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) DEFAULT NULL,
  `BookingId` int(11) NOT NULL,
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
  KEY `BookingId` (`BookingId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `bookingusers_ibfk_1` FOREIGN KEY (`BookingId`) REFERENCES `bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `bookingusers_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=latin1;

/*Table structure for table `Bookings` */

DROP TABLE IF EXISTS `Bookings`;

CREATE TABLE `Bookings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ForId` smallint(6) NOT NULL COMMENT 'Enum: Me, Friend, Group',
  `UserId` int(11) NOT NULL,
  `ListingId` int(11) NOT NULL,
  `ServiceId` int(11) NOT NULL,
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
  `TotalVolume` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Commission` decimal(10,3) NOT NULL DEFAULT '0.000',
  `AgentCommission` decimal(10,3) NOT NULL DEFAULT '0.000',
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Approved, Declined, In Progress, Complete',
  `PaidOut` tinyint(1) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `SMSCount` int(11) NOT NULL DEFAULT '0',
  `SMSCost` decimal(10,3) NOT NULL DEFAULT '0.000',
  `TransactionFee` decimal(10,3) NOT NULL DEFAULT '0.000',
  `IMTT` decimal(10,3) NOT NULL DEFAULT '0.000',
  `ReceiptPhoto` varchar(1024) DEFAULT NULL,
  `PaymentMethodId` int(6) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `ListingId` (`ListingId`),
  KEY `bookings_ibfk_3` (`ServiceId`),
  CONSTRAINT `bookings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `bookings_ibfk_2` FOREIGN KEY (`ListingId`) REFERENCES `listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `bookings_ibfk_3` FOREIGN KEY (`ServiceId`) REFERENCES `services` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8730 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Categories` */

DROP TABLE IF EXISTS `Categories`;

CREATE TABLE `Categories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ParentId` int(11) DEFAULT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Config` */

DROP TABLE IF EXISTS `Config`;

CREATE TABLE `Config` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Key` varchar(256) DEFAULT NULL,
  `Value` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Counters` */

DROP TABLE IF EXISTS `Counters`;

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
  CONSTRAINT `counters_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `counters_ibfk_3` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `counters_ibfk_4` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=165 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Devices` */

DROP TABLE IF EXISTS `Devices`;

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
  CONSTRAINT `devices_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Faqs` */

DROP TABLE IF EXISTS `Faqs`;

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
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Journals` */

DROP TABLE IF EXISTS `Journals`;

CREATE TABLE `Journals` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(256) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  `BookingId` int(11) DEFAULT NULL,
  `Amount` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Reconciled` tinyint(1) NOT NULL DEFAULT '0',
  `EcoCashReference` varchar(128) DEFAULT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  CONSTRAINT `journals_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `journals_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `bookings` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=81 DEFAULT CHARSET=latin1;

/*Table structure for table `Listings` */

DROP TABLE IF EXISTS `Listings`;

CREATE TABLE `Listings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `Description` varchar(2048) DEFAULT NULL,
  `Location` varchar(256) DEFAULT NULL,
  `Latitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Longitude` decimal(11,8) NOT NULL DEFAULT '0.00000000',
  `Brand` varchar(256) DEFAULT NULL,
  `HorsePower` int(11) DEFAULT NULL,
  `Year` int(11) DEFAULT NULL,
  `ConditionId` smallint(6) NOT NULL DEFAULT '0',
  `GroupServices` tinyint(1) NOT NULL DEFAULT '0',
  `AvailableWithoutFuel` tinyint(1) NOT NULL DEFAULT '0',
  `AvailableWithFuel` tinyint(1) NOT NULL DEFAULT '0',
  `Photos` text,
  `AverageRating` decimal(10,3) NOT NULL DEFAULT '0.000',
  `RatingCount` int(11) NOT NULL DEFAULT '0',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `listings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `listings_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4867 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Log` */

DROP TABLE IF EXISTS `Log`;

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
) ENGINE=InnoDB AUTO_INCREMENT=1242 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Messages` */

DROP TABLE IF EXISTS `Messages`;

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
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Notifications` */

DROP TABLE IF EXISTS `Notifications`;

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
  CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `notifications_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2109 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Pages` */

DROP TABLE IF EXISTS `Pages`;

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
  CONSTRAINT `pages_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `pages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Ratings` */

DROP TABLE IF EXISTS `Ratings`;

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
  CONSTRAINT `ratings_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `ratings_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `ratings_ibfk_3` FOREIGN KEY (`BookingId`) REFERENCES `bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Services` */

DROP TABLE IF EXISTS `Services`;

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
  PRIMARY KEY (`Id`),
  KEY `ListingId` (`ListingId`),
  KEY `SubcategoryId` (`CategoryId`),
  CONSTRAINT `services_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `services_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=991 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Templates` */

DROP TABLE IF EXISTS `Templates`;

CREATE TABLE `Templates` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `Title` varchar(256) DEFAULT NULL,
  `HTML` longtext,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `TransactionFees` */

DROP TABLE IF EXISTS `TransactionFees`;

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

DROP TABLE IF EXISTS `Transactions`;

CREATE TABLE `Transactions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClientCorrelator` varchar(64) NOT NULL,
  `BookingId` int(11) NOT NULL,
  `BookingUserId` int(11) NOT NULL,
  `ServerReference` varchar(256) DEFAULT NULL,
  `EcoCashReference` varchar(256) DEFAULT NULL,
  `Amount` decimal(10,3) NOT NULL DEFAULT '0.000',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `Log` text,
  `Error` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `BookingId` (`BookingId`),
  KEY `BookingUserId` (`BookingUserId`),
  CONSTRAINT `transactions_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `bookings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `transactions_ibfk_3` FOREIGN KEY (`BookingUserId`) REFERENCES `bookingusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=319 DEFAULT CHARSET=utf8mb4;

/*Table structure for table `Users` */

DROP TABLE IF EXISTS `Users`;

CREATE TABLE `Users` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
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
  `VerificationCode` varchar(8) DEFAULT NULL,
  `VerificationCodeExpiry` datetime DEFAULT NULL,
  `NotificationPreferences` smallint(6) NOT NULL DEFAULT '0',
  `InterestId` smallint(6) NOT NULL COMMENT 'Enum: Seeking, Offering',
  `LanguageId` smallint(6) NOT NULL DEFAULT '1',
  `AgentId` int(11) DEFAULT NULL,
  `SignalRConnectionId` varchar(256) DEFAULT NULL,
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Verified',
  `RoleList` varchar(1024) DEFAULT NULL,
  `AgentTypeId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `PaymentMethod` smallint(6) NOT NULL DEFAULT '0',
  `BankAccount` varchar(2048) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `AgentId` (`AgentId`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`AgentId`) REFERENCES `agents` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=44940 DEFAULT CHARSET=utf8mb4;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

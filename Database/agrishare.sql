/*
SQLyog Ultimate v12.2.5 (64 bit)
MySQL - 5.7.21-log : Database - agrishare
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
/*Table structure for table `BookingUsers` */

DROP TABLE IF EXISTS `BookingUsers`;

CREATE TABLE `BookingUsers` (
  `Id` int(11) NOT NULL,
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
  CONSTRAINT `bookingusers_ibfk_1` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `BookingUsers` */

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
  `Quantity` decimal(10,3) NOT NULL DEFAULT '0.000',
  `Distance` decimal(10,3) NOT NULL,
  `IncludeFuel` tinyint(1) NOT NULL DEFAULT '0',
  `StartDate` date NOT NULL,
  `EndDate` date NOT NULL,
  `Price` decimal(10,3) NOT NULL DEFAULT '0.000',
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Approved, Declined, In Progress, Complete',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `ListingId` (`ListingId`),
  KEY `bookings_ibfk_3` (`ServiceId`),
  CONSTRAINT `bookings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `bookings_ibfk_2` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `bookings_ibfk_3` FOREIGN KEY (`ServiceId`) REFERENCES `Services` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Data for the table `Bookings` */

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
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4;

/*Data for the table `Config` */

insert  into `Config`(`Id`,`Key`,`Value`,`DateCreated`,`LastModified`,`Deleted`) values 
(1,'Application Name','AgriShare','2018-09-07 10:53:24','2018-09-07 10:53:24',0),
(2,'SMS Username','agrishare_dev_remote','2018-09-07 10:53:50','2018-09-07 10:53:50',0),
(3,'SMS Sender','AgriShare','2018-09-07 10:54:06','2018-09-07 10:54:06',0),
(4,'Log API','True','2018-09-07 10:54:14','2018-09-07 10:54:14',0),
(5,'Live Email','False','2018-09-07 12:16:34','2018-09-07 12:16:34',0),
(6,'Developer Email Address','brad+agrishare@c2.co.zw','2018-09-07 12:17:24','2018-09-07 12:17:55',0),
(7,'Application Email Address','hello@agrishare.app','2018-09-07 12:19:31','2018-09-07 12:19:31',0),
(8,'Mandrill API Key','0JP6hLVgdN26ONftkdFbsA','2018-09-07 12:20:17','2018-09-07 12:20:17',0),
(9,'Redis Connection','localhost:6379,defaultDatabase=11','2018-09-07 12:26:50','2018-09-07 12:27:01',0),
(10,'Web URL','localhost','2018-09-07 12:29:59','2018-09-07 12:29:59',0),
(11,'CDN Absolute Path','C:\\Users\\Brad\\Documents\\Working Files\\Agrishare\\CDN\\','2018-09-07 14:23:08','2018-09-07 14:24:10',0);

/*Table structure for table `Counters` */

DROP TABLE IF EXISTS `Counters`;

CREATE TABLE `Counters` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Event` varchar(16) DEFAULT NULL,
  `Category` varchar(16) DEFAULT NULL,
  `Date` date NOT NULL,
  `Hits` int(11) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

/*Data for the table `Counters` */

insert  into `Counters`(`Id`,`Event`,`Category`,`Date`,`Hits`,`DateCreated`,`LastModified`,`Deleted`) values 
(1,'Launch','App','2018-09-16',4,'2018-09-16 11:48:39','2018-09-16 11:48:39',0);

/*Table structure for table `Faqs` */

DROP TABLE IF EXISTS `Faqs`;

CREATE TABLE `Faqs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Question` varchar(4096) DEFAULT NULL,
  `Answer` text,
  `SortOrder` int(11) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Data for the table `Faqs` */

/*Table structure for table `Listings` */

DROP TABLE IF EXISTS `Listings`;

CREATE TABLE `Listings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
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
  `AverageRating` decimal(10,3) NOT NULL DEFAULT '0.000',
  `RatingCount` int(11) NOT NULL DEFAULT '0',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `listings_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `listings_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

/*Data for the table `Listings` */

insert  into `Listings`(`Id`,`UserId`,`CategoryId`,`Title`,`Description`,`Location`,`Latitude`,`Longitude`,`Brand`,`HorsePower`,`Year`,`ConditionId`,`GroupServices`,`Photos`,`AverageRating`,`RatingCount`,`StatusId`,`DateCreated`,`LastModified`,`Deleted`) values 
(2,10000,1,'Tractor Services For You','Lorem ipsum dolores sit amet','Harare','-17.804540','31.126410','John Deere',60,2006,1,1,NULL,'0.000',0,1,'2018-09-17 14:33:07','2018-09-17 14:33:07',0),
(3,10000,1,'Cultivation Is Our Game','Lorem ipsum dolores sit amet','Harare','-17.804540','31.126410','John Deere',60,2006,1,1,NULL,'0.000',0,1,'2018-09-17 14:35:26','2018-09-17 14:35:26',0),
(4,10000,1,'Planting Plants','Lorem ipsum dolores sit amet','Harare','-17.804540','31.126410','MANLY',2500,2006,1,1,NULL,'0.000',0,1,'2018-09-17 14:36:55','2018-09-17 14:47:52',1);

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
) ENGINE=InnoDB AUTO_INCREMENT=63 DEFAULT CHARSET=utf8mb4;

/*Data for the table `Log` */

insert  into `Log`(`Id`,`User`,`Title`,`Description`,`LevelId`,`DateCreated`,`LastModified`,`Deleted`) values 
(1,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 13:11:51','2018-09-07 13:11:51',0),
(2,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 13:12:03','2018-09-07 13:12:03',0),
(3,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 13:18:28','2018-09-07 13:18:28',0),
(4,'','/register/telephone/lookup','-ENDPOINT-\r\n/register/telephone/lookup\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\nPlease register to continue\r\n\r\n',1,'2018-09-07 13:20:45','2018-09-07 13:20:45',0),
(5,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 13:23:52','2018-09-07 13:23:52',0),
(6,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 13:24:38','2018-09-07 13:24:38',0),
(7,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 13:25:25','2018-09-07 13:25:25',0),
(8,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 13:26:42','2018-09-07 13:26:42',0),
(9,'','/register','-ENDPOINT-\r\n/register\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 2\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-REQUEST BODY-\r\n{}\r\n-RESPONSE-\r\n is not a valid cell number. The number should start with 07 and contain 10 digits.\r\n\r\n',1,'2018-09-07 13:26:50','2018-09-07 13:26:50',0),
(10,'','/register','-ENDPOINT-\r\n/register\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 50\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-REQUEST BODY-\r\n{\n	\"FirstName\": \"Bradley\",\n	\"LastName\": \"Searle\"\n}\r\n-RESPONSE-\r\n{\"User.Telephone\":{\"Value\":null,\"Errors\":[{\"Exception\":null,\"ErrorMessage\":\"The Telephone field is required.\"}]}}\r\n\r\n',1,'2018-09-07 13:35:59','2018-09-07 13:35:59',0),
(11,'','/register','-ENDPOINT-\r\n/register\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 124\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-REQUEST BODY-\r\n{\n	\"FirstName\": \"Bradley\",\n	\"LastName\": \"Searle\",\n	\"Telephone\": \"0774185815\",\n	\"DateOfBirth\": \"1977-03-22\",\n	\"GenderId\": 1\n}\r\n-RESPONSE-\r\n{\"User\":{\"Id\":1,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":null,\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":null,\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":true,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}\r\n\r\n',1,'2018-09-07 13:44:16','2018-09-07 13:44:16',0),
(12,'','/register','-ENDPOINT-\r\n/register\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 158\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-REQUEST BODY-\r\n{\n	\"FirstName\": \"Bradley\",\n	\"LastName\": \"Searle\",\n	\"EmailAddress\": \"brad@c2.co.zw\",\n	\"Telephone\": \"0774185815\",\n	\"DateOfBirth\": \"1977-03-22\",\n	\"GenderId\": 1\n}\r\n-RESPONSE-\r\n0774185815 has already been registered\r\n\r\n',1,'2018-09-07 13:45:03','2018-09-07 13:45:03',0),
(13,'','/register/telephone/lookup','-ENDPOINT-\r\n/register/telephone/lookup\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\nPlease register to continue\r\n\r\n',1,'2018-09-07 13:46:01','2018-09-07 13:46:01',0),
(14,'','/register/telephone/lookup','-ENDPOINT-\r\n/register/telephone/lookup\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\nPlease register to continue\r\n\r\n',1,'2018-09-07 13:46:06','2018-09-07 13:46:06',0),
(15,'','/register/telephone/lookup','-ENDPOINT-\r\n/register/telephone/lookup\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\nPlease register to continue\r\n\r\n',1,'2018-09-07 13:46:45','2018-09-07 13:46:45',0),
(16,'','/register/telephone/lookup','-ENDPOINT-\r\n/register/telephone/lookup\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\nPlease register to continue\r\n\r\n',1,'2018-09-07 13:46:47','2018-09-07 13:46:47',0),
(17,'','/register/telephone/lookup','-ENDPOINT-\r\n/register/telephone/lookup\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\n{\"User\":{\"Id\":1,\"FirstName\":\"Bradley\",\"StatusId\":1}}\r\n\r\n',1,'2018-09-07 13:47:22','2018-09-07 13:47:22',0),
(18,'','/register/code/verify','-ENDPOINT-\r\n/register/code/verify\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nUserId: 1\r\nCode: 27492\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 13:48:55','2018-09-07 13:48:55',0),
(19,'','/register/telephone/lookup','-ENDPOINT-\r\n/register/telephone/lookup\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\nPlease register to continue\r\n\r\n',1,'2018-09-07 13:52:36','2018-09-07 13:52:36',0),
(20,'','/register','-ENDPOINT-\r\n/register\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 174\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-REQUEST BODY-\r\n{\n	\"FirstName\": \"Bradley\",\n	\"LastName\": \"Searle\",\n	\"EmailAddress\": \"brad@c2.co.zw\",\n	\"Telephone\": \"0774185815\",\n	\"PIN\": \"1234\",\n	\"DateOfBirth\": \"1977-03-22\",\n	\"GenderId\": 1\n}\r\n-RESPONSE-\r\n{\"User\":{\"Id\":10000,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":\"brad@c2.co.zw\",\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":null,\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":true,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}\r\n\r\n',1,'2018-09-07 13:52:42','2018-09-07 13:52:42',0),
(21,'','/register/code/verify','-ENDPOINT-\r\n/register/code/verify\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nUserId: 1\r\nCode: 2954\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 13:54:37','2018-09-07 13:54:37',0),
(22,'','/login','-ENDPOINT-\r\n/login\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\nPIN: 12345\r\n\r\n-RESPONSE-\r\nPhone number or PIN not recognised.\r\n\r\n',1,'2018-09-07 13:57:32','2018-09-07 13:57:32',0),
(23,'','/login','-ENDPOINT-\r\n/login\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\nPIN: 1234\r\n\r\n-RESPONSE-\r\n{\"Auth\":{\"User\":{\"Id\":10000,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":\"brad@c2.co.zw\",\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":\"86a24ca4-b4f8-4c65-ba70-86ed0c918a82\",\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":true,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}}\r\n\r\n',1,'2018-09-07 13:57:42','2018-09-07 13:57:42',0),
(24,'','/code/resend','-ENDPOINT-\r\n/code/resend\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 07741858157\r\n\r\n-RESPONSE-\r\nUnable to send verification code - please try again\r\n\r\n',1,'2018-09-07 14:00:21','2018-09-07 14:00:21',0),
(25,'','/code/resend','-ENDPOINT-\r\n/code/resend\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nTelephone: 0774185815\r\n\r\n-RESPONSE-\r\n\"Please check your messages\"\r\n\r\n',1,'2018-09-07 14:00:25','2018-09-07 14:00:25',0),
(26,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 14:02:49','2018-09-07 14:02:49',0),
(27,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 14:03:06','2018-09-07 14:03:06',0),
(28,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 14:04:34','2018-09-07 14:04:34',0),
(29,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 36500\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:05:06','2018-09-07 14:05:06',0),
(30,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 36500\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:05:10','2018-09-07 14:05:10',0),
(31,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:05:15','2018-09-07 14:05:15',0),
(32,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:05:18','2018-09-07 14:05:18',0),
(33,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:05:32','2018-09-07 14:05:32',0),
(34,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 14:06:03','2018-09-07 14:06:03',0),
(35,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 14:07:10','2018-09-07 14:07:10',0),
(36,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:10:11','2018-09-07 14:10:11',0),
(37,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:10:18','2018-09-07 14:10:18',0),
(38,'','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\nInvalid code\r\n\r\n',1,'2018-09-07 14:10:20','2018-09-07 14:10:20',0),
(39,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: ga=C300CC001E004C003B00BA00EF007400E200BA00FE00F10056006B0017007200\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-07 14:10:41','2018-09-07 14:10:41',0),
(40,'Bradley Searle','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\n{\"Auth\":{\"User\":{\"Id\":10000,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":\"brad@c2.co.zw\",\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":\"86a24ca4-b4f8-4c65-ba70-86ed0c918a82\",\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":true,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}}\r\n\r\n',1,'2018-09-07 14:10:54','2018-09-07 14:10:54',0),
(41,'Bradley Searle','/pin/change','-ENDPOINT-\r\n/pin/change\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nCode: 3650\r\nPIN: 1111\r\n\r\n-RESPONSE-\r\n{\"User\":{\"Id\":10000,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":\"brad@c2.co.zw\",\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":\"86a24ca4-b4f8-4c65-ba70-86ed0c918a82\",\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":true,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}\r\n\r\n',1,'2018-09-07 14:11:22','2018-09-07 14:11:22',0),
(42,'Bradley Searle','/profile/edit','-ENDPOINT-\r\n/profile/edit\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 174\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-REQUEST BODY-\r\n{\n	\"FirstName\": \"Bradley\",\n	\"LastName\": \"Searle\",\n	\"EmailAddress\": \"brad@c2.co.zw\",\n	\"Telephone\": \"0774185815\",\n	\"PIN\": \"1234\",\n	\"DateOfBirth\": \"1977-03-22\",\n	\"GenderId\": 1\n}\r\n-RESPONSE-\r\n{\"User\":{\"Id\":10000,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":\"brad@c2.co.zw\",\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":\"86a24ca4-b4f8-4c65-ba70-86ed0c918a82\",\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":true,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}\r\n\r\n',1,'2018-09-07 14:13:18','2018-09-07 14:13:18',0),
(43,'Bradley Searle','/profile/update','-ENDPOINT-\r\n/profile/update\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 174\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-REQUEST BODY-\r\n{\n	\"FirstName\": \"Bradley\",\n	\"LastName\": \"Searle\",\n	\"EmailAddress\": \"brad@c2.co.zw\",\n	\"Telephone\": \"0774185815\",\n	\"PIN\": \"1234\",\n	\"DateOfBirth\": \"1977-03-22\",\n	\"GenderId\": 1\n}\r\n-RESPONSE-\r\n{\"User\":{\"Id\":10000,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":\"brad@c2.co.zw\",\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":\"86a24ca4-b4f8-4c65-ba70-86ed0c918a82\",\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":true,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}\r\n\r\n',1,'2018-09-07 14:15:45','2018-09-07 14:15:45',0),
(44,'Bradley Searle','/profile/preferences/notifications/update','-ENDPOINT-\r\n/profile/preferences/notifications/update\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.2.0\r\n\r\n-QUERYSTRING-\r\nSMS: True\r\nPushNotifications: False\r\nEmail: False\r\n\r\n-RESPONSE-\r\n{\"User\":{\"Id\":10000,\"FirstName\":\"Bradley\",\"LastName\":\"Searle\",\"EmailAddress\":\"brad@c2.co.zw\",\"Telephone\":\"0774185815\",\"DateOfBirth\":\"1977-03-22T00:00:00\",\"GenderId\":1,\"Gender\":\"Male\",\"AuthToken\":\"86a24ca4-b4f8-4c65-ba70-86ed0c918a82\",\"NotificationPreferences\":{\"SMS\":true,\"PushNotifications\":false,\"Email\":false},\"InterestId\":0,\"Interest\":\"None\"}}\r\n\r\n',1,'2018-09-07 14:15:57','2018-09-07 14:15:57',0),
(45,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: gloo_auth=5C00; ga=E800EC002B0059001F00BA00CD007000E2006001F700D0007300480033004C00\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-16 11:41:02','2018-09-16 11:41:02',0),
(46,'','/faqs/list','-ENDPOINT-\r\n/faqs/list\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: gloo_auth=5C00; ga=E800EC002B0059001F00BA00CD007000E2006001F700D0007300480033004C00\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-RESPONSE-\r\n{\"List\":[]}\r\n\r\n',1,'2018-09-16 11:42:21','2018-09-16 11:42:21',0),
(47,'','/counter/update','-ENDPOINT-\r\n/counter/update\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: gloo_auth=5C00; ga=E800EC002B0059001F00BA00CD007000E2006001F700D0007300480033004C00\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-QUERYSTRING-\r\nCategory: App\r\nEvent: Launch\r\nDate: 2018-09-16\r\nHits: 1\r\n\r\n-RESPONSE-\r\n\"OK\"\r\n\r\n',1,'2018-09-16 11:48:40','2018-09-16 11:48:40',0),
(48,'','/counter/update','-ENDPOINT-\r\n/counter/update\r\n\r\n-HEADERS-\r\nCache-Control: max-age=0\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: gloo_auth=5C00; ga=E800EC002B0059001F00BA00CD007000E2006001F700D0007300480033004C00\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-QUERYSTRING-\r\nCategory: App\r\nEvent: Launch\r\nDate: 2018-09-16\r\nHits: 1\r\n\r\n-RESPONSE-\r\n\"OK\"\r\n\r\n',1,'2018-09-16 11:48:47','2018-09-16 11:48:47',0),
(49,'','/counter/update','-ENDPOINT-\r\n/counter/update\r\n\r\n-HEADERS-\r\nCache-Control: max-age=0\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: gloo_auth=5C00; ga=E800EC002B0059001F00BA00CD007000E2006001F700D0007300480033004C00\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-QUERYSTRING-\r\nCategory: App\r\nEvent: Launch\r\nDate: 2018-09-16\r\nHits: 1\r\n\r\n-RESPONSE-\r\n\"OK\"\r\n\r\n',1,'2018-09-16 11:49:15','2018-09-16 11:49:15',0),
(50,'','/counter/update','-ENDPOINT-\r\n/counter/update\r\n\r\n-HEADERS-\r\nConnection: keep-alive\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8\r\nAccept-Encoding: gzip, deflate, br\r\nAccept-Language: en-US,en;q=0.9\r\nCookie: gloo_auth=5C00; ASP.NET_SessionId=bkltggb23yg4ejt3aq2sfzww; ga=C40012006D0030206300E600C1007E0172002D0078001D0044000F001100DB00\r\nHost: localhost:56750\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36\r\nUpgrade-Insecure-Requests: 1\r\n\r\n-QUERYSTRING-\r\nCategory: App\r\nEvent: Launch\r\nDate: 2018-09-16\r\nHits: 1\r\n\r\n-RESPONSE-\r\n\"OK\"\r\n\r\n',1,'2018-09-17 13:05:29','2018-09-17 13:05:29',0),
(51,'Bradley Searle','/listings/add','-ENDPOINT-\r\n/listings/add\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 288\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Brand: \"John Deere\",\n	CategoryId: 1,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 60,\n	Location: \"Harare\",\n	Mobile: true,\n	Services: null,\n	Title: \"Tractor Services For You\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Model.Services\":{\"Value\":null,\"Errors\":[{\"Exception\":null,\"ErrorMessage\":\"The Services field is required.\"}]}}\r\n\r\n',1,'2018-09-17 13:11:28','2018-09-17 13:11:28',0),
(52,'Bradley Searle','/listings/add','-ENDPOINT-\r\n/listings/add\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 519\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Brand: \"MAN\",\n	CategoryId: 1,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 2500,\n	Location: \"Harare\",\n	Services: [\n			{\n				SubcategoryId: 6,\n				Mobile: true,\n				QuantityUnitId: 1,\n				TimeUnitId: 1,\n				DistanceUnitId: 1,\n				MaximumDistance: 75,\n				PricePerQuantityUnit: 99.99,\n				FuelPerQuantityUnit: 15,\n				TimePerQuantityUnit: 3,\n				PricePerDistanceUnit: 18\n			}\n		],\n	Title: \"Planting Plants\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Listing\":{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":1,\"ParentId\":null,\"Title\":\"Tractors\",\"DateCreated\":\"2018-09-16T10:48:50\",\"LastModified\":\"2018-09-16T10:48:50\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MAN\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"ParentId\":1,\"Title\":\"Planting\",\"DateCreated\":\"2018-09-16T10:49:58\",\"LastModified\":\"2018-09-16T10:49:58\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:36:56\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"}}\r\n\r\n',1,'2018-09-17 14:36:56','2018-09-17 14:36:56',0),
(53,'Bradley Searle','/listings','-ENDPOINT-\r\n/listings\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-QUERYSTRING-\r\nPageIndex: 0\r\nPageSize: 20\r\n\r\n-RESPONSE-\r\n{\"List\":[{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MAN\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:36:56\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"},{\"Id\":3,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Cultivation Is Our Game\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"John Deere\",\"HorsePower\":60,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":2,\"ListingId\":3,\"Subcategory\":{\"Id\":5,\"Title\":\"Cultivation\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:35:27\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:35:26\"},{\"Id\":2,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Tractor Services For You\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"John Deere\",\"HorsePower\":60,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":1,\"ListingId\":2,\"Subcategory\":{\"Id\":4,\"Title\":\"Ploughing\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":50.000,\"PricePerQuantityUnit\":79.990,\"FuelPerQuantityUnit\":10.000,\"TimePerQuantityUnit\":2.000,\"PricePerDistanceUnit\":15.000,\"DateCreated\":\"2018-09-17T14:33:07\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:33:07\"}]}\r\n\r\n',1,'2018-09-17 14:38:29','2018-09-17 14:38:29',0),
(54,'Bradley Searle','/listings/edit','-ENDPOINT-\r\n/listings/edit\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 529\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Id: 4,\n	Brand: \"MANLY\",\n	CategoryId: 1,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 2500,\n	Location: \"Harare\",\n	Services: [\n			{\n				SubcategoryId: 6,\n				Mobile: true,\n				QuantityUnitId: 1,\n				TimeUnitId: 1,\n				DistanceUnitId: 1,\n				MaximumDistance: 75,\n				PricePerQuantityUnit: 99.99,\n				FuelPerQuantityUnit: 15,\n				TimePerQuantityUnit: 3,\n				PricePerDistanceUnit: 18\n			}\n		],\n	Title: \"Planting Plants\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Listing\":{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MANLY\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:36:56\"},{\"Id\":4,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:40:48\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"}}\r\n\r\n',1,'2018-09-17 14:40:48','2018-09-17 14:40:48',0),
(55,'Bradley Searle','/listings/edit','-ENDPOINT-\r\n/listings/edit\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 540\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Id: 4,\n	Brand: \"MANLY\",\n	CategoryId: 1,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 2500,\n	Location: \"Harare\",\n	Services: [\n			{\n				Id: 3,\n				SubcategoryId: 6,\n				Mobile: true,\n				QuantityUnitId: 1,\n				TimeUnitId: 1,\n				DistanceUnitId: 1,\n				MaximumDistance: 75,\n				PricePerQuantityUnit: 99.99,\n				FuelPerQuantityUnit: 15,\n				TimePerQuantityUnit: 3,\n				PricePerDistanceUnit: 18\n			}\n		],\n	Title: \"Planting Plants\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Listing\":{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MANLY\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:36:56\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"}}\r\n\r\n',1,'2018-09-17 14:41:23','2018-09-17 14:41:23',0),
(56,'Bradley Searle','/listings/edit','-ENDPOINT-\r\n/listings/edit\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 538\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Id: 4,\n	Brand: \"MANLY\",\n	CategoryId: 1,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 2500,\n	Location: \"Harare\",\n	Services: [\n			{\n				Id: 3,\n				SubcategoryId: 6,\n				Mobile: true,\n				QuantityUnitId: 1,\n				TimeUnitId: 1,\n				DistanceUnitId: 1,\n				MaximumDistance: 75,\n				PricePerQuantityUnit: 100,\n				FuelPerQuantityUnit: 20,\n				TimePerQuantityUnit: 5,\n				PricePerDistanceUnit: 20\n			}\n		],\n	Title: \"Planting Plants\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Listing\":{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MANLY\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:36:56\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"}}\r\n\r\n',1,'2018-09-17 14:41:44','2018-09-17 14:41:44',0),
(57,'Bradley Searle','/listings/edit','-ENDPOINT-\r\n/listings/edit\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 538\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Id: 4,\n	Brand: \"MANLY\",\n	CategoryId: 1,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 2500,\n	Location: \"Harare\",\n	Services: [\n			{\n				Id: 3,\n				SubcategoryId: 6,\n				Mobile: true,\n				QuantityUnitId: 1,\n				TimeUnitId: 1,\n				DistanceUnitId: 1,\n				MaximumDistance: 75,\n				PricePerQuantityUnit: 100,\n				FuelPerQuantityUnit: 20,\n				TimePerQuantityUnit: 5,\n				PricePerDistanceUnit: 20\n			}\n		],\n	Title: \"Planting Plants\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Listing\":{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MANLY\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":100.000,\"FuelPerQuantityUnit\":20.000,\"TimePerQuantityUnit\":5.000,\"PricePerDistanceUnit\":20.000,\"DateCreated\":\"2018-09-17T14:36:56\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"}}\r\n\r\n',1,'2018-09-17 14:42:16','2018-09-17 14:42:16',0),
(58,'Bradley Searle','/listings/edit','-ENDPOINT-\r\n/listings/edit\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 538\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Id: 4,\n	Brand: \"MANLY\",\n	CategoryId: 2,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 2500,\n	Location: \"Harare\",\n	Services: [\n			{\n				Id: 3,\n				SubcategoryId: 6,\n				Mobile: true,\n				QuantityUnitId: 1,\n				TimeUnitId: 1,\n				DistanceUnitId: 1,\n				MaximumDistance: 75,\n				PricePerQuantityUnit: 100,\n				FuelPerQuantityUnit: 20,\n				TimePerQuantityUnit: 5,\n				PricePerDistanceUnit: 20\n			}\n		],\n	Title: \"Planting Plants\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Listing\":{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":2,\"Title\":\"Lorries\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MANLY\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":100.000,\"FuelPerQuantityUnit\":20.000,\"TimePerQuantityUnit\":5.000,\"PricePerDistanceUnit\":20.000,\"DateCreated\":\"2018-09-17T14:36:56\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"}}\r\n\r\n',1,'2018-09-17 14:45:53','2018-09-17 14:45:53',0),
(59,'Bradley Searle','/listings/edit','-ENDPOINT-\r\n/listings/edit\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nContent-Length: 538\r\nContent-Type: application/json\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-REQUEST BODY-\r\n{\n	Id: 4,\n	Brand: \"MANLY\",\n	CategoryId: 1,\n	ConditionId: 1,\n	Latitude: -17.80454,\n	Longitude: 31.12641,\n	Description: \"Lorem ipsum dolores sit amet\",\n	GroupServices: true,\n	HorsePower: 2500,\n	Location: \"Harare\",\n	Services: [\n			{\n				Id: 3,\n				SubcategoryId: 6,\n				Mobile: true,\n				QuantityUnitId: 1,\n				TimeUnitId: 1,\n				DistanceUnitId: 1,\n				MaximumDistance: 75,\n				PricePerQuantityUnit: 100,\n				FuelPerQuantityUnit: 20,\n				TimePerQuantityUnit: 5,\n				PricePerDistanceUnit: 20\n			}\n		],\n	Title: \"Planting Plants\",\n	Year: 2006\n}\r\n\r\n-RESPONSE-\r\n{\"Listing\":{\"Id\":4,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Planting Plants\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"MANLY\",\"HorsePower\":2500,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":3,\"ListingId\":4,\"Subcategory\":{\"Id\":6,\"Title\":\"Planting\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":100.000,\"FuelPerQuantityUnit\":20.000,\"TimePerQuantityUnit\":5.000,\"PricePerDistanceUnit\":20.000,\"DateCreated\":\"2018-09-17T14:36:56\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:36:55\"}}\r\n\r\n',1,'2018-09-17 14:45:58','2018-09-17 14:45:58',0),
(60,'Bradley Searle','/listings/delete','-ENDPOINT-\r\n/listings/delete\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-QUERYSTRING-\r\nListingId: 999\r\n\r\n-RESPONSE-\r\nListing not found\r\n\r\n',1,'2018-09-17 14:47:20','2018-09-17 14:47:20',0),
(61,'Bradley Searle','/listings/delete','-ENDPOINT-\r\n/listings/delete\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-QUERYSTRING-\r\nListingId: 4\r\n\r\n-RESPONSE-\r\n{\"Id\":4}\r\n\r\n',1,'2018-09-17 14:47:52','2018-09-17 14:47:52',0),
(62,'Bradley Searle','/listings','-ENDPOINT-\r\n/listings\r\n\r\n-HEADERS-\r\nCache-Control: no-cache\r\nConnection: keep-alive\r\nAccept: */*\r\nAccept-Encoding: gzip, deflate\r\nAuthorization: 86a24ca4-b4f8-4c65-ba70-86ed0c918a82\r\nHost: localhost:56750\r\nUser-Agent: PostmanRuntime/7.3.0\r\n\r\n-QUERYSTRING-\r\nPageIndex: 0\r\nPageSize: 20\r\n\r\n-RESPONSE-\r\n{\"List\":[{\"Id\":3,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Cultivation Is Our Game\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"John Deere\",\"HorsePower\":60,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":2,\"ListingId\":3,\"Subcategory\":{\"Id\":5,\"Title\":\"Cultivation\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":75.000,\"PricePerQuantityUnit\":99.990,\"FuelPerQuantityUnit\":15.000,\"TimePerQuantityUnit\":3.000,\"PricePerDistanceUnit\":18.000,\"DateCreated\":\"2018-09-17T14:35:27\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:35:26\"},{\"Id\":2,\"UserId\":10000,\"Category\":{\"Id\":1,\"Title\":\"Tractors\"},\"Title\":\"Tractor Services For You\",\"Description\":\"Lorem ipsum dolores sit amet\",\"Location\":\"Harare\",\"Latitude\":-17.804540,\"Longitude\":31.126410,\"Brand\":\"John Deere\",\"HorsePower\":60,\"Year\":2006,\"ConditionId\":1,\"Condition\":\"New\",\"GroupServices\":true,\"Photos\":null,\"AverageRating\":0.000,\"RatingCount\":0,\"Services\":[{\"Id\":1,\"ListingId\":2,\"Subcategory\":{\"Id\":4,\"Title\":\"Ploughing\"},\"Mobile\":true,\"TotalVolume\":0.000,\"QuantityUnitId\":1,\"QuantityUnit\":\"Hectare\",\"TimeUnitId\":1,\"TimeUnit\":\"Hectares Per Hour\",\"DistanceUnitId\":1,\"DistanceUnit\":\"Km\",\"MinimumQuantity\":0.000,\"MaximumDistance\":50.000,\"PricePerQuantityUnit\":79.990,\"FuelPerQuantityUnit\":10.000,\"TimePerQuantityUnit\":2.000,\"PricePerDistanceUnit\":15.000,\"DateCreated\":\"2018-09-17T14:33:07\"}],\"StatusId\":1,\"Status\":\"Live\",\"DateCreated\":\"2018-09-17T14:33:07\"}]}\r\n\r\n',1,'2018-09-17 14:47:58','2018-09-17 14:47:58',0);

/*Table structure for table `Notifications` */

DROP TABLE IF EXISTS `Notifications`;

CREATE TABLE `Notifications` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `BookingId` int(11) DEFAULT NULL,
  `Title` varchar(1024) DEFAULT NULL,
  `TypeId` smallint(6) NOT NULL DEFAULT '0',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `notifications_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Data for the table `Notifications` */

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
  CONSTRAINT `pages_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `Pages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Data for the table `Pages` */

/*Table structure for table `Ratings` */

DROP TABLE IF EXISTS `Ratings`;

CREATE TABLE `Ratings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ListingId` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `Comments` varchar(1024) DEFAULT NULL,
  `Rating` decimal(10,1) NOT NULL DEFAULT '0.0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ListingId` (`ListingId`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ratings_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `ratings_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Data for the table `Ratings` */

/*Table structure for table `Services` */

DROP TABLE IF EXISTS `Services`;

CREATE TABLE `Services` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ListingId` int(11) NOT NULL,
  `SubcategoryId` int(11) NOT NULL,
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
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `ListingId` (`ListingId`),
  KEY `SubcategoryId` (`SubcategoryId`),
  CONSTRAINT `services_ibfk_1` FOREIGN KEY (`ListingId`) REFERENCES `Listings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `services_ibfk_2` FOREIGN KEY (`SubcategoryId`) REFERENCES `Categories` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

/*Data for the table `Services` */

insert  into `Services`(`Id`,`ListingId`,`SubcategoryId`,`Mobile`,`TotalVolume`,`QuantityUnitId`,`TimeUnitId`,`DistanceUnitId`,`MinimumQuantity`,`MaximumDistance`,`PricePerQuantityUnit`,`FuelPerQuantityUnit`,`TimePerQuantityUnit`,`PricePerDistanceUnit`,`DateCreated`,`LastModified`,`Deleted`) values 
(1,2,4,1,'0.000',1,1,1,'0.000','50.000','79.990','10.000','2.000','15.000','2018-09-17 14:33:07','2018-09-17 14:33:07',0),
(2,3,5,1,'0.000',1,1,1,'0.000','75.000','99.990','15.000','3.000','18.000','2018-09-17 14:35:27','2018-09-17 14:35:27',0),
(3,4,6,1,'0.000',1,1,1,'0.000','75.000','100.000','20.000','5.000','20.000','2018-09-17 14:36:56','2018-09-17 14:45:58',0);

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Data for the table `Templates` */

/*Table structure for table `Transactions` */

DROP TABLE IF EXISTS `Transactions`;

CREATE TABLE `Transactions` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `BookingId` int(11) NOT NULL,
  `BookingUserId` int(11) DEFAULT NULL,
  `Reference` varchar(256) DEFAULT NULL,
  `Amount` decimal(10,3) NOT NULL DEFAULT '0.000',
  `StatusId` smallint(6) NOT NULL DEFAULT '0',
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `BookingId` (`BookingId`),
  KEY `BookingUserId` (`BookingUserId`),
  CONSTRAINT `transactions_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `transactions_ibfk_2` FOREIGN KEY (`BookingId`) REFERENCES `Bookings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `transactions_ibfk_3` FOREIGN KEY (`BookingUserId`) REFERENCES `BookingUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*Data for the table `Transactions` */

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
  `SignalRConnectionId` varchar(256) DEFAULT NULL,
  `StatusId` smallint(6) NOT NULL DEFAULT '0' COMMENT 'Enum: Pending, Verified',
  `RoleList` varchar(1024) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10001 DEFAULT CHARSET=utf8mb4;

/*Data for the table `Users` */

insert  into `Users`(`Id`,`FirstName`,`LastName`,`EmailAddress`,`Telephone`,`GenderId`,`DateOfBirth`,`Password`,`Salt`,`AuthToken`,`FailedLoginAttempts`,`VerificationCode`,`VerificationCodeExpiry`,`NotificationPreferences`,`InterestId`,`SignalRConnectionId`,`StatusId`,`RoleList`,`DateCreated`,`LastModified`,`Deleted`) values 
(10000,'Bradley','Searle','brad@c2.co.zw','0774185815',1,'1977-03-22','LXzUZ1cbn9TN+CuYOtAhbaeEocvIjyvg5Suk9PHYNlWX0Q4taq4myoQKiOvp7JWZUDQDsHsOxhOsM2UuQLDfNg==','cZfuV81bIXbsUOx/um3snn+VBLAmKpf1GOaGTs4yw3OnVMEksy03C8QkweZ6haM7uzYa31ISzQaaclTJnG+BGw==','86a24ca4-b4f8-4c65-ba70-86ed0c918a82',0,'3650','2018-09-08 12:00:25',1,0,NULL,2,'User','2018-09-07 13:52:41','2018-09-07 14:15:57',0);

/*Table structure for table `categories` */

DROP TABLE IF EXISTS `categories`;

CREATE TABLE `categories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ParentId` int(11) DEFAULT NULL,
  `Title` varchar(256) DEFAULT NULL,
  `DateCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastModified` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4;

/*Data for the table `categories` */

insert  into `categories`(`Id`,`ParentId`,`Title`,`DateCreated`,`LastModified`,`Deleted`) values 
(1,NULL,'Tractors','2018-09-16 10:48:50','2018-09-16 10:48:50',0),
(2,NULL,'Lorries','2018-09-16 10:48:52','2018-09-16 10:48:52',0),
(3,NULL,'Processing','2018-09-16 10:48:55','2018-09-16 10:48:55',0),
(4,1,'Ploughing','2018-09-16 10:49:30','2018-09-16 11:29:50',0),
(5,1,'Cultivation','2018-09-16 10:49:42','2018-09-16 11:29:46',0),
(6,1,'Planting','2018-09-16 10:49:58','2018-09-16 10:49:58',0),
(7,1,'Fertilizer Application','2018-09-16 10:50:05','2018-09-16 10:50:05',0),
(8,1,'Herbicide/Pesticide Spraying','2018-09-16 10:50:17','2018-09-16 11:29:35',0),
(9,1,'Discing','2018-09-16 10:50:19','2018-09-16 11:29:31',0),
(10,2,'Lorry','2018-09-17 13:17:06','2018-09-17 13:17:25',0),
(11,3,'Maize Grinder','2018-09-16 10:50:27','2018-09-17 13:17:23',0),
(12,3,'Maize Sheller','2018-09-16 10:50:33','2018-09-17 13:17:21',0),
(13,3,'Peanut Butter Maker','2018-09-16 10:50:36','2018-09-17 13:17:20',0),
(14,3,'Peanut Sheller','2018-09-16 10:51:11','2018-09-17 13:17:19',0);

/* Function  structure for function  `GetDistance` */

/*!50003 DROP FUNCTION IF EXISTS `GetDistance` */;
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

-- Export by KissEditor at 2023年6月4日 22:48:19

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `file`, for database file
--
DROP TABLE IF EXISTS `file`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `file` (
  `fileName` varchar(50) NOT NULL,
  `owner` int(11) NOT NULL,
  `data` mediumblob NOT NULL,
  PRIMARY KEY (`fileName`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `message`
--
DROP TABLE IF EXISTS `message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `message` (
	`uid` bigint(20) NOT NULL AUTO_INCREMENT,
	`title` text NOT NULL,
	`question` text NOT NULL,
	`nickname` text NOT NULL,
	`ip` text NOT NULL,
	`hideIp` tinyint(4) NOT NULL,
	`contact` text NOT NULL,
	`createTime` timestamp NULL DEFAULT NULL,
	`answer` text NOT NULL,
	`answerTime` timestamp NULL DEFAULT NULL,
	PRIMARY KEY (`uid`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `logaccount` in LogManager
--
DROP TABLE IF EXISTS `logaccount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `logaccount` (
	`uid` int(11) NOT NULL AUTO_INCREMENT,
	`acctId` int(11) NOT NULL,
	`logType` int(11) NOT NULL,
	`ip` text NOT NULL,
	`createTime` datetime NOT NULL,
	PRIMARY KEY (`uid`),
	KEY `index_acctId` (`acctId`)) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `logitem` in LogManager
--
DROP TABLE IF EXISTS `logitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `logitem` (
	`uid` int(11) NOT NULL AUTO_INCREMENT,
	`acctId` int(11) NOT NULL,
	`logType` int(11) NOT NULL,
	`changeCount` int(11) NOT NULL,
	`finalCount` int(11) NOT NULL,
	`createTime` datetime NOT NULL,
	PRIMARY KEY (`uid`),
	KEY `index_acctId` (`acctId`)) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `logmail` in LogManager
--
DROP TABLE IF EXISTS `logmail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `logmail` (
	`uid` int(11) NOT NULL AUTO_INCREMENT,
	`acctId` int(11) NOT NULL,
	`logType` int(11) NOT NULL,
	`appendix` text NOT NULL,
	`content` text NOT NULL,
	`title` text NOT NULL,
	`createTime` datetime NOT NULL,
	PRIMARY KEY (`uid`),
	KEY `index_acctId` (`acctId`)) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `account`
--
DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `account` (
	`uid` int(11) NOT NULL AUTO_INCREMENT,
	`acctType` int(11) NOT NULL,
	`name` varchar(64) NOT NULL,
	`createTime` timestamp NOT NULL,
	`password` varchar(64) NOT NULL,
	`nickname` varchar(64) NOT NULL,
	`money` int(11) NOT NULL DEFAULT 0,
	`token` varchar(64) NOT NULL,
	`tokenExpireTime` datetime NOT NULL DEFAULT current_timestamp(),
	`score` int(11) NOT NULL DEFAULT 0,
	`scoreTime` datetime NOT NULL DEFAULT current_timestamp(),
	`lastLoginTime` datetime NOT NULL DEFAULT current_timestamp(),
	`lastLoginIP` varchar(64) NOT NULL,
	`email` varchar(64) NOT NULL,
	PRIMARY KEY (`uid`),
	UNIQUE KEY `index_name` (`name`,`acctType`),
	KEY `index_token` (`token`),
	KEY `index_score` (`score`,`scoreTime`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mail`
--
DROP TABLE IF EXISTS `mail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mail` (
	`uid` int(11) NOT NULL AUTO_INCREMENT,
	`acctId` int(11) NOT NULL,
	`senderId` int(11) NOT NULL,
	`senderName` text NOT NULL,
	`title` text NOT NULL,
	`content` text NOT NULL,
	`appendix` text NOT NULL,
	`createTime` datetime NOT NULL,
	`wasRead` tinyint(4) NOT NULL DEFAULT 0,
	`received` tinyint(4) NOT NULL DEFAULT 0,
	PRIMARY KEY (`uid`),
	KEY `index_acctId` (`acctId`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `item`
--
DROP TABLE IF EXISTS `item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `item` (
	`uid` int(11) NOT NULL AUTO_INCREMENT,
	`itemId` int(11) NOT NULL,
	`acctId` int(11) NOT NULL,
	`count` int(11) NOT NULL,
	PRIMARY KEY (`uid`),
	KEY `index_acctId` (`acctId`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `signin`
--
DROP TABLE IF EXISTS `signin`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `signin` (
	`acctId` int(11) NOT NULL,
	`month` int(11) NOT NULL DEFAULT ,
	`signInList` text NOT NULL DEFAULT ,
	`vipSignInList` text NOT NULL DEFAULT ,
	PRIMARY KEY (`acctId`),
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;


/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

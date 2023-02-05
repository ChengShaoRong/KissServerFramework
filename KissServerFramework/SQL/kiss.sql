-- MariaDB dump 10.17  Distrib 10.4.11-MariaDB, for Win64 (AMD64)
--
-- Host: localhost    Database: kiss
-- ------------------------------------------------------
-- Server version	10.4.11-MariaDB

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
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `account` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `nickname` varchar(64) NOT NULL,
  `acctType` int(11) NOT NULL,
  `password` varchar(64) NOT NULL,
  `money` int(11) NOT NULL,
  `createTime` timestamp NOT NULL DEFAULT current_timestamp(),
  `score` int(11) NOT NULL,
  `scoreTime` datetime NOT NULL DEFAULT current_timestamp(),
  `lastLoginTime` datetime DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `index_name` (`name`,`acctType`) USING BTREE,
  KEY `index_score` (`score`,`scoreTime`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;
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
  KEY `index_acctId` (`acctId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `logaccount`
--

DROP TABLE IF EXISTS `logaccount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `logaccount` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `acctId` int(11) NOT NULL,
  `logType` int(11) NOT NULL,
  `ip` varchar(16) NOT NULL,
  `createTime` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`uid`),
  KEY `acctId` (`acctId`)
) ENGINE=InnoDB AUTO_INCREMENT=191 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `logitem`
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
  `createTime` int(11) NOT NULL,
  PRIMARY KEY (`uid`),
  KEY `index_acctId` (`acctId`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `logmail`
--

DROP TABLE IF EXISTS `logmail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `logmail` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `acctId` int(11) NOT NULL,
  `logType` int(11) NOT NULL,
  `title` text NOT NULL,
  `content` text NOT NULL,
  `appendix` text NOT NULL,
  `createTime` datetime NOT NULL,
  PRIMARY KEY (`uid`),
  KEY `index_acctId` (`acctId`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4;
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
  `wasRead` tinyint(4) NOT NULL,
  `received` tinyint(4) NOT NULL,
  PRIMARY KEY (`uid`),
  KEY `index_acctId` (`acctId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `signin`
--

DROP TABLE IF EXISTS `signin`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `signin` (
  `acctId` int(11) NOT NULL,
  `month` int(11) NOT NULL,
  `signInList` text NOT NULL,
  `vipSignInList` text NOT NULL,
  PRIMARY KEY (`acctId`)
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

-- Dump completed on 2023-02-02 17:52:51

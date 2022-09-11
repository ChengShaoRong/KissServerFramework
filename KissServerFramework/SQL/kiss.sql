-- phpMyAdmin SQL Dump
-- version 5.0.1
-- https://www.phpmyadmin.net/
--
-- 主机： 127.0.0.1
-- 生成日期： 2022-09-11 15:56:55
-- 服务器版本： 10.4.11-MariaDB
-- PHP 版本： 7.4.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 数据库： `kiss`
--
CREATE DATABASE IF NOT EXISTS `kiss` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `kiss`;

-- --------------------------------------------------------

--
-- 表的结构 `account`
--

DROP TABLE IF EXISTS `account`;
CREATE TABLE `account` (
  `uid` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `nickname` varchar(64) NOT NULL,
  `acctType` int(11) NOT NULL,
  `password` varchar(64) NOT NULL,
  `money` int(11) NOT NULL,
  `createTime` timestamp NOT NULL DEFAULT current_timestamp(),
  `score` int(11) NOT NULL,
  `scoreTime` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- 表的结构 `logaccount`
--

DROP TABLE IF EXISTS `logaccount`;
CREATE TABLE `logaccount` (
  `UID` int(11) NOT NULL,
  `acctUid` int(11) NOT NULL,
  `logType` int(11) NOT NULL,
  `ip` varchar(16) NOT NULL,
  `createTime` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- 表的结构 `rank`
--

DROP TABLE IF EXISTS `rank`;
CREATE TABLE `rank` (
  `uid` int(11) NOT NULL,
  `rank` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `score` int(11) NOT NULL,
  `createTime` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- 转储表的索引
--

--
-- 表的索引 `account`
--
ALTER TABLE `account`
  ADD PRIMARY KEY (`uid`),
  ADD UNIQUE KEY `index_name` (`name`,`acctType`) USING BTREE,
  ADD KEY `index_score` (`score`,`scoreTime`) USING BTREE;

--
-- 表的索引 `logaccount`
--
ALTER TABLE `logaccount`
  ADD PRIMARY KEY (`UID`);

--
-- 表的索引 `rank`
--
ALTER TABLE `rank`
  ADD UNIQUE KEY `unique_uid` (`uid`) USING BTREE,
  ADD KEY `index_rank` (`rank`);

--
-- 在导出的表使用AUTO_INCREMENT
--

--
-- 使用表AUTO_INCREMENT `account`
--
ALTER TABLE `account`
  MODIFY `uid` int(11) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `logaccount`
--
ALTER TABLE `logaccount`
  MODIFY `UID` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

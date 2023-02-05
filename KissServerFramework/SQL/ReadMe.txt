The 'kiss.sql' is the sql of KissServerFramework database. Here are the guide for set up the 'kiss' database.

1 Install MySQL. If you had installed MySQL, skip to next step.
	1.1 Download XAMPP from web page: 
		https://www.apachefriends.org/download.html
	1.2 Install it just click 'Next' button until finished.
		Why install XAMPP but not direct install MySQL? Because XAMPP is simple and stupid way to install MySQL, just next next next...
	1.3 Make sure the MySQL is running. You can check it in XAMPP Control Panel.

2 Create database 'kiss' and import database struct from 'kiss.sql'.
	2.1 Window platform
		2.1.1 Open the window command line prompt and change directory to your install path of MySQL.
			cd C:\xampp\mysql\bin
		2.1.2 Input command to create database 'kiss'
			mysqladmin -u root -p create kiss
		2.1.3 Input command to import database struct from 'kiss.sql'. That 'kiss.sql' you should repeace with your absolute path.
			mysql kiss < kiss.sql -u root -p

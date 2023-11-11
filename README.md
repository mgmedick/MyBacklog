## About

A site to create your own lists and add, remove and import games from them.

## Getting stated:

1. Install node.js and npm [Downloading and installing Node.js and npm](https://docs.npmjs.com/downloading-and-installing-node-js-and-npm).
2. Clone the repo using your preferred method.
   - Visual Studio Code should work out of the box.
   - Run "npm install" under GameStatsApp/GameStatsApp.UI to install packages.
   - Run "npm run build" to build clientside code.
2. There is no hosted test database so you’ll have to set one up locally (steps 3-8).
3. Make sure you have MySQL v8.0.35 or later version installed, instructions below.
   - [how to install mysql linux](https://www.digitalocean.com/community/tutorial_collections/how-to-install-mysql) (choose your distro).
   - [how to install mysql windows](https://www.lifewire.com/how-to-install-mysql-windows-10-4584021)
5. Find your mysqld.cnf (linux) or default.ini (windows) file.
   - linux: mysqld.cnf is normally located "/etc/mysql/mysql.conf.d/".
   - windows: default.ini is normally located "C:\ProgramData\MySQL\MySQL Server 8.0\" (hidden folder).
7. Add the following lines to the end of your mysqld.cnf (or default.ini if windows).
   - lower_case_table_names = 1
   - optimizer_switch=block_nested_loop=off
   - group_concat_max_len = 1000000
8. Follow step 2 of these instructions [how to import/export databases in mysql](https://www.digitalocean.com/community/tutorials/how-to-import-and-export-databases-in-mysql-or-mariadb) and import the [gamestatsapp_test_dump.sql](https://github.com/mgmedick/GameStatsAppDatabaseScripts/blob/main/gamestatsapp_test_dump.sql) MySQL dump file.
9. Edit the connection string in the "appsettings.json" file with your MySQL credentials, user (usually "root") and password.
10. Start debugging, email/message me if you have any questions.
    - Use login credentials below for user already created in "gamestatsapp_test" database.
       - Username: testuser@gmail.com
       - Password: E1gj#5Jkeb
    - Unfortunately you won't be able to register any new users while debugging because appsettings.json doesn't include email server credentials.
       - Additionally login with google/facebook won't work due to missing credentials.
       - Lastly the import feature won't work due to missing credentials.
    - If you need the additional credentials for testing signup/login/import just email/message me and I can send you non-redacted appsettings.json.

## Stack:

- Vue 3
- .Net 6.0
- MySQL 8.0
- Vanilla JavaScript
- Bootstrap 5.3

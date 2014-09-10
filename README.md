ASPTest
=======

Simple ASP.NET page with some database interaction

Uses the Microsoft Adventure Works Database (AdventureWorks2012 Data File) - [download here](http://msftdbprodsamples.codeplex.com/downloads/get/165399)

##### Requirements

* Requests from the user start and end date for the report

* Defaults (i.e., on the first page load) the start date to be the beginning of the previous month (e.g., if today is July 15, then the start date would be June 1)

* Defaults the end date to be the end of the previous month (e.g., if today is July 15, then the start date would be June 30)

* The table below will be filtered by due date to be between the start and end date provided by the user (i.e., by default, if today is July 15, the table would only show invoices dated between June 1 and June 30)

* Upon clicking the Submit button, shows in the HTML the first 15 rows of the table below

* Upon clicking the export button, generates an Excel Spreadsheet with all rows of the table below.

![the table](http://i.imgur.com/2KZXNbo.png)




﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using jabapp.EntityFramework;
using System.Collections;
using System.Diagnostics;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace jabapp {
	public partial class Site : System.Web.UI.Page {
		protected void Page_Load(object sender, EventArgs e) {
			if (!Page.IsPostBack) {
				//assume user is interested in last month's sales
				DateTime[] defaults = generateDefaultDates(System.DateTime.Now.AddMonths(-1));
				DateTime defaultStart = defaults[0];
				DateTime defaultEnd = defaults[1];
				CalDateSelector.VisibleDate = defaultStart;
				CalDateSelector.SelectedDate = defaultStart;
				CalDateSelector.SelectedDates.Add(defaultEnd);
				TextBoxStartDate.Text = defaultStart.ToShortDateString();
				TextBoxEndDate.Text = defaultEnd.ToShortDateString();
			}
		}

		protected void CalDateSelector_SelectionChanged(object sender, EventArgs e) {
			if (TextBoxStartDate.Text.Length > 1 && TextBoxEndDate.Text.Length > 1) { //adding a third selection -> clear all
				TextBoxStartDate.Text = "";
				TextBoxEndDate.Text = "";
				ButtonSubmit.Enabled = false;
				ButtonExport.Enabled = false;
			}

			if (TextBoxStartDate.Text.Length < 1) {
				TextBoxStartDate.Text = CalDateSelector.SelectedDate.ToShortDateString();
				CalDateSelector.SelectedDates.Add(DateTime.Parse(TextBoxStartDate.Text));
			} else {
				TextBoxEndDate.Text = CalDateSelector.SelectedDate.ToShortDateString();
				DateTime startDate = DateTime.Parse(TextBoxStartDate.Text);
				DateTime endDate = DateTime.Parse(TextBoxEndDate.Text);
				CalDateSelector.SelectedDates.Add(startDate);
				CalDateSelector.SelectedDates.Add(endDate);
				ButtonSubmit.Enabled = true;
				ButtonExport.Enabled = true;
			}
		}

		protected void ButtonSubmit_Click(object sender, EventArgs e) {
			if (CalDateSelector.SelectedDates.Count != 2) { return; } //shouldn't ever happen, but better safe than sorry

			ArrayList queryTable = getSalesBetweenDates(CalDateSelector.SelectedDates[0], CalDateSelector.SelectedDates[1], 15);
			//write all the results to a table
			for (int i = 0; i < queryTable.Count; i++) {
				TableRow row = new TableRow();
				foreach (String cellString in (String[])queryTable[i]) {
					TableCell cell = new TableCell();
					cell.Text = cellString;
					row.Cells.Add(cell);
				}
				OutputTable.Rows.Add(row);	
			}//for
		}//method

		protected void ButtonExport_Click(object sender, EventArgs e) {
			ArrayList queryTable = getSalesBetweenDates(CalDateSelector.SelectedDates[0], CalDateSelector.SelectedDates[1], -1);
			ExcelPackage excel = new ExcelPackage();
			ExcelWorkbook activeWorkbook = excel.Workbook;
			ExcelWorksheet activeSheet = activeWorkbook.Worksheets.Add("Invoice Report");
			String accountingStyle = activeWorkbook.CreateAccountingFormat();
			String normalStyle = "Normal";

			SalesRecord[] data = new SalesRecord[queryTable.Count - 1];
			for (int i = 1; i < queryTable.Count; i++) {
				String[] row = (String[])queryTable[i];
				data[i - 1] = new SalesRecord {
					SoldAt = row[0], SoldTo = row[1], AcctNo = row[2], InvoiceNo = row[3], CustomerPONo = row[4], OrderDate = row[5],
					DueDate = row[6], InvoiceTotal = row[7], ProductNo = row[8], OrderQTY = row[9], UnitNet = row[10], LineTotal = row[11] };
			}

			Column[] columns = new Column[((String[])queryTable[0]).Length];
			for (int i = 0; i < columns.Length; i++) {
				String style = ((String[])queryTable[0])[i].Contains("Total") || ((String[])queryTable[0])[i].Contains("Net") ? accountingStyle : normalStyle;
				columns[i] = new Column { Title = ((String[])queryTable[0])[i], Style = style, Action = j => j.iterateProp(), };


			}//for

			activeSheet.SaveData(columns, data);
			File.WriteAllBytes("..\\..\\Report.xlsx", excel.GetAsByteArray()); //saves to the C Drive. Inelegant, but it works.
		}

		/// <summary>
		/// Creates datetimes representing the first and last days of the given month (as specificed by userDate). 
		/// Null userdate will throw an exception.
		/// Returns 2 element array - [0] = start, [1] = end. 
		/// </summary>
		public static DateTime[] generateDefaultDates(DateTime userDate) {
			if (userDate == null) {
				throw new Exception("Please specify a valid userDate");
			}

			DateTime[] startAndEndDates = new DateTime[2];

			int defaultYear = userDate.Year;
			int defaultMonth = userDate.Month;
			int defaultStartDay = 1;
			int defaultEndDay = DateTime.DaysInMonth(defaultYear, defaultMonth);

			DateTime defaultStart = new DateTime(defaultYear, defaultMonth, defaultStartDay);
			DateTime defaultEnd = new DateTime(defaultYear, defaultMonth, defaultEndDay);
			startAndEndDates[0] = defaultStart;
			startAndEndDates[1] = defaultEnd;
			return startAndEndDates;
		}

		/// <summary>
		/// Returns a table of sales between the two given dates - inclusive, [start, end]. Table is an arraylist of String arrays.
		/// Will only search for recordLimit number of rows, -1 means all records.
		/// </summary>
		public static ArrayList getSalesBetweenDates(DateTime start, DateTime end, int recordLimit) {
			if (start.CompareTo(end) > 0) { //params in the wrong order
				DateTime temp = start;
				start = end;
				end = temp;
			}
			
			AdventureWorksDB db = new AdventureWorksDB();
			ArrayList table = new ArrayList();
			String[] headers = {"Sold At", "Sold To", "Account Number", "Invoice #", "Customer PO #", 
								   "Order Date", "Due Date", "Invoice Total", "Product Number", "Order Qty", "Unit Net", "Line Total"};
			table.Add(headers);
			using (db) {

				IQueryable<SalesOrderHeader> salesQuery = from purchase in db.SalesOrderHeaders
															 where purchase.DueDate.CompareTo(start) >= 0 && purchase.DueDate.CompareTo(end) <= 0
															 select purchase;
				
				int records = 0;
				foreach (SalesOrderHeader purchase in salesQuery) { //O(n^2) isn't the best, but it beats further database trawling.
					String[] row = new String[12];

					if (purchase.OnlineOrderFlag) {
						row[0] = "Online";
					} else {
						row[0] = purchase.Customer.Store.Name;
					}
					row[1] = purchase.Customer.Person.FirstName + " " + purchase.Customer.Person.LastName;
					row[2] = purchase.Customer.AccountNumber;
					row[3] = purchase.SalesOrderNumber;
					row[4] = purchase.PurchaseOrderNumber;
					row[5] = purchase.OrderDate.ToShortDateString();
					row[6] = purchase.DueDate.ToShortDateString();
					row[7] =  string.Format("{0:C}",purchase.TotalDue);

					bool multipleItems = false;
					foreach (SalesOrderDetail item in purchase.SalesOrderDetails) {
						String[] aliasRow;
						if (!multipleItems) {
							aliasRow = row;
						} else {
							aliasRow = new String[12];
						}
						//find matching product, get product number (use first element only because there shouldn't be duplicated ProductIDs)
						aliasRow[8] = (from product in db.Products where product.ProductID == item.ProductID select product.ProductNumber).ToArray()[0];
						aliasRow[9] = item.OrderQty.ToString();
						//apply discount, format as currency
						aliasRow[10] = string.Format("{0:C}", (item.OrderQty * (item.UnitPrice - (item.UnitPrice * item.UnitPriceDiscount)))); 
						aliasRow[11] = string.Format("{0:C}",item.LineTotal);

						table.Add(aliasRow);
						multipleItems = true; //multiple items on one invoice has special printing rules
						records++;
						if (recordLimit > 0 && records >= recordLimit) {
							break;
						}
					}
					if (recordLimit > 0 && records >= recordLimit) {
						break;
					}
				}//outer for
			}//using
			db.Dispose();
			return table;
		}//Method

	}//class
}//namespace
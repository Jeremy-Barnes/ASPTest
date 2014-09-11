using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using jabapp.EntityFramework;
using System.Collections;
using System.Diagnostics;

namespace jabapp {
	public partial class Site : System.Web.UI.Page {

		private ArrayList queryTable;

		protected void Page_Load(object sender, EventArgs e) {
			if (!Page.IsPostBack) {
				//assume user is interested in last month's sales
				DateTime[] defaults = generateDefaultDates(System.DateTime.Now.AddMonths(-1).AddYears(-6));
				DateTime defaultStart = defaults[0];
				DateTime defaultEnd = defaults[1];
				CalDateSelector.VisibleDate = defaultStart;
				CalDateSelector.SelectedDate = defaultStart;
				CalDateSelector.SelectedDates.Add(defaultEnd);
				TextBoxStartDate.Text = defaultStart.ToShortDateString();
				TextBoxEndDate.Text = defaultEnd.ToShortDateString();
			} else {
				if (CalDateSelector.SelectedDates.Count == 2) {
					DateTime date = CalDateSelector.SelectedDates[0];
					DateTime date2 = CalDateSelector.SelectedDates[1];

					if (date.CompareTo(date2) == -1) {
						queryTable = getSalesBetweenDates(date, date2);
					} else {
						queryTable = getSalesBetweenDates(date2, date);
					}
				}
			}//postback
		}//method

		protected void CalDateSelector_SelectionChanged(object sender, EventArgs e) {
			if (TextBoxStartDate.Text.Length > 1 && TextBoxEndDate.Text.Length > 1) {
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

			for (int i = 0; i < 15; i++) {
				TableRow row = new TableRow();
				if(i > queryTable.Count){
					break;
				} else {
					foreach (String cellString in (String[])queryTable[i]) {
						TableCell cell = new TableCell();
						cell.Text = cellString;
						row.Cells.Add(cell);
					}
					OutputTable.Rows.Add(row);	
				}
			}
		}

		protected void ButtonExport_Click(object sender, EventArgs e) {

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
		/// </summary>
		public static ArrayList getSalesBetweenDates(DateTime start, DateTime end) {
			AdventureWorksDB db = new AdventureWorksDB();
			ArrayList table = new ArrayList();
			String[] headers = {"Sold At", "Sold To", "Account Number", "Invoice #", "Customer PO #", 
								   "Order Date", "Due Date", "Invoice Total", "Product Number", "Order Qty", "Unit Net", "Line Total"};
			table.Add(headers);
			using (db) {

				IQueryable<SalesOrderHeader> salesQuery = from purchase in db.SalesOrderHeaders
															 where purchase.DueDate.CompareTo(start) >= 0 && purchase.DueDate.CompareTo(end) <= 0
															 select purchase;
				//O(n^2) isn't the best, but it beats further database trawling.
				foreach (SalesOrderHeader purchase in salesQuery) {
					String[] row = new String[12];

					if (purchase.OnlineOrderFlag) {
						row[0] = "Online";
					} else {
						row[0] = purchase.Customer.Store.Name;
					}
					row[1] = purchase.Customer.Person.FirstName + " " + purchase.Customer.Person.LastName;
					row[2] = purchase.AccountNumber;
					row[3] = purchase.SalesOrderID.ToString(); //TODO - find the invoice number, I think this is the wrong field
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
						aliasRow[8] = item.ProductID.ToString(); //TODO - find the product number, this is the wrong field
						aliasRow[9] = item.OrderQty.ToString();
						//apply discount, format as currency
						aliasRow[10] = string.Format("{0:C}", (item.OrderQty * (item.UnitPrice - (item.UnitPrice * item.UnitPriceDiscount)))); 
						aliasRow[11] = string.Format("{0:C}",item.LineTotal); //11 and 10 must match

						table.Add(aliasRow);

						multipleItems = true;
					}
				}//outer for
			}//using
			db.Dispose();
			return table;
		}//Method



	}//class
}//namespace
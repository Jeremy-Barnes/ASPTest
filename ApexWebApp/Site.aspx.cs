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
		


		protected void Page_Load(object sender, EventArgs e) {
			if (!Page.IsPostBack) {
				//assume user is interested in last month's sales
				DateTime[] defaults = generateDefaultDates(System.DateTime.Now.AddMonths(-1));
				DateTime defaultStart = defaults[0];
				DateTime defaultEnd = defaults[1];
				setSelectedDates(defaultStart, defaultEnd);
				//getSalesBetweenDates(defaultStart, defaultEnd);
			}
		}

		private void setSelectedDates(DateTime start, DateTime end) {
			CalDateSelector.VisibleDate = start;
			CalDateSelector.SelectedDate = start;
			CalDateSelector.SelectedDates.Add(end);
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
					row[5] = purchase.OrderDate.ToString();
					row[6] = purchase.DueDate.ToString();
					row[7] = purchase.TotalDue.ToString();

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
						aliasRow[10] = (item.OrderQty * item.UnitPrice * item.UnitPriceDiscount).ToString();
						aliasRow[11] = item.LineTotal.ToString(); //11 and 10 must match

						table.Add(aliasRow);

						multipleItems = true;
					}
					Debug.Print("sdfsd");
				}//outer for
			}//using
			db.Dispose();
			return table;
		}//method
	}//class
}//namespace
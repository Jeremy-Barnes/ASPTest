using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jabapp {
	public class Column : SpreadsheetBuilder.ColumnTemplate<SalesRecord>{}

	public class SalesRecord {
		public string SoldAt { get; set;}
		public string SoldTo { get; set;}
		public string AcctNo { get; set;}
		public string InvoiceNo { get; set;}
		public string CustomerPONo { get; set;}
		public string OrderDate { get; set;}
		public string DueDate { get; set;}
		public string InvoiceTotal { get; set;}
		public string ProductNo { get; set;}
		public string OrderQTY { get; set;}
		public string UnitNet { get; set;}
		public string LineTotal { get; set;}
	}
}
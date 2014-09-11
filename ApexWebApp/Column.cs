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

		private int iteratorIndex = -1;

		public string iterateProp(){
			iteratorIndex = (iteratorIndex + 1) % 12; //never go off the end, wrap around
			switch(iteratorIndex){
				case 0: return SoldAt;
				case 1: return SoldTo;
				case 2: return AcctNo;
				case 3: return InvoiceNo;
				case 4: return CustomerPONo;
				case 5: return OrderDate;
				case 6: return DueDate;
				case 7: return InvoiceTotal;
				case 8: return ProductNo;
				case 9: return OrderQTY;
				case 10: return UnitNet;
				case 11: return LineTotal;
				default: return "Iterator Error";
			}
		}

		public string getProp(String i) {
			switch (i) {
				case "Sold At":
					return SoldAt;
				case "Sold To":
					return SoldTo;
				case "Account Number":
					return AcctNo;
				case "Invoice #":
					return InvoiceNo;
				case "Customer PO #":
					return CustomerPONo;
				case "Order Date":
					return OrderDate;
				case "Due Date":
					return DueDate;
				case "Invoice Total":
					return InvoiceTotal;
				case "Product Number":
					return ProductNo;
				case "Order Qty":
					return OrderQTY;
				case "Unit Net":
					return UnitNet;
				case "Line Total":
					return LineTotal;
				default:
					return i;
			}
		}

		
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class MISReptFun
    {
                 public static CashSummary GetCashSummary(String Str)
        {
            CashSummary nn = new CashSummary();
            DataSet ds = MainFunction.SDataSet(Str, "tbl");
            int sno = 1;
            List<CashSummaryDetail> cls = new List<CashSummaryDetail>();
            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                CashSummaryDetail cs = new CashSummaryDetail();
                cs.Sno = sno;
                cs.Receipt = rr["Prefix"].ToString() + rr["CashBillno"].ToString();
                cs.Amount = (decimal)rr["NetAmount"];
                cs.PIN = rr["PinNo"].ToString();
                cs.Time = rr["datetime"].ToString();
                cs.User = rr["Userid"].ToString();
                cls.Add(cs);
                sno += 1;
            }
            nn.CashDtl = cls;
            return nn;
        }
                 public static List<CashIssueDetail> GetCashIssueDetails(string Str)
        {
            List<CashIssueDetail> ll = new List<CashIssueDetail>();
            try
            {
                DataSet ds = MainFunction.SDataSet(Str, "Tb");
                int count = 1;
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    CashIssueDetail nn = new CashIssueDetail();
                    nn.Sno = count;
                    nn.OrderNo = rr["order1"].ToString();
                    nn.OrderDate = MainFunction.DateFormat(rr["datetime1"].ToString(), "dd", "MM", "yyyy");
                    nn.Item = rr["description"].ToString();
                    nn.Company = rr["code"].ToString();
                    nn.Price = (decimal)rr["price"];
                    nn.QTY = (decimal)rr["quantity"];
                    nn.UOM = rr["unit"].ToString();
                    nn.Station = rr["Stationname"].ToString();


                    ll.Add(nn);
                    count += 1;
                }

                return ll;
            }
            catch (Exception e)
            {
                return ll;
            }


        }
                 public static PhOperatorWiseDispensesHeader PHWiseDispenses(string Str, int ReportType, string gIcode)
        {
            PhOperatorWiseDispensesHeader ll = new PhOperatorWiseDispensesHeader();
            try
            {
                List<PhOperatorWiseDispenses> tbl = new List<PhOperatorWiseDispenses>();
                int Sno = 1;
                long GrandTotal = 0;
                long GrandCashCount = 0;
                long GrandCompanyCount = 0;
                DataSet ds = MainFunction.SDataSet(Str, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    PhOperatorWiseDispenses yy = new PhOperatorWiseDispenses();
                    yy.SNO = Sno;
                    switch (ReportType)
                    {
                        case 1:
                            {
                                yy.PEmpoyeeID = rr["PEMployeeID"].ToString()
                                    + "    -" + rr["FIRSTNAME"].ToString()
                                    + " " + rr["MIDDLENAME"].ToString()
                                    + " " + rr["LASTNAME"].ToString();
                                yy.CompanyCount = (long)(int)rr["COMPANY"];
                                yy.CashCount = (long)(int)rr["CASH"];
                                yy.Total = yy.CompanyCount + yy.CashCount;
                                GrandCashCount += yy.CashCount;
                                GrandCompanyCount += yy.CompanyCount;
                                GrandTotal += yy.Total;
                                break;
                            }
                        case 2:
                            {
                                yy.PEmpoyeeID = rr["PEMployeeID"].ToString()
                                  + "    -" + rr["FIRSTNAME"].ToString()
                                  + " " + rr["MIDDLENAME"].ToString()
                                  + " " + rr["LASTNAME"].ToString();
                                yy.CompanyCount = (long)(int)rr["COMPANY"];
                                yy.CashCount = (long)(int)rr["CASH"];
                                yy.Total = yy.CompanyCount + yy.CashCount;
                                GrandCashCount += yy.CashCount;
                                GrandCompanyCount += yy.CompanyCount;
                                GrandTotal += yy.Total;
                                break;
                            }
                        case 3:
                            {
                                yy.prefix = rr["Prefix"].ToString() + "-" + rr["StationSLNo"].ToString();
                                yy.PEmpoyeeID = rr["PEmployeeID"].ToString();
                                yy.EmpCode = rr["EmpCode"].ToString();
                                yy.OEmployeeID = rr["OEmployeeID"].ToString();
                                DateTime n = (DateTime)rr["DispatchedDateTime"];
                                yy.DateTime = n.ToString("dd MMM yyyy HH:mm");
                                yy.Company = rr["Company"].ToString();
                                yy.PinNo = MainFunction.getUHID(gIcode, rr["RegistrationNO"].ToString(), true);
                                yy.ItemCode = rr["ItemCode"].ToString();
                                yy.Quatnity = (decimal)rr["DispatchQuantity"];
                                yy.Quatnity = Math.Round(yy.Quatnity, 2);
                                break;
                            }

                        case 4:
                            {
                                yy.prefix = rr["Prefix"].ToString() + "-" + rr["BillNo"].ToString();
                                yy.PEmpoyeeID = rr["PEmployeeID"].ToString();
                                yy.EmpCode = rr["EmpCode"].ToString();
                                yy.OEmployeeID = rr["OEmployeeID"].ToString();
                                DateTime n = (DateTime)rr["DateTime"];
                                yy.DateTime = n.ToString("dd MMM yyyy HH:mm");
                                yy.Company = rr["Company"].ToString();
                                yy.PinNo = MainFunction.getUHID(gIcode, rr["RegNO"].ToString(), true);
                                yy.ItemCode = rr["ItemCode"].ToString();
                                yy.Quatnity = (decimal)rr["Quantity"];
                                yy.Quatnity = Math.Round(yy.Quatnity, 2);
                                break;
                            }
                    }




                    tbl.Add(yy);
                    Sno += 1;
                }
                ll.Datalist = tbl;
                ll.GrandCashCount = GrandCashCount;
                ll.GrandCompanyCount = GrandCompanyCount;
                ll.GrandTotal = GrandTotal;
                return ll;
            }
            catch (Exception ex)
            {
                return ll;

            }


        }
                 public static ItemWiseIssuesDetailsHeader ItemWiseIssuesDetailsFun(string Str)
        {
            ItemWiseIssuesDetailsHeader ll = new ItemWiseIssuesDetailsHeader();
            try
            {
                List<ItemWiseIssuesDetailsDetail> tbl = new List<ItemWiseIssuesDetailsDetail>();
                DataSet ds = MainFunction.SDataSet(Str, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ItemWiseIssuesDetailsDetail yy = new ItemWiseIssuesDetailsDetail();
                                         if ((bool)rr["cancelyesno"] == false)
                    {
                        yy.Name = rr["name"].ToString();
                        if ((int)rr["billtype"] == 1)
                        {
                            yy.BillNo = rr["cashbillno"].ToString();
                        }
                        else
                        {
                            yy.BillNo = rr["creditbillno"].ToString();
                        }
                    }
                    else
                    {
                        yy.Name = rr["name"].ToString() + "** Caneclled Bill  **";
                        if ((int)rr["billtype"] == 1)
                        {
                            yy.BillNo = rr["cashbillno"].ToString();
                        }
                        else
                        {
                            yy.BillNo = rr["creditbillno"].ToString();
                        }
                    }

                    yy.PinNo = rr["pinno"].ToString();
                    DateTime n = (DateTime)rr["datetime"];
                    yy.Date = n.ToString("dd MMM yyyy");
                    yy.Quantity = (decimal)rr["quantity"];

                    tbl.Add(yy);
                }
                ll.ListDetail = tbl;
                return ll;
            }
            catch (Exception ex)
            {
                return ll;

            }


        }
        
        public static ItemLedgerHeader ItemLedger(DateTime dtpFromDt, DateTime dtpToDt, string ItemCode, string gStationname, int Gstationid)
        {
            ItemLedgerHeader ll = new ItemLedgerHeader();
            try
            {
                int i = 0;
                                 string stName = "";
                int iId = 0;
                long clBal = 0;
                long bal1 = 0;
                double convbal = 0;
                int m = 0;
                int y = 0;
                int conversionfactor = 0;
                DateTime StDate;
                DateTime eDate;
                int ID;

                StDate = dtpFromDt;
                eDate = dtpToDt;
                long totalissues = 0;
                long totalreceipts = 0;
                string ItemName = "";
                string unitname = "";
                if (gStationname.Length > 2)
                {
                    stName = gStationname.Substring(1, 2);
                }
                else
                {
                    stName = gStationname;
                }
                long bal = 0;
                List<ItemLedgerDetail> fgReport = new List<ItemLedgerDetail>();
                ID = MainFunction.GetOneVal(" select id as ID from item where deleted=0 and itemcode='" + ItemCode.Trim() + "'", "ID");

                String StrSql = "select item.id,item.name as itemname,itemcode,p.name as unit,sum(quantity) as qoh, b.conversionqty "
                    + "  from item,itemstore b,batchstore,packing p where item.id=" + ID + " and p.id=b.unitid and "
                    + " Batchstore.itemid = Item.ID and b.itemid=item.id and b.stationid= batchstore.stationid And "
                    + " Batchstore.stationId = " + Gstationid + " group by b.conversionqty,itemcode,p.name,item.id,item.name";
                DataSet rsprint = MainFunction.SDataSet(StrSql, "tbl1");
                if (rsprint.Tables[0].Rows.Count == 0)
                {
                    ItemName = "";
                    clBal = 0;
                    bal1 = 0;
                    conversionfactor = 0;
                    unitname = "";
                    ItemCode = "";
                    iId = 0;
                }
                else
                {
                    foreach (DataRow rr in rsprint.Tables[0].Rows)
                    {
                        ItemName = (string)rr["itemname"];
                        clBal = (long)(int)rr["qoh"];
                        bal1 = clBal;
                        conversionfactor = (int)rr["conversionqty"];
                        unitname = (string)rr["unit"];
                        ItemCode = (string)rr["itemcode"];
                        iId = (int)rr["id"];
                    }
                }

                if (DateTime.Now.Month != dtpToDt.Month)
                {
                    m = DateTime.Now.Month;
                    y = DateTime.Now.Year;
                    if (y > dtpToDt.Year)
                    {
                        while (y > dtpToDt.Year)
                        {
                            while (m > dtpToDt.Month)
                            {
                                StrSql = "Select Sum(b.Quantity) as qty, IssType from TransOrder_" + y + "_" + m
                                 + " a, TransOrderDetail_" + y + "_" + m + " b "
                                 + " Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Gstationid
                                 + " and a.datetime >= '" + dtpToDt.AddDays(1) + "' Group By IssType";
                                DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                                foreach (DataRow yy in temp1.Tables[0].Rows)
                                {
                                    if ((byte)yy["IssType"] == 0)
                                    {
                                        clBal -= (int)yy["qty"];
                                    }
                                    else
                                    {
                                        clBal += (int)yy["qty"];
                                    }
                                }
                                m = m - 1;
                            }
                            y = y - 1;
                        }
                    }
                    else if (y == dtpToDt.Year)
                    {
                        while (m > dtpToDt.Month)
                        {
                            StrSql = "Select Sum(b.Quantity) as qty, IssType from TransOrder_" + y + "_" + m
                               + " a, TransOrderDetail_" + y + "_" + m + " b "
                               + " Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Gstationid
                               + " and a.datetime >= '" + dtpToDt.AddDays(1) + "' Group By IssType";
                            DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                            foreach (DataRow yy in temp1.Tables[0].Rows)
                            {
                                if ((byte)yy["IssType"] == 0)
                                {
                                    clBal -= (int)yy["qty"];
                                }
                                else
                                {
                                    clBal += (int)yy["qty"];
                                }
                            }
                            m = m - 1;

                        }
                    }
                    if (dtpToDt.Day <= 31)
                    {

                        StrSql = "Select Sum(b.Quantity) as qty, IssType from TransOrder_" + y + "_" + m
                               + " a, TransOrderDetail_" + y + "_" + m + " b "
                               + "Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Gstationid
                               + " and a.DateTime > '" + dtpToDt.AddDays(1) + "'  Group By IssType";
                        DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                        foreach (DataRow yy in temp1.Tables[0].Rows)
                        {
                            if ((byte)yy["IssType"] == 0)
                            {
                                clBal -= (int)yy["qty"];
                            }
                            else
                            {
                                clBal += (int)yy["qty"];
                            }
                        }

                    }


                }
                else if (DateTime.Now.Month == dtpToDt.Month)
                {
                    StrSql = "Select Sum(b.Quantity) as qty, IssType from TransOrder_" + dtpToDt.Year + "_" + dtpToDt.Month
                            + " a, TransOrderDetail_" + dtpToDt.Year + "_" + dtpToDt.Month + " b "
                            + "Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Gstationid
                            + " and a.DateTime > '" + dtpFromDt + "' and a.datetime  >= '" + dtpToDt.AddDays(1) + "' Group By IssType";

                    DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow yy in temp1.Tables[0].Rows)
                    {
                        if ((byte)yy["IssType"] == 0)
                        {
                            clBal -= (int)yy["qty"];
                        }
                        else
                        {
                            clBal += (int)yy["qty"];
                        }
                    }

                }

                bal = clBal / conversionfactor;

                if (dtpToDt.Date == DateTime.Now.Date)
                {
                    bal = bal1 / conversionfactor;
                }

                while ((DateTime.Now - StDate).Days >= 0)
                {
                    if (StDate.Date == dtpFromDt.Date)
                    {
                        StrSql = "Select a.datetime,a.id,sum(b.quantity) as qty,j.conversionqty ,c.Description,a.IssType,s.name "
                             + " ,a.detailtype,a.stationslno,a.prefix,a.tostationid "
                             + " from TransOrder_" + StDate.Year + "_" + StDate.Month + " a,TransOrderDetail_" + StDate.Year + "_" + StDate.Month
                             + " b,TransactionType c,item i,ITEMSTORE J,station s "
                             + " where s.id=*a.tostationid and i.id=b.itemid and I.ID=J.ITEMID AND J.STATIONID= " + Gstationid
                             + " AND a.StationId=" + Gstationid + " and a.Idkey=b.idkey and b.itemid=" + ID
                             + " and a.DetailType=c.Id and a.DateTime>='" + dtpFromDt.Date
                             + "' and a.datetime < '" + dtpToDt.AddDays(1) + "' "
                             + " group by b.itemid,a.datetime,a.stationslno,a.id,j.conversionqty,c.description,a.isstype,s.name,"
                             + " a.detailtype,a.prefix,a.tostationid order by a.datetime";
                    }
                    else
                    {
                        StrSql = "Select a.datetime,a.id,sum(b.quantity) as qty,j.conversionqty ,c.Description,a.IssType,s.name,"
                            + " a.detailtype,a.stationslno,a.prefix,a.tostationid "
                            + " from  TransOrder_" + StDate.Year + "_" + StDate.Month + " a,TransOrderDetail_" + StDate.Year + "_" + StDate.Month
                            + " b,TransactionType c ,item i,station s,itemstore j  "
                            + " where s.id=*a.tostationid and i.id=b.itemid and i.id=j.itemid and j.stationid= " + Gstationid
                            + " and  a.StationId=" + Gstationid + " and a.Idkey=b.idkey and b.Itemid=" + ID
                            + " and a.DetailType=c.Id and a.datetime>= '" + dtpFromDt.Date
                            + "' and a.datetime < '" + dtpToDt.AddDays(1) + "' "
                            + " group by j.conversionqty,b.itemid,a.datetime,a.id,c.description,a.isstype,s.name, "
                            + " a.detailtype,a.stationslno,a.prefix,a.tostationid order by datetime";

                    }

                    DataSet Temp2 = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow rr in Temp2.Tables[0].Rows)
                    {
                        ItemLedgerDetail led = new ItemLedgerDetail();
                        if ((byte)rr["IssType"] == 1)
                        {
                            bal = bal + ((int)rr["qty"] / (int)rr["conversionqty"]);

                            totalissues = totalissues + ((int)rr["qty"] / (int)rr["conversionqty"]);
                            if ((Int16)rr["detailtype"] == 4)
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "Issued to - " + rr["name"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                led.Receipt = 0;
                            }
                            else if ((Int16)rr["detailtype"] == 12)
                            {
                                led.TransCode = rr["prefix"].ToString() + "-" + rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                led.Receipt = 0;
                            }
                            else if ((Int16)rr["detailtype"] == 2)
                            {
                                string DeptName = MainFunction.GetName("select deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "deptname");
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "Issued to - " + DeptName;
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                led.Receipt = 0;
                            }
                            else if ((Int16)rr["detailtype"] == 6)
                            {
                                DataSet Dumy = MainFunction.SDataSet("select cast(p.registrationno as varchar(20)) + '-' + isnull(s.prefix,s.name) as Ward from allinpatients p,drugorder d,station s "
                                    + " where d.stationid = s.id and p.ipid = d.ipid and d.id = " + (int)rr["id"], "tbl");
                                if (Dumy.Tables[0].Rows.Count == 0)
                                {
                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = rr["Description"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                    led.Issue = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                    led.Receipt = 0;
                                }
                                else
                                {
                                    foreach (DataRow xx in Dumy.Tables[0].Rows)
                                    {
                                        led.TransCode = rr["stationslno"].ToString();
                                        led.Description = rr["Description"].ToString() + "-" + xx["Ward"].ToString();
                                        led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                        led.Issue = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                        led.Receipt = 0;
                                    }
                                }

                            }
                            else
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                led.Issue = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                led.Receipt = 0;
                            }
                        }
                        else
                        {
                            convbal = (double)((int)rr["qty"] / (int)rr["conversionqty"]);

                            bal = bal - (long)convbal;
                            totalreceipts = totalreceipts + ((int)rr["qty"] / (int)rr["conversionqty"]);
                            if ((Int16)rr["detailtype"] == 15)
                            {
                                DataSet rsdesc = MainFunction.SDataSet("select s.name,p.invno from supplier s,purchasereceipt p where  "
                                                   + " p.stationslno=" + rr["stationslno"].ToString() + " and p.stationid=" + Gstationid
                                                   + " and p.supplierid=s.id ", "tbl");
                                if (rsdesc.Tables[0].Rows.Count == 0)
                                {
                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = rr["Description"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                    led.Issue = 0;
                                    led.Receipt = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                }
                                else
                                {
                                    foreach (DataRow xx in rsdesc.Tables[0].Rows)
                                    {

                                        led.TransCode = rr["stationslno"].ToString();
                                        led.Description = "Recd from: " + xx["name"].ToString().Substring(1, 10) + "(Invoice: " + xx["invno"].ToString();
                                        led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                        led.Issue = 0;
                                        led.Receipt = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                    }
                                }
                            }
                            else if ((Int16)rr["detailtype"] == 3)
                            {
                                string DeptName = MainFunction.GetName("select deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "deptname");
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "Return From: " + DeptName;
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                            }
                            else if ((Int16)rr["detailtype"] == 17)
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "Return From: " + rr["name"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                            }
                            else if ((Int16)rr["detailtype"] == 7)
                            {
                                DataSet Dumy = MainFunction.SDataSet("select cast(p.registrationno as varchar(20)) + '-' + isnull(s.prefix,s.name) as Ward from allinpatients p,drugorder d,station s "
                                   + " where d.stationid = s.id and p.ipid = d.ipid and d.id = " + (int)rr["id"], "tbl");
                                if (Dumy.Tables[0].Rows.Count == 0)
                                {
                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = rr["Description"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                    led.Issue = 0;
                                    led.Receipt = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                }
                                else
                                {
                                    foreach (DataRow xx in Dumy.Tables[0].Rows)
                                    {
                                        led.TransCode = rr["stationslno"].ToString();
                                        led.Description = rr["Description"].ToString() + "-" + xx["Ward"].ToString();
                                        led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                        led.Issue = 0;
                                        led.Receipt = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                                    }
                                }

                            }
                            else
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = (long)((int)rr["qty"] / (int)rr["conversionqty"]);
                            }

                        }
                        fgReport.Add(led);
                    }
                    StDate = StDate.AddMonths(1);

                }

                List<ItemLedgerDetail> SortedList = fgReport.OrderBy(o => o.Date).ToList();
                List<ItemLedgerDetail> finallist = new List<ItemLedgerDetail>();
                ItemLedgerDetail OpeningBal = new ItemLedgerDetail();
                OpeningBal.TransCode = "OPENIING";
                OpeningBal.Description = "BALANCE";
                OpeningBal.Stock = (long)Math.Round((decimal)bal, 2);
                finallist.Add(OpeningBal);
                long CountStock = OpeningBal.Stock;
                foreach (ItemLedgerDetail nx in SortedList)
                {
                    nx.Stock = CountStock - nx.Issue + nx.Receipt;
                    finallist.Add(nx);
                    CountStock = nx.Stock;
                }

                bal = 0;
                ItemLedgerDetail CloseingBal = new ItemLedgerDetail();
                CloseingBal.TransCode = "CLOSING";
                CloseingBal.Description = "BALANCE";
                CloseingBal.Stock = (long)Math.Round((decimal)CountStock, 2);
                finallist.Add(CloseingBal);

                ll.TransDetail = finallist;
                ll.TotalIssues = (int)totalissues;
                ll.TotalReceipt = (int)totalreceipts;
                ll.Units = unitname;
                ll.Station = gStationname;
                ll.ItemCode = ItemCode.ToUpper();
                ll.Title = "LEDGER SHEET FROM " + dtpFromDt.Date.ToString("dd-MM-yyyy") + " TO " + dtpToDt.Date.ToString("dd-MM-yyyy");
                ll.ItemName = MainFunction.GetName("select substring(name,1,30) as name from item where itemcode='" + ItemCode.Trim() + "'", "name");
                                 if (ll.TransDetail == null)
                {
                    ll.ErrMsg = "Details not found";
                }
                return ll;
            }
            catch (Exception e)
            {
                ll.ErrMsg = "Details not found";
                return ll;
            }
        }
        
        public static ItemLedgerPriceHeader ItemLedgerPrice(DateTime dtpFromDt, DateTime dtpToDt, string ItemCode,
            string gStationname, int Gstationid, int CatID, int RadioID, string RadioText)
        {
            ItemLedgerPriceHeader ll = new ItemLedgerPriceHeader();
            ll.FromDate = dtpFromDt;
            ll.ToDate = dtpToDt;
            ll.ItemCode = ItemCode;
            ll.gStationname = gStationname;
            ll.Gstationid = Gstationid;
            ll.CatID = CatID;
            ll.RadioID = RadioID;
            ll.RadioText = RadioText;
            try
            {

                if (RadioID == 0)                  {
                    int ItemID = MainFunction.GetOneVal("select ID from Item where Itemcode='" + ItemCode + "'", "ID");
                    ll.ItemID = ItemID;
                    ll = PrvReport(ll);
                }
                else if (RadioID == 1)                  {
                    string ss = "select id,datetime from DepartmentalIssues where stationslno = " + RadioText.Trim()
                        + " and stationid = " + Gstationid;
                    DataSet Main = MainFunction.SDataSet(ss, "tb");
                    if (Main.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow rr in Main.Tables[0].Rows)
                        {
                            dtpFromDt = (DateTime)rr["datetime"];
                            dtpToDt = (DateTime)rr["datetime"];
                            ss = "select distinct b.itemid,b.idkey from TransOrder_" + dtpFromDt.Year + "_" + dtpFromDt.Month + " a,"
                            + " Transorderdetail_" + dtpFromDt.Year + "_" + dtpFromDt.Month
                            + " b where a.idkey = b.idkey and a.id = " + (int)rr["id"] + " and a.detailtype = 2 and a.stationid = " + Gstationid;
                            DataSet temp = MainFunction.SDataSet(ss, "tb");
                            foreach (DataRow yy in temp.Tables[0].Rows)
                            {
                                ll.FromDate = dtpFromDt;
                                ll.ToDate = dtpToDt;
                                ll.ItemID = (int)yy["itemid"];
                                ll.Idkey = (int)yy["idkey"];
                                ll = PrvReport(ll);
                            }
                        }
                    }
                    else
                    {
                        ll.ErrMsg = "Issue No. Does Not Exist..";
                        return ll;
                    }
                }
                else if (RadioID == 2)                  {
                    string ss = "select id,datetime from IndentIssue where stationslno = " + RadioText.Trim() + " and sourceid = " + Gstationid;
                    DataSet Main = MainFunction.SDataSet(ss, "tb");
                    if (Main.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow rr in Main.Tables[0].Rows)
                        {
                            dtpFromDt = (DateTime)rr["datetime"];
                            dtpToDt = (DateTime)rr["datetime"];
                            ss = "select distinct b.itemid,b.idkey from TransOrder_" + dtpFromDt.Year + "_" + dtpFromDt.Month + " a,"
                            + " Transorderdetail_" + dtpFromDt.Year + "_" + dtpFromDt.Month
                            + " b where a.idkey = b.idkey and a.id = " + (int)rr["id"] + " and a.detailtype = 4 and a.stationid = " + Gstationid;
                            DataSet temp = MainFunction.SDataSet(ss, "tb");
                            foreach (DataRow yy in temp.Tables[0].Rows)
                            {
                                ll.FromDate = dtpFromDt;
                                ll.ToDate = dtpToDt;
                                ll.ItemID = (int)yy["itemid"];
                                ll.Idkey = (int)yy["idkey"];
                                ll = PrvReport(ll);
                            }
                        }
                    }
                    else
                    {
                        ll.ErrMsg = "Issue No. Does Not Exist..";
                        return ll;
                    }
                }
                else if (RadioID == 3)                  {
                    string ss = "select d.id,d.dispatcheddatetime as datetime from DrugOrder d,AllInPatients p "
                        + " where d.ipid = p.ipid and d.dispatched >= 2 and d.tostationid = " + Gstationid
                        + " and p.registrationno = " + RadioText;
                    DataSet Pin = MainFunction.SDataSet(ss, "tbl");
                    foreach (DataRow rr in Pin.Tables[0].Rows)
                    {
                        dtpFromDt = (DateTime)rr["datetime"];
                        dtpToDt = (DateTime)rr["datetime"];
                        ss = "select distinct b.itemid,b.idkey from TransOrder_" + dtpFromDt.Year + "_" + dtpFromDt.Month + " a,"
                        + " Transorderdetail_" + dtpFromDt.Year + "_" + dtpFromDt.Month
                        + " b where a.idkey = b.idkey and a.id = " + (int)rr["id"] + " and a.detailtype = 6 and a.stationid = " + Gstationid;
                        DataSet temp = MainFunction.SDataSet(ss, "tb");
                        foreach (DataRow yy in temp.Tables[0].Rows)
                        {
                            ll.FromDate = dtpFromDt;
                            ll.ToDate = dtpToDt;
                            ll.ItemID = (int)yy["itemid"];
                            ll.Idkey = (int)yy["idkey"];
                            ll = PrvReport(ll);
                        }
                    }

                }

                ll.Title = " LEDGER SHEET FROM " + dtpFromDt.ToString("dd-MM-yyyy") + " TO " + dtpToDt.ToString("dd-MM-yyyy");
                ll.ItemCode = ll.TransDetail[0].ItemCode.ToUpper();
                if (ll.TransDetail[0].ItemName.Length > 40)
                {
                    ll.ItemName = ll.TransDetail[0].ItemName.Substring(1, 40);
                }
                else
                {
                    ll.ItemName = ll.TransDetail[0].ItemName;
                }
                ll.Units = ll.TransDetail[0].Units;
                ll.Category = ll.TransDetail[0].Category;
                ll.TotalIssues = ll.TransDetail[0].TotalIssues;
                ll.TotalReceipt = ll.TransDetail[0].TotalReceipt;
                ll.Station = gStationname;
                
                List<ItemLedgerPriceSummary> smli = new List<ItemLedgerPriceSummary>();
                foreach (var xx in ll.TransDetail)
                {
                    bool bAvailable = false;

                    if (xx.DeptID > 0)
                    {
                        foreach (var yy in smli)
                        {
                            if (yy.DeptID == xx.DeptID)
                            {
                                yy.Quantity += xx.Issue;
                                yy.Quantity = Math.Round(yy.Quantity, 2);
                                yy.Amount += xx.Amount;
                                yy.Amount = Math.Round(yy.Amount, 2);
                                bAvailable = true;
                                break;
                            }

                        }
                        if (bAvailable == false)
                        {
                            ItemLedgerPriceSummary tt = new ItemLedgerPriceSummary();
                            tt.DeptID = xx.DeptID;
                            tt.Department = xx.DeptName;
                            tt.Quantity = xx.Issue;
                            tt.Quantity = Math.Round(tt.Quantity, 2);
                            tt.Amount = xx.Amount;
                            tt.Amount = Math.Round(tt.Amount, 2);
                            smli.Add(tt);
                        }

                    }


                }
                ll.TransSummary = smli;
                if (ll.TransDetail == null)
                {

                    ll.ErrMsg = "Details not found";
                }

                return ll;
            }
            catch (Exception e)
            {
                ll.ErrMsg = "Details not found";
                return ll;

            }
        }
        public static ItemLedgerPriceHeader PrvReport(ItemLedgerPriceHeader Mainx)
        {
            ItemLedgerPriceHeader Headerx = new ItemLedgerPriceHeader();
            try
            {
                if (Mainx.Idkey > 0)
                {
                    Headerx.TransDetail = PrvRepIssuanceNo(Mainx);
                }
                else
                {
                    Headerx.TransDetail = LedgerNew(Mainx);
                }

                return Headerx;
            }
            catch (Exception e) { return Headerx; }

        }
        public static List<ItemLedgerPriceDetail> PrvRepIssuanceNo(ItemLedgerPriceHeader Mainx)
        {
            List<ItemLedgerPriceDetail> ll = new List<ItemLedgerPriceDetail>();
            try
            {

                int i = 0;
                                 decimal CostPrice = 0;
                string stName = "";
                int iId = 0;
                decimal clBal = 0;
                decimal ClVal = 0;
                decimal BalVal = 0;
                decimal iqty = 0;
                int m = 0;
                int y = 0;
                int conversionfactor = 0;
                DateTime StDate;
                DateTime eDate;
                int ID = 0;

                StDate = Mainx.FromDate;
                eDate = Mainx.ToDate;
                decimal totalissues = 0;
                decimal totalreceipts = 0;
                string ItemName = "";
                string unitname = "";

                string lblname = "";
                string CatName = "";
                string ItemCode = "";
                stName = Mainx.gStationname.Substring(1, 2);
                decimal bal = 0;
                List<ItemLedgerPriceDetail> fgReport = new List<ItemLedgerPriceDetail>();
                decimal TotalIssueValue = 0;
                decimal TotalReceiptValue = 0;

                DataSet Fi = MainFunction.SDataSet(" select i.id,i.name,g.name as category from "
                    + " item i,itemgroup g where i.deleted=0 and i.categoryid = g.id and i.id = " + Mainx.ItemID, "tbl");
                foreach (DataRow rr in Fi.Tables[0].Rows)
                {
                    ID = (int)rr["id"];
                    lblname = rr["name"].ToString();
                    CatName = rr["category"].ToString();
                }

                DataSet BatchCost = MainFunction.SDataSet(" select costprice from batch  where itemid = " + ID + " order by startdate asc ", "tbl");
                foreach (DataRow rr in BatchCost.Tables[0].Rows)
                {
                    CostPrice = (decimal)rr["costprice"]; break;
                }

                String StrSql = "select item.id,item.name as itemname,itemcode,p.name as unit,sum(bt.quantity) as qoh,"
                    + " sum(bt.quantity * ba.Costprice) as ClValue,b.conversionqty "
                    + " from item,itemstore b,batchstore bt,batch ba,packing p where item.id=" + ID
                    + " and p.id=b.unitid and bt.itemid = Item.ID and b.itemid=item.id and "
                    + " b.stationid= bt.stationid And bt.batchid = ba.batchid and bt.itemid = ba.itemid "
                    + " and bt.stationId = " + Mainx.Gstationid
                    + " group by b.conversionqty,itemcode,p.name,item.id,item.name";

                DataSet rsprint = MainFunction.SDataSet(StrSql, "tbl1");

                foreach (DataRow rr in rsprint.Tables[0].Rows)
                {
                    ItemName = (string)rr["itemname"];
                    clBal = (decimal)(int)rr["qoh"];
                    conversionfactor = (int)rr["conversionqty"];
                    ClVal = (decimal)rr["ClValue"];
                    unitname = (string)rr["unit"];
                    ItemCode = (string)rr["itemcode"];
                    iId = (int)rr["id"];
                    CostPrice = CostPrice * conversionfactor;
                }


                if (DateTime.Now.Month != Mainx.ToDate.Month)
                {
                    m = DateTime.Now.Month;
                    y = DateTime.Now.Year;
                    if (y > Mainx.ToDate.Year)
                    {
                        while (y > Mainx.ToDate.Year)
                        {
                            while (m > Mainx.ToDate.Month)
                            {
                                StrSql = "Select Sum(b.Quantity) as qty, IssType,sum(b.quantity * isnull(bt.Costprice,0)) as Value "
                                 + " from TransOrder_" + y + "_" + m + " a, TransOrderDetail_" + y + "_" + m + " b ,batch bt"
                                 + " Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Mainx.Gstationid
                                 + " and b.batchid *= bt.batchid and b.itemid = bt.itemid "
                                 + " and a.datetime >= '" + Mainx.ToDate.AddDays(1) + "' Group By IssType";
                                DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                                foreach (DataRow yy in temp1.Tables[0].Rows)
                                {
                                    if ((byte)yy["IssType"] == 0)
                                    {
                                        clBal -= (decimal)(int)yy["qty"];
                                        ClVal -= (decimal)yy["Value"];
                                    }
                                    else
                                    {
                                        clBal += (decimal)(int)yy["qty"];
                                        ClVal += (decimal)yy["Value"];
                                    }
                                }
                                m = m - 1;
                            }
                            y = y - 1;
                        }
                    }
                    else if (y == Mainx.ToDate.Year)
                    {
                        while (m > Mainx.ToDate.Month)
                        {
                            StrSql = "Select Sum(b.Quantity) as qty, IssType , sum(b.quantity * isnull(bt.costprice,0)) as Value "
                                + " from TransOrder_" + y + "_" + m + " a, TransOrderDetail_" + y + "_" + m + " b,batch bt "
                               + " Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Mainx.Gstationid
                               + " and b.batchid *= bt.batchid and b.itemid = bt.itemid "
                               + " and a.datetime >= '" + Mainx.ToDate.AddDays(1) + "' Group By IssType";
                            DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                            foreach (DataRow yy in temp1.Tables[0].Rows)
                            {
                                if ((byte)yy["IssType"] == 0)
                                {
                                    clBal -= (decimal)(int)yy["qty"];
                                    ClVal -= (decimal)yy["Value"];
                                }
                                else
                                {
                                    clBal += (decimal)(int)yy["qty"];
                                    ClVal += (decimal)yy["Value"];
                                }
                            }
                            m = m - 1;
                        }
                    }
                    if (Mainx.ToDate.Day <= 31)
                    {

                        StrSql = "Select Sum(b.Quantity) as qty, IssType,sum(b.quantity * isnull(bt.costprice,0)) as Value from TransOrder_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month
                             + " a, TransOrderDetail_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month + " b,batch bt "
                             + "Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And bt.batchid = b.batchid And a.stationid = " + Mainx.Gstationid
                             + " and a.idkey > " + Mainx.Idkey + " Group By IssType";
                        DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                        foreach (DataRow yy in temp1.Tables[0].Rows)
                        {
                            if ((byte)yy["IssType"] == 0)
                            {
                                clBal -= (decimal)(int)yy["qty"];
                                ClVal -= (decimal)yy["Value"];
                            }
                            else
                            {
                                clBal += (decimal)(int)yy["qty"];
                                ClVal += (decimal)yy["Value"];
                            }
                        }

                    }


                }
                else if (DateTime.Now.Month == Mainx.ToDate.Month)
                {
                    StrSql = "Select Sum(b.Quantity) as qty, IssType,sum(b.quantity * isnull(bt.costprice,0)) as Value from TransOrder_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month
                            + " a, TransOrderDetail_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month + " b,batch bt "
                            + "Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And bt.batchid = b.batchid And a.stationid = " + Mainx.Gstationid
                            + " and a.idkey > " + Mainx.Idkey + " Group By IssType";

                    DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow yy in temp1.Tables[0].Rows)
                    {
                        if ((byte)yy["IssType"] == 0)
                        {
                            clBal -= (decimal)(int)yy["qty"];
                            ClVal -= (decimal)yy["Value"];
                        }
                        else
                        {
                            clBal += (decimal)(int)yy["qty"];
                            ClVal += (decimal)yy["Value"];
                        }
                    }

                }

                bal = clBal / conversionfactor;
                BalVal = ClVal;

                while ((DateTime.Now - StDate).Days >= 0)
                {

                    StrSql = "Select a.datetime,a.id,sum(b.quantity) as qty,j.conversionqty ,c.Description,a.IssType,s.name "
                         + " ,a.detailtype,a.stationslno,a.prefix "
                         + " ,d.costprice,d.mrp,a.tostationid,isnull(CONVERT(CHAR(12),D.EXPIRYDATE,101),'') AS EXPIRYDATE,sum(b.quantity * d.costprice) as Value "
                         + " from TransOrder_" + StDate.Year + "_" + StDate.Month + " a,TransOrderDetail_" + StDate.Year + "_" + StDate.Month
                         + " b,TransactionType c,item i,ITEMSTORE J,station s,batch d  "
                         + " where d.itemid=i.id and d.batchid=b.batchid and s.id=*a.tostationid and i.id=b.itemid "
                         + " and I.ID=J.ITEMID AND J.STATIONID= " + Mainx.Gstationid + " AND a.StationId=" + Mainx.Gstationid
                         + " and a.Idkey=b.idkey and b.itemid=" + ID + " and a.DetailType=c.Id and a.idkey = " + Mainx.Idkey
                         + " group by b.itemid,a.datetime,a.stationslno,a.id,j.conversionqty,d.costprice,d.mrp,"
                         + " c.description,a.isstype,s.name,a.detailtype,a.prefix,a.tostationid,CONVERT(CHAR(12),D.EXPIRYDATE,101) order by a.datetime";


                    DataSet Temp2 = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow rr in Temp2.Tables[0].Rows)
                    {
                        decimal qty = (int)rr["qty"];
                        decimal Conv = (int)rr["conversionqty"];
                        string ExptDdate = "";
                        if (rr["EXPIRYDATE"].ToString() == null || rr["EXPIRYDATE"].ToString() == "")
                        {
                            ExptDdate = "";
                        }
                        else
                        {
                            ExptDdate = MainFunction.DateFormat(rr["EXPIRYDATE"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                        }
                        ItemLedgerPriceDetail led = new ItemLedgerPriceDetail();
                        if ((byte)rr["IssType"] == 1)
                        {
                                                         bal += qty / Conv;
                            BalVal += (decimal)rr["Value"];
                                                         totalissues = totalissues + qty / Conv;
                            if ((Int16)rr["detailtype"] == 4)
                            {
                                DataSet xx = MainFunction.SDataSet("select s.departmentid, deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "tbl");
                                foreach (DataRow xxx in xx.Tables[0].Rows)
                                {
                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = "ID - " + rr["name"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                    led.Issue = qty / Conv;
                                    led.Receipt = 0;
                                    led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.Price = Math.Round(led.Price, 2);
                                    led.Amount = (decimal)rr["Value"];
                                    led.Amount = Math.Round(led.Amount, 2);
                                    led.ExpDate = ExptDdate;
                                    led.DeptID = (int)xxx["departmentid"];
                                    led.DeptName = xxx["deptname"].ToString();
                                    led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.UnitCost = Math.Round(led.UnitCost, 2);
                                    led.IssValue = (decimal)rr["Value"];
                                    led.IssValue = Math.Round(led.IssValue, 2);
                                    TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                                }
                            }
                            else if ((Int16)rr["detailtype"] == 12)
                            {
                                led.TransCode = rr["prefix"].ToString() + "-" + rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = qty / Conv;
                                led.Receipt = 0;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                led.UnitCost = Math.Round(led.UnitCost, 2);
                                led.IssValue = (decimal)rr["Value"];
                                led.IssValue = Math.Round(led.IssValue, 2);
                                TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                            }
                            else if ((Int16)rr["detailtype"] == 2)
                            {
                                DataSet xx = MainFunction.SDataSet("select id,deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "tbl");
                                foreach (DataRow xxx in xx.Tables[0].Rows)
                                {
                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = "ID - " + xxx["deptname"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                    led.Issue = qty / Conv;
                                    led.Receipt = 0;
                                    led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.Price = Math.Round(led.Price, 2);
                                    led.Amount = (decimal)rr["Value"];
                                    led.Amount = Math.Round(led.Amount, 2);
                                    led.ExpDate = ExptDdate;
                                    led.DeptID = (int)xxx["id"];
                                    led.DeptName = xxx["deptname"].ToString();
                                    led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.UnitCost = Math.Round(led.UnitCost, 2);
                                    led.IssValue = (decimal)rr["Value"];
                                    led.IssValue = Math.Round(led.IssValue, 2);
                                    TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                                }

                            }
                            else
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = qty / Conv;
                                led.Receipt = 0;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                led.UnitCost = Math.Round(led.UnitCost, 2);
                                led.IssValue = (decimal)rr["Value"];
                                led.IssValue = Math.Round(led.IssValue, 2);
                                TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                            }
                        }
                        else
                        {
                                                         bal -= qty / Conv;
                            BalVal -= (decimal)rr["Value"];
                                                         totalreceipts = totalreceipts + qty / Conv;
                            if ((Int16)rr["detailtype"] == 15)
                            {
                                DataSet rsdesc = MainFunction.SDataSet(" select s.name,p.invno,pr.unitepr  "
                                    + " from supplier s,purchasereceipt p,purchasereceiptdetail pr where  "
                                    + " p.id = pr.receiptid and pr.itemid = " + ID
                                    + " and p.stationslno=" + rr["stationslno"].ToString()
                                        + " and p.stationid=" + Mainx.Gstationid + " and p.supplierid=s.id ", "tbl");

                                foreach (DataRow xx in rsdesc.Tables[0].Rows)
                                {

                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = "RD-" + xx["name"].ToString().Substring(1, 10) + "(Invoice: " + xx["invno"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                    led.Issue = 0;
                                    led.Receipt = qty / Conv;
                                    if ((decimal)xx["CostPrice"] == 0)
                                    {
                                        led.Price = (decimal)rr["unitepr"];
                                        led.Price = Math.Round(led.Price, 2);
                                    }
                                    else
                                    {
                                        led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                        led.Price = Math.Round(led.Price, 2);
                                    }
                                    led.Amount = (decimal)rr["Value"];
                                    led.Amount = Math.Round(led.Amount, 2);
                                    led.ExpDate = ExptDdate;
                                    led.UnitCost = led.Price;

                                    led.RecValue = (decimal)rr["Value"];
                                    led.RecValue = Math.Round(led.RecValue, 2);
                                    TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];

                                }

                            }
                            else if ((Int16)rr["detailtype"] == 3)
                            {
                                string DeptName = MainFunction.GetName("select deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "deptname");
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "RD-" + DeptName;
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];
                            }
                            else if ((Int16)rr["detailtype"] == 17)
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "RE-" + rr["name"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];
                            }
                            else if ((Int16)rr["detailtype"] == 5)
                            {
                                string slno = MainFunction.GetName("select a.stationslno from IndentIssue a where a.id =  " + (int)rr["id"], "deptname");
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString() + " (" + slno + ")";
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];

                            }
                            else
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];
                            }

                        }
                        fgReport.Add(led);
                    }
                    StDate = StDate.AddMonths(1);

                }

                List<ItemLedgerPriceDetail> SortedList = fgReport.OrderBy(o => o.Date).ToList();
                List<ItemLedgerPriceDetail> finallist = new List<ItemLedgerPriceDetail>();
                ItemLedgerPriceDetail OpeningBal = new ItemLedgerPriceDetail();
                OpeningBal.TransCode = "OPENIING";
                OpeningBal.Description = "BALANCE";
                if (bal == 0)
                {
                    OpeningBal.Stock = 0;
                }
                else
                {
                    OpeningBal.Stock = (decimal)Math.Round((decimal)bal, 2);
                    OpeningBal.Value = BalVal - OpeningBal.IssValue + OpeningBal.RecValue;
                    OpeningBal.Value = Math.Round(OpeningBal.Value, 2);
                    OpeningBal.UnitCost = OpeningBal.Value / OpeningBal.Stock;
                    OpeningBal.UnitCost = Math.Round(OpeningBal.UnitCost, 2);
                }
                finallist.Add(OpeningBal);
                decimal CountStock = OpeningBal.Stock;
                foreach (ItemLedgerPriceDetail nx in SortedList)
                {
                    nx.Stock = CountStock - (decimal)nx.Issue + (decimal)nx.Receipt;
                    nx.Stock = Math.Round(nx.Stock, 2);
                    nx.Value = BalVal - nx.IssValue + nx.RecValue;
                    nx.Value = Math.Round(nx.Value, 2);
                    nx.UnitCost = nx.Value / nx.Stock;
                    nx.UnitCost = Math.Round(nx.UnitCost, 2);
                    finallist.Add(nx);
                    CountStock = nx.Stock;
                    BalVal -= nx.IssValue + nx.RecValue;
                }
                bal = 0;
                ItemLedgerPriceDetail CloseingBal = new ItemLedgerPriceDetail();
                CloseingBal.TransCode = "CLOSING";
                CloseingBal.Description = "BALANCE";
                CloseingBal.Stock = (decimal)Math.Round((decimal)CountStock, 2);
                CloseingBal.Value = Math.Round(BalVal, 2);
                CloseingBal.UnitCost = CloseingBal.Value / CloseingBal.Stock;
                CloseingBal.UnitCost = Math.Round(CloseingBal.UnitCost, 2);
                finallist.Add(CloseingBal);

                ll = finallist;
                
                                 ll[0].ItemCode = ItemCode;
                ll[0].ItemName = ItemName;
                ll[0].Units = unitname;
                ll[0].Category = CatName;
                ll[0].TotalIssues = (int)(decimal)totalissues;
                ll[0].TotalReceipt = (int)(decimal)totalreceipts;
                return ll;
            }
            catch (Exception e) { return ll; }


        }
        public static List<ItemLedgerPriceDetail> LedgerNew(ItemLedgerPriceHeader Mainx)
        {
            List<ItemLedgerPriceDetail> ll = new List<ItemLedgerPriceDetail>();
            try
            {

                                 decimal CostPrice = 0;
                string stName = "";
                int iId = 0;
                decimal clBal = 0;
                decimal ClVal = 0;
                decimal BalVal = 0;
                decimal iqty = 0;
                int m = 0;
                int y = 0;
                int conversionfactor = 0;
                DateTime StDate;
                DateTime eDate;
                int ID = 0;

                StDate = Mainx.FromDate;
                eDate = Mainx.ToDate;
                decimal totalissues = 0;
                decimal totalreceipts = 0;
                string ItemName = "";
                string unitname = "";

                string lblname = "";
                string CatName = "";
                string ItemCode = "";
                stName = Mainx.gStationname.Substring(1, 2);
                decimal bal = 0;
                List<ItemLedgerPriceDetail> fgReport = new List<ItemLedgerPriceDetail>();
                decimal TotalIssueValue = 0;
                decimal TotalReceiptValue = 0;

                DataSet Fi = MainFunction.SDataSet(" select i.id,i.name,g.name as category from "
                    + " item i,itemgroup g where i.deleted=0 and i.categoryid = g.id and i.id = " + Mainx.ItemID, "tbl");
                foreach (DataRow rr in Fi.Tables[0].Rows)
                {
                    ID = (int)rr["id"];
                    lblname = rr["name"].ToString();
                    CatName = rr["category"].ToString();
                }

                DataSet BatchCost = MainFunction.SDataSet(" select costprice from batch  where itemid = " + ID + " order by startdate asc ", "tbl");
                foreach (DataRow rr in BatchCost.Tables[0].Rows)
                {
                    CostPrice = (decimal)rr["costprice"]; break;
                }

                String StrSql = "select item.id,item.name as itemname,itemcode,p.name as unit,sum(bt.quantity) as qoh,"
                    + " sum(bt.quantity * ba.Costprice) as ClValue,b.conversionqty "
                    + " from item,itemstore b,batchstore bt,batch ba,packing p where item.id=" + ID
                    + " and p.id=b.unitid and bt.itemid = Item.ID and b.itemid=item.id and "
                    + " b.stationid= bt.stationid And bt.batchid = ba.batchid and bt.itemid = ba.itemid "
                    + " and bt.stationId = " + Mainx.Gstationid
                    + " group by b.conversionqty,itemcode,p.name,item.id,item.name";

                DataSet rsprint = MainFunction.SDataSet(StrSql, "tbl1");

                foreach (DataRow rr in rsprint.Tables[0].Rows)
                {
                    ItemName = (string)rr["itemname"];
                    clBal = (decimal)(int)rr["qoh"];
                    conversionfactor = (int)rr["conversionqty"];
                    ClVal = (decimal)rr["ClValue"];
                    unitname = (string)rr["unit"];
                    ItemCode = (string)rr["itemcode"];
                    iId = (int)rr["id"];
                    CostPrice = CostPrice * conversionfactor;
                }


                if (DateTime.Now.Month != Mainx.ToDate.Month)
                {
                    m = DateTime.Now.Month;
                    y = DateTime.Now.Year;
                    if (y > Mainx.ToDate.Year)
                    {
                        while (y > Mainx.ToDate.Year)
                        {
                            while (m > Mainx.ToDate.Month)
                            {
                                StrSql = "Select Sum(b.Quantity) as qty, IssType,sum(b.quantity * isnull(bt.Costprice,0)) as Value "
                                 + " from TransOrder_" + y + "_" + m + " a, TransOrderDetail_" + y + "_" + m + " b ,batch bt"
                                 + " Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Mainx.Gstationid
                                 + " and b.batchid *= bt.batchid and b.itemid = bt.itemid "
                                 + " and a.datetime >= '" + Mainx.ToDate.AddDays(1) + "' Group By IssType";
                                DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                                foreach (DataRow yy in temp1.Tables[0].Rows)
                                {
                                    if ((byte)yy["IssType"] == 0)
                                    {
                                        clBal -= (decimal)(int)yy["qty"];
                                        ClVal -= (decimal)yy["Value"];
                                    }
                                    else
                                    {
                                        clBal += (decimal)(int)yy["qty"];
                                        ClVal += (decimal)yy["Value"];
                                    }
                                }
                                m = m - 1;
                            }
                            y = y - 1;
                        }
                    }
                    else if (y == Mainx.ToDate.Year)
                    {
                        while (m > Mainx.ToDate.Month)
                        {
                            StrSql = "Select Sum(b.Quantity) as qty, IssType , sum(b.quantity * isnull(bt.costprice,0)) as Value "
                                + " from TransOrder_" + y + "_" + m + " a, TransOrderDetail_" + y + "_" + m + " b,batch bt "
                               + " Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And a.stationid = " + Mainx.Gstationid
                               + " and b.batchid *= bt.batchid and b.itemid = bt.itemid "
                               + " and a.datetime >= '" + Mainx.ToDate.AddDays(1) + "' Group By IssType";
                            DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                            foreach (DataRow yy in temp1.Tables[0].Rows)
                            {
                                if ((byte)yy["IssType"] == 0)
                                {
                                    clBal -= (decimal)(int)yy["qty"];
                                    ClVal -= (decimal)yy["Value"];
                                }
                                else
                                {
                                    clBal += (decimal)(int)yy["qty"];
                                    ClVal += (decimal)yy["Value"];
                                }
                            }
                            m = m - 1;
                        }
                    }
                    if (Mainx.ToDate.Day <= 31)
                    {

                        StrSql = "Select Sum(b.Quantity) as qty, IssType,sum(b.quantity * isnull(bt.costprice,0)) as Value from TransOrder_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month
                             + " a, TransOrderDetail_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month + " b,batch bt "
                             + "Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And bt.batchid = b.batchid And a.stationid = " + Mainx.Gstationid
                             + " and a.idkey > " + Mainx.Idkey + " Group By IssType";
                        DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                        foreach (DataRow yy in temp1.Tables[0].Rows)
                        {
                            if ((byte)yy["IssType"] == 0)
                            {
                                clBal -= (decimal)(int)yy["qty"];
                                ClVal -= (decimal)yy["Value"];
                            }
                            else
                            {
                                clBal += (decimal)(int)yy["qty"];
                                ClVal += (decimal)yy["Value"];
                            }
                        }

                    }


                }
                else if (DateTime.Now.Month == Mainx.ToDate.Month)
                {
                    StrSql = "Select Sum(b.Quantity) as qty, IssType,sum(b.quantity * isnull(bt.costprice,0)) as Value from TransOrder_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month
                            + " a, TransOrderDetail_" + Mainx.ToDate.Year + "_" + Mainx.ToDate.Month + " b,batch bt "
                            + "Where a.IdKey = b.IdKey And b.ItemId = " + ID + " And bt.batchid = b.batchid And a.stationid = " + Mainx.Gstationid
                            + " and a.idkey > " + Mainx.Idkey + " Group By IssType";

                    DataSet temp1 = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow yy in temp1.Tables[0].Rows)
                    {
                        if ((byte)yy["IssType"] == 0)
                        {
                            clBal -= (decimal)(int)yy["qty"];
                            ClVal -= (decimal)yy["Value"];
                        }
                        else
                        {
                            clBal += (decimal)(int)yy["qty"];
                            ClVal += (decimal)yy["Value"];
                        }
                    }

                }

                bal = clBal / conversionfactor;
                BalVal = ClVal;

                while ((DateTime.Now - StDate).Days >= 0)
                {
                    if (StDate == eDate)
                    {
                        StrSql = " Select a.datetime,a.id,sum(b.quantity) as qty,j.conversionqty,c.Description,a.IssType,"
                            + " s.name,a.detailtype,a.stationslno,a.prefix,d.costprice,d.mrp,"
                            + " a.tostationid,CONVERT(CHAR(12),D.EXPIRYDATE,113) AS EXPIRYDATE,"
                            + " sum(b.quantity * d.costprice) as Value "
                            + " from TransOrder_" + StDate.Year + "_" + StDate.Month
                            + " a,TransOrderDetail_" + StDate.Year + "_" + StDate.Month + " b,TransactionType c "
                            + " ,item i,ITEMSTORE J,station s,batch d "
                            + " where d.itemid=i.id and d.batchid=b.batchid "
                            + " and s.id=*a.tostationid and i.id=b.itemid and I.ID=J.ITEMID AND J.STATIONID= " + Mainx.Gstationid
                            + " AND a.StationId=" + Mainx.Gstationid + " and a.Idkey=b.idkey and b.itemid=" + ID
                            + " and a.DetailType=c.Id and a.DateTime>='" + Mainx.FromDate
                            + "' and a.datetime < '" + Mainx.ToDate.AddDays(1) + "' "
                            + "  group by b.itemid,a.datetime,a.stationslno,a.id,j.conversionqty,d.costprice,d.mrp"
                            + " ,c.description,a.isstype,s.name,a.detailtype,a.prefix,a.tostationid,"
                            + " CONVERT(CHAR(12),D.EXPIRYDATE,113) order by a.datetime";
                    }
                    else
                    {
                        StrSql = " Select a.datetime,a.id,sum(b.quantity) as qty,j.conversionqty,c.Description,a.IssType, "
                            + " s.name,a.detailtype,a.stationslno,a.prefix,d.costprice,d.mrp,"
                            + " a.tostationid,CONVERT(CHAR(12),D.EXPIRYDATE,113) AS EXPIRYDATE,"
                            + " sum(b.quantity * d.costprice) as Value "
                            + " from  TransOrder_" + StDate.Year + "_" + StDate.Month
                            + " a,TransOrderDetail_" + StDate.Year + "_" + StDate.Month + " b,TransactionType c "
                            + " ,item i,station s,itemstore j,batch d "
                            + " where d.itemid=i.id and d.batchid=b.batchid "
                            + " and s.id=*a.tostationid and i.id=b.itemid and i.id=j.itemid and j.stationid= " + Mainx.Gstationid
                            + " and  a.StationId=" + Mainx.Gstationid + " and a.Idkey=b.idkey and b.Itemid=" + ID
                            + " and a.datetime < '" + Mainx.ToDate.AddDays(1) + "' and a.DetailType=c.Id "
                            + " group by j.conversionqty,b.itemid,a.datetime,a.id,c.description,a.isstype,s.name,"
                            + " a.detailtype,a.stationslno,a.prefix,d.costprice,d.mrp,a.tostationid, "
                            + " CONVERT(CHAR(12),D.EXPIRYDATE,113) order by datetime";

                    }

                    DataSet Temp2 = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow rr in Temp2.Tables[0].Rows)
                    {
                        decimal qty = (int)rr["qty"];
                        decimal Conv = (int)rr["conversionqty"];
                        string ExptDdate = "";
                        if (rr["EXPIRYDATE"].ToString() == null || rr["EXPIRYDATE"].ToString() == "")
                        {
                            ExptDdate = "";
                        }
                        else
                        {
                            ExptDdate = MainFunction.DateFormat(rr["EXPIRYDATE"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                        }
                        ItemLedgerPriceDetail led = new ItemLedgerPriceDetail();
                        if ((byte)rr["IssType"] == 1)
                        {
                                                         bal += qty / Conv;
                            BalVal += (decimal)rr["Value"];
                                                         totalissues = totalissues + qty / Conv;
                            if ((Int16)rr["detailtype"] == 4)
                            {
                                DataSet xx = MainFunction.SDataSet("select s.departmentid, deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "tbl");
                                foreach (DataRow xxx in xx.Tables[0].Rows)
                                {
                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = "ID - " + rr["name"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                    led.Issue = qty / Conv;
                                    led.Receipt = 0;
                                    led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.Price = Math.Round(led.Price, 2);
                                    led.Amount = (decimal)rr["Value"];
                                    led.Amount = Math.Round(led.Amount, 2);
                                    led.ExpDate = ExptDdate;
                                    led.DeptID = (int)xxx["departmentid"];
                                    led.DeptName = xxx["deptname"].ToString();
                                    led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.UnitCost = Math.Round(led.UnitCost, 2);
                                    led.IssValue = (decimal)rr["Value"];
                                    led.IssValue = Math.Round(led.IssValue, 2);
                                    TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                                }
                            }
                            else if ((Int16)rr["detailtype"] == 12)
                            {
                                led.TransCode = rr["prefix"].ToString() + "-" + rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = qty / Conv;
                                led.Receipt = 0;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                led.UnitCost = Math.Round(led.UnitCost, 2);
                                led.IssValue = (decimal)rr["Value"];
                                led.IssValue = Math.Round(led.IssValue, 2);
                                TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                            }
                            else if ((Int16)rr["detailtype"] == 2)
                            {
                                DataSet xx = MainFunction.SDataSet("select id,deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "tbl");
                                foreach (DataRow xxx in xx.Tables[0].Rows)
                                {
                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = "ID - " + xxx["deptname"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                    led.Issue = qty / Conv;
                                    led.Receipt = 0;
                                    led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.Price = Math.Round(led.Price, 2);
                                    led.Amount = (decimal)rr["Value"];
                                    led.Amount = Math.Round(led.Amount, 2);
                                    led.ExpDate = ExptDdate;
                                    led.DeptID = (int)xxx["id"];
                                    led.DeptName = xxx["deptname"].ToString();
                                    led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                    led.UnitCost = Math.Round(led.UnitCost, 2);
                                    led.IssValue = (decimal)rr["Value"];
                                    led.IssValue = Math.Round(led.IssValue, 2);
                                    TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                                }

                            }
                            else
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = qty / Conv;
                                led.Receipt = 0;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = (decimal)rr["CostPrice"] * conversionfactor;
                                led.UnitCost = Math.Round(led.UnitCost, 2);
                                led.IssValue = (decimal)rr["Value"];
                                led.IssValue = Math.Round(led.IssValue, 2);
                                TotalIssueValue = TotalIssueValue + (decimal)rr["Value"];
                            }
                        }
                        else
                        {
                                                         bal -= qty / Conv;
                            BalVal -= (decimal)rr["Value"];
                                                         totalreceipts = totalreceipts + qty / Conv;
                            if ((Int16)rr["detailtype"] == 15)
                            {
                                DataSet rsdesc = MainFunction.SDataSet(" select s.name,p.invno,pr.unitepr  "
                                    + " from supplier s,purchasereceipt p,purchasereceiptdetail pr where  "
                                    + " p.id = pr.receiptid and pr.itemid = " + ID
                                    + " and p.stationslno=" + rr["stationslno"].ToString()
                                        + " and p.stationid=" + Mainx.Gstationid + " and p.supplierid=s.id ", "tbl");

                                foreach (DataRow xx in rsdesc.Tables[0].Rows)
                                {

                                    led.TransCode = rr["stationslno"].ToString();
                                    led.Description = "RD-" + xx["name"].ToString().Substring(1, 10) + "(Invoice: " + xx["invno"].ToString();
                                    led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "HH", "MM", "", "/", "");
                                    led.Issue = 0;
                                    led.Receipt = qty / Conv;
                                    if ((decimal)xx["CostPrice"] == 0)
                                    {
                                        led.Price = (decimal)rr["unitepr"];
                                        led.Price = Math.Round(led.Price, 2);
                                    }
                                    else
                                    {
                                        led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                        led.Price = Math.Round(led.Price, 2);
                                    }
                                    led.Amount = (decimal)rr["Value"];
                                    led.Amount = Math.Round(led.Amount, 2);
                                    led.ExpDate = ExptDdate;
                                    led.UnitCost = led.Price;

                                    led.RecValue = (decimal)rr["Value"];
                                    led.RecValue = Math.Round(led.RecValue, 2);
                                    TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];

                                }

                            }
                            else if ((Int16)rr["detailtype"] == 3)
                            {
                                string DeptName = MainFunction.GetName("select deptcode + '-' + name as deptname from department where id = " + (int)rr["tostationid"], "deptname");
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "RD-" + DeptName;
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];
                            }
                            else if ((Int16)rr["detailtype"] == 17)
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = "RE-" + rr["name"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];
                            }
                            else if ((Int16)rr["detailtype"] == 5)
                            {
                                string slno = MainFunction.GetName("select a.stationslno from IndentIssue a where a.id =  " + (int)rr["id"], "deptname");
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString() + " (" + slno + ")";
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];

                            }
                            else
                            {
                                led.TransCode = rr["stationslno"].ToString();
                                led.Description = rr["Description"].ToString();
                                led.Date = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                led.Issue = 0;
                                led.Receipt = qty / Conv;
                                led.Price = (decimal)rr["CostPrice"] * conversionfactor;
                                led.Price = Math.Round(led.Price, 2);
                                led.Amount = (decimal)rr["Value"];
                                led.Amount = Math.Round(led.Amount, 2);
                                led.ExpDate = ExptDdate;
                                led.UnitCost = led.Price;
                                led.RecValue = (decimal)rr["Value"];
                                led.RecValue = Math.Round(led.RecValue, 2);
                                TotalReceiptValue = TotalReceiptValue + (decimal)rr["Value"];
                            }

                        }
                        fgReport.Add(led);
                    }
                    StDate = StDate.AddMonths(1);

                }

                List<ItemLedgerPriceDetail> SortedList = fgReport.OrderBy(o => o.Date).ToList();
                List<ItemLedgerPriceDetail> finallist = new List<ItemLedgerPriceDetail>();
                ItemLedgerPriceDetail OpeningBal = new ItemLedgerPriceDetail();
                OpeningBal.TransCode = "OPENIING";
                OpeningBal.Description = "BALANCE";
                if (bal == 0)
                {
                    OpeningBal.Stock = 0;
                }
                else
                {
                    OpeningBal.Stock = (decimal)Math.Round((decimal)bal, 2);
                    OpeningBal.Value = BalVal - OpeningBal.IssValue + OpeningBal.RecValue;
                    OpeningBal.Value = Math.Round(OpeningBal.Value, 2);
                    OpeningBal.UnitCost = OpeningBal.Value / OpeningBal.Stock;
                    OpeningBal.UnitCost = Math.Round(OpeningBal.UnitCost, 2);
                }
                finallist.Add(OpeningBal);
                decimal CountStock = OpeningBal.Stock;
                foreach (ItemLedgerPriceDetail nx in SortedList)
                {
                    nx.Stock = CountStock - (decimal)nx.Issue + (decimal)nx.Receipt;
                    nx.Stock = Math.Round(nx.Stock, 2);
                    nx.Value = BalVal - nx.IssValue + nx.RecValue;
                    nx.Value = Math.Round(nx.Value, 2);
                    nx.UnitCost = nx.Value / nx.Stock;
                    nx.UnitCost = Math.Round(nx.UnitCost, 2);
                    finallist.Add(nx);
                    CountStock = nx.Stock;
                    BalVal -= nx.IssValue + nx.RecValue;
                }
                bal = 0;
                ItemLedgerPriceDetail CloseingBal = new ItemLedgerPriceDetail();
                CloseingBal.TransCode = "CLOSING";
                CloseingBal.Description = "BALANCE";
                CloseingBal.Stock = (decimal)Math.Round((decimal)CountStock, 2);
                CloseingBal.Value = Math.Round(BalVal, 2);
                CloseingBal.UnitCost = CloseingBal.Value / CloseingBal.Stock;
                CloseingBal.UnitCost = Math.Round(CloseingBal.UnitCost, 2);
                finallist.Add(CloseingBal);

                ll = finallist;
                
                                 ll[0].ItemCode = ItemCode;
                ll[0].ItemName = ItemName;
                ll[0].Units = unitname;
                ll[0].Category = CatName;
                ll[0].TotalIssues = (int)(decimal)totalissues;
                ll[0].TotalReceipt = (int)(decimal)totalreceipts;
                return ll;
            }
            catch (Exception e) { return ll; }


        }
        
        public static ExpiryDateReportHeader ExpiryDateReport(DateTime dtpFromDt, DateTime dtpToDt,
            string gStationname, int Gstationid, int CatID, int RadioID, int chkAsOfDate, int chkAsPrice)
        {
            ExpiryDateReportHeader ll = new ExpiryDateReportHeader();
            try
            {
                bool gRepWithPrice = false;
                string StrSql = "";
                string strOrderby = "";
                string strCriteria = " ";
                if (chkAsPrice == 1) { gRepWithPrice = true; ll.WithPrice = true; }


                if (RadioID == 0)                   {
                    strOrderby = " Order by a.ItemCode ";

                }
                else if (RadioID == 1)
                {
                    strOrderby = " Order by substring(a.itemcode,len(a.itemcode),1) ";

                }
                else
                {
                    strOrderby = " Order by b.ExpiryDate ";

                }


                if (CatID != 0)
                {
                    strCriteria = " and a.Categoryid = " + CatID;

                }


                if (chkAsOfDate == 1)
                {
                    StrSql = "select a.name,a.id,(bs.quantity/(s.conversionqty / 1.0))  as qoh,b.batchno,"
                        + " (b.costprice * (s.Conversionqty/1.0)) as Costprice,b.expirydate,c.name as cat,a.itemcode,b.batchid from"
                        + " item a,batch b,batchstore bs,itemgroup c,itemstore s where a.id = b.itemid and"
                        + " a.deleted=0 and bs.quantity>0 and b.batchid = bs.batchid and bs.stationid = " + Gstationid + " and a.categoryid=c.id and a.id = s.itemid and s.stationid = " + Gstationid + "  and"
                        + " b.expirydate < '" + DateTime.Now.Date.AddDays(1).ToString("dd-MMM-yyyy") + "'" + strCriteria + strOrderby;

                }
                else
                {
                    StrSql = "select a.name,a.id,(bs.quantity/ (s.conversionqty /1.0))  as qoh,b.batchno,"
                        + " (b.costprice * (S.conversionqty / 1.0)) as Costprice,b.expirydate,c.name as cat,a.itemcode,b.batchid from"
                        + " item a,batch b,batchstore bs,itemgroup c,itemstore s where a.id = b.itemid and"
                        + " a.deleted=0 and bs.quantity>0 and b.batchid = bs.batchid and bs.stationid = " + Gstationid + " and a.categoryid=c.id and a.id = s.itemid and s.stationid = " + Gstationid + " and b.expirydate>= '" + dtpFromDt.ToString("dd-MMM-yyyy") + "' and"
                        + " b.expirydate < '" + dtpToDt.AddDays(1).ToString("dd-MMM-yyyy") + "'" + strCriteria + strOrderby;
                }
                DataSet dst = MainFunction.SDataSet(StrSql, "tbl");
                decimal cTotValue = 0;
                decimal cValue = 0;
                int SNO = 0;
                List<ExpiryDateReportDetail> dtllist = new List<ExpiryDateReportDetail>();
                if (dst.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in dst.Tables[0].Rows)
                    {
                        SNO += 1;
                        DataSet supplier = MainFunction.SDataSet("select s.code,s.name "
                        + " from supplier s,purchasereceiptdetail pr,purchasereceipt p "
                        + " where p.id = pr.receiptid and p.supplierid = s.id "
                        + "  and pr.itemid = " + (int)rr["id"] + " and pr.batchid = " + (int)rr["batchid"], "tbl");
                        cValue = (decimal)rr["qoh"] * (decimal)rr["Costprice"];
                        if (supplier.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow yy in supplier.Tables[0].Rows)
                            {
                                ExpiryDateReportDetail dtl = new ExpiryDateReportDetail();
                                dtl.SNO = SNO;
                                dtl.ItemCode = rr["itemcode"].ToString();
                                dtl.ItemName = rr["name"].ToString();
                                dtl.QOH = (decimal)rr["qoh"];
                                dtl.BatchNo = rr["batchno"].ToString();
                                dtl.ExpiryDate = MainFunction.DateFormat(rr["expirydate"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                dtl.CostPrice = (decimal)rr["Costprice"];
                                dtl.Value = cValue;
                                dtl.SupplierCode = yy["code"].ToString();
                                dtl.SupplierName = yy["name"].ToString();
                                dtllist.Add(dtl);
                            }
                        }
                        else
                        {
                            ExpiryDateReportDetail dtl = new ExpiryDateReportDetail();
                            dtl.SNO = SNO;
                            dtl.ItemCode = rr["itemcode"].ToString();
                            dtl.ItemName = rr["name"].ToString();
                            dtl.QOH = (decimal)rr["qoh"];
                            dtl.BatchNo = rr["batchno"].ToString();
                            dtl.ExpiryDate = MainFunction.DateFormat(rr["expirydate"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                            dtl.CostPrice = (decimal)rr["Costprice"];
                            dtl.Value = cValue;
                            dtllist.Add(dtl);
                        }
                        cTotValue = cTotValue + cValue;
                    }

                    if (gRepWithPrice == true)
                    {
                        ll.TotalValue = cTotValue;
                    }

                }
                else
                {
                    ll.ErrMsg = "Details not found";
                    return ll;

                }

                ll.TransDetail = dtllist;
                if (chkAsOfDate == 1)
                {
                    ll.Title = "Expiry Date Report as on" + DateTime.Now.Date.ToString("dd-MM-yyyy");
                }
                else
                {
                    ll.Title = " Expiry Date Report for items expiring between " + dtpFromDt.ToString("dd-MM-yyyy") + "  and " + dtpToDt.ToString("dd-MM-yyyy");

                }
                if (CatID == 0) { ll.Category = "All "; } else { ll.Category = MainFunction.GetName("select name from Itemgroup where id=" + CatID, "name"); }



                return ll;
            }
            catch (Exception e) { return ll; }
        }
        


        public static ISBSHeader ISBS(int SupID, int RadioID, int gStationID)
        {
            ISBSHeader ll = new ISBSHeader();
            if (RadioID == 0)
            { ll.Title = "Total Stock BY Supplier - Total Stock = 0 "; }
            else if (RadioID == 1)
            { ll.Title = "Total Stock BY Supplier - Total Stock > 0 "; }
            else
            { ll.Title = "Total Stock BY Supplier - Total Stock = All "; }

            ll.Supplier = MainFunction.GetName("select Name from supplier where id=" + SupID, "Name");


            try
            {
                List<ISBSDetail> lst = new List<ISBSDetail>();
                DataSet ds = SP_TO_DataSet(SupID);
                int slno = 0;
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    slno++;
                    ISBSDetail nn = new ISBSDetail();
                    nn.ID = (int)rr["id"];
                    nn.ItemCode = rr["itemcode"].ToString();
                    nn.ItemName = rr["name"].ToString();
                    nn.conversionqty = (int)rr["conversionqty"];
                    nn.Unit = rr["unit"].ToString();

                    nn.totiss = Math.Round((decimal)rr["totiss"], 2);
                    nn.qty1 = Math.Round((decimal)rr["qty1"] / nn.conversionqty, 2);                     nn.qty2 = Math.Round((decimal)rr["qty3"] / nn.conversionqty, 2);                     nn.qty3 = Math.Round((decimal)rr["qty4"] / nn.conversionqty, 2);                     nn.qty4 = Math.Round((decimal)rr["qty2"] / nn.conversionqty, 2);                     nn.qty5 = Math.Round((decimal)rr["qty5"] / nn.conversionqty, 2); 
                    nn.totiss_inp = Math.Round((decimal)rr["totiss_inp"], 2);
                    nn.inp_conversionqty = (int)rr["inp_conversionqty"];
                    nn.inp_unit = rr["inp_uniT"].ToString();
                    nn.TotalStock = nn.qty1 + nn.qty2 + nn.qty3 + nn.qty4 + nn.qty5;
                                         nn.TotalIssuance = Math.Round((decimal)rr["totiss" + gStationID.ToString()] / nn.conversionqty);

                    nn.inp_unit = nn.Unit;



                                                              switch (RadioID)
                    {
                        case 0: { if (nn.TotalStock == 0) { lst.Add(nn); };break; }
                        case 1: { if (nn.TotalStock > 0) { lst.Add(nn); };break; }
                        default: { lst.Add(nn); break; }

                                             }
                }
                List<ISBSDetail> xx = lst.OrderBy(o => o.ItemName).ToList();
                int SNO = 1;
                foreach (var item in xx)
                {
                    item.SNO = SNO;
                    SNO += 1;
                }

                                                   
                                                                                                                                                                                                                             
                                  
                ll.TransDetail = xx;
                return ll;
            }
            catch (Exception e) { return ll; }
        }
        public static DataSet SP_TO_DataSet(int @supplierid)
        {
            string SqlConn = MainFunction.SqlConn;
            using (SqlConnection Sconn = new SqlConnection(SqlConn))
            {
                Sconn.Open();
                using (SqlCommand SCom = new SqlCommand())
                {

                    DateTime dt = DateTime.Now;
                    string @enddate = dt.ToString("dd MMM yyyy");
                    string @startdate = dt.AddMonths(-2).ToString(("dd MMM yyyy"));

                    string @hd1 = "Transorder_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString();
                    string @dd1 = "Transorderdetail_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString();

                    string @hd2 = "Transorder_" + DateTime.Now.AddMonths(-1).Year.ToString() + "_" + DateTime.Now.AddMonths(-1).Month.ToString();
                    string @dd2 = "Transorderdetail_" + DateTime.Now.AddMonths(-1).Year.ToString() + "_" + DateTime.Now.AddMonths(-1).Month.ToString();

                    string @hd3 = "Transorder_" + DateTime.Now.AddMonths(-2).Year.ToString() + "_" + DateTime.Now.AddMonths(-2).Month.ToString();
                    string @dd3 = "Transorderdetail_" + DateTime.Now.AddMonths(-2).Year.ToString() + "_" + DateTime.Now.AddMonths(-2).Month.ToString();




                    SCom.Connection = Sconn;
                    SCom.CommandType = CommandType.StoredProcedure;
                    SCom.CommandText = "newITEM_STOCK_BY_SUPPLIER";
                    SCom.CommandTimeout = 400;
                    SCom.Parameters.AddWithValue("@supplierid", @supplierid);
                    SCom.Parameters.AddWithValue("@startdate", @startdate);
                    SCom.Parameters.AddWithValue("@enddate", @enddate);

                    SCom.Parameters.AddWithValue("@hdTable1", @hd1);
                    SCom.Parameters.AddWithValue("@dtTable1", @dd1);

                    SCom.Parameters.AddWithValue("@hdTable2", @hd2);
                    SCom.Parameters.AddWithValue("@dtTable2", @dd2);

                    SCom.Parameters.AddWithValue("@hdTable3", @hd3);
                    SCom.Parameters.AddWithValue("@dtTable3", @dd3);

                    SqlDataAdapter SAdpter = new SqlDataAdapter();
                    DataSet Rset = new DataSet();
                    SAdpter.SelectCommand = SCom;
                    SAdpter.Fill(Rset, "tb");
                    return Rset;


                                                                                                                                                                                                                                                                            }
            }


        }
        
        public static ItemMasterListHeader ItemList(int gStationId, int CatID, string prefix, string suffix, int QtyList, int OrderList, string FromDate, string ToDate, int chkWithoutLoc, int chkWithExpiry, int chkCost, int ShelfID)
        {
            ItemMasterListHeader ll = new ItemMasterListHeader();
            try
            {
                List<ItemMasterListDetail> dtlList = new List<ItemMasterListDetail>();
                string strCriteria = "";
                if (prefix.Length > 0)
                {
                    strCriteria = " and a.itemcode like '" + prefix.Trim() + "%'";
                }
                string strSuffix = "";
                if (suffix.Length > 0)
                {
                    strSuffix = " and a.itemcode like   '" + "%" + suffix.Trim() + "'";
                }
                string groupst = "";
                string StrSql = "";
                decimal TotalAmt = 0;

                if (QtyList == 1)                  {
                    if (OrderList == 0)
                    {
                        groupst = " group by a.name,a.id,a.conversionqty,b.name,e.name,a.itemcode,a.deleted,a.qoh,a.totalcost,a.expirydate ";
                    }
                    else
                    {
                        groupst = " group by a.itemcode,a.name,a.id,a.conversionqty,b.name,e.name,a.deleted,a.qoh,a.totalcost,a.expirydate ";
                    }

                                                                                    
                    if (ShelfID == 0)
                    {
                        StrSql = "Select a.id as itemid,a.name,a.conversionqty,isnull(a.totalcost,0) as totcost,a.qoh as qoh,b.name as unit,"
                       + " e.name as cat,a.itemcode,a.deleted,a.expirydate as ExpirationDate,'' as BatchQty,1 as type "
                       + " from mms_itemmaster a,packing b,itemgroup e,batch bt"
                       + " where  a.categoryid=e.id and a.unitid=b.id and bt.itemid = a.id " + strCriteria + " " + strSuffix + " and a.qoh>0 and a.stationid=" + gStationId
                       + " and a.categoryid=" + CatID + " " + groupst;
                    }
                    else if (ShelfID > 0)
                    {
                        StrSql = "Select a.id as itemid,a.name,a.conversionqty,isnull(a.totalcost,0) as totcost,a.qoh as qoh,b.name as unit,"
                       + " e.name as cat,a.itemcode,a.deleted,a.expirydate as ExpirationDate,'' as BatchQty,1 as Type "
                       + " from mms_itemmaster a,packing b,itemgroup e,batch bt,itemLocation l"
                       + " where  a.categoryid=e.id and a.unitid=b.id and bt.itemid = a.id " + strCriteria + " " + strSuffix + " and a.qoh>0 and a.stationid=" + gStationId
                       + " and l.itemid = a.id and l.stationid=a.stationid and l.shelfid = " + ShelfID + " " + groupst;
                    }

                                                                                                                                                                                                                                                                                                                                                                                                                                    

                    StrSql = StrSql.ToLower();
                    DataSet rsReport = MainFunction.SDataSet(StrSql, "tbl");
                    TotalAmt = 0;
                    int sno = 1;
                    foreach (DataRow rr in rsReport.Tables[0].Rows)
                    {
                        bool Added = false;
                        string Str = "";
                        string rackshelf = "";
                        if (ShelfID == 0)
                        {
                            Str = " select r.name as rack from itemlocation l,rack r Where l.Itemid = " + (int)rr["itemid"] + " And l.rackId = r.id "
                                + " and l.stationid=" + gStationId
                                + " Union "
                                + " select s.name as shelf from itemlocation l,shelf s Where l.Itemid = " + (int)rr["itemid"] + " And l.ShelfId = s.id "
                                + " and l.stationid=" + gStationId;

                        }
                        else
                        {
                            Str = " select r.name as rack from itemlocation l,rack r Where l.Itemid = " + (int)rr["itemid"] + " And l.rackId = r.id "
                                + " and l.stationid=" + gStationId
                                + "  Union "
                                + " select s.name as shelf from itemlocation l,shelf s Where l.Itemid = " + (int)rr["itemid"] + " And s.id = " + ShelfID + " and l.ShelfId = s.id"
                                + " and l.stationid=" + gStationId;
                        }
                        DataSet rsTemp = MainFunction.SDataSet(Str, "tbl");
                        if (rsTemp.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow yy in rsTemp.Tables[0].Rows)
                            {
                                rackshelf = rackshelf + (rackshelf.Trim().Length == 0 ? "" + yy["rack"].ToString() : "--" + yy["rack"].ToString());
                            }
                        }
                        else
                        {
                            rackshelf = "";

                        }

                        if (chkWithoutLoc == 0)
                        {
                            if ((int)rr["qoh"] == 0 && (int)rr["deleted"] == 1)
                            { }
                            else
                            {
                                ItemMasterListDetail dtl = new ItemMasterListDetail();
                                dtl.SNO = sno;
                                dtl.ItemCode = rr["itemcode"].ToString();
                                dtl.ItemName = rr["name"].ToString();
                                dtl.Unit = rr["unit"].ToString();

                                decimal QTY = (decimal)(int)rr["qoh"];
                                dtl.QOH = (decimal)(QTY / (int)rr["conversionqty"]);

                                dtl.RackShelf = rackshelf;
                                dtl.TotalCost = (decimal)rr["totcost"];
                                                                                                  if ((int)rr["type"] == 1)
                                {
                                    TotalAmt = TotalAmt + (decimal)rr["totcost"];
                                }
                                dtlList.Add(dtl);
                                Added = true;
                                sno += 1;


                            }
                        }
                        else
                        {
                            if (rackshelf.Length == 0)
                            {
                                if ((int)rr["qoh"] == 0 && (int)rr["deleted"] == 1) { }
                                else
                                {
                                    ItemMasterListDetail dtl = new ItemMasterListDetail();
                                    dtl.SNO = sno;
                                    dtl.ItemCode = rr["itemcode"].ToString();
                                    dtl.ItemName = rr["name"].ToString();
                                    dtl.Unit = rr["unit"].ToString();

                                    decimal QTY = (decimal)(int)rr["qoh"];
                                    dtl.QOH = (decimal)(QTY / (int)rr["conversionqty"]);

                                    dtl.RackShelf = rackshelf;
                                    dtl.TotalCost = (decimal)rr["totcost"];
                                                                                                              if ((int)rr["type"] == 1)
                                    {
                                        TotalAmt = TotalAmt + (decimal)rr["totcost"];
                                    }
                                    dtlList.Add(dtl);
                                    Added = true;
                                    sno += 1;

                                }
                            }
                        }


                        
                        if (chkWithExpiry == 1 && Added == true)
                        {
                            StrSql = " Select 0 as totcost,0 as qoh,bt.Expirydate as expirationdate,bts.Quantity as batchqty "
                                + " from Batch bt,batchstore bts where bt.itemid = " + (int)rr["itemid"] + " and bts.quantity > 0 "
                                + " and bt.batchid=bts.batchid and bt.batchno=bts.batchno and bt.itemid=bts.itemid and bts.stationid=" + gStationId
                                ;

                            DataSet expdate = MainFunction.SDataSet(StrSql, "tb");
                            foreach (DataRow nn in expdate.Tables[0].Rows)
                            {
                                ItemMasterListDetail dtl = new ItemMasterListDetail();
                                dtl.SNO = sno - 1;

                                decimal QTY = (decimal)(int)nn["batchqty"];
                                dtl.QOH = Math.Round((decimal)(QTY / (int)rr["conversionqty"]), 2);


                                dtl.ItemName = String.IsNullOrEmpty(nn["expirationdate"].ToString()) == true ? ""
                                    : MainFunction.DateFormat(nn["expirationdate"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                dtl.ItemName = "Expiry Date : " + dtl.ItemName.PadRight(15, ' ') + "   < -------- >     Batch Qty : " + dtl.QOH.ToString();
                                dtl.QOH = 0;
                                dtlList.Add(dtl);

                            }
                        }
                        Added = false;

                    }



                }
                else                 {
                    if (OrderList == 0)                          groupst = "order by a.name";
                    else
                    {
                        groupst = "order by a.itemcode";
                    }

                    StrSql = "Select a.id as itemid,a.conversionqty,a.name,a.tax,b.name as unit,"
                    + " e.name as cat,a.itemcode "
                    + " from mms_itemmaster a,packing b,itemgroup e"
                    + " where  a.categoryid=e.id and a.stationid=" + gStationId + " and  a.unitid=b.id  " + strCriteria + " " + strSuffix
                    + " and  a.categoryid=" + CatID + " " + groupst;
                    StrSql = StrSql.ToLower();

                    DataSet rsReport = MainFunction.SDataSet(StrSql, "tbl");
                    TotalAmt = 0;
                    int sno = 1;
                    foreach (DataRow rr in rsReport.Tables[0].Rows)
                    {

                        string Str = "";
                        string rackshelf = "";


                        Str = " select r.name as rack from itemlocation l,rack r Where l.Itemid = " + (int)rr["itemid"] + " And l.rackId = r.id "
                            + " and l.stationid=" + gStationId
                            + " Union "
                       + " select s.name as shelf from itemlocation l,shelf s Where l.Itemid = " + (int)rr["itemid"] + " And l.ShelfId = s.id"
                       + " and l.stationid=" + gStationId;
                        DataSet rsTemp = MainFunction.SDataSet(Str.ToLower(), "tb");
                        if (rsTemp.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow yy in rsTemp.Tables[0].Rows)
                            {
                                rackshelf = rackshelf + (rackshelf.Trim().Length == 0 ? "" + yy["rack"].ToString() : "--" + yy["rack"].ToString());

                            }
                        }
                        else { rackshelf = ""; }


                        StrSql = "select sum(bt.costprice*bts.quantity) as totcost,sum(bts.quantity) as qoh,'' as expirationdate, "
                            + " 0 as batchqty "
                            + " from batch bt,batchstore bts where bt.itemid = " + (int)rr["itemid"]
                            + " and bt.batchid=bts.batchid and bt.batchno=bts.batchno and bt.itemid=bts.itemid and bts.stationid=" + gStationId
                            + " group by bt.itemid ";

                        DataSet rsBtTemp = MainFunction.SDataSet(StrSql.ToLower(), "tb");
                        if (rsBtTemp.Tables[0].Rows.Count == 0)
                        {
                            if (chkWithoutLoc == 0)
                            {

                                ItemMasterListDetail dtl = new ItemMasterListDetail();
                                dtl.SNO = sno;
                                dtl.ItemCode = rr["itemcode"].ToString();
                                dtl.ItemName = rr["name"].ToString();
                                dtl.Unit = rr["unit"].ToString();
                                dtl.RackShelf = rackshelf;
                                dtlList.Add(dtl);
                                sno += 1;

                            }
                            else
                            {
                                if (rackshelf.Length == 0)
                                {
                                    ItemMasterListDetail dtl = new ItemMasterListDetail();
                                    dtl.SNO = sno;
                                    dtl.ItemCode = rr["itemcode"].ToString();
                                    dtl.ItemName = rr["name"].ToString();
                                    dtl.Unit = rr["unit"].ToString();
                                    dtl.RackShelf = rackshelf;
                                    dtlList.Add(dtl);
                                    sno += 1;
                                }

                            }



                        }
                        else
                        {
                            bool Added = false;
                            foreach (DataRow xx in rsBtTemp.Tables[0].Rows)
                            {

                                if (chkWithoutLoc == 0)
                                {
                                    ItemMasterListDetail dtl = new ItemMasterListDetail();
                                    dtl.SNO = sno;
                                    dtl.ItemCode = rr["itemcode"].ToString();
                                    dtl.ItemName = rr["name"].ToString();
                                    dtl.Unit = rr["unit"].ToString();

                                    decimal QTY = (decimal)(int)xx["qoh"];
                                    dtl.QOH = (decimal)(QTY / (int)rr["conversionqty"]);


                                    dtl.RackShelf = rackshelf;
                                    dtl.TotalCost = (decimal)xx["totcost"];
                                                                                                              TotalAmt = TotalAmt + (decimal)xx["totcost"];
                                    dtlList.Add(dtl);
                                    Added = true;
                                    sno += 1;
                                }
                                else
                                {
                                    if (rackshelf.Length == 0)
                                    {
                                        ItemMasterListDetail dtl = new ItemMasterListDetail();
                                        dtl.SNO = sno;
                                        dtl.ItemCode = rr["itemcode"].ToString();
                                        dtl.ItemName = rr["name"].ToString();
                                        dtl.Unit = rr["unit"].ToString();

                                        decimal QTY = (decimal)(int)xx["qoh"];
                                        dtl.QOH = (decimal)(QTY / (int)rr["conversionqty"]);

                                        dtl.RackShelf = rackshelf;
                                        dtl.TotalCost = (decimal)xx["totcost"];
                                                                                                                          TotalAmt = TotalAmt + (decimal)xx["totcost"];
                                        dtlList.Add(dtl);
                                        Added = true;
                                        sno += 1;
                                    }

                                }
                            }

                            
                            if (chkWithExpiry == 1 && Added == true)
                            {
                                StrSql = " Select 0 as totcost,0 as qoh,bt.Expirydate as expirationdate,bts.Quantity as batchqty "
                                    + " from Batch bt,batchstore bts where bt.itemid = " + (int)rr["itemid"] + " and bts.quantity > 0 "
                                    + " and bt.batchid=bts.batchid and bt.batchno=bts.batchno and bt.itemid=bts.itemid and bts.stationid=" + gStationId
                                    ;

                                DataSet expdate = MainFunction.SDataSet(StrSql, "tb");
                                foreach (DataRow nn in expdate.Tables[0].Rows)
                                {
                                    ItemMasterListDetail dtl = new ItemMasterListDetail();
                                    dtl.SNO = sno - 1;

                                    decimal QTY = (decimal)(int)nn["batchqty"];
                                    dtl.QOH = Math.Round((decimal)(QTY / (int)rr["conversionqty"]), 2);

                                    dtl.ItemName = String.IsNullOrEmpty(nn["expirationdate"].ToString()) == true ? ""
                                        : MainFunction.DateFormat(nn["expirationdate"].ToString(), "dd", "MM", "yyyy", "", "", "", "/", "");
                                    dtl.ItemName = "Expiry Date : " + dtl.ItemName.PadRight(15, ' ') + "   < -------- >     Batch Qty : " + dtl.QOH.ToString();
                                    dtl.QOH = 0;
                                    dtlList.Add(dtl);
                                }
                            }
                            Added = false;

                        }
                    }


                }
                ll.TransDetail = dtlList;
                ll.Category = MainFunction.GetName("select name from itemgroup where id=" + CatID, "Name");
                ll.Station = MainFunction.GetName("select name from Station where id=" + gStationId, "Name");
                ll.TotalCost = TotalAmt;
                ll.Title = "ITEM MASTER WITH LOCATION LIST AS ON " + DateTime.Now.Date.ToString("dd-MMM-yyyy");
                ll.ShowCost = chkCost;
                ll.ShowExpirydate = chkWithExpiry;
                return ll;
            }
            catch (Exception e) { return ll; }


        }
        
        public static IPWiseIssueHeader IPWise(int Gstationid, int Stationid, int IPID, string FromDate, string Todate)
        {
            IPWiseIssueHeader ll = new IPWiseIssueHeader();
            try
            {
                string StrSql = " ";
                string Cond = " ";
                int sno = 0;
                string ip = "";
                                 if (Stationid > 0) { Cond = " and s.ID= " + Stationid; }
                if (IPID > 0) { Cond += "and ip.ipid=" + IPID; }

                StrSql = "select B.Name as BedNo , "
                + " ip.issueauthoritycode +'.' + right('0000000000'+ ltrim(cast(ip.RegistrationNo as varchar(10))),10) as RegNo, "
                + " IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') as PtName, "
                + " cast(ip.Age as varchar(10)) + A.Name as Age,case ip.sex when 1 then 'Female' else 'Male' end as Sex, "
                + " ip.Admitdatetime AdmitDate, S.Code as Station, "
                + " case  d.PrescriptionID when 0 then 'PR' else 'DO' end as pType,d.DispatchedDateTime DDAte, "
                + " I.ItemCode, I.Name ItemName,ds.DispatchQuantity Qty, p.Name pack "
                + " from Bed B, Station S, inpatient ip, AgeType A, Item i, Drugorder d, drugorderdetailsubstitute ds, packing p "
                + " where d.ToStationId=" + Gstationid + " and D.DispatchedDateTime > '" + FromDate + "' And D.DispatchedDateTime <'" + Todate + "'  "
                + " and A.Id = Ip.AgeType and b.ipid = ip.IpId and S.Id = b.StationId  and D.ipid = ip.IpId and d.Id=ds.OrderId and I.Id=ds.SubstituteId "
                + " And P.Id = ds.UnitID " + Cond + " Order by S.Name,b.name,IP.RegistrationNo,d.PrescriptionID";
                DataSet rsItem = MainFunction.SDataSet(StrSql, "tbl");
                List<IPWiseIssueDetail> dtllist = new List<IPWiseIssueDetail>();
                foreach (DataRow rr in rsItem.Tables[0].Rows)
                {
                    if (rr["RegNo"].ToString() != ip)
                    {
                        ip = rr["RegNo"].ToString();
                        sno = 1;                         IPWiseIssueDetail dtl = new IPWiseIssueDetail();
                        dtl.SNO = 0;
                        dtl.PInfo = "0";
                        dtl.Type = rr["BedNo"].ToString() + ',' + rr["RegNo"].ToString() + ',' + rr["PtName"].ToString()
                                  + rr["Age"].ToString() + ',' + rr["Sex"].ToString() + ','
                                  + MainFunction.DateFormat(rr["AdmitDate"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-", "")
                                  + ',' + rr["Station"].ToString();
                        dtllist.Add(dtl);

                        IPWiseIssueDetail dtl2 = new IPWiseIssueDetail();
                        dtl2.Type = rr["pType"].ToString();
                        dtl2.SNO = sno;
                        dtl2.TranDate = MainFunction.DateFormat(rr["DDAte"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-", "");
                        dtl2.ItemCode = rr["ItemCode"].ToString();
                        dtl2.ItemName = rr["ItemName"].ToString();
                        dtl2.QOH = (decimal)rr["Qty"];
                        dtl2.Unit = rr["pack"].ToString();
                        dtllist.Add(dtl2);
                    }
                    else
                    {
                        IPWiseIssueDetail dtl2 = new IPWiseIssueDetail();
                        dtl2.Type = rr["pType"].ToString();
                        dtl2.SNO = sno;
                        dtl2.TranDate = MainFunction.DateFormat(rr["DDAte"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-", "");
                        dtl2.ItemCode = rr["ItemCode"].ToString();
                        dtl2.ItemName = rr["ItemName"].ToString();
                        dtl2.QOH = (decimal)rr["Qty"];
                        dtl2.Unit = rr["pack"].ToString();
                        dtllist.Add(dtl2);
                    }
                    sno += 1;
                }

                ll.TransDetail = dtllist;
                return ll;
            }
            catch (Exception e) { return ll; }

        }

        
        public static List<ErrMedModel> ErrMedFun(DateTime ee)
        {
            List<ErrMedModel> ll = new List<ErrMedModel>();
            try
            {
                DataSet ds = SP_TO_DataSet2(ee);
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ErrMedModel nn = new ErrMedModel();
                    nn.GROUPID = (int)rr["GROUPID"];
                    nn.GroupName = rr["GroupName"].ToString();
                    nn.Title = rr["Title"].ToString();
                    nn.Ccount = rr["Ccount"].ToString();
                    ll.Add(nn);
                }
                return ll;
            }
            catch (Exception e) { return ll; }
        }
        public static DataSet SP_TO_DataSet2(DateTime @Dat)
        {
            string SqlConn = MainFunction.SqlConn;
            using (SqlConnection Sconn = new SqlConnection(SqlConn))
            {
                Sconn.Open();
                using (SqlCommand SCom = new SqlCommand())
                {
                    string dsss = @Dat.ToString("dd MMM yyyy");


                    SCom.Connection = Sconn;
                    SCom.CommandType = CommandType.StoredProcedure;
                    SCom.CommandText = "SP_ErrMedicationSummary";
                    SCom.CommandTimeout = 400;
                    SCom.Parameters.AddWithValue("@DAT", dsss);

                    SqlDataAdapter SAdpter = new SqlDataAdapter();
                    DataSet Rset = new DataSet();
                    SAdpter.SelectCommand = SCom;
                    SAdpter.Fill(Rset, "tb");
                    return Rset;


                                                                                                                                                                                                                                                                            }
            }


        }

        
        public static List<ActiveProfileMdl> GetListActive(int StationID, int IPID, int RptType)
        {
            List<ActiveProfileMdl> ll = new List<ActiveProfileMdl>();
            try
            {
                DataSet ds = SP_TO_DataSetActiveProfile(StationID, IPID, RptType);
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ActiveProfileMdl nn = new ActiveProfileMdl();
                    nn.State = rr["State"].ToString();
                    nn.PinNO = rr["PinNO"].ToString();
                    nn.emp = rr["emp"].ToString();
                    nn.bed = rr["bed"].ToString();
                    nn.xemp = rr["xemp"].ToString();
                    nn.StartDate = rr["StartDate"].ToString();
                    nn.EndDate = rr["EndDate"].ToString();
                    nn.PINNO2 = (int)rr["PINNO2"];
                    nn.topline = (int)rr["topline"];
                    ll.Add(nn);
                }
                return ll;
            }
            catch (Exception e) { return ll; }


        }
        public static DataSet SP_TO_DataSetActiveProfile(int Stationid, int ipid, int RptType)
        {
            string SqlConn = MainFunction.SqlConn;
            using (SqlConnection Sconn = new SqlConnection(SqlConn))
            {
                Sconn.Open();
                using (SqlCommand SCom = new SqlCommand())
                {
                    SCom.Connection = Sconn;
                    SCom.CommandType = CommandType.StoredProcedure;
                    SCom.CommandText = "MMS_RptActiveProfile";
                    SCom.CommandTimeout = 400;
                    SCom.Parameters.AddWithValue("@ParaStation", Stationid);
                    SCom.Parameters.AddWithValue("@ParaIPID", ipid);
                    SCom.Parameters.AddWithValue("@ParaRptType", RptType);

                    SqlDataAdapter SAdpter = new SqlDataAdapter();
                    DataSet Rset = new DataSet();
                    SAdpter.SelectCommand = SCom;
                    SAdpter.Fill(Rset, "tb");
                    return Rset;


                                                                                                                                                                                                                                                                            }
            }


        }
        
        public static AdjustIssueRptMdl AdjIssuereport(int CatID, bool chkCost, DateTime FromDate, DateTime ToDate, User UserT)
        {
            AdjustIssueRptMdl ll = new AdjustIssueRptMdl();
            try
            {
                string strCriteria = "";
                int lconvqty = 0;
                if (CatID > 0) { strCriteria = " and i.categoryid = " + CatID; }
                string StrSql = "SELECT A.ID,AD.ITEMID,A.STATIONSLNO,A.REFNO,A.DATETIME,(I.ITEMCODE + '--' + I.NAME) AS NAME,AD.Quantity,"
                + " P.NAME AS UNIT,cast(isnull(ad.unitid,0) as int) unitid,bt.costprice,AD.remarks, A.OperatorId "
                + " FROM ADJUSTISSUES A,ADJUSTISSUEDETAIL AD,ITEM I,PACKING P,batch bt "
                + " WHERE A.ID = AD.ADJISSID AND ad.batchid = bt.batchid and AD.ITEMID = I.ID AND AD.UNITID *= P.ID AND STATIONID = " + UserT.selectedStationID
                + " and a.datetime > = '" + FromDate + "' and a.datetime < '" + ToDate.AddDays(1) + "'" + strCriteria
                + " ORDER BY STATIONSLNO    ";
                if (ToDate < FromDate) { ll.ErrMsg = "To date can't be less than the from date"; return ll; }
                DataSet ds = MainFunction.SDataSet(StrSql, "tb");
                List<AdjustIssueRptMdlDtl> dtlList = new List<AdjustIssueRptMdlDtl>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        if ((int)rr["unitid"] == 0)
                        {
                            lconvqty = 1;
                        }
                        else
                        {

                            lconvqty = MainFunction.GetQuantity((int)rr["unitid"], (int)rr["ITEMID"]);
                        }
                        AdjustIssueRptMdlDtl dtl = new AdjustIssueRptMdlDtl();
                        dtl.IssueNo = rr["ID"].ToString();
                        dtl.RefNo = rr["REFNO"].ToString();
                        dtl.Date = MainFunction.DateFormat(rr["DATETIME"].ToString(), "dd", "MM", "yyyy");
                        dtl.Item = rr["NAME"].ToString();
                        dtl.Qty = Math.Round((decimal)rr["Quantity"], 2).ToString();
                        dtl.Unit = rr["UNIT"].ToString();
                        dtl.Remarks = rr["remarks"].ToString();
                        dtl.UserId = rr["OperatorId"].ToString();
                        dtl.Cost = Math.Round((lconvqty * (decimal)rr["Quantity"] * (decimal)rr["costprice"]), 2).ToString();
                        ll.TotalAmount += (lconvqty * (decimal)rr["Quantity"] * (decimal)rr["costprice"]);

                        dtlList.Add(dtl);
                    }
                }
                else
                {
                    ll.ErrMsg = "Details Not Found"; return ll;

                }
                ll.TransDetail = dtlList;
                ll.Title = "ADJUSTMENT ISSUE FROM " + FromDate.ToString("dd-MM-yyyy") + "    To  " + ToDate.ToString("dd-MM-yyyy");
                ll.Station = UserT.StationName;
                ll.chkWithAmount = chkCost;
                return ll;
            }
            catch (Exception e) { return ll; }
        }

        
        public static AdjustRecRptMdl AdjReceiptreport(int CatID, bool chkCost, DateTime FromDate, DateTime ToDate, User UserT)
        {
            AdjustRecRptMdl ll = new AdjustRecRptMdl();
            try
            {
                string strCriteria = "";
                int lconvqty = 0;
                if (CatID > 0) { strCriteria = " and i.categoryid = " + CatID; }

                string StrSql = "SELECT E.EMPLOYEEID,AD.REMARKS,ad.QOH,AD.ITEMID,A.STATIONSLNO,a.ID,A.REFNO,A.DATETIME,I.ITEMCODE ,I.NAME,AD.QUANTITY,"
                + " P.NAME AS UNIT,ad.unitid,(ad.quantity * AD.EPR) AS amount,bt.costprice "
                + " , (ad.quantity + ad.QOH) AS NewQty "
                + " FROM adjustreceipt A,adjustreceiptdetail AD,ITEM I,PACKING P,EMPLOYEE E,BATCH BT "
                + " WHERE A.OPERATORID = E.ID AND A.ID = AD.adjreceiptid and AD.ITEMID = I.ID AND AD.UNITID *= P.ID "
                + " AND ad.batchid = bt.batchid AND AD.ITEMID = BT.ITEMID AND STATIONID = " + UserT.selectedStationID
                + " and a.datetime > = '" + FromDate + "' and a.datetime < '" + ToDate.AddDays(1) + "'" + strCriteria
                + " ORDER BY a.datetime,a.STATIONSLNO    ";


                if (ToDate < FromDate) { ll.ErrMsg = "To date can't be less than the from date"; return ll; }
                DataSet ds = MainFunction.SDataSet(StrSql, "tb");
                List<AdjustRecRptMdlDtl> dtlList = new List<AdjustRecRptMdlDtl>();
                string DDate = "";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        AdjustRecRptMdlDtl dtl = new AdjustRecRptMdlDtl();
                        if (DDate != MainFunction.DateFormat(rr["DATETIME"].ToString(), "dd", "MMM", "yyyy"))
                        {
                            DDate = MainFunction.DateFormat(rr["DATETIME"].ToString(), "dd", "MMM", "yyyy");
                            dtl.Date = DDate;
                        }
                        else { dtl.Date = ""; }

                        dtl.ItemCode = rr["ITEMCODE"].ToString();
                        dtl.ItemName = rr["NAME"].ToString();
                        dtl.OriginalQty = string.Format("{0:0,0.00}", Math.Round((decimal)rr["QOH"], 2).ToString());
                        dtl.AdjQty = string.Format("{0:0,0.00}", Math.Round((decimal)rr["QUANTITY"], 2).ToString());
                        dtl.Amount = string.Format("{0:0,0.00}", Math.Round((decimal)rr["amount"], 2).ToString());
                        dtl.NewQty = string.Format("{0:0,0.00}", Math.Round((decimal)rr["NewQty"], 2).ToString());
                        dtl.UserID = rr["EMPLOYEEID"].ToString();
                        dtl.DocNo = rr["ID"].ToString();
                        dtl.CostPrice = string.Format("{0:0,0.00}", Math.Round((decimal)rr["costprice"], 2).ToString());
                        ll.TotalAmount += (decimal)rr["amount"];
                        dtlList.Add(dtl);
                    }
                }
                else
                {
                    ll.ErrMsg = "Details Not Found"; return ll;

                }
                ll.TransDetail = dtlList;
                ll.Title = "ADJUSTMENT RECEIPT FROM " + FromDate.ToString("dd-MM-yyyy") + "    To  " + ToDate.ToString("dd-MM-yyyy");
                ll.Station = UserT.StationName;
                ll.chkWithAmount = chkCost;
                return ll;
            }
            catch (Exception e) { return ll; }
        }

        
        public static ADTRptMdl ADTReport(int TypeID, int StationID, string STName, string PIN, DateTime FromDate, DateTime ToDate, User UserT)
        {
            ADTRptMdl mm = new ADTRptMdl();
            try
            {
                mm.Title = "LIST OF Admission/Discharge Patient";
                mm.Station = STName;
                string StrSql = "";
                if (TypeID == 0)
                {
                    string Cond = "";
                    if (PIN.Length > 2) { Cond = " And ip.RegistrationNo = " + PIN; }
                    if (StationID > 0) { Cond = " and S.Id= " + StationID; }


                    StrSql = "select IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') as patientname, "
                 + " ip.Admitdatetime IDate,  ip.RegistrationNo, A.Name as AgeType, ip.Age, Ip.Sex, B.Name as BedNo, "
                 + " S.Code as Station  from Bed B, Station S, inpatient ip, AgeType A "
                 + "  where A.Id = Ip.AgeType and b.ipid = ip.IpId and S.Id = b.StationId " + Cond
                 + " Order by S.Name,b.name,IP.RegistrationNo ";
                }
                else if (TypeID == 1)
                {
                    StrSql = "select IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') as patientname, "
                 + " ip.Admitdatetime IDate,  ip.RegistrationNo, A.Name as AgeType, ip.Age, Ip.Sex, B.Name as BedNo, "
                 + " S.Code as Station  from  Bed B, Station S, Oldinpatient ip, AgeType A,"
                 + " bedtransfers bt,  (Select Ipid, Max(Id) id from bedtransfers group by ipid) t   "
                 + " where A.Id = Ip.AgeType and bt.ipid = ip.IpId and b.Id = bt.bedid and bt.id=t.id and S.Id = b.StationId  and "
                 + " ip.DischargeDateTime >'" + FromDate + "' and ip.DischargeDateTime <'" + ToDate + "' Order by S.Name,b.name,IP.RegistrationNo ";
                }
                else if (TypeID == 2)
                {
                    StrSql = "select IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') as patientname, "
                   + " ip.Admitdatetime IDate,  ip.RegistrationNo, A.Name as AgeType, ip.Age, Ip.Sex, B.Name as BedNo, "
                   + " S.Code as Station  from Bed B, Station S, inpatient ip, AgeType A , OtOrder O "
                   + "  where ip.ipid=o.IPIDOPID  and A.Id = Ip.AgeType and b.ipid = ip.IpId and S.Id = b.StationId "
                   + " And O.Released <> 1 AND O.PatientType=1 and datediff(hour,O.otstartdatetime,getdate())<=2  Order by S.Name ";

                }
                DataSet ds = MainFunction.SDataSet(StrSql, "tbl");
                int slno = 1;
                mm.TransDetail = new List<ADTRptMdlDtl>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ADTRptMdlDtl n = new ADTRptMdlDtl();
                    n.slno = slno.ToString();
                    n.bed = row["BedNo"].ToString();
                    n.pin = MainFunction.getUHID(UserT.gIACode, row["RegistrationNo"].ToString(), false);
                    n.name = row["patientname"].ToString();
                    n.age = row["Age"].ToString() + ' ' + row["AgeType"].ToString();
                    n.sex = row["Sex"].ToString() == "1" ? "Female" : "Male";
                    n.AdmDate = MainFunction.DateFormat(row["IDate"].ToString(), "dd", "MM", "yyyy", "hh", "mm", "ss");
                    n.station = row["Station"].ToString();
                    mm.TransDetail.Add(n);
                    slno += 1;

                }
                return mm;
            }
            catch (Exception e) { return mm; }




        }

        
        public static List<AntiUsedMdl> GetListAnti(DateTime AsOfMonth, User UserT)
        {

            List<AntiUsedMdl> ll = new List<AntiUsedMdl>();
            try
            {
                string StrSql = "select ot.ipidopid as IPID,pt.PTName,PT.RegNo "
                + " from OTOrder as OT,"
                + " (Select inpatient.ipid, registrationno as RegNo,Title + '. ' + Firstname + ' ' + MiddleName + ' ' + Lastname as PTName, "
                + " bed.name, AdmitDatetime,'' as DischargeDatetime,CompanyID "
                + " from inpatient,bed where bed.ipid=inpatient.ipid "
                + " Union All  "
                + " select ipid,registrationno,Title + '. ' + Firstname + ' ' + MiddleName + ' ' + Lastname as PTName, "
                + " '---',AdmitDatetime,DischargeDatetime as DischargeDateTime,CompanyID "
                + " from oldinpatient ) as PT "
                + " where ot.ipidopid=pt.ipid "
                + " And month(OTStartDateTime)=" + AsOfMonth.Month
                + " and YEAR(OTStartDateTime)=" + AsOfMonth.Year + " order by otstartdatetime ";
                DataSet Ds = MainFunction.SDataSet(StrSql, "tb");
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow IPID in Ds.Tables[0].Rows)
                    {
                        DataSet Dtl = SP_TO_DataAntiUsed((int)IPID["IPID"]);
                        foreach (DataRow rr in Dtl.Tables[0].Rows)
                        {
                            AntiUsedMdl rec = new AntiUsedMdl();
                            rec.PIN = IPID["RegNo"].ToString();
                            rec.Name = IPID["PTName"].ToString();
                            rec.Doctor = rr["SDR"].ToString();
                            rec.Item = rr["Name"].ToString();
                            rec.Station = rr["StationName"].ToString();
                            rec.DateTime = MainFunction.DateFormat(rr["Datetime"].ToString(), "dd", "MM", "yy", "hh", "mm", "ss");
                            rec.Status = rr["Status"].ToString();
                            ll.Add(rec);
                        }
                    }
                }
                else
                {
                    AntiUsedMdl n = new AntiUsedMdl();
                    n.ErrMsg = "Can't Load the data!";
                    ll.Add(n);
                }
                return ll;
            }
            catch (Exception e)
            {
                if (ll.Count == 0)
                {
                    AntiUsedMdl n = new AntiUsedMdl();
                    n.ErrMsg = "Can't Load the data!";
                    ll.Add(n);
                }
                else { ll[0].ErrMsg = "Can't Load the data!"; }
                return ll;
            }
        }
        public static DataSet SP_TO_DataAntiUsed(int ipid)
        {
            string SqlConn = MainFunction.SqlConn;
            using (SqlConnection Sconn = new SqlConnection(SqlConn))
            {
                Sconn.Open();
                using (SqlCommand SCom = new SqlCommand())
                {
                    SCom.Connection = Sconn;
                    SCom.CommandType = CommandType.StoredProcedure;
                    SCom.CommandText = "PR_GetSurgeryChargesAntiBiotic";
                    SCom.CommandTimeout = 400;
                    SCom.Parameters.AddWithValue("@IPID", ipid);

                    SqlDataAdapter SAdpter = new SqlDataAdapter();
                    DataSet Rset = new DataSet();
                    SAdpter.SelectCommand = SCom;
                    SAdpter.Fill(Rset, "tb");
                    return Rset;

                }
            }


        }

    } 
} 






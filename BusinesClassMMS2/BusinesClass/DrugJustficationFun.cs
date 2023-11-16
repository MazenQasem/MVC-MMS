using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class DrugJustificationFun
    {
        public static DrugJustificationModel Clear()
        {
            DrugJustificationModel nn = new DrugJustificationModel();
            List<TempListMdl> SelectionList = new List<TempListMdl>();
            TempListMdl node1 = new TempListMdl();
            node1.ID = 0; node1.Name = "All"; SelectionList.Add(node1);
            TempListMdl node2 = new TempListMdl();
            node2.ID = 1; node2.Name = "Based on C/S"; SelectionList.Add(node2);
            TempListMdl node3 = new TempListMdl();
            node3.ID = 2; node3.Name = "Empirical"; SelectionList.Add(node3);
            TempListMdl node4 = new TempListMdl();
            node4.ID = 3; node4.Name = "Pre- Operative"; SelectionList.Add(node4);
            nn.Tlist = SelectionList;

             
            return nn;
        }


        public static List<DetailsView> LoadView(string txt)
        {
            List<DetailsView> getviews = new List<DetailsView>();
            try
            {
                if (txt == "All") { txt = ""; } else { txt = "  AND B.Selection_Basis='" + txt + "' "; }

                string StrSql = " select b.orderId as OrderId,b.serviceid,c.registrationno as Registrationno, "
                    + " d.name as StationName, e.name as DrugName, b.justification as Justification,b.accepted as accepted,"
                    + " b.Accept_Reject_Reason,d.prefix  + '-' + cast(a.stationslno as varchar) as OrderNo "
                    + " ,doc.empcode + '-' + doc.name as DoctorName , emp.name as NurseName "
                    + " ,case b.accepted when 0 then 'Pending' when 1 then 'Accepted' when -1 then 'Rejected' else '<First Order>' end as ADescription "
                    + " ,case CHARINDEX('FLAG',b.justification,1) when 0 then b.justification else 'ORDER' end as DocJust "
                    + " from DrugOrder a, antibiotic_Justification_Drugorder b, inpatient c, station d, item e "
                    + " ,employee emp,doctor doc "
                    + " Where a.ipid = c.ipid And a.ID = b.orderId And a.StationID = d.ID And b.ServiceId = e.ID "
                    + " and a.doctorid=doc.id and a.operatorid=emp.id " + txt
                    + " and (b.accepted = -1 or b.accepted = -2)  and datediff(mi,b.dr_date,GetDate())> 30 order by b.dr_date desc ";


                DataSet ds = MainFunction.SDataSet(StrSql, "tb");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    DetailsView dtl = new DetailsView();
                    dtl.OrderNo = rr["OrderNo"].ToString();
                    dtl.ServiceID = (int)rr["serviceid"];
                    dtl.PIN = rr["Registrationno"].ToString();
                    dtl.Station = rr["StationName"].ToString();
                    dtl.DrugName = rr["DrugName"].ToString();
                    dtl.DoctorJustification = rr["DocJust"].ToString();
                    dtl.Acknowledgement = rr["ADescription"].ToString();
                    dtl.AcknowRemarks = rr["Accept_Reject_Reason"].ToString();
                    dtl.OrderdDoctor = rr["DoctorName"].ToString();
                    dtl.NurseName = rr["NurseName"].ToString();
                    dtl.OrderID = (int)rr["OrderId"];
                    dtl.Accepted = (int)rr["accepted"];
                    dtl.MainAccepted = (int)rr["accepted"];

                    getviews.Add(dtl);
                }
                return getviews;

            }
            catch (Exception e) { return getviews; }
        }

        public static List<DetailsView> Save(List<DetailsView> tbl, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    foreach (DetailsView row in tbl)
                    {

                        if (row.MainAccepted != row.Accepted)
                        {
                            string str = "update antibiotic_Justification_Drugorder set accepted =" + row.Accepted
                             + " , accept_reject_reason = '" + row.AcknowRemarks + "',decision_station=" + UserInfo.selectedStationID
                             + " ,Decision_datetime = getdate() where orderid = " + row.OrderID;
                            bool UpdateAntibiotic = MainFunction.SSqlExcuite(str, Trans);
                        }
                    }
                    Trans.Commit();
                    tbl[0].ErrMsg = "Data Saved";
                    return tbl;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                tbl[0].ErrMsg = "Error while saving!";
                return tbl;
            }

        }







    } 
} 





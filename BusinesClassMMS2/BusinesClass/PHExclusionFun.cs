using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class PHExclusionFun
    {
        public static PHExclusionModel Clear(int gStation)
        {
            PHExclusionModel nn = new PHExclusionModel();

            return nn;
        }

        public static List<TempListMdl> InsertItem(string Str)
        {
            List<TempListMdl> ll = new List<TempListMdl>();

            try
            {
                DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    TempListMdl l = new TempListMdl();
                    l.ID = (int)rr["ID"];
                    l.Name = rr["Name"].ToString();
                    ll.Add(l);
                }

                return ll;
            }
            catch (Exception e)
            {
                TempListMdl f = new TempListMdl();
                f.ErrMsg = e.Message;
                ll.Add(f);
                return ll;

            }
        }




        public static PHExclusionModel Save(PHExclusionModel order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    string StrSql = "";
                    if (order.OPIP == "OP")
                    {
                        StrSql = "Delete from OPCompanyItemServices where CategoryId ="
                            + order.CategoryID + " and  CompanyId="
                            + order.CompanyID + " and Gradeid= "
                            + order.GradeID + " and ServiceId=11";
                        bool update = MainFunction.SSqlExcuite(StrSql, Trans);

                        foreach (var itm in order.ItemList)
                        {
                            if (itm.ID > 0)
                            {
                                StrSql = "Insert into OPCompanyItemServices (CategoryID,CompanyID,GradeId,ServiceID"
                                    + " ,DepartmentID,ItemID,IncludeType,StartDatetime,OperatorId)  "
                                    + " values (" + order.CategoryID + "," + order.CompanyID
                                    + "," + order.GradeID + " ,11,0," + itm.ID + " ,1,GetDate(),1 ) ";
                                bool itmupdate = MainFunction.SSqlExcuite(StrSql, Trans);
                            }
                        }
                    }
                    else
                    {
                        StrSql = "Delete from IpCompanyItemServices where CategoryId ="
                            + order.CategoryID + " and  CompanyId=" + order.CompanyID
                            + " and Gradeid= " + order.GradeID + " and ServiceId=5";
                        bool update = MainFunction.SSqlExcuite(StrSql, Trans);
                        foreach (var itm in order.ItemList)
                        {
                            if (itm.ID > 0)
                            {
                                StrSql = "Insert into IpCompanyItemServices (CategoryID,CompanyID,GradeId,ServiceID"
                                    + ",DepartmentID,ItemID,IncludeType,Datetime)  "
                                    + " values (" + order.CategoryID + "," + order.CompanyID
                                    + "," + order.GradeID + " ,5,0," + itm.ID + " ,1,GetDate()) ";
                                bool itmupdate = MainFunction.SSqlExcuite(StrSql, Trans);
                            }
                        }

                    }
                    order.ErrMsg = "Data Saved";
                    Trans.Commit();
                    return order;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                order.ErrMsg = "Error while saving!";
                return order;
            }

        }

    }

}






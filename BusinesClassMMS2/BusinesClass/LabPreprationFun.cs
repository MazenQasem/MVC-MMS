using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class LabPreprationFun
    {
        public static LabModel Clear(int gStation)
        {
            LabModel nn = new LabModel();

            return nn;
        }

        public static List<ProfileItems> InsertItem(int ProfID, List<ProfileItems> ExistList = null)
        {
            List<ProfileItems> ll = new List<ProfileItems>();
            try
            {
                int GetMaxID = MainFunction.GetOneVal("select id as MaxID from labpreparation where itemid=" + ProfID, "MaxID");
                int GetUOM = MainFunction.GetOneVal("select unitid as UnitID from labpreparation where itemid=" + ProfID, "UnitID");

                string Str = "";
                if (ExistList == null)
                {
                    
                    Str = "select i.id as profileid,a.id,a.name,b.qty,u.name as unit,i.unitid as packid,b.unitid "
                    + " from item a,labpreparationdetail b,labpreparation i,packing u "
                    + " where a.id=b.itemid and u.id = b.unitid and b.preparationid = i.id and i.itemid = " + ProfID
                    + " order by slno ";
                }
                else
                {
                    Str = "select distinct i.id ,i.name,i.unitid,u.name as unit,0 as qty,0 as profileid,0 as packid "
                        + " from Item i ,packing u where i.deleted=0 and i.ID=" + ProfID
                        + " and u.id=i.unitid ";
                }

                DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int sno = 0;
                    if (ExistList == null)
                    {
                        sno = 1;
                    }
                    else
                    {
                        sno = ExistList.Count + 1;
                    }
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        ProfileItems l = new ProfileItems();
                        l.SNO = sno;
                        l.ID = (int)rr["id"];
                        l.Drug = rr["name"].ToString();
                        l.Unit = rr["unit"].ToString();
                        l.Qty = (int)rr["qty"];
                        l.UnitID = (int)rr["unitid"];
                                                 l.MaxID = GetMaxID;
                                                 l.ProUOMID = GetUOM;

                        if (sno > 1 && ExistList != null)
                        {
                            ExistList.Add(l);
                            ll = ExistList;
                        }
                        else
                        {
                            ll.Add(l);
                            sno += 1;
                        }
                    }
                }

                return ll;
            }
            catch (Exception e)
            {
                ll[0].ErrMsg = "Item Error: " + e.Message;

                return ll;
            }




        }
        public static LabModel Save(LabModel order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                int GetMaxID = 0;
                if (order.MaxID == 0)
                {
                    GetMaxID = MainFunction.GetOneVal("select id as MaxID from labpreparation where itemid=" + order.ProfileID, "MaxID");
                }

                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    if (order.ProfileID > 0)                       {


                        string StrSql = " Update labpreparation set unitid = " + order.ProfileUnitID
                            + ", modifiedby = " + UserInfo.EmpID + ", modifieddatetime = sysdatetime() "
                            + " where id = " + order.MaxID
                            + " and itemid=" + order.ProfileID;
                        bool SaveOld = MainFunction.SSqlExcuite(StrSql, Trans);

                        StrSql = "delete from labpreparationdetail where preparationid =" + order.MaxID;
                        bool SaveOldDetail = MainFunction.SSqlExcuite(StrSql, Trans);




                        int count = 1;
                        foreach (var it in order.SelectedItems)
                        {
                            StrSql = "insert into labpreparationdetail(preparationid,itemid,unitid,qty,slno)values"
                            + "(" + order.MaxID
                            + "," + it.ID
                            + "," + it.UnitID
                            + "," + it.Qty
                            + "," + count + ") ";
                            bool insdtl = MainFunction.SSqlExcuite(StrSql, Trans);
                            count += 1;
                        }

                        order.ErrMsg = "Profile definition saved successfully";

                    }

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






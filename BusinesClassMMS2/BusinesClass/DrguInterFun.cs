using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class DrugInterFun
    {
        public static List<GenericDrugDetail> ViewGenDtl(int GID, string GName)
        {
            List<GenericDrugDetail> ll = new List<GenericDrugDetail>();
            try
            {
                string Sql = "select a.id,a.name,b.Discription  "
                + " from M_generic a,L_DrugDrugInteraction b"
                + " Where a.id = b.interactingGenericId And b.genericId =" + GID + " order by a.name ";
                DataSet ds = MainFunction.SDataSet(Sql, "tbl");
                int seq = 1;
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    GenericDrugDetail dtl = new GenericDrugDetail();
                    dtl.Seq = seq;
                    dtl.GenericID = GID;
                    dtl.Generic = GName;
                    dtl.DrugID = (int)rr["id"];
                    dtl.Drug = rr["name"].ToString();
                    dtl.Reaction = rr["Discription"].ToString();
                    ll.Add(dtl);
                    seq += 1;
                }

                return ll;
            }
            catch (Exception e) { return ll; }
        }

        public static MessageModel Save(List<GenericDrugDetail> tbl, User Operator)
        {
            SqlConnection Con = MainFunction.MainConn();
            MessageModel Msg = new MessageModel();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    int gSavedById = Operator.EmpID;
                    int GenericID = tbl[0].GenericID;
                    if (GenericID > 0)
                    {
                        string StrSql = "Insert into canDrugDrugInteraction (GenericID,InteractingGenericId,Discription,grouporgeneric,StartDatetime, "
                            + " EndDateTime,OperatorId, DeletedBy,DeletedDatetime) "
                            + "  select  GenericID,InteractingGenericId,Discription,grouporgeneric,StartDatetime,EndDateTime,OperatorId, " + gSavedById
                            + " , getdate()  from l_drugdruginteraction where genericid=" + GenericID + " ";
                        bool Del = MainFunction.SSqlExcuite(StrSql, Trans);

                        StrSql = "Delete from l_drugdruginteraction where genericid=" + GenericID;
                        bool DelActual = MainFunction.SSqlExcuite(StrSql, Trans);

                        foreach (GenericDrugDetail row in tbl)
                        {
                            var sql = "Insert into L_DrugDrugInteraction(genericId,InteractingGenericId,Discription)"
                            + " values(" + row.GenericID + "," + row.DrugID + ",'" + row.Reaction.ToUpper() + "' )";
                            bool InsertNew = MainFunction.SSqlExcuite(sql, Trans);

                        }
                    }
                    else                      {
                        string txtGeneric = tbl[0].Generic;
                        string StrSql = "Insert into M_generic(name,hipar,startdatetime,operatorID,deleted) values('" + txtGeneric + "',0,getdate()," + gSavedById + ",0)";
                        bool inserNew = MainFunction.SSqlExcuite(StrSql, Trans);

                        int getNewID = MainFunction.GetOneVal("Select max(id) as id from M_generic", "id", Trans);
                        foreach (GenericDrugDetail row in tbl)
                        {
                            var sql = "Insert into L_DrugDrugInteraction(genericId,InteractingGenericId,Discription)"
                            + " values(" + getNewID + "," + row.DrugID + ",'" + row.Reaction.ToUpper() + "' )";
                            bool InsertNew = MainFunction.SSqlExcuite(sql, Trans);

                        }



                    }

                    Trans.Commit();
                    Msg.Message = "Interacting generics modified.";
                    Msg.isSuccess = true;
                    return Msg;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                Msg.Message = "Error while saving!";
                Msg.id = 0;
                Msg.isSuccess = false;
                return Msg;
            }
        }

        
        public static DrugForumlary ViewGenDtlF(int GID)
        {
            DrugForumlary dtl = new DrugForumlary();
            try
            {
                string Sql = "Select id,GENERIC,BRANDNAME,THERAPEUTICCATEGORY,[USE] as USee,CONTRAINDICATION,WARNINGPRECAUTIONS, "
                    + " ADVERSEREACTIONS,MECHANISMOFACTION,USUALDOSAGE,DOSAGEFORMS,REMARKS from DFDetail where ID=" + GID;
                DataSet ds = MainFunction.SDataSet(Sql, "tbl");
                int seq = 1;
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    dtl.GenericID = (int)rr["id"];
                    dtl.BrandName = rr["BRANDNAME"].ToString();
                    dtl.Category = rr["THERAPEUTICCATEGORY"].ToString();
                    dtl.Use = rr["USee"].ToString();
                    dtl.Indication = rr["CONTRAINDICATION"].ToString();
                    dtl.Warning = rr["WARNINGPRECAUTIONS"].ToString();
                    dtl.Reaction = rr["ADVERSEREACTIONS"].ToString();
                    dtl.Mechanism = rr["MECHANISMOFACTION"].ToString();
                    dtl.Dosage = rr["USUALDOSAGE"].ToString();
                    dtl.DosageForms = rr["DOSAGEFORMS"].ToString();
                    dtl.Remarks = rr["REMARKS"].ToString();
                }

                return dtl;
            }
            catch (Exception e) { return dtl; }
        }

        public static MessageModel DeleteF(DrugForumlary tbl, User Operator)
        {
            SqlConnection Con = MainFunction.MainConn();
            MessageModel Msg = new MessageModel();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    string SQL = " Insert into CanDFDetail (ID,GENERIC,BRANDNAME,THERAPEUTICCATEGORY,[USE], " +
                    " CONTRAINDICATION,WARNINGPRECAUTIONS,ADVERSEREACTIONS,MECHANISMOFACTION,USUALDOSAGE," +
                    " DOSAGEFORMS,REMARKS,DTIME,OPERATORID,DEL_MOD,CANDTIME,CANOPERATORID) select ID,GENERIC,BRANDNAME,THERAPEUTICCATEGORY,[USE], " +
                    " CONTRAINDICATION,WARNINGPRECAUTIONS,ADVERSEREACTIONS,MECHANISMOFACTION,USUALDOSAGE," +
                    " DOSAGEFORMS,REMARKS,DTIME,OPERATORID,0,getdate()," + Operator.EmpID + " from DFDetail where id= " + tbl.GenericID;
                    bool MovTOCancelTable = MainFunction.SSqlExcuite(SQL, Trans);
                    bool Delete = MainFunction.SSqlExcuite(" Delete from DFDetail Where ID=" + tbl.GenericID, Trans);

                    Trans.Commit();
                    Msg.Message = "Record was Deleted";
                    Msg.isSuccess = true;
                    return Msg;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                Msg.Message = "Error while deleting!";
                Msg.id = 0;
                Msg.isSuccess = false;
                return Msg;
            }
        }
        public static MessageModel SaveF(DrugForumlary tbl, User Operator)
        {
            SqlConnection Con = MainFunction.MainConn();
            MessageModel Msg = new MessageModel();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    string SQL = "";
                    if (tbl.GenericID == 0)
                    {
                        SQL = " Insert Into DFDetail (GENERIC,BRANDNAME,THERAPEUTICCATEGORY,[USE], "
                        + " CONTRAINDICATION,WARNINGPRECAUTIONS,ADVERSEREACTIONS,MECHANISMOFACTION,USUALDOSAGE,"
                        + " DOSAGEFORMS,REMARKS,DTIME,OPERATORID) Values ('" + tbl.GenericName + "', '" + tbl.BrandName + "', "
                        + " '" + tbl.Category + "','" + tbl.Use + "','" + tbl.Indication + "','" + tbl.Warning + "', "
                        + " '" + tbl.Reaction + "','" + tbl.Mechanism + "','" + tbl.Dosage + "','" + tbl.DosageForms + "', "
                        + " '" + tbl.Remarks + "',getdate()," + Operator.EmpID + " )";
                    }
                    else
                    {
                                                 string xSQL = " Insert into CanDFDetail (ID,GENERIC,BRANDNAME,THERAPEUTICCATEGORY,[USE], " +
                        " CONTRAINDICATION,WARNINGPRECAUTIONS,ADVERSEREACTIONS,MECHANISMOFACTION,USUALDOSAGE," +
                        " DOSAGEFORMS,REMARKS,DTIME,OPERATORID,DEL_MOD,CANDTIME,CANOPERATORID) select ID,GENERIC,BRANDNAME,THERAPEUTICCATEGORY,[USE], " +
                        " CONTRAINDICATION,WARNINGPRECAUTIONS,ADVERSEREACTIONS,MECHANISMOFACTION,USUALDOSAGE," +
                        " DOSAGEFORMS,REMARKS,DTIME,OPERATORID,0,getdate()," + Operator.EmpID + " from DFDetail where id= " + tbl.GenericID;

                        bool MovTOCancelTable = MainFunction.SSqlExcuite(xSQL, Trans);
                        SQL = " Update DFDetail set BrandName='" + tbl.BrandName + "',"
                        + " THERAPEUTICCATEGORY ='" + tbl.Category + "', "
                        + " [USE]='" + tbl.Use + "', "
                        + " CONTRAINDICATION='" + tbl.Indication + "', "
                        + " WARNINGPRECAUTIONS='" + tbl.Warning + "', "
                        + " ADVERSEREACTIONS='" + tbl.Reaction + "', "
                        + " MECHANISMOFACTION='" + tbl.Mechanism + "', "
                        + " USUALDOSAGE='" + tbl.Dosage + "', "
                        + " DOSAGEFORMS='" + tbl.DosageForms + "', "
                        + " REMARKS='" + tbl.Remarks + "' "
                        + " Where ID=" + tbl.GenericID;
                    }


                    bool UpdateRec = MainFunction.SSqlExcuite(SQL, Trans);


                    Trans.Commit();
                    Msg.Message = "Record was Updated";
                    Msg.isSuccess = true;
                    return Msg;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                Msg.Message = "Error while Saving!";
                Msg.id = 0;
                Msg.isSuccess = false;
                return Msg;
            }
        }





    }

}







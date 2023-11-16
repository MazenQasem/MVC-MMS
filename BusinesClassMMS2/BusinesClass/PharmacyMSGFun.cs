using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class PharmacyMSGFun
    {
        public static DrugAllergyMdl ViewDetail(int Pin, User US)
        {
            DrugAllergyMdl Mn = new DrugAllergyMdl();
            try
            {
                string StrSql = "Select (lTrim(O.Title) + ' ' + lTrim(isnull(O.FirstName,'')) + ' ' + lTrim(isnull(O.MiddleName,'')) + ' ' "
                    + "  + lTrim(isnull(O.LastName,''))) as Name, "
                    + "O.Sex,O.Age,a.name as agetype,isnull(o.otherallergies,'') otherallergies "
                    + " from Patient O,agetype a where o.agetype=a.id and O.RegistrationNo=" + Pin + " and o.issueauthoritycode='" + US.gIACode + "'";
                DataSet Ds = MainFunction.SDataSet(StrSql, "tbL");
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in Ds.Tables[0].Rows)
                    {
                        Mn.PinNo = Pin.ToString();
                        Mn.PTName = rr["Name"].ToString();
                        if (rr["Sex"].ToString() == "1") { Mn.Sex = "Female"; }
                        else if (rr["Sex"].ToString() == "2") { Mn.Sex = "Male"; }
                        else if (rr["Sex"].ToString() == "3") { Mn.Sex = "Unknown"; }
                        else if (rr["Sex"].ToString() == "4") { Mn.Sex = "Others"; }
                        Mn.othertxt = rr["otherallergies"].ToString();
                        Mn.Age = rr["Age"].ToString() + "  " + rr["agetype"].ToString();
                        Mn.ErrMsg = "";
                        Mn.PHmsgList = GetItemList(Mn.PinNo);
                    }
                }
                else
                {
                    Mn.ErrMsg = "Patient PIN Not Found!";
                }
                return Mn;
            }
            catch (Exception e) { Mn.ErrMsg = "Error While loading !"; return Mn; }
        }
        public static List<PHMsgTable> GetItemList(string PIN)
        {
            List<PHMsgTable> ll = new List<PHMsgTable>();
            try
            {
                string sql = "select a.id,a.message,e.name as operator,s.name as station,a.deleted "
                    + " from pharmacistmsg a,employee e,station s where e.id=a.operatorid and a.stationid=s.id and a.deleted=0 "
                    + " and a.pinno =" + PIN;
                DataSet ds = MainFunction.SDataSet(sql, "tb");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    PHMsgTable nn = new PHMsgTable();
                    nn.ID = rr["id"].ToString();
                    nn.msg = rr["message"].ToString();
                    nn.Operator = rr["operator"].ToString();
                    nn.Station = rr["station"].ToString();
                    nn.deleted = rr["deleted"].ToString();
                    ll.Add(nn);
                }
                return ll;
            }
            catch (Exception e) { return ll; }


        }


        public static MessageModel Save(DrugAllergyMdl order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            MessageModel Msg = new MessageModel();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    bool DelRec = MainFunction.SSqlExcuite("Delete from Pharmacistmsg where PinNo=" + order.PIN, Trans);
                    if (order.PHmsgList.Count > 0)
                    {
                        foreach (PHMsgTable rec in order.PHmsgList)
                        {
                            int DelFalg = rec.deleted == "True" ? 1 : 0;
                            string Str = "Insert into  Pharmacistmsg (Pinno,message,OperatorID,stationid,DateTime,deleted) "
                                + " Values(" + order.PIN + ",'" + rec.msg + "',"
                                + UserInfo.EmpID + "," + UserInfo.selectedStationID + ",sysdatetime(),"
                                + DelFalg + " )";
                            bool SaveG = MainFunction.SSqlExcuite(Str, Trans);
                        }
                    }
                    Trans.Commit();
                    Msg.Message = "Message saved successfully";
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
    }

}






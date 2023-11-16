using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class DrugAllergyFun
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
                        Mn.DrugList = GetItemList(Mn.PinNo);
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
        public static List<TempListMdl> GetItemList(string PIN)
        {
            List<TempListMdl> ll = new List<TempListMdl>();
            try
            {
                string sql = "select a.Name as Name,b.watdrugid as ID from M_Generic a,patientdrugallergies b where a.id=b.watdrugid and b.registrationno=" + PIN;
                DataSet ds = MainFunction.SDataSet(sql, "tb");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    TempListMdl nn = new TempListMdl();
                    nn.ID = (int)rr["ID"];
                    nn.Name = rr["Name"].ToString();
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
                    bool DelRec = MainFunction.SSqlExcuite("Delete from patientDrugAllergies where registrationno=" + order.PIN, Trans);
                    if (order.DrugList.Count > 0)
                    {
                        foreach (TempListMdl rec in order.DrugList)
                        {
                            string Str = "insert into patientdrugallergies (RegistrationNo,IssueAuthorityCode,WatDrugID) " +
                                " values(" + order.PIN + ",'" + UserInfo.gIACode + "'," + rec.ID + ")";
                            bool SaveG = MainFunction.SSqlExcuite(Str, Trans);
                        }
                    }
                    string OtherTxt = order.othertxt == null ? "" : order.othertxt.ToString();
                    string Othestr = "update patient set otherallergies = '" + OtherTxt + "' where registrationno =" + order.PIN;
                    bool SaveT = MainFunction.SSqlExcuite(Othestr, Trans);
                    Trans.Commit();
                    Msg.Message = "Allergies of Patient Recorded";
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
        public static DrugAllergyMdl PrintOut(string PIN, User UserInfo)
        {
            DrugAllergyMdl Mn = new DrugAllergyMdl();
            try
            {
                string StrSql = "Select (lTrim(O.Title) + ' ' + lTrim(isnull(O.FirstName,'')) + ' ' + lTrim(isnull(O.MiddleName,'')) + ' ' "
                    + "  + lTrim(isnull(O.LastName,''))) as Name, "
                    + "O.Sex,O.Age,a.name as agetype,isnull(o.otherallergies,'') otherallergies "
                    + " from Patient O,agetype a where o.agetype=a.id and O.RegistrationNo=" + PIN + " and o.issueauthoritycode='" + UserInfo.gIACode + "'";
                DataSet Ds = MainFunction.SDataSet(StrSql, "tbL");
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in Ds.Tables[0].Rows)
                    {
                        Mn.PinNo = MainFunction.getUHID(UserInfo.gIACode, PIN, true);
                        Mn.PTName = rr["Name"].ToString();
                        if (rr["Sex"].ToString() == "1") { Mn.Sex = "Female"; }
                        else if (rr["Sex"].ToString() == "2") { Mn.Sex = "Male"; }
                        else if (rr["Sex"].ToString() == "3") { Mn.Sex = "Unknown"; }
                        else if (rr["Sex"].ToString() == "4") { Mn.Sex = "Others"; }
                        Mn.othertxt = rr["otherallergies"].ToString();
                        Mn.Age = rr["Age"].ToString() + "  " + rr["agetype"].ToString();
                        Mn.ErrMsg = "";
                        Mn.DrugList = GetItemList(PIN);
                        Mn.OperatorName = UserInfo.EmployeeID.ToString() + " -  " + UserInfo.Name;
                        Mn.DateTime = MainFunction.DateFormat(DateTime.Now.ToString(), "dd", "MM", "yyyy");
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
    }

}






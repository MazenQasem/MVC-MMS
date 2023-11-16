using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class MedicationErrorFun
    {

        public static MedErrHeader LoadList(string Query)
        {
            MedErrHeader hed = new MedErrHeader();
            try
            {
                string strsql2 = Query;
                DataSet ds = MainFunction.SDataSet(strsql2.ToLower(), "tbl");
                int SNO = 1;
                List<MedErrShowList> lst = new List<MedErrShowList>();
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    MedErrShowList ll = new MedErrShowList();
                    ll.SNO = SNO;
                    ll.OrderID = (int)rr["id"];
                    ll.PIN = MainFunction.getUHID(rr["patauthcode"].ToString(), rr["patregno"].ToString(), true);
                    ll.PTName = rr["ptname"].ToString();
                    ll.Age = rr["age"].ToString();
                    ll.Sex = rr["gender"].ToString();
                    ll.Diagnosis = rr["diagnosis"].ToString();
                    ll.Date = rr["reportdate"].ToString();
                    ll.EventDate = rr["eventdatetime"].ToString();
                    ll.OperatorID = (int)rr["operatorid"];
                    ll.IPID = (int)rr["patregno"];
                    lst.Add(ll);
                    SNO++;

                }

                hed.showlist = lst;
                return hed;
            }
            catch (Exception e) { hed.ErrMsg = e.Message; return hed; }



        }
        public static MedErrHeader PatientData(string Query)
        {
            MedErrHeader ll = new MedErrHeader();
            try
            {
                DataSet ds = MainFunction.SDataSet(Query.ToLower(), "tb");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ll.txtPtName = rr["patname"].ToString();
                    ll.txtAge = rr["age"].ToString() + ' ' + rr["agetype"].ToString();
                    ll.txtSex = rr["gender"].ToString();
                    ll.txtAddress = rr["address1"].ToString() + rr["address2"].ToString() + ',' + rr["country"].ToString() + '.';
                    ll.txtDateTime = DateTime.Now.ToString("dd-MMM-yyyy");
                    ll.txtDrugAllergies = MainFunction.GetName("select a.name as Name from "
                        + " M_Generic a,patientDrugallergies b "
                        + " where a.id=b.WatDrugID and b.Registrationno=" + rr["patregno"].ToString()
                        + " and b.IssueAuthorityCode='" + rr["patauthcode"].ToString() + "'", "Name");
                    ll.txtFoodAllergies = MainFunction.GetName("select a.name as Name from "
                        + " foodrawitems a,patientfoodallergies b "
                        + " where a.id=b.foodid and b.Registrationno=" + rr["patregno"].ToString()
                        + " and b.IssueAuthorityCode='" + rr["patauthcode"].ToString() + "'", "Name");
                    ll.txtPinNo = MainFunction.getUHID(rr["patauthcode"].ToString(), rr["patregno"].ToString(), true);

                }


                return ll;
            }
            catch (Exception e) { ll.ErrMsg = e.Message; return ll; }

        }
        public static MedErrHeader PatientDataDetail(string Query)
        {
            MedErrHeader ll = new MedErrHeader();
            try
            {
                bool stageloop = false;
                bool causloop = false;
                DataSet ds = MainFunction.SDataSet(Query.ToLower(), "tb");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {

                    switch ((int)rr["serviceid"])
                    {
                        case 4:
                            {
                                ll.lstErrType = (int)rr["service"];
                                ll.txtErrType = rr["description"].ToString();
                                break;
                            }
                        case 5:
                            {
                                if ((int)rr["service"] == 1)
                                {
                                    ll.UsedByPatient = true;
                                }
                                else
                                {
                                    ll.UsedByPatient = false;
                                }
                                break;
                            }
                        case 6:
                            {
                                if (stageloop == true)
                                {
                                    ll.lstStage += ',';
                                }
                                ll.lstStage += rr["service"].ToString();                                  stageloop = true;
                                break;
                            }
                        case 7:
                            {
                                ll.txtErrDescription = rr["description"].ToString();
                                break;
                            }
                        case 8:
                            {
                                ll.lstOutcome = (int)rr["service"];
                                break;
                            }
                        case 9:
                            {
                                ll.txtIntervention = rr["description"].ToString();
                                break;
                            }
                        case 10:
                            {
                                ll.txtGenericName = rr["description"].ToString();
                                ll.txtStrength = rr["service"].ToString();
                                break;
                            }
                        case 11:
                            {
                                ll.lstDosage1 = (int)rr["service"];
                                ll.txtDosage1 = rr["description"].ToString();
                                break;
                            }

                        case 12:
                            {
                                ll.lstRoute = (int)rr["service"];
                                ll.txtRoute = rr["description"].ToString();
                                break;
                            }
                        case 13:
                            {
                                ll.lstDosage2 = (int)rr["service"];
                                ll.txtDosage2 = rr["description"].ToString();
                                break;
                            }
                        case 14:
                            {
                                ll.lstErrMadeBy = (int)rr["service"];
                                ll.txtErrMadeBy = rr["description"].ToString();
                                break;
                            }
                        case 15:
                            {
                                if ((int)rr["service"] == 1)
                                {
                                    ll.isoptErrPerputed = true;
                                }
                                else
                                {
                                    ll.isoptErrPerputed = false;
                                }
                                ll.txtErrPerputed = rr["description"].ToString();
                                break;
                            }
                        case 16:
                            {
                                ll.lstErrDiscoverBy = (int)rr["service"];
                                ll.txtErrDiscoverBy = rr["description"].ToString();
                                break;
                            }
                        case 18:
                            {
                                ll.txtHowDiscovered = rr["description"].ToString();
                                break;
                            }
                        case 19:
                            {
                                ll.txtAction = rr["description"].ToString();
                                break;
                            }
                        case 21:
                            {
                                ll.lstInitialErr = (int)rr["service"];
                                break;
                            }
                        case 22:
                            {
                                if (causloop == true)
                                {
                                    ll.lstCauses += ',';
                                }
                                ll.lstCauses += rr["service"].ToString();                                  causloop = true;
                                break;
                            }
                        case 23:
                            {
                                ll.lstErrAction = (int)rr["service"];
                                ll.txtErrAction = rr["description"].ToString();
                                break;
                            }

                        case 24:
                            {
                                ll.txtRecommendation = rr["description"].ToString();
                                break;
                            }
                    }
                    ll.RptDate = MainFunction.DateFormat(rr["reportdate"].ToString(), "yyyy", "MM", "dd", "hh", "mm", "", "-", ":");
                    ll.DtpDateDiscover = MainFunction.DateFormat(rr["eventdisdatetime"].ToString(), "yyyy", "MM", "dd", "hh", "mm", "", "-", ":");
                    ll.DtpErrDate = MainFunction.DateFormat(rr["eventdatetime"].ToString(), "yyyy", "MM", "dd", "hh", "mm", "", "-", ":");
                    ll.txtRptNo = rr["id"].ToString();
                    ll.txtDiagnosis = rr["diagnosis"].ToString();
                    ll.txtDateTime = rr["orderdatetime"].ToString();

                }


                return ll;
            }
            catch (Exception e) { ll.ErrMsg = e.Message; return ll; }

        }

        public static string Saved(MedErrHeader hh, User uu)
        {
            int MastOrderID = 0;
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    string Strsql = "Insert Into O_MedicalError(Registrationno,Issueauthoritycode,diagnosis,reportdate,eventdatetime,eventdisdatetime,"
                        + " operatorid,orderdatetime) "
                        + " values (" + MainFunction.getRegNumber(hh.txtPinNo) + ",'" + MainFunction.getAuthorityCode(hh.txtPinNo)
                        + "','" + hh.txtDiagnosis + "','" + hh.RptDate + "','" + hh.DtpErrDate + "',"
                        + "'" + hh.DtpDateDiscover + "'," + uu.EmpID + ",sysdatetime()  ) ";

                    bool inser = MainFunction.SSqlExcuite(Strsql, Trans);

                    MastOrderID = MainFunction.GetOneVal("select max(id) as maxid from O_MedicalError", "maxid", Trans);
                    string Rs = "";
                    Rs = SaveCmbTxt(MastOrderID, 4, hh.lstErrType, hh.txtErrType, Trans);
                    if (hh.UsedByPatient == true)
                    {
                        Rs = SaveCmbTxt(MastOrderID, 5, 1, "", Trans);
                    }
                    else
                    {
                        Rs = SaveCmbTxt(MastOrderID, 5, 0, "", Trans);
                    }
                    if (string.IsNullOrEmpty(hh.lstStage) == false)
                    {
                        Rs = SaveList(MastOrderID, 6, hh.lstStage.Split(',').ToList(), "Stage", Trans);
                    }
                    Rs = SaveCmbTxt(MastOrderID, 7, -1, hh.txtErrDescription, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 8, hh.lstOutcome, hh.txtOutcome, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 9, -1, hh.txtIntervention, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 10, Convert.ToInt32(hh.txtStrength), hh.txtGenericName, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 11, hh.lstDosage1, hh.txtDosage1, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 12, hh.lstRoute, hh.txtRoute, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 13, hh.lstDosage2, hh.txtDosage2, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 14, hh.lstErrMadeBy, hh.txtErrMadeBy, Trans);

                    if (hh.isoptErrPerputed == true)
                    {
                        Rs = SaveCmbTxt(MastOrderID, 15, 1, hh.txtErrPerputed, Trans);
                    }
                    else
                    {
                        Rs = SaveCmbTxt(MastOrderID, 15, 0, hh.txtErrPerputed, Trans);
                    }
                    Rs = SaveCmbTxt(MastOrderID, 16, hh.lstErrDiscoverBy, hh.txtErrDiscoverBy, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 18, -1, hh.txtHowDiscovered, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 19, -1, hh.txtAction, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 21, hh.lstInitialErr, hh.txtInitialErr, Trans);
                    if (string.IsNullOrEmpty(hh.lstCauses) == false)
                    {
                        Rs = SaveList(MastOrderID, 22, hh.lstCauses.Split(',').ToList(), "Causes", Trans);
                    }
                    Rs = SaveCmbTxt(MastOrderID, 23, hh.lstErrAction, hh.txtErrAction, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 24, -1, hh.txtRecommendation, Trans);



                    Trans.Commit();
                }

                return " Report No. " + MastOrderID + " saved successfully";
            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                return "Error: " + e.Message;
            }


        }
        public static string Update(MedErrHeader hh, User uu, int MastOrderID)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    string Strsql = "update O_MedicalError set diagnosis='" + hh.txtDiagnosis + "' "
                    + " ,eventdatetime='" + hh.DtpErrDate + "',eventdisdatetime='" + hh.DtpDateDiscover + "' "
                    + ",operatorid=" + uu.EmpID + ",orderdatetime=sysdatetime()  where id=" + MastOrderID;

                    bool update = MainFunction.SSqlExcuite(Strsql, Trans);



                    bool DelDetail = MainFunction.SSqlExcuite("delete from d_medicalerror where recordid=" + MastOrderID, Trans);



                    string Rs = "";
                    Rs = SaveCmbTxt(MastOrderID, 4, hh.lstErrType, hh.txtErrType, Trans);
                    if (hh.UsedByPatient == true)
                    {
                        Rs = SaveCmbTxt(MastOrderID, 5, 1, "", Trans);
                    }
                    else
                    {
                        Rs = SaveCmbTxt(MastOrderID, 5, 0, "", Trans);
                    }

                    Rs = SaveList(MastOrderID, 6, hh.lstStage.Split(',').ToList(), "Stage", Trans);
                    Rs = SaveCmbTxt(MastOrderID, 7, -1, hh.txtErrDescription, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 8, hh.lstOutcome, hh.txtOutcome, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 9, -1, hh.txtIntervention, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 10, Convert.ToInt32(hh.txtStrength), hh.txtGenericName, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 11, hh.lstDosage1, hh.txtDosage1, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 12, hh.lstRoute, hh.txtRoute, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 13, hh.lstDosage2, hh.txtDosage2, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 14, hh.lstErrMadeBy, hh.txtErrMadeBy, Trans);

                    if (hh.isoptErrPerputed == true)
                    {
                        Rs = SaveCmbTxt(MastOrderID, 15, 1, hh.txtErrPerputed, Trans);
                    }
                    else
                    {
                        Rs = SaveCmbTxt(MastOrderID, 15, 0, hh.txtErrPerputed, Trans);
                    }
                    Rs = SaveCmbTxt(MastOrderID, 16, hh.lstErrDiscoverBy, hh.txtErrDiscoverBy, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 18, -1, hh.txtHowDiscovered, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 19, -1, hh.txtAction, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 21, hh.lstInitialErr, hh.txtInitialErr, Trans);

                    Rs = SaveList(MastOrderID, 22, hh.lstCauses.Split(',').ToList(), "Causes", Trans);
                    Rs = SaveCmbTxt(MastOrderID, 23, hh.lstErrAction, hh.txtErrAction, Trans);
                    Rs = SaveCmbTxt(MastOrderID, 24, -1, hh.txtRecommendation, Trans);



                    Trans.Commit();
                }

                return " Report No. " + MastOrderID + " Updated successfully";
            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                return "Error: " + e.Message;
            }


        }

        public static string SaveCmbTxt(int ReportID, int LevelID, int cmbValue, string cmbTxt, SqlTransaction Trans)
        {
            string str = "";
            str = "Insert into D_MedicalError(Recordid,serviceid,id,description) values "
                + "(" + ReportID + "," + LevelID + "," + cmbValue + ",'" + cmbTxt + "') ";
            bool Inst = MainFunction.SSqlExcuite(str, Trans);

            return "";
        }

        public static string SaveList(int ReportID, int LevelID, List<string> Lst, string LstName, SqlTransaction Trans)
        {

            if (LstName == "Causes")
            {
                foreach (var item in Lst)
                {

                    string str = "Insert into D_MedicalError(Recordid,serviceid,id,description) values "
                        + "(" + ReportID + "," + LevelID + "," + item + ",'" +
                        MainFunction.GetName("select Name from medical_causeoferror where deleted=0 and ID=" + item, "Name", Trans)
                        + "') ";
                    bool Inst = MainFunction.SSqlExcuite(str, Trans);
                }
            }
            else              {
                foreach (var item in Lst)
                {
                    string txt = "";
                                                                                   switch (item)
                    {
                        case "0": txt = "Physician Ordering"; break;
                        case "1": txt = "Despensing And Delivery"; break;
                        case "2": txt = "Monitoring(Level/Allergy/Drug-Food/Clinical)"; break;
                    }


                    string str = "Insert into D_MedicalError(Recordid,serviceid,id,description) values "
                        + "(" + ReportID + "," + LevelID + "," + item + ",'" + txt + "') ";
                    bool Inst = MainFunction.SSqlExcuite(str, Trans);
                }


            }
            return "";
        }




    }

}






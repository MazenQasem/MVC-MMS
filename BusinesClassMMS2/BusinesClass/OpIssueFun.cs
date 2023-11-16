using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;


namespace MMS2
{
    public class OpIssueFun
    {
        public static Patient OpenClearPatient(String gICODE, String OpreatorName, string ErrMsg = "", bool clearflag = false)
        {
            Patient pt = new Patient();
            pt.OrgnizationIssueCode = gICODE + ".";
            pt.OperatorName = OpreatorName;
            if (ErrMsg.Length > 0)
            {
                pt.ErrMsg = ErrMsg;
            }
            if (clearflag == true)
            {
                pt.PatientNotFound = true;
            }

            return pt;
        }
        public static Patient GetPatientDetails(String PIN, String OpertorName, User UserInfo, int DocID = 0)
        {
            try
            {
                Patient PT = new Patient();
                PT.Registrationno = int.Parse(MainFunction.getRegNumber(PIN));
                PT.IssueAuthorityCode = MainFunction.getAuthorityCode(PIN);
                PT.OperatorName = OpertorName;

                
                string pinstatus = "";
                String StrSql = "Select Isnull(Message,'') as Reason from PinBlock "
                   + " where blocked=1 and RegistrationNo =" + PT.Registrationno;

                DataSet AlertDst = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in AlertDst.Tables[0].Rows)
                {
                    pinstatus = "";
                    pinstatus = PromptPinStatus(PT.Registrationno);
                    if (pinstatus.Length > 0)
                    {
                        PT.ErrMsg = pinstatus + ", Reason:" + rr["Reason"].ToString();
                    }
                    else
                    {
                        PT.ErrMsg = "Patient's PIN is Blocked, Reson: " + rr["Reason"].ToString();
                        PT.PatientNotFound = true;
                        return OpenClearPatient(PT.IssueAuthorityCode, OpertorName, PT.ErrMsg, true);

                    }
                }


                if (DisplayPatientDetails(ref PT, OpertorName, PIN) == true)
                {
                    PT.CheckEmpPin = MainFunction.CheckEmployee(PT.Registrationno, PT.IssueAuthorityCode);
                    PT.PHalerts = GetPHAlerts(PT.Registrationno);
                    GetLatestConsultation(ref PT, DocID, UserInfo);


                }

                                 if (PT.CategoryId == 1 && PT.CompanyId == 1) { return CashBillControl(PT); }
                if (string.IsNullOrEmpty(PT.ErrMsg) == false) { return PT; }

                return PT;
            }
            catch (Exception e)
            {
                Patient Pt2 = new Patient();
                Pt2 = OpenClearPatient(MainFunction.getAuthorityCode(PIN), OpertorName);
                Pt2.ErrMsg = "PIN Number not found, Check your Entry!";
                Pt2.PatientNotFound = true;
                return Pt2;
            }

        }
        public static bool DisplayPatientDetails(ref Patient PT, String OpertorName, String PIN)
        {
            try
            {
                

                string StrSql = "Select o.vip,(lTrim(isnull(O.Title, '')) + ' ' + lTrim(isnull(O.FamilyName, '')) + ' ' + lTrim(isnull(O.FirstName, '')) "
                    + " + ' ' + lTrim(isnull(O.MiddleName, '')) + ' ' + lTrim(isnull(O.LastName, ''))) as Name, "
                    + " O.Sex,O.Age,O.GRADEID,o.agetype,o.categoryid,o.companyid from Patient O "
                    + " where O.RegistrationNo=" + PT.Registrationno + " and o.issueauthoritycode='" + PT.IssueAuthorityCode + "'";
                DataSet PTDST = MainFunction.SDataSet(StrSql, "tbl");
                if (PTDST.Tables[0].Rows.Count <= 0)
                {
                    PT = OpenClearPatient(MainFunction.getAuthorityCode(PIN), OpertorName);
                    PT.ErrMsg = "PIN Number not found, Check your Entry!";
                    PT.PatientNotFound = true;
                    return false;
                }

                foreach (DataRow rr in PTDST.Tables[0].Rows)
                {
                    PT.PatientName = rr["Name"].ToString();
                    PT.Vip = MainFunction.NullToBool(rr["vip"].ToString());
                    PT.Sex = (short)rr["Sex"];
                    PT.SexTitle = MainFunction.GetName("select Name from sex where id=" + PT.Sex, "Name");
                    PT.Age = (short)rr["Age"];
                    PT.Agetype = (short)rr["agetype"];
                    PT.AgeTitle = rr["Age"].ToString() + ' ' + MainFunction.GetName("select Name from Agetype where id=" + PT.Agetype, "Name");
                    PT.GradeId = (int)rr["GRADEID"];
                    PT.CompanyId = (int)rr["companyid"];
                    PT.CategoryId = (int)rr["categoryid"];


                }

                PT.Allergey = new List<TempListMdl>();
                
                StrSql = "select Name as Name from M_Generic a,patientdrugallergies b where b.watdrugid=a.ID and b.registrationno=" + PT.Registrationno + " and b.issueauthoritycode='" + PT.IssueAuthorityCode + "'";
                DataSet nn = MainFunction.SDataSet(StrSql, "tbl");
                int i = 1;
                foreach (DataRow rr in nn.Tables[0].Rows)
                {
                    TempListMdl tmp = new TempListMdl();
                    tmp.ID = i;
                    tmp.Name = rr["Name"].ToString();
                    PT.Allergey.Add(tmp);
                    ++i;
                }

                i = 1;
                
                PT.OtherAllergey = new List<TempListMdl>();
                StrSql = "select OtherAllergies as Name from patient where  registrationno=" + PT.Registrationno + " and issueauthoritycode='" + PT.IssueAuthorityCode + "'";
                DataSet xx = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in xx.Tables[0].Rows)
                {
                    TempListMdl tmp = new TempListMdl();
                    tmp.ID = i;
                    tmp.Name = rr["Name"].ToString();
                    PT.OtherAllergey.Add(tmp);
                    ++i;
                }


                return true;

            }
            catch (Exception e) { PT.ErrMsg = "DisplayPatientDetails loadin Error:" + e.Message; return false; }


        }
        public static Patient GetLatestConsultation(ref Patient PT, int DocID, User UserInfo)
        {
            try
            {
                CashDiscount(ref PT);

                PT.DoctorId = 0;
                if (DocID > 0) { PT.DoctorId = DocID; }
                else
                {
                    
                    String StrSql = "SELECT top 1 DoctorId from OpCompanybillDetail where ServiceId=2 and "
                            + " RegistrationNo = " + PT.Registrationno + " And IssueAuthorityCode = '"
                            + PT.IssueAuthorityCode + "' order by billdatetime desc";
                    DataSet DocVisitDst = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow rr in DocVisitDst.Tables[0].Rows)
                    {
                        PT.DoctorId = (int)rr["DoctorId"];
                        CashDiscount(ref PT);
                        if (UserInfo.gHospitalBranchCode == "110")
                        {
                            GetDetails(ref PT);
                        }
                        else if (UserInfo.gHospitalBranchCode == "120")
                        {
                            GetDetailsAseer(ref PT);
                        }
                        else if (UserInfo.gHospitalBranchCode == "170")
                        {
                            GetDetailsCairo(ref PT);
                        }
                        else if (UserInfo.gHospitalBranchCode == "180")                         {
                            GetDetailsAseer(ref PT);
                        }
                    }
                }
                return PT;

            }
            catch (Exception e) { PT.ErrMsg = "Error loading Doctor Visit data: " + e.Message; return PT; }

        }
        public static Patient CashDiscount(ref Patient PT)
        {

            

            String StrSql = "Select top 1 DiscountId,Reason,IsNull(DoctorId,0)DoctorId,Operatorid  from OPDiscountApproval "
                + "where Deleted=0 and RegNo =" + PT.Registrationno + " and IACode ='" + PT.IssueAuthorityCode
                + "' and VisitEndDate >='" + DateTime.Now.Date + "' order by datetime desc";
            DataSet Discountdst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in Discountdst.Tables[0].Rows)
            {
                if ((int)rr["DoctorId"] == 0)
                {
                    PT.disAuthoriseID = (int)rr["Operatorid"];
                    PT.DiscountTypeID = (int)rr["DiscountId"];
                    PT.DisReason = rr["Reason"].ToString();
                }
                else if ((int)rr["DoctorId"] == PT.DoctorId)
                {
                    PT.DiscountTypeID = (int)rr["DiscountId"];
                    PT.DisReason = rr["Reason"].ToString();
                    PT.disAuthoriseID = (int)rr["Operatorid"];
                }
            }
            return PT;

        }
        
        public static Patient GetDetails(ref Patient PT)
        {
            if (PT.DoctorId == 0) { return PT; }

            int mGradeid = 0, mCompanyid = 1, mCategoryid = 1, intCheck = 0;
            string StrSql = "";
            
                         PT.mDeptId = MainFunction.GetID("select departmentid from employee where id = " + PT.DoctorId, "departmentid");

            if (PT.Registrationno == 0)
            {
                                 return CashBillControl(PT);
            }

            
            StrSql = "select 0 as authorityid, a.categoryid,a.companyid,a.gradeid,'' as LoaExpiryDate "
            + " from Patient a where  a.IssueAuthorityCode = '" + PT.IssueAuthorityCode + "'"
            + " and a.registrationno = " + PT.Registrationno;
            DataSet LOAdstCheck = MainFunction.SDataSet(StrSql, "tbl");
            if (LOAdstCheck.Tables[0].Rows.Count == 0)
            {
                PT.ErrMsg = "Patient details not found.";
                return PT;
            }
            else
            {
                foreach (DataRow rr in LOAdstCheck.Tables[0].Rows)
                {
                    mGradeid = (int)rr["gradeid"];
                    mCompanyid = (int)rr["companyid"];
                    mCategoryid = (int)rr["categoryid"];

                    PT.GradeId = (int)rr["gradeid"];
                    PT.CompanyId = (int)rr["companyid"];
                    PT.CategoryId = (int)rr["categoryid"];
                }
            }
            
            StrSql = "SELECT ID FROM GRADE WHERE ID=" + PT.GradeId
            + " AND COMPANYID=" + PT.CompanyId + " AND CATEGORYID=" + PT.CategoryId;
            DataSet GradeDst = MainFunction.SDataSet(StrSql, "tbl");
            if (GradeDst.Tables[0].Rows.Count == 0)
            {
                PT.ErrMsg = "Patient details not found.";
                return PT;
            }

            PT.CompanyName = MainFunction.GetName("select code + ' - ' + name as Name from company where id=" + PT.CompanyId, "Name");
            PT.CategoryName = MainFunction.GetName("select code + ' - ' + name as Name from category where id=" + PT.CategoryId, "Name");
            if (PT.CategoryId == 1 && PT.CompanyId == 1)
            {
                                 return CashBillControl(PT);
            }
            else if (PT.CategoryId > 1 && PT.CompanyId > 1)
            {
                PT.mLOAConsultation = CheckLOAConsultation(int.Parse(PT.CompanyId.ToString()));
                if (PT.mLOAConsultation > 0)
                {
                    PT.mAuthorityid = GetAuthorityId(PT.Registrationno, PT.IssueAuthorityCode, int.Parse(PT.CategoryId.ToString())
                        , int.Parse(PT.CompanyId.ToString()), int.Parse(PT.GradeId.ToString()), int.Parse(PT.DoctorId.ToString()));

                    if (PT.mAuthorityid == 0)
                    {
                                                                      }

                }
                else
                {
                    PT.mAuthorityid = GetAuthorityId(PT.Registrationno, PT.IssueAuthorityCode, int.Parse(PT.CategoryId.ToString())
                           , int.Parse(PT.CompanyId.ToString()), int.Parse(PT.GradeId.ToString()), int.Parse(PT.DoctorId.ToString()));
                    if (PT.mAuthorityid == 0)
                    {
                        PT.mAuthorityid = -1;
                    }

                }


            }

            
            if (PT.DoctorId > 0)
            {
                StrSql = "Select regno  from "
                + "  ARReleaseExclusions "
                + " where regno=" + PT.Registrationno + " and opbillid>0 and serviceid=2 and itemid= " + PT.DoctorId
                + " union all "
                + " select regno from ARReleaseExclusions "
                + " where  Deptid = " + PT.mDeptId + "  and OpBillId>0  ";
                DataSet ArReldst = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in ArReldst.Tables[0].Rows)
                {
                    if ((int)rr["regno"] == 0)
                    {
                        PT.ErrMsg = "Exclusions has been blocked Check With AR department";
                                                 return CashBillControl(PT);
                    }

                }
            }

            
            StrSql = "select count(*) as ct1 from Category "
                   + " where active = 0 and deleted =0 and DateDiff(d,CAST(GETDATE() AS DATE),CAST(VALIDTILL AS DATE))>=0 "
                   + " and id = " + PT.CategoryId;
            DataSet ChkCategorydst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ChkCategorydst.Tables[0].Rows)
            {
                if ((int)rr["ct1"] == 0)
                {
                    PT.ErrMsg = "Category has been blocked";
                                         return CashBillControl(PT);
                }

            }

            
            StrSql = "select count(*) as ct1 from company "
                   + " where active = 0 and deleted =0 and DateDiff(d,CAST(GETDATE() AS DATE),CAST(VALIDTILL AS DATE))>=0 "
                   + " and id = " + PT.CompanyId;
            DataSet ChkCompanydst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ChkCompanydst.Tables[0].Rows)
            {
                if ((int)rr["ct1"] == 0)
                {
                    PT.ErrMsg = "Company has been blocked";
                                         return CashBillControl(PT);
                }

            }

            if (PT.CategoryId == 35) { GetGosiPatient(ref PT); }

                         PT.BillisCredit = true;             PT.GradeId = mGradeid;
            PT.CategoryId = mCategoryid;


            StrSql = "Select count(*) as Cnt from "
            + " ArApprovalOrder a, ArApprovalOrderDetails b where "
            + " a.id=b.OrderNo and a.OPIP=0 and a.AuthorityNo =" + PT.mAuthorityid
            + " and serviceid= 11 and "
            + " ApprovalId = 1 And Billed = 0 "
            + " and DateDiff(d,getdate(),b.validdatetime)>0 ";
            DataSet ArApproDst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ArApproDst.Tables[0].Rows)
            {
                if ((int)rr["Cnt"] > 0)
                {
                    intCheck = 1;
                }
                else
                {
                    StrSql = "Select count(*) as Cnt  from "
                + "  ARReleaseExclusions "
                + " where  Deptid = " + PT.mDeptId + "  and OpBillId=0  "
                + " and RegNo=" + PT.Registrationno + " and IACode ='" + PT.IssueAuthorityCode + "'";
                    DataSet Schck = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow nnn in Schck.Tables[0].Rows)
                    {
                        if ((int)nnn["Cnt"] > 0)
                        {
                            intCheck = 1;
                        }
                        else
                        {
                            intCheck = 0;
                        }

                    }
                }
            }

            if (intCheck == 0)
            {
                StrSql = "select count(*) as c1 from opcompanydeptservices "
                    + " where companyid = " + PT.CompanyId
                    + " and categoryid = " + PT.CategoryId
                    + " and gradeid = " + PT.GradeId + " and serviceid = 11 and departmentid = " + PT.mDeptId;
                DataSet ns = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow vv in ns.Tables[0].Rows)
                {
                    if ((int)vv["c1"] > 0)
                    {
                        PT.ErrMsg = "This Service for the Department is excluded";
                        PT.BillisCredit = false;

                    }
                    else
                    {
                        StrSql = "select count(*) as c1 from opcompanyservices "
                        + " where categoryid = " + PT.CategoryId
                        + " and companyid = " + PT.CompanyId
                        + " and gradeid = " + PT.GradeId + " and serviceid = 11";

                        DataSet itmdst = MainFunction.SDataSet(StrSql, "tbl");
                        foreach (DataRow ii in itmdst.Tables[0].Rows)
                        {
                            if ((int)ii["c1"] > 0)
                            {
                                PT.ErrMsg = "This Service is excluded";
                                                                 return CashBillControl(PT);
                            }
                        }
                    }
                }
            }

            if (PT.CategoryId != 43)
            {
                PT.gDedPerType = 0;
                StrSql = "select deductable,isnull(percentage,0) percentage,isnull(amount,0) amount from opcompanydeptdeductable "
                + " where categoryid = " + PT.CategoryId
                + " and companyid = " + PT.CompanyId
                + " and gradeid = " + PT.GradeId + " and serviceid = 11 and "
                + " departmentid = " + PT.mDeptId;
                DataSet depdud = MainFunction.SDataSet(StrSql, "tbl");
                if (depdud.Tables[0].Rows.Count == 0)
                {
                    StrSql = "select deductable,percentage,amount from "
                    + " opcompanygradedeductable where categoryid = " + PT.CategoryId
                    + " and companyid = " + PT.CompanyId
                    + " and gradeid = " + PT.GradeId
                     + " and serviceid = 11";
                    DataSet tempDed = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow dd in tempDed.Tables[0].Rows)
                    {
                        if (decimal.Parse(dd["amount"].ToString()) > 0)
                        {
                            PT.mDeductper = (decimal)dd["amount"];
                            PT.gDedPerType = 2;
                        }
                        else
                        {
                            PT.mDeductper = (decimal)dd["percentage"];
                            PT.gDedPerType = 1;
                        }

                    }

                }
                else
                {
                    foreach (DataRow dd in depdud.Tables[0].Rows)
                    {
                        if ((int)dd["amount"] > 0)
                        {
                            PT.mDeductper = (decimal)dd["amount"];
                            PT.gDedPerType = 2;
                        }
                        else
                        {
                            PT.mDeductper = (decimal)dd["percentage"];
                            PT.gDedPerType = 1;
                        }

                    }
                }

                StrSql = "select  Deductable from IPDeductableType where IPOPType=1 AND "
                       + " categoryid = " + PT.CategoryId + " and gradeid = " + PT.GradeId + " and "
                       + " companyid = " + PT.CompanyId;
                DataSet ipded = MainFunction.SDataSet(StrSql, "tbl");
                if (ipded.Tables[0].Rows.Count == 0)
                {
                    PT.mDeducttype = 0;
                }
                else
                {
                    foreach (DataRow rr in ipded.Tables[0].Rows)
                    {
                        if ((int)rr["Deductable"] == -1)
                        { PT.mDeducttype = 0; }
                        else { PT.mDeducttype = (int)rr["Deductable"]; }

                    }
                }

            }
            else if (PT.CategoryId == 43)
            {
                StrSql = "Select A.OPPHDeduct, A.OPPHDeductType, A.DedType, A.OPPHAmtLmt from StaffHealthAdmin A, GRADE B "
                    + " where A.Code = B.NAME and a.serviceid = 11 AND B.ID = " + PT.GradeId;
                DataSet ca = MainFunction.SDataSet(StrSql, "tbl");
                if (ca.Tables[0].Rows.Count == 0)
                {

                }
                else
                {
                    foreach (DataRow rr in ca.Tables[0].Rows)
                    {
                        PT.gOPPHAmtLmt = (long)rr["OPPHAmtLmt"];
                        if ((int)rr["OpPHDeductType"] == 1)
                        {
                            PT.mDeductper = (int)rr["OpPHDeductType"];
                            PT.gDedPerType = 1;                          }
                        else
                        {
                            PT.mDeductper = (int)rr["OpPHDeductType"];
                            PT.gDedPerType = 2;                          }
                        PT.mDeducttype = (int)rr["DedType"];
                    }
                }

                if (CheckPharmacyLimits(ref PT) == false)
                {
                    PT.ErrMsg = "Pharmacy LOA Limits not defined. Contact AR to define LOA Limits.";
                                         return CashBillControl(PT);
                }

            }


            if (PT.mDeducttype == 0)
            {
                if (PT.mDeductper > 0) { PT.lblDeductableText += PT.mDeductper.ToString(); }
                else { PT.lblDeductableText += "0%"; }
                if (PT.gDedPerType == 1) { PT.lblDeductableText += "%"; }
                else if (PT.gDedPerType == 2) { PT.lblDeductableText += "SR"; }
                PT.lblDeductableText += "Deductable After Discount";
            }
            else if (PT.mDeducttype == 1)
            {
                if (PT.mDeductper > 0) { PT.lblDeductableText += PT.mDeductper.ToString(); }
                else { PT.lblDeductableText += "0%"; }
                if (PT.gDedPerType == 1) { PT.lblDeductableText += "%"; }
                else if (PT.gDedPerType == 2) { PT.lblDeductableText += "SR"; }
                PT.lblDeductableText += "Deductable Before Discount";
            }


            if (PT.mLOAConsultation > 0 && PT.mAuthorityid > 0)
            {
                if (CheckLOAAmount(ref PT, 0) == false)
                {
                    if (PT.mApproval == false)
                    {
                        PT.ErrMsg = "LOA Amount Exceeded. Need Approval.";
                        return PT;
                    }
                    else
                    {
                        PT.ErrMsg = "Awaiting For Approval From AR Department";
                        return PT;
                    }

                }

                int xdays = 0; long xloan = 0;
                if (CheckLOADays(ref PT, ref xdays, ref xloan) == false)
                {
                    if (PT.mApproval == false)
                    {
                        PT.ErrMsg = "LOA Days Exceeded.Contact AR Department.LOA Days :  " + xloan + " . Bill days:  " + xdays;
                                                                                                    

                                                                           
                                             }
                    else
                    {
                        PT.ErrMsg = "Awaiting For Approval From AR Department";
                    }
                }



            }

            if (PT.BillisCredit == true)
            {
                                 GetLOAAmt(ref PT);
                                 DisplayCreditDets(ref PT);
            }

            return PT;


        }
        
        public static Patient GetDetailsCairo(ref Patient PT)
        {
            if (PT.DoctorId == 0) { return PT; }

            int mGradeid = 0, mCompanyid = 1, mCategoryid = 1, intCheck = 0;
            string StrSql = "";
            
                         PT.mDeptId = MainFunction.GetID("select departmentid from employee where id = " + PT.DoctorId, "departmentid");

            if (PT.Registrationno == 0)
            {
                                 return CashBillControl(PT);
            }

            
            StrSql = "select 0 as authorityid, a.categoryid,a.companyid,a.gradeid,'' as LoaExpiryDate "
            + " from Patient a where  a.IssueAuthorityCode = '" + PT.IssueAuthorityCode + "'"
            + " and a.registrationno = " + PT.Registrationno;
            DataSet LOAdstCheck = MainFunction.SDataSet(StrSql, "tbl");
            if (LOAdstCheck.Tables[0].Rows.Count == 0)
            {
                PT.ErrMsg = "Patient details not found.";
                return PT;
            }
            else
            {
                foreach (DataRow rr in LOAdstCheck.Tables[0].Rows)
                {
                    mGradeid = (int)rr["gradeid"];
                    mCompanyid = (int)rr["companyid"];
                    mCategoryid = (int)rr["categoryid"];

                    PT.GradeId = (int)rr["gradeid"];
                    PT.CompanyId = (int)rr["companyid"];
                    PT.CategoryId = (int)rr["categoryid"];
                }
            }
            
            StrSql = "SELECT ID FROM GRADE WHERE ID=" + PT.GradeId
            + " AND COMPANYID=" + PT.CompanyId + " AND CATEGORYID=" + PT.CategoryId;
            DataSet GradeDst = MainFunction.SDataSet(StrSql, "tbl");
            if (GradeDst.Tables[0].Rows.Count == 0)
            {
                PT.ErrMsg = "Patient details not found.";
                return PT;
            }

            PT.CompanyName = MainFunction.GetName("select code + ' - ' + name as Name from company where id=" + PT.CompanyId, "Name");
            PT.CategoryName = MainFunction.GetName("select code + ' - ' + name as Name from category where id=" + PT.CategoryId, "Name");
            if (PT.CategoryId == 1 && PT.CompanyId == 1)
            {
                                 return CashBillControl(PT);
            }
            else if (PT.CategoryId > 1 && PT.CompanyId > 1)
            {
                PT.mLOAConsultation = CheckLOAConsultation(int.Parse(PT.CompanyId.ToString()));
                if (PT.mLOAConsultation > 0)
                {
                    PT.mAuthorityid = GetAuthorityId(PT.Registrationno, PT.IssueAuthorityCode, int.Parse(PT.CategoryId.ToString())
                        , int.Parse(PT.CompanyId.ToString()), int.Parse(PT.GradeId.ToString()), int.Parse(PT.DoctorId.ToString()));

                    if (PT.mAuthorityid == 0)
                    {
                                                                      }

                }
                else
                {
                    PT.mAuthorityid = GetAuthorityId(PT.Registrationno, PT.IssueAuthorityCode, int.Parse(PT.CategoryId.ToString())
                           , int.Parse(PT.CompanyId.ToString()), int.Parse(PT.GradeId.ToString()), int.Parse(PT.DoctorId.ToString()));
                    if (PT.mAuthorityid == 0)
                    {
                        PT.mAuthorityid = -1;
                    }

                }


            }

            
            if (PT.DoctorId > 0)
            {
                StrSql = "Select regno  from "
                + "  ARReleaseExclusions "
                + " where regno=" + PT.Registrationno + " and opbillid>0 and serviceid=2 and itemid= " + PT.DoctorId
                + " union all "
                + " select regno from ARReleaseExclusions "
                + " where  Deptid = " + PT.mDeptId + "  and OpBillId>0  ";
                DataSet ArReldst = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in ArReldst.Tables[0].Rows)
                {
                    if ((int)rr["regno"] == 0)
                    {
                        PT.ErrMsg = "Exclusions has been blocked Check With AR department";
                                                 return CashBillControl(PT);
                    }

                }
            }

            
            StrSql = "select count(*) as ct1 from Category "
                   + " where active = 0 and deleted =0 and DateDiff(d,CAST(GETDATE() AS DATE),CAST(VALIDTILL AS DATE))>=0 "
                   + " and id = " + PT.CategoryId;
            DataSet ChkCategorydst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ChkCategorydst.Tables[0].Rows)
            {
                if ((int)rr["ct1"] == 0)
                {
                    PT.ErrMsg = "Category has been blocked";
                                         return CashBillControl(PT);
                }

            }

            
            StrSql = "select count(*) as ct1 from company "
                   + " where active = 0 and deleted =0 and DateDiff(d,CAST(GETDATE() AS DATE),CAST(VALIDTILL AS DATE))>=0 "
                   + " and id = " + PT.CompanyId;
            DataSet ChkCompanydst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ChkCompanydst.Tables[0].Rows)
            {
                if ((int)rr["ct1"] == 0)
                {
                    PT.ErrMsg = "Company has been blocked";
                                         return CashBillControl(PT);
                }

            }

                         PT.BillisCredit = true;             PT.GradeId = mGradeid;
            PT.CategoryId = mCategoryid;


            StrSql = "Select count(*) as Cnt from "
            + " ArApprovalOrder a, ArApprovalOrderDetails b where "
            + " a.id=b.OrderNo and a.OPIP=0 and a.AuthorityNo =" + PT.mAuthorityid
            + " and serviceid= 11 and "
            + " ApprovalId = 1 And Billed = 0 "
            + " and DateDiff(d,getdate(),b.validdatetime)>0 ";
            DataSet ArApproDst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ArApproDst.Tables[0].Rows)
            {
                if ((int)rr["Cnt"] > 0)
                {
                    intCheck = 1;
                }
                else
                {
                    StrSql = "Select count(*) as Cnt  from "
                + "  ARReleaseExclusions "
                + " where  Deptid = " + PT.mDeptId + "  and OpBillId=0  "
                + " and RegNo=" + PT.Registrationno + " and IACode ='" + PT.IssueAuthorityCode + "'";
                    DataSet Schck = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow nnn in Schck.Tables[0].Rows)
                    {
                        if ((int)nnn["Cnt"] > 0)
                        {
                            intCheck = 1;
                        }
                        else
                        {
                            intCheck = 0;
                        }

                    }
                }
            }

            if (intCheck == 0)
            {
                StrSql = "select count(*) as c1 from opcompanydeptservices "
                    + " where companyid = " + PT.CompanyId
                    + " and categoryid = " + PT.CategoryId
                    + " and gradeid = " + PT.GradeId + " and serviceid = 11 and departmentid = " + PT.mDeptId;
                DataSet ns = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow vv in ns.Tables[0].Rows)
                {
                    if ((int)vv["c1"] > 0)
                    {
                        PT.ErrMsg = "This Service for the Department is excluded";
                        PT.BillisCredit = false;

                    }
                    else
                    {
                        StrSql = "select count(*) as c1 from opcompanyservices "
                        + " where categoryid = " + PT.CategoryId
                        + " and companyid = " + PT.CompanyId
                        + " and gradeid = " + PT.GradeId + " and serviceid = 11";

                        DataSet itmdst = MainFunction.SDataSet(StrSql, "tbl");
                        foreach (DataRow ii in itmdst.Tables[0].Rows)
                        {
                            if ((int)ii["c1"] > 0)
                            {
                                PT.ErrMsg = "This Service is excluded";
                                                                 return CashBillControl(PT);
                            }
                        }
                    }
                }
            }


            PT.gDedPerType = 0;
            StrSql = "select deductable,isnull(percentage,0) percentage,isnull(amount,0) amount from opcompanydeptdeductable "
            + " where categoryid = " + PT.CategoryId
            + " and companyid = " + PT.CompanyId
            + " and gradeid = " + PT.GradeId + " and serviceid = 11 and "
            + " departmentid = " + PT.mDeptId;
            DataSet depdud = MainFunction.SDataSet(StrSql, "tbl");
            if (depdud.Tables[0].Rows.Count == 0)
            {
                StrSql = "select deductable,percentage,amount from "
                + " opcompanygradedeductable where categoryid = " + PT.CategoryId
                + " and companyid = " + PT.CompanyId
                + " and gradeid = " + PT.GradeId
                 + " and serviceid = 11";
                DataSet tempDed = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow dd in tempDed.Tables[0].Rows)
                {
                    if (decimal.Parse(dd["amount"].ToString()) > 0)
                    {
                        PT.mDeductper = (decimal)dd["amount"];
                        PT.gDedPerType = 2;
                    }
                    else
                    {
                        PT.mDeductper = (decimal)dd["percentage"];
                        PT.gDedPerType = 1;
                    }

                }

            }
            else
            {
                foreach (DataRow dd in depdud.Tables[0].Rows)
                {
                    if ((int)dd["amount"] > 0)
                    {
                        PT.mDeductper = (decimal)dd["amount"];
                        PT.gDedPerType = 2;
                    }
                    else
                    {
                        PT.mDeductper = (decimal)dd["percentage"];
                        PT.gDedPerType = 1;
                    }

                }
            }

            StrSql = "select  Deductable from IPDeductableType where IPOPType=1 AND "
                   + " categoryid = " + PT.CategoryId + " and gradeid = " + PT.GradeId + " and "
                   + " companyid = " + PT.CompanyId;
            DataSet ipded = MainFunction.SDataSet(StrSql, "tbl");
            if (ipded.Tables[0].Rows.Count == 0)
            {
                PT.mDeducttype = 0;
            }
            else
            {
                foreach (DataRow rr in ipded.Tables[0].Rows)
                {
                    if ((int)rr["Deductable"] == -1)
                    { PT.mDeducttype = 0; }
                    else { PT.mDeducttype = (int)rr["Deductable"]; }

                }
            }




            if (PT.mDeducttype == 0)
            {
                if (PT.mDeductper > 0) { PT.lblDeductableText += PT.mDeductper.ToString(); }
                else { PT.lblDeductableText += "0%"; }
                if (PT.gDedPerType == 1) { PT.lblDeductableText += "%"; }
                else if (PT.gDedPerType == 2) { PT.lblDeductableText += "SR"; }
                PT.lblDeductableText += "Deductable After Discount";
            }
            else if (PT.mDeducttype == 1)
            {
                if (PT.mDeductper > 0) { PT.lblDeductableText += PT.mDeductper.ToString(); }
                else { PT.lblDeductableText += "0%"; }
                if (PT.gDedPerType == 1) { PT.lblDeductableText += "%"; }
                else if (PT.gDedPerType == 2) { PT.lblDeductableText += "SR"; }
                PT.lblDeductableText += "Deductable Before Discount";
            }


            if (PT.mLOAConsultation > 0 && PT.mAuthorityid > 0)
            {
                if (CheckLOAAmount(ref PT, 0) == false)
                {
                    if (PT.mApproval == false)
                    {
                        PT.ErrMsg = "LOA Amount Exceeded. Need Approval.";
                        return PT;
                    }
                    else
                    {
                        PT.ErrMsg = "Awaiting For Approval From AR Department";
                        return PT;
                    }

                }

                int xdays = 0; long xloan = 0;
                if (CheckLOADays(ref PT, ref xdays, ref xloan) == false)
                {
                    if (PT.mApproval == false)
                    {
                        PT.ErrMsg = "LOA Days Exceeded.Contact AR Department.LOA Days :  " + xloan + " . Bill days:  " + xdays;
                                                                                                    

                                                                           
                                             }
                    else
                    {
                        PT.ErrMsg = "Awaiting For Approval From AR Department";
                    }
                }



            }

            if (PT.BillisCredit == true)
            {
                                 GetLOAAmt(ref PT);
                                 DisplayCreditDets(ref PT);
            }

            return PT;


        }
        
        public static Patient GetDetailsAseer(ref Patient PT)
        {
            if (PT.DoctorId == 0) { return PT; }

            int mGradeid = 0, mCompanyid = 1, mCategoryid = 1, intCheck = 0;
            string StrSql = "";
            
                         PT.mDeptId = MainFunction.GetID("select departmentid from employee where id = " + PT.DoctorId, "departmentid");

            if (PT.Registrationno == 0)
            {
                                 return CashBillControl(PT);
            }

            
            StrSql = "select 0 as authorityid, a.categoryid,a.companyid,a.gradeid,'' as LoaExpiryDate "
            + " from Patient a where  a.IssueAuthorityCode = '" + PT.IssueAuthorityCode + "'"
            + " and a.registrationno = " + PT.Registrationno;
            DataSet LOAdstCheck = MainFunction.SDataSet(StrSql, "tbl");
            if (LOAdstCheck.Tables[0].Rows.Count == 0)
            {
                PT.ErrMsg = "Patient details not found.";
                return PT;
            }
            else
            {
                foreach (DataRow rr in LOAdstCheck.Tables[0].Rows)
                {
                    mGradeid = (int)rr["gradeid"];
                    mCompanyid = (int)rr["companyid"];
                    mCategoryid = (int)rr["categoryid"];

                    PT.GradeId = (int)rr["gradeid"];
                    PT.CompanyId = (int)rr["companyid"];
                    PT.CategoryId = (int)rr["categoryid"];
                }
            }
            
            StrSql = "SELECT ID FROM GRADE WHERE ID=" + PT.GradeId
            + " AND COMPANYID=" + PT.CompanyId + " AND CATEGORYID=" + PT.CategoryId;
            DataSet GradeDst = MainFunction.SDataSet(StrSql, "tbl");
            if (GradeDst.Tables[0].Rows.Count == 0)
            {
                PT.ErrMsg = "Patient details not found.";
                return PT;
            }

            PT.CompanyName = MainFunction.GetName("select code + ' - ' + name as Name from company where id=" + PT.CompanyId, "Name");
            PT.CategoryName = MainFunction.GetName("select code + ' - ' + name as Name from category where id=" + PT.CategoryId, "Name");
            if (PT.CategoryId == 1 && PT.CompanyId == 1)
            {
                                 return CashBillControl(PT);
            }
            else if (PT.CategoryId > 1 && PT.CompanyId > 1)
            {
                PT.mLOAConsultation = CheckLOAConsultation(int.Parse(PT.CompanyId.ToString()));
                if (PT.mLOAConsultation > 0)
                {
                    PT.mAuthorityid = GetAuthorityId(PT.Registrationno, PT.IssueAuthorityCode, int.Parse(PT.CategoryId.ToString())
                        , int.Parse(PT.CompanyId.ToString()), int.Parse(PT.GradeId.ToString()), int.Parse(PT.DoctorId.ToString()));

                    if (PT.mAuthorityid == 0)
                    {
                                                                      }

                }
                else
                {
                    PT.mAuthorityid = GetAuthorityId(PT.Registrationno, PT.IssueAuthorityCode, int.Parse(PT.CategoryId.ToString())
                           , int.Parse(PT.CompanyId.ToString()), int.Parse(PT.GradeId.ToString()), int.Parse(PT.DoctorId.ToString()));
                    if (PT.mAuthorityid == 0)
                    {
                        PT.mAuthorityid = -1;
                    }

                }


            }

            
            if (PT.DoctorId > 0)
            {
                StrSql = "Select regno  from "
                + "  ARReleaseExclusions "
                + " where regno=" + PT.Registrationno + " and opbillid>0 and serviceid=2 and itemid= " + PT.DoctorId
                + " union all "
                + " select regno from ARReleaseExclusions "
                + " where  Deptid = " + PT.mDeptId + "  and OpBillId>0  ";
                DataSet ArReldst = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in ArReldst.Tables[0].Rows)
                {
                    if ((int)rr["regno"] == 0)
                    {
                        PT.ErrMsg = "Exclusions has been blocked Check With AR department";
                                                 return CashBillControl(PT);
                    }

                }
            }

            
            StrSql = "select count(*) as ct1 from Category "
                   + " where active = 0 and deleted =0 and DateDiff(d,CAST(GETDATE() AS DATE),CAST(VALIDTILL AS DATE))>=0 "
                   + " and id = " + PT.CategoryId;
            DataSet ChkCategorydst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ChkCategorydst.Tables[0].Rows)
            {
                if ((int)rr["ct1"] == 0)
                {
                    PT.ErrMsg = "Category has been blocked";
                                         return CashBillControl(PT);
                }

            }

            
            StrSql = "select count(*) as ct1 from company "
                   + " where active = 0 and deleted =0 and DateDiff(d,CAST(GETDATE() AS DATE),CAST(VALIDTILL AS DATE))>=0 "
                   + " and id = " + PT.CompanyId;
            DataSet ChkCompanydst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ChkCompanydst.Tables[0].Rows)
            {
                if ((int)rr["ct1"] == 0)
                {
                    PT.ErrMsg = "Company has been blocked";
                                         return CashBillControl(PT);
                }

            }

            if (PT.CategoryId == 30) { GetGosiPatient(ref PT); }

                         PT.BillisCredit = true;             PT.GradeId = mGradeid;
            PT.CategoryId = mCategoryid;


            StrSql = "Select count(*) as Cnt from "
            + " ArApprovalOrder a, ArApprovalOrderDetails b where "
            + " a.id=b.OrderNo and a.OPIP=0 and a.AuthorityNo =" + PT.mAuthorityid
            + " and serviceid= 11 and "
            + " ApprovalId = 1 And Billed = 0 "
            + " and DateDiff(d,getdate(),b.validdatetime)>0 ";
            DataSet ArApproDst = MainFunction.SDataSet(StrSql, "tbl");
            foreach (DataRow rr in ArApproDst.Tables[0].Rows)
            {
                if ((int)rr["Cnt"] > 0)
                {
                    intCheck = 1;
                }
                else
                {
                    StrSql = "Select count(*) as Cnt  from "
                + "  ARReleaseExclusions "
                + " where  Deptid = " + PT.mDeptId + "  and OpBillId=0  "
                + " and RegNo=" + PT.Registrationno + " and IACode ='" + PT.IssueAuthorityCode + "'";
                    DataSet Schck = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow nnn in Schck.Tables[0].Rows)
                    {
                        if ((int)nnn["Cnt"] > 0)
                        {
                            intCheck = 1;
                        }
                        else
                        {
                            intCheck = 0;
                        }

                    }
                }
            }

            if (intCheck == 0)
            {
                StrSql = "select count(*) as c1 from opcompanydeptservices "
                    + " where companyid = " + PT.CompanyId
                    + " and categoryid = " + PT.CategoryId
                    + " and gradeid = " + PT.GradeId + " and serviceid = 11 and departmentid = " + PT.mDeptId;
                DataSet ns = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow vv in ns.Tables[0].Rows)
                {
                    if ((int)vv["c1"] > 0)
                    {
                        PT.ErrMsg = "This Service for the Department is excluded";
                        PT.BillisCredit = false;

                    }
                    else
                    {
                        StrSql = "select count(*) as c1 from opcompanyservices "
                        + " where categoryid = " + PT.CategoryId
                        + " and companyid = " + PT.CompanyId
                        + " and gradeid = " + PT.GradeId + " and serviceid = 11";

                        DataSet itmdst = MainFunction.SDataSet(StrSql, "tbl");
                        foreach (DataRow ii in itmdst.Tables[0].Rows)
                        {
                            if ((int)ii["c1"] > 0)
                            {
                                PT.ErrMsg = "This Service is excluded";
                                                                 return CashBillControl(PT);
                            }
                        }
                    }
                }
            }

            if (PT.CategoryId != 43)
            {
                PT.gDedPerType = 0;
                StrSql = "select deductable,isnull(percentage,0) percentage,isnull(amount,0) amount from opcompanydeptdeductable "
                + " where categoryid = " + PT.CategoryId
                + " and companyid = " + PT.CompanyId
                + " and gradeid = " + PT.GradeId + " and serviceid = 11 and "
                + " departmentid = " + PT.mDeptId;
                DataSet depdud = MainFunction.SDataSet(StrSql, "tbl");
                if (depdud.Tables[0].Rows.Count == 0)
                {
                    StrSql = "select deductable,percentage,amount from "
                    + " opcompanygradedeductable where categoryid = " + PT.CategoryId
                    + " and companyid = " + PT.CompanyId
                    + " and gradeid = " + PT.GradeId
                     + " and serviceid = 11";
                    DataSet tempDed = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow dd in tempDed.Tables[0].Rows)
                    {
                        if (decimal.Parse(dd["amount"].ToString()) > 0)
                        {
                            PT.mDeductper = (decimal)dd["amount"];
                            PT.gDedPerType = 2;
                        }
                        else
                        {
                            PT.mDeductper = (decimal)dd["percentage"];
                            PT.gDedPerType = 1;
                        }

                    }

                }
                else
                {
                    foreach (DataRow dd in depdud.Tables[0].Rows)
                    {
                        if ((int)dd["amount"] > 0)
                        {
                            PT.mDeductper = (decimal)dd["amount"];
                            PT.gDedPerType = 2;
                        }
                        else
                        {
                            PT.mDeductper = (decimal)dd["percentage"];
                            PT.gDedPerType = 1;
                        }

                    }
                }

                StrSql = "select  Deductable from IPDeductableType where IPOPType=1 AND "
                       + " categoryid = " + PT.CategoryId + " and gradeid = " + PT.GradeId + " and "
                       + " companyid = " + PT.CompanyId;
                DataSet ipded = MainFunction.SDataSet(StrSql, "tbl");
                if (ipded.Tables[0].Rows.Count == 0)
                {
                    PT.mDeducttype = 0;
                }
                else
                {
                    foreach (DataRow rr in ipded.Tables[0].Rows)
                    {
                        if ((int)rr["Deductable"] == -1)
                        { PT.mDeducttype = 0; }
                        else { PT.mDeducttype = (int)rr["Deductable"]; }

                    }
                }

            }
            else if (PT.CategoryId == 43)
            {
                StrSql = "Select A.OPPHDeduct, A.OPPHDeductType, A.DedType, A.OPPHAmtLmt from StaffHealthAdmin A, GRADE B "
                    + " where A.Code = B.NAME and a.serviceid = 11 AND B.ID = " + PT.GradeId;
                DataSet ca = MainFunction.SDataSet(StrSql, "tbl");
                if (ca.Tables[0].Rows.Count == 0)
                {

                }
                else
                {
                    foreach (DataRow rr in ca.Tables[0].Rows)
                    {
                        PT.gOPPHAmtLmt = (long)rr["OPPHAmtLmt"];
                        if ((int)rr["OpPHDeductType"] == 1)
                        {
                            PT.mDeductper = (int)rr["OpPHDeductType"];
                            PT.gDedPerType = 1;                          }
                        else
                        {
                            PT.mDeductper = (int)rr["OpPHDeductType"];
                            PT.gDedPerType = 2;                          }
                        PT.mDeducttype = (int)rr["DedType"];
                    }
                }

                if (CheckPharmacyLimits(ref PT) == false)
                {
                    PT.ErrMsg = "Pharmacy LOA Limits not defined. Contact AR to define LOA Limits.";
                                         return CashBillControl(PT);
                }

            }


            if (PT.mDeducttype == 0)
            {
                if (PT.mDeductper > 0) { PT.lblDeductableText += PT.mDeductper.ToString(); }
                else { PT.lblDeductableText += "0%"; }
                if (PT.gDedPerType == 1) { PT.lblDeductableText += "%"; }
                else if (PT.gDedPerType == 2) { PT.lblDeductableText += "SR"; }
                PT.lblDeductableText += "Deductable After Discount";
            }
            else if (PT.mDeducttype == 1)
            {
                if (PT.mDeductper > 0) { PT.lblDeductableText += PT.mDeductper.ToString(); }
                else { PT.lblDeductableText += "0%"; }
                if (PT.gDedPerType == 1) { PT.lblDeductableText += "%"; }
                else if (PT.gDedPerType == 2) { PT.lblDeductableText += "SR"; }
                PT.lblDeductableText += "Deductable Before Discount";
            }


            if (PT.mLOAConsultation > 0 && PT.mAuthorityid > 0)
            {
                if (CheckLOAAmount(ref PT, 0) == false)
                {
                    if (PT.mApproval == false)
                    {
                        PT.ErrMsg = "LOA Amount Exceeded. Need Approval.";
                        return PT;
                    }
                    else
                    {
                        PT.ErrMsg = "Awaiting For Approval From AR Department";
                        return PT;
                    }

                }

                int xdays = 0; long xloan = 0;
                if (CheckLOADays(ref PT, ref xdays, ref xloan) == false)
                {
                    if (PT.mApproval == false)
                    {
                        PT.ErrMsg = "LOA Days Exceeded.Contact AR Department.LOA Days :  " + xloan + " . Bill days:  " + xdays;
                                                                                                    

                                                                           
                                             }
                    else
                    {
                        PT.ErrMsg = "Awaiting For Approval From AR Department";
                    }
                }



            }

            if (PT.BillisCredit == true)
            {
                                 GetLOAAmt(ref PT);
                                 DisplayCreditDets(ref PT);
            }

            return PT;


        }



        public static Patient GetGosiPatient(ref Patient pt)
        {
            try
            {
                String StrSql = "Select * from Patient where IssueAuthorityCode = '" + pt.IssueAuthorityCode + "'"
                    + " and RegistrationNo = " + pt.Registrationno
                    + " and CompanyId = " + pt.CompanyId
                    + " and CategoryId = " + pt.CategoryId;
                DataSet Gosidst = MainFunction.SDataSet(StrSql, "tbl");
                if (Gosidst.Tables[0].Rows.Count == 0)
                {

                                         return CashBillControl(pt);
                }
                else
                {
                    foreach (DataRow rr in Gosidst.Tables[0].Rows)
                    {
                        StrSql = "Select a.id,a.name,a.active,a.InsurenceNo from CompanyInsurenceInformation a, ClaimFileId b"
                                + " where a.categoryid = b.categoryid and a.companyid = b.companyid and b.regno = " + pt.Registrationno + " and "
                                + " a.CategoryId = " + pt.CategoryId
                                + " and a.CompanyId = " + pt.CompanyId
                                + " and a.InsurenceNo = b.InsuranceNo";
                        DataSet Climdst = MainFunction.SDataSet(StrSql, "tbl");
                        if (Climdst.Tables[0].Rows.Count == 0)
                        {
                            StrSql = "Select RegDateTime, GetDate() as DateTime from Patient "
                            + " where IssueAuthorityCode = '" + pt.IssueAuthorityCode
                            + "' and registrationno = " + pt.Registrationno;
                            DataSet ptdest = MainFunction.SDataSet(StrSql, "tbl");
                            foreach (DataRow xx in ptdest.Tables[0].Rows)
                            {
                                TimeSpan tm = DateTime.Parse(xx["DateTime"].ToString()) - DateTime.Parse(xx["RegDateTime"].ToString());
                                if (tm.TotalDays == 0)
                                {
                                    pt.ErrMsg = "GOSI Insurance for this PIN is not defined. Contact AR Department.  Still do you want to Generate Bill for this Patient.";
                                                                         return CashBillControl(pt);
                                }
                                else if (tm.TotalDays > 0)
                                {
                                    pt.ErrMsg = "GOSI Insurance for this PIN is not defined. Contact AR Department.";
                                                                         return CashBillControl(pt);
                                }
                                pt.CompanyId = 1;
                                pt.CategoryId = 1;

                            }
                        }
                        else
                        {
                            foreach (DataRow tt in Climdst.Tables[0].Rows)
                            {
                                                                 if ((bool)tt["active"] == true)
                                {
                                    pt.ErrMsg = "GOSI Insurance of this PIN is Blocked. Contact AR Department. Still do you want to Generate Bill for this Patient.";
                                                                         return CashBillControl(pt);
                                }

                            }

                        }

                    }
                }

                return pt;
            }
            catch (Exception e) { pt.ErrMsg = "CheckPharmacyLimits : " + e.Message; return pt; }

        }





        public static Patient DisplayCreditDets(ref Patient pt)
        {
            try
            {
                string StrSql = "Select PolicyNo,IDExpiryDate,MedIDNumber from Patient p with (nolock) where registrationno=" + pt.Registrationno
                + " and IssueAuthorityCode ='" + pt.IssueAuthorityCode + "'";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rsData in n.Tables[0].Rows)
                {
                    pt.TxtLOAletterno = rsData["PolicyNo"].ToString() + "";
                    pt.TxtInsCardno = rsData["MedIDNumber"].ToString() + "";
                    pt.lblIdExpiry = string.IsNullOrEmpty(rsData["IDExpiryDate"].ToString()) ? DateTime.Today.Date.ToString("dd-MMM-yyyy") :
                                     MainFunction.DateFormat(rsData["IDExpiryDate"].ToString(), "dd", "MMM", "yyyy");
                    pt.TxtCompanyRemarks = "";
                }
                StrSql = "Select * from CompanyMessages where CategoryId =" + pt.CategoryId
                    + " and CompanyId =" + pt.CompanyId;
                DataSet dn = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rsData in dn.Tables[0].Rows)
                {
                    pt.TxtCompanyRemarks = "";
                    pt.TxtCompanyRemarks = "( (1) )" + rsData["Message1"].ToString() + "; **** ;" +
                        "( (2) )" + rsData["Message2"].ToString() + "; **** ;" +
                        "( (3) )" + rsData["Message3"].ToString() + "; **** ;" +
                        "( (4) )" + rsData["Message4"].ToString() + "; **** ;" +
                        "( (5) )" + rsData["Message5"].ToString() + "; **** ;" +
                        "( (6) )" + rsData["Message6"].ToString() + "; **** ;" +
                        "( (7) )" + rsData["Message7"].ToString();
                }
                return pt;
            }
            catch (Exception e) { pt.ErrMsg = "DisplayPolicy: " + e.Message; return pt; }

        }
        public static Patient GetLOAAmt(ref Patient Pt)
        {
            try
            {
                string StrSql = "";
                if (Pt.mLOAConsultation > 0)
                {
                    StrSql = "select d.Checked, d.LoaExpiryDate,d.loaamount,d.nodays,d.approval,d.loabalance,a.id as CategoryId,a.Code as CatCode,a.name as Category,"
                     + " b.code,b.id as CompanyId,b.name as Company , "
                     + " c.id as GradeId,c.name as Grade,e.id as Doctorid,e.EmpCode,e.name as Doctor,d.loadatetime,d.AuthorityBillNo,"
                     + " IsNull(d.LOAType,0) LOAType ,IsNull(d.PharmacyAmount,0) PharmacyAmount from "
                     + " category a,company b,grade c,OpLoaOrder d,employee  e where"
                     + " a.id=d.categoryid and b.id=d.companyid and c.id=d.gradeid and e.id=d.doctorid  and d.authorityid= " + Pt.mAuthorityid;

                }
                else
                {
                    StrSql = "select a.id as CategoryId,a.Code as CatCode,a.name as Category, b.code,b.id as CompanyId,b.name as Company , "
                     + " c.id as GradeId,c.name as Grade,e.id as Doctorid,e.EmpCode,e.name as Doctor from "
                     + " category a,company b,grade c,Patient d,employee  e where"
                     + " a.id=d.categoryid and b.id=d.companyid and c.id=d.gradeid and e.id=" + Pt.DoctorId + " and d.registrationno = " + Pt.Registrationno;
                }
                DataSet rsApproval = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in rsApproval.Tables[0].Rows)
                {
                    Pt.lblcategory = rr["CatCode"].ToString() + "-" + rr["Category"].ToString();
                    Pt.CategoryId = (int)rr["CategoryId"];
                    Pt.lblCompany = rr["code"].ToString() + "-" + rr["Company"].ToString();
                    Pt.CompanyId = (int)rr["CompanyId"];
                    Pt.lblgrade = rr["Grade"].ToString();
                    Pt.GradeId = (int)rr["GradeId"];
                    if (Pt.mLOAConsultation > 0)
                    {
                        Pt.lbldate = MainFunction.DateFormat(rr["loadatetime"].ToString(), "dd", "MMM", "yyyy");
                        Pt.lblLOAAmt = rr["loaamount"].ToString();
                        var LoaTYpe = MainFunction.NullToInteger(rr["LOAType"].ToString());


                        if (LoaTYpe == 0)
                        {
                            Pt.lblLOAAmt = rr["loaamount"].ToString();
                        }
                        else
                        {
                            Pt.lblLOAAmt = rr["PharmacyAmount"].ToString();
                        }
                        Pt.lblLoaDays = rr["nodays"].ToString();
                        LoaTYpe = MainFunction.NullToInteger(rr["LOAType"].ToString());
                        if (LoaTYpe == 0)
                        {
                            StrSql = "select isnull(sum(paidamount),0) as LOADetail from OpLOADetail where Authorityid=" + Pt.mAuthorityid;
                            DataSet nw = MainFunction.SDataSet(StrSql, "tbl");
                            foreach (DataRow xx in nw.Tables[0].Rows)
                            {
                                var LOADetail = MainFunction.NullToInteger(xx["LOADetail"].ToString());
                                if (LOADetail > 0)
                                {
                                    var loaamt = MainFunction.NullToDecmial(Pt.lblLOAAmt);
                                    var loadtl = LOADetail;                                     var tot = loaamt - loadtl;
                                                                         Pt.lblLoaBal = tot.ToString();
                                }
                                else
                                {
                                    Pt.lblLoaBal = Pt.lblLOAAmt;

                                }

                            }

                        }
                        else
                        {
                            StrSql = "select isnull(Sum(case when serviceid <>11 then paidamount else 0 end),0) NPHAmount,"
                                + " Sum(case when serviceid =11 then paidamount else 0 end) PHAmount  from OpLOADetail where Authorityid=" + Pt.mAuthorityid;
                            DataSet nx = MainFunction.SDataSet(StrSql, "tbl");
                            foreach (DataRow yy in nx.Tables[0].Rows)
                            {
                                LoaTYpe = MainFunction.NullToInteger(rr["LOAType"].ToString());
                                if (LoaTYpe == 1)
                                {
                                    var loaamt = MainFunction.NullToDecmial(Pt.lblLOAAmt);
                                    var loadtl = (decimal)yy["PHAmount"];
                                    var tot = loaamt - loadtl;
                                                                         Pt.lblLoaBal = tot.ToString();
                                }
                                else
                                {
                                    var loaamt = MainFunction.NullToDecmial(Pt.lblLOAAmt);
                                    var loadtl = (decimal)yy["NPHAmount"];
                                    var loaph = (decimal)yy["PHAmount"];
                                    var tot = loaamt - loadtl - loaph;
                                                                         Pt.lblLoaBal = tot.ToString();
                                }


                            }

                        }

                    }

                }

                return Pt;
            }
            catch (Exception e) { Pt.ErrMsg = "GetLoAAmt:" + e.Message; return Pt; }

        }
        public static bool CheckLOADays(ref Patient pt, ref int DaysNo, ref long LoanNo)
        {
            try
            {
                TimeSpan NoOfDays;
                pt.revisitdays = 0;
                string str = "select nodays No_of_days from oploaorder "
                        + " where authorityid = " + pt.mAuthorityid;
                DataSet ns = MainFunction.SDataSet(str, "tbl");
                if (ns.Tables[0].Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    foreach (DataRow rr in ns.Tables[0].Rows)
                    {
                        pt.revisitdays = (int)rr["No_of_days"];

                    }
                }


                str = "select nodays no_of_days, loadatetime as billdatetime,getdate() as sysdate  from oploaorder "
                    + " with (nolock) where  authorityid = " + pt.mAuthorityid;
                DataSet nn = MainFunction.SDataSet(str, "tbl");
                foreach (DataRow xx in nn.Tables[0].Rows)
                {
                    NoOfDays = DateTime.Today.Date - DateTime.Parse(xx["billdatetime"].ToString());
                    if (NoOfDays.TotalDays > pt.revisitdays)
                    {
                        LoanNo = pt.revisitdays;
                        DaysNo = (int)NoOfDays.TotalDays;
                        return false;
                    }

                }

                return true;


            }
            catch (Exception e) { pt.ErrMsg = "CheclLoadDays" + e.Message; return false; }



        }
        public static bool CheckLOAAmount(ref Patient pt, decimal itemamount = 0)
        {
            try
            {
                decimal gbillamount = 0;
                int revisitdays = 0;
                string StrSql = "select (Case when loatype =0 then loaamount else pharmacyamount end) "
                            + " amount,nodays no_of_days from oploaorder with (nolock) where authorityid = " + pt.mAuthorityid;
                DataSet loadst = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in loadst.Tables[0].Rows)
                {
                    revisitdays = (int)rr["no_of_days"];
                    pt.LoaLimit = (decimal)rr["amount"];

                }

                StrSql = "select  loaamount=(Case when c.loatype =0 then c.loaamount "
                    + " else c.pharmacyamount end),(Case when c.loatype =0 then c.loaamount "
                    + " else c.pharmacyamount end) - (case when c.loatype = 0 then "
                    + " isnull((Select Sum(PaidAmount) from OPLoaDetail where AUthorityid = "
                    + pt.mAuthorityid + " and serviceid <> 11),0) else isnull((Select Sum(PaidAmount) "
                    + " from OPLoaDetail where AUthorityid = " + pt.mAuthorityid + " and serviceid = 11),0) end) as LOABalance,"
                    + " c.Approval from oploaorder c where  authorityid = " + pt.mAuthorityid;
                DataSet loadst2 = MainFunction.SDataSet(StrSql, "tbl");
                if (loadst2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in loadst2.Tables[0].Rows)
                    {
                        if ((bool)rr["Approval"] == false)
                        {
                            pt.mApproval = false;
                        }
                        else { pt.mApproval = true; }

                        if ((decimal)rr["loaamount"] >= (decimal)rr["LOABalance"])
                        {
                            gbillamount = itemamount;
                            if ((decimal)rr["LOABalance"] < (decimal)(gbillamount - pt.mDedAmt))
                            {
                                return false;
                            }
                            else
                            {
                                if ((decimal)(gbillamount - pt.mDedAmt) > pt.LoaLimit)
                                {
                                    return false;
                                }

                            }
                        }



                    }
                }
                else
                {
                    gbillamount = itemamount;
                    if ((decimal)(gbillamount - pt.mDedAmt) > pt.LoaLimit)
                    {
                        return false;

                    }

                }

                return true;
            }
            catch (Exception e) { pt.ErrMsg = "Check LOA AMount : " + e.Message; return false; }
        }
        public static bool CheckPharmacyLimits(ref Patient pt)
        {
            try
            {
                String Str = "Select PharmacyAmount,No_Of_Days  as LDays from OpLOA where CategoryId = " + pt.CategoryId
                    + " and CompanyId = " + pt.CompanyId + " and Gradeid = " + pt.GradeId + " and LOAType = 1";
                DataSet ns = MainFunction.SDataSet(Str, "tbl");
                if (ns.Tables[0].Rows.Count == 0)
                {
                    return false;
                }

                Str = "Select Count(*) PCount from OPServiceLOA where CategoryId = " + pt.CategoryId
                + " and CompanyId = " + pt.CompanyId + " and Gradeid = " + pt.GradeId;
                DataSet xns = MainFunction.SDataSet(Str, "tbl");
                if (xns.Tables[0].Rows.Count == 0)
                {
                    return false;
                }
                foreach (DataRow rr in xns.Tables[0].Rows)
                {

                    if ((int)rr["PCount"] == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e) { pt.ErrMsg = "CheckPharmacyLimits : " + e.Message; return false; }
        }

        public static string LOAApproval(string PIN, int DoctorId, int Gstationid, int gOperatorid)
        {
            int regNo = int.Parse(MainFunction.getRegNumber(PIN.ToString()));
            string IACode = MainFunction.getAuthorityCode(PIN);
            string Msg = "";
            DataSet pindst = MainFunction.SDataSet(" Select Count(*) PCount from ARLOAOrder "
            + " where checked=0 and RegNo =" + regNo + " and IACode ='"
            + IACode + "' and DoctorId =" + DoctorId, "tbl");

            foreach (DataRow rr in pindst.Tables[0].Rows)
            {
                if ((int)rr["PCount"] > 0)
                {
                    Msg = "There is an Approval pending for LOA";
                }
                else
                {
                    bool insertOrder = MainFunction.SSqlExcuite("Insert into ARLOAOrder (RegNo,IACode,DoctorId,Checked,StationId,OperatorId,DateTime) "
                            + " values (" + regNo + ",'" + IACode + "'," + DoctorId + ",0," + Gstationid + ", " + gOperatorid + ",GetDate())");
                }
            }
            return Msg;

        }
        public static int GetAuthorityId(int regNo, string IACode, int CatID, int CompId, int GradeId, int DoctorId)
        {
            try
            {
                int Msg = 0;
                DataSet pindst = MainFunction.SDataSet("select max(AuthorityId) AuthorityId from OpLOAOrder where registrationno=" + regNo
                    + " and IssueAuthorityCode ='" + IACode + "'   And doctorid = " + DoctorId + " and deleted=0 "
                    + " and categoryid = " + CatID + " and companyid = " + CompId + " and gradeid= " + GradeId + "", "tbl");
                foreach (DataRow rr in pindst.Tables[0].Rows)
                {
                                         Msg = (int)rr["AuthorityId"];
                }
                return Msg;
            }
            catch (Exception e) { return 0; }
        }
        public static int CheckLOAConsultation(int CompanyID)
        {
            try
            {
                bool Msg = false;
                DataSet pindst = MainFunction.SDataSet("Select isnull(LOAConsultation,0) LOAConsultation from company where id=" + CompanyID, "tbl");
                foreach (DataRow rr in pindst.Tables[0].Rows)
                {
                    Msg = (bool)rr["LOAConsultation"];
                }
                if (Msg == true)
                { return 1; }
                else { return 0; }
            }
            catch (Exception e) { return 0; }
        }
        public static String PromptPinStatus(int RegistrationNo)
        {
            string Msg = "";
            DataSet pindst = MainFunction.SDataSet("EXEC SP_OP_PROMPTPINSTATUS " + RegistrationNo, "tbl");
            foreach (DataRow rr in pindst.Tables[0].Rows)
            {
                Msg = rr["Reason"].ToString();
            }
            return Msg;
        }
        public static List<PHAlertMsg> GetPHAlerts(int RegistrationNo)
        {
            int slno = 0;
            List<PHAlertMsg> aa = new List<PHAlertMsg>();
            DataSet pindst = MainFunction.SDataSet("select a.id,a.message,e.name as operator,s.name as station "
                + " from pharmacistmsg a,employee e,station s where e.id=a.operatorid and a.stationid=s.id "
                + " and a.deleted=0 and a.pinno =" + RegistrationNo + " order by a.id desc", "tbl");
            foreach (DataRow rr in pindst.Tables[0].Rows)
            {
                PHAlertMsg pha = new PHAlertMsg();
                slno += 1;
                pha.Slno = slno;
                pha.Message = rr["message"].ToString();
                pha.Operator = rr["operator"].ToString();
                pha.Station = rr["station"].ToString();
                aa.Add(pha);
            }

            return aa;
        }
        public static List<Prescription> LoadPrescription(int mRegNo, int DoctorID, User Userdata)
        {
            List<Prescription> ls = new List<Prescription>();
            int stationid = Userdata.selectedStationID;
            String gIACode = Userdata.gIACode;
            try
            {
                string StrSql = "";
                if (stationid == 18)
                {
                    StrSql = "SELECT cp.ID as OrderNo,cp.VisitID as VisitID,cp.DateTime as DateTime, "
             + " e.Name as Operator,s.Name as Station,cp.doctorid as DoctorID,cp.corder, 1 prescription "
             + " FROM employee e,Station s,clinicalprescription cp "
             + " WHERE cp.issueauthoritycode = '" + gIACode + "' "
             + " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid "
             + " and cp.datetime >= getdate() - 30 and "
             + " (select count(*) from ClinicalPrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 "
             + " Union All SELECT cp.ID as [Order No],cp.VisitID,cp.DateTime, "
             + " e.Name as Operator ,s.Name as Station,cp.doctorid,null as corder, 0 prescription "
             + " FROM employee e,Station s,MSF_Prescription cp WHERE cp.deleted=0 and cp.issueauthoritycode = '" + gIACode + "' "
             + " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid "
             + " and cp.datetime >= getdate() - 30 and (select count(*) from "
             + " MSF_PrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 "
             + " order by cp.datetime ";
                }
                else
                {
                    StrSql = "SELECT cp.ID as OrderNo,cp.VisitID as VisitID,cp.DateTime as DateTime, "
            + " e.Name as Operator,s.Name as Station,cp.doctorid DoctorID,cp.corder, 1 prescription "
            + " FROM employee e,Station s,clinicalprescription cp "
            + " WHERE cp.issueauthoritycode = '" + gIACode + "' "
            + " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid and e.id=" + DoctorID + " "
            + " and cp.datetime >= getdate() - 30  "
            + " and (select count(*) from ClinicalPrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 "
            + " Union All SELECT cp.ID as [Order No],cp.VisitID,cp.DateTime, "
            + " e.Name as Operator ,s.Name as Station,cp.doctorid,null as corder, 0 prescription "
            + " FROM employee e,Station s,MSF_Prescription cp WHERE cp.deleted=0 and cp.issueauthoritycode = '" + gIACode + "' "
            + " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid and e.id=" + DoctorID + " "
            + " and cp.datetime >= getdate() - 30 and (select count(*) from "
            + " MSF_PrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 "
            + " order by cp.datetime ";
                }

                DataSet predst = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in predst.Tables[0].Rows)
                {
                    Prescription ps = new Prescription();
                    ps.OrderNo = (int)rr["OrderNo"];
                    ps.DateTime = MainFunction.DateFormat(rr["DateTime"].ToString(), "dd", "MMM", "yyyy");
                    ps.DoctorID = (int)rr["DoctorID"];
                    ps.Operator = rr["Operator"].ToString();
                    ps.Station = rr["Station"].ToString();
                    ps.VisitId = (int)rr["VisitID"];
                    ps.MPrescriptionF = (int)rr["prescription"];
                    ps.corder = (bool)rr["corder"];
                    ls.Add(ps);
                }
                return ls;
            }
            catch (Exception e)
            {
                ls = new List<Prescription>();
                Prescription pp = new Prescription();
                pp.ErrMsg = "Loading Prescription:" + e.Message;
                ls.Add(pp);
                return ls;

            }


        }
        public static List<PrescriptionDetail> DisplayPrescription(int orderId, string DDate,
            int MPrescriptionF, int Gstationid, int RegNumber = 0, int DocID = 0)
        {
            List<PrescriptionDetail> pt = new List<PrescriptionDetail>();
            PrescriptionDetail xpt = new PrescriptionDetail();
            try
            {
                DateTime dd = DateTime.Parse(DDate);
                DateTime bb = DateTime.Parse("31 Jan 2003");                 long lquantity;
                long issquantity;
                String StrSql = "";
                if (MPrescriptionF == 1 && dd > bb)
                {
                    StrSql = "select (select Count(Id) from ClinicalPrescription where "
            + " Registrationno=" + RegNumber + " and DoctorID =" + DocID + " ) PCount, "
            + " c.Weight,a.strength_no as Itemstrength,isnull(a.drugstate,0) as drugstate,a.name,b.drugid,"
            + " isnull(a.conversionqty,1) as conversionqty,b.quantity,b.strength_no as Strength,b.routeofadmin,b.frequency_id,b.duration_no,b.duration_id,a.itemcode,"
            + " f.description as frequency,d.description as duration,b.beforeAfter,f.Value as FrequencyConvValue,"
            + " isnull(d.ConversionValue,0) as DurationConvValue,f.period,b.orderid as orderno,z.datetime"
            + " from Item a,ClinicalPrescriptionDetail b,Frequency f,DurationComponent d,"
            + " ClinicalVisit c, ClinicalPrescription z "
            + " where a.id=b.drugid and f.id = b.Frequency_id and b.duration_id = d.id  "
            + " And Z.visitid = c.ID and z.id=b.orderid and b.orderId = " + orderId;



                    DataSet pr = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow rr in pr.Tables[0].Rows)
                    {

                        issquantity = long.Parse(rr["Quantity"].ToString()) / long.Parse(rr["Conversionqty"].ToString());
                        if (issquantity < 1)
                        {
                            issquantity = 1;
                        }
                        var strengh = (int)rr["strength"];
                        var itmstr = (decimal)rr["itemstrength"];
                        var durno = (Int16)rr["Duration_no"];
                        var durval = (int)rr["DurationConvValue"];
                        var freq = (int)rr["FrequencyConvValue"];
                        var drugst = (Byte)rr["drugstate"];
                        var perio = (int)rr["period"];
                        lquantity = CalculateQty(strengh, itmstr, durno, durval, freq
                       , drugst, perio);

                        xpt = new PrescriptionDetail();
                        xpt.drugid = (int)rr["drugid"];
                        xpt.Name = rr["Name"].ToString();
                        xpt.strength = (int)rr["strength"];
                        xpt.RouteofAdmin = rr["RouteofAdmin"].ToString();
                        xpt.frequency = rr["frequency"].ToString();
                        xpt.issquantity = issquantity;
                        xpt.Duration = rr["Duration_no"].ToString().Trim() + " " + rr["duration"].ToString().Trim();
                        xpt.OrderNo = (int)rr["OrderNo"];
                        xpt.lquantity = lquantity;
                        StrSql = "Select  g.Name from m_generic g,itemgeneric s where g.id=s.genericid and  s.itemid=" + rr["drugid"];

                        DataSet temp = MainFunction.SDataSet(StrSql, "tbl2");
                        foreach (DataRow yy in temp.Tables[0].Rows)
                        {
                            xpt.GenericName = yy["Name"].ToString().Trim();
                        }

                        xpt.DateTime = rr["DateTime"].ToString();
                        pt.Add(xpt);




                    }

                }
                else if (MPrescriptionF == 0 && dd > bb)
                {
                    StrSql = " Select a.Itemid as drugid,a.Qty * b.ConversionQty AS OrderQty "
                    + " from MSF_PrescriptionDetail a, Itempacking b where a.itemid = b.itemid and a.packid=b.packid and a.Orderid=" + orderId;
                    DataSet pr = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow rr in pr.Tables[0].Rows)
                    {
                        StrSql = " Select i.ID,(i.itemcode + ' - ' + i.Name) as name,i.drugtype,s.conversionqty,s.tax,b.mrp,b.sellingprice,Bs.Quantity, "
                + "s.unitid,u.name as unit, b.batchid,b.BatchNo, b.ExpiryDate,isnull(i.criticalitem,0) as criticalitem,"
                + " i.itemcode as Code,i.Name as Item, b.costprice, Isnull(i.CategoryID,0) ItemCategoryID "
                + " FROM Item i,ITEMSTORE s ,packing u,batch b, BatchStore Bs  where i.id=" + rr["drugid"] + " "
                + " and s.unitid=u.id and b.itemid=i.id  and S.Stationid =  " + Gstationid + " "
                + " and I.Id = s.ItemId  and bs.itemid=i.id  and b.batchid=bs.batchid and "
                + " ltrim(rtrim(B.BatchNo)) = ltrim(rtrim(Bs.BatchNo)) and Bs.StationId = " + Gstationid + " "
                + " and bs.quantity>0  Order By B.Startdate ";
                        DataSet temp = MainFunction.SDataSet(StrSql, "tbl2");
                        foreach (DataRow yy in temp.Tables[0].Rows)
                        {

                            lquantity = long.Parse(rr["OrderQty"].ToString());


                        }



                    }


                }

                return pt;

            }
            catch (Exception e)
            {
                pt = new List<PrescriptionDetail>();
                xpt = new PrescriptionDetail();
                xpt.ErrMsg = "Error loading Prescription details!";
                pt.Add(xpt);
                return pt;

            }
        }
        public static long CalculateQty(long ReqDose, decimal ActualSt, long Duration_no, long DurationConvValue
            , long FrequencyConvValue, long drugstate, long lPeriod)
        {
            int iTemp = 0; long result = 0;
            if (ActualSt == 0) { ActualSt = 1; }
            switch (drugstate)
            {
                case 0: if (ReqDose > ActualSt)
                    {
                        iTemp = (int)(ReqDose / ActualSt);

                        if ((ReqDose / ActualSt) - iTemp < 1 && (ReqDose / ActualSt) - iTemp > 0)
                        {
                            iTemp = iTemp + 1;
                            result = (Duration_no * DurationConvValue * FrequencyConvValue * iTemp) / lPeriod;
                        }
                        else
                        {
                            result = (Duration_no * DurationConvValue * FrequencyConvValue * iTemp) / lPeriod;
                        }
                    }
                    else
                    {
                        if (ReqDose <= ActualSt)
                        {
                            iTemp = 1;
                            result = (Duration_no * DurationConvValue * FrequencyConvValue) / lPeriod;
                        }

                    };

                    break;
                case 1:
                    result = (Duration_no * DurationConvValue * FrequencyConvValue * ReqDose) / lPeriod; break;
                case 2:
                    result = 1; break;
            }
            return result;


        }
        public static List<Diagnosis> GetDiagnosis(long pinno, long orderId, string xxx)
        {
            List<Diagnosis> pt = new List<Diagnosis>();
            try
            {
                string str = "select a.icdcode,a.icddescription,a.visitid "
                    + "  from icddetail a,clinicalprescription b "
                    + " where a.visitid =b.visitid and b.registrationno=" + pinno
                    + " and b.id=" + orderId + " AND b.issueauthoritycode = '" + xxx + "'";
                DataSet ds = MainFunction.SDataSet(str, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    Diagnosis tt = new Diagnosis();
                    tt.VisitId = (int)rr["visitid"];
                    tt.ICDCode = rr["icdcode"].ToString();
                    tt.Description = rr["icddescription"].ToString();

                    pt.Add(tt);

                }



                return pt;
            }

            catch (Exception e)
            {
                pt = new List<Diagnosis>();
                Diagnosis dd = new Diagnosis();
                dd.ErrMsg = "Error Reading Diagnosis: " + e.Message;
                pt.Add(dd);
                return pt;
            }


        }
        public static Patient CashBillControl(Patient PT)
        {
            PT.CompanyId = 1;
            PT.CategoryId = 1;
            PT.CompanyName = MainFunction.GetName("select code + ' - ' + name as Name from company where id=" + PT.CompanyId, "Name");
            PT.CategoryName = MainFunction.GetName("select code + ' - ' + name as Name from category where id=" + PT.CategoryId, "Name");
            PT.BillisCredit = false;
            return PT;
        }
        public static Item ItemSelect(ItemSelectParam itm, int mItemID = 0, List<ItemSelectParam> SelectedItemList = null)
        {
                         Item ItemInsert = new Item();
            try
            {
                int lSmallQty;
                int lLargeQty;
                string strCrtItems;
                decimal cprice;
                int dispQty = 1;                    int qoh;
                int intCheck;
                DataSet rsBatch = null;

                ItemInsert.ErrorFlag = false;
                if (itm.optCrPat == true)
                {
                                         if (ServiceLoaCheck(ref itm) == false)
                    {
                        ItemInsert.ErrMsg = itm.ErrMsg;
                        ItemInsert.ErrorFlag = true;
                        return ItemInsert;

                    };

                    intCheck = CheckLOAAppServices(11, itm.mDeptId, itm.mItemID, itm.mAuthorityid, itm.gIACode, itm.RegNo);

                    String StrSql = "select count(*) as c1 from opcompanyitemservices where companyid = "
                         + itm.cmbCompany + " and categoryid = " + itm.mCategoryid + " and serviceid=11 and gradeid = " + itm.mGradeId
                         + " and itemid = " + itm.mItemID + "  and departmentid = " + itm.mDeptId;
                    int C1 = 0;
                    DataSet Temp = MainFunction.SDataSet(StrSql, "tb1");
                    foreach (DataRow rr in Temp.Tables[0].Rows)
                    {
                        C1 = (int)rr["c1"];
                    }
                    if (intCheck == 0)
                    {
                        if (C1 > 0)
                        {
                            ItemInsert.ErrMsg = "Item is Excluded for the Company Bill";
                            ItemInsert.ErrorFlag = true;
                            return ItemInsert;
                        }
                    }

                    if (intCheck == 0)
                    {
                        StrSql = "select count(*) as c1 from opcompanydeptservices where companyid = " + itm.cmbCompany
                          + " and categoryid = " + itm.mCategoryid + " and gradeid = " + itm.mGradeId + " and serviceid = 11 and departmentid = "
                          + itm.mDeptId;
                        C1 = (int)MainFunction.GetOneVal(StrSql, "c1");
                        if (C1 > 0)
                        {
                            ItemInsert.ErrMsg = "This Service is excluded.";
                            ItemInsert.ErrorFlag = true;
                            return ItemInsert;
                        }
                        else
                        {
                            StrSql = "select count(*) as c1 from opcompanyservices where companyid = " + itm.cmbCompany
                                 + " and categoryid = " + itm.mCategoryid + " and gradeid = " + itm.mGradeId + " and serviceid = 11";
                            C1 = (int)MainFunction.GetOneVal(StrSql, "c1");
                            if (C1 > 0)
                            {
                                ItemInsert.ErrMsg = "This Service is excluded.";
                                ItemInsert.ErrorFlag = true;
                                return ItemInsert;
                            }
                        }
                    }
                }

                                 if (itm.InsertItemValue != null)
                {
                                         if (itm.InsertItemValue.Count > 0)
                    {
                        String blnDrugInteraction = DrugInteraction(ref itm, itm.mItemID, itm.mItemName, 1, itm.InsertItemValue);
                                                 if (itm.StrWhole != "" && itm.StrWhole != null)
                        {
                                                         ItemInsert.AlertDrugInteraction = true;
                        }
                        else
                        { itm.blnDrugInteraction = false; }
                        if (itm.blnDrugInteraction == true)
                        {
                            ItemInsert.ErrMsg = "DrugInteraction Can't Select this item";
                            ItemInsert.ErrorFlag = true;
                            return ItemInsert;
                        }

                    }
                }
                string StrSqlx = "";
                if (itm.mPrescription < 0)
                {
                    StrSqlx = " select a.id "
                    + " from clinicalPrescription a,ClinicalPrescriptionDetail b,DurationComponent d "
                    + " Where a.ID = b.orderId And b.drugid = " + itm.mItemID + " And b.duration_id = d.ID And b.dispatched = 1 and a.registrationno = " + itm.RegNo
                    + " and getdate() between a.datetime and dateadd(d,b.duration_no * d.ConversionValue,a.datetime)";
                    DataSet Temp = MainFunction.SDataSet(StrSqlx, "tb1");
                    if (Temp.Tables[0].Rows.Count > 0)
                    {
                                                 ItemInsert.AlertItemIssuedAlerdy = true;                     }

                }
                else
                {
                    if (itm.optCrPat == true)
                    {
                        StrSqlx = " select top 1 A.ID,A.DATETIME,A.creditbillno from CashIssue a,CashIssueDetail b with (nolock) "
                       + " Where a.regno = " + itm.RegNo + " and a.ID = b.BillNo and b.ServiceID = " + itm.mItemID + "  "
                       + " and a.CancelYesNo = 0 and a.Cash_Credit=1 "
                       + " and a.datetime between dateadd(d,-30,getdate()) and getdate() order by a.id desc ";
                        DataSet Temp = MainFunction.SDataSet(StrSqlx, "tb1");
                        if (Temp.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in Temp.Tables[0].Rows)
                            {
                                ItemInsert.ErrMsg = "Item already issued to the patient on " + rr["DATETIME"].ToString() + " of BIllNO: " + rr["creditbillno"].ToString();
                                ItemInsert.AlertItemIssuedAlerdy = true;                             }
                        }

                    }

                }



                if (itm.listview == 0)
                {
                    rsBatch = MainFunction.SDataSet(
                       " Select i.ID, (i.itemcode + ' - ' + i.Name) as name, (i.Name + ' - ' + i.itemcode) as nameCode "
                      + " ,i.drugtype,s.conversionqty,s.tax,"
                      + " b.mrp,b.sellingprice,Bs.Quantity,s.unitid,u.name as unit, b.batchid,b.BatchNo, "
                      + " b.ExpiryDate,isnull(i.criticalitem,0) as criticalitem,i.itemcode as Code,i.Name as Item, b.costprice "
                      + " , Isnull(i.CategoryID,0) ItemCategoryID "
                      + " ,isnull(s.minlevel,0) as MinLevel "
                      + " FROM Item i,ITEMSTORE s ,packing u,batch b, BatchStore Bs "
                      + " where s.unitid=u.id and b.itemid=i.id "
                      + " and i.id=" + itm.mItemID
                      + " and S.Stationid = " + itm.StationID
                      + " and I.Id = s.ItemId "
                      + " and bs.itemid=i.id "
                      + " and ltrim(rtrim(B.BatchNo)) = ltrim(rtrim(Bs.BatchNo)) and b.batchid=bs.batchid "
                      + " and Bs.StationId = " + itm.StationID
                      + " and bs.quantity>0 "
                      + " Order By B.Startdate", "tb1");

                                                                                    
                }
                else
                {
                    rsBatch = MainFunction.SDataSet(
                          " Select i.ID,(i.itemcode + ' - ' + i.Name) as name,i.drugtype,s.conversionqty,s.tax, "
                        + " b.mrp,b.sellingprice,Bs.Quantity,s.unitid,u.name as unit, b.batchid,b.BatchNo, b.ExpiryDate,"
                        + " isnull(i.criticalitem,0) as criticalitem,i.itemcode as Code,i.Name as Item, b.costprice, Isnull(i.CategoryID,0) ItemCategoryID "
                        + " ,isnull(s.minlevel,0) as MinLevel "
                        + " FROM Item i,ITEMSTORE s ,packing u,batch b, BatchStore Bs  "
                        + " where i.id=" + itm.mItemID + " and s.unitid=u.id and b.itemid=i.id  and "
                        + " S.Stationid = " + itm.StationID + " and I.Id = s.ItemId  and bs.itemid=i.id  and b.batchid=bs.batchid "
                        + " and ltrim(rtrim(B.BatchNo)) = ltrim(rtrim(Bs.BatchNo)) and "
                        + " Bs.StationId =" + itm.StationID + " and bs.quantity>0  Order By B.Startdate", "tb1");

                }

                decimal lngMyCP = 0;
                if (rsBatch.Tables[0].Rows.Count == 0)
                {
                    ItemInsert.ErrMsg = "No batch found for this items! can't Issue.";
                    ItemInsert.ErrorFlag = true;
                    return ItemInsert;

                }
                foreach (DataRow rr in rsBatch.Tables[0].Rows)
                {

                    if ((decimal)rr["costprice"] > lngMyCP)
                    {
                        int conqty = 1;
                        if ((int)rr["conversionqty"] > 0)
                        { conqty = (int)rr["conversionqty"]; }

                        lngMyCP = (decimal)rr["costprice"] * conqty;

                    }
                }
                qoh = 0;
                foreach (DataRow rr in rsBatch.Tables[0].Rows)
                {
                    qoh += (int)rr["Quantity"];

                }



                strCrtItems = "";
                int cc = rsBatch.Tables[0].Rows.Count - 1;                  foreach (DataRow rr in rsBatch.Tables[0].Rows)
                {
                    lSmallQty = qoh;
                    int conqty = 1;
                    if ((int)rr["conversionqty"] > 0)
                    { conqty = (int)rr["conversionqty"]; }
                                         lLargeQty = qoh / conqty;
                    cprice = (decimal)rr["mrp"] * conqty;
                    if (lngMyCP > cprice)
                    {
                        ItemInsert.ErrMsg = "Cost Price (" + lngMyCP + ") is greater than Selling Price (" + cprice + ")." + "Bill Cannot be made";
                        ItemInsert.ErrorFlag = true;
                        return ItemInsert;

                    }

                                                              if (itm.substitutetype == 0)
                    {
                        foreach (DataRow XK in rsBatch.Tables[0].Rows)
                        {
                            if (cc == rsBatch.Tables[0].Rows.Count - 1)
                            {
                                cc += 1;
                                if ((bool)XK["criticalitem"] == true)
                                {
                                    strCrtItems = strCrtItems + XK["Name"].ToString();
                                }

                                List<InsertedItemsList> xx = new List<InsertedItemsList>();

                                InsertedItemsList nn = new InsertedItemsList();
                                if (itm.InsertItemValue == null)
                                {
                                    nn.SNO = 1;
                                }
                                else
                                {
                                    nn.SNO = itm.InsertItemValue.Count + 1;
                                }
                                nn.qoh = lLargeQty;
                                nn.DrugName = XK["Name"].ToString();
                                nn.BatchNo = "";
                                nn.Highunit = XK["unit"].ToString();
                                nn.price = Math.Round(cprice, 2);
                                nn.tax = (float)Math.Round(MainFunction.NullToFloat(XK["Tax"].ToString()), 2);
                                nn.qty = dispQty;
                                nn.UnitList = XK["unit"].ToString();
                                nn.amount = dispQty * Math.Round(cprice, 2);
                                nn.NewUomName = XK["unit"].ToString();
                                nn.DispatchQty = 0;
                                nn.qoh2 = qoh;
                                nn.Drugtype = (int)XK["DrugType"];
                                nn.conversionqty = (int)XK["Conversionqty"];
                                nn.batchid = XK["Batchid"].ToString();
                                nn.purqty = 0;
                                nn.MinLevel = (int)XK["MinLevel"];
                                nn.lsmallqty = lSmallQty;
                                nn.NewUomID = int.Parse(XK["UnitID"].ToString()); ;
                                nn.PrescriptionID = itm.mPrescription;
                                nn.Deductabletype = 0;
                                nn.DeductablePerAmounttype = 0;
                                nn.DeductablePerAmount = 0;
                                nn.DiscountPerAmountType = 0;
                                nn.DiscountPerAmount = 0;


                                nn.OrderedItem = XK["Name"].ToString();
                                nn.OrderedItemid = int.Parse(XK["ID"].ToString());

                                nn.temp3 = null;
                                nn.Name = XK["Item"].ToString();
                                nn.ItemCode = XK["Code"].ToString();
                                nn.ID = int.Parse(XK["ID"].ToString());

                                if (itm.InsertItemValue == null)
                                {
                                    xx.Add(nn);
                                    ItemInsert.InsertItemValue = xx;
                                }
                                else
                                {
                                    itm.InsertItemValue.Add(nn);
                                    ItemInsert.InsertItemValue = itm.InsertItemValue;
                                }
                            }
                        }



                    }

                    if (strCrtItems.Length > 0)
                    {
                        ItemInsert.ErrMsg = "Following are Critical Item(s):  " + strCrtItems;
                        ItemInsert.ErrorFlag = true;
                        return ItemInsert;
                    }

                } 









                
                return ItemInsert;
                                  
            }
            catch (Exception e)
            {
                Item i = new Item();
                i.ErrorFlag = true;
                i.ErrMsg = "Error Loading this Item : " + e.Message;
                return i;


            }

        }
        public static Item ItemSelectGeneric(ItemSelectParam itm, int GenericID = 0)
        {
                         Item ItemInsert = new Item();
            try
            {
                int lSmallQty;
                int lLargeQty;
                string strCrtItems;
                decimal cprice;
                int dispQty = 0;
                int qoh;
                int intCheck;
                DataSet rsBatch = null;

                ItemInsert.ErrorFlag = false;
                if (itm.optCrPat == true)
                {
                                         if (ServiceLoaCheck(ref itm) == false)
                    {
                        ItemInsert.ErrMsg = itm.ErrMsg;
                        ItemInsert.ErrorFlag = true;
                        return ItemInsert;

                    };

                    intCheck = CheckLOAAppServices(11, itm.mDeptId, itm.mItemID, itm.mAuthorityid, itm.gIACode, itm.RegNo);

                    String StrSql = "select count(*) as c1 from opcompanyitemservices where companyid = "
                         + itm.cmbCompany + " and categoryid = " + itm.mCategoryid + " and serviceid=11 and gradeid = " + itm.mGradeId
                         + " and itemid = " + itm.mItemID + "  and departmentid = " + itm.mDeptId;
                    int C1 = 0;
                    DataSet Temp = MainFunction.SDataSet(StrSql, "tb1");
                    foreach (DataRow rr in Temp.Tables[0].Rows)
                    {
                        C1 = (int)rr["c1"];
                    }
                    if (intCheck == 0)
                    {
                        if (C1 > 0)
                        {
                            ItemInsert.ErrMsg = "Item is Excluded for the Company Bill";
                            ItemInsert.ErrorFlag = true;
                            return ItemInsert;
                        }
                    }

                    if (intCheck == 0)
                    {
                        StrSql = "select count(*) as c1 from opcompanydeptservices where companyid = " + itm.cmbCompany
                          + " and categoryid = " + itm.mCategoryid + " and gradeid = " + itm.mGradeId + " and serviceid = 11 and departmentid = "
                          + itm.mDeptId;
                        C1 = (int)MainFunction.GetOneVal(StrSql, "c1");
                        if (C1 > 0)
                        {
                            ItemInsert.ErrMsg = "This Service is excluded.";
                            ItemInsert.ErrorFlag = true;
                            return ItemInsert;
                        }
                        else
                        {
                            StrSql = "select count(*) as c1 from opcompanyservices where companyid = " + itm.cmbCompany
                                 + " and categoryid = " + itm.mCategoryid + " and gradeid = " + itm.mGradeId + " and serviceid = 11";
                            C1 = (int)MainFunction.GetOneVal(StrSql, "c1");
                            if (C1 > 0)
                            {
                                ItemInsert.ErrMsg = "This Service is excluded.";
                                ItemInsert.ErrorFlag = true;
                                return ItemInsert;
                            }
                        }
                    }
                }

                                 if (itm.InsertItemValue != null)
                {
                                         if (itm.InsertItemValue.Count > 0)
                    {
                        String blnDrugInteraction = DrugInteraction(ref itm, itm.mItemID, itm.mItemName, 1, itm.InsertItemValue);
                                                 if (itm.StrWhole != "" && itm.StrWhole != null)
                        {
                                                         ItemInsert.StrWhole = itm.StrWhole;
                            ItemInsert.AlertDrugInteraction = true;

                        }
                        else
                        { itm.blnDrugInteraction = false; }
                        if (itm.blnDrugInteraction == true)
                        {
                            ItemInsert.ErrMsg = "DrugInteraction Can't Select this item";
                            ItemInsert.ErrorFlag = true;
                            return ItemInsert;
                        }

                    }
                }
                string StrSqlx = "";
                if (itm.mPrescription < 0)
                {
                    StrSqlx = " select a.id "
                    + " from clinicalPrescription a,ClinicalPrescriptionDetail b,DurationComponent d "
                    + " Where a.ID = b.orderId And b.drugid = " + itm.mItemID + " And b.duration_id = d.ID And b.dispatched = 1 and a.registrationno = " + itm.RegNo
                    + " and getdate() between a.datetime and dateadd(d,b.duration_no * d.ConversionValue,a.datetime)";
                    DataSet Temp = MainFunction.SDataSet(StrSqlx, "tb1");
                    if (Temp.Tables[0].Rows.Count > 0)
                    {
                                                 ItemInsert.AlertItemIssuedAlerdy = true;                     }

                }
                else
                {
                    if (itm.optCrPat == true)
                    {
                        StrSqlx = " select top 1 A.ID,A.DATETIME,A.creditbillno from CashIssue a,CashIssueDetail b with (nolock) "
                       + " Where a.regno = " + itm.RegNo + " and a.ID = b.BillNo and b.ServiceID = " + itm.mItemID + "  "
                       + " and a.CancelYesNo = 0 and a.Cash_Credit=1 "
                       + " and a.datetime between dateadd(d,-30,getdate()) and getdate() order by a.id desc ";
                        DataSet Temp = MainFunction.SDataSet(StrSqlx, "tb1");
                        if (Temp.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in Temp.Tables[0].Rows)
                            {
                                ItemInsert.ErrMsg = "Item already issued to the patient on " + rr["DATETIME"].ToString() + " of BIllNO: " + rr["creditbillno"].ToString();
                                ItemInsert.AlertItemIssuedAlerdy = true;                             }
                        }

                    }

                }



                if (itm.listview == 0)
                {
                    rsBatch = MainFunction.SDataSet(
                       " Select i.ID, (i.itemcode + ' - ' + i.Name) as name, (i.Name + ' - ' + i.itemcode) as nameCode "
                      + " ,i.drugtype,s.conversionqty,s.tax,"
                      + " b.mrp,b.sellingprice,Bs.Quantity,s.unitid,u.name as unit, b.batchid,b.BatchNo, "
                      + " b.ExpiryDate,isnull(i.criticalitem,0) as criticalitem,i.itemcode as Code,i.Name as Item, b.costprice "
                      + " , Isnull(i.CategoryID,0) ItemCategoryID "
                      + " ,isnull(s.minlevel,0) as MinLevel "
                      + " FROM Item i,ITEMSTORE s ,packing u,batch b, BatchStore Bs "
                      + " where s.unitid=u.id and b.itemid=i.id "
                      + " and i.id=" + itm.mItemID
                      + " and S.Stationid = " + itm.StationID
                      + " and I.Id = s.ItemId "
                      + " and bs.itemid=i.id "
                      + " and ltrim(rtrim(B.BatchNo)) = ltrim(rtrim(Bs.BatchNo)) and b.batchid=bs.batchid "
                      + " and Bs.StationId = " + itm.StationID
                      + " and bs.quantity>0 "
                      + " Order By B.Startdate", "tb1");

                                                                                    
                }
                else
                {
                    rsBatch = MainFunction.SDataSet(
                          " Select i.ID,(i.itemcode + ' - ' + i.Name) as name,i.drugtype,s.conversionqty,s.tax, "
                        + " b.mrp,b.sellingprice,Bs.Quantity,s.unitid,u.name as unit, b.batchid,b.BatchNo, b.ExpiryDate,"
                        + " isnull(i.criticalitem,0) as criticalitem,i.itemcode as Code,i.Name as Item, b.costprice, Isnull(i.CategoryID,0) ItemCategoryID "
                        + " ,isnull(s.minlevel,0) as MinLevel "
                        + " FROM Item i,ITEMSTORE s ,packing u,batch b, BatchStore Bs  "
                        + " where i.id=" + itm.mItemID + " and s.unitid=u.id and b.itemid=i.id  and "
                        + " S.Stationid = " + itm.StationID + " and I.Id = s.ItemId  and bs.itemid=i.id  and b.batchid=bs.batchid "
                        + " and ltrim(rtrim(B.BatchNo)) = ltrim(rtrim(Bs.BatchNo)) and "
                        + " Bs.StationId =" + itm.StationID + " and bs.quantity>0  Order By B.Startdate", "tb1");

                }

                decimal lngMyCP = 0;
                if (rsBatch.Tables[0].Rows.Count == 0)
                {
                    ItemInsert.ErrMsg = "No batch found for this items! can't Issue.";
                    ItemInsert.ErrorFlag = true;
                    return ItemInsert;

                }
                foreach (DataRow rr in rsBatch.Tables[0].Rows)
                {

                    if ((decimal)rr["costprice"] > lngMyCP)
                    {
                        int conqty = 1;
                        if ((int)rr["conversionqty"] > 0)
                        { conqty = (int)rr["conversionqty"]; }

                        lngMyCP = (decimal)rr["costprice"] * conqty;

                    }
                }
                qoh = 0;
                foreach (DataRow rr in rsBatch.Tables[0].Rows)
                {
                    qoh += (int)rr["Quantity"];

                }



                strCrtItems = "";
                int cc = rsBatch.Tables[0].Rows.Count - 1;                  foreach (DataRow rr in rsBatch.Tables[0].Rows)
                {
                    lSmallQty = qoh;
                    int conqty = 1;
                    if ((int)rr["conversionqty"] > 0)
                    { conqty = (int)rr["conversionqty"]; }
                                         lLargeQty = qoh / conqty;
                    cprice = (decimal)rr["mrp"] * conqty;
                    if (lngMyCP > cprice)
                    {
                        ItemInsert.ErrMsg = "Cost Price (" + lngMyCP + ") is greater than Selling Price (" + cprice + ")." + "Bill Cannot be made";
                        ItemInsert.ErrorFlag = true;
                        return ItemInsert;

                    }

                                                              if (itm.substitutetype == 0)
                    {
                        foreach (DataRow XK in rsBatch.Tables[0].Rows)
                        {
                            if (cc == rsBatch.Tables[0].Rows.Count - 1)
                            {
                                cc += 1;
                                if ((bool)XK["criticalitem"] == true)
                                {
                                    strCrtItems = strCrtItems + XK["Name"].ToString();
                                }

                                List<InsertedItemsList> xx = new List<InsertedItemsList>();

                                InsertedItemsList nn = new InsertedItemsList();
                                if (itm.InsertItemValue == null)
                                {
                                    nn.SNO = 1;
                                }
                                else
                                {
                                    nn.SNO = itm.InsertItemValue.Count + 1;
                                }
                                nn.qoh = lLargeQty;
                                nn.DrugName = XK["Name"].ToString();
                                nn.BatchNo = "";
                                nn.Highunit = XK["unit"].ToString();
                                nn.price = Math.Round(cprice, 2);
                                nn.tax = (float)Math.Round(MainFunction.NullToFloat(XK["Tax"].ToString()), 2);
                                nn.qty = dispQty;
                                nn.UnitList = XK["unit"].ToString();
                                nn.amount = 0;
                                nn.NewUomName = XK["unit"].ToString();
                                nn.DispatchQty = 0;
                                nn.qoh2 = qoh;
                                nn.Drugtype = (int)XK["DrugType"];
                                nn.conversionqty = (int)XK["Conversionqty"];
                                nn.batchid = XK["Batchid"].ToString();
                                nn.purqty = 0;
                                nn.MinLevel = (int)XK["MinLevel"];
                                nn.lsmallqty = lSmallQty;
                                nn.NewUomID = int.Parse(XK["UnitID"].ToString()); ;
                                nn.PrescriptionID = itm.mPrescription;
                                nn.Deductabletype = 0;
                                nn.DeductablePerAmounttype = 0;
                                nn.DeductablePerAmount = 0;
                                nn.DiscountPerAmountType = 0;
                                nn.DiscountPerAmount = 0;
                                if (GenericID == 0)
                                {                                      nn.OrderedItem = XK["Name"].ToString();
                                    nn.OrderedItemid = int.Parse(XK["ID"].ToString());
                                }
                                else
                                {
                                    nn.OrderedItem = itm.OrderedItemName;
                                    nn.OrderedItemid = itm.OrderItemId;

                                }
                                nn.temp3 = null;
                                nn.Name = XK["Item"].ToString();
                                nn.ItemCode = XK["Code"].ToString();
                                nn.ID = int.Parse(XK["ID"].ToString());

                                if (itm.InsertItemValue == null)
                                {
                                    xx.Add(nn);
                                    ItemInsert.InsertItemValue = xx;
                                }
                                else
                                {
                                    if (GenericID == 0)
                                    {
                                        itm.InsertItemValue.Add(nn);
                                        ItemInsert.InsertItemValue = itm.InsertItemValue;
                                    }
                                    else
                                    {                                          for (int i = 0; i < itm.InsertItemValue.Count; i++)
                                        {
                                            if (itm.InsertItemValue[i].ID != itm.OrderItemId)
                                            {
                                                itm.InsertItemValue[i].SNO = i + 1;
                                                xx.Add(itm.InsertItemValue[i]);
                                            }
                                            else
                                            {
                                                nn.SNO = i + 1;
                                                xx.Add(nn);

                                            }

                                        }
                                        ItemInsert.InsertItemValue = xx;
                                    }
                                }
                            }
                        }



                    }

                    if (strCrtItems.Length > 0)
                    {
                        ItemInsert.ErrMsg = "Following are Critical Item(s):  " + strCrtItems;
                        ItemInsert.ErrorFlag = true;
                        return ItemInsert;
                    }

                } 









                
                return ItemInsert;
                                  
            }
            catch (Exception e)
            {
                Item i = new Item();
                i.ErrorFlag = true;
                i.ErrMsg = "Error Loading this Item : " + e.Message;
                return i;


            }

        }
        public static List<InsertedItemsList> ListItemSelect(List<ItemSelectParam> itm, int stationid, int mItemID = 0, List<ItemSelectParam> SelectedItemList = null)
        {
                         try
            {
                List<InsertedItemsList> ll = new List<InsertedItemsList>();
                int counter = 1;
                bool hold = false;
                
                for (int y = 0; y < itm.Count; y++)
                {
                    if (hold == false && itm[y].InsertItemValue != null && itm[y].InsertItemValue.Count > 0)
                    {
                        for (int j = 0; j < itm[0].InsertItemValue.Count; j++)
                        {
                            InsertedItemsList xx = new InsertedItemsList();
                            itm[0].InsertItemValue[j].SNO = counter;
                            counter++;
                            xx = itm[0].InsertItemValue[j];
                            ll.Add(xx);
                            hold = true;
                        }
                    }
                    if (hold == true)
                    {
                        
                        itm[y].InsertItemValue = null;
                    }

                }

                string ErrMsg = "";
                for (int i = 0; i < itm.Count; i++)
                {
                    try
                    {
                        InsertedItemsList nn = new InsertedItemsList();
                        itm[i].StationID = stationid;
                        Item itt = new Item();
                        itt = ItemSelect(itm[i]);
                        if (itt.AlertDrugInteraction == true || itt.AlertItemIssuedAlerdy == true || itt.ErrMsg != null)
                        {
                            ErrMsg = "Some Items need to be Inserted Manually from Prescription Details Tab";
                            

                        }
                        else
                        {

                            nn = itt.InsertItemValue[0];
                            nn.SNO = counter;
                                                         counter++;
                            ll.Add(nn);

                        }
                    }
                    catch (Exception e)
                    {
                        
                        ll[0].ErrMsg = "Some Items need to be Inserted Manually from Prescription Details Tab";
                    }
                }
                ll[0].ErrMsg = ErrMsg;
                return ll;
            }
            catch (Exception e)
            {
                List<InsertedItemsList> i = new List<InsertedItemsList>();
                i[0].ErrMsg = "Error Loading Prescription Items : " + e.Message;
                return i;


            }

        }
                 public static string DrugInteraction(ref ItemSelectParam iitm, int lngItemId = 0, string strName = "", int intType = 0, IList<InsertedItemsList> fgItem = null)
        {
            try
            {
                int intRow;
                String strBrandItem;
                String StrDrugInteraction = "";
                String StrSql;

                strBrandItem = "";
                iitm.blnDrugInteraction = false;
                if (fgItem.Count == 1) { return ""; }
                                 for (intRow = 0; intRow < fgItem.Count; intRow++)
                {
                    if (intRow == 0)
                                         { strBrandItem = fgItem[intRow].ID.ToString(); }
                    else
                    {
                                                 strBrandItem = strBrandItem + "," + fgItem[intRow].ID.ToString();

                    }

                }

                if (intType == 1)
                {
                    StrSql = "select Distinct a.name,c.id,c.Name,b.discription,b.Name as ItemName "
                        + " from M_Generic a,M_Generic c,"
                        + " (select Distinct (a.genericID),a.InteractingGenericId, "
                        + " a.discription,c.Name from l_DrugDrugInteraction a,Itemgeneric b ,Item c, ItemGeneric Uq "
                        + " where c.id=b.itemid and b.itemid in (" + strBrandItem + ") and b.genericid=a.genericid and uq.itemid = " + lngItemId + "  "
                        + " and a.InteractingGenericId = uq.genericid )b "
                        + " Where a.ID = b.genericid And c.ID = b.InteractingGenericId ";
                }
                else
                {

                    StrSql = "Select distinct l.Genericid,l.InteractingGenericid, l.Discription,i.Name as ItemName from l_DrugDrugInteraction l,Item i, "
                           + " (Select Itemid,GenericId from ItemGeneric where Itemid = " + lngItemId + ") a, "
                           + " (Select Itemid,GenericId from ItemGeneric where Itemid in (" + strBrandItem + ")) b"
                           + " Where l.GenericId = a.GenericId And l.InteractingGenericId = b.GenericId"
                           + " and i.id = b.Itemid"
                           + " Union all"
                           + " Select distinct l.Genericid,l.InteractingGenericid, l.Discription,i.Name as ItemName from l_DrugDrugInteraction l,Item i,"
                           + " (Select Itemid,GenericId from ItemGeneric where Itemid in (" + strBrandItem + ")) a,"
                           + " (Select Itemid,GenericId from ItemGeneric where Itemid = " + lngItemId + ") b"
                           + " Where l.GenericId = a.GenericId And l.InteractingGenericId = b.GenericId"
                           + " and i.id = a.Itemid";
                }

                if (intType == 1) { iitm.StrWhole = ""; }

                DataSet ds = MainFunction.SDataSet(StrSql, "tbl");
                if (ds.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {

                        if (StrDrugInteraction.Trim() != "" && StrDrugInteraction != null)
                        {
                            StrDrugInteraction = "<--> When Interacts with  " + (string)rr["ItemName"] + "   " + (string)rr["Discription"];
                        }
                        else
                        {
                            StrDrugInteraction = "When Interacts with  " + (string)rr["ItemName"] + "   " + (string)rr["Discription"];
                        }

                        if (iitm.StrWhole.Trim() != "" && iitm.StrWhole != null)
                        {
                            if (intType == 1)
                            {
                                iitm.StrWhole = iitm.StrWhole + StrDrugInteraction + " <---> ";
                            }
                            else
                            {
                                iitm.StrWhole = iitm.StrWhole + "<--->" + strName + StrDrugInteraction + " <---> ";
                            }
                        }
                        else
                        {
                            iitm.StrWhole = strName + " <---> " + StrDrugInteraction + " <---> ";

                        }
                    }


                }

                                 return iitm.blnDrugInteraction.ToString();
            }
            catch (Exception e) { return ""; }

        }
        public static List<TempListMdl> ItemUOMList(int ItemID)
        {
            List<TempListMdl> nn = new List<TempListMdl>();
            DataSet ds = MainFunction.SDataSet("Select P.Id ID,P.Name Name from ItemPacking I,Packing P where P.Id=I.PackId and itemid=" + ItemID + " order by name", "tb");
            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                TempListMdl n = new TempListMdl();
                n.ID = (int)rr["ID"];
                n.Name = rr["Name"].ToString();
                nn.Add(n);
            }
            return nn;


        }
        public static List<TempListMdl> ItemGenericList(int ItemID, int Stationid)
        {
            List<TempListMdl> nn = new List<TempListMdl>();
            string ss = "select a.id,a.name,a.itemcode from Item a, itemgeneric b,batchstore s,BATCH SS "
         + " where a.id=b.itemid and a.id=s.itemid and  s.quantity>0 and s.STATIONID=" + Stationid + " AND "
         + " S.BATCHID=SS.BATCHID AND S.BATCHNO=SS.BATCHNO AND S.ITEMID=SS.ITEMID AND "
         + " b.genericid in(select genericid from itemgeneric where  itemid = " + ItemID + " ) "
         + " group by  a.id,a.name,a.itemcode ";

            DataSet ds = MainFunction.SDataSet(ss, "tb");
            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                TempListMdl n = new TempListMdl();
                n.ID = (int)rr["ID"];
                n.Name = rr["Name"].ToString();
                nn.Add(n);
            }

            if (nn.Count == 0)
            {
                string ssx = "select a.id,a.name,a.itemcode from Item a where id=" + ItemID;
                DataSet Oneds = MainFunction.SDataSet(ssx, "tb");
                foreach (DataRow xx in Oneds.Tables[0].Rows)
                {
                    TempListMdl n = new TempListMdl();
                    n.ID = (int)xx["ID"];
                    n.Name = xx["Name"].ToString();
                    nn.Add(n);
                }


            }

            return nn;


        }
        public static InsertedItemsList GetNewConverisionInfo(InsertedItemsList xx)
        {
            try
            {
                long lSmallQty;
                long lconvqty;
                int lGetQty;
                decimal curSp;
                decimal Tax;
                string pQty;
                decimal amt;
                long CurQty;

                CurQty = xx.lsmallqty / MainFunction.GetQuantity((int?)xx.NewUomID, xx.ID);

                pQty = xx.batchid;
                lconvqty = xx.conversionqty;
                if (lconvqty == 0) { lconvqty = 1; }

                lGetQty = MainFunction.GetQuantity((int?)xx.NewUomID, xx.ID);
                lSmallQty = lconvqty * xx.qoh;

                if ((long)(xx.DispatchQty * lGetQty) > lSmallQty)
                { xx.DispatchQty = xx.qoh2 / lGetQty; }





                curSp = Math.Round(xx.price / lconvqty, 4);

                amt = curSp * lGetQty * xx.DispatchQty;

                Tax = amt * (decimal)xx.tax;
                Tax = Tax * (decimal)0.01;
                xx.amount = Math.Round(amt + Tax, 2);


                return xx;
            }
            catch (Exception e) { xx.ErrMsg = "Error converting Item Info"; return xx; }

        }
        public static int CheckLOAAppServices(long ServiceId, long DeptId, long Itemid, long AuthId, String gIACode, int regNo)
        {
            try
            {
                int tempVal = 0;
                if (AuthId == 0) { return 0; }
                DataSet ds = MainFunction.SDataSet("Exec PR_CheckOPLOAAppovalsDept " + AuthId + ", " + ServiceId + "," + Itemid + "", "tbl");
                int AppQty = 1;
                List<AppItemService> AppItemService = new List<AppItemService>();
                int i = 0;
                if (ds.Tables[0].Rows.Count > 1)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        tempVal = (int)rr["Type"];
                        AppItemService[i].appid = (long)rr["OrderNo"];
                        AppItemService[i].ServiceId = (long)rr["ServiceId"];
                        AppItemService[i].Itemid = (long)rr["Itemid"];
                        AppQty = (int)rr["Qty"];
                        i += 1;
                    }

                }
                else
                {

                    string StrSql = "Select 1 Type,Qty,OrderNo=0,ServiceId,ItemId,DeptId from ARReleaseExclusions "
                      + " where Serviceid= " + ServiceId + " and Deptid = 0 and ItemId =" + Itemid + " and OpBillId=0 and RegNo=" + regNo
                      + " and IACode ='" + gIACode + "'";

                    DataSet ns = MainFunction.SDataSet(StrSql, "tbl2");
                    if (ns.Tables[0].Rows.Count <= 0)
                    {
                        ns.Clear();
                        StrSql = "Select 1 Type,Qty,OrderNo=0,ServiceId,ItemId,Deptid from ARReleaseExclusions "
                             + " where OpBillId=0 and DeptId= " + DeptId + " and RegNo=" + regNo + " and IACode ='" + gIACode + "' and ItemId=0";
                        ns = MainFunction.SDataSet(StrSql, "tbl2");
                    }
                    else if (ns.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rr in ns.Tables[0].Rows)
                        {
                            if (AppItemService.Count > 0) { tempVal = 1; return tempVal; }
                            tempVal = (int)rr["Type"];
                            AppItemService[i].appid = (long)rr["OrderNo"];
                            AppItemService[i].ServiceId = (long)rr["ServiceId"];
                            AppItemService[i].Itemid = (long)rr["Itemid"];
                            AppQty = (int)rr["Qty"];
                            i += 1;
                        }
                    }
                    else
                    {
                        tempVal = 0;
                        AppQty = 1;
                    }
                }
                return 1;
            }
            catch (Exception e)
            {
                                 return 0;
            }

        }
        public static Deposit GetDepositInfo(Deposit dt)
        {
            try
            {
                dt.optCrPat = false;
                string StrSql = "";
                StrSql = " Select a.id,a.Amount - (IsNUll(Sum(b.BillAmount),0) + isnull((select sum(amount) " +
                         " FROM iptransactions where receiptno = SUBSTRING(a.receiptno,2,20) group by ReceiptNo), 0)) deposit, " +
                         " isnull((select sum(amount) FROM iptransactions " +
                         " where receiptno = SUBSTRING(a.receiptno,2,20) group by ReceiptNo), 0) ipamount" +
                         " FROM  OpDeposit a, OPDepositDetails b where a.id*=b.DepositId and a.ReceiptNo = '" + dt.DepositChkNo + "'" +
                         " And billno is not null AND RegistrationNo=" + dt.RegNo + " and IssueAuthorityCode ='" + dt.gIACode + "' group by a.amount,a.id,a.receiptno ";
                DataSet ds = MainFunction.SDataSet(StrSql, "tb");
                if (ds.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        dt.NotifyMsg = "Balance Amount for this Receipt :  " + (decimal)rr["deposit"];
                        dt.NetBalance = (decimal)rr["deposit"];
                        dt.IPBalance = (decimal)rr["ipamount"];
                        dt.DepositID = (int)rr["id"];
                    }
                }
                else if (ds.Tables[0].Rows.Count == 0)
                {
                    StrSql = "Select a.id,a.RegistrationNo,a.IssueAuthorityCode" +
                             ",a.Amount - (IsNUll(Sum(b.BillAmount),0) + isnull((select sum(amount) " +
                             " FROM iptransactions where receiptno = SUBSTRING(a.receiptno,2,20) group by ReceiptNo), 0)) deposit, " +
                             " isnull((select sum(amount) FROM iptransactions " +
                             " WHERE receiptno = SUBSTRING(a.receiptno,2,20) group by ReceiptNo), 0) ipamount" +
                             " from  OpDeposit a, OPDepositDetails b,OpDepDependents c" +
                             " where a.Id = c.DepositId and a.id = b.DepositId" +
                             " and a.ReceiptNo ='" + dt.DepositChkNo + "' and c.RegNo=" + dt.RegNo +
                             " and billno is not null and IACode ='" + dt.gIACode + "' group by a.amount,a.id,a.RegistrationNo" +
                             ",a.IssueAuthorityCode,a.receiptno";
                    DataSet dss = MainFunction.SDataSet(StrSql, "tb");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rr in dss.Tables[0].Rows)
                        {
                            dt.NotifyMsg = "Balance Amount for this Receipt :  " + (decimal)rr["deposit"] +
                                "    ;Deposited By PIN : " + MainFunction.getUHID((string)rr["IssueAuthorityCode"], (string)rr["RegistrationNo"], false);
                            dt.NetBalance = (decimal)rr["deposit"];
                            dt.IPBalance = (decimal)rr["ipamount"];
                            dt.DepositID = (int)rr["id"];

                        }
                    }

                    else
                    {

                        dt.NotifyMsg = "Receipt Number Does Not Exists";
                        dt.NetBalance = 0;
                        dt.IPBalance = 0;
                        dt.DepositID = 0;

                    }
                }

                return dt;
            }
            catch (Exception e) { dt.ErrMsg = "Can't get the Deposit Information!"; return dt; }

        }
        public static bool ServiceLoaCheck(ref ItemSelectParam tt)
        {
            try
            {
                int listcount = 0;
                if (tt.InsertItemValue != null)
                { listcount = tt.InsertItemValue.Count; }
                if (listcount > 1) { return true; }


                DataSet ds = MainFunction.SDataSet("select count(*) as cnt from opserviceloa" +
                    " where categoryid = " + tt.mCategoryid + " and companyid = " + tt.cmbCompany
                    + " and gradeid = " + tt.mGradeId + " and opserviceid = 11", "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    if ((int)rr["cnt"] == 0)
                    {
                        tt.ErrMsg = "LOA limits are not defined.Please contact AR Dept.";
                        return false;
                    }
                }



                DataSet ds2 = MainFunction.SDataSet("select count(*) as cnt from opLoa" +
               "  where categoryid =  " + tt.mCategoryid + " and companyid = " + tt.cmbCompany
               + " and gradeid = " + tt.mGradeId, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    if ((int)rr["cnt"] == 0)
                    {
                        tt.ErrMsg = "LOA limits are not defined.Please contact AR Dept.";
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                tt.ErrMsg = "Error Getting Service LOA Limits";
                return false;
            }
        }
        
        public static Patient Bill(Patient Patient)
        {
            try
            {
                int iDedType, idedpertype, iDisPerType, gDedPerType, mDeducttype, gDisPerType;
                decimal idedperamt, iDisPerAmt, mDeductper, mDisAmt, mDedAmt;
                string st;
                
                iDedType = 0; idedpertype = 0; iDisPerType = 0; gDedPerType = 0; mDeducttype = 0; gDisPerType = 0;
                idedperamt = 0; iDisPerAmt = 0; mDeductper = 0; mDisAmt = 0; mDedAmt = 0;


                if (validate(Patient) == true)
                {

                    if (Patient.BillisCredit == true)
                    {
                        st = "select deductable,percentage,amount from opcompanydeptdeductable where categoryid = " + Patient.CategoryId
                           + "  and gradeid = " + Patient.GradeId + " and companyid = " + Patient.CompanyId
                           + " and serviceid = 11 and departmentid = " + Patient.mDeptId;
                        DataSet ds1 = MainFunction.SDataSet(st, "t1");

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in ds1.Tables[0].Rows)
                            {
                                if ((decimal)rr["amount"] > 0)
                                {
                                    mDeductper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                    gDedPerType = 2;
                                }
                                else
                                {
                                    mDeductper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                    gDedPerType = 1;
                                }

                            }
                        }
                        else
                        {
                            st = "select deductable,percentage,amount from opcompanygradedeductable where categoryid = " + Patient.CategoryId
                              + "  and gradeid = " + Patient.GradeId + " and companyid = " + Patient.CompanyId
                              + " and serviceid = 11";
                            DataSet ds2 = MainFunction.SDataSet(st, "t2");
                            if (ds2.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rr in ds2.Tables[0].Rows)
                                {
                                    if ((decimal)rr["amount"] > 0)
                                    {
                                        mDeductper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                        gDedPerType = 2;
                                    }
                                    else
                                    {
                                        mDeductper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                        gDedPerType = 1;
                                    }
                                }
                            }


                        }
                        Patient.mDeductper = mDeductper;

                        st = "select  Deductable from IPDeductableType where IPOPType=1 AND " +
                             " categoryid = " + Patient.CategoryId + " and gradeid = " + Patient.GradeId + " and " +
                             " companyid = " + Patient.CompanyId;
                        DataSet ds3 = MainFunction.SDataSet(st, "t3");
                        if (ds3.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in ds3.Tables[0].Rows)
                            {
                                if ((int)rr["Deductable"] == -1) { mDeducttype = 0; }
                                else { mDeducttype = (int)rr["Deductable"]; }
                            }
                        }
                        else { mDeducttype = 0; }


                        st = "select percentage,amount from opcompanydeptdiscount where categoryid = " + Patient.CategoryId
                            + " and companyid = " + Patient.CompanyId + " and serviceid = 11 and gradeid = " + Patient.GradeId
                            + " and departmentid = " + Patient.mDeptId;
                        DataSet ds4 = MainFunction.SDataSet(st, "t4");
                        if (ds4.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in ds4.Tables[0].Rows)
                            {
                                if ((decimal)rr["amount"] > 0)
                                {

                                    Patient.mdisper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                    gDisPerType = 1;
                                }
                                else
                                {

                                    Patient.mdisper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                    gDisPerType = 0;
                                }
                            }
                        }
                        else
                        {
                            st = "select percentage,amount from opcompanyservicediscount where categoryid = " + Patient.CategoryId
                              + " and companyid = " + Patient.CompanyId + " and serviceid = 11 and gradeid = " + Patient.GradeId;
                            DataSet ds5 = MainFunction.SDataSet(st, "t5");
                            if (ds5.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rr in ds5.Tables[0].Rows)
                                {
                                    if ((decimal)rr["amount"] > 0)
                                    {

                                        Patient.mdisper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                        gDisPerType = 1;
                                    }
                                    else
                                    {

                                        Patient.mdisper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                        gDisPerType = 0;
                                    }
                                }



                            }

                            for (int i = 0; i < Patient.InsertItemValue.Count; i++)
                            {
                                st = "select deductable,percentage,amount from opcompanyitemdeductable where companyid = " + Patient.CompanyId
                                    + " and categoryid = " + Patient.CategoryId + " and gradeid = " + Patient.GradeId
                                    + " and serviceid = 11 and itemid = " + Patient.InsertItemValue[i].ID;
                                DataSet ds6 = MainFunction.SDataSet(st, "t6");
                                if (ds6.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow rr in ds6.Tables[0].Rows)
                                    {
                                        if ((decimal)rr["amount"] > 0)
                                        {
                                            idedperamt = MainFunction.NullToDecmial(rr["amount"].ToString());
                                            idedpertype = 1;
                                        }
                                        else
                                        {
                                            idedperamt = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                            idedpertype = 0;
                                        }
                                        iDedType = MainFunction.NullToInteger(rr["deductable"].ToString());
                                    }


                                }
                                else
                                {
                                    idedperamt = mDeductper;
                                    idedpertype = gDedPerType;
                                    iDedType = mDeducttype;
                                }

                                
                                st = "select percentage,amount from opcompanyitemdiscount where companyid = " + Patient.CompanyId
                                + " and categoryid = " + Patient.CategoryId + " and gradeid = " + Patient.GradeId
                                + " and serviceid = 11 and itemid = " + Patient.InsertItemValue[i].ID;
                                DataSet ds7 = MainFunction.SDataSet(st, "t7");
                                if (ds7.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow rr in ds7.Tables[0].Rows)
                                    {
                                        if ((decimal)rr["amount"] > 0)
                                        {
                                            iDisPerAmt = MainFunction.NullToDecmial(rr["amount"].ToString());
                                            iDisPerType = 1;
                                        }
                                        else
                                        {
                                            iDisPerAmt = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                            iDisPerType = 0;

                                        }
                                    }

                                }
                                else
                                {
                                    iDisPerAmt = Patient.mdisper;
                                    iDisPerType = gDisPerType;

                                }
                                Patient.InsertItemValue[i].Deductabletype = iDedType;
                                Patient.InsertItemValue[i].DeductablePerAmounttype = idedpertype;
                                Patient.InsertItemValue[i].DeductablePerAmount = idedperamt;
                                Patient.InsertItemValue[i].DiscountPerAmountType = iDisPerType;
                                Patient.InsertItemValue[i].DiscountPerAmount = iDisPerAmt;

                            }







                        }
                    } 
                    if (InitBatchArray(ref Patient) == false)
                    { return Patient; }

                                         string TempListMinLevel = "";
                    foreach (InsertedItemsList xxt in Patient.InsertItemValue)
                    {
                        if (MainFunction.CheckQOH_Min(xxt.ID, xxt.ItemCode, (int)(long)xxt.conversionqty, xxt.MinLevel,
                            (long)(int)xxt.qoh, (int)(decimal)xxt.qty) == true)
                        {
                            Patient.QOHMinLevelFlag = true;
                            TempListMinLevel = TempListMinLevel + ',' + xxt.ItemCode;
                        }
                    }
                    if (Patient.QOHMinLevelFlag == true)
                    {
                        Patient.ErrMsg = TempListMinLevel;
                                             }

                    mDisAmt = 0;
                    mDedAmt = 0;
                                         for (int i = 0; i < Patient.arrItem.Count; i++)
                    {
                        mDisAmt = mDisAmt + Patient.arrItem[i].disamt;
                        mDedAmt = mDedAmt + Patient.arrItem[i].dedAmt;
                    }



                    
                    if (Patient.BillisCredit == true)
                    {
                        Patient.lblCreditBillAmount = Patient.TotalAmount;                         Patient.lblCreditDiscount = mDisAmt;                         Patient.lblNetAmount = Patient.TotalAmount - mDisAmt;                         Patient.lbldedamt = mDedAmt;                         Patient.lbldonationAMT = 0;                         Patient.txtAmountToBeCollected = mDedAmt;                         Patient.lblBalance = Patient.TotalAmount - mDisAmt - mDedAmt; 
                                                 decimal lblNet = Patient.lblNetAmount;
                        if (CheckLOAAmount(ref Patient, lblNet) == false)
                        {
                            Patient.ErrMsg = "LOA Amount Exceeded. Need for Approval.";
                            return Patient;
                        }
                    }
                    else
                    {
                        if (Patient.Registrationno > 0) { Patient.lblCreditDiscount = Patient.mdisper; }
                        else { mDisAmt = 0; Patient.lblCreditDiscount = 0; }


                        Patient.lblCreditBillAmount = Patient.TotalAmount;
                        Patient.lblCreditDiscount = mDisAmt;                         Patient.lblNetAmount = Patient.TotalAmount - mDisAmt;                         Patient.lbldedamt = 0;                         Patient.lbldonationAMT = 0;                         Patient.txtAmountToBeCollected = Patient.TotalAmount - mDisAmt;                         Patient.lblBalance = 0; 

                    }
                    if (Patient.strExpBatch == null) { Patient.strExpBatch = ""; }
                    if (Patient.strExpBatch.Length > 0)
                    {
                        Patient.BatchExpiryDialogTXT = "Following batches found expired." + Patient.strExpBatch + ", Do you want to continue?";
                        Patient.BatchExpiryDialog = true;
                                             }

                    
                    decimal GetDecimal = 0;
                    if ((Patient.txtAmountToBeCollected != (int)Patient.txtAmountToBeCollected) &&
                        Patient.txtAmountToBeCollected > 0)
                    {
                        GetDecimal = ((int)(Patient.txtAmountToBeCollected) + 1) - Patient.txtAmountToBeCollected;
                        Patient.lbldonationAMT = GetDecimal;
                    }

                    return Patient;
                }
                else
                {

                    return Patient;
                } 

                return Patient;
            }
            catch (Exception e)
            {
                Patient.ErrMsg = "Error when try processing this order";
                return Patient;
            }


        }
        public static bool validate(Patient Patient)
        {
            if (Patient.BillisCredit == true)
            {
                if (Patient.mLOAConsultation > 0)
                {
                    int xdays = 0; long xloan = 0;
                    if (CheckLOADays(ref Patient, ref xdays, ref xloan) == false)
                    {
                        Patient.ErrMsg = "LOA Days Exceeded.Contact AR Department.LOA Days :  " +
                            xloan + " . Bill days:  " + xdays;
                        return false;
                    }

                }
            }

            if (ValidatePrescription(Patient.Registrationno, Patient.IssueAuthorityCode, Patient.gStationID, Patient.DoctorId) == true
                && Patient.mPresid == 0)
            {
                Patient.PrescriptionDialogTxt = "Please select the Prescription! click on View Order button.";
                Patient.PrescriptionDialog = true;
                             }



            return true;
        }

        public static bool ValidatePrescription(int mRegNo, string gIaCode, int gStationid, int? docID)
        {
            try
            {
                string sql = "";
                if (gStationid == 18)
                {
                    sql = "SELECT cp.ID as [Order No],cp.VisitID as VisitID,cp.DateTime as [Date Time], " +
         " e.Name as Operator,s.Name as Station,cp.doctorid,cp.corder, 1 prescription " +
         " FROM employee e,Station s,clinicalprescription cp " +
         " WHERE cp.issueauthoritycode = '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid " +
         " and cp.datetime >= getdate() - 30 and " +
         " (select count(*) from ClinicalPrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " Union All SELECT cp.ID as [Order No],cp.VisitID,cp.DateTime, " +
         " e.Name as Operator ,s.Name as Station,cp.doctorid,0 as corder, 0 prescription " +
         " FROM employee e,Station s,MSF_Prescription cp WHERE cp.deleted=0 and cp.issueauthoritycode = '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid " +
         " and cp.datetime >= getdate() - 30 and (select count(*) from " +
         " MSF_PrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " order by cp.datetime ";

                }
                else
                {
                    sql = "SELECT cp.ID as [Order No],cp.VisitID as VisitID,cp.DateTime as [Date Time], " +
         " e.Name as Operator,s.Name as Station,cp.doctorid,cp.corder, 1 prescription " +
         " FROM employee e,Station s,clinicalprescription cp " +
         " WHERE cp.issueauthoritycode = '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid and e.id=" + docID +
         " and cp.datetime >= getdate() - 30 and " +
         " (select count(*) from ClinicalPrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " Union All SELECT cp.ID as [Order No],cp.VisitID,cp.DateTime, " +
         " e.Name as Operator ,s.Name as Station,cp.doctorid,0 as corder, 0 prescription " +
         " FROM employee e,Station s,MSF_Prescription cp WHERE cp.deleted=0 and cp.issueauthoritycode =  '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid and e.id=" + docID +
         " and cp.datetime >= getdate() - 30 and (select count(*) from " +
         " MSF_PrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " order by cp.datetime ";


                }
                DataSet dt = MainFunction.SDataSet(sql, "tb");
                if (dt.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {

                    return false;
                }



                return true;
            }
            catch (Exception e) { return false; }


        }
        public static bool InitBatchArray(ref Patient pt, SqlTransaction Trans = null)
        {
            try
            {
                                 List<InsertedItemsList> arrItem = new List<InsertedItemsList>();


                decimal TotReqQty, mQuantity, mQuantityBat;
                int lGetQty;
                long chkQty; int lItemid = 0;

                                                                    
                if (pt.Registrationno != 0 && pt.BillisCredit == true)
                {
                    string str = "select count(*) as c1 from employee where regno = " + pt.Registrationno
                        + " and deleted = 0";

                    DataSet ds = MainFunction.SDataSet(str, "tb", Trans);

                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        if ((int)rr["c1"] != 0 && pt.Registrationno != 0)
                        {
                            DataSet dss = MainFunction.SDataSet("select percentage from employeediscountpercent where deleted =0", "tb", Trans);
                            if (dss.Tables[0].Rows.Count > 0)
                            {
                                pt.mdisper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                pt.mStaff = true;
                            }
                            else { pt.mdisper = 0; }
                        }
                    }
                }

                decimal PHAmt = pt.gOPPHAmtLmt;
                                 for (int i = 0; i < pt.InsertItemValue.Count; i++)
                {
                                         lGetQty = MainFunction.GetQuantity((int?)pt.InsertItemValue[i].NewUomID, (int)pt.InsertItemValue[i].ID);
                    mQuantityBat = pt.InsertItemValue[i].qty * lGetQty;
                    mQuantity = mQuantityBat / lGetQty;
                    TotReqQty = mQuantity;
                    if (mQuantity > 0)
                    {
                        string sss = "Select b.mrp,b.costprice,Bs.Quantity,B.ItemId AS Id,"
                            + " i.itemcode,i.name,B.BatchNo,b.batchid, b.expirydate "
                            + " FROM batch b, BatchStore Bs,item i"
                            + " where B.ItemId =" + pt.InsertItemValue[i].ID
                            + " and Bs.ItemId =" + pt.InsertItemValue[i].ID
                            + " and B.BatchNo = Bs.BatchNo "
                            + " and B.Batchid = Bs.Batchid "
                            + " and bs.itemid=i.id and b.itemid = i.id "
                            + " and Bs.StationId = " + pt.gStationID
                            + " and Bs.Quantity > 0 "
                            + " Order By b.expirydate,B.Startdate";
                        DataSet getdtl = MainFunction.SDataSet(sss, "tbl", Trans);
                        foreach (DataRow rr in getdtl.Tables[0].Rows)
                        {
                            DateTime exCdate = new DateTime(1900, 1, 1);
                            DateTime b = (DateTime)rr["Expirydate"];
                            DateTime Expirydate = new DateTime(b.Year, b.Month, b.Day);
                            if ((Expirydate != exCdate) &&
                                (Expirydate <= DateTime.Now) &&
                                Expirydate != null
                                )
                            {
                                pt.strExpBatch = pt.strExpBatch + ":" + rr["BatchNo"].ToString()
                                    + ",BatchNo:" + rr["BatchNo"].ToString() + " ,Drug :"
                                    + pt.InsertItemValue[i].DrugName;
                            }

                            if ((int)rr["Id"] == lItemid)
                            {
                                if (
                                    ((int)rr["Quantity"] - pt.InsertItemValue[i].qty == 0) &&
                                    (pt.InsertItemValue[i - 1].Name == pt.InsertItemValue[i].Name))
                                {
                                    goto _NextRec;
                                }
                            }

                            if (((int)rr["Quantity"] / lGetQty) >= mQuantity)
                            {
                                InsertedItemsList it = new InsertedItemsList();
                                it.ID = (int)rr["Id"];
                                it.BatchNo = rr["BatchNo"].ToString();
                                it.batchid = rr["Batchid"].ToString();
                                it.ItemCode = rr["ItemCode"].ToString();
                                it.Name = rr["Name"].ToString();
                                it.qty = mQuantity;
                                it.DedQty = (int)(it.qty * lGetQty);
                                it.OrderedItemid = pt.InsertItemValue[i].OrderedItemid;
                                it.PrescriptionID = pt.InsertItemValue[i].PrescriptionID;
                                it.NewUomID = pt.InsertItemValue[i].NewUomID;
                                it.tax = pt.InsertItemValue[i].tax;
                                it.Epr = (decimal)rr["CostPrice"] * lGetQty;
                                it.price = ((decimal)rr["MRP"] * lGetQty) * (decimal)(1 + it.tax / 100);
                                it.ItemTotal = Math.Round((it.qty * ((decimal)rr["MRP"] * lGetQty) * (decimal)(1 + (it.tax / 100))), 2);

                                if (pt.BillisCredit == true)
                                {
                                    if (pt.CategoryId == 43)
                                    {
                                        if (pt.InsertItemValue[i].DeductablePerAmounttype == 0)
                                        {
                                            it.dedAmt = Math.Round(it.qty * pt.InsertItemValue[i].DeductablePerAmount, 2);
                                        }
                                        else
                                        {
                                            if (pt.InsertItemValue[i].Deductabletype == 1)
                                            {
                                                if (PHAmt > it.ItemTotal)
                                                { it.dedAmt = Math.Round((it.ItemTotal * pt.InsertItemValue[i].DiscountPerAmount) / 100, 2); }
                                                else
                                                {
                                                    it.dedAmt = Math.Round(((it.ItemTotal - PHAmt) * pt.InsertItemValue[i].DiscountPerAmount) / 100, 2);
                                                }
                                            }
                                            else
                                            {
                                                if (PHAmt > it.ItemTotal - it.disamt)
                                                {
                                                    it.dedAmt = Math.Round(((it.ItemTotal - it.disamt) * pt.InsertItemValue[i].DiscountPerAmount) / 100, 2);
                                                }
                                                else
                                                {
                                                    it.dedAmt = Math.Round(((it.ItemTotal - it.disamt - PHAmt) * pt.InsertItemValue[i].DiscountPerAmount) / 100, 2);
                                                }

                                            }
                                            if (pt.gOPPHAmtLmt < it.ItemTotal) { PHAmt = 0; }
                                            if (PHAmt < it.ItemTotal && pt.InsertItemValue[i].DeductablePerAmounttype > 0) { PHAmt = 0; }
                                        }

                                    }                                     else
                                    {
                                        if (pt.InsertItemValue[i].DeductablePerAmounttype == 0)
                                        {
                                            it.dedAmt = Math.Round(
                                              it.qty * pt.InsertItemValue[i].DeductablePerAmount
                                              , 2);
                                        }
                                        else
                                        {
                                            if (pt.InsertItemValue[i].Deductabletype == 1)
                                            {
                                                it.dedAmt = Math.Round(
                                                (it.ItemTotal * pt.InsertItemValue[i].DeductablePerAmount) / 100
                                                , 2);
                                            }
                                            else
                                            {
                                                it.dedAmt = Math.Round(
                                                ((it.ItemTotal - it.disamt) * pt.InsertItemValue[i].DeductablePerAmount) / 100
                                                , 2);
                                            }
                                        }
                                    } 
                                }                                 else
                                {
                                    it.Deductabletype = 0;
                                    it.DeductablePerAmounttype = 0;
                                    it.DeductablePerAmount = 0;
                                    it.DiscountPerAmount = 0;
                                    it.DiscountPerAmountType = 0;
                                    if (pt.mStaff == true && pt.Registrationno > 0)
                                    {
                                        it.DiscountPerAmount = pt.mdisper;
                                        it.disamt = Math.Round((it.ItemTotal * pt.mdisper) / 100, 2);
                                    }
                                    else
                                    {
                                        it.DiscountPerAmount = pt.InsertItemValue[i].DiscountPerAmount;
                                        it.disamt = 0;
                                        if (pt.InsertItemValue[i].DiscountPerAmountType == 1)
                                        { it.disamt = Math.Round(it.qty * pt.InsertItemValue[i].DiscountPerAmount, 2); }
                                        else if (pt.InsertItemValue[i].DiscountPerAmountType == 0)
                                        {
                                            it.disamt = Math.Round((it.ItemTotal * pt.InsertItemValue[i].DiscountPerAmount) / 100, 2);
                                        }
                                    }

                                }                                 pt.TotalAmount += it.ItemTotal;
                                mQuantity = 0;
                                arrItem.Add(it);


                            }                              else
                            {
                                InsertedItemsList it = new InsertedItemsList();
                                mQuantity = mQuantity - ((int)rr["Quantity"] / lGetQty);

                                it.ID = (int)rr["Id"];
                                it.BatchNo = rr["BatchNo"].ToString();
                                it.batchid = rr["Batchid"].ToString();
                                it.ItemCode = rr["ItemCode"].ToString();
                                it.Name = rr["Name"].ToString();
                                it.OrderedItemid = pt.InsertItemValue[i].OrderedItemid;
                                it.PrescriptionID = pt.InsertItemValue[i].PrescriptionID;
                                it.qty = ((int)rr["Quantity"] / lGetQty);
                                it.DedQty = (int)(it.qty * lGetQty);
                                it.NewUomID = pt.InsertItemValue[i].NewUomID;
                                it.tax = pt.InsertItemValue[i].tax;
                                it.Epr = (decimal)rr["CostPrice"] * lGetQty;
                                it.price = ((decimal)rr["MRP"] * lGetQty) * (decimal)(1 + (it.tax / 100));
                                it.ItemTotal = Math.Round((it.qty * (((decimal)rr["MRP"] * lGetQty) * (decimal)(1 + (it.tax / 100)))), 2);

                                if (pt.BillisCredit == true)                                 {
                                    if (pt.CategoryId == 43)
                                    {
                                        if (PHAmt - it.ItemTotal > 0)
                                        {
                                            pt.InsertItemValue[i].DeductablePerAmount = 0;
                                            PHAmt = PHAmt - it.ItemTotal;
                                        }
                                    }
                                    it.Deductabletype = pt.InsertItemValue[i].Deductabletype;
                                    it.DeductablePerAmounttype = pt.InsertItemValue[i].DeductablePerAmounttype;
                                    it.DeductablePerAmount = pt.InsertItemValue[i].DeductablePerAmount;
                                    it.DiscountPerAmount = pt.InsertItemValue[i].DiscountPerAmount;
                                    it.DiscountPerAmountType = pt.InsertItemValue[i].DiscountPerAmountType;

                                    if (pt.InsertItemValue[i].DiscountPerAmountType == 1)
                                    { it.disamt = Math.Round(it.qty * pt.InsertItemValue[i].DiscountPerAmount, 2); }
                                    else if (pt.InsertItemValue[i].DiscountPerAmountType == 0)
                                    { it.disamt = Math.Round((it.ItemTotal * pt.InsertItemValue[i].DiscountPerAmount) / 100, 2); }
                                    if (pt.CategoryId == 43)
                                    {
                                        if (pt.InsertItemValue[i].DeductablePerAmounttype == 0)
                                        {
                                            if (PHAmt > it.ItemTotal)
                                            { it.dedAmt = Math.Round(it.qty * pt.InsertItemValue[i].DeductablePerAmount, 2); }
                                            else
                                            { it.dedAmt = Math.Round(it.qty * pt.InsertItemValue[i].DeductablePerAmount, 2); }
                                        }
                                        else
                                        {
                                            if (pt.InsertItemValue[i].Deductabletype == 1)
                                            {
                                                if (PHAmt > it.ItemTotal)
                                                {
                                                    it.dedAmt = Math.Round((it.ItemTotal * pt.InsertItemValue[i].DeductablePerAmount) / 100, 2);
                                                }
                                                else
                                                {
                                                    it.dedAmt = Math.Round(((it.ItemTotal - PHAmt) * pt.InsertItemValue[i].DeductablePerAmount) / 100, 2);
                                                }
                                            }
                                            else
                                            {
                                                if (PHAmt > it.ItemTotal - it.disamt)
                                                {
                                                    it.dedAmt = Math.Round(((it.ItemTotal - it.disamt) * pt.InsertItemValue[i].DeductablePerAmount) / 100, 2);
                                                }
                                                else
                                                {
                                                    it.dedAmt = Math.Round(((it.ItemTotal - it.disamt - PHAmt) * pt.InsertItemValue[i].DeductablePerAmount) / 100, 2);
                                                }
                                            }
                                            if (pt.gOPPHAmtLmt < it.ItemTotal) { PHAmt = 0; }
                                            if (PHAmt < it.ItemTotal && pt.InsertItemValue[i].DeductablePerAmount > 0) { PHAmt = 0; }
                                        }
                                    }
                                    else
                                    {
                                        if (pt.InsertItemValue[i].DeductablePerAmounttype == 0)
                                        {
                                            it.dedAmt = Math.Round(it.qty * pt.InsertItemValue[i].DeductablePerAmount, 2);
                                        }
                                        else
                                        {
                                            if (pt.InsertItemValue[i].Deductabletype == 1)
                                            {
                                                it.dedAmt = Math.Round((it.ItemTotal * pt.InsertItemValue[i].DeductablePerAmount) / 100, 2);
                                            }
                                            else
                                            {
                                                it.dedAmt = Math.Round(((it.ItemTotal - it.disamt) * pt.InsertItemValue[i].DeductablePerAmount) / 100, 2);
                                            }
                                        }
                                    }
                                }                                 else
                                {
                                    it.Deductabletype = 0;
                                    it.DeductablePerAmounttype = 0;
                                    it.DiscountPerAmountType = 0;
                                    it.dedAmt = 0;
                                    if (pt.mStaff == true && pt.Registrationno != 0)
                                    {
                                        it.DiscountPerAmount = pt.mdisper;
                                        it.disamt = Math.Round(
                                            (it.ItemTotal * pt.mdisper) / 100
                                            , 2);
                                    }
                                    else
                                    {
                                        it.DiscountPerAmount = pt.InsertItemValue[i].DiscountPerAmount;
                                        it.disamt = 0;
                                        if (pt.InsertItemValue[i].DiscountPerAmountType == 1)
                                        {
                                            it.disamt = Math.Round(it.qty * pt.InsertItemValue[i].DiscountPerAmount, 2);
                                        }
                                        else
                                        {
                                            it.disamt = Math.Round((it.ItemTotal * pt.InsertItemValue[i].DiscountPerAmount) / 100, 2);
                                        }
                                    }
                                }                                 pt.TotalAmount += it.ItemTotal;
                                arrItem.Add(it);
                            } 


                        _NextRec:
                            lItemid = (int)rr["Id"];
                        }                         if (mQuantity != 0)
                        { pt.ErrMsg = "Insufficient Stock in a Single batch for: " + pt.InsertItemValue[i].ItemCode + " , Change the Issue unit "; return false; }

                    }                     lItemid = pt.InsertItemValue[i].ID;
                } 

                pt.arrItem = arrItem;

                if (pt.BillisCredit == true)
                {
                    if (CalMaxDeductable(ref pt, Trans) == false) { pt.ErrMsg = "Error while Calculating"; return false; }
                }


                return true;
            }
            catch (Exception e)
            {
                pt.ErrMsg = "Error when try to read the Item Batch";
                return false;
            }


        }
        private static bool CalMaxDeductable(ref Patient pt, SqlTransaction Trans = null)
        {
            try
            {
                decimal lngMaxDed = 0;
                bool blnCheckMaxDed = false;
                DateTime LoaExpiryDate;
                string StrSql = "select IsNull(a.Amount,0) Amount, IsNull(IsMaxDeductable,0) IsMaxDeductable from OPMaxDeductable a "
                    + " where a.CategoryID =" + pt.CategoryId + " And "
                    + " a.companyid =" + pt.CompanyId + " and "
                    + " a.gradeid=" + pt.GradeId + " ";
                DataSet ds = MainFunction.SDataSet(StrSql, "tbl", Trans);
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    lngMaxDed = (decimal)rr["Amount"];
                    if (pt.CategoryId > 0 && pt.CategoryId != 43)
                    {
                        if ((bool)rr["IsMaxDeductable"] == true) { blnCheckMaxDed = true; }
                        if (blnCheckMaxDed == true)
                        {
                            if (pt.mAuthorityid > 0)
                            {
                                DataSet dsn = MainFunction.SDataSet("Select LoaExpiryDate, Isnull(NoDays,0) NoOfDays from OPLoaOrder where AuthorityId = "
                                    + pt.mAuthorityid, "tbl", Trans);
                                foreach (DataRow nn in dsn.Tables[0].Rows)
                                {
                                    LoaExpiryDate = (DateTime)nn["LoaExpiryDate"];
                                    LoaExpiryDate = LoaExpiryDate.AddDays((int)nn["NoOfDays"]);
                                    StrSql = "select IsNull(Sum(PaidAmount),0 )as PaidAmount "
                                            + " from OpCompanyBillDetail where AuthorityId =" + pt.mAuthorityid
                                            + " and BillDateTime <= '" + LoaExpiryDate.ToString("yyyy-MM-dd") + "'";
                                    DataSet xx = MainFunction.SDataSet(StrSql, "tbl21", Trans);
                                    foreach (DataRow yy in xx.Tables[0].Rows)
                                    {
                                        lngMaxDed = lngMaxDed - (decimal)yy["PaidAmount"];
                                        if (lngMaxDed < 0) { lngMaxDed = 0; }

                                    }

                                }

                            }


                        }
                    }

                    for (int i = 0; i < pt.arrItem.Count; i++)
                    {
                        if (lngMaxDed != 0)
                        {
                            pt.arrItem[i].dedAmt = GetDeductableAmount(pt.arrItem[i].dedAmt, lngMaxDed);
                        }
                        else
                        {
                            pt.arrItem[i].dedAmt = 0;
                        }
                    }
                }

                return true;


            }
            catch (Exception e) { return false; }


        }
        public static decimal GetDeductableAmount(decimal dedAmt, decimal lngMaxDed)
        {
            try
            {
                if (lngMaxDed <= dedAmt)
                {
                    return lngMaxDed;
                }
                else
                {
                    return dedAmt;
                }
            }
            catch (Exception e) { return 0; }
        }

        
        public static Patient SaveBill(Patient Patient, User UserData)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                                         Patient.COBILLNO = 0;
                    Patient.sPrefix = "";
                    int i; int iCashDisType = 0;
                    long mTransId;
                    long MaxBillNo = 0;
                    long csbillno = 0;
                    long csMaxBillNo = 0;
                    int optCredit = 0;
                                         string dPrefix = "";
                    string StrSql;
                    string OrderTable = "CashIssue";
                    string OrderDetailTable = "cashIssueDetail";
                                                              string BatchStoreTable = "BatchStore";
                                          

                    int Gstationid = UserData.selectedStationID;
                    string gIaCode = UserData.gIACode;
                    int gSavedById = UserData.EmpID;
                    Patient.OperatorID = UserData.EmpID;

                                         if (Patient.BillisCredit == true)
                    {
                        int iDedType, idedpertype, iDisPerType, gDedPerType, mDeducttype, gDisPerType;
                        decimal idedperamt, iDisPerAmt, mDeductper, mDisAmt, mDedAmt;
                        string st;
                        
                        iDedType = 0; idedpertype = 0; iDisPerType = 0; gDedPerType = 0; mDeducttype = 0; gDisPerType = 0;
                        idedperamt = 0; iDisPerAmt = 0; mDeductper = 0; mDisAmt = 0; mDedAmt = 0;


                        st = "select deductable,percentage,amount from opcompanydeptdeductable where categoryid = " + Patient.CategoryId
                           + "  and gradeid = " + Patient.GradeId + " and companyid = " + Patient.CompanyId
                           + " and serviceid = 11 and departmentid = " + Patient.mDeptId;
                        DataSet ds1 = MainFunction.SDataSet(st, "t1", Trans);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in ds1.Tables[0].Rows)
                            {
                                if ((decimal)rr["amount"] > 0)
                                {
                                    mDeductper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                    gDedPerType = 2;
                                }
                                else
                                {
                                    mDeductper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                    gDedPerType = 1;
                                }

                            }
                        }
                        else
                        {
                            st = "select deductable,percentage,amount from opcompanygradedeductable where categoryid = " + Patient.CategoryId
                              + "  and gradeid = " + Patient.GradeId + " and companyid = " + Patient.CompanyId
                              + " and serviceid = 11";
                            DataSet ds2 = MainFunction.SDataSet(st, "t2", Trans);
                            if (ds2.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rr in ds2.Tables[0].Rows)
                                {
                                    if ((decimal)rr["amount"] > 0)
                                    {
                                        mDeductper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                        gDedPerType = 2;
                                    }
                                    else
                                    {
                                        mDeductper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                        gDedPerType = 1;
                                    }
                                }
                            }


                        }
                        Patient.mDeductper = mDeductper;

                        st = "select  Deductable from IPDeductableType where IPOPType=1 AND " +
                             " categoryid = " + Patient.CategoryId + " and gradeid = " + Patient.GradeId + " and " +
                             " companyid = " + Patient.CompanyId;
                        DataSet ds3 = MainFunction.SDataSet(st, "t3", Trans);
                        if (ds3.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in ds3.Tables[0].Rows)
                            {
                                if ((int)rr["Deductable"] == -1) { mDeducttype = 0; }
                                else { mDeducttype = (int)rr["Deductable"]; }
                            }
                        }
                        else { mDeducttype = 0; }


                        st = "select percentage,amount from opcompanydeptdiscount where categoryid = " + Patient.CategoryId
                            + " and companyid = " + Patient.CompanyId + " and serviceid = 11 and gradeid = " + Patient.GradeId
                            + " and departmentid = " + Patient.mDeptId;
                        DataSet ds4 = MainFunction.SDataSet(st, "t4", Trans);
                        if (ds4.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in ds4.Tables[0].Rows)
                            {
                                if ((decimal)rr["amount"] > 0)
                                {

                                    Patient.mdisper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                    gDisPerType = 1;
                                }
                                else
                                {

                                    Patient.mdisper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                    gDisPerType = 0;
                                }
                            }
                        }
                        else
                        {
                            st = "select percentage,amount from opcompanyservicediscount where categoryid = " + Patient.CategoryId
                              + " and companyid = " + Patient.CompanyId + " and serviceid = 11 and gradeid = " + Patient.GradeId;
                            DataSet ds5 = MainFunction.SDataSet(st, "t5", Trans);
                            if (ds5.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rr in ds5.Tables[0].Rows)
                                {
                                    if ((decimal)rr["amount"] > 0)
                                    {

                                        Patient.mdisper = MainFunction.NullToDecmial(rr["amount"].ToString());
                                        gDisPerType = 1;
                                    }
                                    else
                                    {

                                        Patient.mdisper = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                        gDisPerType = 0;
                                    }
                                }



                            }

                            for (int ii = 0; ii < Patient.InsertItemValue.Count; ii++)
                            {
                                st = "select deductable,percentage,amount from opcompanyitemdeductable where companyid = " + Patient.CompanyId
                                    + " and categoryid = " + Patient.CategoryId + " and gradeid = " + Patient.GradeId
                                    + " and serviceid = 11 and itemid = " + Patient.InsertItemValue[ii].ID;
                                DataSet ds6 = MainFunction.SDataSet(st, "t6", Trans);
                                if (ds6.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow rr in ds6.Tables[0].Rows)
                                    {
                                        if ((decimal)rr["amount"] > 0)
                                        {
                                            idedperamt = MainFunction.NullToDecmial(rr["amount"].ToString());
                                            idedpertype = 1;
                                        }
                                        else
                                        {
                                            idedperamt = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                            idedpertype = 0;
                                        }
                                        iDedType = MainFunction.NullToInteger(rr["deductable"].ToString());
                                    }


                                }
                                else
                                {
                                    idedperamt = mDeductper;
                                    idedpertype = gDedPerType;
                                    iDedType = mDeducttype;
                                }

                                
                                st = "select percentage,amount from opcompanyitemdiscount where companyid = " + Patient.CompanyId
                                + " and categoryid = " + Patient.CategoryId + " and gradeid = " + Patient.GradeId
                                + " and serviceid = 11 and itemid = " + Patient.InsertItemValue[ii].ID;
                                DataSet ds7 = MainFunction.SDataSet(st, "t7", Trans);
                                if (ds7.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow rr in ds7.Tables[0].Rows)
                                    {
                                        if ((decimal)rr["amount"] > 0)
                                        {
                                            iDisPerAmt = MainFunction.NullToDecmial(rr["amount"].ToString());
                                            iDisPerType = 1;
                                        }
                                        else
                                        {
                                            iDisPerAmt = MainFunction.NullToDecmial(rr["percentage"].ToString());
                                            iDisPerType = 0;

                                        }
                                    }

                                }
                                else
                                {
                                    iDisPerAmt = Patient.mdisper;
                                    iDisPerType = gDisPerType;

                                }
                                Patient.InsertItemValue[ii].Deductabletype = iDedType;
                                Patient.InsertItemValue[ii].DeductablePerAmounttype = idedpertype;
                                Patient.InsertItemValue[ii].DeductablePerAmount = idedperamt;
                                Patient.InsertItemValue[ii].DiscountPerAmountType = iDisPerType;
                                Patient.InsertItemValue[ii].DiscountPerAmount = iDisPerAmt;
                            }
                        }
                    } 





                    if (validate(Patient) == true && InitBatchArray(ref Patient) == true)
                    {
                        Patient.NetAmount = Patient.TotalAmount;
                        if (Patient.BillisCredit == true)
                        {
                            StrSql = "Update  Opbilltype set currentno= currentno where id=2 and stationid=" + Gstationid;
                            bool UpdateOpBillType = MainFunction.SSqlExcuite(StrSql, Trans);

                            DataSet ds1 = MainFunction.SDataSet("select name,CurrentNo from  Opbilltype where id =2 and stationid= " + Gstationid, "tb1", Trans);
                            foreach (DataRow rr in ds1.Tables[0].Rows)
                            {
                                Patient.COBILLNO = (int)rr["CurrentNo"] + 1;
                                Patient.sPrefix = rr["name"].ToString().Trim();
                            }

                                                         StrSql = "Insert into " + OrderTable + "(StationId,OperatorID,actualdate,DateTime,Name,doctorname," +
                             " age,agetype,sex,Amount,discount,cash_credit,comp_id,creditbillno,prefix,doctorid," +
                             " NetAmount,cancelyesno,Billtype,pinno,compcredit,categoryid,gradeid,deducttype," +
                             " deductper,deductableamt,balance,dis_amount,authorityid,departmentid,regno," +
                             " DispatchDoctorID,cashbillno) Values(" + Gstationid + "," + gSavedById + ",getdate(),getdate(),'" + Patient.PatientName + "'," +
                             " '" + Patient.DoctorName + "'," + Patient.Age + "," + Patient.Agetype + "," + Patient.Sex +
                             " ," + Patient.TotalAmount + "," + Patient.lblCreditDiscount + ",1," + Patient.CompanyId + "," +
                             Patient.COBILLNO + ",'" + Patient.sPrefix + "'," + Patient.DoctorId + "," + Patient.lblBalance + "," +
                             " 0,2,'" + MainFunction.getUHID(Patient.IssueAuthorityCode, Patient.Registrationno.ToString(), false) + "',1," +
                             Patient.CategoryId + "," + Patient.GradeId + "," + Patient.mDeducttype + "," + Patient.mDeductper + "," +
                             Patient.lbldedamt + "," + Patient.lblBalance + "," + Patient.mdisper + "," + Patient.mAuthorityid + "," +
                             Patient.mDeptId + "," + Patient.Registrationno + "," + Patient.DispatchDoctorID + ",0)";

                            bool InsertIntoCashIssue = MainFunction.SSqlExcuite(StrSql, Trans);

                                                         StrSql = "Select max(id) as maxId from " + OrderTable + " where stationid=" + Gstationid;
                            DataSet ds2 = MainFunction.SDataSet(StrSql, "tb2", Trans);
                            foreach (DataRow rr in ds2.Tables[0].Rows)
                            {
                                MaxBillNo = (int)rr["maxId"];
                            }

                            StrSql = " Update  Opbilltype set currentno= " + Patient.COBILLNO + " where id=2 and stationid=" + Gstationid;
                            bool UpdateOpBilltype2 = MainFunction.SSqlExcuite(StrSql, Trans);

                                                         if (Patient.lbldedamt > 0)
                            {
                                StrSql = " Update  Opbilltype set currentno= currentno where id=1 and stationid=" + Gstationid;
                                bool UpdateOpBilltype3 = MainFunction.SSqlExcuite(StrSql, Trans);

                                DataSet ds3 = MainFunction.SDataSet("select name,CurrentNo from  Opbilltype where id =1 and stationid= " + Gstationid, "tbl3", Trans);
                                foreach (DataRow rr in ds3.Tables[0].Rows)
                                {
                                    csbillno = (int)rr["CurrentNo"] + 1;
                                    dPrefix = rr["name"].ToString().Trim();
                                }

                                                                                                  StrSql = "Insert into " + OrderTable + "(StationId,OperatorID,ACTUALDATE,DateTime,Name,doctorname,age,agetype,sex,Amount,dis_amount,cash_credit,comp_id,CompanyBillNo,prefix,doctorid,NetAmount,cancelyesno,Billtype,pinno,compcredit,categoryid,gradeid,deducttype,deductper,deductableamt,balance,creditbillid,cashbillno,regno) Values(" +
                                     +Gstationid + "," + gSavedById + ",getdate(),getdate(),'" + Patient.PatientName + "','" + Patient.DoctorName
                                     + "'," + Patient.Age + "," + Patient.Agetype + "," + Patient.Sex + "," + Patient.lbldedamt + ",0,0,"
                                     + Patient.CompanyId + ", " + 0 + ",'" + dPrefix + "'," + Patient.DoctorId + "," + Patient.lbldedamt + ",0,1,'" +
                                     MainFunction.getUHID(Patient.IssueAuthorityCode, Patient.Registrationno.ToString(), false) + "',1," + Patient.CategoryId + ","
                                     + Patient.GradeId + "," + Patient.mDeducttype + "," + Patient.mDeductper + "," + Patient.lbldedamt + ","
                                     + Patient.lblBalance + "," + MaxBillNo + "," + csbillno + "," + Patient.Registrationno + ")";

                                bool InsertCashIntoCashissue = MainFunction.SSqlExcuite(StrSql, Trans);

                                StrSql = " Update  Opbilltype set currentno= " + csbillno + " where id=1 and stationid=" + Gstationid;
                                bool UpdateOpbilltype4 = MainFunction.SSqlExcuite(StrSql, Trans);

                                                                 StrSql = "select max(id) as maxid from " + OrderTable + " where stationid=" + Gstationid;
                                DataSet ds4 = MainFunction.SDataSet(StrSql, "tbl4", Trans);
                                foreach (DataRow rr in ds4.Tables[0].Rows)
                                {
                                    csMaxBillNo = (int)rr["maxid"];

                                }
                            }
                        }                         else if (Patient.BillisCredit == false)
                        {
                            StrSql = " Update  Opbilltype set currentno= currentno where id=1 and stationid=" + Gstationid;
                            bool UpdateOpbilltype5 = MainFunction.SSqlExcuite(StrSql, Trans);
                            DataSet ds5 = MainFunction.SDataSet("select name,CurrentNo from  Opbilltype where id =1 and stationid= " + Gstationid, "tb5", Trans);
                            foreach (DataRow rr in ds5.Tables[0].Rows)
                            {
                                Patient.COBILLNO = (int)rr["CurrentNo"] + 1;
                                Patient.sPrefix = rr["name"].ToString().Trim();

                            }

                            if (Patient.BillisCredit == true) { optCredit = 1; } else { optCredit = 0; }
                                                         StrSql = "Insert into " + OrderTable + "(StationId,OperatorID,actualdate,DateTime,Name,"
                            + " doctorname,age,agetype,sex,PaymentMode,Amount,dis_amount,cash_credit,"
                            + " comp_id,doctorid,cashbillno,amexslno,NetAmount,pinno,"
                            + " cancelyesno,BillType,Prefix,discount,regno,CashDiscountType,DispatchDoctorID) "
                            + " Values("
                            + Gstationid + "," + gSavedById + ",getdate(),getdate(),'" + Patient.PatientName + "',"
                            + " '" + Patient.DoctorName + "'," + Patient.Age + ","
                            + Patient.Agetype + "," + Patient.Sex + ","
                            + optCredit + "," + Patient.TotalAmount + "," + Patient.mdisper + ","
                            + " 0,0," + Patient.DoctorId + "," + Patient.COBILLNO + ",0," + Patient.lblNetAmount + ","
                            + " '" + MainFunction.getUHID(Patient.IssueAuthorityCode, Patient.Registrationno.ToString(), false)
                            + "',0,1,'" + Patient.sPrefix + "'," + Patient.lblCreditDiscount + ","
                            + Patient.Registrationno + "," + iCashDisType + "," + Patient.DispatchDoctorID + ")";

                            bool InsertINtoCashIssue4 = MainFunction.SSqlExcuite(StrSql, Trans);


                                                         StrSql = "Select max(id) as maxId from " + OrderTable + " where stationid=" + Gstationid;
                            DataSet ds6 = MainFunction.SDataSet(StrSql, "tbl6", Trans);
                            foreach (DataRow rr in ds6.Tables[0].Rows)
                            {
                                MaxBillNo = (int)rr["maxId"];

                            }


                            StrSql = " Update  Opbilltype set currentno= " + Patient.COBILLNO + " where id=1 and stationid=" + Gstationid;
                            bool updateOpbilltype6 = MainFunction.SSqlExcuite(StrSql, Trans);


                        } 

                        for (i = 0; i < Patient.InsertItemValue.Count; i++)
                        {
                            if (Patient.InsertItemValue[i].PrescriptionID > 0)
                            {
                                bool UpdateClincialPrescription = MainFunction.SSqlExcuite(" " +
                                 " Update clinicalprescription set corder =1, billid=" + MaxBillNo + " where id = " + Patient.InsertItemValue[i].PrescriptionID, Trans);

                                bool InsertIntoPrescBill = MainFunction.SSqlExcuite(" " +
                                    "Insert Into clinicalprescriptionBills Values (" + Patient.InsertItemValue[i].PrescriptionID + "," + MaxBillNo + ")", Trans);

                            }

                        }
                        mTransId = MainFunction.SaveInTranOrder(Trans, UserData.selectedStationID, MaxBillNo, Patient.COBILLNO, 1, 12, UserData.selectedStationID, Patient.sPrefix, 0, Patient.Registrationno);
                        if (mTransId == 0)
                        {
                            if (MainFunction.CheckTransTable() == "Try Again")
                            {
                                mTransId = MainFunction.SaveInTranOrder(Trans, UserData.selectedStationID, MaxBillNo, Patient.COBILLNO, 1, 12, UserData.selectedStationID, Patient.sPrefix, 0, Patient.Registrationno);
                                if (mTransId == 0)
                                {
                                    Trans.Rollback();
                                    Con.Close();
                                    Patient.ErrMsg = "Can't Create the TransOrder Table";
                                    return Patient;
                                }
                            }
                            else
                            {
                                Trans.Rollback();
                                Con.Close();
                                Patient.ErrMsg = MainFunction.CheckTransTable();
                                return Patient;

                            }

                            ;
                        }


                                                 for (i = 0; i < Patient.arrItem.Count; i++)
                        {
                                                                                      if (Patient.arrItem[i].qty > 0)
                            {
                                                                                                                                                                                                                                                                                                         
                                StrSql = "Insert into " + OrderDetailTable + "(BillNo,Quantity,ServiceID,BatchNo,Price,unitid,tax,slno,batchid,deductabletype"
                                    + " ,deductpertype,dedperamt,dedamt,discpertype,disperamt,disamt,epr,orderitemid) Values("
                                    + MaxBillNo + "," + Patient.arrItem[i].qty + "," + Patient.arrItem[i].ID + ",'"
                                    + Patient.arrItem[i].BatchNo + "'," + Patient.arrItem[i].price + "," + Patient.arrItem[i].NewUomID
                                    + "," + Patient.arrItem[i].tax + "," + (i + 1) + ",'" + Patient.arrItem[i].batchid
                                    + "'," + Patient.arrItem[i].Deductabletype + "," + Patient.arrItem[i].DeductablePerAmounttype
                                    + "," + Patient.arrItem[i].DeductablePerAmount + "," + Patient.arrItem[i].dedAmt
                                    + "," + Patient.arrItem[i].DiscountPerAmountType + "," + Patient.arrItem[i].DiscountPerAmount
                                    + "," + Patient.arrItem[i].disamt + "," + Patient.arrItem[i].Epr + "," + Patient.arrItem[i].OrderedItemid + ")";

                                bool InserIntoCashissueDtl = MainFunction.SSqlExcuite(StrSql, Trans);

                                
                                                                                                                                   StrSql = "Update " + BatchStoreTable + " Set Quantity=Quantity-" + Patient.arrItem[i].DedQty + " where ItemId="
                                    + Patient.arrItem[i].ID + " and BatchNo='" + Patient.arrItem[i].BatchNo
                                    + "' and batchid=" + Patient.arrItem[i].batchid + " and stationId = " + Gstationid;
                                bool UpdateBatchStore = MainFunction.SSqlExcuite(StrSql, Trans);

                                if (MainFunction.MazValidateBatchStoreQty(Patient.arrItem[i].ID, int.Parse(Patient.arrItem[i].batchid), Gstationid, Trans) == false)
                                {
                                    if (Con.State == ConnectionState.Open)
                                    {
                                        Con.BeginTransaction().Rollback();
                                        Con.Close();
                                    }
                                    Patient.ErrMsg = "Can't Issue Stock for some Item!";
                                    return Patient;
                                }


                                                                 if (Patient.arrItem[i].PrescriptionID > 0)
                                {
                                    if (Gstationid == 18)
                                    {
                                        StrSql = "Update MSF_PrescriptionDETAIL set dispatched = 1 where orderid = " + Patient.arrItem[i].PrescriptionID
                                            + " and ITEMID = " + Patient.arrItem[i].OrderedItemid;
                                    }
                                    else
                                    {
                                        StrSql = "Update ClinicalPrescriptionDetail set dispatched = 1 where orderid = " + Patient.arrItem[i].PrescriptionID
                                             + " and drugid = " + Patient.arrItem[i].OrderedItemid;

                                    }
                                    bool updatePrescription = MainFunction.SSqlExcuite(StrSql, Trans);
                                }

                                 
                                DataSet temp1 = MainFunction.SDataSet("select costprice,sellingprice from batch where batchid ="
                                    + Patient.arrItem[i].batchid, "tbl", Trans);
                                if (temp1.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow rr in temp1.Tables[0].Rows)
                                    {
                                                                                                                                                                   if (MainFunction.SaveInTranOrderDetail(Trans, mTransId, Patient.arrItem[i].ID, Patient.arrItem[i].DedQty
                                            , Patient.arrItem[i].BatchNo, Patient.arrItem[i].batchid, (decimal)rr["costprice"],
                                            (decimal)rr["sellingprice"], (decimal)rr["costprice"]) == false)
                                        {
                                            Trans.Rollback();
                                            Con.Close();
                                            Patient.ErrMsg = "Error with TransOrderDetail Table";
                                            return Patient;
                                        }
                                    }

                                }
                                else
                                {
                                                                                                              if (MainFunction.SaveInTranOrderDetail(Trans, mTransId, Patient.arrItem[i].ID, Patient.arrItem[i].DedQty
                                                , Patient.arrItem[i].BatchNo, Patient.arrItem[i].batchid) == false)
                                    {
                                        Trans.Rollback();
                                        Con.Close();
                                        Patient.ErrMsg = "Error with TransOrderDetail Table";
                                        return Patient;
                                    }

                                }
                            }

                        }


                                                                                                   bool FounUpdatePres = CHECKPrescription(Trans, Patient.Registrationno, gIaCode, Gstationid, Patient.DoctorId, Patient.InsertItemValue);


                        if (SaveOPBillInServer(Patient, Patient.COBILLNO, Patient.sPrefix, MaxBillNo, Trans, UserData.CurrentIPAddress) == false)
                        {
                            Trans.Rollback();
                            Con.Close();
                            Patient.ErrMsg = "Error, Can't save the bill !(OPBILL)";
                            return Patient;
                        }

                    }


                    
                    Patient.MaxBillNo = MaxBillNo;
                    Patient.csMaxBillNo = csMaxBillNo;

                    Trans.Commit();
                    return Patient;
                }
            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                Patient.ErrMsg = "Error when try processing this Bill";
                return Patient;
            }
        }
        
        public static bool CHECKPrescription(SqlTransaction Trans, int mRegNo, string gIaCode, int gStationid, int? docID, List<InsertedItemsList> arr)
        {
            try
            {
                string sql = "";
                if (gStationid == 18)
                {
                    sql = "SELECT cp.ID as OrderNo,cp.VisitID as VisitID,cp.DateTime as [Date Time], " +
         " e.Name as Operator,s.Name as Station,cp.doctorid,cp.corder, 1 prescription " +
         " FROM employee e,Station s,clinicalprescription cp " +
         " WHERE cp.issueauthoritycode = '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid " +
         " and cp.datetime >= getdate() - 30 and " +
         " (select count(*) from ClinicalPrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " Union All SELECT cp.ID as [Order No],cp.VisitID,cp.DateTime, " +
         " e.Name as Operator ,s.Name as Station,cp.doctorid,0 as corder, 0 prescription " +
         " FROM employee e,Station s,MSF_Prescription cp WHERE cp.deleted=0 and cp.issueauthoritycode = '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid " +
         " and cp.datetime >= getdate() - 30 and (select count(*) from " +
         " MSF_PrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " order by cp.datetime ";

                }
                else
                {
                    sql = "SELECT cp.ID as OrderNo,cp.VisitID as VisitID,cp.DateTime as [Date Time], " +
         " e.Name as Operator,s.Name as Station,cp.doctorid,cp.corder, 1 prescription " +
         " FROM employee e,Station s,clinicalprescription cp " +
         " WHERE cp.issueauthoritycode = '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid and e.id=" + docID +
         " and cp.datetime >= getdate() - 30 and " +
         " (select count(*) from ClinicalPrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " Union All SELECT cp.ID as [Order No],cp.VisitID,cp.DateTime, " +
         " e.Name as Operator ,s.Name as Station,cp.doctorid,0 as corder, 0 prescription " +
         " FROM employee e,Station s,MSF_Prescription cp WHERE cp.deleted=0 and cp.issueauthoritycode =  '" + gIaCode + "' " +
         " and cp.registrationno =  " + mRegNo + "  and e.ID = cp.doctorID and s.id=cp.stationid and e.id=" + docID +
         " and cp.datetime >= getdate() - 30 and (select count(*) from " +
         " MSF_PrescriptionDetail where dispatched = 0 and orderid =  cp.id) > 0 " +
         " order by cp.datetime ";


                }
                DataSet dt = MainFunction.SDataSet(sql, "tb", Trans);
                if (dt.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in dt.Tables[0].Rows)
                    {
                                                 for (int i = 0; i < arr.Count; i++)
                        {
                            sql = "select * from ClinicalPrescriptionDetail where orderid=" + rr["OrderNo"] + " and drugid=" + arr[i].OrderedItemid;
                            DataSet FoundDst = MainFunction.SDataSet(sql, "tbl1", Trans);
                            foreach (DataRow xx in FoundDst.Tables[0].Rows)
                            {
                                sql = "Update ClinicalPrescriptionDetail set dispatched = 1 where " +
                                         " orderid = " + rr["OrderNo"] + " and drugid = " + arr[i].OrderedItemid;
                                bool updateClincpresc = MainFunction.SSqlExcuite(sql, Trans);
                            }


                        }
                    }
                    return true;
                }
                else
                {

                    return false;
                }

                return false;
            }
            catch (Exception e) { return false; }


        }
        
        public static bool SaveOPBillInServer(Patient pt, long COBILLNO, string sPrefix, long MaxBillNo, SqlTransaction Trans, string IPAddress)
        {
            try
            {
                string StrSql;
                int intRecorsaffected = 0;
                int intsex = 0;
                string strother = "";
                string txtCLetterNo = "";                  decimal Balanceamount = 0;
                int? lngcompany = 0;
                decimal totAmt = 0;
                decimal mDisAmt = pt.lblCreditDiscount;
                int i = 0;
                int intI = 0;
                int maxId = 1;
                int STSLNO = 1;
                decimal LoaLimit = 0;
                int revisitdays = 0;
                string OrderTable = "CashIssue";


                if (pt.BillisCredit == false)
                {
                    lngcompany = 1;
                }
                else
                {
                    if (pt.CompanyId > 0)
                    {
                        lngcompany = pt.CompanyId;
                    }
                }

                if (pt.Sex == 0)
                {
                    intsex = pt.Sex;
                    strother = pt.Sexothers;
                }
                else
                {
                    intsex = pt.Sex;
                }

                Balanceamount = pt.lblBalance;
                intRecorsaffected = 0;

                intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber("Update ordermaxid set tableid=tableid where tableid=1 AND STATIONID =0", Trans);
                if (intRecorsaffected == 0) { return false; }

                DataSet rsMax = MainFunction.SDataSet("select maxid from ordermaxid where tableid = 1 AND STATIONID =0 ", "tbl", Trans);
                foreach (DataRow rr in rsMax.Tables[0].Rows)
                {
                    maxId = (int)rr["maxid"] + 1;
                }

                intRecorsaffected = 0;
                intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber("Update ordermaxid set tableid=tableid where tableid=1 AND STATIONID =" + pt.gStationID, Trans);
                if (intRecorsaffected == 0) { return false; }

                DataSet rsMax1 = MainFunction.SDataSet("select maxid from ordermaxid where tableid = 1 AND STATIONID =" + pt.gStationID, "tbl", Trans);
                foreach (DataRow rr in rsMax1.Tables[0].Rows)
                {
                    STSLNO = (int)rr["maxid"] + 1;
                }

                if (pt.BillisCredit == true)
                {
                    StrSql = "Insert into opbill(id,DateTime,Amount,discount,Balance,OperatorID,StationID,RegistrationNo,Name,Age,AgeType,Sex,SexOthers,issueauthoritycode,companyid,diposit,STATIONSLNO,GradeId,Deductable,DeductType,CategoryId,LoaNo,LoaAppDate,actualdate)";
                    StrSql = StrSql + " Values(";
                    StrSql = StrSql + "" + maxId + ",GETDATE()," + pt.TotalAmount + "," + mDisAmt + "," + pt.lblBalance
                             + "," + pt.OperatorID + "," + pt.gStationID + "," + pt.Registrationno + ",'" + pt.PatientName + "' " + ", " + pt.Age + ","
                             + pt.Agetype + "," + intsex + ",'" + strother + "','" + pt.IssueAuthorityCode + "'," + lngcompany + "," + pt.lblBalDeposit
                             + "," + STSLNO + " ," + pt.GradeId + "," + pt.mDeductper + "," + pt.mDeducttype + "," + pt.CategoryId
                             + ",'" + MainFunction.NullToInteger(txtCLetterNo) + "',GETDATE(),getdate())";

                }
                else
                {
                    StrSql = "Insert into opbill(id,DateTime,Amount,discount,Balance,OperatorID,StationID,RegistrationNo,Name,Age,AgeType,Sex,SexOthers,issueauthoritycode,companyid,diposit,STATIONSLNO,GradeId,Deductable,DeductType,CategoryId,LoaNo,LoaAppDate,actualdate)";
                    StrSql = StrSql + " Values(";
                    StrSql = StrSql + "" + maxId + ",GETDATE()," + pt.TotalAmount + "," + mDisAmt + "," + pt.lblBalance
                        + "," + pt.OperatorID + "," + pt.gStationID + "," + pt.Registrationno + ",'" + pt.PatientName + "'," + pt.Age + ","
                        + pt.Agetype + "," + intsex + ",'" + strother + "','" + pt.IssueAuthorityCode + "'," + lngcompany + "," + pt.lblBalDeposit
                        + "," + STSLNO + ",1,0,0,1,'" + MainFunction.NullToInteger(txtCLetterNo) + "',GETDATE(),getdate())";
                }
                intRecorsaffected = 0;
                intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                if (intRecorsaffected == 0) { return false; }


                intRecorsaffected = 0;
                intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber("update ordermaxid set maxid = " + maxId + " where tableid = 1 AND STATIONID =0", Trans);
                if (intRecorsaffected == 0) { return false; }

                intRecorsaffected = 0;
                intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber("update ordermaxid set maxid = " + STSLNO + " where tableid = 1 AND STATIONID =" + pt.gStationID, Trans);
                if (intRecorsaffected == 0) { return false; }


                
                int intDonateInd = 0;
                decimal intDonateAmount = 0;
                if (pt.lbldonationAMT > 0)
                {
                    intDonateInd = 1;
                    intDonateAmount = pt.lbldonationAMT;
                }

                if (pt.BillisCredit == false)
                {
                    StrSql = "insert into opbilldetail (opbillid,billno,amount,balance,billtypeid,DonationInd,DonationAmount) values ("
                             + maxId + ",'" + sPrefix + COBILLNO + "', " + pt.lblNetAmount + ",0,1," + intDonateInd + "," + intDonateAmount + ")";
                }
                else
                {
                    StrSql = "insert into opbilldetail (opbillid,billno,amount,balance,billtypeid,DonationInd,DonationAmount) values ("
                        + maxId + ",'" + sPrefix + COBILLNO + "', " + pt.lbldedamt + "," + pt.lblBalance + ",2," + intDonateInd + "," + intDonateAmount + ")";
                }
                bool InsertIntoOpbilldetail = MainFunction.SSqlExcuite(StrSql, Trans);

                if (pt.BillisCredit == true)
                {
                    StrSql = "select yesno from opserviceloa where categoryid = " + pt.CategoryId + " and companyid = " + pt.CompanyId + " and gradeid = " + pt.GradeId
                           + " and opserviceid = 11";
                    DataSet rsLoa = MainFunction.SDataSet(StrSql, "tbl", Trans);
                    foreach (DataRow rr in rsLoa.Tables[0].Rows)
                    {
                        if ((bool)rr["yesno"] == false)
                        {
                            if (pt.mAuthorityid <= 0)
                            {
                                LoaDetails(pt, Trans);
                            }

                            StrSql = "select isnull((Case when loatype =0 then loaamount else pharmacyamount end),0) "
                                   + " amount,isnull(nodays,0) no_of_days from oploaorder where authorityid = " + pt.mAuthorityid;

                            DataSet rsPrice1 = MainFunction.SDataSet(StrSql, "tbl", Trans);
                            foreach (DataRow nn in rsPrice1.Tables[0].Rows)
                            {
                                LoaLimit = (decimal)nn["amount"];
                                revisitdays = (int)nn["no_of_days"];
                            }

                            StrSql = "insert into opLOAdetail (Opbillid,CategoryId,CompanyId,GradeId,BillDatetime,NoDays,LOAamount, "
                                    + " PaidAmount,Deductable,Registrationno,DiscountAmount,DeductAmount,authorityid,ServiceId) "
                                    + " values(" + maxId + "," + pt.CategoryId + "," + pt.CompanyId
                                    + "," + pt.GradeId + ",getdate()," + revisitdays + "," + LoaLimit + ","
                                    + pt.TotalAmount + "," + pt.mDeducttype + "," + pt.Registrationno + ","
                                    + pt.lblCreditDiscount + "," + pt.lbldedamt + "," + pt.mAuthorityid + ",11)";
                            bool InsertIntoOpLoaDetail = MainFunction.SSqlExcuite(StrSql, Trans);
                        }
                    }
                }


                if (mDisAmt > 0)
                { 
                                                                                   bool InsertDisc = MainFunction.SSqlExcuite("insert into opdiscountdetail (opbillid,discount,authority,reasonfordiscount,serviceid,OpDiscountType,AuthorisedBy) "
                       + " values (" + maxId + "," + mDisAmt + ",'" + pt.CompanyName + "','',11,0 ,0)", Trans);
                     
                }

                intRecorsaffected = 0;
                StrSql = "update " + OrderTable + " set opbillid = " + maxId + " where id = " + MaxBillNo;
                intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                if (intRecorsaffected == 0) { return false; }

                int departmentId = 0;
                DataSet rsTemp = MainFunction.SDataSet("select departmentid from station where id = " + pt.gStationID, "tbl", Trans);
                foreach (DataRow rr in rsTemp.Tables[0].Rows)
                {
                    departmentId = (int)rr["departmentid"];
                }

                List<AppItemService> AppServices = new List<AppItemService>();
                for (i = 0; i < pt.arrItem.Count; i++)
                {
                    if (pt.BillisCredit == true)
                    {
                        if (pt.arrItem[i].qty > 0)
                        {
                            StrSql = "Insert into opcompanybilldetail (registrationno,opbillid,billno,companyid,"
                                + "categoryid,gradeid,doctorid,serviceid,itemid,itemcode,itemname,discounttype,deductable,billamount,"
                                + "paidamount,discount,balance,ACTUALDATE,billdatetime,departmentid,quantity,billtypeid,"
                                + "issueauthoritycode,operatorid,stationid,authorityid,posted,batchid,IssueQty,IssueUnit) values ("
                                + pt.Registrationno + "," + maxId + ",'" + sPrefix + COBILLNO + "'," + pt.CompanyId + ","
                                + pt.CategoryId + "," + pt.GradeId + "," + pt.DoctorId + ","
                                + "11," + pt.arrItem[i].ID + ",'" + pt.arrItem[i].ItemCode + "','" + pt.arrItem[i].Name + "',1," + pt.arrItem[i].Deductabletype + "," + pt.arrItem[i].ItemTotal + ","
                                + pt.arrItem[i].dedAmt + "," + pt.arrItem[i].disamt + "," + (pt.arrItem[i].ItemTotal - pt.arrItem[i].dedAmt - pt.arrItem[i].disamt).ToString() + ","
                                + "getdate(),getdate()," + departmentId + "," + pt.arrItem[i].DedQty + ",2,'" + pt.IssueAuthorityCode + "'," + pt.OperatorID + "," + pt.gStationID + "," + pt.mAuthorityid
                                + ",1," + pt.arrItem[i].batchid + "," + pt.arrItem[i].qty + "," + pt.arrItem[i].NewUomID + ")";

                            intRecorsaffected = 0;
                            intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                            if (intRecorsaffected == 0) { return false; }


                            StrSql = "Insert into ARcompanybilldetail (registrationno,opbillid,billno,companyid,"
                                + "categoryid,gradeid,doctorid,serviceid,itemid,itemcode,itemname,discounttype,deductable,billamount,"
                                + "paidamount,discount,balance,billdatetime,departmentid,quantity,billtypeid,"
                                + "issueauthoritycode,operatorid,stationid,authorityid,posted,batchid,ARegistrationno,"
                                + "AIssueauthoritycode,ACategoryId,ACompanyId,AGradeId,ADoctorID,ABillAmount,APaidAmount,"
                                + "ADiscount,ABalance,AQUantity,AAuthorityId,ABillDateTime,IssueQty,IssueUnit) values ("
                                + pt.Registrationno + "," + maxId + ",'" + sPrefix + COBILLNO + "'," + pt.CompanyId + ","
                                + pt.CategoryId + "," + pt.GradeId + "," + pt.DoctorId + ","
                                + "11," + pt.arrItem[i].ID + ",'" + pt.arrItem[i].ItemCode + "','" + pt.arrItem[i].Name + "',1," + pt.arrItem[i].Deductabletype + "," + pt.arrItem[i].ItemTotal + ","
                                + pt.arrItem[i].dedAmt + "," + pt.arrItem[i].disamt + "," + (pt.arrItem[i].ItemTotal - pt.arrItem[i].dedAmt - pt.arrItem[i].disamt).ToString() + ","
                                + "getdate()," + departmentId + "," + pt.arrItem[i].DedQty + ",2,'" + pt.IssueAuthorityCode + "'," + pt.OperatorID + "," + pt.gStationID
                                + "," + pt.mAuthorityid + ",1," + pt.arrItem[i].batchid + ","
                                + pt.Registrationno + ",'" + pt.IssueAuthorityCode + "'," + pt.CategoryId + "," + pt.CompanyId + "," + pt.GradeId + ","
                                + pt.DoctorId + "," + pt.arrItem[i].ItemTotal + "," + pt.arrItem[i].dedAmt + "," + pt.arrItem[i].disamt + ","
                                + (pt.arrItem[i].ItemTotal - pt.arrItem[i].dedAmt - pt.arrItem[i].disamt).ToString() + ","
                                + pt.arrItem[i].qty + "," + pt.mAuthorityid + ",getdate()," + pt.arrItem[i].qty + "," + pt.arrItem[i].NewUomID + ")";

                            intRecorsaffected = 0;
                            intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                            if (intRecorsaffected == 0) { return false; }


                            
                            int intCheck = CheckLOAAppServices(11, pt.mDeptId, pt.arrItem[i].ID, pt.mAuthorityid, pt.IssueAuthorityCode, pt.Registrationno, ref AppServices);
                            for (intI = 0; intI < AppServices.Count; intI++)
                            {
                                if (i < pt.arrItem.Count)
                                {
                                    if (pt.arrItem[i].ID == AppServices[intI].Itemid && AppServices[intI].appid != 0)
                                    {
                                        StrSql = "Update ArApprovalOrderDetails  set Billed=1,IPOPId= " + maxId + " where OrderNo ="
                                            + AppServices[intI].appid + " and ServiceId=" + AppServices[intI].ServiceId + "  and ItemId= "
                                            + AppServices[intI].Itemid + "";
                                        bool UpdateArApproval = MainFunction.SSqlExcuite(StrSql, Trans);

                                    }
                                    else if (AppServices[intI].appid == 0)
                                    {
                                        StrSql = "Update ARReleaseExclusions  set OpBillId= " + maxId + " where ServiceId="
                                            + AppServices[intI].ServiceId + "  and ItemId= " + AppServices[intI].Itemid
                                            + " and DeptId=" + AppServices[intI].DeptId + "  and OpBillId=0 and RegNo ="
                                            + pt.Registrationno + " and IACode ='" + pt.IssueAuthorityCode + "' ";
                                        bool UpdateARReleaseExclusions = MainFunction.SSqlExcuite(StrSql, Trans);

                                    }

                                }


                            }
                        }


                    }
                    else if (pt.BillisCredit == false)
                    {
                        if (pt.arrItem[i].qty > 0)
                        {
                            StrSql = "Insert into opcompanybilldetail (registrationno,opbillid,billno,companyid,"
                                + "categoryid,gradeid,doctorid,serviceid,itemid,itemcode,itemname,discounttype,deductable,billamount,"
                                + "paidamount,discount,balance,actualdate,billdatetime,departmentid,quantity,billtypeid,"
                                + "issueauthoritycode,operatorid,stationid,authorityid,batchid,IssueQty,IssueUnit,posted) values ("
                                + pt.Registrationno + "," + maxId + ",'" + sPrefix + COBILLNO + "',1,"
                                + "1,1," + pt.DoctorId + ","
                                + "11," + pt.arrItem[i].ID + ",'" + pt.arrItem[i].ItemCode + "','" + pt.arrItem[i].Name + "',1," + pt.arrItem[i].Deductabletype
                                + "," + pt.arrItem[i].ItemTotal + ","
                                + (pt.arrItem[i].ItemTotal - pt.arrItem[i].disamt).ToString() + "," + pt.arrItem[i].disamt + ",0,"
                                + "getdate(),getdate()," + departmentId + "," + pt.arrItem[i].DedQty + ",1,'" + pt.IssueAuthorityCode
                                + "'," + pt.OperatorID + "," + pt.gStationID + ",0," + pt.arrItem[i].batchid + "," + pt.arrItem[i].qty + "," + pt.arrItem[i].NewUomID + ",1)";
                            intRecorsaffected = 0;
                            intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                            if (intRecorsaffected == 0) { return false; }
                        }

                    }

                }


                if (pt.lbldonationAMT > 0)
                {
                    if (pt.BillisCredit == true)
                    {
                        StrSql = "insert into DonationOpCompanyBillDetail (RegistrationNo,OPBillId,BillNo, "
                        + " CompanyId,CategoryId,GradeId,DoctorId,ServiceId,ItemId,DiscountType,Deductable, "
                        + " BillAmount,PaidAmount,Discount,Balance,ActualDate,Billdatetime,DepartmentId,Quantity, "
                        + " BilltypeId,IssueAuthorityCode,OperatorId,stationid,AuthorityId,BatchId,ItemCode, "
                        + " ItemName, SubPolicy) values ("
                        + pt.Registrationno + "," + maxId + ",'" + sPrefix + COBILLNO + "',"
                        + pt.CompanyId + "," + pt.CategoryId + "," + pt.GradeId
                        + "," + pt.DoctorId + ",7,8857," + pt.DiscountTypeID                           + "," + pt.mDeducttype                            + "," + pt.lbldonationAMT + "," + pt.lbldonationAMT + ",0,0,getdate(),getdate(),"
                        + departmentId + ",1,2,'" + pt.IssueAuthorityCode + "'," + pt.OperatorID + "," + pt.gStationID + "," + pt.mAuthorityid
                        + ",0,'FMADM-9999','DONATION TO CHARITY',0 )";
                        intRecorsaffected = 0;
                        intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                        if (intRecorsaffected == 0) { return false; }
                    }
                    else if (pt.BillisCredit == false)
                    {
                        StrSql = "insert into DonationOpCompanyBillDetail (RegistrationNo,OPBillId,BillNo, "
                        + " CompanyId,CategoryId,GradeId,DoctorId,ServiceId,ItemId,DiscountType,Deductable, "
                        + " BillAmount,PaidAmount,Discount,Balance,ActualDate,Billdatetime,DepartmentId,Quantity, "
                        + " BilltypeId,IssueAuthorityCode,OperatorId,stationid,AuthorityId,BatchId,ItemCode, "
                        + " ItemName, SubPolicy) values ("
                        + pt.Registrationno + "," + maxId + ",'" + sPrefix + COBILLNO + "',"
                        + " 1,1,1 "
                        + "," + pt.DoctorId + ",7,8857," + +pt.DiscountTypeID                           + "," + pt.mDeducttype                            + "," + pt.lbldonationAMT + "," + pt.lbldonationAMT + ",0,0,getdate(),getdate(),"
                        + departmentId + ",1,2,'" + pt.IssueAuthorityCode + "'," + pt.OperatorID + "," + pt.gStationID + "," + pt.mAuthorityid
                        + ",0,'FMADM-9999','DONATION TO CHARITY',0 )";

                        intRecorsaffected = 0;
                        intRecorsaffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                        if (intRecorsaffected == 0) { return false; }

                    }

                }




                totAmt = pt.lblNetAmount;
                StrSql = "Select a.id,a.Amount - (IsNUll(Sum(b.BillAmount),0) + isnull((select sum(amount) " +
                 " FROM iptransactions where receiptno = SUBSTRING(a.receiptno,2,20) group by ReceiptNo), 0)) balance, " +
                 " isnull((select sum(amount) FROM iptransactions " +
                 " where receiptno = SUBSTRING(a.receiptno,2,20) group by ReceiptNo), 0) ipamount" +
                 " FROM  OpDeposit a, OPDepositDetails b where a.id*=b.DepositId and a.ReceiptNo = '" + pt.DepositChkNo + "'" +
                 " And billno is not null AND RegistrationNo=" + pt.Registrationno + " and IssueAuthorityCode ='" + pt.IssueAuthorityCode +
                 "' group by a.amount,a.id,a.receiptno ";

                DataSet Depositrs = MainFunction.SDataSet(StrSql, "tbl", Trans);
                foreach (DataRow rr in Depositrs.Tables[0].Rows)
                {
                    if (totAmt > 0)
                    {
                        if ((decimal)rr["balance"] - totAmt >= 0)
                        {
                            bool UpdateDeposit = MainFunction.SSqlExcuite("insert into opdepositdetails (depositid,billno,billamount) " +
                                               " values (" + (int)rr["id"] + "," + maxId + "," + totAmt + ")", Trans);
                        }
                        else
                        {
                            pt.ErrMsg = "Error when try to save the Deposit amount.";
                            return false;

                        }

                    }


                }

                int BillTYpe = 1;
                if (pt.BillisCredit == true) { BillTYpe = 1; } else { BillTYpe = 0; }
                
                bool InsertInPrinterTable = MainFunction.SSqlExcuite(" INSERT INTO HIS.FRONTOFFICE.BillPrinting "
                    + " ([BillId],[BillType],[WorkStationIp],[OrderId]) VALUES (" + maxId + "," + BillTYpe + ",'" + IPAddress + "'," + MaxBillNo + ")", Trans);



                return true;
            }
            catch (Exception e) { return false; }


        }
        public static bool LoaDetails(Patient pt, SqlTransaction Trans)
        {
            try
            {
                int TPAId = 0;
                int? LOADoctorId;
                int LOAId = 0;
                string LOABillNo = "";
                int RecordsAff;
                int i; string StrSql;
                LOADoctorId = pt.DoctorId;
                StrSql = "update optransaction set currentno=currentno where id=4 ";
                bool UpdateOpt = MainFunction.SSqlExcuite(StrSql, Trans);
                DataSet rsPrice1 = MainFunction.SDataSet("select currentno,name from optransaction where id = 4", "tbl", Trans);
                foreach (DataRow rr in rsPrice1.Tables[0].Rows)
                {
                    LOAId = (int)rr["currentno"] + 1;
                    LOABillNo = rr["name"].ToString();
                }
                pt.mAuthorityid = LOAId;

                StrSql = "update optransaction set currentno= " + pt.mAuthorityid + " where id=4 ";
                bool Updateopt2 = MainFunction.SSqlExcuite(StrSql, Trans);

                StrSql = "Select isnull(TPAId,0) as TPAid from Company where id=" + pt.CompanyId;
                DataSet rsPrice2 = MainFunction.SDataSet(StrSql, "tbl", Trans);
                foreach (DataRow rr in rsPrice2.Tables[0].Rows)
                {
                    TPAId = (int)rr["TPAid"];
                }

                StrSql = "Select ServiceId, LOAAmount as Amount, LOADays as No_Of_Days from OpLOAService a where a.companyid =" + pt.CompanyId
                       + " and a.categoryid=" + pt.CategoryId + " and a.gradeid = " + pt.GradeId + " ";
                DataSet rsPrice3 = MainFunction.SDataSet(StrSql, "tbl", Trans);
                if (rsPrice3.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in rsPrice3.Tables[0].Rows)
                    {
                        int xx = 0;
                        if (pt.Registrationno == 0) { xx = 1; } else { xx = pt.Registrationno; }
                        StrSql = "insert into opLOAorder (registrationno,nodays,loaamount,loabalance,AuthorityId ,AuthorityBillno,loadatetime ,deleted,categoryid,companyid,gradeid,doctorid,IssueAuthoritycode,LetterNo,MedIdNumber,Checked, LoaExpiryDate,TPAId,ServiceId,LOAType,PharmacyAmount,Name) "
                       + " values(" + pt.Registrationno + "," + (int)rr["No_Of_Days"] + "," + (decimal)rr["Amount"] + ",0, " + LOAId + ", '" + LOABillNo + "',getdate()," + xx + "," + pt.CategoryId + "," + pt.CompanyId + " ,"
                       + pt.GradeId + "," + LOADoctorId + ",'" + pt.IssueAuthorityCode + "','" + pt.TxtLOAletterno.ToUpper() + "','" + pt.TxtInsCardno.ToUpper() + "', 0 ,GetDate() ," + TPAId + "," + (int)rr["ServiceId"] + ",2,0,'" + pt.PatientName + "')";

                        RecordsAff = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                        if (RecordsAff == 0)
                        {
                            return false;
                        }


                    }
                }
                else                  {
                    StrSql = "select No_Of_Days,Amount,LOAType,Isnull(PharmacyAmount,0) PharmacyAmount from oploa a where a.companyid =" + pt.CompanyId
                           + " and a.categoryid=" + pt.CategoryId + " and a.gradeid = " + pt.GradeId + " ";

                    DataSet rsPrice4 = MainFunction.SDataSet(StrSql, "tbl", Trans);
                    if (rsPrice4.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rr4 in rsPrice4.Tables[0].Rows)
                        {
                            int rr = 0;
                            if (pt.Registrationno == 0) { rr = 1; } else { rr = pt.Registrationno; }
                            StrSql = "Insert into OpLOAorder (registrationno,nodays,loaamount,loabalance,AuthorityId ,AuthorityBillno,loadatetime ,deleted,categoryid,companyid,gradeid,doctorid,IssueAuthoritycode,LetterNo,MedIdNumber,Checked, LoaExpiryDate,TPAId,ServiceId,LOAType,PharmacyAmount,Name) "
                            + " values(" + pt.Registrationno + "," + (int)rr4["No_Of_Days"] + "," + (decimal)rr4["PharmacyAmount"] + ",0 ," + LOAId + ", '" + LOABillNo + "',getdate()," + rr + "," + pt.CategoryId + "," + pt.CompanyId + " ,"
                            + pt.GradeId + "," + LOADoctorId + ",'" + pt.IssueAuthorityCode + "','" + pt.TxtLOAletterno.ToUpper() + "','"
                            + pt.TxtInsCardno.ToUpper() + "', 0,GetDate()  ," + TPAId + ",0," + (int)rr4["LOAType"] + "," + (decimal)rr4["PharmacyAmount"] + ",'" + pt.PatientName + "')";
                        }
                    }
                    else
                    {
                        StrSql = "Insert into OpLOAOrder (registrationno,nodays,loaamount,loabalance,AuthorityId ,AuthorityBillno,loadatetime ,deleted,categoryid,companyid,gradeid,doctorid,IssueAuthoritycode,LetterNo,MedIdNumber,Checked, LoaExpiryDate,TPAId,ServiceId,PharmacyAmount,LOAType,Name) "
                            + " values(" + pt.Registrationno + ",0,0,0, " + LOAId + ", '" + LOABillNo + "',getdate(),1," + pt.CategoryId + "," + pt.CompanyId + " ,"
                            + pt.GradeId + "," + LOADoctorId + ",'" + pt.IssueAuthorityCode + "','" + pt.TxtLOAletterno.ToUpper() + "','" + pt.TxtInsCardno.ToUpper() + "', 0 ,GetDate()," + TPAId + ",0,0,0,'" + pt.PatientName + "')";

                    }

                    RecordsAff = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                    if (RecordsAff == 0)
                    {
                        return false;
                    }

                }





                return true;
            }
            catch (Exception ed) { return false; }




        }
        public static int CheckLOAAppServices(long ServiceId, long DeptId, long Itemid, long AuthId, String gIACode, int regNo, ref List<AppItemService> AppItemService)
        {
            try
            {
                int tempVal = 0;
                if (AuthId == 0) { return 0; }
                DataSet ds = MainFunction.SDataSet("Exec PR_CheckOPLOAAppovalsDept " + AuthId + ", " + ServiceId + "," + Itemid + "", "tbl");
                int AppQty = 1;
                                 int i = 0;
                if (ds.Tables[0].Rows.Count > 1)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        tempVal = (int)rr["Type"];
                        AppItemService[i].appid = (long)rr["OrderNo"];
                        AppItemService[i].ServiceId = (long)rr["ServiceId"];
                        AppItemService[i].Itemid = (long)rr["Itemid"];
                        AppQty = (int)rr["Qty"];
                        i += 1;
                    }

                }
                else
                {

                    string StrSql = "Select 1 Type,Qty,OrderNo=0,ServiceId,ItemId,DeptId from ARReleaseExclusions "
                      + " where Serviceid= " + ServiceId + " and Deptid = 0 and ItemId =" + Itemid + " and OpBillId=0 and RegNo=" + regNo
                      + " and IACode ='" + gIACode + "'";

                    DataSet ns = MainFunction.SDataSet(StrSql, "tbl2");
                    if (ns.Tables[0].Rows.Count <= 0)
                    {
                        ns.Clear();
                        StrSql = "Select 1 Type,Qty,OrderNo=0,ServiceId,ItemId,Deptid from ARReleaseExclusions "
                             + " where OpBillId=0 and DeptId= " + DeptId + " and RegNo=" + regNo + " and IACode ='" + gIACode + "' and ItemId=0";
                        ns = MainFunction.SDataSet(StrSql, "tbl2");
                    }
                    else if (ns.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rr in ns.Tables[0].Rows)
                        {
                            if (AppItemService.Count > 0) { tempVal = 1; return tempVal; }
                            tempVal = (int)rr["Type"];
                            AppItemService[i].appid = (long)rr["OrderNo"];
                            AppItemService[i].ServiceId = (long)rr["ServiceId"];
                            AppItemService[i].Itemid = (long)rr["Itemid"];
                            AppQty = (int)rr["Qty"];
                            i += 1;
                        }
                    }
                    else
                    {
                        tempVal = 0;
                        AppQty = 1;
                    }
                }
                return 1;
            }
            catch (Exception e)
            {
                                 return 0;
            }

        }

        
        public static List<SearchTable> FoundBills(int billno, int billType, int Gstationid)
        {
            List<SearchTable> xxl = new List<SearchTable>();
            int count = 0;
            try
            {
                string StrSql = "";
                if (billType == 0)                 {
                    StrSql = "select c.id,c.cashbillno as slno,c.Name,c.Datetime,c.creditbillid,c.doctorname,e.Name as Operator,C.amount,c.cancelyesno,f.name as cancelledby,c.compcredit,c.billtype  from cashissue c,employee e,employee f where  c.cashbillno= "
                             + billno + " and c.cancelledby *= f.id and e.id=c.operatorid and c.stationid=" + Gstationid + " ";
                }
                else
                {
                    StrSql = "select c.id,c.creditbillno as slno,c.Name,c.Datetime,c.creditbillid,c.doctorname,e.Name as Operator,C.amount,c.cancelyesno,f.name as cancelledby,c.compcredit,c.billtype  from cashissue c,employee e,employee f where  c.creditbillno = "
                             + billno + " and c.cancelledby *= f.id and e.id=c.operatorid and c.stationid=" + Gstationid + " and c.compcredit =1 ";

                }
                DataSet ds = MainFunction.SDataSet(StrSql, "tbl");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {

                        SearchTable tt = new SearchTable();
                        count += 1;
                        if ((byte)rr["compcredit"] == 1 && (int)rr["billtype"] == 1)
                        {

                            DataSet nn = MainFunction.SDataSet("select creditbillno from cashissue where id = " + (int)rr["creditbillid"], "tbl");
                            foreach (DataRow xx in nn.Tables[0].Rows)
                            {
                                tt.ErrMsg = "This is a Cash Bill Generated for Credit Bill No. " + (int)xx["creditbillno"];
                                xxl.Add(tt);
                                return xxl;
                            }

                        }
                        if ((bool)rr["cancelyesno"] == true)
                        {
                            tt.slno = count;
                            tt.ID = (int)rr["id"];
                            tt.PatientName = (string)rr["Name"];
                            tt.DoctorName = (string)rr["doctorname"];
                            tt.Date = string.Format("{0:dd-MMM-yyyy}", (DateTime)rr["Datetime"]);
                                                         tt.OperatorName = (string)rr["Operator"];
                            tt.BillNo = (int)rr["slno"];
                            tt.Canceled = (bool)rr["cancelyesno"];
                            tt.ErrMsg = "This bill was cancelled by " + MainFunction.NullToString(rr["cancelledby"].ToString())
                                + " .Please check cancelled bills report for details.";
                            xxl.Add(tt);
                        }
                        else
                        {
                            tt.slno = count;
                            tt.ID = (int)rr["id"];
                            tt.PatientName = (string)rr["Name"];
                            tt.DoctorName = (string)rr["doctorname"];
                            tt.Date = string.Format("{0:dd-MMM-yyyy}", (DateTime)rr["Datetime"]);
                            tt.OperatorName = (string)rr["Operator"];
                            tt.BillNo = (int)rr["slno"];
                            tt.Canceled = (bool)rr["cancelyesno"];
                            xxl.Add(tt);
                        }


                    }

                }
                else
                {
                    SearchTable tt = new SearchTable();
                    tt.ErrMsg = "Bill no:" + billno + " does not exist.";
                    xxl.Add(tt);
                    return xxl;

                }

                return xxl;
            }
            catch (Exception e) { SearchTable tt = new SearchTable(); tt.ErrMsg = "No Bills found!"; xxl.Add(tt); return xxl; }


        }
        public static Patient GetBIllDetails(Patient PT)
        {
            try
            {
                Patient rs = new Patient();
                string StrSql = "select datetime,age,agetype,sex,name,doctorname,paymentmode,cardname,cardno,Amount,discount as dis_amount,holderName"
                              + ",card_validity,cash_credit,cancelyesno,comp_id,bankname,doctorid,prefix,pinno,billtype,compcredit,categoryid,gradeid"
                              + ",deducttype,deductper,deductableamt,balance,creditbillid,authorityid,isnull(cashdiscounttype,0) as cashDiscountType,"
                              + " isnull(DispatchDoctorID,0) DispatchDoctorID from cashissue where id=" + PT.COBILLNO + "";
                DataSet nn = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in nn.Tables[0].Rows)
                {
                    rs.lblBillNo = rr["prefix"].ToString() + PT.lblBillNo;
                    rs.mAuthorityid = MainFunction.NullToInteger(rr["authorityid"].ToString());
                    rs.AgeTitle = rr["age"].ToString() + ' ' + MainFunction.GetName("select Name from Agetype where id=" + (byte)rr["agetype"], "Name");
                    rs.SexTitle = MainFunction.GetName("select Name from sex where id=" + (byte)rr["sex"], "Name");
                    rs.CompanyName = MainFunction.GetName("select code + ' - ' + name as Name from company where id=" + (int)rr["comp_id"], "Name");
                    rs.CategoryName = MainFunction.GetName("select code + ' - ' + name as Name from category where id=" + (int)rr["categoryid"], "Name");
                    rs.DisDoctorName = MainFunction.GetName("select name as Name from employee where id=" + (int)rr["DispatchDoctorID"], "Name");
                    rs.DoctorName = MainFunction.GetName("select Empcode + ' - ' + name as Name from doctor where id=" + (int)rr["doctorid"], "Name");
                    rs.IssueAuthorityCode = rr["pinno"].ToString();
                    rs.PatientName = PT.PatientName;
                    rs.OperatorName = PT.OperatorName;
                    rs.lbldate = PT.lbldate;
                                         if (PT.BillisCredit == true)
                    {
                        rs.strExpBatch = "Credit";
                    }
                    else
                    {
                        rs.strExpBatch = "Cash";
                    }
                    if ((byte)rr["deducttype"] == 1)
                    { rs.lblDeductableText = (int)((decimal)rr["deductper"]) + "% Deductable After Discount"; }
                    else if ((byte)rr["deducttype"] == 0)
                    { rs.lblDeductableText = (int)((decimal)rr["deductper"]) + "% Deductable Before Discount"; }

                }

                StrSql = "select  a.serviceid,b.itemcode as itemcode,b.name as itemname,b.conversionqty,b.drugtype,a.quantity"
                    + " ,a.batchno,a.price,a.unitid,a.tax,c.name as unitname,d.quantity as qoh "
                    + " from cashissuedetail  a ,item  b ,packing c ,batchstore d  "
                    + " where  a.batchid=d.batchid and b.id = a.serviceid and a.unitid=c.id and a.batchno=d.batchno and  "
                    + " a.serviceid=d.itemid  and  d.stationid=" + PT.gStationID + " and a.billno =" + PT.COBILLNO + "";
                DataSet dd = MainFunction.SDataSet(StrSql, "tbl");
                int count = 0;
                List<InsertedItemsList> ll = new List<InsertedItemsList>();
                foreach (DataRow rr in dd.Tables[0].Rows)
                {
                    int lGetQty = MainFunction.GetQuantity((int)rr["serviceid"], (int)rr["unitid"]);
                    int LargeQty = (int)rr["qoh"];
                    float tax = (float)Math.Round(MainFunction.NullToFloat(rr["tax"].ToString()), 2);
                    int lSmallQty = (int)((decimal)rr["quantity"] * (decimal)lGetQty);
                    decimal cprice = (decimal)rr["price"];
                    decimal totAmt = ((decimal)rr["price"] * (decimal)rr["quantity"] * (decimal)(tax * 0.01))
                                    + ((decimal)rr["price"] * (decimal)rr["quantity"]);

                    InsertedItemsList tbl = new InsertedItemsList();
                    count += 1;
                    tbl.SNO = count;
                    tbl.qoh = LargeQty;
                    tbl.DrugName = rr["itemcode"].ToString() + " - " + rr["itemname"].ToString();
                    tbl.BatchNo = rr["batchno"].ToString();
                    tbl.Highunit = rr["unitname"].ToString();
                    tbl.price = Math.Round(cprice, 2);
                    tbl.tax = tax;
                    tbl.qty = (int)((decimal)rr["quantity"]);
                    tbl.UnitList = rr["unitname"].ToString();
                    tbl.amount = totAmt;
                    ll.Add(tbl);
                }

                rs.InsertItemValue = ll;


                return rs;

            }
            catch (Exception e)
            {
                Patient px = new Patient();
                px.ErrMsg = "Can't Read the Bill details!";
                return px;
            }


        }

        
        public static PrnIssueMaster CreditPrnIssue(Patient pt, bool Cancelled, long MaxBillNo = 0, long billid = 0, int btype = 0)
        {
            PrnIssueMaster PrnIssue = new PrnIssueMaster();
            try
            {
                int i;
                string bno = "";
                string BName;
                string mskCardNo = "";
                                 decimal DisPer = 0;
                int cashbillno = 0;
                                 int creditbillno = 0;

                byte deducttype = 0;
                decimal deductableamt = 0;
                decimal balance = 0;
                decimal deductper = 0;
                decimal disamount = 0;

                string empname = "";
                string EmployeeID = "";
                bool mDuplicate = false;
                                 string empcode = "";
                byte CardName = 0;

                string StrSql = "select o.pinno,isnull(o.printed,0) printed,isnull(o.cashbillno,0) cashbillno,isnull(o.creditbillno,0) creditbillno ," +
                " f.employeeid,f.name as empname,isnull(o.Dis_Amount,0) Dis_Amount,o.creditbillid,isnull(o.discount,0) discount," +
                " isnull(o.deducttype,0) deducttype,isnull(o.deductper,0) deductper" +
                ",isnull(o.deductableamt,0) deductableamt,isnull(o.balance,0) balance,isnull(o.prefix,'PCS') prefix,isnull(o.CardName,'0') CardName, " +
                " isnull(o.amexslno,0) amexslno,o.datetime as printDate" +
                " from cashissue o,employee f where stationid =" + pt.gStationID +
                " and o.operatorid = f.id and o.id= " + MaxBillNo + "";
                DataSet rsprint = MainFunction.SDataSet(StrSql, "Tbl");
                foreach (DataRow rr in rsprint.Tables[0].Rows)
                {
                    empcode = rr["employeeid"].ToString().Trim();
                    empname = rr["empname"].ToString().Trim();
                    DisPer = (decimal)rr["dis_amount"];
                    disamount = (decimal)rr["discount"];
                    deducttype = (byte)rr["deducttype"];
                    deductableamt = (decimal)rr["deductableamt"];
                    deductper = (decimal)rr["deductper"];
                    balance = (decimal)rr["balance"];
                    cashbillno = (int)rr["cashbillno"];
                    creditbillno = (int)rr["creditbillno"];
                    CardName = (byte)rr["CardName"];
                    PrnIssue.dtpIssueDt = rr["printDate"].ToString();
                    if (Cancelled == false)
                    {
                        if ((byte)rr["printed"] > 0)
                        {
                            mDuplicate = true;
                        }
                    }
                    else
                    {
                        mDuplicate = false;
                    }

                    if (String.IsNullOrEmpty(rr["pinno"].ToString()) == false)
                    {
                        DataSet rsEmp = MainFunction.SDataSet("select e.employeeid from Employee E,FamilyDetails F where f.Empid = e.id "
                            + " and f.regno = " + MainFunction.getRegNumber(rr["pinno"].ToString()) + " and f.pinno = '"
                            + MainFunction.getAuthorityCode(rr["pinno"].ToString())
                            + "'", "tbl");
                        foreach (DataRow rse in rsEmp.Tables[0].Rows)
                        {
                            EmployeeID = rse["employeeid"].ToString();
                        }
                    }

                    if (rr["CardName"].ToString() == "0")
                    {
                        bno = rr["prefix"].ToString();

                    }
                    else
                    {
                        if ((int)rr["amexslno"] > 0)
                        {
                            bno = "AE";
                        }
                        else
                        {
                            bno = "CC";
                        }

                    }

                    if (cashbillno > 0)
                    {
                        DataSet rsTemp = MainFunction.SDataSet("select isnull(dis_amount,0) dis_amount ,isnull(discount,0) discount from cashissue where id  = " + (int)rr["creditbillid"], "tbl");
                        foreach (DataRow rsT in rsTemp.Tables[0].Rows)
                        {
                            DisPer = (decimal)rsT["dis_amount"];
                            disamount = (decimal)rsT["discount"];

                        }
                    }

                } 


                switch (bno)
                {
                    case "AE": BName = "AMERICAN EXPRESS"; break;
                    case "CC":
                        switch (CardName)
                        {
                            case 1: BName = "MASTER CARD"; break;
                            case 2: BName = "VISA CARD"; break;
                            case 4: BName = "DINERS"; break;
                        }

                        break;
                    case "PCS": BName = "CASH SALE"; break;
                    case "CS": BName = "CASH SALE"; break;
                    case "WCSI": BName = "CASH SALE"; break;
                    case "RCSI": BName = "CASH SALE"; break;

                    default:
                        DataSet TEM = MainFunction.SDataSet("SELECT NAME FROM COMPANY WHERE  ID = " + pt.CompanyId, "tbl");
                        foreach (DataRow te in TEM.Tables[0].Rows)
                        {
                            BName = te["NAME"].ToString();
                        }
                        break;
                }



                if (mskCardNo == "____-____-____-____")
                {
                    PrnIssue.Duplicate = "";
                    if (mDuplicate == true) { PrnIssue.Duplicate = "DUPLICATE"; }
                    PrnIssue.bno = bno;
                    PrnIssue.cashbillno = cashbillno.ToString().PadLeft(10, '0');
                    PrnIssue.creditbillno = creditbillno.ToString().PadLeft(10, '0');
                    if (cashbillno > 0)
                    {
                        PrnIssue.BillType = "Cash Bill";
                        PrnIssue.creditbillno = "";
                    }
                    else
                    {
                        PrnIssue.BillType = "Credit Bill";
                        PrnIssue.cashbillno = "";
                    }

                }
                else
                {
                    PrnIssue.Duplicate = "";
                    if (mDuplicate == true) { PrnIssue.Duplicate = "DUPLICATE"; }
                    PrnIssue.bno = bno;
                    PrnIssue.cashbillno = cashbillno.ToString().PadLeft(10, '0');
                    PrnIssue.creditbillno = creditbillno.ToString().PadLeft(10, '0');
                    if (cashbillno > 0)
                    {
                        PrnIssue.BillType = "Cash Bill";
                        PrnIssue.creditbillno = "";
                    }
                    else
                    {
                        PrnIssue.BillType = "Credit Bill";
                        PrnIssue.cashbillno = "";
                    }


                }

                if (Cancelled == true)
                { PrnIssue.CANCELLED = "CANCELLED"; }
                else
                { PrnIssue.CANCELLED = ""; }

                PrnIssue.RegNo = MainFunction.getUHID(pt.IssueAuthorityCode, pt.Registrationno.ToString(), true);
                PrnIssue.PTName = pt.PatientName;
                PrnIssue.CompanyName = MainFunction.GetName("select name from company where id=" + pt.CompanyId, "name");
                if (EmployeeID.Length > 0)
                {
                    PrnIssue.EmployeeID = EmployeeID.ToString();
                }
                else
                {
                    PrnIssue.EmployeeID = "";
                }



                
                StrSql = "select  b.itemcode,b.name as itemname,a.quantity ,a.batchno,a.price,a.tax,c.name as unitname,d.expirydate, left(E.Name,1) AS Cat "
                + " ,round( (a.quantity * a.price) * (1+(a.tax/100)) ,2) as itemAmt "
                + " from cashissuedetail  a ,item  b ,packing c ,batch d, Itemgroup E  where  b.id = a.serviceid and a.unitid=c.id and a.batchno=d.batchno "
                + " AND a.batchid=d.batchid and E.Id = B.Categoryid and  a.serviceid=d.itemid  and  a.billno =" + MaxBillNo + " order by a.slno";
                DataSet rsItem = MainFunction.SDataSet(StrSql, "tbl");

                PrnIssue.ItemList = new List<PrnIssueDetail>();

                foreach (DataRow xr in rsItem.Tables[0].Rows)
                {
                    PrnIssueDetail prndtl = new PrnIssueDetail();
                    prndtl.ItemCode = xr["itemcode"].ToString();
                    prndtl.ItemName = xr["itemname"].ToString();
                    prndtl.quantity = (int)(decimal)xr["quantity"];
                                         prndtl.itemAmt = (decimal)xr["itemAmt"];

                    PrnIssue.BillAmount = PrnIssue.BillAmount + prndtl.itemAmt;

                    PrnIssue.ItemList.Add(prndtl);
                }

                
                if (deducttype == 1)
                {
                    PrnIssue.DoctorName = pt.DoctorName;
                    PrnIssue.DisDoctorName = MainFunction.GetName("select name from employee where id=" + pt.DispatchDoctorID, "Name");
                    PrnIssue.DisPer = DisPer;
                    PrnIssue.Dis = disamount;
                    PrnIssue.NetAmt = PrnIssue.BillAmount - PrnIssue.Dis;
                    if (PrnIssue.bno == "CCR" || PrnIssue.bno == "PCR" || PrnIssue.bno == "WCRI" ||
                        PrnIssue.bno == "RCRI")
                    {
                        PrnIssue.PaidTitle = "Paid Amount";
                        PrnIssue.deductper = deductper;
                        PrnIssue.deductableamt = deductableamt;

                    }
                    else
                    {
                        PrnIssue.PaidTitle = "Deductable Amount";
                        PrnIssue.deductper = deductper;
                        PrnIssue.deductableamt = deductableamt;

                    }

                    PrnIssue.balance = balance;                   }
                else
                {
                    PrnIssue.DoctorName = pt.DoctorName;
                    PrnIssue.DisDoctorName = MainFunction.GetName("select name from employee where id=" + pt.DispatchDoctorID, "Name");
                    if (PrnIssue.bno == "CCR" || PrnIssue.bno == "PCR" || PrnIssue.bno == "WCRI" ||
                            PrnIssue.bno == "RCRI")
                    {
                        PrnIssue.PaidTitle = PrnIssue.DisDoctorName + "    Paid Amount";
                        PrnIssue.deductper = deductper;
                        PrnIssue.deductableamt = deductableamt;
                    }
                    else
                    {
                        PrnIssue.PaidTitle = PrnIssue.DisDoctorName.Trim() + "    Deductable Amount";
                        PrnIssue.deductper = deductper;
                        PrnIssue.deductableamt = deductableamt;
                    }
                    PrnIssue.Dis = PrnIssue.Dis;                     PrnIssue.balance = balance;                   }
                PrnIssue.CashierName = empcode + '-' + empname;
                PrnIssue.BillID = (int)(long)MaxBillNo;

                StrSql = "insert into MMS_CashIssueRptMaster( BillID,RegNo,PTName,Duplicate,bno,cashbillno,creditbillno,BillType,Cancelled," +
                         "CompanyName,Employeeid,dtpIssueDT,DoctorName,DisDoctorName,PaidTitle,CashierName,BillAmount,DisPer,Dis,NetAmt," +
                         "deductper,deductableamt,balance,DonationAmt)  values("
                    + PrnIssue.BillID + ",'" + PrnIssue.RegNo.Trim() + "','" + PrnIssue.PTName.Trim() + "',"
                    + "'" + PrnIssue.Duplicate.Trim() + "','" + PrnIssue.bno.Trim() + "','" + PrnIssue.cashbillno.Trim() + "',"
                    + "'" + PrnIssue.creditbillno.Trim() + "','" + PrnIssue.BillType.Trim() + "','" + PrnIssue.CANCELLED.Trim() + "',"
                    + "'" + PrnIssue.CompanyName.Trim() + "','" + PrnIssue.EmployeeID.Trim() + "','" + PrnIssue.dtpIssueDt.Trim() + "',"
                    + "'" + PrnIssue.DoctorName.Trim() + "','" + PrnIssue.DisDoctorName.Trim() + "','" + PrnIssue.PaidTitle.Trim() + "',"
                    + "'" + PrnIssue.CashierName.Trim() + "'," + PrnIssue.BillAmount + "," + PrnIssue.DisPer + ","
                    + PrnIssue.Dis + "," + PrnIssue.NetAmt + "," + PrnIssue.deductper + ","
                    + PrnIssue.deductableamt + "," + PrnIssue.balance + "," + PrnIssue.DonationAmt + " )";
                bool insertintomaster = MainFunction.SSqlExcuite(StrSql);

                foreach (var rr in PrnIssue.ItemList)
                {
                    StrSql = "insert into MMS_CashIssueRptDetail (BillID,ItemName,ItemCode,quantity,ItemAmt) values ("
                        + PrnIssue.BillID + ",'" + rr.ItemName.Trim() + "','" + rr.ItemCode.Trim() + "'"
                        + "," + rr.quantity + "," + rr.itemAmt + ")";
                    bool insertintodetail = MainFunction.SSqlExcuite(StrSql);
                }

                return PrnIssue;
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }
        public static PrnIssueMaster CashPrnIssue(Patient pt, bool Cancelled, long MaxBillNo = 0, long LinktoCredit = 0, long CreditBillID = 0)
        {
            PrnIssueMaster PrnIssue = new PrnIssueMaster();
            try
            {
                int i;
                string bno = "";
                string BName;
                string mskCardNo = "";
                                 decimal DisPer = 0;
                int cashbillno = 0;
                                 int creditbillno = 0;

                byte deducttype = 0;
                decimal deductableamt = 0;
                decimal balance = 0;
                decimal deductper = 0;
                decimal disamount = 0;

                string empname = "";
                string EmployeeID = "";
                bool mDuplicate = false;
                                 string empcode = "";
                byte CardName = 0;

                string StrSql = "select o.pinno,isnull(o.printed,0) printed,isnull(o.cashbillno,0) cashbillno,isnull(o.creditbillno,0) creditbillno ," +
                " f.employeeid,f.name as empname,isnull(o.Dis_Amount,0) Dis_Amount,o.creditbillid,isnull(o.discount,0) discount," +
                " isnull(o.deducttype,0) deducttype,isnull(o.deductper,0) deductper" +
                ",isnull(o.deductableamt,0) deductableamt,isnull(o.balance,0) balance,isnull(o.prefix,'PCS') prefix,isnull(o.CardName,'0') CardName, " +
                " isnull(o.amexslno,0) amexslno,o.datetime as PrintDate" +
                " from cashissue o,employee f where stationid =" + pt.gStationID +
                " and o.operatorid = f.id and o.id= " + MaxBillNo + "";
                DataSet rsprint = MainFunction.SDataSet(StrSql, "Tbl");
                foreach (DataRow rr in rsprint.Tables[0].Rows)
                {
                    empcode = rr["employeeid"].ToString().Trim();
                    empname = rr["empname"].ToString().Trim();
                    DisPer = (decimal)rr["dis_amount"];
                    disamount = (decimal)rr["discount"];
                    deducttype = (byte)rr["deducttype"];
                    deductableamt = (decimal)rr["deductableamt"];
                    deductper = (decimal)rr["deductper"];
                    balance = (decimal)rr["balance"];
                    cashbillno = (int)rr["cashbillno"];
                    creditbillno = (int)rr["creditbillno"];
                    CardName = (byte)rr["CardName"];
                    PrnIssue.dtpIssueDt = rr["PrintDate"].ToString();
                    if (Cancelled == false)
                    {
                        if ((byte)rr["printed"] > 0)
                        {
                            mDuplicate = true;
                        }
                    }
                    else
                    {
                        mDuplicate = false;
                    }

                    if (String.IsNullOrEmpty(rr["pinno"].ToString()) == false)
                    {
                        DataSet rsEmp = MainFunction.SDataSet("select e.employeeid from Employee E,FamilyDetails F where f.Empid = e.id "
                            + " and f.regno = " + MainFunction.getRegNumber(rr["pinno"].ToString()) + " and f.pinno = '"
                            + MainFunction.getAuthorityCode(rr["pinno"].ToString())
                            + "'", "tbl");
                        foreach (DataRow rse in rsEmp.Tables[0].Rows)
                        {
                            EmployeeID = rse["employeeid"].ToString();
                        }
                    }


                    if (rr["CardName"].ToString() == "0")
                    {
                        bno = rr["prefix"].ToString();

                    }
                    else
                    {
                        if ((int)rr["amexslno"] > 0)
                        {
                            bno = "AE";
                        }
                        else
                        {
                            bno = "CC";
                        }

                    }



                    if (cashbillno > 0)
                    {
                        DataSet rsTemp = MainFunction.SDataSet("select isnull(dis_amount,0) dis_amount ,isnull(discount,0) discount from cashissue where id  = " + (int)rr["creditbillid"], "tbl");
                        foreach (DataRow rsT in rsTemp.Tables[0].Rows)
                        {
                            DisPer = (decimal)rsT["dis_amount"];
                            disamount = (decimal)rsT["discount"];

                        }
                    }

                } 


                switch (bno)
                {
                    case "AE": BName = "AMERICAN EXPRESS"; break;
                    case "CC":
                        switch (CardName)
                        {
                            case 1: BName = "MASTER CARD"; break;
                            case 2: BName = "VISA CARD"; break;
                            case 4: BName = "DINERS"; break;
                        }

                        break;
                    case "PCS": BName = "CASH SALE"; break;
                    case "CS": BName = "CASH SALE"; break;
                    case "WCSI": BName = "CASH SALE"; break;
                    case "RCSI": BName = "CASH SALE"; break;

                    default:
                        DataSet TEM = MainFunction.SDataSet("SELECT NAME FROM COMPANY WHERE  ID = " + pt.CompanyId, "tbl");
                        foreach (DataRow te in TEM.Tables[0].Rows)
                        {
                            BName = te["NAME"].ToString();
                        }
                        break;
                }


                if (mskCardNo == "____-____-____-____")
                {
                    PrnIssue.Duplicate = "";
                    if (mDuplicate == true) { PrnIssue.Duplicate = "DUPLICATE"; }
                    PrnIssue.bno = bno;
                    PrnIssue.cashbillno = cashbillno.ToString().PadLeft(10, '0');
                    PrnIssue.creditbillno = creditbillno.ToString().PadLeft(10, '0');
                    if (cashbillno > 0)
                    {
                        PrnIssue.BillType = "Cash Bill";
                        PrnIssue.creditbillno = "";
                    }
                    else
                    {
                        PrnIssue.BillType = "Credit Bill";
                        PrnIssue.cashbillno = "";
                    }

                }
                else
                {
                    PrnIssue.Duplicate = "";
                    if (mDuplicate == true) { PrnIssue.Duplicate = "DUPLICATE"; }
                    PrnIssue.bno = bno;
                    PrnIssue.cashbillno = cashbillno.ToString().PadLeft(10, '0');
                    PrnIssue.creditbillno = creditbillno.ToString().PadLeft(10, '0');
                    if (cashbillno > 0)
                    {
                        PrnIssue.BillType = "Cash Bill";
                        PrnIssue.creditbillno = "";
                    }
                    else
                    {
                        PrnIssue.BillType = "Credit Bill";
                        PrnIssue.cashbillno = "";
                    }


                }

                if (Cancelled == true)
                { PrnIssue.CANCELLED = "CANCELLED"; }
                else
                { PrnIssue.CANCELLED = ""; }

                PrnIssue.RegNo = MainFunction.getUHID(pt.IssueAuthorityCode, pt.Registrationno.ToString(), true);
                PrnIssue.PTName = pt.PatientName;
                PrnIssue.CompanyName = MainFunction.GetName("select name from company where id=" + pt.CompanyId, "name");
                if (EmployeeID.Length > 0)
                {
                    PrnIssue.EmployeeID = EmployeeID.ToString();
                }
                else
                {
                    PrnIssue.EmployeeID = "";
                }



                
                if (LinktoCredit == 1)
                {
                    StrSql = "select  b.itemcode,b.name as itemname,a.quantity ,a.batchno,a.price,a.tax,c.name as unitname,d.expirydate, left(E.Name,1) AS Cat,dedamt "
                    + " ,round(dedamt * (1+(a.tax/100)),2) as itemAmt "
                    + " ,round( (a.quantity * a.price) * (1+(a.tax/100)) ,2) as BillAmount "
                    + " from cashissuedetail  a ,item  b ,packing c ,batch d, Itemgroup E  where  b.id = a.serviceid and a.unitid=c.id and a.batchno=d.batchno "
                    + " AND a.batchid=d.batchid and E.Id = B.Categoryid and  a.serviceid=d.itemid  and  a.billno =" + CreditBillID + " order by a.slno";
                }
                else
                {
                    StrSql = "select  b.itemcode,b.name as itemname,a.quantity ,a.batchno,a.price,a.tax,c.name as unitname,d.expirydate, left(E.Name,1) AS Cat,dedamt "
                    + " ,round( (a.quantity * a.price) * (1+(a.tax/100)) ,2) as itemAmt "
                    + " ,round( (a.quantity * a.price) * (1+(a.tax/100)) ,2) as BillAmount "
                    + " from cashissuedetail  a ,item  b ,packing c ,batch d, Itemgroup E  where  b.id = a.serviceid and a.unitid=c.id and a.batchno=d.batchno "
                    + " AND a.batchid=d.batchid and E.Id = B.Categoryid and  a.serviceid=d.itemid  and  a.billno =" + MaxBillNo + " order by a.slno";

                }
                DataSet rsItem = MainFunction.SDataSet(StrSql, "tbl");

                PrnIssue.ItemList = new List<PrnIssueDetail>();
                foreach (DataRow xr in rsItem.Tables[0].Rows)
                {
                    PrnIssueDetail prndtl = new PrnIssueDetail();
                    prndtl.ItemCode = xr["itemcode"].ToString();
                    prndtl.ItemName = xr["itemname"].ToString();
                    prndtl.quantity = (int)(decimal)xr["quantity"];
                                         prndtl.itemAmt = (decimal)xr["itemAmt"];
                                         PrnIssue.BillAmount += (decimal)xr["BillAmount"];
                    PrnIssue.ItemList.Add(prndtl);
                }

                
                PrnIssue.BillID = (int)(long)MaxBillNo;
                PrnIssue.DoctorName = pt.DoctorName;
                PrnIssue.DisDoctorName = MainFunction.GetName("select name from employee where id=" + pt.DispatchDoctorID, "name");
                PrnIssue.DisPer = DisPer;
                PrnIssue.Dis = disamount;
                PrnIssue.NetAmt = PrnIssue.BillAmount - PrnIssue.Dis;

                PrnIssue.PaidTitle = "Deductable Amount";
                PrnIssue.deductper = deductper;
                PrnIssue.deductableamt = deductableamt;
                PrnIssue.balance = balance;
                PrnIssue.CashierName = empcode + '-' + empname;
                PrnIssue.DonationAmt = pt.lbldonationAMT;
                if (pt.lbldonationAMT == 0)
                {

                    PrnIssue.balance = PrnIssue.deductableamt;

                }
                else if (pt.lbldonationAMT > 0)
                {
                    PrnIssue.balance = PrnIssue.DonationAmt + PrnIssue.deductableamt;
                }


                StrSql = "insert into MMS_CashIssueRptMaster( BillID,RegNo,PTName,Duplicate,bno,cashbillno,creditbillno,BillType,Cancelled," +
                     "CompanyName,Employeeid,dtpIssueDT,DoctorName,DisDoctorName,PaidTitle,CashierName,BillAmount,DisPer,Dis,NetAmt," +
                     "deductper,deductableamt,balance,DonationAmt)  values("
                   + PrnIssue.BillID + ",'" + PrnIssue.RegNo.Trim() + "','" + PrnIssue.PTName.Trim() + "',"
                   + "'" + PrnIssue.Duplicate.Trim() + "','" + PrnIssue.bno.Trim() + "','" + PrnIssue.cashbillno.Trim() + "',"
                   + "'" + PrnIssue.creditbillno.Trim() + "','" + PrnIssue.BillType.Trim() + "','" + PrnIssue.CANCELLED.Trim() + "',"
                   + "'" + PrnIssue.CompanyName.Trim() + "','" + PrnIssue.EmployeeID.Trim() + "','" + PrnIssue.dtpIssueDt.Trim() + "',"
                   + "'" + PrnIssue.DoctorName.Trim() + "','" + PrnIssue.DisDoctorName.Trim() + "','" + PrnIssue.PaidTitle.Trim() + "',"
                   + "'" + PrnIssue.CashierName.Trim() + "'," + PrnIssue.BillAmount + "," + PrnIssue.DisPer + ","
                   + PrnIssue.Dis + "," + PrnIssue.NetAmt + "," + PrnIssue.deductper + ","
                   + PrnIssue.deductableamt + "," + PrnIssue.balance + "," + PrnIssue.DonationAmt
                   + " )";
                bool insertintomaster = MainFunction.SSqlExcuite(StrSql);


                foreach (var rr in PrnIssue.ItemList)
                {
                    StrSql = "insert into MMS_CashIssueRptDetail (BillID,ItemName,ItemCode,quantity,ItemAmt) values ("
                        + PrnIssue.BillID + ",'" + rr.ItemName.Trim() + "','" + rr.ItemCode.Trim() + "'"
                        + "," + rr.quantity + "," + rr.itemAmt + ")";
                    bool insertintodetail = MainFunction.SSqlExcuite(StrSql);
                }






                return PrnIssue;
            }
            catch (Exception e) { throw new Exception(e.Message); }
        }





                                                                                                                              
                                             
                                                      
                                                                                                                                                                                                                                                                                                         
                                                                                                   
                           
                                                                                                   
         
                                                               
                  
         


                                                                                          
                                             
                                                                        


                                                                                                                                                         
                                                                                                                                                                  

         
                                    
                                                                                                   


                                                      
         
                                                                        
         
                  
                                                                                                                              
                                                      
         
                                                                                                                                                                                                                        
                                                                                                            
                                                               
                                                                                                                                                                  
                                             
                                                      
                                                                                                                                                                                                                                                                                                         
                                                                                                   

                           
                                                                                                   
         


                                                               
                  
         


                                                                                          
                                             
                                                                        

                                                                                                                                                         
                                                                                                                                                                  

         
                                    
                                                                                                   


                                                                                                                                                
                  
                                                                                                                     
                                                               
                                                                        
         
                                             

                                                                                                                     

                                                               





                                    

    }
}

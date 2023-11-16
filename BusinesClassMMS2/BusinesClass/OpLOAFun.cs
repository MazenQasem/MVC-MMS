using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class OpLOAFun
    {
        public static OPLOAOrder GetDetails(string RegNo, String IssueAuthCode)
        {
            OPLOAOrder xlo = new OPLOAOrder();
            try
            {
                String StrSql = "select registrationno from patient where registrationno =" + MainFunction.getRegNumber(RegNo) +
               " and issueauthoritycode= '" + MainFunction.getAuthorityCode(RegNo) + "'";
                DataSet DS = MainFunction.SDataSet(StrSql, "Tb1");
                if (DS.Tables[0].Rows.Count == 0)
                {
                    return Clear("Wrong PIN# Not Found!", IssueAuthCode);
                }
                foreach (DataRow Dr in DS.Tables[0].Rows)
                {
                    xlo.RegistrationNo = MainFunction.NullToInteger2(Dr["registrationno"].ToString());
                    xlo.TxtRegNumber = MainFunction.getUHID(IssueAuthCode, Dr["registrationno"].ToString(),true);


                    StrSql = "Select a.authorityid,a.authoritybillno + ' ' + b.empcode as AuthorityNo,b.name as Doctor  from oploaorder a,employee b " +
                       " where  a.doctorid=b.id and  a.deleted=0 and Registrationno= " + MainFunction.getRegNumber(RegNo) +
                       " and IssueauthorityCode ='" + MainFunction.getAuthorityCode(RegNo) + "' order by authorityid";

                    DataSet Ds = MainFunction.SDataSet(StrSql, "Tb2");
                    List<TempListMdl> tt = new List<TempListMdl>();
                    if (Ds.Tables[0].Rows.Count == 0) { return Clear("No Details Found!", IssueAuthCode); }
                    foreach (DataRow DR in Ds.Tables[0].Rows)
                    {
                        TempListMdl LL = new TempListMdl();
                        LL.Name = DR["AuthorityNo"].ToString();
                        LL.ID = int.Parse(DR["authorityid"].ToString());
                        tt.Add(LL);
                    }
                    xlo.AuthorityLIST = tt;
                }


                TempListMdl DumyRow1 = new TempListMdl();
                DumyRow1.Name = "";
                DumyRow1.ID = 0;
                xlo.AuthorityLIST.Add(DumyRow1);

                return xlo;
            }
            catch (Exception e) { return Clear("Not Found", IssueAuthCode); }
        }

        public static OPLOAOrder Clear(String ErrMsg, string IssueAuthCode)
        {
            OPLOAOrder ns = new OPLOAOrder();
            ns.ErrMsg = ErrMsg;
            ns.TxtRegNumber = IssueAuthCode + '.';

            List<TempListMdl> all = new List<TempListMdl>();
            TempListMdl tt = new TempListMdl();
            tt.ID = 0;
            tt.Name = "";
            all.Add(tt);
            ns.AuthorityLIST = all;
            return ns;


        }

        public static OPLOAOrder GetAuthorityDetail(int cmbAuthorityNo)
        {
            OPLOAOrder OP = new OPLOAOrder();
            String StrSql = "select d.Checked, d.LoaExpiryDate,d.letterno,d.loaamount,d.nodays,d.approval " +
                 ",d.loabalance,a.id as CategoryId,a.name as Category, b.id as CompanyId,b.name as Company, b.code CompCode" +
                 ", c.id as GradeId,c.name as Grade,e.id as Doctorid,e.EmpCode,e.name as Doctor,d.loadatetime,d.AuthorityBillNo" +
                 ",IsNull(d.LOAType,0) LOAType ,IsNull(d.PharmacyAmount,0) PharmacyAmount from " +
                 " category a,company b,grade c,OpLoaOrder d,employee  e where" +
                 " a.id=d.categoryid and b.id=d.companyid and c.id=d.gradeid and e.id=d.doctorid  and d.authorityid= " + cmbAuthorityNo.ToString();
            DataSet DM = MainFunction.SDataSet(StrSql, "Main");
            if (DM.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow DR in DM.Tables[0].Rows)
                {
                    OP.CategoryName = DR["Category"].ToString();
                    OP.CategoryId = int.Parse(DR["Categoryid"].ToString());
                    OP.CompanyName = DR["compcode"].ToString() + " - " + DR["Company"].ToString();
                    OP.CompanyId = int.Parse(DR["CompanyId"].ToString());
                    OP.GradeName = DR["Grade"].ToString();
                    OP.GradeId = int.Parse(DR["GradeId"].ToString());
                    OP.DoctorId = int.Parse(DR["DoctorId"].ToString());
                    OP.DoctorName = DR["empcode"].ToString() + " : " + DR["Doctor"].ToString();
                    OP.Letterno = DR["letterno"].ToString();
                                         OP.StrLoaDateTime = MainFunction.DateFormat(DR["loadatetime"].ToString(), "dd", "MMM", "yyyy", "hh", "mm", "ss", "-", ":");

                    OP.LOAamount = decimal.Parse(DR["loaamount"].ToString());
                    OP.PrvLoa = decimal.Parse(DR["loaamount"].ToString());

                    OP.PharmacyAmount = decimal.Parse(DR["PharmacyAmount"].ToString());
                    OP.PrvPhAmount = decimal.Parse(DR["PharmacyAmount"].ToString());

                    OP.NoDays = int.Parse(DR["nodays"].ToString());
                    OP.PrvDays = int.Parse(DR["nodays"].ToString());  
                    OP.ApprovalNo = "";
                    OP.Notes = "";
                    OP.LOAType = short.Parse(DR["LOAType"].ToString());
                    OP.ConsumedLoaAmount = 0;
                    OP.ConsumedPhAmount = 0;
                    OP.AuthorityId = int.Parse(cmbAuthorityNo.ToString());


                }

                
                StrSql = "select isnull(ApprovalNo,0) ApprovalNo, Isnull(Notes,'') Notes  from  OpLoaOrderModify where autorityid =" + cmbAuthorityNo;
                DataSet ApprovalDS = MainFunction.SDataSet(StrSql, "appr");
                foreach (DataRow Dr in ApprovalDS.Tables[0].Rows)
                {
                    OP.ApprovalNo = Dr["ApprovalNo"].ToString();
                    OP.Notes = Dr["Notes"].ToString();
                }

                
                if (OP.LOAType == 0)
                {
                    StrSql = "select sum(paidamount) as LOADetail from OpLOADetail where Authorityid=" + cmbAuthorityNo;
                    DataSet LOATypeDS = MainFunction.SDataSet(StrSql, "the");
                    foreach (DataRow RR in LOATypeDS.Tables[0].Rows)
                    {
                        OP.ConsumedLoaAmount = decimal.Parse(RR["LOADetail"].ToString());
                    }

                }
                else
                {

                    StrSql = "select Sum(case when serviceid <>11 then paidamount else 0 end) NPHAmount," +
                             " Sum(case when serviceid =11 then paidamount else 0 end) PHAmount  from OpLOADetail where Authorityid=" + cmbAuthorityNo;
                    DataSet LOATypeDS = MainFunction.SDataSet(StrSql, "the");
                    foreach (DataRow RR in LOATypeDS.Tables[0].Rows)
                    {
                        OP.ConsumedLoaAmount = MainFunction.NullToDecmial(RR["NPHAmount"].ToString());
                        OP.ConsumedPhAmount = MainFunction.NullToDecmial(RR["PHAmount"].ToString());
                    }

                }
                OP.BtnUpdateAllowed = true;

            }
            else
            {
                OP.BtnUpdateAllowed = false;

            }
            return OP;

        }

        public static Boolean Save(OPLOAOrder sp, int SavedBy)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    int lngMaxId = 0;

                    String StrSql = "";
                    
                    
                    StrSql = "insert into OpLoaOrderModify(AutorityId,Registrationno,Amount,PharmacyAmount,NoDays,Datetime,OperatorId,ApprovalNo,Notes) " +
                        " values( " + sp.AuthorityId + "," + MainFunction.getRegNumber(sp.TxtRegNumber) + "," + sp.PrvLoa + "," + sp.PrvPhAmount + "," + sp.PrvDays +
                        ",getdate(), " + SavedBy + ", '" + sp.ApprovalNo + "', '" + sp.Notes + "' )";

                    int NoOfRecordAffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);

                    if (NoOfRecordAffected == 0)
                    {
                        Trans.Rollback(); sp.ErrMsg = "Could not Save"; return false;
                    }
                    NoOfRecordAffected = 0;                                                                                                                                                                         
                    
                    
                    StrSql = "update oploaorder set LoaAmount=" + sp.LOAamount + ",PharmacyAmount =" + sp.PharmacyAmount +
                        ", approval=0 ,nodays=" + sp.NoDays + " where registrationno=" + MainFunction.getRegNumber(sp.TxtRegNumber) +
                        " and issueauthoritycode ='" + MainFunction.getAuthorityCode(sp.TxtRegNumber) +
                        "' and authorityid = " + sp.AuthorityId;
                    NoOfRecordAffected = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                    if (NoOfRecordAffected == 0)
                    {
                        Trans.Rollback(); sp.ErrMsg = "Could not Save"; return false;
                    }
                    NoOfRecordAffected = 0; 
                    sp.ErrMsg = "Record(s) Saved Successfully";

                    Trans.Commit();
                    return true;
                }
            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                sp.ErrMsg = "Saving>>" + e.Message;
                return false;
            }
        }


    }

}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace MMS2
{
    public class ListAllFun
    {


        public List<DoctorList> getAllDoctors()
        {
            var doctors = new List<DoctorList>();
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT doc.ID AS Id, doc.empcode+ '-' + doc.FirstName + ' ' + doc.MiddleName  + ' ' +doc.LastName  AS  Name FROM Doctor doc where deleted = 0 Order by doc.empcode ");
                doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<DoctorList>();

            }
            catch (Exception ex)
            {
                return doctors;
            }

            return doctors;

        }

        public List<CategoryList> getAllCategories()
        {
            var doctors = new List<CategoryList>();
            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("select Id,Name from Itemgroup order by name  ");
                doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<CategoryList>();

            }
            catch (Exception ex)
            {
                return doctors;
            }

            return doctors;

        }

         public List<PinList> getAllInpatientEmployee()
        {
            var doctors = new List<PinList>();
            try
            {
 
                StringBuilder query = new StringBuilder();
                query.Append("SELECT  *  FROM  ( select '0' As IpId, 'ALL' as RegNo  union all ");
                query.Append("SELECT  InPatient.IPID as IPId,(Inpatient.issueauthoritycode +'.' + REPLICATE('0',10-(LEN(CONVERT(Varchar(10),Inpatient.registrationno)))) + CONVERT(Varchar(10),Inpatient.registrationno)) as RegNo  FROM InPatient,Bed WHERE InPatient.IPID = Bed.IPID AND AdmitDateTime> '23-Dec-2006' and (Bed.Status = 5 or Bed.Status = 4)  ");
                query.Append(" ) x order by x.RegNo ");
                doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<PinList>();

            }
            catch (Exception ex)
            {
                return doctors;
            }

            return doctors;

        } 

         public List<DirectIpIssueList> getAjax_ViewDatafromIPid(int PinId , int StationId)
         {
             var doctors = new List<DirectIpIssueList>();
             try
             {

                 StringBuilder query = new StringBuilder();

 

                 query.Append(" Select I.Id as ID,i.StationSLno,B.Name as Bed,I.DateTime as [DateTime],O.Name as Operator, I.IPID as IpNo,D.Name as Doctor,"+
                 " P.FirstName + P.MiddleName + P.LastName as PatientName from DrugOrder  I,Inpatient P,Doctor D,Employee O,Bed B "+
                 " where B.ID=I.BedID and I.IPID=P.IPID and D.Id=I.DoctorID and I.OperatorId=O.Id   ");
                 query.Append(" and ( '" + PinId + "' = '0' OR  I.IPID= '" + PinId + "') ");
                 query.Append(" and i.stationid = '" + StationId + "'  and I.tostationid= '" + StationId + "' order by DateTime desc ");
   
                 doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<DirectIpIssueList>();

             }
             catch (Exception ex)
             {
                 return doctors;
             }

             return doctors;

         }

         public List<BedList> getAllBedName()
        {
            var doctors = new List<BedList>();
            try
            {
 
                StringBuilder query = new StringBuilder();
                query.Append(" SELECT InPatient.IPID as Id, Bed.Name FROM InPatient,Bed WHERE InPatient.IPID = Bed.IPID AND AdmitDateTime> '23-Dec-2006' and (Bed.Status = 5 or Bed.Status = 4)  ");
                doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<BedList>();

            }
            catch (Exception ex)
            {
                return doctors;
            }

            return doctors;

        }

         public List<BedList> getAllPatientList()
         {
             var doctors = new List<BedList>();
             try
             {

                 StringBuilder query = new StringBuilder();
                 query.Append("select * from (SELECT InPatient.IPID as Id , InPatient.Title+' ' +InPatient.FirstName+' ' + InPatient.MiddleName+' ' + InPatient.LastName  as Name  ");
                 query.Append(" FROM InPatient,Bed WHERE InPatient.IPID = Bed.IPID  ");
                 query.Append("  AND AdmitDateTime> '23-Dec-2006' and (Bed.Status = 5 or Bed.Status = 4) ) x order by x.Name  ");

                 doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<BedList>();

             }
             catch (Exception ex)
             {
                 return doctors;
             }

             return doctors;

         }
         public List<InformationDatalist> getInpatientInfo(int IpId)
         {
             var doctors = new List<InformationDatalist>();
             try
             {
                
                 StringBuilder query = new StringBuilder();
                 query.Append("  Select I.IPID,(lTrim(I.FirstName) + ' ' + LTrim(I.MiddleName) + ' ' + lTrim(I.LastName)) as Name,   ");
                 query.Append(" I.Sex,I.Age,I.SexOthers,i.DoctorId,B.Name as BedName,b.id as bedid , isnull(I.VIP,0) as VIP  ");
                 query.Append("   from Inpatient I,Bed B where B.IPID=I.IPID    and ( '" + IpId + "' = '0' OR   I.IPID= '" + IpId + "' )  ");

                 doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<InformationDatalist>();
                 

                 

             }
             catch (Exception ex)
             {
                 return doctors;
             }

             return doctors;

         }

         public int checkIsPatientIsinOP(int IpId)
         {
             try
             {
                 var checkOP = MainFunction.ExecuteSQLAndReturnDataTable(" Select FromDatetime,ToDatetime from OTSchedule where  ipidopid = '" + IpId + "' and PatientType = 1 and ReservedConfirmed = 2 and getdate() >= fromdatetime and getdate() <= ToDatetime ");  
                 if (checkOP.Rows.Count > 0)
                 { return 1; } else { return 0; }
             }
             catch (Exception)
             {
                 return 0;
             }          
         }

         public DirectIpSaveModel  checkdispatch3days(DirectIpSaveModel order)
         {
            
             foreach (var it in order.IssueList)
             {
                 String Msg = "";
                 string StrSql = " select  b.ServiceId,a.ipid,a.DispatchedDateTime as DDate,b.DispatchQuantity as Qty,i.itemcode,i.name "
                + "from drugorder a   left join DrugOrderDetailSubstitute b on a.ID=b.OrderId  left join Item i on b.ServiceId=i.Id  "
                + "Where a.DispatchedDateTime > DateAdd(Day, -3, sysdatetime())  and b.ServiceId= '" + it.ID + "'       and a.ipid = '" + order.IpId + "' ";
                 DataSet nw = MainFunction.SDataSet(StrSql, "tbl2");
                 if (nw.Tables[0].Rows.Count > 0)
                 {
                     Msg = "The following items are already issued:<br/> ";
                     int counter = 1;
                     foreach (DataRow nn in nw.Tables[0].Rows)
                     {
                         Msg += "("+counter + ") " + nn["itemcode"] + " " + nn["name"] + " Date:" + nn["DDate"] + "  Qty:" + nn["Qty"] + "<br/>";
                         counter = counter + 1;
                     }
                     order.ErrMsg = Msg;
                 }
             }
             return order;
         }

       
        



          
	

    }
}

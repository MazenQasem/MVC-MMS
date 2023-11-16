using System;
using System.Collections.Generic;
using System.Web.Mvc;
  
namespace MMS2.Controllers
{
    [MMCFilter]      public class OpLoaController : Controller
    {
                  
        public ActionResult Index(String RegNo = "0", int AuthoId = 0)
        {
                         User UserData = (User)Session["User"];
            int featureid = 506;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }


            OPLOAOrder op = new OPLOAOrder();

            List<TempListMdl> all = new List<TempListMdl>();
            TempListMdl tt = new TempListMdl();
            tt.ID = 0;
            tt.Name = "";
            all.Add(tt);

            op.AuthorityLIST = all;
            op.TxtRegNumber = UserData.gIACode + '.';

            if (RegNo.Length > 4)              {
                                 op = OpLOAFun.GetDetails(RegNo.ToString(), UserData.gIACode);


                if (AuthoId > 0)
                {
                    op.AuthorityId = AuthoId;
                }
            }

            return View(op);
        }
        public JsonResult GetApprovalDtl(int RegNO, int AuthoID)
        {
            OPLOAOrder OP = new OPLOAOrder();
            OP = OpLOAFun.GetAuthorityDetail(AuthoID);

            return Json(OP);
        }
        public JsonResult SaveApproval(OPLOAOrder OP)
        {
            User UserData = (User)Session["User"];
            int featureid = 506;
            int functionid = 2;
            if (MainFunction.UserAllowedFunction(UserData,featureid, functionid) == true)
            {
                                 bool bn = OpLOAFun.Save(OP, UserData.EmpID);
                if (bn == false)
                {
                    return Json(new MessageModel { Message = "Error during saving,Check Your Entry!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
                }
                else
                { 
                    return Json(new MessageModel { Message = "Successfully Saved!", isSuccess = true, date = DateTime.Now.ToShortDateString() }); 
                }
            }
            else
            {
                return Json(new MessageModel { Message = "you not allowed to Save!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
            }

        }

    }
}

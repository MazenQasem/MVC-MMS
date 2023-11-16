using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 
namespace MMS2
{
    public partial class AlterMRP
    {
        public DateTime CurrentDateTime_Add { get; set; }
        public List<Item> DrugList_Add {get;set;}
        public List<Item> ConsumableList_Add { get; set; }
        public List<Item> OthersList_Add { get; set; }
        
        public List<Item> UpdatedMRPList_Add { get; set; }
        public String OperatorName { get; set; }
        public String ErrMsg { get; set; }
        
    }
    public partial class Item
    {
                          public string BatchNo { get; set; }
        public string BatchID { get; set; }
        public float BatchTax { get; set; }        
        public decimal OldCP { get; set; }
        public decimal OldMRP { get; set; }
        [Required]
        public decimal NewMRP { get; set; }
        public DateTime ExpriyDate { get; set; }
        public DateTime ExpriyDateOld { get; set; }
        public String UOM { get; set; }
    }
}

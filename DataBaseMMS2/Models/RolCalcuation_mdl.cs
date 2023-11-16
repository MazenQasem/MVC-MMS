
using System;
using System.Collections.Generic;
 
namespace MMS2
{
    public partial class ItemAvgSale
    {
        public int ShowMonth { get; set; }
        public int ShowFactor { get; set; }
        public List<ItemGroup> ItemGroupList { get; set; }
        public int ItemGrouplistID { get; set; }
        public List<MMS_ItemMaster> ItemList { get; set; }
        public int ItemListID { get; set; }
        public bool AllItem { get; set; }
        public String ErrMsg { get; set; }

    }
}
 using System;
using System.Collections.Generic;



namespace MMS2
{

                    
          

          
          
          
    public class ManufacturerS
    {
        public Manufacturer  Manufacturerx { get; set; }
        
        public List<MMS_ItemMaster> ItemList { get; set; }
        public int ItemlistID { get; set; }

        public IList<ManufactrureItems> SelectedManufacturerItems { get; set; }
        public int[] SelectedItemsID { get; set; }


        public List<SupplierS> SupplierList { get; set; }
        public int SupplierID { get; set; }

        public List<ManufacturerSupplier> SelectedSupplier { get; set; }
        public int SelectedManSupID { get; set; }

        public String MessageShow { get; set; }
    }

    public class ManufactrureItems
    {
        public int ManufacturerID { get; set; }
        public int ItemID { get; set; }
        public string PartNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
    }

    public class ManufacturerSupplier
    {
        public int ManufacturerID { get; set; }
        public int SupplierID { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }

    
}
using System.ComponentModel.DataAnnotations;
 using System;
using System.Collections.Generic;



namespace MMS2
{
    public class SupplierS
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public String Code { get; set; }
        [Required(ErrorMessage = "Please Write the SupplierName ")]
        public String Name { get; set; }
        [Required]
        public String ContactPerson { get; set; }
        [Required]
        public String Grade { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String City { get; set; }
        public String Country { get; set; }
        public String Box { get; set; }
        [Required]
        public String Phone1 { get; set; }
        public String Phone2 { get; set; }
        public String Page { get; set; }
        public String Cell { get; set; }
        public String Fax { get; set; }
        public String Telex { get; set; }
        public String Email { get; set; }
        public String web_site { get; set; }
        public Byte PaymentType { get; set; }
        public int PaymentDays { get; set; }
        public Int16 NetDueDays { get; set; }
        public Int16 DiscountDays { get; set; }
        public Byte Percentage { get; set; }
        public Single CreditLimit { get; set; }
        public Boolean VendorStatus { get; set; }
        public Boolean OverSeas { get; set; }
        public Boolean Active { get; set; }
        public int BankID { get; set; }
        public String AccountNo { get; set; }
        public Boolean Deleted { get; set; }
        public DateTime StartdateTime { get; set; }
        public DateTime EndDatetime { get; set; }

        public List<BankS> BankList { get; set; }

        public List<MMS_ItemMaster> ItemList { get; set; }
        public int ItemlistID { get; set; }

        public IList<SupplierItems> SelectedSupplierItems { get; set; }

        public int[] SelectedItemsID { get; set; }


        public List<Manufacturer> ManufacturerList { get; set; }
        public int ManufacturerID { get; set; }

        public List<SupplierManf> SelectedManufacturer { get; set; }
        public int SelectedSupManID { get; set; }

        public String SupplierErr { get; set; }
    }

    public class SupplierItems
    {
        public int SUPPLIERID { get; set; }
        public int ItemID { get; set; }
        public string Itemname { get; set; }
        public string Itemcode { get; set; }
    }

    public class SupplierManf
    {
        public int SUPPLIERID { get; set; }
        public int ManufacturerID { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }

}
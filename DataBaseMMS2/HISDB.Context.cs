        
namespace MMS2
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HISEntities : DbContext
    {
        public HISEntities()
            : base("name=HISEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<BatchStore> BatchStores { get; set; }
        public DbSet<ItemGroup> ItemGroups { get; set; }
        public DbSet<ItemStore> ItemStores { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Packing> Packings { get; set; }
        public DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
        public DbSet<PurchaseReceiptDetail> PurchaseReceiptDetails { get; set; }
        public DbSet<supplier> suppliers { get; set; }
        public DbSet<AlterMRP> AlterMRPs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<ItemAvgSale> ItemAvgSales { get; set; }
        public DbSet<BatchLocator> BatchLocators { get; set; }
        public DbSet<Rack> Racks { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<MMS_ItemMaster> MMS_ItemMaster { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<OPBService> OPBServices { get; set; }
        public DbSet<OPLOAOrder> OPLOAOrders { get; set; }
        public DbSet<OPLOAOrderModify> OPLOAOrderModifies { get; set; }
        public DbSet<OPLOAService> OPLOAServices { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}

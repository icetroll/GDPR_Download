namespace GDPR_Download.API.Data.Migration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnection : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GuestLoginModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Mail = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        Sent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GuestLoginModels");
        }
    }
}

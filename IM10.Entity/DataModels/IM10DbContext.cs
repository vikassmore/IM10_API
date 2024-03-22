using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IM10.Entity.DataModels;

public partial class IM10DbContext : DbContext
{
    public IM10DbContext()
    {
    }

    public IM10DbContext(DbContextOptions<IM10DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdvContentDetail> AdvContentDetails { get; set; }

    public virtual DbSet<AdvContentMapping> AdvContentMappings { get; set; }

    public virtual DbSet<AppDetail> AppDetails { get; set; }

    public virtual DbSet<CampaignDetail> CampaignDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<ContentAuditLog> ContentAuditLogs { get; set; }

    public virtual DbSet<ContentDetail> ContentDetails { get; set; }

    public virtual DbSet<ContentFlag> ContentFlags { get; set; }

    public virtual DbSet<ContentType> ContentTypes { get; set; }

    public virtual DbSet<ContentView> ContentViews { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EndorsmentDetail> EndorsmentDetails { get; set; }

    public virtual DbSet<EndorsmentType> EndorsmentTypes { get; set; }

    public virtual DbSet<Fcmnotification> Fcmnotifications { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<ListingDetail> ListingDetails { get; set; }

    public virtual DbSet<LogInformation> LogInformations { get; set; }

    public virtual DbSet<MarketingCampaign> MarketingCampaigns { get; set; }

    public virtual DbSet<MenuPage> MenuPages { get; set; }

    public virtual DbSet<Otpautherization> Otpautherizations { get; set; }

    public virtual DbSet<PlayerData> PlayerData { get; set; }

    public virtual DbSet<PlayerDetail> PlayerDetails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleMenuPageMapping> RoleMenuPageMappings { get; set; }

    public virtual DbSet<SportMaster> SportMasters { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<UserAuditLog> UserAuditLogs { get; set; }

    public virtual DbSet<UserDeviceMapping> UserDeviceMappings { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    public virtual DbSet<UserPlayerMapping> UserPlayerMappings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=meshbasqldb.c5nzv8jpq3sq.ap-south-1.rds.amazonaws.com;Initial Catalog=IM10DB;User ID=IM10Admin;Password=Admin@IM10;TrustServerCertificate=True;Trusted_Connection=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdvContentDetail>(entity =>
        {
            entity.HasKey(e => e.AdvertiseContentId).HasName("PK_AdvVideoContentDetails");

            entity.Property(e => e.Comment).HasMaxLength(200);
            entity.Property(e => e.FinalPrice).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.City).WithMany(p => p.AdvContentDetails)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentDetails_City");

            entity.HasOne(d => d.ContentType).WithMany(p => p.AdvContentDetails)
                .HasForeignKey(d => d.ContentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentDetails_ContentType");

            entity.HasOne(d => d.Nation).WithMany(p => p.AdvContentDetails)
                .HasForeignKey(d => d.NationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentDetails_Country");

            entity.HasOne(d => d.Player).WithMany(p => p.AdvContentDetails)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentDetails_PlayerDetails");

            entity.HasOne(d => d.State).WithMany(p => p.AdvContentDetails)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentDetails_State");
        });

        modelBuilder.Entity<AdvContentMapping>(entity =>
        {
            entity.HasKey(e => e.AdvContentMapId).HasName("PK_VideoAdvContentMapping");

            entity.ToTable("AdvContentMapping");

            entity.HasOne(d => d.AdvertiseContent).WithMany(p => p.AdvContentMappings)
                .HasForeignKey(d => d.AdvertiseContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentMapping_AdvContentDetails");

            entity.HasOne(d => d.Category).WithMany(p => p.AdvContentMappings)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentMapping_Category");

            entity.HasOne(d => d.Content).WithMany(p => p.AdvContentMappings)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentMapping_ContentDetails");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.AdvContentMappings)
                .HasForeignKey(d => d.SubCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdvContentMapping_SubCategory");
        });

        modelBuilder.Entity<AppDetail>(entity =>
        {
            entity.HasKey(e => e.AppDetailsId);

            entity.Property(e => e.AppName)
                .HasMaxLength(50)
                .HasColumnName("App Name");
            entity.Property(e => e.Appkey).HasColumnName("APPKey");
        });

        modelBuilder.Entity<CampaignDetail>(entity =>
        {
            entity.HasKey(e => e.CampaignId);

            entity.Property(e => e.SocialMediaViews).HasMaxLength(100);

            entity.HasOne(d => d.MarketingCampaign).WithMany(p => p.CampaignDetails)
                .HasForeignKey(d => d.MarketingCampaignId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CampaignDetails_MarketingCampaigns");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Sport).WithMany(p => p.Categories)
                .HasForeignKey(d => d.SportId)
                .HasConstraintName("FK_Category_SportMaster");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_VideoComment");

            entity.Property(e => e.Comment1)
                .HasMaxLength(500)
                .HasColumnName("Comment");
            entity.Property(e => e.Liked).HasDefaultValueSql("((0))");
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Shared).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Content).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_ContentDetails");

            entity.HasOne(d => d.ContentType).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ContentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_ContentType");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_Comments_Comments");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_UserMaster");
        });

        modelBuilder.Entity<ContentAuditLog>(entity =>
        {
            entity.HasKey(e => e.ContentLogId).HasName("PK_VideoContentLog");

            entity.Property(e => e.Comment).HasMaxLength(200);
            entity.Property(e => e.ContentTitle).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Content).WithMany(p => p.ContentAuditLogs)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentAuditLogs_ContentDetails");
        });

        modelBuilder.Entity<ContentDetail>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK_VideoContentDetails");

            entity.Property(e => e.Comment).HasMaxLength(200);
            entity.Property(e => e.ContentFileName1).HasColumnName("ContentFileName_1");
            entity.Property(e => e.ContentFilePath1).HasColumnName("ContentFilePath_1");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Thumbnail1).HasColumnName("Thumbnail_1");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.ContentDetails)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentDetails_Category");

            entity.HasOne(d => d.ContentType).WithMany(p => p.ContentDetails)
                .HasForeignKey(d => d.ContentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentDetails_ContentType");

            entity.HasOne(d => d.Language).WithMany(p => p.ContentDetails)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK_ContentDetails_Language");

            entity.HasOne(d => d.Player).WithMany(p => p.ContentDetails)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentDetails_PlayerDetails");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.ContentDetails)
                .HasForeignKey(d => d.SubCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentDetails_SubCategory");
        });

        modelBuilder.Entity<ContentFlag>(entity =>
        {
            entity.HasKey(e => e.ContentFlagId).HasName("PK_VideoContentFlags");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentFlags)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentFlags_ContentDetails");

            entity.HasOne(d => d.ContentType).WithMany(p => p.ContentFlags)
                .HasForeignKey(d => d.ContentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentFlags_ContentType");

            entity.HasOne(d => d.Player).WithMany(p => p.ContentFlags)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentFlags_PlayerDetails");

            entity.HasOne(d => d.User).WithMany(p => p.ContentFlags)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentFlags_UserMaster");
        });

        modelBuilder.Entity<ContentType>(entity =>
        {
            entity.ToTable("ContentType");

            entity.Property(e => e.ContentName).HasMaxLength(100);
        });

        modelBuilder.Entity<ContentView>(entity =>
        {
            entity.HasOne(d => d.Content).WithMany(p => p.ContentViews)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentViews_ContentDetails");

            entity.HasOne(d => d.ContentType).WithMany(p => p.ContentViews)
                .HasForeignKey(d => d.ContentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentViews_ContentType");

            entity.HasOne(d => d.Player).WithMany(p => p.ContentViews)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentViews_PlayerDetails");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");

            entity.Property(e => e.CountryCode).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<EndorsmentDetail>(entity =>
        {
            entity.HasKey(e => e.EndorsmentId);

            entity.ToTable("EndorsmentDetail");

            entity.Property(e => e.EndorsmentType).HasMaxLength(100);
            entity.Property(e => e.FinalPrice).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(200);

            entity.HasOne(d => d.Listing).WithMany(p => p.EndorsmentDetails)
                .HasForeignKey(d => d.ListingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EndorsmentDetail_ListingDetails");

            entity.HasOne(d => d.Player).WithMany(p => p.EndorsmentDetails)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EndorsmentDetail_PlayerDetails");
        });

        modelBuilder.Entity<EndorsmentType>(entity =>
        {
            entity.ToTable("EndorsmentType");

            entity.Property(e => e.EndorsmentDescription)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.EndorsmentName).HasMaxLength(100);
        });

        modelBuilder.Entity<Fcmnotification>(entity =>
        {
            entity.ToTable("FCMNotification");

            entity.Property(e => e.FcmnotificationId).HasColumnName("FCMNotificationID");

            entity.HasOne(d => d.Player).WithMany(p => p.Fcmnotifications)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FCMNotification_PlayerDetails");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("Language");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ListingDetail>(entity =>
        {
            entity.HasKey(e => e.ListingId);

            entity.Property(e => e.CompanyEmailId).HasMaxLength(100);
            entity.Property(e => e.CompanyMobile).HasMaxLength(15);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CompanyPhone).HasMaxLength(15);
            entity.Property(e => e.CompanyWebSite).HasMaxLength(100);
            entity.Property(e => e.ContactPersonEmailId).HasMaxLength(100);
            entity.Property(e => e.ContactPersonMobile).HasMaxLength(15);
            entity.Property(e => e.ContactPersonName).HasMaxLength(200);
            entity.Property(e => e.FinalPrice).HasMaxLength(100);
            entity.Property(e => e.IsGlobal).HasDefaultValueSql("((0))");
            entity.Property(e => e.Position).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Category).WithMany(p => p.ListingDetails)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_ListingDetails_Category");

            entity.HasOne(d => d.City).WithMany(p => p.ListingDetails)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_ListingDetails_City");

            entity.HasOne(d => d.Nation).WithMany(p => p.ListingDetails)
                .HasForeignKey(d => d.NationId)
                .HasConstraintName("FK_ListingDetails_Country");

            entity.HasOne(d => d.Player).WithMany(p => p.ListingDetails)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ListingDetails_PlayerDetails");

            entity.HasOne(d => d.State).WithMany(p => p.ListingDetails)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("FK_ListingDetails_State");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.ListingDetails)
                .HasForeignKey(d => d.SubCategoryId)
                .HasConstraintName("FK_ListingDetails_SubCategory");
        });

        modelBuilder.Entity<LogInformation>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("LogInformation");

            entity.Property(e => e.AdditionalInformation).HasMaxLength(200);
            entity.Property(e => e.LogMessage).HasMaxLength(200);
            entity.Property(e => e.LogSource).HasMaxLength(100);
            entity.Property(e => e.LogType).HasMaxLength(100);
            entity.Property(e => e.StackTrace).HasMaxLength(100);
        });

        modelBuilder.Entity<MarketingCampaign>(entity =>
        {
            entity.HasKey(e => e.MarketingCampaignId).HasName("PK_SalesPersonMaster");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Content).WithMany(p => p.MarketingCampaigns)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MarketingCampaigns_ContentDetails");

            entity.HasOne(d => d.ContentType).WithMany(p => p.MarketingCampaigns)
                .HasForeignKey(d => d.ContentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MarketingCampaigns_ContentType1");

            entity.HasOne(d => d.Player).WithMany(p => p.MarketingCampaigns)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MarketingCampaigns_PlayerDetails1");
        });

        modelBuilder.Entity<MenuPage>(entity =>
        {
            entity.ToTable("MenuPage");

            entity.Property(e => e.MenuPageName).HasMaxLength(50);
        });

        modelBuilder.Entity<Otpautherization>(entity =>
        {
            entity.HasKey(e => e.Otpid).HasName("PK_OPTAutherization");

            entity.ToTable("OTPAutherization");

            entity.Property(e => e.Otpid).HasColumnName("OTPId");
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .HasColumnName("OTP");
            entity.Property(e => e.OtpvalidDateTime).HasColumnName("OTPValidDateTime");

            entity.HasOne(d => d.User).WithMany(p => p.Otpautherizations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OTPAutherization_UserMaster");
        });

        modelBuilder.Entity<PlayerData>(entity =>
        {
            entity.HasKey(e => e.PlayerDataId);

            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerData)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlayerData_PlayerDetails");
        });

        modelBuilder.Entity<PlayerDetail>(entity =>
        {
            entity.HasKey(e => e.PlayerId);

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.BankAcountNo).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PancardNo)
                .HasMaxLength(15)
                .HasColumnName("PANCardNo");

            entity.HasOne(d => d.Sport).WithMany(p => p.PlayerDetails)
                .HasForeignKey(d => d.SportId)
                .HasConstraintName("FK_PlayerDetails_SportMaster");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<RoleMenuPageMapping>(entity =>
        {
            entity.HasKey(e => e.RoleMenuPageId).HasName("PK_RolePermissionMapping");

            entity.ToTable("RoleMenuPageMapping");

            entity.HasOne(d => d.MenuPage).WithMany(p => p.RoleMenuPageMappings)
                .HasForeignKey(d => d.MenuPageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleMenuPageMapping_MenuPage");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenuPageMappings)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleMenuPageMapping_Role");
        });

        modelBuilder.Entity<SportMaster>(entity =>
        {
            entity.HasKey(e => e.SportId);

            entity.ToTable("SportMaster");

            entity.Property(e => e.SportName).HasMaxLength(100);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.StateCode).HasMaxLength(50);

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_State_Country");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.ToTable("SubCategory");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubCategory_Category");
        });

        modelBuilder.Entity<UserAuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK_AuditLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.UserAuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAuditLogs_UserMaster");
        });

        modelBuilder.Entity<UserDeviceMapping>(entity =>
        {
            entity.HasKey(e => e.UserDeviceId);

            entity.ToTable("UserDeviceMapping");

            entity.HasOne(d => d.User).WithMany(p => p.UserDeviceMappings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDeviceMapping_UserMaster");
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserMaster");

            entity.Property(e => e.AppId).HasColumnName("AppID");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.EmailId).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MobileNo).HasMaxLength(15);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserPlayerMapping>(entity =>
        {
            entity.HasKey(e => e.UserPlayerId);

            entity.ToTable("UserPlayerMapping");

            entity.HasOne(d => d.Player).WithMany(p => p.UserPlayerMappings)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPlayerMapping_PlayerDetails");

            entity.HasOne(d => d.User).WithMany(p => p.UserPlayerMappings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPlayerMapping_UserMaster");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

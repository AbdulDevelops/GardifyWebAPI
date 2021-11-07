namespace GardifyModels.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using GardifyWebAPI.App_Code;

    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("name=DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        



        public virtual DbSet<AlertCondition> AlertCondition { get; set; }
        public virtual DbSet<Alert> Alert { get; set; }
        public virtual DbSet<AlertTrigger> AlertTrigger { get; set; }
        public virtual DbSet<ArticleReference> ArticleReference { get; set; }
        public virtual DbSet<ArticleCategory> ArticleCategories { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<BetaKeys> BetaKey { get; set; }
        public virtual DbSet<BetaMembers> BetaMember { get; set; }
        public virtual DbSet<BlacklistedWord> BlacklistedWords { get; set; }
        public virtual DbSet<BonusHistory> BonusHistories { get; set; }
        public virtual DbSet<DiaryEntry> DiaryEntry { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<AdminDevice> AdminDevices { get; set; }
        public virtual DbSet<EcoElement> EcoElements { get; set; }
        public virtual DbSet<Error> Errors { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<FaqEntry> FaqEntries { get; set; }
        public virtual DbSet<FaqAnswer> FaqAnswers { get; set; }
        public virtual DbSet<FileSystemObject> FileSystemObject { get; set; }
        public virtual DbSet<ForumHeader> ForumHeaders { get; set; }
        public virtual DbSet<ForumPost> ForumPost { get; set; }
        public virtual DbSet<Garden> Gardens { get; set; }
        public virtual DbSet<GardenNote> GardenNotes { get; set; }

        public virtual DbSet<GardenCategory> GardenCategories { get; set; }
        public virtual DbSet<GardenAlbum> GardenAlbums { get; set; }
        public virtual DbSet<GardenAlbumFileToModule> GardenAlbumFileToModules { get; set; }
        public virtual DbSet<GardenPresiFileToModule> GardenPresiFileToModules { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<ImportHistory> ImportHistory { get; set; }
        public virtual DbSet<InternalComment> InternalComment { get; set; }
        public virtual DbSet<FavoriteGardenImage> FavoriteGardenImages { get; set; }
        public virtual DbSet<LastViewed> LastViewed { get; set; }
        public virtual DbSet<LexiconTerm> LexiconTerms { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<NewsEntry> NewsEntry { get; set; }
        public virtual DbSet<InsatPostEntry> InsatPostEntry { get; set; }
        public virtual DbSet<ParentGardenCategory> ParentGardenCategories { get; set; }
        public virtual DbSet<PlantCharacteristicCategory> PlantCharacteristicCategory { get; set; }
        public virtual DbSet<PlantCharacteristic> PlantCharacteristic { get; set; }
        public virtual DbSet<Plant> Plants { get; set; }
        public virtual DbSet<PlantTagCategory> PlantTagCategory { get; set; }
        public virtual DbSet<PlantTagSuperCategory> PlantTagSuperCategory { get; set; }
        public virtual DbSet<PlantTag> PlantTags { get; set; }
        public virtual DbSet<PlantTagCount> PlantTagCounts { get; set; }

        public virtual DbSet<PointsPending> PointsPending { get; set; }
        public virtual DbSet<Property> Property { get; set; }
        public virtual DbSet<ShopOrder> ShopOrders { get; set; }
        public virtual DbSet<PushSubscription> PushSubscriptions { get; set; }
        public virtual DbSet<ReferencesToFileSystemObject> ReferencesToFileSystemObject { get; set; }
        public virtual DbSet<RatingEntry> RatingEntries { get; set; }
        public virtual DbSet<SearchQuery> SearchQueries { get; set; }
        public virtual DbSet<ShopCartEntry> ShopCartEntry { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }
        public virtual DbSet<Subscription> Subscription { get; set; }
        public virtual DbSet<Synonym> Synonym { get; set; }
        public virtual DbSet<StatisticEntry> StatisticEntries { get; set; }
        public virtual DbSet<StatisticEvent> StatisticEvents { get; set; }
        public virtual DbSet<TaxonomicTree> TaxonomicTree { get; set; }
        public virtual DbSet<TempEmail> TempEmails { get; set; }
        public virtual DbSet<Todo> Todoes { get; set; }
        public virtual DbSet<TodoCyclic> TodoCyclic { get; set; }
        public virtual DbSet<TodoTemplate> TodoTemplate { get; set; }
        public virtual DbSet<UserList> UserLists { get; set; }
        public virtual DbSet<UserPlant> UserPlants { get; set; }
        public virtual DbSet<UserPlantCount> UserPlantCounts { get; set; }

        public virtual DbSet<VideoReference> VideoReference { get; set; }
        public virtual DbSet<WatchlistEntry> WatchlistEntry { get; set; }
        public virtual DbSet<ImageScan> ImageScans { get; set; }
        public virtual DbSet<NewsLetter> NewsLetters { get; set; }
        public virtual DbSet<UserDevicesList> UserDevicesLists { get; set; }
        public virtual DbSet<UserEcoElement> UserEcoElements { get; set; }
        public virtual DbSet<UserPlantToUserList> UserPlantToUserLists { get; set; }
        public virtual DbSet<UserSettings> UsersSettings { get; set; }
        public virtual DbSet<UserWarning> UserWarnings { get; set; }
        public virtual DbSet<PlantDoc> PlantDocs { get; set; }
        public virtual DbSet<VideoEntry> VideoEntries { get; set; }
        public virtual DbSet<PlantDocAnswer> PlantDocAnswers { get; set; }
        public virtual DbSet<LastViewedArticle> LastViewedArticles { get; set; }
        public virtual DbSet<TagToImage> TagToImages { get; set; }
        public virtual DbSet<AlbumImageInfo> AlbumImageInfos { get; set; }
        public virtual DbSet<AlbumImageComment> AlbumImageComments { get; set; }

        public virtual DbSet<LaunchPageContent> LaunchPageContents { get; set; }

        public virtual DbSet<CommunityPost> CommunityPosts { get; set; }
        public virtual DbSet<CommunityAnswer> CommunityAnswers { get; set; }


        public virtual DbSet<PlantSearchPropertyItem> PlantSearchPropertyItems { get; set; }

        public virtual DbSet<GardenPresentation> GardenPresentations { get; set; }

        public virtual DbSet<GardenPresentationImage> GardenPresentationImages { get; set; }
        public virtual DbSet<GardenContactShowStatus> GardenContactShowStatuses { get; set; }
        public virtual DbSet<GardenContactList> GardenContactLists { get; set; }

        public virtual DbSet<UserContact> UserContacts { get; set; }
        public virtual DbSet<UserContactMessage> UserContactMessages { get; set; }

        public virtual DbSet<TempTableSearch> TempTableSearches { get; set; }

        public virtual DbSet<PlantSuggestMerge> PlantSuggestMerges { get; set; }
        public virtual DbSet<MaintenanceSetting> MaintenanceSettings { get; set; }
        public virtual DbSet<AppVersion> AppVersions { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        //public virtual DbSet<TaxonomicRelationTodoItemModel> TaxonomieRelationTodo { get; set; }


        public virtual DbSet<City> Cities { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlantSearchPropertyItem>().HasKey(p => p.id);

            modelBuilder.Entity<AlertTrigger>()
                .HasMany(e => e.Conditions)
                .WithRequired(e => e.Trigger)
                .HasForeignKey(e => e.TriggerId);           

            modelBuilder.Entity<Group>()
                .HasMany(e => e.PlantsWithThisGroupd)
                .WithMany(e => e.PlantGroups)
                .Map(m => m.ToTable("GroupPlant").MapRightKey("Plant_Id"));

            modelBuilder.Entity<ParentGardenCategory>()
                .HasMany(e => e.GardenCategories)
                .WithRequired(e => e.ParentCategory)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<PlantCharacteristicCategory>()
                .HasMany(e => e.PlantCharacteristics)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<Plant>()
                .HasMany(e => e.PlantTags)
                .WithMany(e => e.PlantsWithThisTag)
                .Map(m => m.ToTable("PlantTagPlant"));

            modelBuilder.Entity<PlantTagCategory>()
                .HasMany(e => e.Childs)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<PlantTagCategory>()
                .HasMany(e => e.TagsInThisCategory)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<FaqEntry>()
                .Map(m => m.ToTable("FaqEntries", "dbo"));

            modelBuilder.Entity<MaintenanceSetting>().HasKey(m => m.Id);

            //modelBuilder.Entity<TaxonomicRelationTodoItemModel>().HasKey(tr => new { tr.TemplateId, tr.TaxomId });

            modelBuilder.Entity<GardenAlbumFileToModule>()
                .HasKey(ga => new { ga.GardenAlbumId, ga.FileToModuleID });
            modelBuilder.Entity<GardenPresiFileToModule>()
               .HasKey(ga => new { ga.GardenPresiId, ga.FileToModuleID });

            modelBuilder.Entity<FavoriteGardenImage>()
                .HasKey(fa => new { fa.UserId, fa.FileToModuleID });

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<TaxonomicTree>().ToTable("TaxonomicTree", "dbo");
            //modelBuilder.Entity<Plant>().ToTable("Plants", "dbo");
            //modelBuilder.Entity<PlantTagCategory>().ToTable("PlantTagCategories", "dbo");
            //modelBuilder.Entity<PlantTag>().ToTable("PlantTags", "dbo");
            //modelBuilder.Entity<PlantCharacteristicCategory>().ToTable("PlantCharacteristicCategories", "dbo");
            //modelBuilder.Entity<PlantCharacteristic>().ToTable("PlantCharacteristics", "dbo");

        }

        public override int SaveChanges()
        {
            var userName = "System";
            var addedEntities = ChangeTracker.Entries()
                                    .Where(x => x.Entity is _BaseEntity &&
                                    (x.State == EntityState.Added));
            var updatedEntities = ChangeTracker.Entries()
                                    .Where(x => x.Entity is _BaseEntity &&
                                    (x.State == EntityState.Modified));
            //foreach (var entity in addedEntities)
            //{
            //    ((_BaseEntity)entity.Entity).OnCreate(userName);
            //}
            //foreach (var entity in updatedEntities)
            //{
            //    ((_BaseEntity)entity.Entity).OnEdit(userName);
            //}
            return base.SaveChanges();
        }
    }
}

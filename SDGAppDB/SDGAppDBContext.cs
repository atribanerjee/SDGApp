
using SDGAppDB.POCO;
using System;
using System.Collections.Generic;
using System.Data.Entity;


namespace SDGAppDB
{
    public partial class SDGAppDBContext : DbContext
    {
        public SDGAppDBContext(String connectionString)
            : base("name=SDGAppDBContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Database.Connection.ConnectionString = connectionString;

        }


        public DbSet<User> User { get; set; }
        public DbSet<BPDetails> BPDetails { get; set; }
        public DbSet<CityDetail> CityDetail { get; set; }
        public DbSet<Country> Country { get; set; }

        public DbSet<DeviceDetail> DeviceDetail { get; set; }
       
        public DbSet<PlannedActivities> PlannedActivities { get; set; }
        public DbSet<RecognisedActivities> RecognisedActivities { get; set; }
        public DbSet<Role> Role { get; set; }

        public DbSet<StateDetail> StateDetail { get; set; }
        public DbSet<TagCategories> TagCategories { get; set; }
        public DbSet<TagDetails> TagDetails { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<AttributeValueType> AttributeValueType { get; set; }

        public DbSet<UserActivityDetails> UserActivityDetails { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<AttributeType> AttributeType { get; set; }

        public DbSet<AttributeRuleType> AttributeRuleType { get; set; }
        public DbSet<AttributeRuleLabel> AttributeRuleLabel { get; set; }
        public DbSet<AttributeRule> AttributeRule { get; set; }
        public DbSet<POCO.Attribute> Attribute { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<CompanyUserRole> CompanyUserRole { get; set; }
        public DbSet<HelpModule> HelpModule { get; set; }
        public DbSet<UserFeedBack> UserFeedBack { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<HelpContent> HelpContent { get; set; }
        public DbSet<WorkOutActivity> WorkOutActivity { get; set; }
        public DbSet<SleepActivity> SleepActivity { get; set; }
        public DbSet<SleepDataDtls> SleepDataDtls { get; set; }
        public DbSet<SleepType> SleepType { get; set; }
        public DbSet<TagLabel> TagLabel { get; set; }
        public DbSet<TagsMaster> TagsMaster { get; set; }
        public DbSet<UserMessage> UserMessage { get; set; }
        public DbSet<UserMessageType> UserMessageType { get; set; }
        public DbSet<UserContacts> UserContacts { get; set; }
        public DbSet<WidgetStyle> WidgetStyle { get; set; }
       
        public DbSet<LibraryTopic> LibraryTopic { get; set; }
        public DbSet<LibraryWork> LibraryWork { get; set; }
        public DbSet<LibraryWorkDocument> LibraryWorkDocument { get; set; }
        public DbSet<UserMeasurement> UserMeasurement { get; set; }

        public DbSet<TabMaster> TabMaster { get; set; }
        public DbSet<WidgetMaster> WidgetMaster { get; set; }
        public DbSet<UserThirdPartyAPIKey> UserThirdPartyAPIKey { get; set; }

        public DbSet<MessageAttachment> MessageAttachment { get; set; }

        public DbSet<TagLabelType> TagLabelType { get; set; }
        public DbSet<AppType> AppType { get; set; }
        public DbSet<AppDetails> AppDetails { get; set; }
        public DbSet<DeviceInformation> DeviceInformation { get; set; }

        public DbSet<CarePeople> CarePeople { get; set; }

    }
}

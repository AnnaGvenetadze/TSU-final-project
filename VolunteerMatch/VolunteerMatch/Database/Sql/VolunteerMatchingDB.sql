drop table dbo.Users

create database VolunteerMatchDB

create table dbo.Users (
	UserId				UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Email               NVARCHAR(255) NOT NULL,
    PasswordHash        NVARCHAR(255) NOT NULL,
    Role                NVARCHAR(20) NOT NULL,
	LastLoginAt         DATETIMEOFFSET NULL,
    CreatedAt           DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

	CONSTRAINT UQ_Users_Email UNIQUE (Email),
	CONSTRAINT CK_Users_Email_NotBlank CHECK (LEN(LTRIM(RTRIM(Email))) > 0),
	CONSTRAINT PK_Users PRIMARY KEY (UserId),
	CONSTRAINT CK_Users_Role 
		CHECK (LTRIM(RTRIM(Users.Role)) IN (N'მოხალისე', N'ორგანიზაცია')),
	CONSTRAINT CK_Users_PasswordHash_NotBlank
		CHECK (LEN(LTRIM(RTRIM(PasswordHash))) > 0)

);

-- Age must be calculated here and Location - it will be prefiltered with events' compatible columns
CREATE TABLE dbo.VolunteerProfiles (
	VolunteerId     UNIQUEIDENTIFIER NOT NULL,
    FirstName       NVARCHAR(100) NOT NULL,
    LastName        NVARCHAR(100) NOT NULL,
    BirthDate       DATE NOT NULL,
    Citizenship     NVARCHAR(100) NOT NULL,
    Profession      NVARCHAR(200) NOT NULL,
    Languages       NVARCHAR(500) NOT NULL,
    Skills          NVARCHAR(MAX) NOT NULL,
    Interests       NVARCHAR(MAX) NOT NULL,
	Description     NVARCHAR(MAX) NULL,
	LinkedInUrl		NVARCHAR(300) NULL,
    ProfilePhotoUrl NVARCHAR(MAX) NULL,
    Technologies    NVARCHAR(500) NULL,
    Experience      NVARCHAR(MAX) NULL,
	Education		NVARCHAR(500) NULL,
    AverageRating   DECIMAL(3,2)  NULL DEFAULT 0, --

    CONSTRAINT PK_VolunteerProfiles PRIMARY KEY (VolunteerId),
	CONSTRAINT FK_VolunteerProfiles_Users
		FOREIGN KEY (VolunteerId) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.OrganizationProfiles (
	OrganizationId      UNIQUEIDENTIFIER NOT NULL,
    OrganizationName NVARCHAR(200) NOT NULL,
    Description      NVARCHAR(MAX) NOT NULL,
    LinkedInUrl		 NVARCHAR(300) NULL,
	ProfilePhotoUrl NVARCHAR(MAX) NULL, 

    CONSTRAINT PK_OrganizationProfiles PRIMARY KEY (OrganizationId),
    CONSTRAINT FK_OrganizationProfiles_Users
        FOREIGN KEY (OrganizationId) REFERENCES dbo.Users(UserId)
);


CREATE TABLE dbo.Events (
    EventId             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrganizationId      UNIQUEIDENTIFIER NOT NULL,
    Title               NVARCHAR(200) NOT NULL,
    Description         NVARCHAR(2000) NOT NULL,
    Requirements        NVARCHAR(1000) NOT NULL,
    Location            NVARCHAR(100) NOT NULL,
    StartDate           DATETIMEOFFSET NOT NULL,
    EndDate             DATETIMEOFFSET NOT NULL,
    DailyStartTime      TIME NOT NULL,
    DailyEndTime        TIME NOT NULL,
    VolunteersAmount    INT NOT NULL,
    Benefits            NVARCHAR(1000) NOT NULL,
    --SpeakersJsons        NVARCHAR(MAX) NULL, --წაიშალა, მოსაფიქრებელია
    MainPhotoUrl        NVARCHAR(MAX) NULL,
    Photo2Url           NVARCHAR(MAX) NULL,
    Photo3Url           NVARCHAR(MAX) NULL,
    AdditionalInfo      NVARCHAR(1000) NULL,
    IsActive            BIT NOT NULL DEFAULT 1,
    CreatedAt           DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT PK_Events PRIMARY KEY (EventId),

    CONSTRAINT FK_Events_Organization
        FOREIGN KEY (OrganizationId)
        REFERENCES dbo.OrganizationProfiles(OrganizationId),

    CONSTRAINT CHK_Events_EndDate_After_StartDate
        CHECK (EndDate >= StartDate),

    CONSTRAINT CHK_Events_DailyEndTime_After_DailyStartTime
        CHECK (DailyEndTime > DailyStartTime),

    CONSTRAINT CHK_Events_VolunteersAmount_Positive
        CHECK (VolunteersAmount > 0)
);

ALTER TABLE Events
ADD CONSTRAINT DF_Events_IsActive
DEFAULT 1 FOR IsActive;

ALTER TABLE dbo.Events
ADD CONSTRAINT CK_Events_Benefits_NotBlank
CHECK (LEN(LTRIM(RTRIM(Benefits))) > 0);

ALTER TABLE dbo.Events
ADD CONSTRAINT CK_Events_AdditionalInfo_NotBlank
CHECK (AdditionalInfo IS NULL OR LEN(LTRIM(RTRIM(AdditionalInfo))) > 0);

---- Optional: enforce valid JSON if present
--ALTER TABLE dbo.Events
--ADD CONSTRAINT CK_Events_SpeakersJson_IsJson
--CHECK (ISJSON(SpeakersJson) = 1);-- დატესტვისთვის სტრინგად შეცვალე დადროპე ეს შეზღუდვა 

------------------------------------- აქამდე შევქმენი -----------------------------------------
CREATE TABLE dbo.FavoriteEvents (
    VolunteerId UNIQUEIDENTIFIER NOT NULL,
    EventId     UNIQUEIDENTIFIER NOT NULL,
    CreatedAt   DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT PK_FavoriteEvents PRIMARY KEY (VolunteerId, EventId),
    CONSTRAINT FK_FavoriteEvents_Volunteer
        FOREIGN KEY (VolunteerId) REFERENCES dbo.VolunteerProfiles(VolunteerId),
    CONSTRAINT FK_FavoriteEvents_Event
        FOREIGN KEY (EventId) REFERENCES dbo.Events(EventId)
);


----მხოლოდ ერთი კომენტი -------------- სამომავლო პერსპექტივაში ----------------------------
--CREATE TABLE dbo.VolunteerComments (
--    CommentId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
--    VolunteerId          UNIQUEIDENTIFIER NOT NULL,
--    AuthorOrganizationId UNIQUEIDENTIFIER NOT NULL,
--    Text                 NVARCHAR(MAX) NOT NULL,
--    StarRating           TINYINT NOT NULL,
--    CreatedAt            DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
--    UpdatedAt            DATETIMEOFFSET NULL,

--    CONSTRAINT PK_VolunteerComments PRIMARY KEY (CommentId),
--    CONSTRAINT UQ_VolunteerComment UNIQUE (VolunteerId, AuthorOrganizationId),
--    CONSTRAINT FK_VC_Volunteer
--        FOREIGN KEY (VolunteerId) REFERENCES dbo.VolunteerProfiles(VolunteerId),
--    CONSTRAINT FK_VC_AuthorOrg
--        FOREIGN KEY (AuthorOrganizationId) REFERENCES dbo.OrganizationProfiles(OrganizationId)
--);


--CREATE TABLE dbo.OrganizationComments (
--    CommentId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
--    OrganizationId       UNIQUEIDENTIFIER NOT NULL,
--    AuthorVolunteerId UNIQUEIDENTIFIER NOT NULL,
--    Text                 NVARCHAR(MAX) NOT NULL,
--    StarRating           TINYINT NOT NULL,
--    CreatedAt            DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
--    UpdatedAt            DATETIMEOFFSET NULL,

--    CONSTRAINT PK_OrganizationComments PRIMARY KEY (CommentId),
--    CONSTRAINT UQ_OrganizationComment UNIQUE (OrganizationId, AuthorOrganizationId),
--    CONSTRAINT FK_OC_TargetOrg
--        FOREIGN KEY (OrganizationId) REFERENCES dbo.OrganizationProfiles(OrganizationId),
--    CONSTRAINT FK_OC_AuthorVolunteer
--        FOREIGN KEY (AuthorVolunteerId) REFERENCES dbo.VolunteerProfiles(VolunteerId) 
--);

CREATE TABLE dbo.Tags (
    TagId     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name      NVARCHAR(100) NOT NULL,
	-- აქ isActive არაა და თეგის წაშლა რომ არ მოგვიწიოს კარგი სია უნდა მოვიფიქროთ

    CONSTRAINT PK_Tags PRIMARY KEY (TagId),
    CONSTRAINT UQ_Tags_Name UNIQUE (Name)
);


CREATE TABLE dbo.VolunteerTags (
    VolunteerId UNIQUEIDENTIFIER NOT NULL,
    TagId       UNIQUEIDENTIFIER NOT NULL, 

    CONSTRAINT PK_VolunteerTags PRIMARY KEY (VolunteerId, TagId),

    CONSTRAINT FK_VolunteerTags_Volunteer
        FOREIGN KEY (VolunteerId)
        REFERENCES dbo.VolunteerProfiles(VolunteerId),

    CONSTRAINT FK_VolunteerTags_Tag
        FOREIGN KEY (TagId)
        REFERENCES dbo.Tags(TagId)
);

CREATE TABLE dbo.EventTags (
    EventId   UNIQUEIDENTIFIER NOT NULL,
    TagId     UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT PK_EventTags PRIMARY KEY (EventId, TagId),

    CONSTRAINT FK_EventTags_Event
        FOREIGN KEY (EventId)
        REFERENCES dbo.Events(EventId),

    CONSTRAINT FK_EventTags_Tag
        FOREIGN KEY (TagId)
        REFERENCES dbo.Tags(TagId)
); 
---- მვპ-სთვის აღარაა საჭირო ცალკე ცხრილად რადგან ეს ფუნქციონალი ბოლომდეა ახლა 
---- მეჩინგ ფუნქციონალზე დამოკიდებული და ადუბლირებს მის ინფოს
--CREATE TABLE dbo.Notifications (
--    NotificationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
--    UserId         UNIQUEIDENTIFIER NOT NULL,
--    VolunteerEventMatchId UNIQUEIDENTIFIER NOT NULL,
--    Type           TINYINT NOT NULL,
--    Message        NVARCHAR(500) NOT NULL,

--    CreatedAt      DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

--    CONSTRAINT PK_Notifications PRIMARY KEY (NotificationId),

--    CONSTRAINT FK_Notifications_Users
--        FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),

--    CONSTRAINT FK_Notifications_VolunteerEventMatches
--        FOREIGN KEY (VolunteerEventMatchId)
--        REFERENCES dbo.VolunteerEventMatches(VolunteerEventMatchId),

--    CONSTRAINT CK_Notifications_Type CHECK ([Type] IN (0, 1, 2, 3, 4))
--);

--CREATE INDEX IX_Notifications_UserId_CreatedAt
--ON dbo.Notifications(UserId, CreatedAt DESC); 


create table dbo.VolunteerEventMatches
(
    VolunteerEventMatchId uniqueidentifier not null default newsequentialid(),
    VolunteerId uniqueidentifier not null,
    EventId uniqueidentifier not null,

    RequestedByRole nvarchar(30) not null,
    Status tinyint not null,
    CreatedAt datetimeoffset not null default sysdatetimeoffset(),

    constraint PK_VolunteerEventMatches
        primary key (VolunteerEventMatchId),

    constraint FK_VolunteerEventMatches_Volunteers
        foreign key (VolunteerId) references dbo.VolunteerProfiles(VolunteerId),

    constraint FK_VolunteerEventMatches_Events
        foreign key (EventId) references dbo.Events(EventId),

    constraint CK_VolunteerEventMatches_RequestedByRole
        check (RequestedByRole in (N'მოხალისე', N'ორგანიზაცია')),

    constraint CK_VolunteerEventMatches_Status
        check (Status in (0, 1, 2, 3)),

    constraint UQ_VolunteerEventMatches_Volunteer_Event
        unique (VolunteerId, EventId)
); 

CREATE TABLE Skills
(
    SkillId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);
CREATE TABLE Interests
(
    InterestId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.RefreshTokens
(
    RefreshTokenId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    TokenHash NVARCHAR(150) NOT NULL,

    ExpiresAt DATETIME2(0) NOT NULL,
    RevokedAt DATETIME2(0) NULL,
    CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_RefreshTokens_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
);

CREATE UNIQUE INDEX IX_RefreshTokens_TokenHash
ON dbo.RefreshTokens(TokenHash);

CREATE INDEX IX_RefreshTokens_UserId
ON dbo.RefreshTokens(UserId);

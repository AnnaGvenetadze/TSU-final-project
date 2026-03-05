drop table dbo.Users

create database VolunteerMatchDB

create table dbo.Users (
	UserId         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Email               NVARCHAR(255) NOT NULL,
    PasswordHash        NVARCHAR(255) NOT NULL,
    Role                NVARCHAR(20) NOT NULL, --- უნდა თუ აღარ ? ---
	LastLoginAt         DATETIMEOFFSET NULL,
    CreatedAt           DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

	CONSTRAINT UQ_Volunteers_Email UNIQUE (Email),
	CONSTRAINT PK_Users PRIMARY KEY (UserId)
);

-- Age must be calculated here and Location - it will be prefiltered with events' compatible columns
CREATE TABLE dbo.Volunteers (
	VolunteerId         UNIQUEIDENTIFIER NOT NULL,
    FirstName       NVARCHAR(100) NOT NULL,
    LastName        NVARCHAR(100) NOT NULL,
    BirthDate       DATE NOT NULL,
    Citizenship     NVARCHAR(100) NOT NULL,
    Profession      NVARCHAR(200) NOT NULL,
    Languages       NVARCHAR(500) NOT NULL,
    Skills          NVARCHAR(MAX) NOT NULL,
    Interests       NVARCHAR(MAX) NOT NULL,
    ProfilePhotoUrl NVARCHAR(500) NULL,
    Technologies    NVARCHAR(500) NULL,
    Experience      NVARCHAR(MAX) NULL,
    Location        NVARCHAR(200) NULL,
    PreferredDays   NVARCHAR(200) NULL,
    --AverageRating   DECIMAL(3,2) NOT NULL DEFAULT 0,

    CONSTRAINT PK_VolunteerProfiles PRIMARY KEY (VolunteerId),
	CONSTRAINT FK_VolunteerProfiles_Users
		FOREIGN KEY (VolunteerId) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.Organizations (
	OrganizationId      UNIQUEIDENTIFIER NOT NULL,
    OrganizationName NVARCHAR(200) NOT NULL,
    Description      NVARCHAR(MAX) NOT NULL,
    PhoneNumber      NVARCHAR(50) NULL,
    LinkedInUrl		 NVARCHAR(300) NULL,
    --AverageRating    DECIMAL(3,2) NOT NULL DEFAULT 0,

    CONSTRAINT PK_OrganizationProfiles PRIMARY KEY (OrganizationId),
    CONSTRAINT FK_OrganizationProfiles_Users
        FOREIGN KEY (OrganizationId) REFERENCES dbo.Users(UserId)
);


CREATE TABLE dbo.Events (
    EventId         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrganizationId  UNIQUEIDENTIFIER NOT NULL,
    Title           NVARCHAR(200) NOT NULL,
    Description     NVARCHAR(MAX) NOT NULL,
    Requirements    NVARCHAR(MAX) NOT NULL,
    AgeMin          TINYINT NULL,
    AgeMax          TINYINT NULL,
    Location		NVARCHAR(100) NOT NULL,
    StartDate		DATETIMEOFFSET NOT NULL,
    EndDate			DATETIMEOFFSET NOT NULL,
    Status          NVARCHAR(20) NOT NULL,
    IsActive        BIT NOT NULL DEFAULT 1, -- Organization deletes event
    CreatedAt       DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT PK_Events PRIMARY KEY (EventId),
    CONSTRAINT FK_Events_Organization
        FOREIGN KEY (OrganizationId)
        REFERENCES dbo.OrganizationProfiles(OrganizationId)
);


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


CREATE TABLE dbo.MatchingSuggestions (
    SuggestionId         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    VolunteerId          UNIQUEIDENTIFIER NOT NULL,
    EventId              UNIQUEIDENTIFIER NOT NULL,
    Initiator            NVARCHAR(20) NOT NULL,
    MatchScore           TINYINT NOT NULL,
    VolunteerApproved    BIT NULL,
    OrganizationApproved BIT NULL,
    Status               NVARCHAR(30) NOT NULL,
    CreatedAt            DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    ExpiresAt            DATETIMEOFFSET NULL,

    CONSTRAINT PK_MatchingSuggestions PRIMARY KEY (SuggestionId),
    CONSTRAINT FK_MS_Volunteer
        FOREIGN KEY (VolunteerId) REFERENCES dbo.VolunteerProfiles(VolunteerId),
    CONSTRAINT FK_MS_Event
        FOREIGN KEY (EventId) REFERENCES dbo.Events(EventId)
);

----მხოლოდ ერთი კომენტი
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
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT PK_Tags PRIMARY KEY (TagId),
    CONSTRAINT UQ_Tags_Name UNIQUE (Name)
);


CREATE TABLE dbo.VolunteerTags (
    VolunteerId UNIQUEIDENTIFIER NOT NULL,
    TagId       UNIQUEIDENTIFIER NOT NULL,
    CreatedAt   DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

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
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT PK_EventTags PRIMARY KEY (EventId, TagId),

    CONSTRAINT FK_EventTags_Event
        FOREIGN KEY (EventId)
        REFERENCES dbo.Events(EventId),

    CONSTRAINT FK_EventTags_Tag
        FOREIGN KEY (TagId)
        REFERENCES dbo.Tags(TagId)
);

----------------------- აქამდე შევქმენი თეიბლები ----------------------------------
CREATE TABLE dbo.Permissions (
    PermissionId   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PermissionName NVARCHAR(100) NOT NULL,
    CreatedAt      DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT PK_Permissions PRIMARY KEY (PermissionId),
    CONSTRAINT UQ_Permissions_Name UNIQUE (PermissionName)
);


CREATE TABLE dbo.UserPermissions (
    UserPermissionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId           UNIQUEIDENTIFIER NOT NULL,
    PermissionId     UNIQUEIDENTIFIER NOT NULL,
    CreatedAt        DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT PK_UserPermissions PRIMARY KEY (UserPermissionId),
    CONSTRAINT UQ_UserPermissions UNIQUE (UserId, PermissionId),
    CONSTRAINT FK_UP_User
        FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_UP_Permission
        FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(PermissionId)
);
-------------------------- ახალი დამატებული --------------------------
ALTER TABLE dbo.Events
ADD SpeakersJson    NVARCHAR(MAX) NOT NULL,
	Benefits        NVARCHAR(MAX) NOT NULL,
    AdditionalInfo  NVARCHAR(MAX) NULL;

-- Optional: enforce valid JSON if present
ALTER TABLE dbo.Events
ADD CONSTRAINT CK_Events_SpeakersJson_IsJson
CHECK (ISJSON(SpeakersJson) = 1);-- დატესტვისთვის სტრინგად შეცვალე დადროპე ეს შეზღუდვა 

ALTER TABLE dbo.Events
ADD CONSTRAINT CK_Events_Benefits_NotBlank
CHECK (LEN(LTRIM(RTRIM(Benefits))) > 0);

ALTER TABLE dbo.Events
ADD CONSTRAINT CK_Events_AdditionalInfo_NotBlank
CHECK (AdditionalInfo IS NULL OR LEN(LTRIM(RTRIM(AdditionalInfo))) > 0);
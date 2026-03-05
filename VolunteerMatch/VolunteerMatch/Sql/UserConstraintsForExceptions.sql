select * from Users
select * from UserPermissions

delete from Users
where UserId = '40D84052-861B-4D6E-AE1C-D95F884F26E3'
-- Role allowed values
ALTER TABLE dbo.Users
ADD CONSTRAINT CK_Users_Role
CHECK (
    LTRIM(RTRIM(Users.Role)) IN (N'მოხალისე', N'ორგანიზაცია')
);

-- Email not blank
ALTER TABLE dbo.Users
ADD CONSTRAINT CK_Users_Email_NotBlank
CHECK (LEN(LTRIM(RTRIM(Email))) > 0);

-- PasswordHash not blank
ALTER TABLE dbo.Users
ADD CONSTRAINT CK_Users_PasswordHash_NotBlank
CHECK (LEN(LTRIM(RTRIM(PasswordHash))) > 0);

-- Optional but very useful: normalized email uniqueness
ALTER TABLE dbo.Users
ADD NormalizedEmail AS LOWER(LTRIM(RTRIM(Email))) PERSISTED;

CREATE UNIQUE INDEX UX_Users_NormalizedEmail
ON dbo.Users(NormalizedEmail);
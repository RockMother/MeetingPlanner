
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 02/15/2014 11:41:01
-- Generated from EDMX file: C:\Projects\MeetingPlanner\DataAccess\MeetingPlanner.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Meeting];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MeetingMeetingStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MeetingSet] DROP CONSTRAINT [FK_MeetingMeetingStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_MeetingMembersMeeting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MeetingMembersSet] DROP CONSTRAINT [FK_MeetingMembersMeeting];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMeetingDatesMeeting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMeetingDatesSet] DROP CONSTRAINT [FK_UserMeetingDatesMeeting];
GO
IF OBJECT_ID(N'[dbo].[FK_webpages_UsersInRoles_webpages_Roles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[webpages_UsersInRoles] DROP CONSTRAINT [FK_webpages_UsersInRoles_webpages_Roles];
GO
IF OBJECT_ID(N'[dbo].[FK_webpages_UsersInRoles_UserProfile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[webpages_UsersInRoles] DROP CONSTRAINT [FK_webpages_UsersInRoles_UserProfile];
GO
IF OBJECT_ID(N'[dbo].[FK_MeetingUserProfile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MeetingSet] DROP CONSTRAINT [FK_MeetingUserProfile];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMeetingDatesUserProfile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMeetingDatesSet] DROP CONSTRAINT [FK_UserMeetingDatesUserProfile];
GO
IF OBJECT_ID(N'[dbo].[FK_MeetingMembersUserProfile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MeetingMembersSet] DROP CONSTRAINT [FK_MeetingMembersUserProfile];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[MeetingSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MeetingSet];
GO
IF OBJECT_ID(N'[dbo].[MeetingStatusSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MeetingStatusSet];
GO
IF OBJECT_ID(N'[dbo].[MeetingMembersSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MeetingMembersSet];
GO
IF OBJECT_ID(N'[dbo].[UserMeetingDatesSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserMeetingDatesSet];
GO
IF OBJECT_ID(N'[dbo].[UserProfile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserProfile];
GO
IF OBJECT_ID(N'[dbo].[webpages_Membership]', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_Membership];
GO
IF OBJECT_ID(N'[dbo].[webpages_OAuthMembership]', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_OAuthMembership];
GO
IF OBJECT_ID(N'[dbo].[webpages_Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_Roles];
GO
IF OBJECT_ID(N'[dbo].[webpages_UsersInRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[webpages_UsersInRoles];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MeetingSet'
CREATE TABLE [dbo].[MeetingSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MeetingStatusId] int  NOT NULL,
    [UserProfileId] int  NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'MeetingStatusSet'
CREATE TABLE [dbo].[MeetingStatusSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'MeetingMembersSet'
CREATE TABLE [dbo].[MeetingMembersSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MeetingId] int  NOT NULL,
    [UserProfileId] int  NOT NULL
);
GO

-- Creating table 'UserMeetingDatesSet'
CREATE TABLE [dbo].[UserMeetingDatesSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [MeetingId] int  NOT NULL,
    [IsAvaliable] bit  NOT NULL,
    [UserProfileId] int  NULL
);
GO

-- Creating table 'UserProfile'
CREATE TABLE [dbo].[UserProfile] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(56)  NOT NULL
);
GO

-- Creating table 'webpages_Membership'
CREATE TABLE [dbo].[webpages_Membership] (
    [UserId] int  NOT NULL,
    [CreateDate] datetime  NULL,
    [ConfirmationToken] nvarchar(128)  NULL,
    [IsConfirmed] bit  NULL,
    [LastPasswordFailureDate] datetime  NULL,
    [PasswordFailuresSinceLastSuccess] int  NOT NULL,
    [Password] nvarchar(128)  NOT NULL,
    [PasswordChangedDate] datetime  NULL,
    [PasswordSalt] nvarchar(128)  NOT NULL,
    [PasswordVerificationToken] nvarchar(128)  NULL,
    [PasswordVerificationTokenExpirationDate] datetime  NULL
);
GO

-- Creating table 'webpages_OAuthMembership'
CREATE TABLE [dbo].[webpages_OAuthMembership] (
    [Provider] nvarchar(30)  NOT NULL,
    [ProviderUserId] nvarchar(100)  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'webpages_Roles'
CREATE TABLE [dbo].[webpages_Roles] (
    [RoleId] int IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'webpages_UsersInRoles'
CREATE TABLE [dbo].[webpages_UsersInRoles] (
    [webpages_Roles_RoleId] int  NOT NULL,
    [UserProfiles_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'MeetingSet'
ALTER TABLE [dbo].[MeetingSet]
ADD CONSTRAINT [PK_MeetingSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MeetingStatusSet'
ALTER TABLE [dbo].[MeetingStatusSet]
ADD CONSTRAINT [PK_MeetingStatusSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MeetingMembersSet'
ALTER TABLE [dbo].[MeetingMembersSet]
ADD CONSTRAINT [PK_MeetingMembersSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserMeetingDatesSet'
ALTER TABLE [dbo].[UserMeetingDatesSet]
ADD CONSTRAINT [PK_UserMeetingDatesSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserProfile'
ALTER TABLE [dbo].[UserProfile]
ADD CONSTRAINT [PK_UserProfile]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [UserId] in table 'webpages_Membership'
ALTER TABLE [dbo].[webpages_Membership]
ADD CONSTRAINT [PK_webpages_Membership]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [Provider], [ProviderUserId] in table 'webpages_OAuthMembership'
ALTER TABLE [dbo].[webpages_OAuthMembership]
ADD CONSTRAINT [PK_webpages_OAuthMembership]
    PRIMARY KEY CLUSTERED ([Provider], [ProviderUserId] ASC);
GO

-- Creating primary key on [RoleId] in table 'webpages_Roles'
ALTER TABLE [dbo].[webpages_Roles]
ADD CONSTRAINT [PK_webpages_Roles]
    PRIMARY KEY CLUSTERED ([RoleId] ASC);
GO

-- Creating primary key on [webpages_Roles_RoleId], [UserProfiles_Id] in table 'webpages_UsersInRoles'
ALTER TABLE [dbo].[webpages_UsersInRoles]
ADD CONSTRAINT [PK_webpages_UsersInRoles]
    PRIMARY KEY NONCLUSTERED ([webpages_Roles_RoleId], [UserProfiles_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MeetingStatusId] in table 'MeetingSet'
ALTER TABLE [dbo].[MeetingSet]
ADD CONSTRAINT [FK_MeetingMeetingStatus]
    FOREIGN KEY ([MeetingStatusId])
    REFERENCES [dbo].[MeetingStatusSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MeetingMeetingStatus'
CREATE INDEX [IX_FK_MeetingMeetingStatus]
ON [dbo].[MeetingSet]
    ([MeetingStatusId]);
GO

-- Creating foreign key on [MeetingId] in table 'MeetingMembersSet'
ALTER TABLE [dbo].[MeetingMembersSet]
ADD CONSTRAINT [FK_MeetingMembersMeeting]
    FOREIGN KEY ([MeetingId])
    REFERENCES [dbo].[MeetingSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MeetingMembersMeeting'
CREATE INDEX [IX_FK_MeetingMembersMeeting]
ON [dbo].[MeetingMembersSet]
    ([MeetingId]);
GO

-- Creating foreign key on [MeetingId] in table 'UserMeetingDatesSet'
ALTER TABLE [dbo].[UserMeetingDatesSet]
ADD CONSTRAINT [FK_UserMeetingDatesMeeting]
    FOREIGN KEY ([MeetingId])
    REFERENCES [dbo].[MeetingSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMeetingDatesMeeting'
CREATE INDEX [IX_FK_UserMeetingDatesMeeting]
ON [dbo].[UserMeetingDatesSet]
    ([MeetingId]);
GO

-- Creating foreign key on [webpages_Roles_RoleId] in table 'webpages_UsersInRoles'
ALTER TABLE [dbo].[webpages_UsersInRoles]
ADD CONSTRAINT [FK_webpages_UsersInRoles_webpages_Roles]
    FOREIGN KEY ([webpages_Roles_RoleId])
    REFERENCES [dbo].[webpages_Roles]
        ([RoleId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [UserProfiles_Id] in table 'webpages_UsersInRoles'
ALTER TABLE [dbo].[webpages_UsersInRoles]
ADD CONSTRAINT [FK_webpages_UsersInRoles_UserProfile]
    FOREIGN KEY ([UserProfiles_Id])
    REFERENCES [dbo].[UserProfile]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_webpages_UsersInRoles_UserProfile'
CREATE INDEX [IX_FK_webpages_UsersInRoles_UserProfile]
ON [dbo].[webpages_UsersInRoles]
    ([UserProfiles_Id]);
GO

-- Creating foreign key on [UserProfileId] in table 'MeetingMembersSet'
ALTER TABLE [dbo].[MeetingMembersSet]
ADD CONSTRAINT [FK_MeetingMembersUserProfile]
    FOREIGN KEY ([UserProfileId])
    REFERENCES [dbo].[UserProfile]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MeetingMembersUserProfile'
CREATE INDEX [IX_FK_MeetingMembersUserProfile]
ON [dbo].[MeetingMembersSet]
    ([UserProfileId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
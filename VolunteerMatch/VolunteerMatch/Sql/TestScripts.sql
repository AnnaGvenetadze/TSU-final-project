select * from Users
select * from OrganizationProfiles
select * from VolunteerProfiles

alter table OrganizationProfiles
drop column AverageRating -- ჩაწერილების ამოშლას მთხოვს

alter table Events
drop column SpeakersJson

alter table VolunteerProfiles -- უკვე დაემატა
add Education NVARCHAR(500) NULL

alter table OrganizationProfiles
add ProfilePhotoUrl NVARCHAR(500) NULL

---------- ჩანაწერის (ექაუნთის) წაშლა
delete from OrganizationProfiles
where OrganizationId = 'EE24C50C-A218-F111-8432-C0B5D79C0C94'

delete from Users
where UserId = 'EE24C50C-A218-F111-8432-C0B5D79C0C94'
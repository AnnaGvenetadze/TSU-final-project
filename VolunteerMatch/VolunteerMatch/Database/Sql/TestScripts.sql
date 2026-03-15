select * from Users
select * from OrganizationProfiles
select * from Events

select * from VolunteerProfiles
order by FirstName

update Events
set IsActive = 1
where EventId = '6ac72c80-eb1e-f111-8433-c0b5d79c0c94'

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
where UserId = 'EE24C50C-A218-F111-8432-C0B5D79C0C9'
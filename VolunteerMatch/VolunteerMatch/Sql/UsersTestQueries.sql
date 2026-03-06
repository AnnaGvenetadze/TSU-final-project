select * from Users
select * from OrganizationProfiles
select * from VolunteerProfiles

alter table OrganizationProfiles
drop column AverageRating -- ჩაწერილების ამოშლას მთხოვს

---------- ჩანაწერის (ექაუნთის) წაშლა
delete from OrganizationProfiles
where OrganizationId = 'EE24C50C-A218-F111-8432-C0B5D79C0C94'

delete from Users
where UserId = 'EE24C50C-A218-F111-8432-C0B5D79C0C94'
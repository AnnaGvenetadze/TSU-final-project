INSERT INTO Skills (Name)
VALUES
(N'კომუნიკაცია'),
(N'ლიდერობა'),
(N'გუნდურობა'),
(N'ორგანიზება'),
(N'ლოგისტიკა'),
(N'კოორდინაცია'),
(N'მენტორობა'),
(N'კონსულტირება'),
(N'სწავლება'),
(N'თარგმნა'),
(N'წერა'),
(N'რედაქტირება'),
(N'პრეზენტაცია'),
(N'მოლაპარაკება'),
(N'კვლევა'),
(N'ანალიზი'),
(N'მონაცემები'),
(N'დიზაინი'),
(N'ფოტოგრაფია'),
(N'ვიდეოგადაღება'),
(N'კონტენტი'),
(N'სოციალური მედია'),
(N'ტექნიკური მხარდაჭერა'),
(N'კომპიუტერული უნარები'),
(N'პირველადი დახმარება'),
(N'ბავშვებთან მუშაობა'),
(N'ახალგაზრდებთან მუშაობა'),
(N'ხანდაზმულებთან მუშაობა'),
(N'ინკლუზიური მუშაობა'),
(N'ადმინისტრირება'),
(N'ფონდების მოძიება'),
(N'საზოგადოებასთან ურთიერთობა');
INSERT INTO Interests (Name)
VALUES
(N'ბავშვების სწავლება'),
(N'მოსწავლეების დახმარება'),
(N'სტუდენტური მენტორობა'),
(N'ენების პრაქტიკა'),
(N'წიგნიერება'),
(N'კარიერული კონსულტაცია'),
(N'პროფესიული ორიენტაცია'),
(N'ახალგაზრდული ბანაკები'),
(N'ლიდერობის აქტივობები'),

(N'დასუფთავების აქციები'),
(N'ხეების დარგვა'),
(N'ნარჩენების დახარისხება'),
(N'ეკოაქტივობები'),
(N'გარემოსდაცვითი ცნობიერება'),

(N'კონცერტები'),
(N'გამოფენები'),
(N'თეატრი'),
(N'ფესტივალები'),
(N'შემოქმედებითი ვორქშოფები'),

(N'საკვების დარიგება'),
(N'ტანსაცმლის შეგროვება'),
(N'ჰუმანიტარული პაკეტები'),
(N'ოჯახების დახმარება'),
(N'კრიზისული დახმარება'),

(N'ხანდაზმულთა მხარდაჭერა'),
(N'შშმ მხარდაჭერა'),
(N'ბავშვთა მხარდაჭერა'),
(N'სოციალური მხარდაჭერა'),
(N'ინკლუზიური აქტივობები'),

(N'სპორტული ღონისძიებები'),
(N'საბავშვო სპორტი'),
(N'ჯანსაღი ცხოვრება'),
(N'გუნდური თამაშები'),

(N'ტექნოლოგიური ვორქშოფები'),
(N'ციფრული უნარები'),
(N'კვლევითი პროექტები'),
(N'მონაცემების შეგროვება'),
(N'IT მხარდაჭერა'),

(N'რეგისტრაციის მხარდაჭერა'),
(N'სტუმრების მიღება'),
(N'სივრცის მომზადება'),
(N'ღონისძიების ლოგისტიკა'),

(N'ცხოველთა მოვლა'),
(N'თავშესაფრების მხარდაჭერა'),
(N'გაშვილების კამპანიები'),
(N'ცხოველთა ცნობიერება'),

(N'ჯანმრთელობის ცნობიერება'),
(N'სისხლის დონაცია'),
(N'პირველადი დახმარება'),
(N'ფსიქოსოციალური მხარდაჭერა'),

(N'სამეზობლო ინიციატივები'),
(N'საჯარო სივრცეები'),
(N'საზოგადოებრივი კამპანიები');

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

------------------ მოხალისე მარიამ გიგიშვილს შევუსაბამე -------------------
INSERT INTO VolunteerInterests (VolunteerId, InterestId)
SELECT
    'EA5C777A-0722-F111-8435-C0B5D79C0C94',
    v.InterestId
FROM
(
    VALUES
    ('171FB3BF-0DE5-4E11-8D88-04993B8AF575'),
    ('401D1D6A-4678-4EE2-BCF4-969D8F0014D7'),
    ('F0C458B6-1F90-4612-8D01-7F8623C5D6A1'),
    ('9C915C8C-E179-40C3-A9E6-38F46DBEB707')
) AS v(InterestId)
WHERE NOT EXISTS
(
    SELECT 1
    FROM VolunteerInterests vi
    WHERE vi.VolunteerId = 'EA5C777A-0722-F111-8435-C0B5D79C0C94'
      AND vi.InterestId = v.InterestId
);

INSERT INTO VolunteerSkills (VolunteerId, SkillId)
SELECT
    'EA5C777A-0722-F111-8435-C0B5D79C0C94',
    v.SkillId
FROM
(
    VALUES
    ('0D5B88EA-1521-465F-BDB7-930C7AEB48E1'),
    ('063D2563-0A22-481B-B2DC-ABC6BC9A3595'),
    ('9153204C-01CC-4498-B43F-063E30B9AC7E'),
    ('4A926363-2C26-4B2E-A84D-FE1F3120902A'),
    ('F936BF44-FA28-4E49-81CE-511416532F9E')
) AS v(SkillId)
WHERE NOT EXISTS
(
    SELECT 1
    FROM VolunteerSkills vs
    WHERE vs.VolunteerId = 'EA5C777A-0722-F111-8435-C0B5D79C0C94'
      AND vs.SkillId = v.SkillId
);

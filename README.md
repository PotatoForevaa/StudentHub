# StudentHub
**Work In Progress**  
project is under development!!!

## Deployment
git clone https://github.com/PotatoForevaa/StudentHub  
sudo docker build -t student_hub StudentHub  
sudo docker run -d -p 5000:5000 -e AdminUser__Username=AdminUsername -e  AdminUser__Password=AdminPassword -e AdminUser__FullName=AdminFullName -e ConnectionStrings__PgSql="Host=postgres;Port=5432;Database=db;User=dbuser;Passwor
d=DbPassword" student_hub  

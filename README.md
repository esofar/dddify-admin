Dddify Admin
======
[development phase]

A out-of-box develop solution for enterprise applications built on top of the Dddify & Ant Design Pro.

Create the database:

    cd src\Dddify.Admin.Infrastructure
    dotnet ef migrations add InitialCreate
    dotnet ef database update

Launch the webapi:

    cd src\Dddify.Admin.Host
    dotnet watch run
	
Launch the clientapp:

	cd src\Dddify.Admin.Host\ClientApp
	npm install
	npm run start
	
	

# MonoMigrations
MonoMigrations is to provide some migrations feature based on Entity Framework 6.
Entity Framework 6 Migrations work with PowerShell. However, in Linux, PowerShell is not available.

## Requirement
- EntityFramework
- MySql.Data.Entity.EF6 (depend on your project)

## Usage
Create a console project. Add the MonoMigrations.dll as the reference to the target project. 
Then in the source code,

`using MonoMigrations;`

Write the implementation of the migrations, for example

```csharp
internal sealed class MyMigrationConfiguration : DbMigrationsConfiguration<FooBarContext> {
  public MyMigrationConfiguration()  {
  	AutomaticMigrationsEnabled = true; //Have to set to true, due to the limitation
  	AutomaticMigrationDataLossAllowed = false; //Set to true if allow data loss
  	
  	//Example using MySQL, this is required if want to use scaffolding (generate the *.cs files)
  	SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
  }
  
  protected override void Seed(OrderBoxContext context) { }
}
```

Then in the Main(),

```csharp
public static void Main (string[] args) {
  var config = new MyMigrationConfiguration ();
  new MigrationsMain<FooBarContext> (config, args);
}
```

Build the console application.

Then, to use the created console,

`./myMigration.exe -a yourMigrationName`

This is based on Add-Migration. 3 files will be created in the working directory,

- XXXXXXXXXXXXXXX_yourMigrationName.cs
- XXXXXXXXXXXXXXX_yourMigrationName.Designer.cs
- XXXXXXXXXXXXXXX_yourMigrationName.resx

where the XXX... are the digits.

Then, to update the database,

`./myMigration.exe -u`

This will only work if `AutomaticMigrationsEnabled = true`.

## Limitations
The current stage can only update with automatic migrations. As a result, the database will contain 
the migration IDs as XXXXXXXXXXXXXXX_AutomaticMigration, and cannot use the migrations that is 
created by "-a" option.

Similarly, the DbMigrator methods GetPendingMigrations() and GetLocalMigrations() do not work, 
except GetDatabaseMigrations().

Lastly, I have tried to use Entity Framework SQLite, but failed to work.

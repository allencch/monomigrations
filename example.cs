using System;
using FooBar; //Contains FooBarContext
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Infrastructure; //DbConnectionInfo
using MonoMigrations;

namespace MigrationExample {
	class MainClass	{
		internal sealed class MyMigrationConfiguration : DbMigrationsConfiguration<FooBarContext> { //The target model's DbContext
			public MyMigrationConfiguration() {
				AutomaticMigrationsEnabled = true;
				AutomaticMigrationDataLossAllowed = false;
				
				SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator()); //This is required if scaffolding
			}

			protected override void Seed(FooBarContext context) { }
		}

		public static void Main (string[] args) {
			var config = new MyMigrationConfiguration ();
			new MigrationsMain<FooBarContext> (config, args);
		}
	}
}

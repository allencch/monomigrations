using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Infrastructure; //MigratorScriptingDecorator
using System.IO;
using System.Resources; //Requires System.Windows.Forms assembly

namespace MonoMigrations
{
	/**
	 * This object should be instantiated with target DbContext. This should be used together 
	 * with other programs wchich contain the DbMigrationsConfiguration.
	 */
	public class MonoMigrations<TContext> where TContext : DbContext
	{
		private DbMigrationsConfiguration<TContext> config;

		public MonoMigrations(DbMigrationsConfiguration<TContext> config) {
			this.config = config;
		}

		//Based on https://stackoverflow.com/questions/20374783/enable-entity-framework-migrations-in-mono
		/**
		 * @param config is the DbMigrationsConfiguration with the DbContext
		 * @param name is the migration name
		 * @return the migration ID
		 */
		public string MigrationsAdd(string name) {
			var scaffolder = new MigrationScaffolder(config);
			var migration = scaffolder.Scaffold(name);

			File.WriteAllText(migration.MigrationId + ".cs", migration.UserCode);
			File.WriteAllText(migration.MigrationId + ".Designer.cs", migration.DesignerCode);
			using (var writer = new ResXResourceWriter(migration.MigrationId + ".resx")) {
				foreach (var resource in migration.Resources) {
					writer.AddResource(resource.Key, resource.Value);
				}
			}

			return migration.MigrationId;
		}

		//Based on http://romiller.com/2012/02/09/running-scripting-migrations-from-code/
		/**
		 * @param config is the DbMigrationsConfiguration with the DbContext
		 */
		public void DatabaseUpdate() {
			var migrator = new DbMigrator (config);
			migrator.Update ();
		}

		/**
		 * This is based on the "dnx ef database" 
		 * @param name, if "0", then revert all database
		 */
		public void DatabaseUpdate(string name) {
			var migrator = new DbMigrator (config);
			migrator.Update (name);
		}

		/**
		 * Fail to list the migrations. Stupid
		 */
		public void MigrationsList() {
			var migrator = new DbMigrator (config);

			//var list = migrator.GetDatabaseMigrations ();
			//var list = migrator.GetPendingMigrations ();
			var list = migrator.GetLocalMigrations();
			foreach(var item in list) {
				Console.WriteLine (item.ToString());
			}
		}


		//Need to add the source (beginning) and target (latest)
		// Does not work
		public void MigrationsScript() {
			var migrator = new DbMigrator (config);
			var scriptor = new MigratorScriptingDecorator (migrator);
			var script = scriptor.ScriptUpdate (sourceMigration: null, targetMigration: null);
			Console.WriteLine (script);
		}
	}

	public class MigrationsMain<TContext> where TContext : DbContext {
		public MigrationsMain(DbMigrationsConfiguration<TContext> config, string[] args) {
			var monoMigration = new MonoMigrations<TContext>(config);
			if (args.Length == 0) {
				Console.WriteLine ("This command should run with parameters");
				return;
			}
			if (args [0] == "-a") {
				Console.WriteLine ("Migrations add");
				string name = args.Length >= 2 ? args [1] : "default";
				string migrationId = monoMigration.MigrationsAdd (name);
				Console.WriteLine (migrationId + ".* are created");
			} else if (args [0] == "-u") {
				if (args.Length >= 2) {
					monoMigration.DatabaseUpdate (args [1]);
				} else {
					monoMigration.DatabaseUpdate ();
				}
				Console.WriteLine ("Database updated");
			} else if (args [0] == "-s") {
				monoMigration.MigrationsScript ();
			} else if (args [0] == "-l") {
				monoMigration.MigrationsList ();
			}
		}


	}



}


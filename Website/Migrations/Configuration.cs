namespace SkeptiForum.Archive.Migrations {

  using System;
  using System.Data.Entity;
  using System.Data.Entity.Migrations;
  using System.Linq;
  using SkeptiForum.Archive.Reporting.Providers;

  internal sealed class Configuration : DbMigrationsConfiguration<ReportingContext> {
    public Configuration() {
      AutomaticMigrationsEnabled = true;
    }

    protected override void Seed(ReportingContext context) {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data. E.g.
      //
      //    context.People.AddOrUpdate(
      //      p => p.FullName,
      //      new Person { FullName = "Andrew Peters" },
      //      new Person { FullName = "Brice Lambson" },
      //      new Person { FullName = "Rowan Miller" }
      //    );
      //
    }
  }
}

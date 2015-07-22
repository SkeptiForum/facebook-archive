/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy.Caney@Ignia.com)
| Client        SkeptiForum
| Project       Reporting
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Data.Entity;
using SkeptiForum.Archive.Reporting;

namespace SkeptiForum.Archive.Reporting.Providers {

  /*============================================================================================================================
  | CLASS: REPORTING CONTEXT
  \---------------------------------------------------------------------------------------------------------------------------*/
  public class ReportingContext : DbContext {

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    public ReportingContext() : base("Reporting") {
    }

    /*==========================================================================================================================
    | METHOD: CREATE (FACTORY)
    \-------------------------------------------------------------------------------------------------------------------------*/
    public static ReportingContext Create() {
      return new ReportingContext();
    }

    /*==========================================================================================================================
    | PROPERTY: INDEX (ENTITY SET)
    \-------------------------------------------------------------------------------------------------------------------------*/
    public DbSet<Activity> ActivityLog { get; set; }

    }
  }

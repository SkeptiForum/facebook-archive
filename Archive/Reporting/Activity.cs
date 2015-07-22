using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeptiForum.Archive.Reporting {

  [Table("ActivityLog")]
  public class Activity {

    public Activity() {}

    [Key]
    [Column("ActivityId")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id {
      get;
      set;
    }

    public long GroupId {
      get;
      set;
    }

    public long PostId {
      get;
      set;
    }

    public long UserId {
      get;
      set;
    }

    public ObjectType Type {
      get;
      set;
    }

    public int LikeCount {
      get;
      set;
    }

    public DateTime DateCreated {
      get;
      set;
    }

  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SkeptiForum.Archive.Models {

  public class SitemapGroupViewModel {

    public SitemapGroupViewModel(FacebookGroup group, IEnumerable<FileInfo> posts) {
      Group = group;
      Posts = posts;
    }

    public FacebookGroup Group { get; set; }

    public IEnumerable<FileInfo> Posts { get; set; }

  }

}
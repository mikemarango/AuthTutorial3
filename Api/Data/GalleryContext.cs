using Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data
{
  public class GalleryContext : DbContext
  {
    public GalleryContext(DbContextOptions<GalleryContext> options) : base(options)
    {
    }

    public DbSet<Image> Images { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }
  }
}

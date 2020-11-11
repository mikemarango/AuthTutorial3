using Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.Data
{
  public class IdentityContext : DbContext
  {
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>(options =>
      {
        options.HasIndex(u => u.Subject).IsUnique();
        options.HasIndex(u => u.UserName).IsUnique();
        options.HasData(new User[]
        {
          new User()
          {
            Id = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Password = "password",
            Subject = "d860efca-22d9-47fd-8249-791ba61b07c7",
            UserName = "Frank",
            IsActive = true
          },
          new User()
          {
            Id = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
            Password = "password",
            Subject = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
            UserName = "Claire",
            IsActive = true
          }
        });
      });

      modelBuilder.Entity<UserClaim>(builder =>
      {
        builder.HasData(new UserClaim[]
        {
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Type = "given_name",
            Value = "Frank"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Type = "family_name",
            Value = "Underwood"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Type = "email",
            Value = "frank@email.com"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Type = "address",
            Value = "Main Road 1"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Type = "subscriptionlevel",
            Value = "FreeUser"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Type = "country",
            Value = "nl"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
            Type = "given_name",
            Value = "Claire"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
            Type = "family_name",
            Value = "Underwood"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
            Type = "email",
            Value = "claire@email.com"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
            Type = "address",
            Value = "Big Street 2"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
            Type = "subscriptionlevel",
            Value = "PayingUser"
          },
          new UserClaim()
          {
            Id = Guid.NewGuid(),
            UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
            Type = "country",
            Value = "be"
          }
        });
      });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      var concurrencyAwareEntityEntries = ChangeTracker.Entries()
        .Where(entry => entry.State.Equals(EntityState.Modified))
        .OfType<IConcurrencyAware>();

      foreach (var entry in concurrencyAwareEntityEntries)
      {
        entry.ConcurrencyStamp = Guid.NewGuid().ToString();
      }

      return base.SaveChangesAsync(cancellationToken);
    }
  }
}

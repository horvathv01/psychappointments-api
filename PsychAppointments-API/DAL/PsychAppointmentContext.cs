using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.DAL;

public class PsychAppointmentContext : DbContext
{
    public DbSet<Psychologist> Psychologists { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Slot> Slots { get; set; }
    
    public DbSet<Address> Addresses { get; set; }

    public PsychAppointmentContext(DbContextOptions<PsychAppointmentContext> contextOptions) : base(contextOptions)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>()
            .HasKey(a => a.Id);
        
        modelBuilder.Entity<Address>()
            .Property(address => address.Id)
            .UseIdentityColumn(); 
        
        modelBuilder.Entity<User>()
            .HasOne(us => us.RegisteredBy)
            .WithMany()
            .HasForeignKey(us => us.RegisteredById);

        modelBuilder.Entity<User>()
            .HasOne(us => us.Address)
            .WithMany()
            .HasForeignKey(us => us.AddressId);
        
        modelBuilder.Entity<User>()
            .Property(user => user.Id)
            .UseIdentityColumn(); 
        
        modelBuilder.Entity<Psychologist>()
            .HasMany(psy => psy.Clients)
            .WithMany(cli => cli.Psychologists)
            .UsingEntity(join => join.ToTable("PsychologistClients"));

        modelBuilder.Entity<Psychologist>()
            .HasMany(psy => psy.Sessions)
            .WithOne(ses => ses.Psychologist)
            .HasForeignKey(ses => ses.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Psychologist>()
            .HasMany(psy => psy.Slots)
            .WithOne(slot => slot.Psychologist)
            .HasForeignKey(slot => slot.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Psychologist>()
            .Property(psy => psy.Id)
            .UseIdentityColumn(); 

        modelBuilder.Entity<Client>()
            .HasMany<Psychologist>(cli => cli.Psychologists)
            .WithMany(psy => psy.Clients)
            .UsingEntity(join => join.ToTable("PsychologistClients")); //same as psychologist-client relations

        modelBuilder.Entity<Client>()
            .HasMany<Session>(cli => cli.Sessions)
            .WithOne(ses => ses.Client)
            .HasForeignKey(ses => ses.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Client>()
            .Property(cli => cli.Id)
            .UseIdentityColumn(); 

        modelBuilder.Entity<Location>()
            .HasMany(loc => loc.Psychologists)
            .WithMany()
            .UsingEntity(join => join.ToTable("LocationPsychologists"));
        
        modelBuilder.Entity<Location>()
            .HasMany(loc => loc.Managers)
            .WithMany(man => man.Locations)
            .UsingEntity(join => join.ToTable("LocationManagers"));

        modelBuilder.Entity<Location>()
            .HasOne(loc => loc.Address)
            .WithMany()
            .HasForeignKey(loc => loc.AddressId);
        
        modelBuilder.Entity<Location>()
            .Property(loc => loc.Id)
            .UseIdentityColumn(); 
        
        modelBuilder.Entity<Manager>()
            .HasMany(man => man.Locations)
            .WithMany(loc => loc.Managers)
            .UsingEntity(join => join.ToTable("LocationManagers")); //same as location-manager relations
        
        modelBuilder.Entity<Manager>()
            .Property(man => man.Id)
            .UseIdentityColumn(); 

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Location)
            .WithMany()
            .HasForeignKey(ses => ses.LocationId);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Psychologist)
            .WithMany(psy => psy.Sessions)
            .HasForeignKey(ses => ses.PsychologistId);


        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.PartnerPsychologist)
            .WithMany()
            .HasForeignKey(ses => ses.PartnerPsychologistId);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Client)
            .WithMany(cli => cli.Sessions)
            .HasForeignKey(ses => ses.ClientId)
            .IsRequired(false);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Slot)
            .WithMany(slot => slot.Sessions)
            .HasForeignKey(ses => ses.SlotId);
        
        modelBuilder.Entity<Session>()
            .Property(ses => ses.Id)
            .UseIdentityColumn(); 
        
        modelBuilder.Entity<Slot>()
            .HasOne(slot => slot.Psychologist)
            .WithMany(psy => psy.Slots)
            .HasForeignKey(slot => slot.PsychologistId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Slot>()
            .HasOne(slot => slot.Location)
            .WithMany()
            .HasForeignKey(slot => slot.LocationId);
        
        modelBuilder.Entity<Slot>()
            .HasMany(slot => slot.Sessions)
            .WithOne(ses => ses.Slot)
            .HasForeignKey(slot => slot.SlotId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Slot>()
            .Property(slot => slot.Id)
            .UseIdentityColumn(); 
        
    }
}
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

    protected PsychAppointmentContext(DbContextOptions<PsychAppointmentContext> contextOptions) : base(contextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Psychologist>()
            .HasMany(psy => psy.Clients)
            .WithMany(cli => cli.Psychologists)
            .UsingEntity(join => join.ToTable("PsychologistClients"));

        modelBuilder.Entity<Psychologist>()
            .HasMany(psy => psy.Sessions)
            .WithOne(ses => ses.Psychologist)
            .HasForeignKey(ses => ses.Id);

        modelBuilder.Entity<Psychologist>()
            .HasMany(psy => psy.Slots)
            .WithOne(slot => slot.Psychologist)
            .HasForeignKey(slot => slot.Id);

        modelBuilder.Entity<Client>()
            .HasMany<Psychologist>(cli => cli.Psychologists)
            .WithMany(psy => psy.Clients)
            .UsingEntity(join => join.ToTable("PsychologistClients")); //same as psychologist-client relations

        modelBuilder.Entity<Client>()
            .HasMany<Session>(cli => cli.Sessions)
            .WithOne(ses => ses.Client)
            .HasForeignKey(ses => ses.Id);

        modelBuilder.Entity<Location>()
            .HasMany(loc => loc.Psychologists)
            .WithMany()
            .UsingEntity(join => join.ToTable("LocationPsychologists"));
        
        modelBuilder.Entity<Location>()
            .HasMany(loc => loc.Managers)
            .WithMany(man => man.Locations)
            .UsingEntity(join => join.ToTable("LocationManagers"));
        
        modelBuilder.Entity<Manager>()
            .HasMany(man => man.Locations)
            .WithMany(loc => loc.Managers)
            .UsingEntity(join => join.ToTable("LocationManagers")); //same as location-manager relations

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Location)
            .WithMany()
            .HasForeignKey(ses => ses.Location.Id);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Psychologist)
            .WithMany(psy => psy.Sessions)
            .HasForeignKey(ses => ses.Psychologist.Id);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.PartnerPsychologist)
            .WithMany(psy => psy.Sessions)
            .HasForeignKey(ses => ses.PartnerPsychologist.Id);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Location)
            .WithMany()
            .HasForeignKey(ses => ses.Location.Id);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Client)
            .WithMany(cli => cli.Sessions)
            .HasForeignKey(ses => ses.Client.Id);

        modelBuilder.Entity<Session>()
            .HasOne(ses => ses.Slot)
            .WithMany(slot => slot.Sessions)
            .HasForeignKey(ses => ses.Slot.Id);
        
        modelBuilder.Entity<Slot>()
            .HasOne(slot => slot.Psychologist)
            .WithMany(psy => psy.Slots)
            .HasForeignKey(slot => slot.Psychologist.Id);

        modelBuilder.Entity<Slot>()
            .HasOne(slot => slot.Location)
            .WithMany()
            .HasForeignKey(slot => slot.Location.Id);
        
        modelBuilder.Entity<Slot>()
            .HasMany(slot => slot.Sessions)
            .WithOne(ses => ses.Slot)
            .HasForeignKey(slot => slot.Location.Id);

    }
}
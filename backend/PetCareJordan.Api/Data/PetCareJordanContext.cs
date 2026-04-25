using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Data;

public class PetCareJordanContext(DbContextOptions<PetCareJordanContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<AdoptionListing> AdoptionListings => Set<AdoptionListing>();
    public DbSet<LostPetReport> LostPetReports => Set<LostPetReport>();
    public DbSet<FoundPetReport> FoundPetReports => Set<FoundPetReport>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<VaccinationRecord> VaccinationRecords => Set<VaccinationRecord>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ChatConversation> ChatConversations => Set<ChatConversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Pet>()
            .HasIndex(pet => pet.CollarId)
            .IsUnique();

        modelBuilder.Entity<Pet>()
            .Property(pet => pet.WeightKg)
            .HasPrecision(8, 2);

        modelBuilder.Entity<LostPetReport>()
            .Property(report => report.RewardAmount)
            .HasPrecision(8, 2);

        modelBuilder.Entity<LostPetReport>()
            .HasOne(report => report.Reporter)
            .WithMany()
            .HasForeignKey(report => report.ReporterId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<FoundPetReport>()
            .HasOne(report => report.Reporter)
            .WithMany()
            .HasForeignKey(report => report.ReporterId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Pet>()
            .HasOne(pet => pet.Owner)
            .WithMany(owner => owner.OwnedPets)
            .HasForeignKey(pet => pet.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdoptionListing>()
            .HasOne(listing => listing.Pet)
            .WithOne(pet => pet.AdoptionListing)
            .HasForeignKey<AdoptionListing>(listing => listing.PetId);

        modelBuilder.Entity<MedicalRecord>()
            .HasOne(record => record.Pet)
            .WithMany(pet => pet.MedicalRecords)
            .HasForeignKey(record => record.PetId);

        modelBuilder.Entity<MedicalRecord>()
            .HasOne(record => record.Vet)
            .WithMany(vet => vet.MedicalRecordsAuthored)
            .HasForeignKey(record => record.VetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VaccinationRecord>()
            .HasOne(record => record.Pet)
            .WithMany(pet => pet.Vaccinations)
            .HasForeignKey(record => record.PetId);

        modelBuilder.Entity<VaccinationRecord>()
            .HasOne(record => record.Vet)
            .WithMany(vet => vet.VaccinationsAuthored)
            .HasForeignKey(record => record.VetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(notification => notification.User)
            .WithMany()
            .HasForeignKey(notification => notification.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatConversation>()
            .HasIndex(conversation => new { conversation.UserId, conversation.VetId })
            .IsUnique();

        modelBuilder.Entity<ChatConversation>()
            .HasOne(conversation => conversation.User)
            .WithMany(user => user.UserConversations)
            .HasForeignKey(conversation => conversation.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatConversation>()
            .HasOne(conversation => conversation.Vet)
            .WithMany(user => user.VetConversations)
            .HasForeignKey(conversation => conversation.VetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(message => message.Conversation)
            .WithMany(conversation => conversation.Messages)
            .HasForeignKey(message => message.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(message => message.Sender)
            .WithMany(user => user.ChatMessagesSent)
            .HasForeignKey(message => message.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

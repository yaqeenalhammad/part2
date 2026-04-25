namespace PetCareJordan.Api.Models;

public enum UserRole
{
    User = 1,
    Vet = 2,
    Admin = 3
}

public enum PetType
{
    Cat = 1,
    Dog = 2,
    Bird = 3,
    Rabbit = 4,
    Other = 5
}

public enum PetGender
{
    Male = 1,
    Female = 2
}

public enum AdoptionStatus
{
    Available = 1,
    Pending = 2,
    Adopted = 3
}

public enum ReportStatus
{
    Active = 1,
    Resolved = 2,
    Pending = 3,
    Rejected = 4
}

public enum NotificationType
{
    VaccineReminder = 1,
    General = 2
}

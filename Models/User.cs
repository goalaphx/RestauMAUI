namespace Restaurant.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; } // Store hashes, not plain text!
    public string FullName { get; set; }
    public UserRole Role { get; set; } // Client, Staff
}

public enum UserRole { Client, Staff }
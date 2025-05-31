using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using PasswordManagerBackend.Models; // Reference your Models namespace

namespace PasswordManagerBackend.Services
{
    public class PasswordService
    {
        private const string DataFilePath = "passwords.json"; // Local file to store data
        private List<PasswordEntry> _entries;

        // These should match the frontend's hashing parameters
        private const int PBKDF2_ITERATIONS = 100000;
        private const int SALT_LENGTH_BYTES = 16;
        private const int HASH_LENGTH_BYTES = 32;

        public PasswordService()
        {
            _entries = LoadEntries();
        }

        public List<PasswordEntry> GetAllEntries()
        {
            return _entries;
        }

        // This method is kept for completeness, though for this app, frontend sends pre-hashed data.
        // In a real app, backend would receive plain password and hash it.
        public (string hashedPassword, byte[] salt) HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SALT_LENGTH_BYTES);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, PBKDF2_ITERATIONS, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HASH_LENGTH_BYTES);
            return (Convert.ToBase64String(hash), salt);
        }

        private List<PasswordEntry> LoadEntries()
        {
            if (!File.Exists(DataFilePath))
            {
                return new List<PasswordEntry>();
            }

            try
            {
                string jsonString = File.ReadAllText(DataFilePath);
                // Use JsonSerializerOptions to handle case-insensitive property matching
                return JsonSerializer.Deserialize<List<PasswordEntry>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PasswordEntry>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading password entries: {ex.Message}");
                return new List<PasswordEntry>();
            }
        }

        private void SaveEntries()
        {
            try
            {
                // Use JsonSerializerOptions for pretty printing the JSON file
                string jsonString = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(DataFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving password entries: {ex.Message}");
            }
        }

        public PasswordEntry AddEntry(PasswordEntry entry)
        {
            // Assign a new ID if it's missing (frontend should generate one, but as a fallback)
            if (string.IsNullOrEmpty(entry.Id))
            {
                entry.Id = Guid.NewGuid().ToString();
            }
            _entries.Add(entry);
            SaveEntries();
            return entry; // Return the added entry (with its ID)
        }

        public bool DeleteEntry(string id)
        {
            var entryToRemove = _entries.FirstOrDefault(e => e.Id == id);
            if (entryToRemove != null)
            {
                _entries.Remove(entryToRemove);
                SaveEntries();
                return true;
            }
            return false;
        }

        public void ClearAllEntries()
        {
            _entries.Clear();
            SaveEntries();
        }
    }
}
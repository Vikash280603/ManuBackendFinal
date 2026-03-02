// ============================================================
// AUTH REPOSITORY INTERFACE
// ============================================================
//
// This file defines a CONTRACT for authentication-related
// database operations.
//
// An INTERFACE:
// - Contains ONLY method definitions
// - Contains NO implementation (no logic, no code body)
//
// Purpose:
// ✔ Enforces consistency
// ✔ Supports clean architecture
// ✔ Makes unit testing easy
// ✔ Allows database implementation to change
//
// Architecture Flow:
// Controller → Service → Repository → Database
//
// Service layer depends on this interface,
// NOT on concrete database code.
// ============================================================



// ============================================================
// USING STATEMENTS
// ============================================================

// Gives access to User entity (database model)
using ManuBackend.Models;



// ============================================================
// NAMESPACE
// ============================================================

namespace ManuBackend.Repository
{
    // =========================================================
    // IAuthRepository INTERFACE
    // =========================================================

    // The "I" prefix is a C# naming convention for interfaces
    //
    // Any class that IMPLEMENTS this interface
    // MUST provide implementations for ALL methods below
    public interface IAuthRepository
    {
        // =====================================================
        // GET USER BY EMAIL
        // =====================================================

        // Purpose:
        // - Find a user using email address
        // - Used during LOGIN
        //
        // async + Task → database operation (I/O bound)
        //
        // User? → nullable return type
        // Means:
        // - Returns User object if found
        // - Returns null if not found
        Task<User?> GetUserByEmailAsync(string email);



        // =====================================================
        // CREATE NEW USER
        // =====================================================

        // Purpose:
        // - Save a new user record to database
        // - Used during SIGNUP
        //
        // Returns:
        // - The saved User object
        //   (includes generated Id from database)
        Task<User> CreateUserAsync(User user);



        // =====================================================
        // CHECK IF EMAIL EXISTS
        // =====================================================

        // Purpose:
        // - Check email uniqueness
        // - Prevent duplicate registrations
        //
        // Returns:
        // - true  → email already exists
        // - false → email is available
        Task<bool> EmailExistsAsync(string email);
    }
}
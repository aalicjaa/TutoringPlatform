using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Domain.Offers;
using TutoringPlatform.Domain.Subjects;
using TutoringPlatform.Domain.Tutors;
using TutoringPlatform.Infrastructure.Identity;

namespace TutoringPlatform.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        var subjectNames = new[]
        {
            "Matematyka",
            "Fizyka",
            "Chemia",
            "Język angielski",
            "Informatyka",
            "Biologia",
            "Język polski",
            "Geografia",
            "Historia"
        };

        var existingSubjects = await db.Subjects.AsNoTracking().ToListAsync();
        var missingSubjects = subjectNames
            .Where(n => existingSubjects.All(s => s.Name != n))
            .Select(n => new Subject { Name = n })
            .ToList();

        if (missingSubjects.Count > 0)
        {
            db.Subjects.AddRange(missingSubjects);
            await db.SaveChangesAsync();
        }

        var subjectsDb = await db.Subjects.AsNoTracking().ToListAsync();
        int S(string name) => subjectsDb.Single(x => x.Name == name).Id;

        var tutorSeeds = new List<(string Email, string DisplayName, string Bio, string City, decimal HourlyRate)>
        {
            ("anna.nowak@demo.pl", "Anna Nowak", "Przygotowanie do matury, zajęcia od podstaw.", "Kraków", 80),
            ("mateusz.kowal@demo.pl", "Mateusz Kowal", "Matematyka i fizyka, zadania, arkusze.", "Warszawa", 110),
            ("katarzyna.zielinska@demo.pl", "Katarzyna Zielińska", "Angielski konwersacje, gramatyka, egzamin ósmoklasisty.", "Gdańsk", 90),
            ("piotr.wisniewski@demo.pl", "Piotr Wiśniewski", "Informatyka, programowanie C#/JavaScript, projekty.", "Wrocław", 130),
            ("magdalena.krawczyk@demo.pl", "Magdalena Krawczyk", "Chemia i biologia, powtórki, mapy myśli.", "Poznań", 95),
            ("tomasz.lewandowski@demo.pl", "Tomasz Lewandowski", "Język polski, rozprawki, lektury, matura.", "Łódź", 85),
            ("oliwia.dabrowska@demo.pl", "Oliwia Dąbrowska", "Matematyka podstawowa i rozszerzona, regularna praca.", "Katowice", 100),
            ("jakub.nowicki@demo.pl", "Jakub Nowicki", "Fizyka – zrozumienie, nie wkuwanie. Zadania maturalne.", "Kraków", 120),
            ("natalia.maj@demo.pl", "Natalia Maj", "Angielski od A1 do C1, konwersacje, przygotowanie do egzaminów.", "Kraków", 105),
            ("kamil.baran@demo.pl", "Kamil Baran", "Matematyka – regularne zajęcia, arkusze maturalne.", "Wrocław", 115),
            ("zuzanna.pawlak@demo.pl", "Zuzanna Pawlak", "Biologia i chemia – matura rozszerzona, powtórki tematyczne.", "Gdańsk", 110),
            ("michal.krol@demo.pl", "Michał Król", "Informatyka – algorytmy, C#, przygotowanie do studiów.", "Warszawa", 140),
            ("alicja.szymanska@demo.pl", "Alicja Szymańska", "Język polski – wypracowania, analiza tekstu.", "Poznań", 90),
            ("szymon.wojcik@demo.pl", "Szymon Wójcik", "Geografia i historia – liceum, matury.", "Łódź", 95)
        };

        var existingEmails = await db.Users.AsNoTracking().Select(u => u.Email!).ToListAsync();
        var hasher = new PasswordHasher<AppUser>();

        var usersToAdd = new List<AppUser>();

        foreach (var t in tutorSeeds)
        {
            var email = t.Email.ToLowerInvariant();
            if (existingEmails.Contains(email))
                continue;

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                UserName = email,
                NormalizedEmail = email.ToUpperInvariant(),
                NormalizedUserName = email.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            user.PasswordHash = hasher.HashPassword(user, "Password1!");

            usersToAdd.Add(user);
        }

        if (usersToAdd.Count > 0)
        {
            db.Users.AddRange(usersToAdd);
            await db.SaveChangesAsync();
        }

        var userIdByEmail = await db.Users.AsNoTracking()
            .Where(u => u.Email != null && u.Email.EndsWith("@demo.pl"))
            .ToDictionaryAsync(u => u.Email!.ToLower(), u => u.Id);

        var existingProfilesByEmail = await db.TutorProfiles.AsNoTracking()
            .Join(db.Users.AsNoTracking(),
                p => p.UserId,
                u => u.Id,
                (p, u) => new { p.Id, Email = u.Email! })
            .ToDictionaryAsync(x => x.Email.ToLower(), x => x.Id);

        var profilesToAdd = new List<TutorProfile>();

        foreach (var t in tutorSeeds)
        {
            var email = t.Email.ToLowerInvariant();
            if (!userIdByEmail.TryGetValue(email, out var userId))
                continue;

            if (existingProfilesByEmail.ContainsKey(email))
                continue;

            profilesToAdd.Add(new TutorProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DisplayName = t.DisplayName,
                Bio = t.Bio,
                City = t.City,
                HourlyRate = t.HourlyRate
            });
        }

        if (profilesToAdd.Count > 0)
        {
            db.TutorProfiles.AddRange(profilesToAdd);
            await db.SaveChangesAsync();
        }
        var tutorProfileIdByName = await db.TutorProfiles.AsNoTracking()
            .GroupBy(t => t.DisplayName)
            .Select(g => new { DisplayName = g.Key, Id = g.OrderBy(x => x.Id).Select(x => x.Id).First() })
            .ToDictionaryAsync(x => x.DisplayName, x => x.Id);


        var offerSeed = new List<LessonOffer>
        {
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Anna Nowak"], SubjectId = S("Matematyka"), DurationMinutes = 60, Price = 80, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Anna Nowak"], SubjectId = S("Matematyka"), DurationMinutes = 60, Price = 90, Mode = "Onsite" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Mateusz Kowal"], SubjectId = S("Matematyka"), DurationMinutes = 60, Price = 110, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Mateusz Kowal"], SubjectId = S("Fizyka"), DurationMinutes = 60, Price = 120, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Katarzyna Zielińska"], SubjectId = S("Język angielski"), DurationMinutes = 60, Price = 90, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Katarzyna Zielińska"], SubjectId = S("Język angielski"), DurationMinutes = 45, Price = 75, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Piotr Wiśniewski"], SubjectId = S("Informatyka"), DurationMinutes = 60, Price = 130, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Piotr Wiśniewski"], SubjectId = S("Informatyka"), DurationMinutes = 90, Price = 180, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Magdalena Krawczyk"], SubjectId = S("Chemia"), DurationMinutes = 60, Price = 95, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Magdalena Krawczyk"], SubjectId = S("Biologia"), DurationMinutes = 60, Price = 95, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Tomasz Lewandowski"], SubjectId = S("Język polski"), DurationMinutes = 60, Price = 85, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Oliwia Dąbrowska"], SubjectId = S("Matematyka"), DurationMinutes = 60, Price = 100, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Oliwia Dąbrowska"], SubjectId = S("Matematyka"), DurationMinutes = 90, Price = 140, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Jakub Nowicki"], SubjectId = S("Fizyka"), DurationMinutes = 60, Price = 115, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Natalia Maj"], SubjectId = S("Język angielski"), DurationMinutes = 60, Price = 105, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Kamil Baran"], SubjectId = S("Matematyka"), DurationMinutes = 60, Price = 115, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Zuzanna Pawlak"], SubjectId = S("Biologia"), DurationMinutes = 60, Price = 110, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Zuzanna Pawlak"], SubjectId = S("Chemia"), DurationMinutes = 60, Price = 110, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Michał Król"], SubjectId = S("Informatyka"), DurationMinutes = 60, Price = 140, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Alicja Szymańska"], SubjectId = S("Język polski"), DurationMinutes = 60, Price = 90, Mode = "Online" },

            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Szymon Wójcik"], SubjectId = S("Geografia"), DurationMinutes = 60, Price = 95, Mode = "Online" },
            new() { Id = Guid.NewGuid(), TutorProfileId = tutorProfileIdByName["Szymon Wójcik"], SubjectId = S("Historia"), DurationMinutes = 60, Price = 95, Mode = "Online" }
        };

        var existingOfferKeys = await db.LessonOffers.AsNoTracking()
            .Select(o => new { o.TutorProfileId, o.SubjectId, o.DurationMinutes, o.Price, o.Mode })
            .ToListAsync();

        bool Exists(LessonOffer o) =>
            existingOfferKeys.Any(x =>
                x.TutorProfileId == o.TutorProfileId &&
                x.SubjectId == o.SubjectId &&
                x.DurationMinutes == o.DurationMinutes &&
                x.Price == o.Price &&
                x.Mode == o.Mode);

        var offersToAdd = offerSeed.Where(o => !Exists(o)).ToList();

        if (offersToAdd.Count > 0)
        {
            db.LessonOffers.AddRange(offersToAdd);
            await db.SaveChangesAsync();
        }
    }
}

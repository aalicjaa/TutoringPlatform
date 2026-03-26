using System;

namespace TutoringPlatform.Mobile.Models;

public sealed class TutorListItem
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = "";

    public string City { get; set; } = "";

    public string SubjectLine { get; set; } = "";

    public string Bio { get; set; } = "";

    public decimal? PriceFrom { get; set; }

    public string Description => Bio;

    public string SubjectCity
    {
        get
        {
            var s = (SubjectLine ?? "").Trim();
            var c = (City ?? "").Trim();

            if (string.IsNullOrWhiteSpace(s)) return c;
            if (string.IsNullOrWhiteSpace(c)) return s;
            return $"{s} • {c}";
        }
    }

    public string PriceText => PriceFrom is null
        ? "Brak ceny"
        : $"Od {PriceFrom:0} zł";

    public string Initials
    {
        get
        {
            var name = (FullName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return "?";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";

            if (parts.Length == 1)
                return char.ToUpper(parts[0][0]).ToString();

            return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
        }
    }
}
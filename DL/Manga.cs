using System;
using System.Collections.Generic;

namespace DL;

public partial class Manga
{
    public string Id { get; set; } = null!;

    public string Gid { get; set; } = null!;

    public string? Type { get; set; }

    public string? Name { get; set; }

    public string? Precision { get; set; }

    public string? Vintage { get; set; }
}

using System;
using System.Collections.Generic;

namespace backPreinscription.Models;

public partial class Bac
{
    public int IdBac { get; set; }

    public int AnneeBacc { get; set; }

    public string NumBacc { get; set; } = null!;

    public string? DocBac { get; set; }

    public bool? EstMalagasy { get; set; }

    public virtual ICollection<Preinscription> Preinscriptions { get; set; } = new List<Preinscription>();
}

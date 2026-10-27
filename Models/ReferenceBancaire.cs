namespace backPreinscription.Models;

/// <summary>
/// Référence d'un reçu bancaire rattachée à un dossier.
///
/// Un candidat qui règle ses frais de préinscription en plusieurs fois possède
/// un reçu par versement : le dossier peut donc en porter plusieurs. La première
/// reste également recopiée dans <c>preinscription.ref_bancaire</c>, que lisent
/// les traitements historiques.
///
/// La table impose qu'une référence appartienne à exactement un dossier
/// (inscription ou préinscription), et qu'elle soit unique : c'est ce qui
/// empêche qu'un même reçu serve à deux candidats.
/// </summary>
public partial class ReferenceBancaire
{
    public int IdRb { get; set; }

    public int? IdInscription { get; set; }

    public string Reference { get; set; } = null!;

    public DateTime? DateInsertion { get; set; }

    public int? IdPreinscription { get; set; }
}

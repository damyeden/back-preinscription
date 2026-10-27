using Microsoft.AspNetCore.Mvc;
using backPreinscription.Models;
using backPreinscription.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace backPreinscription.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        /// <summary>
        /// Longueur maximale d'une référence (colonne reference_bancaire.reference
        /// est un varchar(20) ; on refuse au-delà plutôt que de laisser la base
        /// tronquer ou rejeter en erreur 500).
        /// </summary>
        private const int ReferenceMaxLength = 20;

        private readonly PreinscriptionDbContext _context;

        public RegisterController(PreinscriptionDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Forme canonique d'une référence : sans espaces autour, en majuscules.
        /// Un candidat qui saisit « yma7730/cot  » désigne le même reçu que
        /// « YMA7730/COT » : sans cette normalisation, le contrôle de doublon
        /// laisserait passer deux fois la même preuve de paiement.
        /// </summary>
        private static string NormalizeReference(string? reference)
            => (reference ?? string.Empty).Trim().ToUpperInvariant();

        /// <summary>
        /// Identifiant de l'année universitaire en cours
        /// (annees_universitaires.est_active). Toute la logique annuelle
        /// (rattachement des dossiers, unicité par portail) s'appuie dessus :
        /// changer d'année universitaire se fait donc en base, sans redéploiement.
        /// </summary>
        private int GetCurrentAnneeId()
        {
            return _context.Database
                .SqlQueryRaw<int>(
                    "SELECT id_annee AS \"Value\" FROM annees_universitaires WHERE est_active = true ORDER BY id_annee DESC LIMIT 1")
                .AsEnumerable()
                .FirstOrDefault();
        }

        private bool IsPreinscriptionClosed()
        {
            var currentYear = DateTime.Now.Year;
            var deadline = new DateTime(currentYear, 12, 15, 23, 59, 59);
            return DateTime.Now > deadline;
        }

        [HttpGet("portail/getallportails")]
        public IActionResult GetAllPortails(){
            var portails = _context.Portails.ToList();
            var dataReturn = new List<portailResponse>();
            portails.ForEach(p => dataReturn.Add(new portailResponse
            {
                IdPortail = p.IdPortail,
                Abbreviation = p.Abbreviation,
                NomPortail = p.NomPortail
            }));
            return Ok(dataReturn);
        }

        /// <summary>
        /// Portails déjà préinscrits par un candidat pour l'année universitaire
        /// en cours : le front s'en sert pour les marquer et les désactiver.
        /// </summary>
        [HttpGet("{numBacc}/{anneeBacc}/portails-inscrits")]
        public IActionResult GetPortailsInscrits(string numBacc, int anneeBacc)
        {
            var anneeId = GetCurrentAnneeId();
            if (anneeId == 0) return Ok(Array.Empty<object>());

            var idBac = _context.Bacs
                .Where(b => b.NumBacc == numBacc && b.AnneeBacc == anneeBacc)
                .Select(b => (int?)b.IdBac)
                .FirstOrDefault();

            if (idBac == null) return Ok(Array.Empty<object>());

            var inscrits = _context.Preinscriptions
                .Where(p => p.IdBac == idBac && p.IdAnnee == anneeId && p.IdPortail != null)
                .Select(p => new
                {
                    p.IdPortail,
                    p.IdPreinscription,
                    p.DatePreinscription
                })
                .ToList();

            return Ok(inscrits);
        }

        [HttpGet("portail/{series}/{type}")]
        public IActionResult GetPortails(string series,bool type)
        {

            var idSeries = _context.Series
                .Where(s => s.NomSerie == series)
                .Select(s => s.IdSerie).ToList();
            var idPortails = _context.PortailSeries
                .Where(ps => idSeries.Contains(ps.IdSerie))
                .Select(ps => ps.IdPortail).ToList();
            var portails = _context.Portails
                .Where(m => idPortails.Contains(m.IdPortail) && m.EstAcademique == type)
                .ToList();    
            if (portails == null) return NotFound();
            var dataReturn = new List<portailResponse>();
            portails.ForEach(p => dataReturn.Add(new portailResponse
            {
                IdPortail = p.IdPortail,
                Abbreviation = p.Abbreviation,
                NomPortail = p.NomPortail
            }));
            return Ok(dataReturn);
        }
        [HttpGet("portail/getportailbytype/{type}")]
        public IActionResult GetPortailByType(bool type){
            var portails = _context.Portails
                .Where(m => m.EstAcademique == type)
                .ToList();    
            if (portails == null) return NotFound();
            var dataReturn = new List<portailResponse>();
            portails.ForEach(p => dataReturn.Add(new portailResponse
            {
                IdPortail = p.IdPortail,
                Abbreviation = p.Abbreviation,
                NomPortail = p.NomPortail
            }));
            return Ok(dataReturn);
        }

        [HttpGet("status")]
        public IActionResult GetPreinscriptionStatus()
        {
            var status = new PreinscriptionStatus
            {
                IsClosed = IsPreinscriptionClosed()
            };

            return Ok(status);
        }

        [HttpPost("RegisterTry")]
        public async Task<ActionResult<Preinscription>> RegisterBachelier([FromBody] ApplicationData data)
        {
            Console.WriteLine("Data reçue : " + System.Text.Json.JsonSerializer.Serialize(data));
            
            if (IsPreinscriptionClosed())
            {
                return Problem(
                    title: "Inscriptions fermées",
                    detail: "Les inscriptions sont terminées. La date limite était le 15 novembre à 23h59.",
                    statusCode: StatusCodes.Status403Forbidden,
                    type: "https://example.com/problems/preinscription-closed"
                );
            }

            if (data == null)
            {
                return Problem(
                    title: "Invalid request",
                    detail: "Le corps de la requête est vide ou invalide.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/invalid-request"
                );
            }

            // Récupérations initiales (conserver la logique)
            var candidateInfo = data.CandidateInfo;
            var bankInfo = data.BankInfo;
            var selectedProgram = data.SelectedProgram;

            // Vérifier que les objets attendus existent (renvoit Problem si manquants)
            if (candidateInfo == null || bankInfo == null || selectedProgram == null || data.PersonalInfo == null)
            {
                return Problem(
                    title: "Champs requis manquants",
                    detail: "candidateInfo, bankInfo, selectedProgram et personalInfo sont requis.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/missing-fields"
                );
            }

            // Références bancaires : la principale d'abord, puis les reçus
            // complémentaires. On applique la même normalisation partout
            // (contrôle de doublon comme écriture), sinon une différence de
            // casse ou une espace de saisie laisserait passer un reçu en double.
            var references = new List<string>();
            foreach (var candidate in (bankInfo.References ?? new List<string>())
                         .Prepend(bankInfo.Reference))
            {
                var normalized = NormalizeReference(candidate);
                if (normalized.Length == 0 || references.Contains(normalized))
                    continue;
                references.Add(normalized);
            }

            if (references.Count == 0)
            {
                return Problem(
                    title: "Référence bancaire manquante",
                    detail: "Au moins une référence de reçu est requise.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/missing-bank-reference"
                );
            }

            var referenceTropLongue = references.FirstOrDefault(r => r.Length > ReferenceMaxLength);
            if (referenceTropLongue != null)
            {
                return Problem(
                    title: "Référence bancaire invalide",
                    detail: $"La référence « {referenceTropLongue} » dépasse {ReferenceMaxLength} caractères.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/invalid-bank-reference"
                );
            }

            // Un reçu ne peut servir qu'à un seul dossier. On compare sur la
            // valeur nettoyée : les anciennes lignes peuvent avoir des espaces.
            var referenceExistante = await _context.ReferenceBancaires
                .Where(r => references.Contains(r.Reference.Trim().ToUpper()))
                .Select(r => r.Reference)
                .FirstOrDefaultAsync();

            if (referenceExistante != null)
            {
                return Problem(
                    title: "Référence bancaire déjà utilisée",
                    detail: $"La référence « {referenceExistante.Trim()} » est déjà utilisée pour un autre dossier.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/duplicate-bank-reference"
                );
            }

            // Safely parse NumBacc (avant tu utilisais int.Parse)
            var numBaccStr = candidateInfo.BaccalaureateNumber;
            if (string.IsNullOrWhiteSpace(numBaccStr))
            {
                return Problem(
                    title: "Numéro de baccalauréat invalide",
                    detail: "candidateInfo.baccalaureateNumber est requis et ne doit pas être vide.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/invalid-field"
                );
            }

            if (!int.TryParse(numBaccStr, out var numBacc))
            {
                // Ici on renvoie ProblemDetails au lieu de laisser lever System.FormatException
                return Problem(
                    title: "Format invalide",
                    detail: $"candidateInfo.baccalaureateNumber doit être un entier. Valeur reçue: '{numBaccStr}'.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/invalid-field-format"
                );
            }

            var numBaccValue = numBaccStr.Trim();

            // Vérifier annee de bacc
            var anneeBacc = candidateInfo.GraduationYear;
            if (anneeBacc <= 0)
            {
                return Problem(
                    title: "Année de baccalauréat invalide",
                    detail: "candidateInfo.graduationYear doit être une année positive.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/invalid-field"
                );
            }

            // Vérifier existence du bac et préinscription existante (même logique)
            var Bacc = _context.Bacs
                .Where(b => b.NumBacc == numBaccValue && b.AnneeBacc == anneeBacc)
                .Select(b => new { b.IdBac })
                .FirstOrDefault();

            var anneeId = GetCurrentAnneeId();

            // Un même candidat ne peut s'inscrire qu'une fois par portail et par
            // année universitaire (il peut donc candidater de nouveau l'année suivante).
            var portailExist = new Preinscription();
            if (Bacc != null)
                portailExist = _context.Preinscriptions.FirstOrDefault(p =>
                    p.IdPortail == selectedProgram.IdPortail
                    && p.IdBac == Bacc.IdBac
                    && p.IdAnnee == anneeId);

            if (portailExist != null && portailExist.IdBac != null && portailExist.IdPortail != null)
            {
                return Problem(
                    title: "Préinscription existante",
                    detail: "Vous avez déjà une préinscription pour ce portail cette année universitaire.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/duplicate-preinscription"
                );
            }

            // Si le Bac n'existe pas, on le crée (comme avant)
            var findBacc = _context.Bacs.FirstOrDefault(b => b.NumBacc == numBaccValue && b.AnneeBacc == anneeBacc);
            if (findBacc == null)
            {
                var bacc = new Bac
                {
                    AnneeBacc = anneeBacc,
                    NumBacc = numBaccValue
                };
                _context.Bacs.Add(bacc);
                await _context.SaveChangesAsync();
            }

            // La date de paiement n'est plus saisie par le candidat : on horodate
            // la réception du dossier. L'agence n'est plus collectée non plus.
            // Important : la colonne est un "timestamp without time zone" et Npgsql
            // refuse un DateTime de Kind=Utc dessus. On utilise donc l'heure locale
            // (Kind=Local), comme le reste du contrôleur.
            var datePaiement = DateTime.Now;

            // Conversion ModeInscription (conserver la logique de mapping)
            ModeInscriptionEnum mode;
            if (string.IsNullOrWhiteSpace(data.ModeInscription))
            {
                mode = ModeInscriptionEnum.enligne;
            }
            else
            {
                var m = data.ModeInscription.Trim().ToLowerInvariant();
                mode = m == "presentielle" ? ModeInscriptionEnum.presentielle
                    : m == "sms" ? ModeInscriptionEnum.sms
                    : m == "poste" ? ModeInscriptionEnum.poste
                    : ModeInscriptionEnum.enligne;
            }

            // Construction de la préinscription (même que tu avais)
            var preinscription = new Preinscription
            {
                Email = data.PersonalInfo.Email,
                Tel = data.PersonalInfo.Telephone,
                RefBancaire = references[0],
                Agence = string.Empty,
                DatePaiement = datePaiement,
                IdPortail = selectedProgram.IdPortail,
                IdBac = _context.Bacs.FirstOrDefault(b => b.NumBacc == numBaccValue && b.AnneeBacc == anneeBacc)?.IdBac,
                IdAnnee = anneeId == 0 ? null : anneeId,
                ModeInscription = mode
            };

            _context.Preinscriptions.Add(preinscription);
            await _context.SaveChangesAsync();

            // Toutes les références sont rattachées au dossier qui vient d'être
            // créé : la principale en tête, puis les reçus complémentaires.
            // L'index unique sur reference protège en dernier recours si deux
            // candidats envoient le même reçu au même instant.
            var dateInsertion = DateTime.Now;
            foreach (var reference in references)
            {
                _context.ReferenceBancaires.Add(new ReferenceBancaire
                {
                    IdPreinscription = preinscription.IdPreinscription,
                    Reference = reference,
                    DateInsertion = dateInsertion
                });
            }
            await _context.SaveChangesAsync();

            var dataReturn = new preinscriptionReturn
            {
                Id = preinscription.IdPreinscription,
                Email = preinscription.Email,
                Tel = preinscription.Tel,
                RefBancaire = preinscription.RefBancaire,
                Agence = preinscription.Agence,
                DatePaiement = preinscription.DatePaiement,
                IdPortail = preinscription.IdPortail,
                IdBac = preinscription.IdBac,
                ModeInscription = preinscription.ModeInscription.ToString()
            };

            return Ok(dataReturn);
        }
    }
}

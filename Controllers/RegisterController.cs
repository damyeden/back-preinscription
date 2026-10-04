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
        private readonly PreinscriptionDbContext _context;
        private readonly IEmailService _emailService;

        public RegisterController(PreinscriptionDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private bool IsPreinscriptionClosed()
        {
            var currentYear = DateTime.Now.Year;
            var deadline = new DateTime(currentYear, 12, 15, 23, 59, 59);
            return DateTime.Now > deadline;
        }

        [HttpGet("test-email")]
        public async Task<ActionResult> TestEmail()
        {
            await _emailService.EnvoyerEmailAsync("654.faneva@gmail.com", "Test Email", "Ceci est un test");
            return Ok("Email envoyé !");
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

            // Vérification existence référence bancaire (comme avant)
            var refExisting = _context.Preinscriptions
                .FirstOrDefault(p => p.RefBancaire == data.BankInfo.Reference);
            if (refExisting != null)
            {
                // On garde le même code HTTP (400) mais on renvoie un ProblemDetails standard
                return Problem(
                    title: "Référence bancaire déjà utilisée",
                    detail: "La référence bancaire fournie est déjà utilisée pour une autre préinscription.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/duplicate-bank-reference"
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

            var portailExist = new Preinscription();
            if (Bacc != null)
                portailExist = _context.Preinscriptions.FirstOrDefault(p => p.IdPortail == selectedProgram.IdPortail && p.IdBac == Bacc.IdBac);

            if (portailExist != null && portailExist.IdBac != null && portailExist.IdPortail != null)
            {
                return Problem(
                    title: "Préinscription existante",
                    detail: "Vous avez déjà une préinscription pour ce portail.",
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

            // Parse DatePaiement de façon sûre
            if (string.IsNullOrWhiteSpace(bankInfo.DateRef))
            {
                return Problem(
                    title: "Date de paiement requise",
                    detail: "bankInfo.dateRef est requis.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/invalid-field"
                );
            }

            if (!DateTime.TryParse(bankInfo.DateRef, out var datePaiement))
            {
                return Problem(
                    title: "Format de date invalide",
                    detail: $"bankInfo.dateRef doit être une date valide. Valeur reçue: '{bankInfo.DateRef}'.",
                    statusCode: StatusCodes.Status400BadRequest,
                    type: "https://example.com/problems/invalid-field-format"
                );
            }

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
                RefBancaire = bankInfo.Reference,
                Agence = bankInfo.AgenceRef,
                DatePaiement = datePaiement,
                IdPortail = selectedProgram.IdPortail,
                IdBac = _context.Bacs.FirstOrDefault(b => b.NumBacc == numBaccValue && b.AnneeBacc == anneeBacc)?.IdBac,
                ModeInscription = mode
            };

            _context.Preinscriptions.Add(preinscription);
            await _context.SaveChangesAsync();

            // Envoi d'email — on continue à faire comme avant, on attrape l'exception si besoin
            try
            {
                await _emailService.EnvoyerEmailAsync(
                    preinscription.Email,
                    "Confirmation Préinscription",
                    $"Bonjour {preinscription.Email}, votre préinscription est enregistrée !"
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Erreur envoi email: " + ex.Message);
                // Ne pas échouer la création pour un échec d'email (même comportement conservé)
            }

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

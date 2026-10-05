using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backPreinscription.Models;

namespace backPreinscription.Data;

public partial class PreinscriptionDbContext : DbContext
{
    public PreinscriptionDbContext()
    {
    }

    public PreinscriptionDbContext(DbContextOptions<PreinscriptionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actualite> Actualites { get; set; }

    public virtual DbSet<ActualiteMedium> ActualiteMedia { get; set; }

    public virtual DbSet<Administration> Administrations { get; set; }

    public virtual DbSet<Appartenance> Appartenances { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Bac> Bacs { get; set; }

    public virtual DbSet<Bachelier> Bacheliers { get; set; }

    public virtual DbSet<CategorieActualite> CategorieActualites { get; set; }

    public virtual DbSet<Centre> Centres { get; set; }

    public virtual DbSet<Cofac> Cofacs { get; set; }

    public virtual DbSet<Dvc> Dvcs { get; set; }

    public virtual DbSet<EcoleDoctorale> EcoleDoctorales { get; set; }

    public virtual DbSet<Etablissement> Etablissements { get; set; }

    public virtual DbSet<Fonction> Fonctions { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Historique> Historiques { get; set; }

    public virtual DbSet<LaboMedium> LaboMedia { get; set; }

    public virtual DbSet<Laboratoire> Laboratoires { get; set; }

    public virtual DbSet<Matiere> Matieres { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Mention> Mentions { get; set; }

    public virtual DbSet<Mention1> Mentions1 { get; set; }

    public virtual DbSet<MentionNiveauParcour> MentionNiveauParcours { get; set; }

    public virtual DbSet<MentionResponsable> MentionResponsables { get; set; }

    public virtual DbSet<Niveau> Niveaus { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<Paiement> Paiements { get; set; }

    public virtual DbSet<Parcour> Parcours { get; set; }

    public virtual DbSet<ParcoursProfessionnalisante> ParcoursProfessionnalisantes { get; set; }

    public virtual DbSet<Pat> Pats { get; set; }

    public virtual DbSet<Personne> Personnes { get; set; }

    public virtual DbSet<Personne1> Personnes1 { get; set; }

    public virtual DbSet<Portail> Portails { get; set; }

    public virtual DbSet<PortailSerie> PortailSeries { get; set; }

    public virtual DbSet<PosteAffectation> PosteAffectations { get; set; }

    public virtual DbSet<Preinscription> Preinscriptions { get; set; }

    public virtual DbSet<Professeur> Professeurs { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<RespoLabo> RespoLabos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleUtilisateur> RoleUtilisateurs { get; set; }

    public virtual DbSet<Serie> Series { get; set; }

    public virtual DbSet<Sm> Sms { get; set; }

    public virtual DbSet<Specialite> Specialites { get; set; }

    public virtual DbSet<TitreProfesseur> TitreProfesseurs { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("type_formation", new[] { "Professionnalisante", "Académique" })
            .HasPostgresEnum("type_media", new[] { "Image", "vidéo" })
            .HasPostgresEnum("type_mode_preinscription", new[] { "enligne", "poste", "sms", "presentielle" })
            .HasPostgresEnum("type_niveau", new[] { "L1", "L2", "L3", "M1", "M2", "D1", "D2" })
            .HasPostgresEnum("type_sexe", new[] { "M", "F" });

        modelBuilder.Entity<Actualite>(entity =>
        {
            entity.HasKey(e => e.IdActualite).HasName("actualite_pkey");

            entity.ToTable("actualite");

            entity.Property(e => e.IdActualite)
                .HasDefaultValueSql("nextval('actualite_id_seq'::regclass)")
                .HasColumnName("id_actualite");
            entity.Property(e => e.Contenu).HasColumnName("contenu");
            entity.Property(e => e.DateArchivage)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_archivage");
            entity.Property(e => e.DateCommencement)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_commencement");
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_creation");
            entity.Property(e => e.DateFin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_fin");
            entity.Property(e => e.DateMiseAJour)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_mise_a_jour");
            entity.Property(e => e.DatePublication)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_publication");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IdCategorie).HasColumnName("id_categorie");
            entity.Property(e => e.IsUrgent)
                .HasDefaultValue(false)
                .HasColumnName("is_urgent");
            entity.Property(e => e.Lieu)
                .HasMaxLength(255)
                .HasColumnName("lieu");
            entity.Property(e => e.Titre)
                .HasMaxLength(255)
                .HasColumnName("titre");

            entity.HasOne(d => d.IdCategorieNavigation).WithMany(p => p.Actualites)
                .HasForeignKey(d => d.IdCategorie)
                .HasConstraintName("fk_categorie");
        });

        modelBuilder.Entity<ActualiteMedium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("actualite_media_pkey");

            entity.ToTable("actualite_media");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('actualite_media_id_seq1'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.IdActualite).HasColumnName("id_actualite");
            entity.Property(e => e.IdMedia).HasColumnName("id_media");

            entity.HasOne(d => d.IdActualiteNavigation).WithMany(p => p.ActualiteMedia)
                .HasForeignKey(d => d.IdActualite)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_actualite");

            entity.HasOne(d => d.IdMediaNavigation).WithMany(p => p.ActualiteMedia)
                .HasForeignKey(d => d.IdMedia)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_media");
        });

        modelBuilder.Entity<Administration>(entity =>
        {
            entity.HasKey(e => e.IdAdmin);

            entity.HasIndex(e => e.Username, "IX_Administrations_Username").IsUnique();

            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<Appartenance>(entity =>
        {
            entity.HasKey(e => e.IdAppartenance).HasName("pk_appartenance");

            entity.ToTable("appartenance");

            entity.Property(e => e.IdAppartenance)
                .HasDefaultValueSql("nextval('appartenance_id_appartenance_seq1'::regclass)")
                .HasColumnName("id_appartenance");
            entity.Property(e => e.NomAppartenance).HasColumnName("nom_appartenance");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Bac>(entity =>
        {
            entity.HasKey(e => e.IdBac).HasName("bac_pkey");

            entity.ToTable("bac");

            entity.HasIndex(e => new { e.AnneeBacc, e.NumBacc }, "unique_anne_num").IsUnique();

            entity.Property(e => e.IdBac)
                .HasDefaultValueSql("nextval('bac_id_bac_seq1'::regclass)")
                .HasColumnName("id_bac");
            entity.Property(e => e.AnneeBacc).HasColumnName("annee_bacc");
            entity.Property(e => e.DocBac).HasColumnName("doc_bac");
            entity.Property(e => e.EstMalagasy)
                .HasDefaultValue(true)
                .HasColumnName("est_malagasy");
            entity.Property(e => e.NumBacc).HasColumnName("num_bacc");
        });

        modelBuilder.Entity<Bachelier>(entity =>
        {
            entity.HasKey(e => e.IdBachelier);

            entity.HasIndex(e => e.IdCentre, "IX_Bacheliers_IdCentre");

            entity.HasIndex(e => e.IdEtablissement, "IX_Bacheliers_IdEtablissement");

            entity.HasIndex(e => e.IdMention, "IX_Bacheliers_IdMention");

            entity.HasIndex(e => e.IdOption, "IX_Bacheliers_IdOption");

            entity.HasIndex(e => e.IdPersonne, "IX_Bacheliers_IdPersonne");

            entity.HasIndex(e => new { e.NumeroCandidat, e.Annee }, "IX_Bacheliers_NumeroCandidat_Annee").IsUnique();

            entity.Property(e => e.Moyenne).HasPrecision(18, 2);

            entity.HasOne(d => d.IdCentreNavigation).WithMany(p => p.Bacheliers).HasForeignKey(d => d.IdCentre);

            entity.HasOne(d => d.IdEtablissementNavigation).WithMany(p => p.Bacheliers).HasForeignKey(d => d.IdEtablissement);

            entity.HasOne(d => d.IdMentionNavigation).WithMany(p => p.Bacheliers).HasForeignKey(d => d.IdMention);

            entity.HasOne(d => d.IdOptionNavigation).WithMany(p => p.Bacheliers).HasForeignKey(d => d.IdOption);

            entity.HasOne(d => d.IdPersonneNavigation).WithMany(p => p.Bacheliers).HasForeignKey(d => d.IdPersonne);
        });

        modelBuilder.Entity<CategorieActualite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categorie_actualite_pkey");

            entity.ToTable("categorie_actualite");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('categorie_actualite_id_seq1'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Nom)
                .HasMaxLength(255)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Centre>(entity =>
        {
            entity.HasKey(e => e.IdCentre);

            entity.HasIndex(e => e.IdProvince, "IX_Centres_IdProvince");

            entity.Property(e => e.NomCentre).HasMaxLength(200);

            entity.HasOne(d => d.IdProvinceNavigation).WithMany(p => p.Centres).HasForeignKey(d => d.IdProvince);
        });

        modelBuilder.Entity<Cofac>(entity =>
        {
            entity.HasKey(e => e.IdCofac).HasName("pk_cofac");

            entity.ToTable("cofac");

            entity.Property(e => e.IdCofac)
                .HasDefaultValueSql("nextval('cofac_id_cofac_seq1'::regclass)")
                .HasColumnName("id_cofac");
            entity.Property(e => e.AppartenanceId).HasColumnName("appartenance_id");
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date_creation");
            entity.Property(e => e.PersonneId).HasColumnName("personne_id");

            entity.HasOne(d => d.Appartenance).WithMany(p => p.Cofacs)
                .HasForeignKey(d => d.AppartenanceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_cofac_appartenance");

            entity.HasOne(d => d.Personne).WithMany(p => p.Cofacs)
                .HasForeignKey(d => d.PersonneId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_cofac_personne");
        });

        modelBuilder.Entity<Dvc>(entity =>
        {
            entity.HasKey(e => e.IdDvcs).HasName("pk_doyen_et_vice");

            entity.ToTable("dvcs");

            entity.Property(e => e.IdDvcs)
                .HasDefaultValueSql("nextval('doyen_et_vice_id_doyen_et_vice_seq'::regclass)")
                .HasColumnName("id_dvcs");
            entity.Property(e => e.DateDebut)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_debut");
            entity.Property(e => e.DateFin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_fin");
            entity.Property(e => e.IdFonction).HasColumnName("id_fonction");
            entity.Property(e => e.IdPersonne).HasColumnName("id_personne");

            entity.HasOne(d => d.IdFonctionNavigation).WithMany(p => p.Dvcs)
                .HasForeignKey(d => d.IdFonction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dvcs_cofac");

            entity.HasOne(d => d.IdPersonneNavigation).WithMany(p => p.Dvcs)
                .HasForeignKey(d => d.IdPersonne)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_doyen_et_vice_personne");
        });

        modelBuilder.Entity<EcoleDoctorale>(entity =>
        {
            entity.HasKey(e => e.IdEcoleDoctorale).HasName("pk_ecole_doctorale");

            entity.ToTable("ecole_doctorale");

            entity.Property(e => e.IdEcoleDoctorale)
                .HasDefaultValueSql("nextval('ecole_doctorale_id_ecole_doctorale_seq1'::regclass)")
                .HasColumnName("id_ecole_doctorale");
            entity.Property(e => e.NomEcoleDoctorale)
                .HasMaxLength(250)
                .HasColumnName("nom_ecole_doctorale");
        });

        modelBuilder.Entity<Etablissement>(entity =>
        {
            entity.HasKey(e => e.IdEtablissement);

            entity.Property(e => e.NomEtablissement).HasMaxLength(200);
        });

        modelBuilder.Entity<Fonction>(entity =>
        {
            entity.HasKey(e => e.IdFonction).HasName("pk_fonction");

            entity.ToTable("fonction");

            entity.Property(e => e.IdFonction)
                .HasDefaultValueSql("nextval('fonction_id_fonction_seq1'::regclass)")
                .HasColumnName("id_fonction");
            entity.Property(e => e.NomFonction).HasColumnName("nom_fonction");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.IdGrade).HasName("pk_grade");

            entity.ToTable("grade");

            entity.Property(e => e.IdGrade)
                .HasDefaultValueSql("nextval('grade_id_grade_seq1'::regclass)")
                .HasColumnName("id_grade");
            entity.Property(e => e.NomGrade).HasColumnName("nom_grade");
        });

        modelBuilder.Entity<Historique>(entity =>
        {
            entity.HasKey(e => e.IdHistorique);

            entity.HasIndex(e => e.AdminId, "IX_Historiques_AdminId");

            entity.HasIndex(e => e.IdProvince, "IX_Historiques_IdProvince");

            entity.Property(e => e.DateEvenement).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Admin).WithMany(p => p.Historiques).HasForeignKey(d => d.AdminId);

            entity.HasOne(d => d.IdProvinceNavigation).WithMany(p => p.Historiques).HasForeignKey(d => d.IdProvince);
        });

        modelBuilder.Entity<LaboMedium>(entity =>
        {
            entity.HasKey(e => e.IdLaboMedia).HasName("pk_labo_media");

            entity.ToTable("labo_media");

            entity.Property(e => e.IdLaboMedia)
                .HasDefaultValueSql("nextval('labo_media_id_labo_media_seq1'::regclass)")
                .HasColumnName("id_labo_media");
            entity.Property(e => e.Labo).HasColumnName("labo");
            entity.Property(e => e.Media).HasColumnName("media");

            entity.HasOne(d => d.LaboNavigation).WithMany(p => p.LaboMedia)
                .HasForeignKey(d => d.Labo)
                .HasConstraintName("fk_labo_media_laboratoire");

            entity.HasOne(d => d.MediaNavigation).WithMany(p => p.LaboMedia)
                .HasForeignKey(d => d.Media)
                .HasConstraintName("fk_labo_media_media");
        });

        modelBuilder.Entity<Laboratoire>(entity =>
        {
            entity.HasKey(e => e.IdLabo).HasName("pk_laboratoire");

            entity.ToTable("laboratoire");

            entity.Property(e => e.IdLabo)
                .HasDefaultValueSql("nextval('laboratoire_id_labo_seq1'::regclass)")
                .HasColumnName("id_labo");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(20)
                .HasColumnName("abbreviation");
            entity.Property(e => e.Description)
                .HasColumnType("jsonb")
                .HasColumnName("description");
            entity.Property(e => e.EcoleDoctoraleRattachement).HasColumnName("ecole_doctorale_rattachement");
            entity.Property(e => e.MentionRattachement).HasColumnName("mention_rattachement");
            entity.Property(e => e.NomLabo).HasColumnName("nom_labo");

            entity.HasOne(d => d.EcoleDoctoraleRattachementNavigation).WithMany(p => p.Laboratoires)
                .HasForeignKey(d => d.EcoleDoctoraleRattachement)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_laboratoire_ecole_doctorale");

            entity.HasOne(d => d.MentionRattachementNavigation).WithMany(p => p.Laboratoires)
                .HasForeignKey(d => d.MentionRattachement)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_laboratoire_mention");
        });

        modelBuilder.Entity<Matiere>(entity =>
        {
            entity.HasKey(e => e.IdMatiere);

            entity.Property(e => e.NomMatiere).HasMaxLength(100);
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.HasKey(e => e.IdMedia).HasName("pk_media");

            entity.ToTable("media");

            entity.Property(e => e.IdMedia)
                .HasDefaultValueSql("nextval('media_id_media_seq1'::regclass)")
                .HasColumnName("id_media");
            entity.Property(e => e.Chemin).HasColumnName("chemin");
            entity.Property(e => e.Media).HasColumnName("media");
        });

        modelBuilder.Entity<Mention>(entity =>
        {
            entity.HasKey(e => e.IdMention);

            entity.Property(e => e.Max).HasDefaultValue(20);
            entity.Property(e => e.Min).HasDefaultValue(0);
            entity.Property(e => e.NomMention).HasMaxLength(100);
        });

        modelBuilder.Entity<Mention1>(entity =>
        {
            entity.HasKey(e => e.IdMention).HasName("mention_pkey");

            entity.ToTable("mention");

            entity.Property(e => e.IdMention)
                .HasDefaultValueSql("nextval('mention_id_mention_seq1'::regclass)")
                .HasColumnName("id_mention");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(10)
                .HasColumnName("abbreviation");
            entity.Property(e => e.DescriptionMention).HasColumnName("description_mention");
            entity.Property(e => e.LogoPath).HasColumnName("logo_path");
            entity.Property(e => e.NomMention)
                .HasMaxLength(100)
                .HasColumnName("nom_mention");
        });

        modelBuilder.Entity<MentionNiveauParcour>(entity =>
        {
            entity.HasKey(e => e.IdMnp).HasName("pk_mention_niveau_parcours");

            entity.ToTable("mention_niveau_parcours");

            entity.Property(e => e.IdMnp)
                .HasDefaultValueSql("nextval('mention_niveau_parcours_id_mnp_seq1'::regclass)")
                .HasColumnName("id_mnp");
            entity.Property(e => e.IdMention).HasColumnName("id_mention");
            entity.Property(e => e.IdNiveau).HasColumnName("id_niveau");
            entity.Property(e => e.IdParcours).HasColumnName("id_parcours");
            entity.Property(e => e.IdParcoursProfessionnalisante).HasColumnName("id_parcours_professionnalisante");
            entity.Property(e => e.IdSpecialite).HasColumnName("id_specialite");

            entity.HasOne(d => d.IdMentionNavigation).WithMany(p => p.MentionNiveauParcours)
                .HasForeignKey(d => d.IdMention)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mnp_mention");

            entity.HasOne(d => d.IdNiveauNavigation).WithMany(p => p.MentionNiveauParcours)
                .HasForeignKey(d => d.IdNiveau)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mnp_niveau");

            entity.HasOne(d => d.IdParcoursNavigation).WithMany(p => p.MentionNiveauParcours)
                .HasForeignKey(d => d.IdParcours)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mention_niveau_parcours");

            entity.HasOne(d => d.IdParcoursProfessionnalisanteNavigation).WithMany(p => p.MentionNiveauParcours)
                .HasForeignKey(d => d.IdParcoursProfessionnalisante)
                .HasConstraintName("fk_mnp_parcours_professionnalisante");

            entity.HasOne(d => d.IdSpecialiteNavigation).WithMany(p => p.MentionNiveauParcours)
                .HasForeignKey(d => d.IdSpecialite)
                .HasConstraintName("fk_mnp_specialite");
        });

        modelBuilder.Entity<MentionResponsable>(entity =>
        {
            entity.HasKey(e => e.IdRespoMention).HasName("pk_mention_responsable");

            entity.ToTable("mention_responsable");

            entity.Property(e => e.IdRespoMention)
                .HasDefaultValueSql("nextval('mention_responsable_id_respo_mention_seq1'::regclass)")
                .HasColumnName("id_respo_mention");
            entity.Property(e => e.DateDebut)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date_debut");
            entity.Property(e => e.DateFin).HasColumnName("date_fin");
            entity.Property(e => e.IdMention).HasColumnName("id_mention");
            entity.Property(e => e.IdPersonne).HasColumnName("id_personne");

            entity.HasOne(d => d.IdMentionNavigation).WithMany(p => p.MentionResponsables)
                .HasForeignKey(d => d.IdMention)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_mention_responsable_mention");

            entity.HasOne(d => d.IdPersonneNavigation).WithMany(p => p.MentionResponsables)
                .HasForeignKey(d => d.IdPersonne)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_mention_responsable");
        });

        modelBuilder.Entity<Niveau>(entity =>
        {
            entity.HasKey(e => e.IdNiveau).HasName("pk_niveau");

            entity.ToTable("niveau");

            entity.Property(e => e.IdNiveau)
                .HasDefaultValueSql("nextval('niveau_id_niveau_seq1'::regclass)")
                .HasColumnName("id_niveau");
            entity.Property(e => e.NomNiveau)
                .HasMaxLength(10)
                .HasColumnName("nom_niveau");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.IdNote);

            entity.HasIndex(e => e.IdBachelier, "IX_Notes_IdBachelier");

            entity.HasIndex(e => e.IdMatiere, "IX_Notes_IdMatiere");

            entity.Property(e => e.EstOptionnel).HasDefaultValue(false);
            entity.Property(e => e.ValeurNote).HasPrecision(18, 2);

            entity.HasOne(d => d.IdBachelierNavigation).WithMany(p => p.Notes).HasForeignKey(d => d.IdBachelier);

            entity.HasOne(d => d.IdMatiereNavigation).WithMany(p => p.Notes).HasForeignKey(d => d.IdMatiere);
        });

        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(e => e.IdOption);

            entity.Property(e => e.Serie).HasMaxLength(50);
        });

        modelBuilder.Entity<Paiement>(entity =>
        {
            entity.HasKey(e => e.IdPaiement).HasName("paiement_pkey");

            entity.ToTable("paiement");

            entity.HasIndex(e => new { e.Agence, e.DatePaiement, e.RefBancaire }, "unique_refe_date_agence").IsUnique();

            entity.Property(e => e.IdPaiement)
                .HasDefaultValueSql("nextval('paiement_id_paiement_seq1'::regclass)")
                .HasColumnName("id_paiement");
            entity.Property(e => e.Agence)
                .HasMaxLength(200)
                .HasColumnName("agence");
            entity.Property(e => e.DateInsertion)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date_insertion");
            entity.Property(e => e.DatePaiement).HasColumnName("date_paiement");
            entity.Property(e => e.Espece).HasColumnName("espece");
            entity.Property(e => e.IdPreinscription).HasColumnName("id_preinscription");
            entity.Property(e => e.IdUtilisateur).HasColumnName("id_utilisateur");
            entity.Property(e => e.MotifPaiement)
                .HasMaxLength(255)
                .HasColumnName("motif_paiement");
            entity.Property(e => e.NomBeneficiaire)
                .HasMaxLength(200)
                .HasColumnName("nom_beneficiaire");
            entity.Property(e => e.NomPayeur)
                .HasMaxLength(200)
                .HasColumnName("nom_payeur");
            entity.Property(e => e.RefBancaire)
                .HasMaxLength(100)
                .HasColumnName("ref_bancaire");

            entity.HasOne(d => d.IdPreinscriptionNavigation).WithMany(p => p.Paiements)
                .HasForeignKey(d => d.IdPreinscription)
                .HasConstraintName("paiement_id_preinscription_fkey");

            entity.HasOne(d => d.IdUtilisateurNavigation).WithMany(p => p.Paiements)
                .HasForeignKey(d => d.IdUtilisateur)
                .HasConstraintName("paiement_id_utilisateur_fkey");
        });

        modelBuilder.Entity<Parcour>(entity =>
        {
            entity.HasKey(e => e.IdParcours).HasName("parcours_pkey");

            entity.ToTable("parcours");

            entity.Property(e => e.IdParcours)
                .HasDefaultValueSql("nextval('parcours_id_parcours_seq1'::regclass)")
                .HasColumnName("id_parcours");
            entity.Property(e => e.DescriptionParcours).HasColumnName("description_parcours");
            entity.Property(e => e.NomParcours)
                .HasMaxLength(100)
                .HasColumnName("nom_parcours");
        });

        modelBuilder.Entity<ParcoursProfessionnalisante>(entity =>
        {
            entity.HasKey(e => e.IdParcoursProfessionnalisante).HasName("pk_parcours_professionnalisante");

            entity.ToTable("parcours_professionnalisante");

            entity.Property(e => e.IdParcoursProfessionnalisante)
                .HasDefaultValueSql("nextval('parcours_professionnalisante_id_parcours_professionnalisant_seq'::regclass)")
                .HasColumnName("id_parcours_professionnalisante");
            entity.Property(e => e.NomParcoursProfessionnalisante)
                .HasMaxLength(100)
                .HasColumnName("nom_parcours_professionnalisante");
        });

        modelBuilder.Entity<Pat>(entity =>
        {
            entity.HasKey(e => e.IdPat).HasName("pk_pat");

            entity.ToTable("pat");

            entity.Property(e => e.IdPat)
                .HasDefaultValueSql("nextval('pat_id_pat_seq1'::regclass)")
                .HasColumnName("id_pat");
            entity.Property(e => e.DateDebut)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_debut");
            entity.Property(e => e.DateFin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_fin");
            entity.Property(e => e.IdFonction).HasColumnName("id_fonction");
            entity.Property(e => e.IdGrade).HasColumnName("id_grade");
            entity.Property(e => e.IdPersonne).HasColumnName("id_personne");
            entity.Property(e => e.IdPostAffectation).HasColumnName("id_post_affectation");

            entity.HasOne(d => d.IdFonctionNavigation).WithMany(p => p.Pats)
                .HasForeignKey(d => d.IdFonction)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_pat_fonction");

            entity.HasOne(d => d.IdGradeNavigation).WithMany(p => p.Pats)
                .HasForeignKey(d => d.IdGrade)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_pat_grade");

            entity.HasOne(d => d.IdPersonneNavigation).WithMany(p => p.Pats)
                .HasForeignKey(d => d.IdPersonne)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_pat_personne");

            entity.HasOne(d => d.IdPostAffectationNavigation).WithMany(p => p.Pats)
                .HasForeignKey(d => d.IdPostAffectation)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_pat_poste_affectation");
        });

        modelBuilder.Entity<Personne>(entity =>
        {
            entity.HasKey(e => e.IdPersonne);

            entity.Property(e => e.LieuNaissance).HasMaxLength(200);
            entity.Property(e => e.NomPrenom).HasMaxLength(200);
            entity.Property(e => e.Sexe)
                .HasDefaultValueSql("'M'::character varying")
                .HasColumnType("character varying");
        });

        modelBuilder.Entity<Personne1>(entity =>
        {
            entity.HasKey(e => e.IdPersonne).HasName("pk_personne");

            entity.ToTable("personne");

            entity.Property(e => e.IdPersonne)
                .HasDefaultValueSql("nextval('personne_id_personne_seq1'::regclass)")
                .HasColumnName("id_personne");
            entity.Property(e => e.DateInsertion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_insertion");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IdLabo).HasColumnName("id_labo");
            entity.Property(e => e.Nom).HasColumnName("nom");
            entity.Property(e => e.Prenom).HasColumnName("prenom");
            entity.Property(e => e.Tel)
                .HasMaxLength(20)
                .HasColumnName("tel");
        });

        modelBuilder.Entity<Portail>(entity =>
        {
            entity.HasKey(e => e.IdPortail).HasName("pk_portail");

            entity.ToTable("portail");

            entity.Property(e => e.IdPortail).HasColumnName("id_portail");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(10)
                .HasColumnName("abbreviation");
            entity.Property(e => e.EstAcademique)
                .HasDefaultValue(true)
                .HasColumnName("est_academique");
            entity.Property(e => e.NomPortail).HasColumnName("nom_portail");
        });

        modelBuilder.Entity<PortailSerie>(entity =>
        {
            entity.HasKey(e => e.IdPortserie).HasName("pk_portail_serie");

            entity.ToTable("portail_serie");

            entity.Property(e => e.IdPortserie).HasColumnName("id_portserie");
            entity.Property(e => e.IdPortail).HasColumnName("id_portail");
            entity.Property(e => e.IdSerie).HasColumnName("id_serie");

            entity.HasOne(d => d.IdPortailNavigation).WithMany(p => p.PortailSeries)
                .HasForeignKey(d => d.IdPortail)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_portail_serie_portail");

            entity.HasOne(d => d.IdSerieNavigation).WithMany(p => p.PortailSeries)
                .HasForeignKey(d => d.IdSerie)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_portail_serie_null");
        });

        modelBuilder.Entity<PosteAffectation>(entity =>
        {
            entity.HasKey(e => e.IdPostAffectation).HasName("pk_post_affectation");

            entity.ToTable("poste_affectation");

            entity.Property(e => e.IdPostAffectation)
                .HasDefaultValueSql("nextval('poste_affectation_id_post_affectation_seq1'::regclass)")
                .HasColumnName("id_post_affectation");
            entity.Property(e => e.AbbreviationPost).HasColumnName("abbreviation_post");
            entity.Property(e => e.NomPost).HasColumnName("nom_post");
        });

        modelBuilder.Entity<Preinscription>(entity =>
        {
            entity.HasKey(e => e.IdPreinscription).HasName("preinscription_pkey");

            entity.ToTable("preinscription");

            entity.HasIndex(e => new { e.Agence, e.DatePaiement, e.RefBancaire }, "unique_ref_date_agence").IsUnique();

            entity.Property(e => e.IdPreinscription)
                .HasDefaultValueSql("nextval('preinscription_id_preinscription_seq1'::regclass)")
                .HasColumnName("id_preinscription");
            entity.Property(e => e.AdminValidation).HasColumnName("admin_validation");
            entity.Property(e => e.Agence)
                .HasMaxLength(200)
                .HasColumnName("agence");
            entity.Property(e => e.DatePaiement)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_paiement");
            entity.Property(e => e.DatePreinscription)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_preinscription");
            entity.Property(e => e.DateSelection).HasColumnName("date_selection");
            entity.Property(e => e.DateValidation)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_validation");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.EstSelectionner)
                .HasDefaultValue(false)
                .HasColumnName("est_selectionner");
            entity.Property(e => e.EstValide)
                .HasDefaultValue(false)
                .HasColumnName("est_valide");
            entity.Property(e => e.IdBac).HasColumnName("id_bac");
            entity.Property(e => e.IdAnnee).HasColumnName("id_annee");
            entity.Property(e => e.ModeInscription).HasColumnName("mode_preinscription").HasColumnType("type_mode_inscription");
            entity.Property(e => e.IdPortail).HasColumnName("id_portail");
            entity.Property(e => e.RefBancaire)
                .HasMaxLength(100)
                .HasColumnName("ref_bancaire");
            entity.Property(e => e.Tel)
                .HasMaxLength(100)
                .HasColumnName("tel");

            entity.HasOne(d => d.AdminValidationNavigation).WithMany(p => p.Preinscriptions)
                .HasForeignKey(d => d.AdminValidation)
                .HasConstraintName("preinscription_admin_validation_fkey");

            entity.HasOne(d => d.IdBacNavigation).WithMany(p => p.Preinscriptions)
                .HasForeignKey(d => d.IdBac)
                .HasConstraintName("preinscription_id_bac_fkey");

            entity.HasOne(d => d.IdPortailNavigation).WithMany(p => p.Preinscriptions)
                .HasForeignKey(d => d.IdPortail)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_preinscription_null");
        });

        modelBuilder.Entity<Professeur>(entity =>
        {
            entity.HasKey(e => e.IdProfesseur).HasName("pk_professeur");

            entity.ToTable("professeur");

            entity.Property(e => e.IdProfesseur)
                .HasDefaultValueSql("nextval('professeur_id_professeur_seq1'::regclass)")
                .HasColumnName("id_professeur");
            entity.Property(e => e.Personne).HasColumnName("personne");
            entity.Property(e => e.Titre).HasColumnName("titre");

            entity.HasOne(d => d.PersonneNavigation).WithMany(p => p.Professeurs)
                .HasForeignKey(d => d.Personne)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_professeur_personne");

            entity.HasOne(d => d.TitreNavigation).WithMany(p => p.Professeurs)
                .HasForeignKey(d => d.Titre)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_professeur_titre_professeur");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.IdProvince);

            entity.Property(e => e.NomProvince).HasMaxLength(100);
        });

        modelBuilder.Entity<RespoLabo>(entity =>
        {
            entity.HasKey(e => e.IdRespoLabo).HasName("pk_respo_labo");

            entity.ToTable("respo_labo");

            entity.Property(e => e.IdRespoLabo)
                .HasDefaultValueSql("nextval('respo_labo_id_respo_labo_seq1'::regclass)")
                .HasColumnName("id_respo_labo");
            entity.Property(e => e.DateDebut)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_debut");
            entity.Property(e => e.DateFin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_fin");
            entity.Property(e => e.IdLabo).HasColumnName("id_labo");
            entity.Property(e => e.Responsable).HasColumnName("responsable");

            entity.HasOne(d => d.IdLaboNavigation).WithMany(p => p.RespoLabos)
                .HasForeignKey(d => d.IdLabo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_respo_labo_laboratoire");

            entity.HasOne(d => d.ResponsableNavigation).WithMany(p => p.RespoLabos)
                .HasForeignKey(d => d.Responsable)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_respo_labo_personne");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.IdRole)
                .HasDefaultValueSql("nextval('role_id_role_seq1'::regclass)")
                .HasColumnName("id_role");
            entity.Property(e => e.NomRole)
                .HasMaxLength(200)
                .HasColumnName("nom_role");
        });

        modelBuilder.Entity<RoleUtilisateur>(entity =>
        {
            entity.HasKey(e => e.IdRoleUtilisateur).HasName("role_utilisateur_pkey");

            entity.ToTable("role_utilisateur");

            entity.Property(e => e.IdRoleUtilisateur)
                .HasDefaultValueSql("nextval('role_utilisateur_id_role_utilisateur_seq1'::regclass)")
                .HasColumnName("id_role_utilisateur");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.IdUtilisateur).HasColumnName("id_utilisateur");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.RoleUtilisateurs)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_utilisateur_id_role_fkey");

            entity.HasOne(d => d.IdUtilisateurNavigation).WithMany(p => p.RoleUtilisateurs)
                .HasForeignKey(d => d.IdUtilisateur)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_utilisateur_id_utilisateur_fkey");
        });

        modelBuilder.Entity<Serie>(entity =>
        {
            entity.HasKey(e => e.IdSerie).HasName("pk_serie");

            entity.ToTable("serie");

            entity.Property(e => e.IdSerie).HasColumnName("id_serie");
            entity.Property(e => e.EstTechnique)
                .HasDefaultValue(false)
                .HasColumnName("est_technique");
            entity.Property(e => e.NomSerie).HasColumnName("nom_serie");
        });

        modelBuilder.Entity<Sm>(entity =>
        {
            entity.HasKey(e => e.IdSms).HasName("sms_pkey");

            entity.ToTable("sms");

            entity.Property(e => e.IdSms)
                .HasDefaultValueSql("nextval('sms_id_sms_seq1'::regclass)")
                .HasColumnName("id_sms");
            entity.Property(e => e.Contenu)
                .HasMaxLength(255)
                .HasColumnName("contenu");
            entity.Property(e => e.DateSms).HasColumnName("date_sms");
            entity.Property(e => e.Envoyeur)
                .HasMaxLength(15)
                .HasColumnName("envoyeur");
            entity.Property(e => e.Recepteur)
                .HasMaxLength(15)
                .HasColumnName("recepteur");
        });

        modelBuilder.Entity<Specialite>(entity =>
        {
            entity.HasKey(e => e.IdSpecialite).HasName("specialite_pkey");

            entity.ToTable("specialite");

            entity.Property(e => e.IdSpecialite)
                .HasDefaultValueSql("nextval('specialite_id_specialite_seq1'::regclass)")
                .HasColumnName("id_specialite");
            entity.Property(e => e.NomSpecialite)
                .HasMaxLength(200)
                .HasColumnName("nom_specialite");
        });

        modelBuilder.Entity<TitreProfesseur>(entity =>
        {
            entity.HasKey(e => e.IdTitre).HasName("pk_titre_professeur");

            entity.ToTable("titre_professeur");

            entity.Property(e => e.IdTitre)
                .HasDefaultValueSql("nextval('titre_professeur_id_titre_seq1'::regclass)")
                .HasColumnName("id_titre");
            entity.Property(e => e.NomTitre)
                .HasMaxLength(250)
                .HasColumnName("nom_titre");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.IdUtilisateur).HasName("utilisateurs_pkey");

            entity.ToTable("utilisateurs");

            entity.Property(e => e.IdUtilisateur)
                .HasDefaultValueSql("nextval('utilisateurs_id_utilisateur_seq1'::regclass)")
                .HasColumnName("id_utilisateur");
            entity.Property(e => e.Identifiant)
                .HasMaxLength(200)
                .HasColumnName("identifiant");
            entity.Property(e => e.MotDePasse)
                .HasMaxLength(200)
                .HasColumnName("mot_de_passe");
        });
        modelBuilder.HasSequence<int>("actualite_id_seq");
        modelBuilder.HasSequence<int>("actualite_media_id_seq");
        modelBuilder.HasSequence<int>("actualite_media_id_seq1");
        modelBuilder.HasSequence<int>("appartenance_id_appartenance_seq");
        modelBuilder.HasSequence<int>("appartenance_id_appartenance_seq1");
        modelBuilder.HasSequence<int>("bac_id_bac_seq");
        modelBuilder.HasSequence<int>("bac_id_bac_seq1");
        modelBuilder.HasSequence<int>("categorie_actualite_id_seq");
        modelBuilder.HasSequence<int>("categorie_actualite_id_seq1");
        modelBuilder.HasSequence<int>("cofac_id_cofac_seq");
        modelBuilder.HasSequence<int>("cofac_id_cofac_seq1");
        modelBuilder.HasSequence<int>("doyen_et_vice_id_doyen_et_vice_seq");
        modelBuilder.HasSequence<int>("ecole_doctorale_id_ecole_doctorale_seq");
        modelBuilder.HasSequence<int>("ecole_doctorale_id_ecole_doctorale_seq1");
        modelBuilder.HasSequence<int>("faculte_id_seq");
        modelBuilder.HasSequence<int>("fonction_id_fonction_seq");
        modelBuilder.HasSequence<int>("fonction_id_fonction_seq1");
        modelBuilder.HasSequence<int>("grade_id_grade_seq");
        modelBuilder.HasSequence<int>("grade_id_grade_seq1");
        modelBuilder.HasSequence<int>("labo_media_id_labo_media_seq");
        modelBuilder.HasSequence<int>("labo_media_id_labo_media_seq1");
        modelBuilder.HasSequence<int>("laboratoire_id_labo_seq");
        modelBuilder.HasSequence<int>("laboratoire_id_labo_seq1");
        modelBuilder.HasSequence<int>("media_id_media_seq");
        modelBuilder.HasSequence<int>("media_id_media_seq1");
        modelBuilder.HasSequence<int>("mention_id_mention_seq");
        modelBuilder.HasSequence<int>("mention_id_mention_seq1");
        modelBuilder.HasSequence<int>("mention_id_seq");
        modelBuilder.HasSequence<int>("mention_niveau_parcours_id_mnp_seq");
        modelBuilder.HasSequence<int>("mention_niveau_parcours_id_mnp_seq1");
        modelBuilder.HasSequence<int>("mention_responsable_id_respo_mention_seq");
        modelBuilder.HasSequence<int>("mention_responsable_id_respo_mention_seq1");
        modelBuilder.HasSequence<int>("niveau_id_niveau_seq");
        modelBuilder.HasSequence<int>("niveau_id_niveau_seq1");
        modelBuilder.HasSequence<int>("paiement_id_paiement_seq");
        modelBuilder.HasSequence<int>("paiement_id_paiement_seq1");
        modelBuilder.HasSequence<int>("parcours_id_parcours_seq");
        modelBuilder.HasSequence<int>("parcours_id_parcours_seq1");
        modelBuilder.HasSequence<int>("parcours_id_seq");
        modelBuilder.HasSequence<int>("parcours_professionnalisante_id_parcours_professionnalisant_seq");
        modelBuilder.HasSequence<int>("pat_id_pat_seq");
        modelBuilder.HasSequence<int>("pat_id_pat_seq1");
        modelBuilder.HasSequence<int>("personne_id_personne_seq");
        modelBuilder.HasSequence<int>("personne_id_personne_seq1");
        modelBuilder.HasSequence<int>("poste_affectation_id_post_affectation_seq");
        modelBuilder.HasSequence<int>("poste_affectation_id_post_affectation_seq1");
        modelBuilder.HasSequence<int>("preinscription_id_preinscription_seq");
        modelBuilder.HasSequence<int>("preinscription_id_preinscription_seq1");
        modelBuilder.HasSequence<int>("professeur_id_professeur_seq");
        modelBuilder.HasSequence<int>("professeur_id_professeur_seq1");
        modelBuilder.HasSequence<int>("respo_labo_id_respo_labo_seq");
        modelBuilder.HasSequence<int>("respo_labo_id_respo_labo_seq1");
        modelBuilder.HasSequence<int>("role_id_role_seq");
        modelBuilder.HasSequence<int>("role_id_role_seq1");
        modelBuilder.HasSequence<int>("role_utilisateur_id_role_utilisateur_seq");
        modelBuilder.HasSequence<int>("role_utilisateur_id_role_utilisateur_seq1");
        modelBuilder.HasSequence<int>("sms_id_sms_seq");
        modelBuilder.HasSequence<int>("sms_id_sms_seq1");
        modelBuilder.HasSequence<int>("specialite_id_specialite_seq");
        modelBuilder.HasSequence<int>("specialite_id_specialite_seq1");
        modelBuilder.HasSequence<int>("titre_professeur_id_titre_seq");
        modelBuilder.HasSequence<int>("titre_professeur_id_titre_seq1");
        modelBuilder.HasSequence<int>("utilisateurs_id_utilisateur_seq");
        modelBuilder.HasSequence<int>("utilisateurs_id_utilisateur_seq1");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

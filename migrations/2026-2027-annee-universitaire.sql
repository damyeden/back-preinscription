-- =============================================================================
--  Bascule de l'année universitaire vers 2026-2027
-- =============================================================================
--  Contexte :
--    - la table `annees_universitaires` contient déjà 2025-2026 (active) et
--      2026-2027 (inactive) ;
--    - les 3083 dossiers déjà présents dans `preinscription` ont `id_annee` à
--      NULL : ils appartiennent à l'année 2025-2026 ;
--    - l'année universitaire en cours devient 2026-2027. Les nouveaux dossiers
--      y sont rattachés automatiquement par l'API, qui lit
--      `annees_universitaires.est_active` (aucune modification de code n'est
--      donc nécessaire l'année prochaine : il suffira de basculer le drapeau).
--
--  Exécution :
--    psql "postgresql://admin:sciences5202@localhost:5432/fac_sciences" \
--         -f back-preinscription/migrations/2026-2027-annee-universitaire.sql
-- =============================================================================

BEGIN;

-- 1) Les dossiers déjà en base sont rattachés à l'année universitaire 2025-2026.
UPDATE preinscription
SET id_annee = (
        SELECT id_annee
        FROM annees_universitaires
        WHERE libelle = '2025-2026'
    )
WHERE id_annee IS NULL;

-- 2) L'année universitaire active devient 2026-2027.
UPDATE annees_universitaires
SET est_active = (libelle = '2026-2027');

COMMIT;

-- -----------------------------------------------------------------------------
--  Contrôles
-- -----------------------------------------------------------------------------
SELECT id_annee, libelle, est_active
FROM annees_universitaires
ORDER BY id_annee;

SELECT au.libelle AS annee_universitaire, count(*) AS dossiers
FROM preinscription p
JOIN annees_universitaires au ON au.id_annee = p.id_annee
GROUP BY au.libelle
ORDER BY au.libelle;

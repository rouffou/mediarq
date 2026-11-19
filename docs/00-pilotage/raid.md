---
titre: Registre RAID — Mediarq
auteur: project-manager
statut: en revue
date: 2026-11-15
---

# Registre RAID — Mediarq

> Initialisé le 2026-11-15 à partir du diagnostic ([2026-11-15-status-report.md](2026-11-15-status-report.md)). Criticité = Probabilité × Impact (1-5 chacun). Les propriétaires sont **proposés** (rôles de l'équipe d'agents ou mainteneur) et à confirmer par l'utilisateur. Les sources sont citées ; `[H]` = hypothèse non vérifiée.

## Risques
| ID | Description | Probabilité (1-5) | Impact (1-5) | Criticité | Mitigation | Propriétaire (proposé) | Statut |
|---|---|---|---|---|---|---|---|
| R-001 | Perte/blocage de messages Outbox et doublons Idempotency en production : défauts confirmés (voir I-003, I-004, I-005) dans des packages déjà publiés (v1.5.1) | 4 | 5 | 20 | Corriger en lot L1 (MaxAttempts + backoff + `Failed`, verrou atomique, clé cloisonnée, marqueur « en cours ») ; tests d'intégration Testcontainers ; publier une note de risque dans le README des packages en attendant | Mainteneur + développeur ; QA pour les tests | ouvert |
| R-002 | Aucun scan de sécurité effectif : CodeQL `disabled_inactivity`, aucun `dependabot.yml` ; dépendances obsolètes ou vulnérables non détectées sur 31 packages | 4 | 4 | 16 | Réactiver CodeQL (`gh workflow enable` ou commit), ajouter `dependabot.yml` (nuget + github-actions), triager les alertes | Mainteneur ; DevOps | ouvert |
| R-003 | Dépôt dormant (112 jours sans commit) et documents de pilotage faux (ROADMAP, CHANGELOG, RECAP, CONTEXT) : perte de contexte, perte de confiance des utilisateurs NuGet | 4 | 3 | 12 | Lot L3 (mise à jour ROADMAP/CHANGELOG, archivage des docs historiques) ; décider du cap (D-C) | Chef de projet ; rédacteur technique | ouvert |
| R-004 | Facteur bus = 1 : un seul mainteneur (`[H1]`), pas de revue externe, validation de PR par l'utilisateur uniquement | 3 | 4 | 12 | Documenter le processus de release (release.yml, tags), ajouter CONTRIBUTING/SECURITY si absent `[H]`, suivre la contribution externe | Mainteneur | ouvert |
| R-005 | Changements de comportement à fort impact sur les consommateurs : publisher parallèle par défaut (« A second operation was started on this context » avec `DbContext` partagé), encapsulation d'exceptions de `Send` | 3 | 4 | 12 | ADR de décision (D-B), note de migration, version majeure si nécessaire (SemVer strict, ROADMAP « Continuous ») | Architecte logiciel ; chef de projet | ouvert |
| R-006 | Coût de dispatch plus élevé que prévu en production : graphe cœur scoped (`ServiceCollectionExtensions.cs:241-249`), `UnitOfWorkBehavior` actif pour toutes les requêtes (616 B/requête d'après l'analyse du 2026-10-24 `[H]` non remesuré) | 3 | 3 | 9 | Benchmark « un scope par requête », rendre `UnitOfWorkBehavior` conditionnel, singletonisation du graphe sans état | Développeur ; benchmarks en CI | ouvert |
| R-007 | Régression de packaging NuGet (31 lignes `dotnet pack` codées en dur dans release.yml, version figée par csproj ; un package déjà oublié par le passé, PR #164 ; assets de framework manquants corrigés par #226) | 3 | 3 | 9 | `dotnet pack` sur la solution + `IsPackable`, version unique (MinVer), job de vérification du contenu des packages avant publication | DevOps | ouvert |
| R-008 | Dépendances qui contredisent le positionnement « dependency-free » (Scrutor, `Microsoft.AspNetCore.Http.Abstractions 2.3.0` dans `Mediarq.Core.csproj:33,36`) : dette de compatibilité et de sécurité, argument d'adoption affaibli | 3 | 3 | 9 | Déplacer `HttpUserContext` dans `Mediarq.AspNetCore`, scanner interne (breaking, voir R-005) | Architecte logiciel | ouvert |
| R-009 | Couverture de tests faible sur plusieurs packages (OpenTelemetry 2, Polly 3, DataAnnotations 3, MassTransit 4, UnitOfWork 4 tests, selon l'analyse du 2026-10-24 `[H2]`) ; cible ≥ 85 % non mesurée dans cette passe (issue #39 ouverte) | 3 | 3 | 9 | Plan de tests par package ; seuil Codecov informatif puis bloquant | QA | ouvert |
| R-010 | Connaissance du projet portée par des documents hors dépôt versionné (`cahier des charges/`, `army/` non suivis par git) : perte possible | 2 | 3 | 6 | Décision D-A (commit, déplacement ou ignore) | Utilisateur | ouvert |

## Actions
| ID | Action | Propriétaire | Échéance | Statut |
|---|---|---|---|---|
| A-001 | Décider si les 6 issues P0 de l'analyse (publisher, `Send`, Outbox poison, Outbox verrou, Idempotency concurrence, Idempotency clé) sont créées dans GitHub (non créées par cette passe) | Utilisateur | 2026-11-22 | à faire |
| A-002 | Réactiver CodeQL et Mutation testing (`disabled_inactivity`) | Mainteneur / DevOps | 2026-11-22 | à faire (accord requis) |
| A-003 | Ajouter `.github/dependabot.yml` (nuget + github-actions) | DevOps | 2026-11-29 | à faire |
| A-004 | Créer les issues P1 de l'analyse (cache des échecs, UnitOfWork conditionnel, MQ203, couverture, pack auto, guides DocFX) dans les milestones adéquats | Chef de projet (après accord) | 2026-11-29 | à faire |
| A-005 | Rédiger le correctif de justesse L1 : publisher parallèle (`ParallelNotificationPublisher.cs:15-21`), alignement sur `AggregateExceptionNotificationPublisher` | Développeur | 2026-12-06 | à faire |
| A-006 | Outbox : `MaxAttempts`, `NextAttemptAtUtc` avec backoff, statut `Failed`, réservation atomique | Développeur | 2026-12-13 | à faire |
| A-007 | Idempotency : clé préfixée par type de requête et utilisateur/tenant, marqueur « en cours » atomique (409 ou attente) | Développeur | 2026-12-13 | à faire |
| A-008 | Tests Testcontainers (Outbox avec 2 processeurs, Redis pour l'idempotence concurrente) pour valider A-006/A-007 | QA | 2026-12-13 | à faire |
| A-009 | Rattraper CHANGELOG.md (sections 1.1.0 à 1.5.1, supprimer le doublon `[1.0.0]` l. 474) | Rédacteur technique | 2026-11-29 | à faire |
| A-010 | Mettre à jour ROADMAP.md (v1.4 à v1.6) ; archiver IDEAS, RECAP, next-step, etapes-suivantes, vigilance dans `docs/archive/` ; mettre à jour CONTEXT.md | Rédacteur technique | 2026-11-29 | à faire |
| A-011 | Rédiger l'ADR « version cassante (v2.0) vs note de migration (v1.6) » pour publisher séquentiel, retrait Scrutor/AspNetCore 2.3, encapsulation `Send` | Architecte logiciel | 2026-12-06 | à faire |
| A-012 | Nettoyer l'hygiène git : synchroniser `dev` local sur `origin/dev`, supprimer les branches fusionnées (dont `fix/nuget-meta-package-no-framework-assets`), décider du sort de `.gitignore` modifié et de `*.csproj.user` | Utilisateur (suppression = action irréversible) | 2026-11-22 | à faire |
| A-013 | Régénérer le graphe graphify (`graphify update .`, construit sur `575c09a1`) | Développeur | 2026-11-22 | à faire |
| A-014 | Documenter dans DocFX les packages de fiabilité et transport (`docs/docfx.json` ne couvre que 11 projets sur 31) | Rédacteur technique | 2026-12-20 | à faire |
| A-015 | Release v1.6.0 : pack automatique (MinVer), vérification du contenu des packages, note de release | DevOps | 2026-12-20 | à faire |

## Issues (problèmes avérés)
| ID | Description | Impact | Sévérité (P0-P3) | Propriétaire | Résolution | Statut |
|---|---|---|---|---|---|---|
| I-001 | `ParallelNotificationPublisher` : exception synchrone d'un handler interrompt la boucle de démarrage (`ParallelNotificationPublisher.cs:15-21`), contredit la doc (l.5). Fait : lu dans le code. | Handlers de notification jamais exécutés | P0 | Développeur | A-005 | ouvert |
| I-002 | `Mediator.Send` : encapsulation en `InvalidOperationException` (`Mediator.cs:118`) ; la cohérence synchrone/asynchrone décrite par l'analyse du 2026-10-24 n'est pas revérifiée `[H5]` | Type d'exception variable selon l'écriture du handler | P0 `[H]` | Développeur | A-011 puis correctif | ouvert, à confirmer |
| I-003 | Outbox : message empoisonné bloquant (`OutboxDispatcher.cs:32-34`, `:55-59` : `Attempts++` sans limite ni backoff ni statut `Failed`) | File d'événements bloquée tant que les N premiers messages échouent | P0 | Développeur | A-006 | ouvert |
| I-004 | Outbox : aucun verrou ni bail (grep `LockedUntil|MaxAttempts|SKIP LOCKED` : 0 résultat dans `src/Mediarq.Outbox`) | Doublons avec plusieurs instances | P0 | Développeur | A-006 | ouvert |
| I-005 | Idempotency : clé `"mediarq:idem:" + key` sans type ni utilisateur (`IdempotencyBehavior.cs:51`) ; pas de marqueur « en cours » constaté dans le fichier lu | Collisions entre commandes, risque de fuite de données ; exécutions concurrentes | P0 | Développeur | A-007 | ouvert |
| I-006 | CodeQL et Mutation testing en `disabled_inactivity` (`gh workflow list --all`) | Aucune analyse de sécurité ni de mutation depuis 2026-09-28 | P1 | Mainteneur | A-002 | ouvert |
| I-007 | Aucun `.github/dependabot.yml`, alors que ROADMAP.md:152 le revendique | Dépendances non surveillées | P1 | DevOps | A-003 | ouvert |
| I-008 | CHANGELOG.md sans sections 1.1.0 à 1.5.1 (seulement `[Unreleased]` + `[1.0.0]` dupliqué l. 313 et 474), alors que 7 versions sont publiées ; contredit « every user-facing change lands in the CHANGELOG » (ROADMAP « Continuous », #42) | Notes de version inexploitables | P1 | Rédacteur technique | A-009 | ouvert |
| I-009 | ROADMAP.md, RECAP.md, CONTEXT.md, IDEAS.md, next-step.md, etapes-suivantes.md, vigilance-ameliorations.md périmés (juin 2026) | Pilotage et onboarding trompeurs | P2 | Rédacteur technique | A-010 | ouvert |
| I-010 | 35 propositions (dont les 6 P0 ci-dessus) non tracées dans GitHub : seules 5 issues ouvertes (#39-#42, #178) | Travail identifié non planifiable | P1 | Chef de projet | A-001, A-004 | ouvert |
| I-011 | Branche courante `fix/nuget-meta-package-no-framework-assets` déjà fusionnée (PR #226) ; `dev` local en retard de 5 commits sur `origin/dev` ; 15 branches fusionnées non supprimées | Risque de travailler sur une base périmée, bruit | P2 | Utilisateur | A-012 | ouvert |
| I-012 | Graphe graphify construit sur `575c09a1` (HEAD `7ca85d5`) | Requêtes graphify potentiellement en retard | P3 | Développeur | A-013 | ouvert |
| I-013 | `docs/docfx.json` ne documente que 11 des 31 projets | Site de doc incomplet pour les packages récents | P2 | Rédacteur technique | A-014 | ouvert |

## Décisions
| ID | Décision | Date | Décideur | Lien ADR |
|---|---|---|---|---|
| D-001 | Politique de branches : `main` / `dev`, préfixes `feat` / `fix` / `hotfix`, protection de branche GitHub active (source : mémoire du projet, non revérifiée dans cette passe) | antérieure à 2026-11-15 | Mainteneur (Nicolas Rouffart) | — |
| D-002 | Aucune fusion de PR par un agent : ouvrir la PR, attendre la CI verte, puis laisser l'utilisateur fusionner (source : mémoire du projet) | antérieure à 2026-11-15 | Mainteneur | — |
| D-003 | Backlog de 130 issues et 18 milestones importé dans GitHub : ne pas recréer (mémoire du projet ; `gh` voit aujourd'hui 155 issues dont 150 fermées, 18 milestones) | antérieure à 2026-11-15 | Mainteneur | — |
| D-004 | Mission de diagnostic : lecture seule sur le code, écriture limitée à `docs/00-pilotage/`, aucun commit, push, issue ni PR | 2026-11-15 | Utilisateur | — |
| D-A (à prendre) | Sort de `army/`, `cahier des charges/`, `.gitignore` modifié : committer, déplacer ou ignorer | échéance 2026-11-22 | Utilisateur | — |
| D-B (à prendre) | Corrections à comportement changeant : v1.6 non cassante + note de migration, ou v2.0 | échéance 2026-12-06 | Utilisateur + architecte | à rédiger (A-011) |
| D-C (à prendre) | Cap du projet pour les 6 prochains mois : fiabilité/maintenance d'abord, ou nouvelles features (Kafka/SQS, multi-tenancy, Aspire distribué) | échéance 2026-12-06 | Utilisateur + product-owner | — |

---

## Décisions prises le 2026-11-15

| Réf. | Décision | Suite donnée |
|---|---|---|
| D-A | Versionner `army/` et les diagnostics ; `cahier des charges/` reste local | PR dédiée `chore/agent-team-diagnostics` |
| D-B | v1.6 non cassante : options opt-in en 1.x, changements de défaut reportés à la v2.0 | Issues #229 (`Send`) et #234 (publisher) cadrées en opt-in |
| D-C | Issues P0/P1 + jalon v1.6 « Confiance » | 20 issues créées, #229 à #248 (jalon n°19 ; #235, #237–#243 et #238 en backlog) |
| A-012 | Nettoyage des branches fusionnées | 27 branches locales supprimées ; 18 conservées car leur sommet diffère de celui de leur PR fusionnée ; origin déjà propre |
| A-001 | Correctifs P0 | PR [#228](https://github.com/rouffou/mediarq/pull/228) ouverte, en attente de CI et de relecture |
| A-002 | Scans de sécurité | CodeQL et Mutation testing réactivés ; `dependabot.yml` dans la PR #228 |

Reste ouvert : issues #230 à #233 (parties non couvertes par la PR #228), #229 (encapsulation dans `Send`).

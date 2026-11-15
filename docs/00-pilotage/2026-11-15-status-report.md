---
titre: Status report — Mediarq — diagnostic au 2026-11-15
auteur: project-manager
statut: en revue
date: 2026-11-15
---

# Status report — Mediarq (diagnostic)

> Premier état des lieux du pilotage. Les **faits** citent une source (fichier:ligne, issue, commande `gh`/`git`). Les **hypothèses** sont marquées `[H]`. Rien n'a été compilé ni exécuté : les chiffres de tests viennent de la documentation du dépôt, pas d'un run.

## Météo globale : 🟡

Livraison fonctionnelle quasi complète et publiée, mais **dépôt dormant depuis 112 jours**, **6 défauts de justesse P0 connus et non suivis dans GitHub**, et **scans de sécurité planifiés désactivés**.

| Axe | Météo | Commentaire |
|---|---|---|
| Périmètre | 🟢 | 150 issues fermées sur 155 ; seules 5 restent ouvertes (#178 « considering » + 4 items « Continuous » #39-#42). Les jalons v1.0 → v1.3 de la ROADMAP sont livrés, le backlog aussi (ROADMAP.md, `gh issue list`). |
| Délai | 🟡 | Dernier commit sur `origin/dev` : 2026-07-26 (`37cb619`, #226) ; dernière release v1.5.1 le même jour. Aucune activité depuis, aucun plan daté pour la suite. 7 releases en 30 jours (v1.0.0 le 2026-06-27 → v1.5.1 le 2026-07-26) : rythme soutenu puis arrêt net. |
| Budget | ⚪ | Non applicable / non documenté. `[H3]` projet open source à ressources non chiffrées (un seul mainteneur : Nicolas Rouffart). |
| Qualité | 🟡 | CI verte sur le dernier commit (ci.yml, push + PR du 2026-07-26). Mais : bugs P0 confirmés dans le code (voir ci-dessous), CodeQL et Mutation testing en `disabled_inactivity`, packages de support peu testés (OpenTelemetry 2 tests, Polly 3, DataAnnotations 3 selon `cahier des charges/analyse-projet-propositions-2026-10.md` §2 #28, non revérifié). |
| Risques | 🔴 | 2 risques de criticité ≥ 16/25 (justesse Outbox/Idempotency en production, sécurité sans scan). Détail dans `raid.md`. |

## Faits clés

**Livraison**
- 31 projets dans `src/` (dont `Mediarq.SourceGenerators`, embarqué dans le cœur), 29 projets de tests dans `Tests/` (listing du dépôt).
- Tags `v1.0.0` à `v1.5.1` (`git tag`). Les 7 versions sont sur NuGet.org pour `Mediarq` et `Mediarq.Core` ; `Mediarq.Analyzers` à partir de 1.3.0 ; `Mediarq.Hangfire`, `.Grpc`, `.Testing` en 1.5.0 et 1.5.1 (API publique `api.nuget.org/v3-flatcontainer`, vérifié le 2026-11-15). Les 6 derniers runs `release.yml` sont en succès.
- Le correctif « no supported framework assets » (#226) est fusionné et livré dans la v1.5.1 (PR #227 mergée le 2026-07-26).
- Aucune PR ouverte (`gh pr list`).

**Dépôt et gouvernance**
- La branche courante `fix/nuget-meta-package-no-framework-assets` est **déjà fusionnée** (PR #226) : elle est obsolète. Le `dev` local (`6082554`) est en retard de 5 commits sur `origin/dev` (`37cb619`).
- 48 branches locales, 13 refs distantes ; `git branch --merged origin/dev` : 15 branches déjà fusionnées, supprimables.
- Working tree : `.gitignore` modifié, `army/` et `cahier des charges/` non suivis, `Mediarq.Samples.WebApi.csproj.user` non suivi (`git status`).
- Aucun `.github/dependabot.yml`, alors que ROADMAP.md:152 affirme « Dependabot + CodeQL kept green ».

**Documentation de pilotage obsolète**
- ROADMAP.md : « v1.0 — Public launch (current) » avec « 🔜 Publish all packages to NuGet » alors que v1.5.1 est publiée. Aucune section v1.4/v1.5.
- RECAP.md (« 9 packages », « 187 tests »), IDEAS.md (« Mise à jour : 2026-06-20 »), next-step.md, etapes-suivantes.md (« 111/111 tests »), vigilance-ameliorations.md : figés en juin 2026, avant l'ajout d'une vingtaine de packages.
- CHANGELOG.md : seulement `[Unreleased]` (l. 7) et deux entrées `[1.0.0]` (l. 313 et l. 474, doublon) ; **aucune section 1.1 à 1.5.1** alors que ces versions sont publiées.
- CONTEXT.md (structure, « 12 projets de tests ») périmé de même.
- `docs/docfx.json` ne référence que 11 projets pour la doc API, sur 31.
- Graphe graphify bâti sur `575c09a1` (GRAPH_REPORT.md, 2026-07-25) ; HEAD = `7ca85d5`. `[H]` les `graphify query` peuvent être légèrement en retard ; `graphify update .` n'a pas été lancé (mission en lecture seule).

**Qualité et CI**
- Workflows actifs : `ci.yml`, `benchmarks.yml`, `docs.yml`, `release.yml`, `release-drafter.yml`. **CodeQL** et **Mutation testing** : `disabled_inactivity` (`gh workflow list --all`) ; dernières exécutions le 2026-09-28.
- Garde-fou benchmark temps + allocations en place (#193, #195).

**Défauts confirmés dans le code (lecture seule)**
- `ParallelNotificationPublisher.cs:15-21` : boucle sans try/catch ; une exception synchrone d'un handler empêche de démarrer les suivants (contredit la doc l.5).
- `OutboxDispatcher.cs:32-34` : `Where ProcessedOnUtc == null … OrderBy(OccurredOnUtc).Take(batchSize)` ; `OutboxDispatcher.cs:55-59` incrémente `Attempts` mais sans limite, sans backoff, sans statut `Failed` : un message empoisonné bloque la file. Aucun champ de verrou/bail dans `Mediarq.Outbox` (grep `LockedUntil|MaxAttempts|SKIP LOCKED` : 0 résultat).
- `IdempotencyBehavior.cs:51` : clé `"mediarq:idem:" + IdempotencyKey`, sans type de requête ni utilisateur/tenant.
- `ServiceCollectionExtensions.cs:241-249` : `Mediator`, `HandlerResolver`, `RequestContextFactory`, `PipelineExecutor` enregistrés en scoped.
- `Mediarq.Core.csproj:33,36` : dépendances à `Microsoft.AspNetCore.Http.Abstractions 2.3.0` et `Scrutor`, ce qui contredit le positionnement « dependency-free » (ROADMAP.md).
- `src/Mediarq.UnitOfWork/UnitOfWorkBehavior.cs:15` : le behavior n'implémente que `IPipelineBehavior` (pas de `IConditionalPipelineBehavior` dans le fichier) : actif pour toute requête.
- Mises en cache des échecs `Result` : aucune occurrence de `IsFailure`/`CacheFailures` dans `Mediarq.Caching` ni `Mediarq.Idempotency` (grep), donc la proposition #7 est cohérente avec le code.

**Backlog non tracé dans GitHub**
- `cahier des charges/analyse-projet-propositions-2026-10.md` (2026-10-24) liste 35 propositions (6 P0). Elles ne figurent **pas** dans les issues ouvertes (seules #39-#42 et #178 le sont). Le travail identifié n'est donc pas planifiable en l'état.
- Ce dossier est non versionné (non suivi par git).

## Réalisé cette période (2026-07-26 → 2026-11-15)
- Aucun commit sur `dev`/`main`, aucune release (fait).
- 2026-10-24 : analyse de l'état du projet et 35 propositions rédigées (`cahier des charges/`, non commitée).
- 2026-11-15 : mise en place de l'organisation d'agents (`army/`, non commitée) et présent diagnostic.

Rappel de la dernière période active (juin → juillet 2026) : v1.0.0 → v1.5.1, une vingtaine de packages d'extension ajoutés (Outbox, Idempotency, Saga, Hangfire, Quartz, Deferred, brokers RabbitMQ/ASB/Dapr/gRPC, Aspire, Authorization, RateLimiting, Testing, HealthChecks), analyzers MQ001-MQ007, MQ100, MQ200-MQ202, MQ204.

## Prévu pour la prochaine période (proposition, à valider)
`[H6]` Rythme supposé : 1 mainteneur à disponibilité partielle.

| Lot | Contenu | Effort (bas – probable – haut) | Dépend de |
|---|---|---|---|
| L0 — Hygiène et visibilité | Réactiver CodeQL et Mutation testing ; ajouter `dependabot.yml` ; créer les issues P0/P1 de l'analyse ; supprimer les branches fusionnées ; synchroniser `dev` local ; décider du sort de `army/` et `cahier des charges/` | 0,5 – 1 – 2 j | Décisions D-A, D-C |
| L1 — Correctifs de justesse P0 | Publisher parallèle, encapsulation `Send`, Outbox (poison + verrou), Idempotency (concurrence + clé) | 6 – 10 – 16 j | L0 ; tests d'intégration Testcontainers en parallèle |
| L2 — Release v1.6.0 « fiabilité » | Cache des échecs `Result`, `UnitOfWorkBehavior` conditionnel, MQ203, couverture, pack automatique (MinVer), CHANGELOG 1.1-1.5.1 rattrapé | 5 – 8 – 12 j | L1 |
| L3 — Docs et feuille de route | ROADMAP v1.4-v1.6, archivage des docs obsolètes, guides DocFX Outbox/Idempotency/Saga/brokers | 3 – 5 – 8 j | L0 (parallélisable avec L1) |
| L4 — v2 (changements cassants) | Publisher séquentiel par défaut, cœur sans Scrutor/AspNetCore 2.3, API de configuration fluide | 8 – 12 – 20 j | Décision D-B (ADR) |

Dépendances inter-agents : architecte logiciel / solution pour D-B ; QA pour Testcontainers ; DevOps pour CI, MinVer, workflows ; rédacteur technique pour L3 ; product-owner pour la priorisation du backlog (hors périmètre du chef de projet).

## Points d'attention / besoins de décision
| Sujet | Décision attendue | De qui | Avant le |
|---|---|---|---|
| Bugs de justesse P0 (Outbox, Idempotency, publisher) | Valider qu'ils passent avant toute nouvelle feature et autoriser la création des issues correspondantes (aucune créée dans cette passe) | Utilisateur / mainteneur | 2026-11-22 |
| Scans CodeQL + Mutation testing désactivés ; pas de Dependabot | Autoriser la réactivation des workflows et l'ajout de `dependabot.yml` | Utilisateur / mainteneur | 2026-11-22 |
| `army/` et `cahier des charges/` non versionnés | Les committer, les déplacer, ou les ignorer ; supprimer la branche `fix/nuget-...` désormais inutile | Utilisateur | 2026-11-22 |
| Changements de comportement (publisher séquentiel, retrait Scrutor/AspNetCore 2.3) | Choisir v1.6 non cassante avec note de migration, ou v2.0 ; ADR à rédiger | Utilisateur + architecte | 2026-12-06 |
| Cap du projet | Maintenance + fiabilité, ou nouvelles features (Kafka/SQS, multi-tenancy) ; sinon le projet reste en veille | Utilisateur / product-owner | 2026-12-06 |

## Top 3 risques
1. **R-001 : perte ou blocage de messages Outbox, doublons Idempotency en production** (criticité 20) : défauts confirmés dans le code, packages publiés sur NuGet (1.5.1).
2. **R-002 : aucun scan de sécurité actif, pas de Dependabot** (criticité 16) : CodeQL en `disabled_inactivity`, aucune configuration Dependabot.
3. **R-003 : dépôt dormant et dette documentaire** (criticité 15) : 112 jours sans commit, ROADMAP/CHANGELOG faux, perte de confiance des utilisateurs et de contexte.

Détail et plans de mitigation : [`raid.md`](raid.md).

## Hypothèses
- `[H1]` Le mainteneur unique est Nicolas Rouffart (auteur NuGet et `git config`) ; `gh pr list --state all` ne montre que des PR du dépôt lui-même.
- `[H2]` Les 565 tests et 12 diagnostics cités par l'analyse du 2026-10-24 sont exacts ; non revérifiés (aucune exécution).
- `[H3]` Le budget n'est pas un facteur (projet open source gratuit).
- `[H4]` L'adoption réelle (téléchargements NuGet, issues externes) n'a pas été mesurée : la gravité des défauts P0 côté utilisateurs est donc inconnue.
- `[H5]` Le comportement de `Mediator.Send` sur les exceptions asynchrones (proposition #2 de l'analyse) n'a pas été revérifié : seule la présence du `throw new InvalidOperationException` en `Mediator.cs:118` est constatée.
- `[H6]` Les estimations en jours sont des ordres de grandeur issus de l'effort S/M/L de l'analyse (S ≈ 0,5-1 j, M ≈ 2-4 j, L ≈ 5+ j), pas des estimations d'agents techniques.

## Sources
ROADMAP.md, CHANGELOG.md, RECAP.md, CONTEXT.md, next-step.md, etapes-suivantes.md, IDEAS.md, vigilance-ameliorations.md, `cahier des charges/*.md` (3 fichiers), `git log`/`git tag`/`git status`/`git branch`, `gh issue list`, `gh api milestones`, `gh pr list`, `gh run list`, `gh workflow list --all`, `graphify query`, code des fichiers cités, API NuGet publique.

---

## Mise à jour après correctifs (2026-11-15, branche `fix/reliability-p0`)

| Axe | Avant | Après | Justification |
|---|---|---|---|
| Périmètre | 🟢 | 🟢 | Inchangé. |
| Délai | 🟡 | 🟡 | Le dépôt reste dormant depuis le 2026-07-26 et aucun plan daté n'est validé : dépend des décisions D-A, D-B, D-C. |
| Qualité | 🟡 | 🟢 *sous réserve de la CI de la PR* | 4 défauts P0 corrigés avec tests (Outbox, Idempotency, publisher parallèle) ; Outbox 21 tests, Idempotency 13, Core 213 verts en local ; CodeQL et Mutation testing réactivés ; `dependabot.yml` ajouté. Passe au vert seulement quand la CI de la PR est verte. |
| Risques | 🔴 | 🟡 | R-001 réduit mais pas éteint (voir ci-dessous) ; R-002 traité (scans réactivés, Dependabot) — le premier run CodeQL n'a pas encore eu lieu. |

### Corrigé
- `ParallelNotificationPublisher` : un handler qui lève avant de rendre sa `Task` n'empêche plus les autres de démarrer.
- Outbox : `MaxAttempts` (10 par défaut, au-delà le message est laissé de côté avec son erreur) ; claim optimiste par message via `Attempts` en jeton de concurrence (pas de changement de schéma), sauvegarde message par message, tentative non consommée sur annulation.
- Idempotency : clé cloisonnée par type de requête, `Result` en échec jamais stocké, appels concurrents de même clé sérialisés dans le processus.
- README / `docs/index.md` / `versioning.md` / description du package : retrait de « dependency-free » et de « drop-in », liste complète des 31 packages dans `docs/index.md`.

### Non corrigé (et pourquoi)
- `Mediator.Send` n'encapsule que les exceptions synchrones : l'homogénéiser change le type d'exception pour les appelants existants (changement de comportement, décision D-B).
- Outbox : pas de backoff ni de statut `Failed` explicite, car cela demande une colonne en base (migration pour les utilisateurs existants).
- Idempotency : entre plusieurs instances le contrôle reste best-effort (`IDistributedCache` n'a pas de set-if-absent atomique).
- `docs/docfx.json` couvre 11 projets sur 31 : non modifié, car le build DocFX n'a pas pu être vérifié en local.
- CHANGELOG, ROADMAP, CONTEXT.md et les notes de racine sont ignorés par git : ils ne peuvent pas être corrigés par une PR.
- Les 35 propositions d'octobre n'ont toujours pas d'issues ; branches fusionnées non nettoyées.

---
titre: Diagnostic produit — Mediarq (vision, backlog, trous fonctionnels, MVP de la prochaine étape)
auteur: product-owner
statut: brouillon
date: 2026-11-15
---

# Diagnostic produit — Mediarq

> Légende : **[F]** = fait vérifié (source indiquée) · **[H]** = hypothèse (non vérifiée, à valider) · **[R]** = recommandation du PO.
> Périmètre : lecture seule. Rien n'a été compilé ni exécuté ; aucune issue/PR n'a été touchée.
> Les chiffres NuGet/GitHub sont ceux lus le 2026-11-15 (API NuGet, `gh`) ; ils bougent.

## 0. Synthèse en 10 lignes

1. **Le produit est fonctionnellement très en avance sur son marché réel** : 31 packages publiés (v1.5.1), parité MediatR documentée, 565 tests, 4 jalons de ROADMAP clos. **[F]**
2. **Le marché, lui, n'a pas encore répondu** : 0 étoile, 0 fork, 0 watcher, téléchargements NuGet quasi uniformes entre packages (≈ 260 à 3 800), signature d'un trafic de miroirs/scanners plutôt que d'usage. **[F]** pour les chiffres, **[H]** pour l'interprétation.
3. **Le problème n°1 n'est plus le manque de fonctionnalités mais la confiance** : les briques « fiabilité » (Outbox, Idempotency), qui sont le différenciateur face à MediatR, ont des défauts de correction connus et vérifiés dans le code (§4.3). **[F]**
4. **La promesse « dependency-free » est fausse telle qu'écrite** : `Mediarq.Core` référence Scrutor et `Microsoft.AspNetCore.Http.Abstractions 2.3.0`. **[F]**
5. **Le backlog GitHub est un miroir rétroactif des docs, pas un outil de pilotage** : 150 issues sur 155 sont fermées, 153 ont été créées en juillet 2026 en bloc, et les 35 propositions de l'analyse du 2026-10-24 n'ont aucune issue. **[F]**
6. **Le dépôt est inactif depuis le 2026-07-26** (dernier push) alors que 6 bugs P0 sont identifiés ; seules des exécutions CI planifiées tournent. **[F]**
7. **Risque de dispersion** : 31 packages pour un produit à 0 utilisateur identifié ; chaque package est une dette de maintenance (versions, README, docs, packaging déjà cassé une fois : #164, v1.5.1). **[F]/[H]**
8. **Tension de gouvernance** : la politique SemVer du projet classe tout changement de comportement en MAJOR ; or deux des corrections P0/P1 (#2, #8) changent un comportement. **[F]** → arbitrage requis (§7).
9. **Recommandation** : la prochaine étape est une **release « Confiance » (v1.6)**, pas de nouvelles fonctionnalités : corriger les P0, rendre le backlog pilotable, nettoyer les promesses (docs/README), puis lancer l'adoption (§6).
10. **Ne pas ouvrir de nouveau package avant ces trois chantiers** (Kafka/SQS, multi-tenancy, feature flags : Won't pour l'étape).

---

## 1. Sources consultées

| Source | Usage |
|---|---|
| `README.md`, `docs/index.md`, `docs/guides/*` (11 guides) | Promesse produit, positionnement, cohérence documentaire |
| `ROADMAP.md`, `IDEAS.md`, `CHANGELOG.md`, `RECAP.md`, `CONTEXT.md`, `etapes-suivantes.md`, `next-step.md`, `vigilance-ameliorations.md` | Historique de la vision et des décisions |
| `cahier des charges/` (3 notes : 2026-07-24 perf, 2026-07-25 pistes, 2026-10-24 analyse + 35 propositions) | Backlog « réel » non tracé dans GitHub — **non versionné** (fichier non suivi par git) |
| GitHub (`gh issue list`, `gh api …/milestones`, `gh release list`, `gh pr list`, `gh run list`) | Backlog, jalons, cadence |
| API NuGet (recherche « Mediarq ») | Publication et téléchargements |
| Code : `ParallelNotificationPublisher.cs`, `Mediator.cs`, `OutboxProcessor.cs`, `OutboxDispatcher.cs`, `IdempotencyBehavior.cs`, `Mediarq.Core.csproj`, comptage des tests | Vérification de 6 affirmations de l'analyse d'octobre |
| Recherche web (licence MediatR) | Contexte concurrentiel |

Limite : l'index `graphify-out/` est périmé (construit sur `575c09a1`, HEAD `7ca85d5`) **[F, cahier des charges 2026-10-24 §1]** ; il n'a servi qu'à confirmer la structure doc, les faits de code ont été relus directement.

---

## 2. Vision et positionnement

### 2.1 Ce que le produit dit être
- Vision déclarée : « lightweight, dependency-free, free CQRS mediator for .NET — a drop-in alternative to MediatR (now commercial) with first-class trimming/Native AOT support » **[F, ROADMAP.md L6-7, README.md L13-15]**.
- Trois piliers de différenciation : (a) gratuit/MIT, (b) `Result` railway-oriented + validation intégrés, (c) dispatch sans réflexion / AOT via source generator **[F, README.md « Why Mediarq? »]**.
- Un quatrième pilier est apparu **sans avoir été décidé comme positionnement** : l'écosystème « fiabilité et intégration » (Outbox, Idempotency, Saga, brokers RabbitMQ/ASB/Dapr/gRPC, Hangfire/Quartz, Aspire…). Il représente la majorité des 31 packages de `src/` **[F, `src/`, comptage approximatif non détaillé]**. Le positionnement ROADMAP ne le mentionne pas.

### 2.2 Cohérence vision ↔ réalité

| Promesse | Réalité | Écart |
|---|---|---|
| « dependency-free » | `Mediarq.Core` → Scrutor 6.1.0, `Microsoft.AspNetCore.Http.Abstractions 2.3.0` (obsolète, uniquement pour `HttpUserContext`), + abstractions `Microsoft.Extensions.*` **[F, Mediarq.Core.csproj L33-36]** | **Faux** : à corriger (code = analyse #14, ou message = « minimal dependencies ») |
| « Drop-in alternative / drop-in-shaped » | Signature de `IPipelineBehavior` différente, `IRequest<T>` éclaté en `ICommand`/`IQuery`, pas de `Send(object)`, publisher parallèle par défaut (MediatR : séquentiel) **[F, mediatr-parity.md L13-18, L35-40 ; analyse 2026-10 #8]** | **Partiel** : « migration assistée » (analyzer + shim) est plus juste que « drop-in » |
| « No licensing risk — no revenue threshold » vs MediatR | MediatR ≥ 13 est sous licence commerciale ; sources divergentes sur une tranche gratuite (< 5 M$ de CA) ; les versions ≤ 12 restent open source **[F, recherche web : [FAQ Lucky Penny](https://luckypennysoftware.com/faq), [article 2026-09](https://umamaheswaran.com/2026/09/09/mediatr-commercial-license-alternatives-migration/)]** | L'argument est solide mais **doit être nuancé** : la cible réelle est les équipes au-dessus du seuil gratuit ou refusant la dépendance à un éditeur ; la concurrence « gratuite » inclut aussi `Mediator` (source-gen) et l'épinglage de MediatR 12 **[H]** |
| `Mediarq` = « lean meta-package » | README : méta-package ; `docs/index.md` : « Core mediator, pipeline, results, source generator » (donc le core) ; `docs/index.md` liste 15 packages sur 31 **[F]** | **Incohérence documentaire** (site DocFX en retard) |
| « All packages are at Version 1.0.0, API frozen » | NuGet publie 1.0.0 → 1.5.1 ; `<Version>1.0.0</Version>` figé dans les csproj, version injectée au tag **[F, versioning.md L14 ; Mediarq.Core.csproj L16 ; API NuGet]** | **Incohérence** ; les guides ne disent pas la vérité sur le versionnage |

### 2.3 Personas déduits (à valider — aucun retour utilisateur n'existe)

| Persona | Besoin / JTBD | Ce que Mediarq lui offre | Maturité de l'offre |
|---|---|---|---|
| **P1 — Migrant MediatR** : dev/lead .NET d'une équipe qui doit sortir de MediatR ≥ 13 (licence, politique d'achat) | « Remplacer MediatR sans réécrire 300 handlers ni perdre de comportement » | Analyzer `MQ100` + code fix, `MediatRCompat`, guide de migration, audit de parité | **Forte** (parcours le mieux outillé) — mais manque de preuve sociale et de comparatif perf honnête |
| **P2 — Greenfield DDD/CQRS** : architecte d'une API ASP.NET Core neuve | « Un médiateur avec `Result`, validation et HTTP mapping prêts à l'emploi » | `Result`, `MapMediarq()`, template `mediarq-webapi`, sample WebApi | **Forte** sur le papier ; à confronter à Wolverine.HTTP / FastEndpoints **[H]** |
| **P3 — Plateforme / microservices** : équipe qui veut outbox, idempotence, saga, brokers sans framework de messagerie lourd | « Fiabilité de bout en bout sans MassTransit/Wolverine » | Outbox, Idempotency, Saga, bridges brokers | **Fragile** : défauts de correction connus (§4.3), pas d'inbox, saga en mémoire seulement |
| **P4 — AOT / perf** : équipe cible serverless/AOT | « Zéro réflexion, zéro warning trim » | Source generator, `IsAotCompatible`, CI AOT runtime | **Forte** — mais `MapMediarq` et l'Outbox restent réflexifs **[F, analyse 2026-10 #9, #16]** |

---

## 3. Backlog : cohérence et priorisation

### 3.1 Faits mesurés (GitHub, 2026-11-15)

| Mesure | Valeur | Source |
|---|---|---|
| Issues totales / ouvertes / fermées | 155 / **5** / 150 | `gh issue list --state all` |
| Dont issues « backlog-import » | 130 (conforme à la mémoire projet) + 25 créées après | labels |
| Issues créées en 2026-07 / 2025-11 | **153 / 2** | `createdAt` |
| Issues fermées en 2026-07 | **148 sur 150** | `closedAt` |
| Issues ouvertes | #178 (réduire le graphe scoped, « considering ») + 4 marqueurs « ongoing » (#39-#42, jalon « Continuous ») | `gh issue list --state open` |
| Jalons | 18 : **16 fermés**, 2 ouverts (« Backlog / considering », « Continuous ») | `gh api …/milestones` |
| Jalons « historiques » (docs → jalons) | 13 jalons « Idées / Next Steps / Étapes / Vigilance » = 82 issues, toutes fermées | milestones 7-18 |
| Pull requests | 72 (dernière : #227 « Release v1.5.1 », 2026-07-26) | `gh pr list` |
| Dernier push | 2026-07-26 | `gh repo view` |
| Releases | v1.0.0 (2026-06-27), v1.1.0→v1.4.0 (2026-07-24), v1.5.0 (07-25), v1.5.1 (07-26) | `gh release list` |
| Étoiles / forks / watchers | 0 / 0 / 0 | `gh repo view` |

### 3.2 Lecture produit (faits + analyse)

1. **Le backlog GitHub ne pilote rien.** Il a été reconstruit à partir de fichiers Markdown (IDEAS, étapes, vigilance), puis fermé au fil de l'eau. 98 % des issues sont des **traces de livraison**, pas des demandes. Aucune n'est issue d'un utilisateur externe **[F]**.
2. **Le vrai backlog est hors GitHub et hors git** : `cahier des charges/analyse-projet-propositions-2026-10.md` (35 propositions priorisées P0/P1/P2) est un fichier non suivi ; il n'existe qu'**une** issue ouverte de contenu (#178) **[F]**. Conséquence : six bugs P0 n'ont ni ticket, ni propriétaire, ni jalon.
3. **Les jalons ne reflètent plus la structure du produit.** Les 4 jalons « produit » (v1.0→v1.3) sont clos ; les livraisons v1.4/v1.5 (≈ 25 issues « Backlog / considering ») n'ont pas de jalon de version. 13 jalons « docs » créent du bruit **[F]**.
4. **Priorisation implicite = « ce qui est faisable et visible »**, pas « ce qui crée de la confiance/adoption ». Preuve : IDEAS/ROADMAP privilégient les nouveaux packages (Dapr, Aspire, Grpc, Azure SB, RabbitMQ en 4 jours, 2026-07-24 → 07-26) **avant** de traiter l'outbox poison-message, l'idempotence concurrente ou l'inbox, pourtant identifiés dans `pistes-amelioration-librairie.md` et l'analyse d'octobre **[F, dates de tags et PR]**.
5. **Items ROADMAP obsolètes ou trompeurs** : « v1.0 — Public launch (current) » avec 3 items « 🔜 » (publier sur NuGet, vérifier le site, annonce) alors que v1.5.1 est publié sur NuGet **[F, ROADMAP.md L13-22 vs API NuGet]** ; `IDEAS.md` (mise à jour 2026-06-20), `RECAP.md`, `next-step.md`, `etapes-suivantes.md`, `vigilance-ameliorations.md` décrivent un projet à 9 packages / 111-187 tests **[F]**. Le lecteur ne peut pas distinguer l'état courant. Pas d'item « annonce / adoption » exécuté ni suivi **[F : aucune issue ni PR de communication]**.
6. **Cadence** : 148 issues fermées en un mois puis arrêt. Le modèle de pilotage « une branche + un commit + une issue » a produit un volume élevé mais n'a pas de mécanisme d'entrée de besoins (ni template d'issue utilisateur, ni discussions, ni label « user-request ») **[F pour l'absence d'étoiles/forks ; H pour les templates — non vérifié]**.

### 3.3 Verdict backlog

| Critère | Note | Justification |
|---|---|---|
| Traçabilité | 2/5 | Livraisons tracées, besoins/bugs non tracés |
| Priorisation | 2/5 | Pas de critère explicite ; P0 hors outil |
| Alignement vision | 3/5 | Parité MediatR et AOT très bien servis ; « dependency-free » et fiabilité non |
| Valeur utilisateur mesurée | 1/5 | Aucun signal d'usage, aucun feedback |
| Hygiène (docs/jalons) | 2/5 | Docs périmés, jalons historiques, ROADMAP en retard |

---

## 4. Trous fonctionnels et risques de qualité

### 4.1 Parité avec MediatR (concurrent de référence)

**[F]** L'audit `docs/guides/mediatr-parity.md` est exhaustif et honnête. Seuls manques déclarés : `Send/Publish/CreateStream(object)` (réflexion, choix AOT assumé) et `IRequestExceptionAction` (observation seule).

Frictions de migration restantes **[F, mediatr-parity.md + analyse 2026-10]** :

| Friction | Impact migrant (P1) | Réponse actuelle | Gap |
|---|---|---|---|
| Signature de `IPipelineBehavior` différente (`IMutableRequestContext` + `Func<Task>`) | Corps de chaque behavior à réécrire à la main | Analyzer réécrit l'interface, pas le corps | **Moyen** : automatiser le corps (code fix `next` → `handle`) ou fournir un adaptateur de signature MediatR-like dans `MediatRCompat` **[R]** |
| `IRequest<T>` scindé en `ICommand<T>`/`IQuery<T>` | Choix à faire à chaque requête | Code fix propose les deux | Faible |
| Publisher parallèle par défaut (MediatR séquentiel) | « A second operation was started on this context » avec `DbContext` partagé | Non documenté dans le guide de migration | **Élevé** (piège silencieux) → au minimum documenter **[F analyse #8]** |
| `Publish(object)` / `Send(object)` absents | Code générique/dynamique à contourner | Documenté | Faible, assumé |
| Exceptions de `Send` encapsulées en `InvalidOperationException` si synchrones, brutes si asynchrones | Catch côté appelant fragile, différent de MediatR | Non documenté | **Élevé** (§4.3) |

### 4.2 Comparatif avec le reste du marché **[H, à vérifier par recherche dédiée]**

> Cette section repose sur la connaissance générale de l'écosystème .NET ; seule la licence MediatR a été re-vérifiée (§2.2).

| Concurrent | Positionnement | Ce que Mediarq a / n'a pas |
|---|---|---|
| **Mediator** (martinothamar, MIT, source-gen) | Le « vrai » concurrent en performance **[F, analyse 2026-10 #30]** | Mediarq a Result/validation/analyzers/écosystème ; **pas de benchmark publié contre lui** |
| **Wolverine** (+ Marten) | Médiateur + messaging durable + outbox + sagas + HTTP | Mediarq couvre les mêmes thèmes avec des briques plus légères mais **moins matures** (outbox sans verrou multi-instance, saga en mémoire) |
| **MassTransit** | Messaging, sagas, outbox | Mediarq propose un bridge `Mediarq.MassTransit` et des bridges directs ; pas de saga persistante (EF) |
| **Brighter / FastEndpoints / Immediate.Handlers** | Commandes + dispatcher / endpoints / source-gen | Pas de comparatif |
| **« Plain service classes »** | Option zéro dépendance | Argument à adresser dans le README (pourquoi un médiateur ?) |

### 4.3 Trous de **correction** (plus graves que les trous de fonctionnalités)

Vérifiés dans le code le 2026-11-15 :

| # (analyse 2026-10) | Défaut | Vérification | Sévérité produit |
|---|---|---|---|
| 1 | `ParallelNotificationPublisher` démarre les handlers dans une boucle sans try/catch : une exception **synchrone** d'un handler empêche les suivants de démarrer, contredisant sa propre doc (« Every handler is started ») | **[F, `ParallelNotificationPublisher.cs` L14-23]** | P0 — bug de contrat |
| 2 | `Mediator.Send` : le `try/catch` n'enveloppe que la partie synchrone ; `InvalidOperationException` si levée synchrone, exception brute si asynchrone | **[F, `Mediator.cs` L96-119]** | P0/P1 — **changement de comportement = MAJOR** (§7) |
| 3 | Outbox : compteur `Attempts` incrémenté (L57) mais **aucune limite, aucun backoff, aucun statut Failed** ; lecture `OrderBy(OccurredOnUtc).Take(batchSize)` → N messages empoisonnés bloquent la file | **[F, `OutboxDispatcher.cs` L33-34, L57 ; absence de Max/Backoff]** | P0 |
| 4 | Outbox : pas de verrou/bail (plusieurs instances → doublons) | **[F par absence : aucun `LockedUntil`/lock dans `src/Mediarq.Outbox`]** | P0 |
| 5 | Idempotency : pas de marqueur « en cours » → deux exécutions concurrentes avec la même clé exécutent toutes deux le handler | **[F, `IdempotencyBehavior.cs` L51-65 : get → handle → set non atomique]** | P0 |
| 6 | Idempotency : clé `"mediarq:idem:" + IdempotencyKey`, sans type de requête ni utilisateur/tenant → collision/fuite possible | **[F, `IdempotencyBehavior.cs` L51]** | P0 (sécurité) |
| 7 | Les réponses `Result` en échec sont mises en cache (Caching, Idempotency) | **[F partiel : `IdempotencyBehavior` stocke toute réponse non nulle, sans test `IsFailure`]** ; Caching non relu | P1 |
| 28 | Couverture faible : OpenTelemetry (2 tests), DataAnnotations (3), Polly (3), MassTransit (4), UnitOfWork (4) | **[F, comptage `[Fact]/[Theory]`]** | P1 |

Lecture produit : **les trois briques qui justifient P3 (Outbox, Idempotency, Saga) sont publiées en 1.x stable avec des défauts de fiabilité connus.** Pour une bibliothèque dont la valeur est « ne pas perdre ni dupliquer de messages », c'est un risque réputationnel supérieur à n'importe quel manque de fonctionnalité. Le jeu de tests (565) ne les détecte pas parce qu'il n'exécute pas de scénario multi-instances / concurrent (pas de Testcontainers, #29) **[F + H]**.

### 4.4 Trous de fonctionnalités (par priorité de valeur)

| Trou | Persona | Source | Verdict |
|---|---|---|---|
| Inbox (déduplication côté consommateur des bridges) | P3 | #19 | Complète l'offre brokers ; nécessite d'abord #5/#6 |
| Propagation CorrelationId / traceparent W3C dans les enveloppes | P3 | #21 | Différenciateur observabilité ; cohérent avec `Mediarq.Diagnostics` |
| Saga persistante (EF) + concurrence optimiste | P3 | #18 | Sans elle, `Mediarq.Saga` est une démo (store mémoire) |
| Invalidation de cache + `HybridCache` | P2 | #17 | Bon levier DX, M |
| `MapMediarq` généré (AOT) + groupes/auth/versioning | P2, P4 | #16, #25 | #16 corrige une incohérence de promesse AOT |
| Analyzer MQ203 (handlers dupliqués) | P1, P2 | #26 | Numéro réservé jamais livré ; S |
| Ponts Kafka / SQS-SNS | P3 | #23 | Reporter (aucun signal de demande) |
| Multi-tenancy, feature flags, audit | P2/P3 | #22, #24 | Reporter |

---

## 5. PRD synthétique de la prochaine étape (template `prd.md`)

### 5.1 Problème
Un développeur .NET qui évalue Mediarq comme alternative à MediatR trouve un produit riche (31 packages) mais **sans preuve d'usage ni garantie de fiabilité** : briques de fiabilité aux défauts connus, promesses de README inexactes (« dependency-free », « drop-in »), docs/ROADMAP incohérents avec l'état réel, aucun comparatif de performance crédible, aucun canal d'entrée du feedback. Preuve : 0 étoile/0 fork **[F]**, 6 bugs P0 vérifiés **[F]**, ROADMAP « lancement en cours » alors que 1.5.1 est publié **[F]**.

### 5.2 Objectifs et indicateurs de succès

| Objectif | Indicateur | Valeur actuelle | Cible (proposée **[H]**, à valider) |
|---|---|---|---|
| O1. Rétablir la confiance dans les briques de fiabilité | Bugs P0 ouverts ; scénarios concurrents testés (multi-instances, concurrence idempotente) | 6 P0 ouverts ; 0 test d'intégration concurrent | 0 P0 ouvert ; ≥ 4 scénarios Testcontainers en CI |
| O2. Rendre la promesse produit exacte | Écarts promesse ↔ réalité listés en §2.2 | 5 écarts | 0 écart (ou promesse reformulée) |
| O3. Rendre le backlog pilotable | % du backlog « actif » tracé dans GitHub avec jalon + priorité ; docs obsolètes archivés | ~3 % (1 issue ouverte de contenu) ; 6 docs obsolètes | 100 % ; 0 doc obsolète à la racine |
| O4. Amorcer l'adoption (P1 en priorité) | Étoiles, téléchargements `Mediarq.Core` hors miroirs, issues/discussions externes, projets publics migrés | 0 ; ≈ 3 800 cumulés (non qualifiés) ; 0 ; 0 | À fixer avec le sponsor (ex. 100 étoiles, 5 retours externes, 1 migration publique à 90 jours) |
| O5. Rendre la perf démontrable | Comparatif BenchmarkDotNet publié vs MediatR **et** Mediator, un scope par requête | Comparatif vs MediatR seul, scope unique | Rapport reproductible en CI, publié dans README/docs |

### 5.3 Périmètre — MoSCoW (étape « v1.6 Confiance »)

**Must (MVP de l'étape)** — critère : sans cela le produit ne tient pas sa promesse ou met un utilisateur en danger.

| ID | Élément | Réf. | Pourquoi Must | Effort |
|---|---|---|---|---|
| M1 | Corriger `ParallelNotificationPublisher` (démarrer tous les handlers, capturer les exceptions synchrones) | #1 | Bug de contrat documenté ; correctif non cassant | S |
| M2 | Outbox : `MaxAttempts`, backoff exponentiel (`NextAttemptAtUtc`), statut `Failed` (dead-letter) | #3 | Un message empoisonné arrête toute la file | M |
| M3 | Outbox : réservation atomique (bail/lock) pour multi-instances | #4 | Doublons en prod à ≥ 2 pods | M |
| M4 | Idempotency : marqueur « en cours » atomique + réponse définie (attente ou conflit) | #5 | Garantie centrale de la brique non tenue | M |
| M5 | Idempotency : clé cloisonnée par type de requête (+ utilisateur/tenant) | #6 | Fuite de données possible ; **attention : change les clés déjà stockées** (migration/expiration à documenter) | S |
| M6 | Caching/Idempotency : ne pas mettre en cache les `Result` en échec (option `CacheFailures`, défaut `false`) | #7 | Une erreur transitoire rejouée 24 h ; **comportement par défaut modifié** (§7) | S |
| M7 | Tests d'intégration Testcontainers (Outbox 2 processeurs, Idempotency concurrente) | #29 | Seule façon de **prouver** M2-M5 | M |
| M8 | Documenter dans le guide de migration : publisher parallèle par défaut, encapsulation d'exceptions de `Send` | #8, #2 | Piège silencieux pour P1 ; zéro risque de compatibilité | S |
| M9 | Rendre le backlog pilotable : créer les issues P0/P1 issues de l'analyse d'octobre, jalon v1.6, archiver les docs périmés (`docs/archive/`), mettre à jour ROADMAP (v1.4/v1.5 + lancement), versionner ou ignorer `cahier des charges/` | #33, #35 | Condition pour que la suite soit gouvernable | S |
| M10 | Corriger les promesses publiques : « dependency-free » (reformuler **ou** réaliser #14), `docs/index.md` (31 packages, rôle réel de `Mediarq`), `versioning.md` (« All packages at 1.0.0 ») | §2.2 | Confiance et exactitude | S |

**Should**

| ID | Élément | Réf. | Raison |
|---|---|---|---|
| S1 | `UnitOfWorkBehavior` : `IsActive` statique (chemin rapide) + audit des autres behaviors | #10 | Perf non nécessaire au bon fonctionnement mais contredit la promesse « lean » ; S |
| S2 | Analyzer MQ203 (handlers dupliqués) | #26 | Numéro réservé jamais livré ; erreur d'exécution → erreur de compilation ; S |
| S3 | Pack/versionnage automatisés (`dotnet pack` sur la solution, MinVer) | #31 | Un package déjà oublié (#164), v1.5.1 corrige un défaut de packaging ; S |
| S4 | Renforcer les tests des packages sous-couverts (OTel, Polly, DataAnnotations, MassTransit, UnitOfWork) | #28 | Dette de confiance ; M |
| S5 | Benchmarks vs `Mediator` (source-gen) et un scope par requête, publiés honnêtement | #30, #11 | Nécessaire à O5 ; sans quoi l'argument perf n'est pas crédible ; S–M |
| S6 | Guides DocFX pour Outbox, Idempotency, Saga, brokers, `MapMediarq` | #32 | Les briques qui font la différence ne sont documentées que par README de package ; M |
| S7 | Décider et réaliser le retrait de Scrutor / `Http.Abstractions 2.3.0` du Core | #14 | Rend « dependency-free » vrai ; **MAJOR potentiel** (§7) ; M |
| S8 | Volet adoption : article « migrer de MediatR avec Mediarq » (cas réel), comparatif, GIF quickstart, annonce | ROADMAP v1.0 items « 🔜 » | Le seul levier pour P1 ; faible effort, effet inconnu |

**Could**

| ID | Élément | Réf. |
|---|---|---|
| C1 | Inbox côté consommateur | #19 (après M4/M5) |
| C2 | Propagation de contexte (correlation, `traceparent`) | #21 |
| C3 | Saga persistante EF + concurrence optimiste + timeouts | #18 |
| C4 | Invalidation de cache + `HybridCache` | #17 |
| C5 | `MapMediarq` généré à la compilation (AOT) | #16 |
| C6 | Rétention + réveil de l'Outbox | #20 |
| C7 | Cache « aucun validateur » pour `Publish`, injection du cache de behaviors | #12, #13 |
| C8 | Option fluente de configuration `AddMediarq(o => …)` | #15 (avec #14, v2) |
| C9 | Analyzer MQ205 | #27 |

**Won't (pour l'étape)** — décision explicite, à réexaminer après les premiers retours externes :
Kafka / SQS-SNS (#23), multi-tenancy native (#22), feature flags et audit (#24), `MapMediarq` groupes/auth/versioning (#25), sample distribué Aspire (#34), graphe scoped → singleton (#178/#11) tant que le coût n'est pas mesuré sur un scénario « un scope par requête » (S5).

### 5.4 Priorisation valeur / effort (hors MoSCoW : pour ordonner à l'intérieur d'une catégorie)

Échelle 1-5 ; score = valeur × confiance (0,5-1) / effort. **[H]** : les notes sont des jugements du PO, à challenger.

| Élément | Valeur | Confiance | Effort | Score | Rang |
|---|---:|---:|---:|---:|---:|
| M1 Publisher parallèle | 4 | 1,0 | 1 | 4,0 | 1 |
| M8 Doc migration (pièges) | 4 | 1,0 | 1 | 4,0 | 1 |
| M10 Promesses publiques | 4 | 1,0 | 1 | 4,0 | 1 |
| M5 Clé idempotence cloisonnée | 5 | 0,9 | 1 | 4,5 | — (priorité sécurité) |
| M9 Pilotage / docs | 3 | 1,0 | 1 | 3,0 | 4 |
| M6 `CacheFailures` | 3 | 0,9 | 1 | 2,7 | 5 |
| S1 / S2 / S3 | 3 | 0,8 | 1 | 2,4 | 6 |
| M2 Outbox poison message | 5 | 0,9 | 3 | 1,5 | 7 |
| M4 Idempotence concurrente | 5 | 0,8 | 3 | 1,3 | 8 |
| M3 Outbox multi-instances | 4 | 0,8 | 3 | 1,1 | 9 |
| M7 Testcontainers | 5 | 0,9 | 3 | 1,5 | (condition de M2-M4) |
| S8 Adoption | 5 | 0,3 | 2 | 0,75 | à lancer en parallèle (incertain mais décisif) |
| S5 Benchmarks vs Mediator | 3 | 0,7 | 2 | 1,05 | 10 |
| S7 / #14 Retrait des dépendances du Core | 3 | 0,7 | 3 | 0,7 | 11 (décision v2) |

### 5.5 Parcours utilisateurs clés à protéger (smoke tests produit)
1. **P1 — Migration** : `dotnet add package Mediarq` → analyzer `MQ100` → code fix → démarrage de l'app → tests du consommateur verts (inclut publisher et exceptions).
2. **P2 — Greenfield** : `dotnet new mediarq-webapi` → `dotnet run` → un POST invalide renvoie un ProblemDetails de validation.
3. **P3 — Fiabilité** : commande transactionnelle → message en outbox → 2 processeurs → exactement une publication ; une même clé d'idempotence envoyée deux fois en parallèle → un seul effet.
4. **P4 — AOT** : `AddMediarqCore()` + `AddMediarqHandlers()` → publish AOT → exécution du binaire (déjà en CI **[F, ROADMAP « Continuous »]**).

Chaque parcours doit devenir un test automatisé ou un job CI (standard « critère d'acceptation = test automatisable », `ROLE.md`).

### 5.6 Exigences non fonctionnelles perçues
- **Performance** : parité ou mieux sur le pipeline profond (déjà : 1,00 ×, moins d'allocations **[F, analyse perf 2026-07-24]**) ; dispatch nu à re-mesurer après #175/#177 (valeur actuelle inconnue **[H]**) ; coût « un scope par requête » à mesurer.
- **Fiabilité** : delivery at-least-once **sans blocage de file** et sans doublon multi-instance pour l'Outbox ; au plus un effet par clé pour l'idempotence.
- **Compatibilité** : SemVer strict ; API publique figée (`PublicAPI.*.txt`).
- **Compatibilité plateforme** : net8/9/10, Native AOT, 0 warning trim.
- **Sécurité** : isolation des clés d'idempotence/cache par type et utilisateur/tenant.
- Accessibilité / RGPD / localisation : sans objet pour une bibliothèque (hors messages de validation localisables **[F, `IValidationMessageResolver`]**).

---

## 6. Plan proposé pour la prochaine étape

| Phase | Contenu | Livrable de sortie |
|---|---|---|
| **0 — Pilotage** (≈ 1-2 jours) | M9, M10, M8 ; créer les issues P0/P1 + jalon « v1.6 — Confiance » ; archiver les docs obsolètes | Backlog à jour, promesses exactes, docs de migration complétées |
| **1 — Correctifs non cassants** | M1, M5 (avec note de migration des clés), S1, S2 | Release 1.5.2 (patch) |
| **2 — Fiabilité prouvée** | M7 (Testcontainers) puis M2, M3, M4 ; M6 | Release 1.6.0 (minor) avec section « Reliability » dans le CHANGELOG |
| **3 — Adoption (en parallèle dès la phase 1)** | S8 (article migration, comparatif S5, annonce) | Premiers retours externes → recalibrage de la priorisation |
| **4 — Décision v2** | S7/#14, #2, #8, #15 regroupés dans une note de migration unique | ADR + jalon v2 |

**[R]** Ne rien lancer sur C1-C9 avant la sortie de la phase 2 ; les ré-évaluer à la lumière des retours de la phase 3.

---

## 7. Décisions à trancher (gouvernance produit)

| # | Décision | Options | Recommandation PO |
|---|---|---|---|
| D1 | Politique SemVer vs correctifs de comportement (#2 encapsulation des exceptions, #8 publisher par défaut, #7 `CacheFailures`, M5 clés idempotence) | (a) tout en 1.x avec note ; (b) options opt-in/opt-out en 1.x, bascule du défaut en v2 ; (c) v2 immédiate | **(b)** : options configurables en 1.x (`CacheFailures`, publisher), défaut inchangé tant que la v2 n'est pas décidée ; **M5 est un cas à part** (faille de sécurité) : corriger en 1.x avec note de migration. À valider avec `solution-architect` |
| D2 | Promesse « dependency-free » | (a) retirer le mot ; (b) réaliser #14 | (a) tout de suite (M10), (b) en v2 |
| D3 | Positionnement : « médiateur léger » **ou** « plateforme de fiabilité » | Les deux ne tiennent pas avec un seul mainteneur | Garder « médiateur + Result + AOT » comme message d'entrée ; présenter les briques fiabilité comme **opt-in en maturité croissante** (étiquette « stable / preview » par package) |
| D4 | Statut de maturité par package | Aujourd'hui tous en `1.5.1` | Marquer `Outbox`, `Idempotency`, `Saga`, bridges brokers comme « preview » tant que M2-M4/C3 ne sont pas faits **[R, à valider]** |

---

## 8. Hypothèses

| # | Hypothèse | Impact si fausse |
|---|---|---|
| H1 | Les téléchargements NuGet uniformes (≈ 260-1 500 pour les packages d'extension) viennent de miroirs/scanners/CI, pas d'utilisateurs | Si ce sont de vrais utilisateurs, l'urgence de M2-M6 est plus forte encore (briques en production) |
| H2 | Aucun utilisateur externe n'a ouvert d'issue ou discussion (seules 5 issues ouvertes, toutes internes) | Des retours peuvent exister ailleurs (mail, réseaux) |
| H3 | Le projet est mené par un seul mainteneur (auteur unique dans le README et `Authors`) avec assistance IA | Capacité de maintenance de 31 packages surestimée ; justifie D3/D4 |
| H4 | Les constats de l analyse d octobre non re-vérifiés (#8, #9, #10, #11, #16, #7 côté Caching) restent exacts | À re-vérifier à l'ouverture des issues (par `developer`/`qa`) |
| H5 | La concurrence (`Mediator`, Wolverine, MassTransit, Brighter) est décrite d'après la connaissance générale, non re-vérifiée en 2026 | Le comparatif §4.2 peut être daté ; à valider par recherche dédiée (`business-analyst`) |
| H6 | Les cibles d'indicateurs de §5.2 sont indicatives | À fixer avec le sponsor |
| H7 | Aucune version majeure de MediatR concurrente (< 13) n'est supportée à long terme | Si MediatR ≤ 12 reste maintenu, le « driver licence » est plus faible |

## 9. Questions ouvertes

| # | Question | Destinataire |
|---|---|---|
| Q1 | Quel est l'objectif réel du projet : adoption communautaire, vitrine/portfolio, usage interne, produit commercial dérivé ? (conditionne les cibles O4) | Utilisateur / sponsor |
| Q2 | Existe-t-il des utilisateurs réels (même internes) des packages Outbox / Idempotency ? | Utilisateur |
| Q3 | Accepte-t-on D1(b) (options opt-in, défauts inchangés jusqu'à la v2) ? | Utilisateur + `solution-architect` |
| Q4 | Faut-il marquer certains packages « preview » (D4) ? | Utilisateur |
| Q5 | Le dossier `cahier des charges/` doit-il être versionné (doc de référence) ou ignoré ? | Utilisateur |
| Q6 | Faisabilité technique de M3 (bail multi-provider EF Core : `ExecuteUpdate` vs `SKIP LOCKED`) et du choix « attente vs 409 » de M4 | `solution-architect` |
| Q7 | Valeur actuelle du ratio de perf vs MediatR/Mediator après #175/#177 ? | `qa`/`developer` (relancer les benchmarks) |

## 10. Risques

| Risque | Impact | Mitigation proposée |
|---|---|---|
| Défauts de fiabilité en production chez un adopteur de l'Outbox/Idempotency | Réputationnel fort, perte/duplication de messages | Phase 2 prioritaire ; label « preview » (D4) en attendant |
| Corriger #2/#8/#7 casse des consommateurs 1.x | Violation de la politique SemVer du projet | D1(b) |
| Dispersion (31 packages, 1 mainteneur) | Dette croissante, qualité inégale (cf. tests de 2 à 4 par package) | « Won't » explicite, §5.3 ; maturité par package |
| Packaging fragile (31 lignes `pack`, v1.5.1 pour assets framework manquants, branche courante `fix/nuget-meta-package-no-framework-assets`) | Packages incomplets sur NuGet | S3 (`pack` solution + MinVer) |
| Absence totale de signal marché | Priorisation à l'aveugle | Phase 3 + canal de feedback (issue template « use case », Discussions) |
| Cahier des charges non versionné | Perte de la seule source de backlog réel | M9 / Q5 |

---

## HANDOFF
- **Agent** : product-owner
- **Statut** : terminé
- **Livrables** :
  - `docs/01-produit/2026-11-15-diagnostic-produit.md` — diagnostic produit (vision/positionnement, backlog, trous fonctionnels et de correction, PRD synthétique de l'étape, MoSCoW + valeur/effort, décisions à trancher)
- **Décisions prises** : aucune décision structurante prise (pas d'ADR) ; les recommandations D1-D4 sont **proposées** et attendent validation de l'utilisateur et du `solution-architect`.
- **Hypothèses** : H1-H7 (§8) — notamment : trafic NuGet = miroirs/scanners, projet mono-mainteneur, paysage concurrentiel non re-vérifié, 6 constats de l'analyse d'octobre non re-vérifiés, cibles d'indicateurs indicatives.
- **Questions ouvertes** : Q1-Q5 → utilisateur ; Q6 → `solution-architect` ; Q7 → `qa` / `developer` (§9).
- **Risques identifiés** :
  - Briques de fiabilité (Outbox, Idempotency) publiées en 1.x avec défauts vérifiés — impact fort — priorité phase 2 + marquage « preview » (agent : `developer`, `qa`).
  - Conflit entre correctifs de comportement et politique SemVer — arbitrage D1 (agent : `solution-architect`).
  - Backlog réel hors GitHub et hors git (`cahier des charges/` non suivi) — perte de traçabilité — M9 (agent : `project-manager`).
  - Packaging NuGet fragile — impact moyen — S3 (agent : `devops`).
  - Absence de signal marché / adoption — impact fort sur la valeur — phase 3 (agent : `business-analyst` pour l'étude concurrentielle).
- **Agent(s) suivant(s) recommandé(s)** :
  - `solution-architect` → valider la faisabilité de M3 (bail Outbox multi-provider), M4 (attente vs conflit), la stratégie SemVer D1 et la migration des clés d'idempotence (M5) ; produire les ADR correspondants.
  - `project-manager` → transformer §6 en planning, créer les issues/jalon v1.6 (avec l'accord explicite de l'utilisateur, aucune issue n'a été créée ici), traiter le ménage de docs M9.
  - `business-analyst` → vérifier le paysage concurrentiel (Mediator, Wolverine, MassTransit, Brighter) et qualifier le trafic NuGet réel.
  - `developer` / `qa` → re-vérifier les constats H4, implémenter M1-M6, mettre en place les tests Testcontainers (M7), relancer les benchmarks (Q7).

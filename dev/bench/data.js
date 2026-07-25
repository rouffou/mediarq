window.BENCHMARK_DATA = {
  "lastUpdate": 1784988029835,
  "repoUrl": "https://github.com/rouffou/mediarq",
  "entries": {
    "Mediarq.Benchmarks - Publish": [
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "dfe6d9d5d676ac7b82d7802958df9c4e11034a39",
          "message": "Merge pull request #153 from rouffou/feat/benchmarks-ci\n\nfeat(ci): run BenchmarkDotNet in CI, report-only against a committed baseline",
          "timestamp": "2026-07-24T15:39:50+02:00",
          "tree_id": "4f14551bff3309b9a77ab7f090210e4ea98a6ab0",
          "url": "https://github.com/rouffou/mediarq/commit/dfe6d9d5d676ac7b82d7802958df9c4e11034a39"
        },
        "date": 1784900438013,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 119.68962637583415,
            "unit": "ns",
            "range": "± 0.6941949079213579"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 141.64338278770447,
            "unit": "ns",
            "range": "± 5.690859065173967"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "43e502d49bb4249e73be22bc14d847e2e1705ffe",
          "message": "Merge pull request #158 from rouffou/feat/sourcegen-aot-diagnostic\n\nfeat(sourcegen): add MQ004, review incremental-caching correctness",
          "timestamp": "2026-07-24T16:23:54+02:00",
          "tree_id": "9fd243f8b003ba9c0933758a203b2effe0402a96",
          "url": "https://github.com/rouffou/mediarq/commit/43e502d49bb4249e73be22bc14d847e2e1705ffe"
        },
        "date": 1784903102073,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 174.16137798627219,
            "unit": "ns",
            "range": "± 0.3018406194548515"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 202.570982058843,
            "unit": "ns",
            "range": "± 2.9114815583696063"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "55b597cd274fed9e7940b9a07f19766078a7237f",
          "message": "Merge pull request #159 from rouffou/dev\n\nRelease v1.3 — Performance & observability",
          "timestamp": "2026-07-24T16:33:32+02:00",
          "tree_id": "9fd243f8b003ba9c0933758a203b2effe0402a96",
          "url": "https://github.com/rouffou/mediarq/commit/55b597cd274fed9e7940b9a07f19766078a7237f"
        },
        "date": 1784903671293,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 161.336590051651,
            "unit": "ns",
            "range": "± 3.903038042047742"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 177.98551321029663,
            "unit": "ns",
            "range": "± 0.4399326228439369"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "aa2beda4f5efbd1c69f082e9a485e72ba534e6a2",
          "message": "Merge pull request #162 from rouffou/feat/benchmarks-expanded\n\nfeat(benchmarks): expand Mediarq.Benchmarks coverage",
          "timestamp": "2026-07-24T18:16:27+02:00",
          "tree_id": "ede252887953b3eb5d6712752c392c0598d3c757",
          "url": "https://github.com/rouffou/mediarq/commit/aa2beda4f5efbd1c69f082e9a485e72ba534e6a2"
        },
        "date": 1784909858620,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 156.08181969324747,
            "unit": "ns",
            "range": "± 0.5773170290605447"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 191.23713819185892,
            "unit": "ns",
            "range": "± 1.111217584943043"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "0740472ca6719eaa123f603567dd2ceb1706a408",
          "message": "Merge pull request #165 from rouffou/dev\n\nRelease v1.3.0",
          "timestamp": "2026-07-24T18:32:41+02:00",
          "tree_id": "68a8703ad146605840050f0774e16e78cf4f97ff",
          "url": "https://github.com/rouffou/mediarq/commit/0740472ca6719eaa123f603567dd2ceb1706a408"
        },
        "date": 1784910829281,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 157.12056608994803,
            "unit": "ns",
            "range": "± 1.0805985772867994"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 175.59515698750815,
            "unit": "ns",
            "range": "± 1.5656858488679817"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "d98beb1d7ecc79f4ec03fa6dcd44a63be7945f6d",
          "message": "fix(benchmarks,core): remove benchmark contamination + avoid async state machine on void dispatch (#169)\n\nManyHandlersBenchmarks, CrossLibraryBenchmarks and LifetimeBenchmarks set up\nMediarq via the scanning AddMediarq(...), which auto-discovered\nMediarqPassthroughBehavior (declared for DeepPipelineBenchmarks, in the same\nassembly) as a global open-generic pipeline behavior. Every dispatch in\nthose three benchmarks silently ran through a 1-behavior pipeline while\nMediatR's own registration never auto-discovers IPipelineBehaviors, making\nthe \"base dispatch\" comparison apples-to-oranges. Switched to\nAddMediarqCore() + explicit handler registrations, the same fix\nDeepPipelineBenchmarks already used.\n\nAlso: IRequestHandler<TRequest>'s default-interface adaptation to\nIRequestHandler<TRequest, Unit> no longer uses async/await, so a handler\nthat completes synchronously (the common case) skips the async state\nmachine entirely. No public API change.\n\nTogether these resolve most of #163: the apparent overhead was not a real\nper-handler cost that grows with registered handler count, it was constant\nbenchmark contamination plus one avoidable allocation on the hot path.\n\n  ManyHandlersBenchmarks:  616 B -> 240 B alloc (3.20x -> 2.23x)\n  CrossLibraryBenchmarks:  432 B ->  56 B alloc (2.89x -> 1.58x)\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-24T20:31:36+02:00",
          "tree_id": "0c63b7aff0653d28cc86f30f47176e29c6866ad5",
          "url": "https://github.com/rouffou/mediarq/commit/d98beb1d7ecc79f4ec03fa6dcd44a63be7945f6d"
        },
        "date": 1784917958126,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 105.66403259833653,
            "unit": "ns",
            "range": "± 1.7144109479728276"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 141.86847694714865,
            "unit": "ns",
            "range": "± 1.3746215924837597"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "df4a7a81164290a413d4eff0ecb77524a36e8db2",
          "message": "Merge pull request #170 from rouffou/dev\n\nRelease v1.4.0",
          "timestamp": "2026-07-24T20:34:58+02:00",
          "tree_id": "0c63b7aff0653d28cc86f30f47176e29c6866ad5",
          "url": "https://github.com/rouffou/mediarq/commit/df4a7a81164290a413d4eff0ecb77524a36e8db2"
        },
        "date": 1784918164170,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 149.78647621472678,
            "unit": "ns",
            "range": "± 0.6079467554058966"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 180.66736102104187,
            "unit": "ns",
            "range": "± 0.23978845303173374"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "1d7073fdf9314f2d46de53a54492219567d69e3e",
          "message": "perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:27:00+02:00",
          "tree_id": "0319d05e6964f3692dcd5cac0112a84cd98e586e",
          "url": "https://github.com/rouffou/mediarq/commit/1d7073fdf9314f2d46de53a54492219567d69e3e"
        },
        "date": 1784964473008,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 146.40025154749551,
            "unit": "ns",
            "range": "± 2.4523480440214933"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 189.18264770507812,
            "unit": "ns",
            "range": "± 2.8125153411626713"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3ae94ba91d259df52306a954ea33ec54cfed5099",
          "message": "fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:30:18+02:00",
          "tree_id": "5551d064959f9f22845320d5bec409e1be11132b",
          "url": "https://github.com/rouffou/mediarq/commit/3ae94ba91d259df52306a954ea33ec54cfed5099"
        },
        "date": 1784964681325,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 157.8676663239797,
            "unit": "ns",
            "range": "± 0.22684896070335228"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 183.3965171178182,
            "unit": "ns",
            "range": "± 1.1178326958417881"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a111c4afe4117c95e5a3982474668e3c47738b46",
          "message": "feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:45:35+02:00",
          "tree_id": "983df667b5ac253203fb7f1e2bec6d286490b06c",
          "url": "https://github.com/rouffou/mediarq/commit/a111c4afe4117c95e5a3982474668e3c47738b46"
        },
        "date": 1784965598979,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 153.4168292681376,
            "unit": "ns",
            "range": "± 0.7176755285343415"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 197.75907802581787,
            "unit": "ns",
            "range": "± 2.2575759014416397"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "17ba263cfc9b51ff98fd77971c33243c7b81a615",
          "message": "feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T15:59:05+02:00",
          "tree_id": "8c4bf6c6437104ba0865ea7203c702b15a12c351",
          "url": "https://github.com/rouffou/mediarq/commit/17ba263cfc9b51ff98fd77971c33243c7b81a615"
        },
        "date": 1784988008430,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 161.64653968811035,
            "unit": "ns",
            "range": "± 0.6521515979379378"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 230.30415749549866,
            "unit": "ns",
            "range": "± 3.175406495450081"
          }
        ]
      }
    ],
    "Mediarq.Benchmarks - Send": [
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "dfe6d9d5d676ac7b82d7802958df9c4e11034a39",
          "message": "Merge pull request #153 from rouffou/feat/benchmarks-ci\n\nfeat(ci): run BenchmarkDotNet in CI, report-only against a committed baseline",
          "timestamp": "2026-07-24T15:39:50+02:00",
          "tree_id": "4f14551bff3309b9a77ab7f090210e4ea98a6ab0",
          "url": "https://github.com/rouffou/mediarq/commit/dfe6d9d5d676ac7b82d7802958df9c4e11034a39"
        },
        "date": 1784900464972,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 93.9902245203654,
            "unit": "ns",
            "range": "± 0.5152423638040565"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 125.16761267185211,
            "unit": "ns",
            "range": "± 0.9010588092738969"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 134.34350653489432,
            "unit": "ns",
            "range": "± 0.8492495787118076"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 102.90802721182506,
            "unit": "ns",
            "range": "± 0.7217607139294068"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "43e502d49bb4249e73be22bc14d847e2e1705ffe",
          "message": "Merge pull request #158 from rouffou/feat/sourcegen-aot-diagnostic\n\nfeat(sourcegen): add MQ004, review incremental-caching correctness",
          "timestamp": "2026-07-24T16:23:54+02:00",
          "tree_id": "9fd243f8b003ba9c0933758a203b2effe0402a96",
          "url": "https://github.com/rouffou/mediarq/commit/43e502d49bb4249e73be22bc14d847e2e1705ffe"
        },
        "date": 1784903102271,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 59.89226202170054,
            "unit": "ns",
            "range": "± 1.6570350245656469"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 91.80849619706471,
            "unit": "ns",
            "range": "± 2.910939412772628"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 87.32832113901775,
            "unit": "ns",
            "range": "± 0.5550478733468004"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 64.85146844387054,
            "unit": "ns",
            "range": "± 1.9623274278180864"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "55b597cd274fed9e7940b9a07f19766078a7237f",
          "message": "Merge pull request #159 from rouffou/dev\n\nRelease v1.3 — Performance & observability",
          "timestamp": "2026-07-24T16:33:32+02:00",
          "tree_id": "9fd243f8b003ba9c0933758a203b2effe0402a96",
          "url": "https://github.com/rouffou/mediarq/commit/55b597cd274fed9e7940b9a07f19766078a7237f"
        },
        "date": 1784903698257,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 53.61632605393728,
            "unit": "ns",
            "range": "± 0.6744379107061989"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 82.32407522201538,
            "unit": "ns",
            "range": "± 0.9655977972157925"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 85.05955805381139,
            "unit": "ns",
            "range": "± 2.35077963375893"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 63.4753404657046,
            "unit": "ns",
            "range": "± 0.6680490242802314"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "aa2beda4f5efbd1c69f082e9a485e72ba534e6a2",
          "message": "Merge pull request #162 from rouffou/feat/benchmarks-expanded\n\nfeat(benchmarks): expand Mediarq.Benchmarks coverage",
          "timestamp": "2026-07-24T18:16:27+02:00",
          "tree_id": "ede252887953b3eb5d6712752c392c0598d3c757",
          "url": "https://github.com/rouffou/mediarq/commit/aa2beda4f5efbd1c69f082e9a485e72ba534e6a2"
        },
        "date": 1784909900562,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 98.03321500619252,
            "unit": "ns",
            "range": "± 0.5955003062322197"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 255.38702726364136,
            "unit": "ns",
            "range": "± 2.7405509812574294"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 131.62813003857931,
            "unit": "ns",
            "range": "± 1.3975273306054439"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 234.7066961924235,
            "unit": "ns",
            "range": "± 2.3898864643079567"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "0740472ca6719eaa123f603567dd2ceb1706a408",
          "message": "Merge pull request #165 from rouffou/dev\n\nRelease v1.3.0",
          "timestamp": "2026-07-24T18:32:41+02:00",
          "tree_id": "68a8703ad146605840050f0774e16e78cf4f97ff",
          "url": "https://github.com/rouffou/mediarq/commit/0740472ca6719eaa123f603567dd2ceb1706a408"
        },
        "date": 1784910866181,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 87.84236739079158,
            "unit": "ns",
            "range": "± 1.2427194730253948"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 243.46066935857138,
            "unit": "ns",
            "range": "± 4.7470038168567035"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 128.75518862406412,
            "unit": "ns",
            "range": "± 1.580904873731026"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 219.97728312015533,
            "unit": "ns",
            "range": "± 4.109798311356153"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "d98beb1d7ecc79f4ec03fa6dcd44a63be7945f6d",
          "message": "fix(benchmarks,core): remove benchmark contamination + avoid async state machine on void dispatch (#169)\n\nManyHandlersBenchmarks, CrossLibraryBenchmarks and LifetimeBenchmarks set up\nMediarq via the scanning AddMediarq(...), which auto-discovered\nMediarqPassthroughBehavior (declared for DeepPipelineBenchmarks, in the same\nassembly) as a global open-generic pipeline behavior. Every dispatch in\nthose three benchmarks silently ran through a 1-behavior pipeline while\nMediatR's own registration never auto-discovers IPipelineBehaviors, making\nthe \"base dispatch\" comparison apples-to-oranges. Switched to\nAddMediarqCore() + explicit handler registrations, the same fix\nDeepPipelineBenchmarks already used.\n\nAlso: IRequestHandler<TRequest>'s default-interface adaptation to\nIRequestHandler<TRequest, Unit> no longer uses async/await, so a handler\nthat completes synchronously (the common case) skips the async state\nmachine entirely. No public API change.\n\nTogether these resolve most of #163: the apparent overhead was not a real\nper-handler cost that grows with registered handler count, it was constant\nbenchmark contamination plus one avoidable allocation on the hot path.\n\n  ManyHandlersBenchmarks:  616 B -> 240 B alloc (3.20x -> 2.23x)\n  CrossLibraryBenchmarks:  432 B ->  56 B alloc (2.89x -> 1.58x)\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-24T20:31:36+02:00",
          "tree_id": "0c63b7aff0653d28cc86f30f47176e29c6866ad5",
          "url": "https://github.com/rouffou/mediarq/commit/d98beb1d7ecc79f4ec03fa6dcd44a63be7945f6d"
        },
        "date": 1784917977365,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 81.19308332602183,
            "unit": "ns",
            "range": "± 1.2269095246334356"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 254.26481835047403,
            "unit": "ns",
            "range": "± 1.0276686396191483"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 122.71539489428203,
            "unit": "ns",
            "range": "± 0.28037885681831065"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 221.0615452925364,
            "unit": "ns",
            "range": "± 0.5356836722557761"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "df4a7a81164290a413d4eff0ecb77524a36e8db2",
          "message": "Merge pull request #170 from rouffou/dev\n\nRelease v1.4.0",
          "timestamp": "2026-07-24T20:34:58+02:00",
          "tree_id": "0c63b7aff0653d28cc86f30f47176e29c6866ad5",
          "url": "https://github.com/rouffou/mediarq/commit/df4a7a81164290a413d4eff0ecb77524a36e8db2"
        },
        "date": 1784918175021,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 56.372170309225716,
            "unit": "ns",
            "range": "± 1.570878338201752"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 180.176971077919,
            "unit": "ns",
            "range": "± 5.483253839482801"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 85.89338417847951,
            "unit": "ns",
            "range": "± 4.306876818280229"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 145.3589320977529,
            "unit": "ns",
            "range": "± 1.0116457941776846"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "1d7073fdf9314f2d46de53a54492219567d69e3e",
          "message": "perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:27:00+02:00",
          "tree_id": "0319d05e6964f3692dcd5cac0112a84cd98e586e",
          "url": "https://github.com/rouffou/mediarq/commit/1d7073fdf9314f2d46de53a54492219567d69e3e"
        },
        "date": 1784964504320,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 89.69731938838959,
            "unit": "ns",
            "range": "± 0.8011561219143126"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 238.45052846272787,
            "unit": "ns",
            "range": "± 5.528048991483985"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 139.11709904670715,
            "unit": "ns",
            "range": "± 1.9677916439280392"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 232.03685303529105,
            "unit": "ns",
            "range": "± 2.074959814342952"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3ae94ba91d259df52306a954ea33ec54cfed5099",
          "message": "fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:30:18+02:00",
          "tree_id": "5551d064959f9f22845320d5bec409e1be11132b",
          "url": "https://github.com/rouffou/mediarq/commit/3ae94ba91d259df52306a954ea33ec54cfed5099"
        },
        "date": 1784964697135,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 92.12864170471828,
            "unit": "ns",
            "range": "± 0.3323582224611678"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 246.03448073069254,
            "unit": "ns",
            "range": "± 2.539850142631922"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 133.09718652566275,
            "unit": "ns",
            "range": "± 1.5539986960656234"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 220.47383864720663,
            "unit": "ns",
            "range": "± 1.2423422039514531"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a111c4afe4117c95e5a3982474668e3c47738b46",
          "message": "feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:45:35+02:00",
          "tree_id": "983df667b5ac253203fb7f1e2bec6d286490b06c",
          "url": "https://github.com/rouffou/mediarq/commit/a111c4afe4117c95e5a3982474668e3c47738b46"
        },
        "date": 1784965623508,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 97.668569902579,
            "unit": "ns",
            "range": "± 0.8790155273675833"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 234.6497451464335,
            "unit": "ns",
            "range": "± 2.0034629623822613"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 123.6802575190862,
            "unit": "ns",
            "range": "± 0.9205663988040186"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 208.61401212215424,
            "unit": "ns",
            "range": "± 0.836259164592189"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "17ba263cfc9b51ff98fd77971c33243c7b81a615",
          "message": "feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T15:59:05+02:00",
          "tree_id": "8c4bf6c6437104ba0865ea7203c702b15a12c351",
          "url": "https://github.com/rouffou/mediarq/commit/17ba263cfc9b51ff98fd77971c33243c7b81a615"
        },
        "date": 1784988029136,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 87.05835370222728,
            "unit": "ns",
            "range": "± 0.49513455712597776"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 252.94295183817545,
            "unit": "ns",
            "range": "± 1.913272876716115"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 133.5092021624247,
            "unit": "ns",
            "range": "± 2.1073876748015627"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 225.8471965789795,
            "unit": "ns",
            "range": "± 1.514869030141163"
          }
        ]
      }
    ],
    "Mediarq.Benchmarks - Send (Allocated)": [
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "1d7073fdf9314f2d46de53a54492219567d69e3e",
          "message": "perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:27:00+02:00",
          "tree_id": "0319d05e6964f3692dcd5cac0112a84cd98e586e",
          "url": "https://github.com/rouffou/mediarq/commit/1d7073fdf9314f2d46de53a54492219567d69e3e"
        },
        "date": 1784964516324,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send - Allocated",
            "value": 224,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send - Allocated",
            "value": 616,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean - Allocated",
            "value": 240,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain - Allocated",
            "value": 504,
            "unit": "B"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3ae94ba91d259df52306a954ea33ec54cfed5099",
          "message": "fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:30:18+02:00",
          "tree_id": "5551d064959f9f22845320d5bec409e1be11132b",
          "url": "https://github.com/rouffou/mediarq/commit/3ae94ba91d259df52306a954ea33ec54cfed5099"
        },
        "date": 1784964707667,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send - Allocated",
            "value": 224,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send - Allocated",
            "value": 616,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean - Allocated",
            "value": 240,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain - Allocated",
            "value": 504,
            "unit": "B"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a111c4afe4117c95e5a3982474668e3c47738b46",
          "message": "feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:45:35+02:00",
          "tree_id": "983df667b5ac253203fb7f1e2bec6d286490b06c",
          "url": "https://github.com/rouffou/mediarq/commit/a111c4afe4117c95e5a3982474668e3c47738b46"
        },
        "date": 1784965633602,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send - Allocated",
            "value": 224,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send - Allocated",
            "value": 616,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean - Allocated",
            "value": 240,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain - Allocated",
            "value": 504,
            "unit": "B"
          }
        ]
      }
    ],
    "Mediarq.Benchmarks - Publish (Allocated)": [
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "1d7073fdf9314f2d46de53a54492219567d69e3e",
          "message": "perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:27:00+02:00",
          "tree_id": "0319d05e6964f3692dcd5cac0112a84cd98e586e",
          "url": "https://github.com/rouffou/mediarq/commit/1d7073fdf9314f2d46de53a54492219567d69e3e"
        },
        "date": 1784964516942,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 392,
            "unit": "B"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "3ae94ba91d259df52306a954ea33ec54cfed5099",
          "message": "fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:30:18+02:00",
          "tree_id": "5551d064959f9f22845320d5bec409e1be11132b",
          "url": "https://github.com/rouffou/mediarq/commit/3ae94ba91d259df52306a954ea33ec54cfed5099"
        },
        "date": 1784964710487,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 392,
            "unit": "B"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "rouffou@gmail.com",
            "name": "Nicolas Rouffart",
            "username": "rouffou"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a111c4afe4117c95e5a3982474668e3c47738b46",
          "message": "feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T09:45:35+02:00",
          "tree_id": "983df667b5ac253203fb7f1e2bec6d286490b06c",
          "url": "https://github.com/rouffou/mediarq/commit/a111c4afe4117c95e5a3982474668e3c47738b46"
        },
        "date": 1784965638513,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 392,
            "unit": "B"
          }
        ]
      }
    ]
  }
}
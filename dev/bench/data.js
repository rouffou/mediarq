window.BENCHMARK_DATA = {
  "lastUpdate": 1785048027422,
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
          "id": "f5eb5b01c198672aaff100bea8fb663aa3fd5d01",
          "message": "perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T16:54:44+02:00",
          "tree_id": "baccd9fe7316e21da4755f3b2be14f4c0bfad995",
          "url": "https://github.com/rouffou/mediarq/commit/f5eb5b01c198672aaff100bea8fb663aa3fd5d01"
        },
        "date": 1784991353213,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 146.63493688901266,
            "unit": "ns",
            "range": "± 0.42040145626425657"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 210.12315400441489,
            "unit": "ns",
            "range": "± 1.1967876903636623"
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
          "id": "6082554feb0d9e4fa4a124df658c8caf903e491d",
          "message": "feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T19:53:13+02:00",
          "tree_id": "2181188561efe23fe0a666f586c0e3b8425c5220",
          "url": "https://github.com/rouffou/mediarq/commit/6082554feb0d9e4fa4a124df658c8caf903e491d"
        },
        "date": 1785002058387,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 177.48079045613608,
            "unit": "ns",
            "range": "± 0.34924507987390935"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 220.4058768749237,
            "unit": "ns",
            "range": "± 0.6232839056781743"
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
          "id": "ce2a5a519647a9dcdbff86bf2878ea78e8819293",
          "message": "Release v1.5.0 (#225)\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(quartz): add Mediarq.Quartz package for delayed/scheduled dispatch (#174)\n\nEnqueueAsync/ScheduleAsync extensions on IScheduler run a Mediarq ICommand\nas a Quartz.NET job through the real dispatch pipeline. The command is\nJSON-serialized (System.Text.Json) into the job's JobDataMap alongside its\nAssemblyQualifiedName and reconstructed when the trigger fires.\n\nVerified end-to-end against a real Quartz scheduler and worker\n(Quartz.Extensions.Hosting), not just the job-data shape in isolation.\nSame ICommand-only constraint and generic-over-the-concrete-type pattern\nas Mediarq.Hangfire (each extension captures the concrete command type at\nthe call site so the type-name-based round trip resolves correctly).\n\nCompletes #36 (Hangfire done in a prior PR; gRPC transport for\ncross-service notifications remains open, no immediate plan to pick it up).\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(healthchecks): add Mediarq.HealthChecks package for handler-registration validation (#192)\n\nCatches a missing or ambiguous command/query handler before it surfaces as a\nHandlerNotFoundException on first dispatch. Ships an IHealthCheck for a /health\nendpoint plus AddMediarqHandlerValidationOnStartup, which runs the same check\nonce during host startup and throws so the app fails fast on misconfiguration.\n\nCloses #188\n\n* feat(ci): track allocation regression alongside mean time in the benchmark guardrail (#193)\n\ngithub-action-benchmark's built-in benchmarkdotnet tool only reads BenchmarkDotNet's Mean\nstatistic, so allocation regressions could slip through even with the existing time-based\nalert. Add a benchmark-alloc job (per Send/Publish matrix entry) that converts the same\nBenchmarkDotNet JSON export into the customSmallerIsBetter format via a new converter\nscript and tracks Memory.BytesAllocatedPerOperation as its own alerted history series,\nreusing the artifact the benchmark job already produces instead of rerunning BenchmarkDotNet.\n\nCloses #179\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(testing): add Mediarq.Testing package with SpyMediator and fakes (#197)\n\nNew SpyMediator decorates the registered IMediator, recording every dispatched\nrequest/notification while still delegating to the real one -- handlers, validators\nand pipeline behaviors all run for real, only the bookkeeping is added. AddMediarqSpy()\ndecorates via Scrutor after AddMediarq/AddMediarqCore; ISender/IPublisher are covered\ntoo since both already resolve the current IMediator from the container.\n\nSpyMediatorAssertions (Sent<T>/HasSent<T>/Published<T>/HasPublished<T>) stays\nframework-agnostic so it pairs with whatever assertion library a consumer already uses.\n\nAlso ships FakeClock/FakeUserContext, settable implementations of IClock/IUserContext.\n\nCloses #185\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(efcore): add domain-event support to Mediarq.EntityFrameworkCore (#198)\n\nNew IHasDomainEvents (+ convenience AggregateRoot base class) and DomainEventsInterceptor,\na SaveChanges interceptor that collects and clears events staged on tracked aggregates\nright before the commit, then publishes them only once it actually succeeds -- a failed\ncommit discards the collected events rather than publishing them or re-raising them on\na retry.\n\nAddMediarqDomainEvents() registers the interceptor as scoped IInterceptor on the\napplication service provider, so it's picked up automatically by any AddDbContext<T>(...)\ncall without touching that call -- and scoped (not singleton) so it gets a fresh scoped\nIPublisher per DbContext construction instead of capturing the first one forever.\n\nAsync-only: IPublisher has no synchronous overload, so only SaveChangesAsync is\nintercepted.\n\nCloses #186\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): add automatic minimal API mapping (app.MapMediarq()) (#199)\n\nNew route attributes (MediarqGet/Post/Put/Patch/Delete) + MapMediarq(), which scans\nassemblies for attributed commands/queries and maps each directly as a minimal API\nendpoint, dispatching through ISender. GET/DELETE bind the request's members\nindividually from the route/query string ([AsParameters], no body); POST/PUT/PATCH\nbind the whole request from the JSON body. The response converts the same way\nToHttpResult() already does for Result/Result<T>; a no-result ICommand (response\nUnit) maps a successful dispatch to 204 No Content. An attributed type whose\nresponse is none of those three shapes throws InvalidOperationException at startup\nrather than failing silently.\n\nDelegates are built dynamically per discovered type via MakeGenericMethod against\nfour private generic handler methods (body/params x Result/Result<T>, plus two more\nfor Unit), so [AsParameters]/body-binding attribution on the closed generic method's\nparameters is inspected by RequestDelegateFactory exactly as it would be for a\nhand-written endpoint.\n\nReturns a RouteGroupBuilder so shared conventions (RequireAuthorization, WithTags,\n...) apply to every mapped endpoint at once.\n\nCloses #180\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(ratelimiting): add Mediarq.RateLimiting package for pipeline-level throttling (#200)\n\nNew IRateLimitedRequest marker (PolicyName + optional PartitionKey) and\nRateLimitingBehavior, built on System.Threading.RateLimiting -- no HTTP dependency,\nprotects any hot path directly in the pipeline. A named RateLimiterRegistry maps a\npolicy name to a PartitionedRateLimiter<string>; PartitionKey (or \"*\" when null)\nselects the partition, so different callers (e.g. per user) get independent limits\nunder the same policy.\n\nNo permit available throws RateLimitExceededException (PolicyName/PartitionKey/\nRetryAfter) rather than short-circuiting into a Result -- catch it via an\nIRequestExceptionHandler<,> or an ASP.NET Core exception handler to map it to a\n429, mirroring Polly's own RateLimiterRejectedException convention rather than\nforcing a Result-shaped response the way Mediarq.Authorization does.\n\nCloses #182\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ204 analyzer for a pipeline behavior that is never active (#201)\n\nNew InertConditionalBehaviorAnalyzer flags an IConditionalPipelineBehavior.IsActive\nimplementation that is syntactically always the literal false -- the behavior is\nregistered but can never participate in the pipeline for any request. Same\nsyntactic-only approach as MQ201 (PipelineBehaviorNextAnalyzer): only fires when the\ngetter is literally `false` (expression-bodied property, expression-bodied getter, or\na single `return false;`), so real conditional logic is never flagged regardless of\nhow it evaluates at runtime -- no full flow-analysis proof attempted.\n\nCloses #191\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* chore(samples): wire Authorization, RateLimiting, HealthChecks, domain events into the WebApi sample (#202)\n\nThe Orders sample only demonstrated the extensions that existed before this cycle. It now also\nshowcases the four added since: Mediarq.RateLimiting throttles order creation (429 on rejection),\nMediarq.Authorization protects order confirmation behind a policy (401/403, via a self-contained\ndemo header-auth scheme), Mediarq.EntityFrameworkCore's domain events raise an in-process\nOrderConfirmedDomainEvent on confirm (distinct from OrderPlacedEvent's outbox delivery), and\nMediarq.HealthChecks exposes GET /health.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(deferred): add Mediarq.Deferred package for in-process deferred dispatch (#204)\n\nIDeferredDispatcher.SendLaterAsync/PublishLaterAsync queue a command or notification on a\nSystem.Threading.Channels-backed background worker (DeferredDispatchHostedService) instead of\nrunning its handler(s) inline, decoupling the caller from handler execution time. No external\ndependency, no persistent store — fills the gap between immediate Send/Publish and durable\nscheduling (Mediarq.Hangfire/Mediarq.Quartz) for the \"reliable in-process fire-and-forget\" case.\nA graceful host shutdown stops accepting new work and drains everything already queued before\nstopping, bounded by the host's own shutdown timeout; an exception in one item is logged and does\nnot stop the worker from processing the rest.\n\nCloses #184.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(dapr): add Mediarq.Dapr package for Dapr pub/sub integration (#205)\n\nIDaprPubSubEvent marks a notification for Dapr pub/sub via static abstract PubsubName/Topic\nmembers (not instance properties, since the subscribe side needs routing info before any\nnotification instance exists, and both directions reading the same statics means they can\nnever drift apart).\n\nPublish side: AddMediarqDaprPubSub<TNotification>() registers a forwarder that calls\nDaprClient.PublishEventAsync when the notification is published through Mediarq, mirroring\nMediarq.MassTransit's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: MapDaprPubSubSubscription<TNotification>() maps a minimal-API webhook that\nextracts the `data` field from the CloudEvents 1.0 envelope the Dapr sidecar delivers and\nrepublishes it through IPublisher, and MapDaprPubSubSubscribeEndpoint() serves the\n/dapr/subscribe discovery endpoint the sidecar queries at startup.\n\nCloses #190.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(rabbitmq): add Mediarq.RabbitMQ package for a lightweight broker bridge (#206)\n\nIRabbitMqEvent marks a notification for RabbitMQ via static abstract Exchange/Queue/RoutingKey\nmembers (same static-member rationale as Mediarq.Dapr's IDaprPubSubEvent: the subscriber needs\nrouting info before any instance exists, and both directions reading the same statics means\nthey can never drift apart).\n\nPublish side: AddMediarqRabbitMqPublisher<TNotification>() registers a forwarder that publishes\non a short-lived channel per call, mirroring Mediarq.MassTransit/Mediarq.Dapr's forwarder shape\n(runs alongside in-process handlers).\n\nSubscribe side: AddMediarqRabbitMqSubscriber<TNotification>() registers a background service\nthat declares the exchange/queue/binding, consumes with manual acknowledgement, and republishes\neach delivery through IPublisher — acking only after a successful publish, nacking without\nrequeue on failure so a poison message doesn't loop forever.\n\nThis package never owns the IConnection's lifecycle (bring your own) and does not implement\nduplicate-delivery detection (documented as a follow-up, not silently assumed) — a lightweight\nalternative to Mediarq.MassTransit for the simple pub/sub case, per #189.\n\nFirst half of #189 (RabbitMQ). The Azure Service Bus half is a separate follow-up PR.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(azureservicebus): add Mediarq.AzureServiceBus package, closing #189 (#207)\n\nIAzureServiceBusEvent marks a notification for Azure Service Bus via static abstract\nTopicName/SubscriptionName members (same static-member rationale as Mediarq.Dapr/Mediarq.RabbitMQ:\nthe subscriber needs routing info before any instance exists, and both directions reading the\nsame statics means they can never drift apart). Uses the topic+subscription pub/sub model; this\npackage does not provision the topic/subscription (pre-provision via portal/ARM/Bicep/\nServiceBusAdministrationClient).\n\nPublish side: AddMediarqAzureServiceBusPublisher<TNotification>() registers a forwarder that\nsends on a ServiceBusSender created per publish, mirroring Mediarq.MassTransit/Mediarq.Dapr/\nMediarq.RabbitMQ's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: AddMediarqAzureServiceBusSubscriber<TNotification>() registers a background\nservice that processes the subscription via ServiceBusProcessor and republishes each message\nthrough IPublisher — completing only after a successful publish, dead-lettering on failure (the\nService Bus analogue of \"nack without requeue\") so a poison message doesn't loop forever.\n\nThis package never owns the ServiceBusClient's lifecycle and does not implement\nduplicate-delivery detection — a lightweight alternative to Mediarq.MassTransit for the simple\npub/sub case, per #189.\n\nSecond and final half of #189 (Azure Service Bus). Closes #189.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspire): add Mediarq.Aspire package for .NET Aspire ServiceDefaults integration (#208)\n\nAddMediarqServiceDefaults() on IHostApplicationBuilder, meant to be called from inside a\nconsumer's own dotnet new aspire-servicedefaults-generated ServiceDefaults project alongside its\nown OpenTelemetry/service-discovery/resilience setup — additive, not a replacement.\n\nWires Mediarq.OpenTelemetry's tracing/metrics (AddMediarqInstrumentation on both the tracer and\nmeter providers) and Mediarq.HealthChecks' handler-registration check on top of whatever the\nAspire template already generated, so Mediarq dispatch spans/metrics and a missing/ambiguous\nhandler both show up in the Aspire dashboard. Deliberately does not reimplement OpenTelemetry\nexporter/service-discovery/resilience wiring or map /health and /alive endpoints itself — those\nremain the template's own concern.\n\nCloses #187.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(packaging): add Mediarq logo as the NuGet package icon (#211)\n\n* feat(packaging): add Mediarq logo and embed it as the NuGet package icon\n\nAdds assets/logo.svg (source) and assets/icon.png (256x256), wires\nPackageIcon into src/Directory.Build.props so every package under src/\nships the icon, and adds it to the Mediarq.Templates package as well.\n\n* docs(readme): display the Mediarq logo at the top of the README\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(grpc): add Mediarq.Grpc package for direct point-to-point notification transport (#212)\n\nCloses #36 (gRPC half; Quartz/Hangfire scheduled dispatch already shipped in #171).\n\nIGrpcNotificationEvent (static abstract string ServiceAddress) marks a\nnotification for gRPC delivery to another service. AddMediarqGrpcPublisher<T>()\nforwards it via GrpcNotificationForwarder<T>, which resolves a cached, reused\nHTTP/2 GrpcChannel per ServiceAddress (GrpcChannelCache, disposed by the\ncontainer at shutdown) and calls the generated NotificationServiceClient.\nAddMediarqGrpcSubscriptions() + MapMediarqGrpcNotificationService() +\nMapMediarqGrpcSubscription<T>() receive it back into the pipeline: every\nsubscribed type is multiplexed over one shared RPC method (Publish), keyed by\nthe envelope's type_name against a registry of compile-time-typed\ndeserialize-and-publish delegates (no runtime reflection on the dispatch\npath itself).\n\nShips its own compiled Protobuf/gRPC contract (Protos/notification.proto,\nGrpcServices=\"Both\") so consumers reference this package only, no protoc/\nGrpc.Tools needed downstream. Tried generating the contract with\n--csharp_opt=internal_access to avoid exposing it as public API surface;\nreverted after confirming it's a known limitation (the flag only applies to\nmessage types, not the grpc_csharp_plugin-generated service/client stubs,\ncausing an accessibility mismatch) — the generated surface is public,\ntracked in PublicAPI.Unshipped.txt like every other package's surface.\n\nGrpcChannelCache and GrpcNotificationForwarder's constructor had to be public\nrather than internal: Microsoft.Extensions.DependencyInjection's default\ncontainer only considers public constructors when activating a type, so an\ninternal-typed constructor parameter on a publicly-constructed type silently\nfails DI resolution (caught by two failing tests during development, fixed\nbefore this commit).\n\nVerified: full solution build (0 warnings/errors beyond pre-existing,\nunrelated ones), full test suite (18 new tests, all green), and an ad-hoc\nAOT publish scan of the subscribe side confirmed AddGrpc()/MapGrpcService()\nitself produces zero trim/AOT warnings — the only warning present is the\nreflection-based JsonSerializer.Deserialize<T> call, the same pre-existing,\nunannotated pattern already used in the Dapr/RabbitMQ/AzureServiceBus\npackages, not a new risk category introduced here.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(sourcegen): add MQ005/MQ006/MQ007 diagnostics for stream handlers and notifications (#217)\n\nMQ001/MQ002 already caught duplicate/missing IRequestHandler registrations for\ncommands and queries. The generator collects the same registration data for\nIStreamRequestHandler<,> and INotificationHandler<>, but never diagnosed it,\nso a missing stream handler or an orphan notification only surfaced as a\nruntime HandlerNotFoundException.\n\n- MQ005 (warning): multiple IStreamRequestHandler<,> for the same stream request\n- MQ006 (info): a declared IStreamRequest<T> with no handler in the assembly\n- MQ007 (info): a declared INotification with no handler in the assembly\n\nCloses #213.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ202 to flag duplicate Mediarq routes at compile time (#218)\n\nMapMediarq() maps every [MediarqGet]/[MediarqPost]/[MediarqPut]/[MediarqPatch]/\n[MediarqDelete]-attributed request type as a minimal API endpoint with zero\nuniqueness check across types. Two types declaring the same (HTTP method,\nroute pattern) pair only collided at ASP.NET Core's routing time -- an\nambiguous-match error on the first matching request -- never at build time.\n\nDuplicateRouteAnalyzer (MQ202) collects every Mediarq route attribute in the\ncompilation and flags a duplicate (method, pattern) pair declared by more than\none type, same by-name/by-namespace attribute matching as the existing\nMQ200/MQ201/MQ204 analyzers.\n\nCloses #214.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): declare OpenAPI response metadata for MapMediarq() endpoints (#219)\n\nMapMediarq() built its minimal-API delegates as Func<TRequest, ISender,\nCancellationToken, Task<IResult>>, returning a bare IResult from the Handle*\nhelpers. ASP.NET Core's built-in OpenAPI inference needs a statically-typed\nResults<...> union or explicit .Produces<T>() calls to infer a response\nschema -- neither was present, so every MapMediarq()-mapped endpoint showed\nup in Swagger with an untyped or absent response body.\n\nAttach explicit response metadata to the RouteHandlerBuilder returned by\neach MapGet/MapPost/etc call instead: 200/204 with the success type (driven\nby the response type -- Result, Result<T> or Unit), plus every failure\nstatus ResultError.Type can map to (400/401/403/404/409/500), using the\nexisting ErrorType -> HTTP status mapping in ResultHttpExtensions.\n\nCloses #215.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* docs(samples): showcase Result.WithNotifications(...) cascading in the WebApi sample (#221)\n\nAddOrderNoteHandler now attaches an OrderNoteAddedEvent to its successful\nResult instead of just returning it -- a third, lightweight notification\npattern next to the transactional outbox (CreateOrder) and domain events\n(ConfirmOrder) this sample already demonstrates side by side.\n\nVerified manually: created an order, POSTed a note, confirmed the\n[cascaded notification] log line fires from the new INotificationHandler.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#223)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#224)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T21:49:04+02:00",
          "tree_id": "bc44ddcf81214df474e2f631cd79e3daa27b7f65",
          "url": "https://github.com/rouffou/mediarq/commit/ce2a5a519647a9dcdbff86bf2878ea78e8819293"
        },
        "date": 1785009012298,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 150.2440090974172,
            "unit": "ns",
            "range": "± 0.786124525901364"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 230.56478508313498,
            "unit": "ns",
            "range": "± 2.8152886624749227"
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
          "id": "37cb619bb7460fc2603d44feb78bbf2f3444b50e",
          "message": "fix(packaging): declare lib/ framework assets for Mediarq and Mediarq.Analyzers (#226)\n\nBoth packages intentionally ship no assembly of their own (Mediarq is a\nmeta-package bundling its dependencies, Mediarq.Analyzers ships its DLL\nonly under analyzers/dotnet/cs) — with no lib/ folder at all, NuGet.org\nshows 'There are no supported framework assets in this package' for\nboth, as seen live on v1.5.0.\n\nAdd empty lib/<tfm>/_._ marker files (the standard NuGet convention for\nthis exact case) so NuGet.org lists net8.0/net9.0/net10.0 for Mediarq\nand netstandard2.0 for Mediarq.Analyzers, without shipping a real\nassembly. Suppress the resulting NU5128 for Mediarq.Analyzers, whose\ndependencies are intentionally omitted from the nuspec.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-26T08:33:36+02:00",
          "tree_id": "fa5daa5454b15a84da3f01500fc3e12a5217083f",
          "url": "https://github.com/rouffou/mediarq/commit/37cb619bb7460fc2603d44feb78bbf2f3444b50e"
        },
        "date": 1785047668458,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 128.67035873730978,
            "unit": "ns",
            "range": "± 3.3244860776588574"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 176.12640595436096,
            "unit": "ns",
            "range": "± 0.5120002637724699"
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
          "id": "fdf88ad959f5833c36e62de2424708fd45f34ae5",
          "message": "Release v1.5.1 (#227)\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(quartz): add Mediarq.Quartz package for delayed/scheduled dispatch (#174)\n\nEnqueueAsync/ScheduleAsync extensions on IScheduler run a Mediarq ICommand\nas a Quartz.NET job through the real dispatch pipeline. The command is\nJSON-serialized (System.Text.Json) into the job's JobDataMap alongside its\nAssemblyQualifiedName and reconstructed when the trigger fires.\n\nVerified end-to-end against a real Quartz scheduler and worker\n(Quartz.Extensions.Hosting), not just the job-data shape in isolation.\nSame ICommand-only constraint and generic-over-the-concrete-type pattern\nas Mediarq.Hangfire (each extension captures the concrete command type at\nthe call site so the type-name-based round trip resolves correctly).\n\nCompletes #36 (Hangfire done in a prior PR; gRPC transport for\ncross-service notifications remains open, no immediate plan to pick it up).\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(healthchecks): add Mediarq.HealthChecks package for handler-registration validation (#192)\n\nCatches a missing or ambiguous command/query handler before it surfaces as a\nHandlerNotFoundException on first dispatch. Ships an IHealthCheck for a /health\nendpoint plus AddMediarqHandlerValidationOnStartup, which runs the same check\nonce during host startup and throws so the app fails fast on misconfiguration.\n\nCloses #188\n\n* feat(ci): track allocation regression alongside mean time in the benchmark guardrail (#193)\n\ngithub-action-benchmark's built-in benchmarkdotnet tool only reads BenchmarkDotNet's Mean\nstatistic, so allocation regressions could slip through even with the existing time-based\nalert. Add a benchmark-alloc job (per Send/Publish matrix entry) that converts the same\nBenchmarkDotNet JSON export into the customSmallerIsBetter format via a new converter\nscript and tracks Memory.BytesAllocatedPerOperation as its own alerted history series,\nreusing the artifact the benchmark job already produces instead of rerunning BenchmarkDotNet.\n\nCloses #179\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(testing): add Mediarq.Testing package with SpyMediator and fakes (#197)\n\nNew SpyMediator decorates the registered IMediator, recording every dispatched\nrequest/notification while still delegating to the real one -- handlers, validators\nand pipeline behaviors all run for real, only the bookkeeping is added. AddMediarqSpy()\ndecorates via Scrutor after AddMediarq/AddMediarqCore; ISender/IPublisher are covered\ntoo since both already resolve the current IMediator from the container.\n\nSpyMediatorAssertions (Sent<T>/HasSent<T>/Published<T>/HasPublished<T>) stays\nframework-agnostic so it pairs with whatever assertion library a consumer already uses.\n\nAlso ships FakeClock/FakeUserContext, settable implementations of IClock/IUserContext.\n\nCloses #185\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(efcore): add domain-event support to Mediarq.EntityFrameworkCore (#198)\n\nNew IHasDomainEvents (+ convenience AggregateRoot base class) and DomainEventsInterceptor,\na SaveChanges interceptor that collects and clears events staged on tracked aggregates\nright before the commit, then publishes them only once it actually succeeds -- a failed\ncommit discards the collected events rather than publishing them or re-raising them on\na retry.\n\nAddMediarqDomainEvents() registers the interceptor as scoped IInterceptor on the\napplication service provider, so it's picked up automatically by any AddDbContext<T>(...)\ncall without touching that call -- and scoped (not singleton) so it gets a fresh scoped\nIPublisher per DbContext construction instead of capturing the first one forever.\n\nAsync-only: IPublisher has no synchronous overload, so only SaveChangesAsync is\nintercepted.\n\nCloses #186\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): add automatic minimal API mapping (app.MapMediarq()) (#199)\n\nNew route attributes (MediarqGet/Post/Put/Patch/Delete) + MapMediarq(), which scans\nassemblies for attributed commands/queries and maps each directly as a minimal API\nendpoint, dispatching through ISender. GET/DELETE bind the request's members\nindividually from the route/query string ([AsParameters], no body); POST/PUT/PATCH\nbind the whole request from the JSON body. The response converts the same way\nToHttpResult() already does for Result/Result<T>; a no-result ICommand (response\nUnit) maps a successful dispatch to 204 No Content. An attributed type whose\nresponse is none of those three shapes throws InvalidOperationException at startup\nrather than failing silently.\n\nDelegates are built dynamically per discovered type via MakeGenericMethod against\nfour private generic handler methods (body/params x Result/Result<T>, plus two more\nfor Unit), so [AsParameters]/body-binding attribution on the closed generic method's\nparameters is inspected by RequestDelegateFactory exactly as it would be for a\nhand-written endpoint.\n\nReturns a RouteGroupBuilder so shared conventions (RequireAuthorization, WithTags,\n...) apply to every mapped endpoint at once.\n\nCloses #180\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(ratelimiting): add Mediarq.RateLimiting package for pipeline-level throttling (#200)\n\nNew IRateLimitedRequest marker (PolicyName + optional PartitionKey) and\nRateLimitingBehavior, built on System.Threading.RateLimiting -- no HTTP dependency,\nprotects any hot path directly in the pipeline. A named RateLimiterRegistry maps a\npolicy name to a PartitionedRateLimiter<string>; PartitionKey (or \"*\" when null)\nselects the partition, so different callers (e.g. per user) get independent limits\nunder the same policy.\n\nNo permit available throws RateLimitExceededException (PolicyName/PartitionKey/\nRetryAfter) rather than short-circuiting into a Result -- catch it via an\nIRequestExceptionHandler<,> or an ASP.NET Core exception handler to map it to a\n429, mirroring Polly's own RateLimiterRejectedException convention rather than\nforcing a Result-shaped response the way Mediarq.Authorization does.\n\nCloses #182\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ204 analyzer for a pipeline behavior that is never active (#201)\n\nNew InertConditionalBehaviorAnalyzer flags an IConditionalPipelineBehavior.IsActive\nimplementation that is syntactically always the literal false -- the behavior is\nregistered but can never participate in the pipeline for any request. Same\nsyntactic-only approach as MQ201 (PipelineBehaviorNextAnalyzer): only fires when the\ngetter is literally `false` (expression-bodied property, expression-bodied getter, or\na single `return false;`), so real conditional logic is never flagged regardless of\nhow it evaluates at runtime -- no full flow-analysis proof attempted.\n\nCloses #191\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* chore(samples): wire Authorization, RateLimiting, HealthChecks, domain events into the WebApi sample (#202)\n\nThe Orders sample only demonstrated the extensions that existed before this cycle. It now also\nshowcases the four added since: Mediarq.RateLimiting throttles order creation (429 on rejection),\nMediarq.Authorization protects order confirmation behind a policy (401/403, via a self-contained\ndemo header-auth scheme), Mediarq.EntityFrameworkCore's domain events raise an in-process\nOrderConfirmedDomainEvent on confirm (distinct from OrderPlacedEvent's outbox delivery), and\nMediarq.HealthChecks exposes GET /health.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(deferred): add Mediarq.Deferred package for in-process deferred dispatch (#204)\n\nIDeferredDispatcher.SendLaterAsync/PublishLaterAsync queue a command or notification on a\nSystem.Threading.Channels-backed background worker (DeferredDispatchHostedService) instead of\nrunning its handler(s) inline, decoupling the caller from handler execution time. No external\ndependency, no persistent store — fills the gap between immediate Send/Publish and durable\nscheduling (Mediarq.Hangfire/Mediarq.Quartz) for the \"reliable in-process fire-and-forget\" case.\nA graceful host shutdown stops accepting new work and drains everything already queued before\nstopping, bounded by the host's own shutdown timeout; an exception in one item is logged and does\nnot stop the worker from processing the rest.\n\nCloses #184.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(dapr): add Mediarq.Dapr package for Dapr pub/sub integration (#205)\n\nIDaprPubSubEvent marks a notification for Dapr pub/sub via static abstract PubsubName/Topic\nmembers (not instance properties, since the subscribe side needs routing info before any\nnotification instance exists, and both directions reading the same statics means they can\nnever drift apart).\n\nPublish side: AddMediarqDaprPubSub<TNotification>() registers a forwarder that calls\nDaprClient.PublishEventAsync when the notification is published through Mediarq, mirroring\nMediarq.MassTransit's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: MapDaprPubSubSubscription<TNotification>() maps a minimal-API webhook that\nextracts the `data` field from the CloudEvents 1.0 envelope the Dapr sidecar delivers and\nrepublishes it through IPublisher, and MapDaprPubSubSubscribeEndpoint() serves the\n/dapr/subscribe discovery endpoint the sidecar queries at startup.\n\nCloses #190.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(rabbitmq): add Mediarq.RabbitMQ package for a lightweight broker bridge (#206)\n\nIRabbitMqEvent marks a notification for RabbitMQ via static abstract Exchange/Queue/RoutingKey\nmembers (same static-member rationale as Mediarq.Dapr's IDaprPubSubEvent: the subscriber needs\nrouting info before any instance exists, and both directions reading the same statics means\nthey can never drift apart).\n\nPublish side: AddMediarqRabbitMqPublisher<TNotification>() registers a forwarder that publishes\non a short-lived channel per call, mirroring Mediarq.MassTransit/Mediarq.Dapr's forwarder shape\n(runs alongside in-process handlers).\n\nSubscribe side: AddMediarqRabbitMqSubscriber<TNotification>() registers a background service\nthat declares the exchange/queue/binding, consumes with manual acknowledgement, and republishes\neach delivery through IPublisher — acking only after a successful publish, nacking without\nrequeue on failure so a poison message doesn't loop forever.\n\nThis package never owns the IConnection's lifecycle (bring your own) and does not implement\nduplicate-delivery detection (documented as a follow-up, not silently assumed) — a lightweight\nalternative to Mediarq.MassTransit for the simple pub/sub case, per #189.\n\nFirst half of #189 (RabbitMQ). The Azure Service Bus half is a separate follow-up PR.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(azureservicebus): add Mediarq.AzureServiceBus package, closing #189 (#207)\n\nIAzureServiceBusEvent marks a notification for Azure Service Bus via static abstract\nTopicName/SubscriptionName members (same static-member rationale as Mediarq.Dapr/Mediarq.RabbitMQ:\nthe subscriber needs routing info before any instance exists, and both directions reading the\nsame statics means they can never drift apart). Uses the topic+subscription pub/sub model; this\npackage does not provision the topic/subscription (pre-provision via portal/ARM/Bicep/\nServiceBusAdministrationClient).\n\nPublish side: AddMediarqAzureServiceBusPublisher<TNotification>() registers a forwarder that\nsends on a ServiceBusSender created per publish, mirroring Mediarq.MassTransit/Mediarq.Dapr/\nMediarq.RabbitMQ's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: AddMediarqAzureServiceBusSubscriber<TNotification>() registers a background\nservice that processes the subscription via ServiceBusProcessor and republishes each message\nthrough IPublisher — completing only after a successful publish, dead-lettering on failure (the\nService Bus analogue of \"nack without requeue\") so a poison message doesn't loop forever.\n\nThis package never owns the ServiceBusClient's lifecycle and does not implement\nduplicate-delivery detection — a lightweight alternative to Mediarq.MassTransit for the simple\npub/sub case, per #189.\n\nSecond and final half of #189 (Azure Service Bus). Closes #189.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspire): add Mediarq.Aspire package for .NET Aspire ServiceDefaults integration (#208)\n\nAddMediarqServiceDefaults() on IHostApplicationBuilder, meant to be called from inside a\nconsumer's own dotnet new aspire-servicedefaults-generated ServiceDefaults project alongside its\nown OpenTelemetry/service-discovery/resilience setup — additive, not a replacement.\n\nWires Mediarq.OpenTelemetry's tracing/metrics (AddMediarqInstrumentation on both the tracer and\nmeter providers) and Mediarq.HealthChecks' handler-registration check on top of whatever the\nAspire template already generated, so Mediarq dispatch spans/metrics and a missing/ambiguous\nhandler both show up in the Aspire dashboard. Deliberately does not reimplement OpenTelemetry\nexporter/service-discovery/resilience wiring or map /health and /alive endpoints itself — those\nremain the template's own concern.\n\nCloses #187.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(packaging): add Mediarq logo as the NuGet package icon (#211)\n\n* feat(packaging): add Mediarq logo and embed it as the NuGet package icon\n\nAdds assets/logo.svg (source) and assets/icon.png (256x256), wires\nPackageIcon into src/Directory.Build.props so every package under src/\nships the icon, and adds it to the Mediarq.Templates package as well.\n\n* docs(readme): display the Mediarq logo at the top of the README\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(grpc): add Mediarq.Grpc package for direct point-to-point notification transport (#212)\n\nCloses #36 (gRPC half; Quartz/Hangfire scheduled dispatch already shipped in #171).\n\nIGrpcNotificationEvent (static abstract string ServiceAddress) marks a\nnotification for gRPC delivery to another service. AddMediarqGrpcPublisher<T>()\nforwards it via GrpcNotificationForwarder<T>, which resolves a cached, reused\nHTTP/2 GrpcChannel per ServiceAddress (GrpcChannelCache, disposed by the\ncontainer at shutdown) and calls the generated NotificationServiceClient.\nAddMediarqGrpcSubscriptions() + MapMediarqGrpcNotificationService() +\nMapMediarqGrpcSubscription<T>() receive it back into the pipeline: every\nsubscribed type is multiplexed over one shared RPC method (Publish), keyed by\nthe envelope's type_name against a registry of compile-time-typed\ndeserialize-and-publish delegates (no runtime reflection on the dispatch\npath itself).\n\nShips its own compiled Protobuf/gRPC contract (Protos/notification.proto,\nGrpcServices=\"Both\") so consumers reference this package only, no protoc/\nGrpc.Tools needed downstream. Tried generating the contract with\n--csharp_opt=internal_access to avoid exposing it as public API surface;\nreverted after confirming it's a known limitation (the flag only applies to\nmessage types, not the grpc_csharp_plugin-generated service/client stubs,\ncausing an accessibility mismatch) — the generated surface is public,\ntracked in PublicAPI.Unshipped.txt like every other package's surface.\n\nGrpcChannelCache and GrpcNotificationForwarder's constructor had to be public\nrather than internal: Microsoft.Extensions.DependencyInjection's default\ncontainer only considers public constructors when activating a type, so an\ninternal-typed constructor parameter on a publicly-constructed type silently\nfails DI resolution (caught by two failing tests during development, fixed\nbefore this commit).\n\nVerified: full solution build (0 warnings/errors beyond pre-existing,\nunrelated ones), full test suite (18 new tests, all green), and an ad-hoc\nAOT publish scan of the subscribe side confirmed AddGrpc()/MapGrpcService()\nitself produces zero trim/AOT warnings — the only warning present is the\nreflection-based JsonSerializer.Deserialize<T> call, the same pre-existing,\nunannotated pattern already used in the Dapr/RabbitMQ/AzureServiceBus\npackages, not a new risk category introduced here.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(sourcegen): add MQ005/MQ006/MQ007 diagnostics for stream handlers and notifications (#217)\n\nMQ001/MQ002 already caught duplicate/missing IRequestHandler registrations for\ncommands and queries. The generator collects the same registration data for\nIStreamRequestHandler<,> and INotificationHandler<>, but never diagnosed it,\nso a missing stream handler or an orphan notification only surfaced as a\nruntime HandlerNotFoundException.\n\n- MQ005 (warning): multiple IStreamRequestHandler<,> for the same stream request\n- MQ006 (info): a declared IStreamRequest<T> with no handler in the assembly\n- MQ007 (info): a declared INotification with no handler in the assembly\n\nCloses #213.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ202 to flag duplicate Mediarq routes at compile time (#218)\n\nMapMediarq() maps every [MediarqGet]/[MediarqPost]/[MediarqPut]/[MediarqPatch]/\n[MediarqDelete]-attributed request type as a minimal API endpoint with zero\nuniqueness check across types. Two types declaring the same (HTTP method,\nroute pattern) pair only collided at ASP.NET Core's routing time -- an\nambiguous-match error on the first matching request -- never at build time.\n\nDuplicateRouteAnalyzer (MQ202) collects every Mediarq route attribute in the\ncompilation and flags a duplicate (method, pattern) pair declared by more than\none type, same by-name/by-namespace attribute matching as the existing\nMQ200/MQ201/MQ204 analyzers.\n\nCloses #214.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): declare OpenAPI response metadata for MapMediarq() endpoints (#219)\n\nMapMediarq() built its minimal-API delegates as Func<TRequest, ISender,\nCancellationToken, Task<IResult>>, returning a bare IResult from the Handle*\nhelpers. ASP.NET Core's built-in OpenAPI inference needs a statically-typed\nResults<...> union or explicit .Produces<T>() calls to infer a response\nschema -- neither was present, so every MapMediarq()-mapped endpoint showed\nup in Swagger with an untyped or absent response body.\n\nAttach explicit response metadata to the RouteHandlerBuilder returned by\neach MapGet/MapPost/etc call instead: 200/204 with the success type (driven\nby the response type -- Result, Result<T> or Unit), plus every failure\nstatus ResultError.Type can map to (400/401/403/404/409/500), using the\nexisting ErrorType -> HTTP status mapping in ResultHttpExtensions.\n\nCloses #215.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* docs(samples): showcase Result.WithNotifications(...) cascading in the WebApi sample (#221)\n\nAddOrderNoteHandler now attaches an OrderNoteAddedEvent to its successful\nResult instead of just returning it -- a third, lightweight notification\npattern next to the transactional outbox (CreateOrder) and domain events\n(ConfirmOrder) this sample already demonstrates side by side.\n\nVerified manually: created an order, POSTed a note, confirmed the\n[cascaded notification] log line fires from the new INotificationHandler.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#223)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#224)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(packaging): declare lib/ framework assets for Mediarq and Mediarq.Analyzers (#226)\n\nBoth packages intentionally ship no assembly of their own (Mediarq is a\nmeta-package bundling its dependencies, Mediarq.Analyzers ships its DLL\nonly under analyzers/dotnet/cs) — with no lib/ folder at all, NuGet.org\nshows 'There are no supported framework assets in this package' for\nboth, as seen live on v1.5.0.\n\nAdd empty lib/<tfm>/_._ marker files (the standard NuGet convention for\nthis exact case) so NuGet.org lists net8.0/net9.0/net10.0 for Mediarq\nand netstandard2.0 for Mediarq.Analyzers, without shipping a real\nassembly. Suppress the resulting NU5128 for Mediarq.Analyzers, whose\ndependencies are intentionally omitted from the nuspec.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-26T08:38:57+02:00",
          "tree_id": "fa5daa5454b15a84da3f01500fc3e12a5217083f",
          "url": "https://github.com/rouffou/mediarq/commit/fdf88ad959f5833c36e62de2424708fd45f34ae5"
        },
        "date": 1785048003921,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish",
            "value": 159.66572682062784,
            "unit": "ns",
            "range": "± 0.41002611807618655"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish",
            "value": 258.66602245966595,
            "unit": "ns",
            "range": "± 2.018436432927796"
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
          "id": "f5eb5b01c198672aaff100bea8fb663aa3fd5d01",
          "message": "perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T16:54:44+02:00",
          "tree_id": "baccd9fe7316e21da4755f3b2be14f4c0bfad995",
          "url": "https://github.com/rouffou/mediarq/commit/f5eb5b01c198672aaff100bea8fb663aa3fd5d01"
        },
        "date": 1784991361926,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 77.10729749997456,
            "unit": "ns",
            "range": "± 0.4387465724119741"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 262.9922612508138,
            "unit": "ns",
            "range": "± 1.4249610719055843"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 153.69901180267334,
            "unit": "ns",
            "range": "± 0.5174837053243482"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 230.64847342173258,
            "unit": "ns",
            "range": "± 0.20888879672907212"
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
          "id": "6082554feb0d9e4fa4a124df658c8caf903e491d",
          "message": "feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T19:53:13+02:00",
          "tree_id": "2181188561efe23fe0a666f586c0e3b8425c5220",
          "url": "https://github.com/rouffou/mediarq/commit/6082554feb0d9e4fa4a124df658c8caf903e491d"
        },
        "date": 1785002070048,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 93.07593204577763,
            "unit": "ns",
            "range": "± 0.3076289942979582"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 277.7081133524577,
            "unit": "ns",
            "range": "± 1.9566175017989798"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 165.23860816160837,
            "unit": "ns",
            "range": "± 1.0698289490229995"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 253.5234692891439,
            "unit": "ns",
            "range": "± 0.7456780905848353"
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
          "id": "ce2a5a519647a9dcdbff86bf2878ea78e8819293",
          "message": "Release v1.5.0 (#225)\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(quartz): add Mediarq.Quartz package for delayed/scheduled dispatch (#174)\n\nEnqueueAsync/ScheduleAsync extensions on IScheduler run a Mediarq ICommand\nas a Quartz.NET job through the real dispatch pipeline. The command is\nJSON-serialized (System.Text.Json) into the job's JobDataMap alongside its\nAssemblyQualifiedName and reconstructed when the trigger fires.\n\nVerified end-to-end against a real Quartz scheduler and worker\n(Quartz.Extensions.Hosting), not just the job-data shape in isolation.\nSame ICommand-only constraint and generic-over-the-concrete-type pattern\nas Mediarq.Hangfire (each extension captures the concrete command type at\nthe call site so the type-name-based round trip resolves correctly).\n\nCompletes #36 (Hangfire done in a prior PR; gRPC transport for\ncross-service notifications remains open, no immediate plan to pick it up).\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(healthchecks): add Mediarq.HealthChecks package for handler-registration validation (#192)\n\nCatches a missing or ambiguous command/query handler before it surfaces as a\nHandlerNotFoundException on first dispatch. Ships an IHealthCheck for a /health\nendpoint plus AddMediarqHandlerValidationOnStartup, which runs the same check\nonce during host startup and throws so the app fails fast on misconfiguration.\n\nCloses #188\n\n* feat(ci): track allocation regression alongside mean time in the benchmark guardrail (#193)\n\ngithub-action-benchmark's built-in benchmarkdotnet tool only reads BenchmarkDotNet's Mean\nstatistic, so allocation regressions could slip through even with the existing time-based\nalert. Add a benchmark-alloc job (per Send/Publish matrix entry) that converts the same\nBenchmarkDotNet JSON export into the customSmallerIsBetter format via a new converter\nscript and tracks Memory.BytesAllocatedPerOperation as its own alerted history series,\nreusing the artifact the benchmark job already produces instead of rerunning BenchmarkDotNet.\n\nCloses #179\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(testing): add Mediarq.Testing package with SpyMediator and fakes (#197)\n\nNew SpyMediator decorates the registered IMediator, recording every dispatched\nrequest/notification while still delegating to the real one -- handlers, validators\nand pipeline behaviors all run for real, only the bookkeeping is added. AddMediarqSpy()\ndecorates via Scrutor after AddMediarq/AddMediarqCore; ISender/IPublisher are covered\ntoo since both already resolve the current IMediator from the container.\n\nSpyMediatorAssertions (Sent<T>/HasSent<T>/Published<T>/HasPublished<T>) stays\nframework-agnostic so it pairs with whatever assertion library a consumer already uses.\n\nAlso ships FakeClock/FakeUserContext, settable implementations of IClock/IUserContext.\n\nCloses #185\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(efcore): add domain-event support to Mediarq.EntityFrameworkCore (#198)\n\nNew IHasDomainEvents (+ convenience AggregateRoot base class) and DomainEventsInterceptor,\na SaveChanges interceptor that collects and clears events staged on tracked aggregates\nright before the commit, then publishes them only once it actually succeeds -- a failed\ncommit discards the collected events rather than publishing them or re-raising them on\na retry.\n\nAddMediarqDomainEvents() registers the interceptor as scoped IInterceptor on the\napplication service provider, so it's picked up automatically by any AddDbContext<T>(...)\ncall without touching that call -- and scoped (not singleton) so it gets a fresh scoped\nIPublisher per DbContext construction instead of capturing the first one forever.\n\nAsync-only: IPublisher has no synchronous overload, so only SaveChangesAsync is\nintercepted.\n\nCloses #186\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): add automatic minimal API mapping (app.MapMediarq()) (#199)\n\nNew route attributes (MediarqGet/Post/Put/Patch/Delete) + MapMediarq(), which scans\nassemblies for attributed commands/queries and maps each directly as a minimal API\nendpoint, dispatching through ISender. GET/DELETE bind the request's members\nindividually from the route/query string ([AsParameters], no body); POST/PUT/PATCH\nbind the whole request from the JSON body. The response converts the same way\nToHttpResult() already does for Result/Result<T>; a no-result ICommand (response\nUnit) maps a successful dispatch to 204 No Content. An attributed type whose\nresponse is none of those three shapes throws InvalidOperationException at startup\nrather than failing silently.\n\nDelegates are built dynamically per discovered type via MakeGenericMethod against\nfour private generic handler methods (body/params x Result/Result<T>, plus two more\nfor Unit), so [AsParameters]/body-binding attribution on the closed generic method's\nparameters is inspected by RequestDelegateFactory exactly as it would be for a\nhand-written endpoint.\n\nReturns a RouteGroupBuilder so shared conventions (RequireAuthorization, WithTags,\n...) apply to every mapped endpoint at once.\n\nCloses #180\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(ratelimiting): add Mediarq.RateLimiting package for pipeline-level throttling (#200)\n\nNew IRateLimitedRequest marker (PolicyName + optional PartitionKey) and\nRateLimitingBehavior, built on System.Threading.RateLimiting -- no HTTP dependency,\nprotects any hot path directly in the pipeline. A named RateLimiterRegistry maps a\npolicy name to a PartitionedRateLimiter<string>; PartitionKey (or \"*\" when null)\nselects the partition, so different callers (e.g. per user) get independent limits\nunder the same policy.\n\nNo permit available throws RateLimitExceededException (PolicyName/PartitionKey/\nRetryAfter) rather than short-circuiting into a Result -- catch it via an\nIRequestExceptionHandler<,> or an ASP.NET Core exception handler to map it to a\n429, mirroring Polly's own RateLimiterRejectedException convention rather than\nforcing a Result-shaped response the way Mediarq.Authorization does.\n\nCloses #182\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ204 analyzer for a pipeline behavior that is never active (#201)\n\nNew InertConditionalBehaviorAnalyzer flags an IConditionalPipelineBehavior.IsActive\nimplementation that is syntactically always the literal false -- the behavior is\nregistered but can never participate in the pipeline for any request. Same\nsyntactic-only approach as MQ201 (PipelineBehaviorNextAnalyzer): only fires when the\ngetter is literally `false` (expression-bodied property, expression-bodied getter, or\na single `return false;`), so real conditional logic is never flagged regardless of\nhow it evaluates at runtime -- no full flow-analysis proof attempted.\n\nCloses #191\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* chore(samples): wire Authorization, RateLimiting, HealthChecks, domain events into the WebApi sample (#202)\n\nThe Orders sample only demonstrated the extensions that existed before this cycle. It now also\nshowcases the four added since: Mediarq.RateLimiting throttles order creation (429 on rejection),\nMediarq.Authorization protects order confirmation behind a policy (401/403, via a self-contained\ndemo header-auth scheme), Mediarq.EntityFrameworkCore's domain events raise an in-process\nOrderConfirmedDomainEvent on confirm (distinct from OrderPlacedEvent's outbox delivery), and\nMediarq.HealthChecks exposes GET /health.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(deferred): add Mediarq.Deferred package for in-process deferred dispatch (#204)\n\nIDeferredDispatcher.SendLaterAsync/PublishLaterAsync queue a command or notification on a\nSystem.Threading.Channels-backed background worker (DeferredDispatchHostedService) instead of\nrunning its handler(s) inline, decoupling the caller from handler execution time. No external\ndependency, no persistent store — fills the gap between immediate Send/Publish and durable\nscheduling (Mediarq.Hangfire/Mediarq.Quartz) for the \"reliable in-process fire-and-forget\" case.\nA graceful host shutdown stops accepting new work and drains everything already queued before\nstopping, bounded by the host's own shutdown timeout; an exception in one item is logged and does\nnot stop the worker from processing the rest.\n\nCloses #184.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(dapr): add Mediarq.Dapr package for Dapr pub/sub integration (#205)\n\nIDaprPubSubEvent marks a notification for Dapr pub/sub via static abstract PubsubName/Topic\nmembers (not instance properties, since the subscribe side needs routing info before any\nnotification instance exists, and both directions reading the same statics means they can\nnever drift apart).\n\nPublish side: AddMediarqDaprPubSub<TNotification>() registers a forwarder that calls\nDaprClient.PublishEventAsync when the notification is published through Mediarq, mirroring\nMediarq.MassTransit's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: MapDaprPubSubSubscription<TNotification>() maps a minimal-API webhook that\nextracts the `data` field from the CloudEvents 1.0 envelope the Dapr sidecar delivers and\nrepublishes it through IPublisher, and MapDaprPubSubSubscribeEndpoint() serves the\n/dapr/subscribe discovery endpoint the sidecar queries at startup.\n\nCloses #190.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(rabbitmq): add Mediarq.RabbitMQ package for a lightweight broker bridge (#206)\n\nIRabbitMqEvent marks a notification for RabbitMQ via static abstract Exchange/Queue/RoutingKey\nmembers (same static-member rationale as Mediarq.Dapr's IDaprPubSubEvent: the subscriber needs\nrouting info before any instance exists, and both directions reading the same statics means\nthey can never drift apart).\n\nPublish side: AddMediarqRabbitMqPublisher<TNotification>() registers a forwarder that publishes\non a short-lived channel per call, mirroring Mediarq.MassTransit/Mediarq.Dapr's forwarder shape\n(runs alongside in-process handlers).\n\nSubscribe side: AddMediarqRabbitMqSubscriber<TNotification>() registers a background service\nthat declares the exchange/queue/binding, consumes with manual acknowledgement, and republishes\neach delivery through IPublisher — acking only after a successful publish, nacking without\nrequeue on failure so a poison message doesn't loop forever.\n\nThis package never owns the IConnection's lifecycle (bring your own) and does not implement\nduplicate-delivery detection (documented as a follow-up, not silently assumed) — a lightweight\nalternative to Mediarq.MassTransit for the simple pub/sub case, per #189.\n\nFirst half of #189 (RabbitMQ). The Azure Service Bus half is a separate follow-up PR.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(azureservicebus): add Mediarq.AzureServiceBus package, closing #189 (#207)\n\nIAzureServiceBusEvent marks a notification for Azure Service Bus via static abstract\nTopicName/SubscriptionName members (same static-member rationale as Mediarq.Dapr/Mediarq.RabbitMQ:\nthe subscriber needs routing info before any instance exists, and both directions reading the\nsame statics means they can never drift apart). Uses the topic+subscription pub/sub model; this\npackage does not provision the topic/subscription (pre-provision via portal/ARM/Bicep/\nServiceBusAdministrationClient).\n\nPublish side: AddMediarqAzureServiceBusPublisher<TNotification>() registers a forwarder that\nsends on a ServiceBusSender created per publish, mirroring Mediarq.MassTransit/Mediarq.Dapr/\nMediarq.RabbitMQ's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: AddMediarqAzureServiceBusSubscriber<TNotification>() registers a background\nservice that processes the subscription via ServiceBusProcessor and republishes each message\nthrough IPublisher — completing only after a successful publish, dead-lettering on failure (the\nService Bus analogue of \"nack without requeue\") so a poison message doesn't loop forever.\n\nThis package never owns the ServiceBusClient's lifecycle and does not implement\nduplicate-delivery detection — a lightweight alternative to Mediarq.MassTransit for the simple\npub/sub case, per #189.\n\nSecond and final half of #189 (Azure Service Bus). Closes #189.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspire): add Mediarq.Aspire package for .NET Aspire ServiceDefaults integration (#208)\n\nAddMediarqServiceDefaults() on IHostApplicationBuilder, meant to be called from inside a\nconsumer's own dotnet new aspire-servicedefaults-generated ServiceDefaults project alongside its\nown OpenTelemetry/service-discovery/resilience setup — additive, not a replacement.\n\nWires Mediarq.OpenTelemetry's tracing/metrics (AddMediarqInstrumentation on both the tracer and\nmeter providers) and Mediarq.HealthChecks' handler-registration check on top of whatever the\nAspire template already generated, so Mediarq dispatch spans/metrics and a missing/ambiguous\nhandler both show up in the Aspire dashboard. Deliberately does not reimplement OpenTelemetry\nexporter/service-discovery/resilience wiring or map /health and /alive endpoints itself — those\nremain the template's own concern.\n\nCloses #187.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(packaging): add Mediarq logo as the NuGet package icon (#211)\n\n* feat(packaging): add Mediarq logo and embed it as the NuGet package icon\n\nAdds assets/logo.svg (source) and assets/icon.png (256x256), wires\nPackageIcon into src/Directory.Build.props so every package under src/\nships the icon, and adds it to the Mediarq.Templates package as well.\n\n* docs(readme): display the Mediarq logo at the top of the README\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(grpc): add Mediarq.Grpc package for direct point-to-point notification transport (#212)\n\nCloses #36 (gRPC half; Quartz/Hangfire scheduled dispatch already shipped in #171).\n\nIGrpcNotificationEvent (static abstract string ServiceAddress) marks a\nnotification for gRPC delivery to another service. AddMediarqGrpcPublisher<T>()\nforwards it via GrpcNotificationForwarder<T>, which resolves a cached, reused\nHTTP/2 GrpcChannel per ServiceAddress (GrpcChannelCache, disposed by the\ncontainer at shutdown) and calls the generated NotificationServiceClient.\nAddMediarqGrpcSubscriptions() + MapMediarqGrpcNotificationService() +\nMapMediarqGrpcSubscription<T>() receive it back into the pipeline: every\nsubscribed type is multiplexed over one shared RPC method (Publish), keyed by\nthe envelope's type_name against a registry of compile-time-typed\ndeserialize-and-publish delegates (no runtime reflection on the dispatch\npath itself).\n\nShips its own compiled Protobuf/gRPC contract (Protos/notification.proto,\nGrpcServices=\"Both\") so consumers reference this package only, no protoc/\nGrpc.Tools needed downstream. Tried generating the contract with\n--csharp_opt=internal_access to avoid exposing it as public API surface;\nreverted after confirming it's a known limitation (the flag only applies to\nmessage types, not the grpc_csharp_plugin-generated service/client stubs,\ncausing an accessibility mismatch) — the generated surface is public,\ntracked in PublicAPI.Unshipped.txt like every other package's surface.\n\nGrpcChannelCache and GrpcNotificationForwarder's constructor had to be public\nrather than internal: Microsoft.Extensions.DependencyInjection's default\ncontainer only considers public constructors when activating a type, so an\ninternal-typed constructor parameter on a publicly-constructed type silently\nfails DI resolution (caught by two failing tests during development, fixed\nbefore this commit).\n\nVerified: full solution build (0 warnings/errors beyond pre-existing,\nunrelated ones), full test suite (18 new tests, all green), and an ad-hoc\nAOT publish scan of the subscribe side confirmed AddGrpc()/MapGrpcService()\nitself produces zero trim/AOT warnings — the only warning present is the\nreflection-based JsonSerializer.Deserialize<T> call, the same pre-existing,\nunannotated pattern already used in the Dapr/RabbitMQ/AzureServiceBus\npackages, not a new risk category introduced here.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(sourcegen): add MQ005/MQ006/MQ007 diagnostics for stream handlers and notifications (#217)\n\nMQ001/MQ002 already caught duplicate/missing IRequestHandler registrations for\ncommands and queries. The generator collects the same registration data for\nIStreamRequestHandler<,> and INotificationHandler<>, but never diagnosed it,\nso a missing stream handler or an orphan notification only surfaced as a\nruntime HandlerNotFoundException.\n\n- MQ005 (warning): multiple IStreamRequestHandler<,> for the same stream request\n- MQ006 (info): a declared IStreamRequest<T> with no handler in the assembly\n- MQ007 (info): a declared INotification with no handler in the assembly\n\nCloses #213.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ202 to flag duplicate Mediarq routes at compile time (#218)\n\nMapMediarq() maps every [MediarqGet]/[MediarqPost]/[MediarqPut]/[MediarqPatch]/\n[MediarqDelete]-attributed request type as a minimal API endpoint with zero\nuniqueness check across types. Two types declaring the same (HTTP method,\nroute pattern) pair only collided at ASP.NET Core's routing time -- an\nambiguous-match error on the first matching request -- never at build time.\n\nDuplicateRouteAnalyzer (MQ202) collects every Mediarq route attribute in the\ncompilation and flags a duplicate (method, pattern) pair declared by more than\none type, same by-name/by-namespace attribute matching as the existing\nMQ200/MQ201/MQ204 analyzers.\n\nCloses #214.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): declare OpenAPI response metadata for MapMediarq() endpoints (#219)\n\nMapMediarq() built its minimal-API delegates as Func<TRequest, ISender,\nCancellationToken, Task<IResult>>, returning a bare IResult from the Handle*\nhelpers. ASP.NET Core's built-in OpenAPI inference needs a statically-typed\nResults<...> union or explicit .Produces<T>() calls to infer a response\nschema -- neither was present, so every MapMediarq()-mapped endpoint showed\nup in Swagger with an untyped or absent response body.\n\nAttach explicit response metadata to the RouteHandlerBuilder returned by\neach MapGet/MapPost/etc call instead: 200/204 with the success type (driven\nby the response type -- Result, Result<T> or Unit), plus every failure\nstatus ResultError.Type can map to (400/401/403/404/409/500), using the\nexisting ErrorType -> HTTP status mapping in ResultHttpExtensions.\n\nCloses #215.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* docs(samples): showcase Result.WithNotifications(...) cascading in the WebApi sample (#221)\n\nAddOrderNoteHandler now attaches an OrderNoteAddedEvent to its successful\nResult instead of just returning it -- a third, lightweight notification\npattern next to the transactional outbox (CreateOrder) and domain events\n(ConfirmOrder) this sample already demonstrates side by side.\n\nVerified manually: created an order, POSTed a note, confirmed the\n[cascaded notification] log line fires from the new INotificationHandler.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#223)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#224)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T21:49:04+02:00",
          "tree_id": "bc44ddcf81214df474e2f631cd79e3daa27b7f65",
          "url": "https://github.com/rouffou/mediarq/commit/ce2a5a519647a9dcdbff86bf2878ea78e8819293"
        },
        "date": 1785009027654,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 100.67504739761353,
            "unit": "ns",
            "range": "± 0.24258690414718825"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 292.5930298169454,
            "unit": "ns",
            "range": "± 0.9727335521909755"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 159.6219733953476,
            "unit": "ns",
            "range": "± 0.34707407239632043"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 263.1247394879659,
            "unit": "ns",
            "range": "± 2.9968234780050453"
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
          "id": "37cb619bb7460fc2603d44feb78bbf2f3444b50e",
          "message": "fix(packaging): declare lib/ framework assets for Mediarq and Mediarq.Analyzers (#226)\n\nBoth packages intentionally ship no assembly of their own (Mediarq is a\nmeta-package bundling its dependencies, Mediarq.Analyzers ships its DLL\nonly under analyzers/dotnet/cs) — with no lib/ folder at all, NuGet.org\nshows 'There are no supported framework assets in this package' for\nboth, as seen live on v1.5.0.\n\nAdd empty lib/<tfm>/_._ marker files (the standard NuGet convention for\nthis exact case) so NuGet.org lists net8.0/net9.0/net10.0 for Mediarq\nand netstandard2.0 for Mediarq.Analyzers, without shipping a real\nassembly. Suppress the resulting NU5128 for Mediarq.Analyzers, whose\ndependencies are intentionally omitted from the nuspec.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-26T08:33:36+02:00",
          "tree_id": "fa5daa5454b15a84da3f01500fc3e12a5217083f",
          "url": "https://github.com/rouffou/mediarq/commit/37cb619bb7460fc2603d44feb78bbf2f3444b50e"
        },
        "date": 1785047691070,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 97.01852971315384,
            "unit": "ns",
            "range": "± 0.3789865247616951"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 297.10144933064777,
            "unit": "ns",
            "range": "± 2.7007119495556724"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 159.2544485727946,
            "unit": "ns",
            "range": "± 2.019055003778052"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 250.80157343546549,
            "unit": "ns",
            "range": "± 1.538340185348473"
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
          "id": "fdf88ad959f5833c36e62de2424708fd45f34ae5",
          "message": "Release v1.5.1 (#227)\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(quartz): add Mediarq.Quartz package for delayed/scheduled dispatch (#174)\n\nEnqueueAsync/ScheduleAsync extensions on IScheduler run a Mediarq ICommand\nas a Quartz.NET job through the real dispatch pipeline. The command is\nJSON-serialized (System.Text.Json) into the job's JobDataMap alongside its\nAssemblyQualifiedName and reconstructed when the trigger fires.\n\nVerified end-to-end against a real Quartz scheduler and worker\n(Quartz.Extensions.Hosting), not just the job-data shape in isolation.\nSame ICommand-only constraint and generic-over-the-concrete-type pattern\nas Mediarq.Hangfire (each extension captures the concrete command type at\nthe call site so the type-name-based round trip resolves correctly).\n\nCompletes #36 (Hangfire done in a prior PR; gRPC transport for\ncross-service notifications remains open, no immediate plan to pick it up).\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(healthchecks): add Mediarq.HealthChecks package for handler-registration validation (#192)\n\nCatches a missing or ambiguous command/query handler before it surfaces as a\nHandlerNotFoundException on first dispatch. Ships an IHealthCheck for a /health\nendpoint plus AddMediarqHandlerValidationOnStartup, which runs the same check\nonce during host startup and throws so the app fails fast on misconfiguration.\n\nCloses #188\n\n* feat(ci): track allocation regression alongside mean time in the benchmark guardrail (#193)\n\ngithub-action-benchmark's built-in benchmarkdotnet tool only reads BenchmarkDotNet's Mean\nstatistic, so allocation regressions could slip through even with the existing time-based\nalert. Add a benchmark-alloc job (per Send/Publish matrix entry) that converts the same\nBenchmarkDotNet JSON export into the customSmallerIsBetter format via a new converter\nscript and tracks Memory.BytesAllocatedPerOperation as its own alerted history series,\nreusing the artifact the benchmark job already produces instead of rerunning BenchmarkDotNet.\n\nCloses #179\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(testing): add Mediarq.Testing package with SpyMediator and fakes (#197)\n\nNew SpyMediator decorates the registered IMediator, recording every dispatched\nrequest/notification while still delegating to the real one -- handlers, validators\nand pipeline behaviors all run for real, only the bookkeeping is added. AddMediarqSpy()\ndecorates via Scrutor after AddMediarq/AddMediarqCore; ISender/IPublisher are covered\ntoo since both already resolve the current IMediator from the container.\n\nSpyMediatorAssertions (Sent<T>/HasSent<T>/Published<T>/HasPublished<T>) stays\nframework-agnostic so it pairs with whatever assertion library a consumer already uses.\n\nAlso ships FakeClock/FakeUserContext, settable implementations of IClock/IUserContext.\n\nCloses #185\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(efcore): add domain-event support to Mediarq.EntityFrameworkCore (#198)\n\nNew IHasDomainEvents (+ convenience AggregateRoot base class) and DomainEventsInterceptor,\na SaveChanges interceptor that collects and clears events staged on tracked aggregates\nright before the commit, then publishes them only once it actually succeeds -- a failed\ncommit discards the collected events rather than publishing them or re-raising them on\na retry.\n\nAddMediarqDomainEvents() registers the interceptor as scoped IInterceptor on the\napplication service provider, so it's picked up automatically by any AddDbContext<T>(...)\ncall without touching that call -- and scoped (not singleton) so it gets a fresh scoped\nIPublisher per DbContext construction instead of capturing the first one forever.\n\nAsync-only: IPublisher has no synchronous overload, so only SaveChangesAsync is\nintercepted.\n\nCloses #186\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): add automatic minimal API mapping (app.MapMediarq()) (#199)\n\nNew route attributes (MediarqGet/Post/Put/Patch/Delete) + MapMediarq(), which scans\nassemblies for attributed commands/queries and maps each directly as a minimal API\nendpoint, dispatching through ISender. GET/DELETE bind the request's members\nindividually from the route/query string ([AsParameters], no body); POST/PUT/PATCH\nbind the whole request from the JSON body. The response converts the same way\nToHttpResult() already does for Result/Result<T>; a no-result ICommand (response\nUnit) maps a successful dispatch to 204 No Content. An attributed type whose\nresponse is none of those three shapes throws InvalidOperationException at startup\nrather than failing silently.\n\nDelegates are built dynamically per discovered type via MakeGenericMethod against\nfour private generic handler methods (body/params x Result/Result<T>, plus two more\nfor Unit), so [AsParameters]/body-binding attribution on the closed generic method's\nparameters is inspected by RequestDelegateFactory exactly as it would be for a\nhand-written endpoint.\n\nReturns a RouteGroupBuilder so shared conventions (RequireAuthorization, WithTags,\n...) apply to every mapped endpoint at once.\n\nCloses #180\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(ratelimiting): add Mediarq.RateLimiting package for pipeline-level throttling (#200)\n\nNew IRateLimitedRequest marker (PolicyName + optional PartitionKey) and\nRateLimitingBehavior, built on System.Threading.RateLimiting -- no HTTP dependency,\nprotects any hot path directly in the pipeline. A named RateLimiterRegistry maps a\npolicy name to a PartitionedRateLimiter<string>; PartitionKey (or \"*\" when null)\nselects the partition, so different callers (e.g. per user) get independent limits\nunder the same policy.\n\nNo permit available throws RateLimitExceededException (PolicyName/PartitionKey/\nRetryAfter) rather than short-circuiting into a Result -- catch it via an\nIRequestExceptionHandler<,> or an ASP.NET Core exception handler to map it to a\n429, mirroring Polly's own RateLimiterRejectedException convention rather than\nforcing a Result-shaped response the way Mediarq.Authorization does.\n\nCloses #182\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ204 analyzer for a pipeline behavior that is never active (#201)\n\nNew InertConditionalBehaviorAnalyzer flags an IConditionalPipelineBehavior.IsActive\nimplementation that is syntactically always the literal false -- the behavior is\nregistered but can never participate in the pipeline for any request. Same\nsyntactic-only approach as MQ201 (PipelineBehaviorNextAnalyzer): only fires when the\ngetter is literally `false` (expression-bodied property, expression-bodied getter, or\na single `return false;`), so real conditional logic is never flagged regardless of\nhow it evaluates at runtime -- no full flow-analysis proof attempted.\n\nCloses #191\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* chore(samples): wire Authorization, RateLimiting, HealthChecks, domain events into the WebApi sample (#202)\n\nThe Orders sample only demonstrated the extensions that existed before this cycle. It now also\nshowcases the four added since: Mediarq.RateLimiting throttles order creation (429 on rejection),\nMediarq.Authorization protects order confirmation behind a policy (401/403, via a self-contained\ndemo header-auth scheme), Mediarq.EntityFrameworkCore's domain events raise an in-process\nOrderConfirmedDomainEvent on confirm (distinct from OrderPlacedEvent's outbox delivery), and\nMediarq.HealthChecks exposes GET /health.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(deferred): add Mediarq.Deferred package for in-process deferred dispatch (#204)\n\nIDeferredDispatcher.SendLaterAsync/PublishLaterAsync queue a command or notification on a\nSystem.Threading.Channels-backed background worker (DeferredDispatchHostedService) instead of\nrunning its handler(s) inline, decoupling the caller from handler execution time. No external\ndependency, no persistent store — fills the gap between immediate Send/Publish and durable\nscheduling (Mediarq.Hangfire/Mediarq.Quartz) for the \"reliable in-process fire-and-forget\" case.\nA graceful host shutdown stops accepting new work and drains everything already queued before\nstopping, bounded by the host's own shutdown timeout; an exception in one item is logged and does\nnot stop the worker from processing the rest.\n\nCloses #184.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(dapr): add Mediarq.Dapr package for Dapr pub/sub integration (#205)\n\nIDaprPubSubEvent marks a notification for Dapr pub/sub via static abstract PubsubName/Topic\nmembers (not instance properties, since the subscribe side needs routing info before any\nnotification instance exists, and both directions reading the same statics means they can\nnever drift apart).\n\nPublish side: AddMediarqDaprPubSub<TNotification>() registers a forwarder that calls\nDaprClient.PublishEventAsync when the notification is published through Mediarq, mirroring\nMediarq.MassTransit's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: MapDaprPubSubSubscription<TNotification>() maps a minimal-API webhook that\nextracts the `data` field from the CloudEvents 1.0 envelope the Dapr sidecar delivers and\nrepublishes it through IPublisher, and MapDaprPubSubSubscribeEndpoint() serves the\n/dapr/subscribe discovery endpoint the sidecar queries at startup.\n\nCloses #190.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(rabbitmq): add Mediarq.RabbitMQ package for a lightweight broker bridge (#206)\n\nIRabbitMqEvent marks a notification for RabbitMQ via static abstract Exchange/Queue/RoutingKey\nmembers (same static-member rationale as Mediarq.Dapr's IDaprPubSubEvent: the subscriber needs\nrouting info before any instance exists, and both directions reading the same statics means\nthey can never drift apart).\n\nPublish side: AddMediarqRabbitMqPublisher<TNotification>() registers a forwarder that publishes\non a short-lived channel per call, mirroring Mediarq.MassTransit/Mediarq.Dapr's forwarder shape\n(runs alongside in-process handlers).\n\nSubscribe side: AddMediarqRabbitMqSubscriber<TNotification>() registers a background service\nthat declares the exchange/queue/binding, consumes with manual acknowledgement, and republishes\neach delivery through IPublisher — acking only after a successful publish, nacking without\nrequeue on failure so a poison message doesn't loop forever.\n\nThis package never owns the IConnection's lifecycle (bring your own) and does not implement\nduplicate-delivery detection (documented as a follow-up, not silently assumed) — a lightweight\nalternative to Mediarq.MassTransit for the simple pub/sub case, per #189.\n\nFirst half of #189 (RabbitMQ). The Azure Service Bus half is a separate follow-up PR.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(azureservicebus): add Mediarq.AzureServiceBus package, closing #189 (#207)\n\nIAzureServiceBusEvent marks a notification for Azure Service Bus via static abstract\nTopicName/SubscriptionName members (same static-member rationale as Mediarq.Dapr/Mediarq.RabbitMQ:\nthe subscriber needs routing info before any instance exists, and both directions reading the\nsame statics means they can never drift apart). Uses the topic+subscription pub/sub model; this\npackage does not provision the topic/subscription (pre-provision via portal/ARM/Bicep/\nServiceBusAdministrationClient).\n\nPublish side: AddMediarqAzureServiceBusPublisher<TNotification>() registers a forwarder that\nsends on a ServiceBusSender created per publish, mirroring Mediarq.MassTransit/Mediarq.Dapr/\nMediarq.RabbitMQ's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: AddMediarqAzureServiceBusSubscriber<TNotification>() registers a background\nservice that processes the subscription via ServiceBusProcessor and republishes each message\nthrough IPublisher — completing only after a successful publish, dead-lettering on failure (the\nService Bus analogue of \"nack without requeue\") so a poison message doesn't loop forever.\n\nThis package never owns the ServiceBusClient's lifecycle and does not implement\nduplicate-delivery detection — a lightweight alternative to Mediarq.MassTransit for the simple\npub/sub case, per #189.\n\nSecond and final half of #189 (Azure Service Bus). Closes #189.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspire): add Mediarq.Aspire package for .NET Aspire ServiceDefaults integration (#208)\n\nAddMediarqServiceDefaults() on IHostApplicationBuilder, meant to be called from inside a\nconsumer's own dotnet new aspire-servicedefaults-generated ServiceDefaults project alongside its\nown OpenTelemetry/service-discovery/resilience setup — additive, not a replacement.\n\nWires Mediarq.OpenTelemetry's tracing/metrics (AddMediarqInstrumentation on both the tracer and\nmeter providers) and Mediarq.HealthChecks' handler-registration check on top of whatever the\nAspire template already generated, so Mediarq dispatch spans/metrics and a missing/ambiguous\nhandler both show up in the Aspire dashboard. Deliberately does not reimplement OpenTelemetry\nexporter/service-discovery/resilience wiring or map /health and /alive endpoints itself — those\nremain the template's own concern.\n\nCloses #187.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(packaging): add Mediarq logo as the NuGet package icon (#211)\n\n* feat(packaging): add Mediarq logo and embed it as the NuGet package icon\n\nAdds assets/logo.svg (source) and assets/icon.png (256x256), wires\nPackageIcon into src/Directory.Build.props so every package under src/\nships the icon, and adds it to the Mediarq.Templates package as well.\n\n* docs(readme): display the Mediarq logo at the top of the README\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(grpc): add Mediarq.Grpc package for direct point-to-point notification transport (#212)\n\nCloses #36 (gRPC half; Quartz/Hangfire scheduled dispatch already shipped in #171).\n\nIGrpcNotificationEvent (static abstract string ServiceAddress) marks a\nnotification for gRPC delivery to another service. AddMediarqGrpcPublisher<T>()\nforwards it via GrpcNotificationForwarder<T>, which resolves a cached, reused\nHTTP/2 GrpcChannel per ServiceAddress (GrpcChannelCache, disposed by the\ncontainer at shutdown) and calls the generated NotificationServiceClient.\nAddMediarqGrpcSubscriptions() + MapMediarqGrpcNotificationService() +\nMapMediarqGrpcSubscription<T>() receive it back into the pipeline: every\nsubscribed type is multiplexed over one shared RPC method (Publish), keyed by\nthe envelope's type_name against a registry of compile-time-typed\ndeserialize-and-publish delegates (no runtime reflection on the dispatch\npath itself).\n\nShips its own compiled Protobuf/gRPC contract (Protos/notification.proto,\nGrpcServices=\"Both\") so consumers reference this package only, no protoc/\nGrpc.Tools needed downstream. Tried generating the contract with\n--csharp_opt=internal_access to avoid exposing it as public API surface;\nreverted after confirming it's a known limitation (the flag only applies to\nmessage types, not the grpc_csharp_plugin-generated service/client stubs,\ncausing an accessibility mismatch) — the generated surface is public,\ntracked in PublicAPI.Unshipped.txt like every other package's surface.\n\nGrpcChannelCache and GrpcNotificationForwarder's constructor had to be public\nrather than internal: Microsoft.Extensions.DependencyInjection's default\ncontainer only considers public constructors when activating a type, so an\ninternal-typed constructor parameter on a publicly-constructed type silently\nfails DI resolution (caught by two failing tests during development, fixed\nbefore this commit).\n\nVerified: full solution build (0 warnings/errors beyond pre-existing,\nunrelated ones), full test suite (18 new tests, all green), and an ad-hoc\nAOT publish scan of the subscribe side confirmed AddGrpc()/MapGrpcService()\nitself produces zero trim/AOT warnings — the only warning present is the\nreflection-based JsonSerializer.Deserialize<T> call, the same pre-existing,\nunannotated pattern already used in the Dapr/RabbitMQ/AzureServiceBus\npackages, not a new risk category introduced here.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(sourcegen): add MQ005/MQ006/MQ007 diagnostics for stream handlers and notifications (#217)\n\nMQ001/MQ002 already caught duplicate/missing IRequestHandler registrations for\ncommands and queries. The generator collects the same registration data for\nIStreamRequestHandler<,> and INotificationHandler<>, but never diagnosed it,\nso a missing stream handler or an orphan notification only surfaced as a\nruntime HandlerNotFoundException.\n\n- MQ005 (warning): multiple IStreamRequestHandler<,> for the same stream request\n- MQ006 (info): a declared IStreamRequest<T> with no handler in the assembly\n- MQ007 (info): a declared INotification with no handler in the assembly\n\nCloses #213.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ202 to flag duplicate Mediarq routes at compile time (#218)\n\nMapMediarq() maps every [MediarqGet]/[MediarqPost]/[MediarqPut]/[MediarqPatch]/\n[MediarqDelete]-attributed request type as a minimal API endpoint with zero\nuniqueness check across types. Two types declaring the same (HTTP method,\nroute pattern) pair only collided at ASP.NET Core's routing time -- an\nambiguous-match error on the first matching request -- never at build time.\n\nDuplicateRouteAnalyzer (MQ202) collects every Mediarq route attribute in the\ncompilation and flags a duplicate (method, pattern) pair declared by more than\none type, same by-name/by-namespace attribute matching as the existing\nMQ200/MQ201/MQ204 analyzers.\n\nCloses #214.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): declare OpenAPI response metadata for MapMediarq() endpoints (#219)\n\nMapMediarq() built its minimal-API delegates as Func<TRequest, ISender,\nCancellationToken, Task<IResult>>, returning a bare IResult from the Handle*\nhelpers. ASP.NET Core's built-in OpenAPI inference needs a statically-typed\nResults<...> union or explicit .Produces<T>() calls to infer a response\nschema -- neither was present, so every MapMediarq()-mapped endpoint showed\nup in Swagger with an untyped or absent response body.\n\nAttach explicit response metadata to the RouteHandlerBuilder returned by\neach MapGet/MapPost/etc call instead: 200/204 with the success type (driven\nby the response type -- Result, Result<T> or Unit), plus every failure\nstatus ResultError.Type can map to (400/401/403/404/409/500), using the\nexisting ErrorType -> HTTP status mapping in ResultHttpExtensions.\n\nCloses #215.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* docs(samples): showcase Result.WithNotifications(...) cascading in the WebApi sample (#221)\n\nAddOrderNoteHandler now attaches an OrderNoteAddedEvent to its successful\nResult instead of just returning it -- a third, lightweight notification\npattern next to the transactional outbox (CreateOrder) and domain events\n(ConfirmOrder) this sample already demonstrates side by side.\n\nVerified manually: created an order, POSTed a note, confirmed the\n[cascaded notification] log line fires from the new INotificationHandler.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#223)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#224)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(packaging): declare lib/ framework assets for Mediarq and Mediarq.Analyzers (#226)\n\nBoth packages intentionally ship no assembly of their own (Mediarq is a\nmeta-package bundling its dependencies, Mediarq.Analyzers ships its DLL\nonly under analyzers/dotnet/cs) — with no lib/ folder at all, NuGet.org\nshows 'There are no supported framework assets in this package' for\nboth, as seen live on v1.5.0.\n\nAdd empty lib/<tfm>/_._ marker files (the standard NuGet convention for\nthis exact case) so NuGet.org lists net8.0/net9.0/net10.0 for Mediarq\nand netstandard2.0 for Mediarq.Analyzers, without shipping a real\nassembly. Suppress the resulting NU5128 for Mediarq.Analyzers, whose\ndependencies are intentionally omitted from the nuspec.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-26T08:38:57+02:00",
          "tree_id": "fa5daa5454b15a84da3f01500fc3e12a5217083f",
          "url": "https://github.com/rouffou/mediarq/commit/fdf88ad959f5833c36e62de2424708fd45f34ae5"
        },
        "date": 1785048017040,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send",
            "value": 97.79052686691284,
            "unit": "ns",
            "range": "± 0.6796731815325346"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send",
            "value": 281.51672554016113,
            "unit": "ns",
            "range": "± 1.774383523629678"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean",
            "value": 150.70906154314676,
            "unit": "ns",
            "range": "± 1.905948749184157"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain",
            "value": 244.9495380719503,
            "unit": "ns",
            "range": "± 0.42490452708667253"
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
        "date": 1784988042260,
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
          "id": "f5eb5b01c198672aaff100bea8fb663aa3fd5d01",
          "message": "perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T16:54:44+02:00",
          "tree_id": "baccd9fe7316e21da4755f3b2be14f4c0bfad995",
          "url": "https://github.com/rouffou/mediarq/commit/f5eb5b01c198672aaff100bea8fb663aa3fd5d01"
        },
        "date": 1784991373364,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send - Allocated",
            "value": 224,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send - Allocated",
            "value": 552,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean - Allocated",
            "value": 248,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain - Allocated",
            "value": 440,
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
          "id": "6082554feb0d9e4fa4a124df658c8caf903e491d",
          "message": "feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T19:53:13+02:00",
          "tree_id": "2181188561efe23fe0a666f586c0e3b8425c5220",
          "url": "https://github.com/rouffou/mediarq/commit/6082554feb0d9e4fa4a124df658c8caf903e491d"
        },
        "date": 1785002084042,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send - Allocated",
            "value": 224,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send - Allocated",
            "value": 560,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean - Allocated",
            "value": 256,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain - Allocated",
            "value": 440,
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
          "id": "ce2a5a519647a9dcdbff86bf2878ea78e8819293",
          "message": "Release v1.5.0 (#225)\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(quartz): add Mediarq.Quartz package for delayed/scheduled dispatch (#174)\n\nEnqueueAsync/ScheduleAsync extensions on IScheduler run a Mediarq ICommand\nas a Quartz.NET job through the real dispatch pipeline. The command is\nJSON-serialized (System.Text.Json) into the job's JobDataMap alongside its\nAssemblyQualifiedName and reconstructed when the trigger fires.\n\nVerified end-to-end against a real Quartz scheduler and worker\n(Quartz.Extensions.Hosting), not just the job-data shape in isolation.\nSame ICommand-only constraint and generic-over-the-concrete-type pattern\nas Mediarq.Hangfire (each extension captures the concrete command type at\nthe call site so the type-name-based round trip resolves correctly).\n\nCompletes #36 (Hangfire done in a prior PR; gRPC transport for\ncross-service notifications remains open, no immediate plan to pick it up).\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(healthchecks): add Mediarq.HealthChecks package for handler-registration validation (#192)\n\nCatches a missing or ambiguous command/query handler before it surfaces as a\nHandlerNotFoundException on first dispatch. Ships an IHealthCheck for a /health\nendpoint plus AddMediarqHandlerValidationOnStartup, which runs the same check\nonce during host startup and throws so the app fails fast on misconfiguration.\n\nCloses #188\n\n* feat(ci): track allocation regression alongside mean time in the benchmark guardrail (#193)\n\ngithub-action-benchmark's built-in benchmarkdotnet tool only reads BenchmarkDotNet's Mean\nstatistic, so allocation regressions could slip through even with the existing time-based\nalert. Add a benchmark-alloc job (per Send/Publish matrix entry) that converts the same\nBenchmarkDotNet JSON export into the customSmallerIsBetter format via a new converter\nscript and tracks Memory.BytesAllocatedPerOperation as its own alerted history series,\nreusing the artifact the benchmark job already produces instead of rerunning BenchmarkDotNet.\n\nCloses #179\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(testing): add Mediarq.Testing package with SpyMediator and fakes (#197)\n\nNew SpyMediator decorates the registered IMediator, recording every dispatched\nrequest/notification while still delegating to the real one -- handlers, validators\nand pipeline behaviors all run for real, only the bookkeeping is added. AddMediarqSpy()\ndecorates via Scrutor after AddMediarq/AddMediarqCore; ISender/IPublisher are covered\ntoo since both already resolve the current IMediator from the container.\n\nSpyMediatorAssertions (Sent<T>/HasSent<T>/Published<T>/HasPublished<T>) stays\nframework-agnostic so it pairs with whatever assertion library a consumer already uses.\n\nAlso ships FakeClock/FakeUserContext, settable implementations of IClock/IUserContext.\n\nCloses #185\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(efcore): add domain-event support to Mediarq.EntityFrameworkCore (#198)\n\nNew IHasDomainEvents (+ convenience AggregateRoot base class) and DomainEventsInterceptor,\na SaveChanges interceptor that collects and clears events staged on tracked aggregates\nright before the commit, then publishes them only once it actually succeeds -- a failed\ncommit discards the collected events rather than publishing them or re-raising them on\na retry.\n\nAddMediarqDomainEvents() registers the interceptor as scoped IInterceptor on the\napplication service provider, so it's picked up automatically by any AddDbContext<T>(...)\ncall without touching that call -- and scoped (not singleton) so it gets a fresh scoped\nIPublisher per DbContext construction instead of capturing the first one forever.\n\nAsync-only: IPublisher has no synchronous overload, so only SaveChangesAsync is\nintercepted.\n\nCloses #186\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): add automatic minimal API mapping (app.MapMediarq()) (#199)\n\nNew route attributes (MediarqGet/Post/Put/Patch/Delete) + MapMediarq(), which scans\nassemblies for attributed commands/queries and maps each directly as a minimal API\nendpoint, dispatching through ISender. GET/DELETE bind the request's members\nindividually from the route/query string ([AsParameters], no body); POST/PUT/PATCH\nbind the whole request from the JSON body. The response converts the same way\nToHttpResult() already does for Result/Result<T>; a no-result ICommand (response\nUnit) maps a successful dispatch to 204 No Content. An attributed type whose\nresponse is none of those three shapes throws InvalidOperationException at startup\nrather than failing silently.\n\nDelegates are built dynamically per discovered type via MakeGenericMethod against\nfour private generic handler methods (body/params x Result/Result<T>, plus two more\nfor Unit), so [AsParameters]/body-binding attribution on the closed generic method's\nparameters is inspected by RequestDelegateFactory exactly as it would be for a\nhand-written endpoint.\n\nReturns a RouteGroupBuilder so shared conventions (RequireAuthorization, WithTags,\n...) apply to every mapped endpoint at once.\n\nCloses #180\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(ratelimiting): add Mediarq.RateLimiting package for pipeline-level throttling (#200)\n\nNew IRateLimitedRequest marker (PolicyName + optional PartitionKey) and\nRateLimitingBehavior, built on System.Threading.RateLimiting -- no HTTP dependency,\nprotects any hot path directly in the pipeline. A named RateLimiterRegistry maps a\npolicy name to a PartitionedRateLimiter<string>; PartitionKey (or \"*\" when null)\nselects the partition, so different callers (e.g. per user) get independent limits\nunder the same policy.\n\nNo permit available throws RateLimitExceededException (PolicyName/PartitionKey/\nRetryAfter) rather than short-circuiting into a Result -- catch it via an\nIRequestExceptionHandler<,> or an ASP.NET Core exception handler to map it to a\n429, mirroring Polly's own RateLimiterRejectedException convention rather than\nforcing a Result-shaped response the way Mediarq.Authorization does.\n\nCloses #182\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ204 analyzer for a pipeline behavior that is never active (#201)\n\nNew InertConditionalBehaviorAnalyzer flags an IConditionalPipelineBehavior.IsActive\nimplementation that is syntactically always the literal false -- the behavior is\nregistered but can never participate in the pipeline for any request. Same\nsyntactic-only approach as MQ201 (PipelineBehaviorNextAnalyzer): only fires when the\ngetter is literally `false` (expression-bodied property, expression-bodied getter, or\na single `return false;`), so real conditional logic is never flagged regardless of\nhow it evaluates at runtime -- no full flow-analysis proof attempted.\n\nCloses #191\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* chore(samples): wire Authorization, RateLimiting, HealthChecks, domain events into the WebApi sample (#202)\n\nThe Orders sample only demonstrated the extensions that existed before this cycle. It now also\nshowcases the four added since: Mediarq.RateLimiting throttles order creation (429 on rejection),\nMediarq.Authorization protects order confirmation behind a policy (401/403, via a self-contained\ndemo header-auth scheme), Mediarq.EntityFrameworkCore's domain events raise an in-process\nOrderConfirmedDomainEvent on confirm (distinct from OrderPlacedEvent's outbox delivery), and\nMediarq.HealthChecks exposes GET /health.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(deferred): add Mediarq.Deferred package for in-process deferred dispatch (#204)\n\nIDeferredDispatcher.SendLaterAsync/PublishLaterAsync queue a command or notification on a\nSystem.Threading.Channels-backed background worker (DeferredDispatchHostedService) instead of\nrunning its handler(s) inline, decoupling the caller from handler execution time. No external\ndependency, no persistent store — fills the gap between immediate Send/Publish and durable\nscheduling (Mediarq.Hangfire/Mediarq.Quartz) for the \"reliable in-process fire-and-forget\" case.\nA graceful host shutdown stops accepting new work and drains everything already queued before\nstopping, bounded by the host's own shutdown timeout; an exception in one item is logged and does\nnot stop the worker from processing the rest.\n\nCloses #184.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(dapr): add Mediarq.Dapr package for Dapr pub/sub integration (#205)\n\nIDaprPubSubEvent marks a notification for Dapr pub/sub via static abstract PubsubName/Topic\nmembers (not instance properties, since the subscribe side needs routing info before any\nnotification instance exists, and both directions reading the same statics means they can\nnever drift apart).\n\nPublish side: AddMediarqDaprPubSub<TNotification>() registers a forwarder that calls\nDaprClient.PublishEventAsync when the notification is published through Mediarq, mirroring\nMediarq.MassTransit's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: MapDaprPubSubSubscription<TNotification>() maps a minimal-API webhook that\nextracts the `data` field from the CloudEvents 1.0 envelope the Dapr sidecar delivers and\nrepublishes it through IPublisher, and MapDaprPubSubSubscribeEndpoint() serves the\n/dapr/subscribe discovery endpoint the sidecar queries at startup.\n\nCloses #190.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(rabbitmq): add Mediarq.RabbitMQ package for a lightweight broker bridge (#206)\n\nIRabbitMqEvent marks a notification for RabbitMQ via static abstract Exchange/Queue/RoutingKey\nmembers (same static-member rationale as Mediarq.Dapr's IDaprPubSubEvent: the subscriber needs\nrouting info before any instance exists, and both directions reading the same statics means\nthey can never drift apart).\n\nPublish side: AddMediarqRabbitMqPublisher<TNotification>() registers a forwarder that publishes\non a short-lived channel per call, mirroring Mediarq.MassTransit/Mediarq.Dapr's forwarder shape\n(runs alongside in-process handlers).\n\nSubscribe side: AddMediarqRabbitMqSubscriber<TNotification>() registers a background service\nthat declares the exchange/queue/binding, consumes with manual acknowledgement, and republishes\neach delivery through IPublisher — acking only after a successful publish, nacking without\nrequeue on failure so a poison message doesn't loop forever.\n\nThis package never owns the IConnection's lifecycle (bring your own) and does not implement\nduplicate-delivery detection (documented as a follow-up, not silently assumed) — a lightweight\nalternative to Mediarq.MassTransit for the simple pub/sub case, per #189.\n\nFirst half of #189 (RabbitMQ). The Azure Service Bus half is a separate follow-up PR.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(azureservicebus): add Mediarq.AzureServiceBus package, closing #189 (#207)\n\nIAzureServiceBusEvent marks a notification for Azure Service Bus via static abstract\nTopicName/SubscriptionName members (same static-member rationale as Mediarq.Dapr/Mediarq.RabbitMQ:\nthe subscriber needs routing info before any instance exists, and both directions reading the\nsame statics means they can never drift apart). Uses the topic+subscription pub/sub model; this\npackage does not provision the topic/subscription (pre-provision via portal/ARM/Bicep/\nServiceBusAdministrationClient).\n\nPublish side: AddMediarqAzureServiceBusPublisher<TNotification>() registers a forwarder that\nsends on a ServiceBusSender created per publish, mirroring Mediarq.MassTransit/Mediarq.Dapr/\nMediarq.RabbitMQ's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: AddMediarqAzureServiceBusSubscriber<TNotification>() registers a background\nservice that processes the subscription via ServiceBusProcessor and republishes each message\nthrough IPublisher — completing only after a successful publish, dead-lettering on failure (the\nService Bus analogue of \"nack without requeue\") so a poison message doesn't loop forever.\n\nThis package never owns the ServiceBusClient's lifecycle and does not implement\nduplicate-delivery detection — a lightweight alternative to Mediarq.MassTransit for the simple\npub/sub case, per #189.\n\nSecond and final half of #189 (Azure Service Bus). Closes #189.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspire): add Mediarq.Aspire package for .NET Aspire ServiceDefaults integration (#208)\n\nAddMediarqServiceDefaults() on IHostApplicationBuilder, meant to be called from inside a\nconsumer's own dotnet new aspire-servicedefaults-generated ServiceDefaults project alongside its\nown OpenTelemetry/service-discovery/resilience setup — additive, not a replacement.\n\nWires Mediarq.OpenTelemetry's tracing/metrics (AddMediarqInstrumentation on both the tracer and\nmeter providers) and Mediarq.HealthChecks' handler-registration check on top of whatever the\nAspire template already generated, so Mediarq dispatch spans/metrics and a missing/ambiguous\nhandler both show up in the Aspire dashboard. Deliberately does not reimplement OpenTelemetry\nexporter/service-discovery/resilience wiring or map /health and /alive endpoints itself — those\nremain the template's own concern.\n\nCloses #187.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(packaging): add Mediarq logo as the NuGet package icon (#211)\n\n* feat(packaging): add Mediarq logo and embed it as the NuGet package icon\n\nAdds assets/logo.svg (source) and assets/icon.png (256x256), wires\nPackageIcon into src/Directory.Build.props so every package under src/\nships the icon, and adds it to the Mediarq.Templates package as well.\n\n* docs(readme): display the Mediarq logo at the top of the README\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(grpc): add Mediarq.Grpc package for direct point-to-point notification transport (#212)\n\nCloses #36 (gRPC half; Quartz/Hangfire scheduled dispatch already shipped in #171).\n\nIGrpcNotificationEvent (static abstract string ServiceAddress) marks a\nnotification for gRPC delivery to another service. AddMediarqGrpcPublisher<T>()\nforwards it via GrpcNotificationForwarder<T>, which resolves a cached, reused\nHTTP/2 GrpcChannel per ServiceAddress (GrpcChannelCache, disposed by the\ncontainer at shutdown) and calls the generated NotificationServiceClient.\nAddMediarqGrpcSubscriptions() + MapMediarqGrpcNotificationService() +\nMapMediarqGrpcSubscription<T>() receive it back into the pipeline: every\nsubscribed type is multiplexed over one shared RPC method (Publish), keyed by\nthe envelope's type_name against a registry of compile-time-typed\ndeserialize-and-publish delegates (no runtime reflection on the dispatch\npath itself).\n\nShips its own compiled Protobuf/gRPC contract (Protos/notification.proto,\nGrpcServices=\"Both\") so consumers reference this package only, no protoc/\nGrpc.Tools needed downstream. Tried generating the contract with\n--csharp_opt=internal_access to avoid exposing it as public API surface;\nreverted after confirming it's a known limitation (the flag only applies to\nmessage types, not the grpc_csharp_plugin-generated service/client stubs,\ncausing an accessibility mismatch) — the generated surface is public,\ntracked in PublicAPI.Unshipped.txt like every other package's surface.\n\nGrpcChannelCache and GrpcNotificationForwarder's constructor had to be public\nrather than internal: Microsoft.Extensions.DependencyInjection's default\ncontainer only considers public constructors when activating a type, so an\ninternal-typed constructor parameter on a publicly-constructed type silently\nfails DI resolution (caught by two failing tests during development, fixed\nbefore this commit).\n\nVerified: full solution build (0 warnings/errors beyond pre-existing,\nunrelated ones), full test suite (18 new tests, all green), and an ad-hoc\nAOT publish scan of the subscribe side confirmed AddGrpc()/MapGrpcService()\nitself produces zero trim/AOT warnings — the only warning present is the\nreflection-based JsonSerializer.Deserialize<T> call, the same pre-existing,\nunannotated pattern already used in the Dapr/RabbitMQ/AzureServiceBus\npackages, not a new risk category introduced here.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(sourcegen): add MQ005/MQ006/MQ007 diagnostics for stream handlers and notifications (#217)\n\nMQ001/MQ002 already caught duplicate/missing IRequestHandler registrations for\ncommands and queries. The generator collects the same registration data for\nIStreamRequestHandler<,> and INotificationHandler<>, but never diagnosed it,\nso a missing stream handler or an orphan notification only surfaced as a\nruntime HandlerNotFoundException.\n\n- MQ005 (warning): multiple IStreamRequestHandler<,> for the same stream request\n- MQ006 (info): a declared IStreamRequest<T> with no handler in the assembly\n- MQ007 (info): a declared INotification with no handler in the assembly\n\nCloses #213.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ202 to flag duplicate Mediarq routes at compile time (#218)\n\nMapMediarq() maps every [MediarqGet]/[MediarqPost]/[MediarqPut]/[MediarqPatch]/\n[MediarqDelete]-attributed request type as a minimal API endpoint with zero\nuniqueness check across types. Two types declaring the same (HTTP method,\nroute pattern) pair only collided at ASP.NET Core's routing time -- an\nambiguous-match error on the first matching request -- never at build time.\n\nDuplicateRouteAnalyzer (MQ202) collects every Mediarq route attribute in the\ncompilation and flags a duplicate (method, pattern) pair declared by more than\none type, same by-name/by-namespace attribute matching as the existing\nMQ200/MQ201/MQ204 analyzers.\n\nCloses #214.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): declare OpenAPI response metadata for MapMediarq() endpoints (#219)\n\nMapMediarq() built its minimal-API delegates as Func<TRequest, ISender,\nCancellationToken, Task<IResult>>, returning a bare IResult from the Handle*\nhelpers. ASP.NET Core's built-in OpenAPI inference needs a statically-typed\nResults<...> union or explicit .Produces<T>() calls to infer a response\nschema -- neither was present, so every MapMediarq()-mapped endpoint showed\nup in Swagger with an untyped or absent response body.\n\nAttach explicit response metadata to the RouteHandlerBuilder returned by\neach MapGet/MapPost/etc call instead: 200/204 with the success type (driven\nby the response type -- Result, Result<T> or Unit), plus every failure\nstatus ResultError.Type can map to (400/401/403/404/409/500), using the\nexisting ErrorType -> HTTP status mapping in ResultHttpExtensions.\n\nCloses #215.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* docs(samples): showcase Result.WithNotifications(...) cascading in the WebApi sample (#221)\n\nAddOrderNoteHandler now attaches an OrderNoteAddedEvent to its successful\nResult instead of just returning it -- a third, lightweight notification\npattern next to the transactional outbox (CreateOrder) and domain events\n(ConfirmOrder) this sample already demonstrates side by side.\n\nVerified manually: created an order, POSTed a note, confirmed the\n[cascaded notification] log line fires from the new INotificationHandler.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#223)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#224)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T21:49:04+02:00",
          "tree_id": "bc44ddcf81214df474e2f631cd79e3daa27b7f65",
          "url": "https://github.com/rouffou/mediarq/commit/ce2a5a519647a9dcdbff86bf2878ea78e8819293"
        },
        "date": 1785009042302,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send - Allocated",
            "value": 224,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send - Allocated",
            "value": 560,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean - Allocated",
            "value": 256,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain - Allocated",
            "value": 440,
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
          "id": "37cb619bb7460fc2603d44feb78bbf2f3444b50e",
          "message": "fix(packaging): declare lib/ framework assets for Mediarq and Mediarq.Analyzers (#226)\n\nBoth packages intentionally ship no assembly of their own (Mediarq is a\nmeta-package bundling its dependencies, Mediarq.Analyzers ships its DLL\nonly under analyzers/dotnet/cs) — with no lib/ folder at all, NuGet.org\nshows 'There are no supported framework assets in this package' for\nboth, as seen live on v1.5.0.\n\nAdd empty lib/<tfm>/_._ marker files (the standard NuGet convention for\nthis exact case) so NuGet.org lists net8.0/net9.0/net10.0 for Mediarq\nand netstandard2.0 for Mediarq.Analyzers, without shipping a real\nassembly. Suppress the resulting NU5128 for Mediarq.Analyzers, whose\ndependencies are intentionally omitted from the nuspec.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-26T08:33:36+02:00",
          "tree_id": "fa5daa5454b15a84da3f01500fc3e12a5217083f",
          "url": "https://github.com/rouffou/mediarq/commit/37cb619bb7460fc2603d44feb78bbf2f3444b50e"
        },
        "date": 1785047703959,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "SendBenchmarks.MediatR_Send - Allocated",
            "value": 224,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send - Allocated",
            "value": 560,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Lean - Allocated",
            "value": 256,
            "unit": "B"
          },
          {
            "name": "SendBenchmarks.Mediarq_Send_Plain - Allocated",
            "value": 440,
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
        "date": 1784988042968,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 424,
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
          "id": "f5eb5b01c198672aaff100bea8fb663aa3fd5d01",
          "message": "perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T16:54:44+02:00",
          "tree_id": "baccd9fe7316e21da4755f3b2be14f4c0bfad995",
          "url": "https://github.com/rouffou/mediarq/commit/f5eb5b01c198672aaff100bea8fb663aa3fd5d01"
        },
        "date": 1784991371434,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 424,
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
          "id": "6082554feb0d9e4fa4a124df658c8caf903e491d",
          "message": "feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T19:53:13+02:00",
          "tree_id": "2181188561efe23fe0a666f586c0e3b8425c5220",
          "url": "https://github.com/rouffou/mediarq/commit/6082554feb0d9e4fa4a124df658c8caf903e491d"
        },
        "date": 1785002084026,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 424,
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
          "id": "ce2a5a519647a9dcdbff86bf2878ea78e8819293",
          "message": "Release v1.5.0 (#225)\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(quartz): add Mediarq.Quartz package for delayed/scheduled dispatch (#174)\n\nEnqueueAsync/ScheduleAsync extensions on IScheduler run a Mediarq ICommand\nas a Quartz.NET job through the real dispatch pipeline. The command is\nJSON-serialized (System.Text.Json) into the job's JobDataMap alongside its\nAssemblyQualifiedName and reconstructed when the trigger fires.\n\nVerified end-to-end against a real Quartz scheduler and worker\n(Quartz.Extensions.Hosting), not just the job-data shape in isolation.\nSame ICommand-only constraint and generic-over-the-concrete-type pattern\nas Mediarq.Hangfire (each extension captures the concrete command type at\nthe call site so the type-name-based round trip resolves correctly).\n\nCompletes #36 (Hangfire done in a prior PR; gRPC transport for\ncross-service notifications remains open, no immediate plan to pick it up).\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(healthchecks): add Mediarq.HealthChecks package for handler-registration validation (#192)\n\nCatches a missing or ambiguous command/query handler before it surfaces as a\nHandlerNotFoundException on first dispatch. Ships an IHealthCheck for a /health\nendpoint plus AddMediarqHandlerValidationOnStartup, which runs the same check\nonce during host startup and throws so the app fails fast on misconfiguration.\n\nCloses #188\n\n* feat(ci): track allocation regression alongside mean time in the benchmark guardrail (#193)\n\ngithub-action-benchmark's built-in benchmarkdotnet tool only reads BenchmarkDotNet's Mean\nstatistic, so allocation regressions could slip through even with the existing time-based\nalert. Add a benchmark-alloc job (per Send/Publish matrix entry) that converts the same\nBenchmarkDotNet JSON export into the customSmallerIsBetter format via a new converter\nscript and tracks Memory.BytesAllocatedPerOperation as its own alerted history series,\nreusing the artifact the benchmark job already produces instead of rerunning BenchmarkDotNet.\n\nCloses #179\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(testing): add Mediarq.Testing package with SpyMediator and fakes (#197)\n\nNew SpyMediator decorates the registered IMediator, recording every dispatched\nrequest/notification while still delegating to the real one -- handlers, validators\nand pipeline behaviors all run for real, only the bookkeeping is added. AddMediarqSpy()\ndecorates via Scrutor after AddMediarq/AddMediarqCore; ISender/IPublisher are covered\ntoo since both already resolve the current IMediator from the container.\n\nSpyMediatorAssertions (Sent<T>/HasSent<T>/Published<T>/HasPublished<T>) stays\nframework-agnostic so it pairs with whatever assertion library a consumer already uses.\n\nAlso ships FakeClock/FakeUserContext, settable implementations of IClock/IUserContext.\n\nCloses #185\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(efcore): add domain-event support to Mediarq.EntityFrameworkCore (#198)\n\nNew IHasDomainEvents (+ convenience AggregateRoot base class) and DomainEventsInterceptor,\na SaveChanges interceptor that collects and clears events staged on tracked aggregates\nright before the commit, then publishes them only once it actually succeeds -- a failed\ncommit discards the collected events rather than publishing them or re-raising them on\na retry.\n\nAddMediarqDomainEvents() registers the interceptor as scoped IInterceptor on the\napplication service provider, so it's picked up automatically by any AddDbContext<T>(...)\ncall without touching that call -- and scoped (not singleton) so it gets a fresh scoped\nIPublisher per DbContext construction instead of capturing the first one forever.\n\nAsync-only: IPublisher has no synchronous overload, so only SaveChangesAsync is\nintercepted.\n\nCloses #186\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): add automatic minimal API mapping (app.MapMediarq()) (#199)\n\nNew route attributes (MediarqGet/Post/Put/Patch/Delete) + MapMediarq(), which scans\nassemblies for attributed commands/queries and maps each directly as a minimal API\nendpoint, dispatching through ISender. GET/DELETE bind the request's members\nindividually from the route/query string ([AsParameters], no body); POST/PUT/PATCH\nbind the whole request from the JSON body. The response converts the same way\nToHttpResult() already does for Result/Result<T>; a no-result ICommand (response\nUnit) maps a successful dispatch to 204 No Content. An attributed type whose\nresponse is none of those three shapes throws InvalidOperationException at startup\nrather than failing silently.\n\nDelegates are built dynamically per discovered type via MakeGenericMethod against\nfour private generic handler methods (body/params x Result/Result<T>, plus two more\nfor Unit), so [AsParameters]/body-binding attribution on the closed generic method's\nparameters is inspected by RequestDelegateFactory exactly as it would be for a\nhand-written endpoint.\n\nReturns a RouteGroupBuilder so shared conventions (RequireAuthorization, WithTags,\n...) apply to every mapped endpoint at once.\n\nCloses #180\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(ratelimiting): add Mediarq.RateLimiting package for pipeline-level throttling (#200)\n\nNew IRateLimitedRequest marker (PolicyName + optional PartitionKey) and\nRateLimitingBehavior, built on System.Threading.RateLimiting -- no HTTP dependency,\nprotects any hot path directly in the pipeline. A named RateLimiterRegistry maps a\npolicy name to a PartitionedRateLimiter<string>; PartitionKey (or \"*\" when null)\nselects the partition, so different callers (e.g. per user) get independent limits\nunder the same policy.\n\nNo permit available throws RateLimitExceededException (PolicyName/PartitionKey/\nRetryAfter) rather than short-circuiting into a Result -- catch it via an\nIRequestExceptionHandler<,> or an ASP.NET Core exception handler to map it to a\n429, mirroring Polly's own RateLimiterRejectedException convention rather than\nforcing a Result-shaped response the way Mediarq.Authorization does.\n\nCloses #182\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ204 analyzer for a pipeline behavior that is never active (#201)\n\nNew InertConditionalBehaviorAnalyzer flags an IConditionalPipelineBehavior.IsActive\nimplementation that is syntactically always the literal false -- the behavior is\nregistered but can never participate in the pipeline for any request. Same\nsyntactic-only approach as MQ201 (PipelineBehaviorNextAnalyzer): only fires when the\ngetter is literally `false` (expression-bodied property, expression-bodied getter, or\na single `return false;`), so real conditional logic is never flagged regardless of\nhow it evaluates at runtime -- no full flow-analysis proof attempted.\n\nCloses #191\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* chore(samples): wire Authorization, RateLimiting, HealthChecks, domain events into the WebApi sample (#202)\n\nThe Orders sample only demonstrated the extensions that existed before this cycle. It now also\nshowcases the four added since: Mediarq.RateLimiting throttles order creation (429 on rejection),\nMediarq.Authorization protects order confirmation behind a policy (401/403, via a self-contained\ndemo header-auth scheme), Mediarq.EntityFrameworkCore's domain events raise an in-process\nOrderConfirmedDomainEvent on confirm (distinct from OrderPlacedEvent's outbox delivery), and\nMediarq.HealthChecks exposes GET /health.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(deferred): add Mediarq.Deferred package for in-process deferred dispatch (#204)\n\nIDeferredDispatcher.SendLaterAsync/PublishLaterAsync queue a command or notification on a\nSystem.Threading.Channels-backed background worker (DeferredDispatchHostedService) instead of\nrunning its handler(s) inline, decoupling the caller from handler execution time. No external\ndependency, no persistent store — fills the gap between immediate Send/Publish and durable\nscheduling (Mediarq.Hangfire/Mediarq.Quartz) for the \"reliable in-process fire-and-forget\" case.\nA graceful host shutdown stops accepting new work and drains everything already queued before\nstopping, bounded by the host's own shutdown timeout; an exception in one item is logged and does\nnot stop the worker from processing the rest.\n\nCloses #184.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(dapr): add Mediarq.Dapr package for Dapr pub/sub integration (#205)\n\nIDaprPubSubEvent marks a notification for Dapr pub/sub via static abstract PubsubName/Topic\nmembers (not instance properties, since the subscribe side needs routing info before any\nnotification instance exists, and both directions reading the same statics means they can\nnever drift apart).\n\nPublish side: AddMediarqDaprPubSub<TNotification>() registers a forwarder that calls\nDaprClient.PublishEventAsync when the notification is published through Mediarq, mirroring\nMediarq.MassTransit's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: MapDaprPubSubSubscription<TNotification>() maps a minimal-API webhook that\nextracts the `data` field from the CloudEvents 1.0 envelope the Dapr sidecar delivers and\nrepublishes it through IPublisher, and MapDaprPubSubSubscribeEndpoint() serves the\n/dapr/subscribe discovery endpoint the sidecar queries at startup.\n\nCloses #190.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(rabbitmq): add Mediarq.RabbitMQ package for a lightweight broker bridge (#206)\n\nIRabbitMqEvent marks a notification for RabbitMQ via static abstract Exchange/Queue/RoutingKey\nmembers (same static-member rationale as Mediarq.Dapr's IDaprPubSubEvent: the subscriber needs\nrouting info before any instance exists, and both directions reading the same statics means\nthey can never drift apart).\n\nPublish side: AddMediarqRabbitMqPublisher<TNotification>() registers a forwarder that publishes\non a short-lived channel per call, mirroring Mediarq.MassTransit/Mediarq.Dapr's forwarder shape\n(runs alongside in-process handlers).\n\nSubscribe side: AddMediarqRabbitMqSubscriber<TNotification>() registers a background service\nthat declares the exchange/queue/binding, consumes with manual acknowledgement, and republishes\neach delivery through IPublisher — acking only after a successful publish, nacking without\nrequeue on failure so a poison message doesn't loop forever.\n\nThis package never owns the IConnection's lifecycle (bring your own) and does not implement\nduplicate-delivery detection (documented as a follow-up, not silently assumed) — a lightweight\nalternative to Mediarq.MassTransit for the simple pub/sub case, per #189.\n\nFirst half of #189 (RabbitMQ). The Azure Service Bus half is a separate follow-up PR.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(azureservicebus): add Mediarq.AzureServiceBus package, closing #189 (#207)\n\nIAzureServiceBusEvent marks a notification for Azure Service Bus via static abstract\nTopicName/SubscriptionName members (same static-member rationale as Mediarq.Dapr/Mediarq.RabbitMQ:\nthe subscriber needs routing info before any instance exists, and both directions reading the\nsame statics means they can never drift apart). Uses the topic+subscription pub/sub model; this\npackage does not provision the topic/subscription (pre-provision via portal/ARM/Bicep/\nServiceBusAdministrationClient).\n\nPublish side: AddMediarqAzureServiceBusPublisher<TNotification>() registers a forwarder that\nsends on a ServiceBusSender created per publish, mirroring Mediarq.MassTransit/Mediarq.Dapr/\nMediarq.RabbitMQ's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: AddMediarqAzureServiceBusSubscriber<TNotification>() registers a background\nservice that processes the subscription via ServiceBusProcessor and republishes each message\nthrough IPublisher — completing only after a successful publish, dead-lettering on failure (the\nService Bus analogue of \"nack without requeue\") so a poison message doesn't loop forever.\n\nThis package never owns the ServiceBusClient's lifecycle and does not implement\nduplicate-delivery detection — a lightweight alternative to Mediarq.MassTransit for the simple\npub/sub case, per #189.\n\nSecond and final half of #189 (Azure Service Bus). Closes #189.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspire): add Mediarq.Aspire package for .NET Aspire ServiceDefaults integration (#208)\n\nAddMediarqServiceDefaults() on IHostApplicationBuilder, meant to be called from inside a\nconsumer's own dotnet new aspire-servicedefaults-generated ServiceDefaults project alongside its\nown OpenTelemetry/service-discovery/resilience setup — additive, not a replacement.\n\nWires Mediarq.OpenTelemetry's tracing/metrics (AddMediarqInstrumentation on both the tracer and\nmeter providers) and Mediarq.HealthChecks' handler-registration check on top of whatever the\nAspire template already generated, so Mediarq dispatch spans/metrics and a missing/ambiguous\nhandler both show up in the Aspire dashboard. Deliberately does not reimplement OpenTelemetry\nexporter/service-discovery/resilience wiring or map /health and /alive endpoints itself — those\nremain the template's own concern.\n\nCloses #187.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(packaging): add Mediarq logo as the NuGet package icon (#211)\n\n* feat(packaging): add Mediarq logo and embed it as the NuGet package icon\n\nAdds assets/logo.svg (source) and assets/icon.png (256x256), wires\nPackageIcon into src/Directory.Build.props so every package under src/\nships the icon, and adds it to the Mediarq.Templates package as well.\n\n* docs(readme): display the Mediarq logo at the top of the README\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(grpc): add Mediarq.Grpc package for direct point-to-point notification transport (#212)\n\nCloses #36 (gRPC half; Quartz/Hangfire scheduled dispatch already shipped in #171).\n\nIGrpcNotificationEvent (static abstract string ServiceAddress) marks a\nnotification for gRPC delivery to another service. AddMediarqGrpcPublisher<T>()\nforwards it via GrpcNotificationForwarder<T>, which resolves a cached, reused\nHTTP/2 GrpcChannel per ServiceAddress (GrpcChannelCache, disposed by the\ncontainer at shutdown) and calls the generated NotificationServiceClient.\nAddMediarqGrpcSubscriptions() + MapMediarqGrpcNotificationService() +\nMapMediarqGrpcSubscription<T>() receive it back into the pipeline: every\nsubscribed type is multiplexed over one shared RPC method (Publish), keyed by\nthe envelope's type_name against a registry of compile-time-typed\ndeserialize-and-publish delegates (no runtime reflection on the dispatch\npath itself).\n\nShips its own compiled Protobuf/gRPC contract (Protos/notification.proto,\nGrpcServices=\"Both\") so consumers reference this package only, no protoc/\nGrpc.Tools needed downstream. Tried generating the contract with\n--csharp_opt=internal_access to avoid exposing it as public API surface;\nreverted after confirming it's a known limitation (the flag only applies to\nmessage types, not the grpc_csharp_plugin-generated service/client stubs,\ncausing an accessibility mismatch) — the generated surface is public,\ntracked in PublicAPI.Unshipped.txt like every other package's surface.\n\nGrpcChannelCache and GrpcNotificationForwarder's constructor had to be public\nrather than internal: Microsoft.Extensions.DependencyInjection's default\ncontainer only considers public constructors when activating a type, so an\ninternal-typed constructor parameter on a publicly-constructed type silently\nfails DI resolution (caught by two failing tests during development, fixed\nbefore this commit).\n\nVerified: full solution build (0 warnings/errors beyond pre-existing,\nunrelated ones), full test suite (18 new tests, all green), and an ad-hoc\nAOT publish scan of the subscribe side confirmed AddGrpc()/MapGrpcService()\nitself produces zero trim/AOT warnings — the only warning present is the\nreflection-based JsonSerializer.Deserialize<T> call, the same pre-existing,\nunannotated pattern already used in the Dapr/RabbitMQ/AzureServiceBus\npackages, not a new risk category introduced here.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(sourcegen): add MQ005/MQ006/MQ007 diagnostics for stream handlers and notifications (#217)\n\nMQ001/MQ002 already caught duplicate/missing IRequestHandler registrations for\ncommands and queries. The generator collects the same registration data for\nIStreamRequestHandler<,> and INotificationHandler<>, but never diagnosed it,\nso a missing stream handler or an orphan notification only surfaced as a\nruntime HandlerNotFoundException.\n\n- MQ005 (warning): multiple IStreamRequestHandler<,> for the same stream request\n- MQ006 (info): a declared IStreamRequest<T> with no handler in the assembly\n- MQ007 (info): a declared INotification with no handler in the assembly\n\nCloses #213.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ202 to flag duplicate Mediarq routes at compile time (#218)\n\nMapMediarq() maps every [MediarqGet]/[MediarqPost]/[MediarqPut]/[MediarqPatch]/\n[MediarqDelete]-attributed request type as a minimal API endpoint with zero\nuniqueness check across types. Two types declaring the same (HTTP method,\nroute pattern) pair only collided at ASP.NET Core's routing time -- an\nambiguous-match error on the first matching request -- never at build time.\n\nDuplicateRouteAnalyzer (MQ202) collects every Mediarq route attribute in the\ncompilation and flags a duplicate (method, pattern) pair declared by more than\none type, same by-name/by-namespace attribute matching as the existing\nMQ200/MQ201/MQ204 analyzers.\n\nCloses #214.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): declare OpenAPI response metadata for MapMediarq() endpoints (#219)\n\nMapMediarq() built its minimal-API delegates as Func<TRequest, ISender,\nCancellationToken, Task<IResult>>, returning a bare IResult from the Handle*\nhelpers. ASP.NET Core's built-in OpenAPI inference needs a statically-typed\nResults<...> union or explicit .Produces<T>() calls to infer a response\nschema -- neither was present, so every MapMediarq()-mapped endpoint showed\nup in Swagger with an untyped or absent response body.\n\nAttach explicit response metadata to the RouteHandlerBuilder returned by\neach MapGet/MapPost/etc call instead: 200/204 with the success type (driven\nby the response type -- Result, Result<T> or Unit), plus every failure\nstatus ResultError.Type can map to (400/401/403/404/409/500), using the\nexisting ErrorType -> HTTP status mapping in ResultHttpExtensions.\n\nCloses #215.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* docs(samples): showcase Result.WithNotifications(...) cascading in the WebApi sample (#221)\n\nAddOrderNoteHandler now attaches an OrderNoteAddedEvent to its successful\nResult instead of just returning it -- a third, lightweight notification\npattern next to the transactional outbox (CreateOrder) and domain events\n(ConfirmOrder) this sample already demonstrates side by side.\n\nVerified manually: created an order, POSTed a note, confirmed the\n[cascaded notification] log line fires from the new INotificationHandler.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#223)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#224)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-25T21:49:04+02:00",
          "tree_id": "bc44ddcf81214df474e2f631cd79e3daa27b7f65",
          "url": "https://github.com/rouffou/mediarq/commit/ce2a5a519647a9dcdbff86bf2878ea78e8819293"
        },
        "date": 1785009039725,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 424,
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
          "id": "37cb619bb7460fc2603d44feb78bbf2f3444b50e",
          "message": "fix(packaging): declare lib/ framework assets for Mediarq and Mediarq.Analyzers (#226)\n\nBoth packages intentionally ship no assembly of their own (Mediarq is a\nmeta-package bundling its dependencies, Mediarq.Analyzers ships its DLL\nonly under analyzers/dotnet/cs) — with no lib/ folder at all, NuGet.org\nshows 'There are no supported framework assets in this package' for\nboth, as seen live on v1.5.0.\n\nAdd empty lib/<tfm>/_._ marker files (the standard NuGet convention for\nthis exact case) so NuGet.org lists net8.0/net9.0/net10.0 for Mediarq\nand netstandard2.0 for Mediarq.Analyzers, without shipping a real\nassembly. Suppress the resulting NU5128 for Mediarq.Analyzers, whose\ndependencies are intentionally omitted from the nuspec.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-26T08:33:36+02:00",
          "tree_id": "fa5daa5454b15a84da3f01500fc3e12a5217083f",
          "url": "https://github.com/rouffou/mediarq/commit/37cb619bb7460fc2603d44feb78bbf2f3444b50e"
        },
        "date": 1785047702854,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 424,
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
          "id": "fdf88ad959f5833c36e62de2424708fd45f34ae5",
          "message": "Release v1.5.1 (#227)\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(quartz): add Mediarq.Quartz package for delayed/scheduled dispatch (#174)\n\nEnqueueAsync/ScheduleAsync extensions on IScheduler run a Mediarq ICommand\nas a Quartz.NET job through the real dispatch pipeline. The command is\nJSON-serialized (System.Text.Json) into the job's JobDataMap alongside its\nAssemblyQualifiedName and reconstructed when the trigger fires.\n\nVerified end-to-end against a real Quartz scheduler and worker\n(Quartz.Extensions.Hosting), not just the job-data shape in isolation.\nSame ICommand-only constraint and generic-over-the-concrete-type pattern\nas Mediarq.Hangfire (each extension captures the concrete command type at\nthe call site so the type-name-based round trip resolves correctly).\n\nCompletes #36 (Hangfire done in a prior PR; gRPC transport for\ncross-service notifications remains open, no immediate plan to pick it up).\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(healthchecks): add Mediarq.HealthChecks package for handler-registration validation (#192)\n\nCatches a missing or ambiguous command/query handler before it surfaces as a\nHandlerNotFoundException on first dispatch. Ships an IHealthCheck for a /health\nendpoint plus AddMediarqHandlerValidationOnStartup, which runs the same check\nonce during host startup and throws so the app fails fast on misconfiguration.\n\nCloses #188\n\n* feat(ci): track allocation regression alongside mean time in the benchmark guardrail (#193)\n\ngithub-action-benchmark's built-in benchmarkdotnet tool only reads BenchmarkDotNet's Mean\nstatistic, so allocation regressions could slip through even with the existing time-based\nalert. Add a benchmark-alloc job (per Send/Publish matrix entry) that converts the same\nBenchmarkDotNet JSON export into the customSmallerIsBetter format via a new converter\nscript and tracks Memory.BytesAllocatedPerOperation as its own alerted history series,\nreusing the artifact the benchmark job already produces instead of rerunning BenchmarkDotNet.\n\nCloses #179\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): cache empty-pipeline fact per request type to skip ResolveAll on repeat dispatch (#194)\n\nResolveAll<IPipelineBehavior<TReq,TRes>>() was paid on every Send even when zero behaviors\nare registered for that closed type -- pure DI resolution cost with no value. Add\nPipelineBehaviorRegistrationCache, a per-container singleton memoizing that structural,\nDI-registration-time fact so a repeat dispatch skips the IEnumerable<> resolution\nentirely. Only \"zero registered\" is ever cached: whether a registered behavior is\ncurrently active (IConditionalPipelineBehavior.IsActive) is per-request runtime state\nand is still re-evaluated on every dispatch.\n\nResolved through the existing IHandlerResolver rather than added as a constructor\nparameter, so PipelineExecutor's already-shipped public constructor signature is\nunchanged (non-breaking).\n\nCloses #177\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(ci): never let a missing/unreachable gh-pages branch fail the benchmark jobs (#195)\n\nThe gh-pages branch backing github-action-benchmark's history was found deleted from\norigin between two consecutive CI runs (its cause is unclear -- restored from a local\nremote-tracking ref that still had the full commit history). When gh-pages is missing,\ngithub-action-benchmark's git fetch hard-fails the step, turning this workflow's own\ndocumented \"report-only, never fails the build\" design into an actual build-blocking\nfailure. Add continue-on-error to both tracking steps so a missing/unreachable data\nbranch degrades to a stopped trend instead of a red check.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(authorization): add Mediarq.Authorization package for policy-based authorization (#196)\n\nNew IAuthorizedRequest marker + AuthorizationBehavior pipeline behavior that runs\nASP.NET Core policy-based authorization (IAuthorizationService) before the handler:\nno authenticated user short-circuits with ResultError.Unauthorized (401), an\nauthenticated user failing the named policy short-circuits with the new\nResultError.Forbidden (403). ErrorType gains a Forbidden member and ResultError gains\nUnauthorized/Forbidden factories (both purely additive, non-breaking).\n\nCloses #181\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(testing): add Mediarq.Testing package with SpyMediator and fakes (#197)\n\nNew SpyMediator decorates the registered IMediator, recording every dispatched\nrequest/notification while still delegating to the real one -- handlers, validators\nand pipeline behaviors all run for real, only the bookkeeping is added. AddMediarqSpy()\ndecorates via Scrutor after AddMediarq/AddMediarqCore; ISender/IPublisher are covered\ntoo since both already resolve the current IMediator from the container.\n\nSpyMediatorAssertions (Sent<T>/HasSent<T>/Published<T>/HasPublished<T>) stays\nframework-agnostic so it pairs with whatever assertion library a consumer already uses.\n\nAlso ships FakeClock/FakeUserContext, settable implementations of IClock/IUserContext.\n\nCloses #185\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(efcore): add domain-event support to Mediarq.EntityFrameworkCore (#198)\n\nNew IHasDomainEvents (+ convenience AggregateRoot base class) and DomainEventsInterceptor,\na SaveChanges interceptor that collects and clears events staged on tracked aggregates\nright before the commit, then publishes them only once it actually succeeds -- a failed\ncommit discards the collected events rather than publishing them or re-raising them on\na retry.\n\nAddMediarqDomainEvents() registers the interceptor as scoped IInterceptor on the\napplication service provider, so it's picked up automatically by any AddDbContext<T>(...)\ncall without touching that call -- and scoped (not singleton) so it gets a fresh scoped\nIPublisher per DbContext construction instead of capturing the first one forever.\n\nAsync-only: IPublisher has no synchronous overload, so only SaveChangesAsync is\nintercepted.\n\nCloses #186\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): add automatic minimal API mapping (app.MapMediarq()) (#199)\n\nNew route attributes (MediarqGet/Post/Put/Patch/Delete) + MapMediarq(), which scans\nassemblies for attributed commands/queries and maps each directly as a minimal API\nendpoint, dispatching through ISender. GET/DELETE bind the request's members\nindividually from the route/query string ([AsParameters], no body); POST/PUT/PATCH\nbind the whole request from the JSON body. The response converts the same way\nToHttpResult() already does for Result/Result<T>; a no-result ICommand (response\nUnit) maps a successful dispatch to 204 No Content. An attributed type whose\nresponse is none of those three shapes throws InvalidOperationException at startup\nrather than failing silently.\n\nDelegates are built dynamically per discovered type via MakeGenericMethod against\nfour private generic handler methods (body/params x Result/Result<T>, plus two more\nfor Unit), so [AsParameters]/body-binding attribution on the closed generic method's\nparameters is inspected by RequestDelegateFactory exactly as it would be for a\nhand-written endpoint.\n\nReturns a RouteGroupBuilder so shared conventions (RequireAuthorization, WithTags,\n...) apply to every mapped endpoint at once.\n\nCloses #180\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(ratelimiting): add Mediarq.RateLimiting package for pipeline-level throttling (#200)\n\nNew IRateLimitedRequest marker (PolicyName + optional PartitionKey) and\nRateLimitingBehavior, built on System.Threading.RateLimiting -- no HTTP dependency,\nprotects any hot path directly in the pipeline. A named RateLimiterRegistry maps a\npolicy name to a PartitionedRateLimiter<string>; PartitionKey (or \"*\" when null)\nselects the partition, so different callers (e.g. per user) get independent limits\nunder the same policy.\n\nNo permit available throws RateLimitExceededException (PolicyName/PartitionKey/\nRetryAfter) rather than short-circuiting into a Result -- catch it via an\nIRequestExceptionHandler<,> or an ASP.NET Core exception handler to map it to a\n429, mirroring Polly's own RateLimiterRejectedException convention rather than\nforcing a Result-shaped response the way Mediarq.Authorization does.\n\nCloses #182\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ204 analyzer for a pipeline behavior that is never active (#201)\n\nNew InertConditionalBehaviorAnalyzer flags an IConditionalPipelineBehavior.IsActive\nimplementation that is syntactically always the literal false -- the behavior is\nregistered but can never participate in the pipeline for any request. Same\nsyntactic-only approach as MQ201 (PipelineBehaviorNextAnalyzer): only fires when the\ngetter is literally `false` (expression-bodied property, expression-bodied getter, or\na single `return false;`), so real conditional logic is never flagged regardless of\nhow it evaluates at runtime -- no full flow-analysis proof attempted.\n\nCloses #191\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* chore(samples): wire Authorization, RateLimiting, HealthChecks, domain events into the WebApi sample (#202)\n\nThe Orders sample only demonstrated the extensions that existed before this cycle. It now also\nshowcases the four added since: Mediarq.RateLimiting throttles order creation (429 on rejection),\nMediarq.Authorization protects order confirmation behind a policy (401/403, via a self-contained\ndemo header-auth scheme), Mediarq.EntityFrameworkCore's domain events raise an in-process\nOrderConfirmedDomainEvent on confirm (distinct from OrderPlacedEvent's outbox delivery), and\nMediarq.HealthChecks exposes GET /health.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(deferred): add Mediarq.Deferred package for in-process deferred dispatch (#204)\n\nIDeferredDispatcher.SendLaterAsync/PublishLaterAsync queue a command or notification on a\nSystem.Threading.Channels-backed background worker (DeferredDispatchHostedService) instead of\nrunning its handler(s) inline, decoupling the caller from handler execution time. No external\ndependency, no persistent store — fills the gap between immediate Send/Publish and durable\nscheduling (Mediarq.Hangfire/Mediarq.Quartz) for the \"reliable in-process fire-and-forget\" case.\nA graceful host shutdown stops accepting new work and drains everything already queued before\nstopping, bounded by the host's own shutdown timeout; an exception in one item is logged and does\nnot stop the worker from processing the rest.\n\nCloses #184.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(dapr): add Mediarq.Dapr package for Dapr pub/sub integration (#205)\n\nIDaprPubSubEvent marks a notification for Dapr pub/sub via static abstract PubsubName/Topic\nmembers (not instance properties, since the subscribe side needs routing info before any\nnotification instance exists, and both directions reading the same statics means they can\nnever drift apart).\n\nPublish side: AddMediarqDaprPubSub<TNotification>() registers a forwarder that calls\nDaprClient.PublishEventAsync when the notification is published through Mediarq, mirroring\nMediarq.MassTransit's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: MapDaprPubSubSubscription<TNotification>() maps a minimal-API webhook that\nextracts the `data` field from the CloudEvents 1.0 envelope the Dapr sidecar delivers and\nrepublishes it through IPublisher, and MapDaprPubSubSubscribeEndpoint() serves the\n/dapr/subscribe discovery endpoint the sidecar queries at startup.\n\nCloses #190.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(rabbitmq): add Mediarq.RabbitMQ package for a lightweight broker bridge (#206)\n\nIRabbitMqEvent marks a notification for RabbitMQ via static abstract Exchange/Queue/RoutingKey\nmembers (same static-member rationale as Mediarq.Dapr's IDaprPubSubEvent: the subscriber needs\nrouting info before any instance exists, and both directions reading the same statics means\nthey can never drift apart).\n\nPublish side: AddMediarqRabbitMqPublisher<TNotification>() registers a forwarder that publishes\non a short-lived channel per call, mirroring Mediarq.MassTransit/Mediarq.Dapr's forwarder shape\n(runs alongside in-process handlers).\n\nSubscribe side: AddMediarqRabbitMqSubscriber<TNotification>() registers a background service\nthat declares the exchange/queue/binding, consumes with manual acknowledgement, and republishes\neach delivery through IPublisher — acking only after a successful publish, nacking without\nrequeue on failure so a poison message doesn't loop forever.\n\nThis package never owns the IConnection's lifecycle (bring your own) and does not implement\nduplicate-delivery detection (documented as a follow-up, not silently assumed) — a lightweight\nalternative to Mediarq.MassTransit for the simple pub/sub case, per #189.\n\nFirst half of #189 (RabbitMQ). The Azure Service Bus half is a separate follow-up PR.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(azureservicebus): add Mediarq.AzureServiceBus package, closing #189 (#207)\n\nIAzureServiceBusEvent marks a notification for Azure Service Bus via static abstract\nTopicName/SubscriptionName members (same static-member rationale as Mediarq.Dapr/Mediarq.RabbitMQ:\nthe subscriber needs routing info before any instance exists, and both directions reading the\nsame statics means they can never drift apart). Uses the topic+subscription pub/sub model; this\npackage does not provision the topic/subscription (pre-provision via portal/ARM/Bicep/\nServiceBusAdministrationClient).\n\nPublish side: AddMediarqAzureServiceBusPublisher<TNotification>() registers a forwarder that\nsends on a ServiceBusSender created per publish, mirroring Mediarq.MassTransit/Mediarq.Dapr/\nMediarq.RabbitMQ's forwarder shape (runs alongside in-process handlers).\n\nSubscribe side: AddMediarqAzureServiceBusSubscriber<TNotification>() registers a background\nservice that processes the subscription via ServiceBusProcessor and republishes each message\nthrough IPublisher — completing only after a successful publish, dead-lettering on failure (the\nService Bus analogue of \"nack without requeue\") so a poison message doesn't loop forever.\n\nThis package never owns the ServiceBusClient's lifecycle and does not implement\nduplicate-delivery detection — a lightweight alternative to Mediarq.MassTransit for the simple\npub/sub case, per #189.\n\nSecond and final half of #189 (Azure Service Bus). Closes #189.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspire): add Mediarq.Aspire package for .NET Aspire ServiceDefaults integration (#208)\n\nAddMediarqServiceDefaults() on IHostApplicationBuilder, meant to be called from inside a\nconsumer's own dotnet new aspire-servicedefaults-generated ServiceDefaults project alongside its\nown OpenTelemetry/service-discovery/resilience setup — additive, not a replacement.\n\nWires Mediarq.OpenTelemetry's tracing/metrics (AddMediarqInstrumentation on both the tracer and\nmeter providers) and Mediarq.HealthChecks' handler-registration check on top of whatever the\nAspire template already generated, so Mediarq dispatch spans/metrics and a missing/ambiguous\nhandler both show up in the Aspire dashboard. Deliberately does not reimplement OpenTelemetry\nexporter/service-discovery/resilience wiring or map /health and /alive endpoints itself — those\nremain the template's own concern.\n\nCloses #187.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): add opt-in polymorphic notification publishing (#209)\n\n* feat(core): add opt-in polymorphic notification publishing\n\nIPolymorphicNotification marks a notification whose publish also dispatches to\nINotificationHandler<TBase> for every base type in its class hierarchy, not just its own\nconcrete type -- closing the MediatR-migration friction point called out in #183 (MediatR does\nthis unconditionally; Mediarq keeps it opt-in).\n\nNotificationHandlerWrapperImpl<TNotification> resolves base-type handlers via\nIHandlerResolver's existing non-generic ResolveAll(Type), walking TNotification's BaseType chain\n(lazy, cached per closed type, [RequiresDynamicCode]/[RequiresUnreferencedCode] with a suppressed\ncall site, exactly mirroring AuthorizationBehavior's established reflection-fallback pattern).\nResolved instances are cast straight to INotificationHandler<TNotification> -- no expression-tree\ncompilation needed, since INotificationHandler<in TNotification> is already contravariant.\n\nOrdering: concrete-type handlers run first, then base-type handlers from most to least specific,\nunless a handler implements IOrderedNotificationHandler, whose explicit Order then takes\nprecedence across the whole combined batch -- same OrderBy logic already used for concrete-type\nhandlers, unchanged.\n\nZero behavior/perf change for notification types that don't opt in: IsPolymorphic is a single\ncheap IsAssignableFrom check computed once per closed TNotification type, and the reflection path\nis never reached unless a type actually implements IPolymorphicNotification. Verified against the\nfull existing Mediarq.Tests notification suite (zero regression) plus 7 new tests covering base-\ntype dispatch, non-opted-in isolation, default and explicit ordering across tiers, multi-level\nhierarchies, the no-handler no-op, and the single-handler fast path.\n\nCloses #183.\n\n* test(core): close patch-coverage gaps in polymorphic notification dispatch\n\ncodecov/patch was failing at 86% on PR #209: the ordered-handler scan\nover base-type handlers (reached only when no concrete handler is\nordered) and the empty-hierarchy short-circuit in\nBuildPolymorphicHandlerServiceTypes/ResolvePolymorphicHandlers had no\ndedicated test.\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* perf(core): share behavior-registration cache with Send's dispatch path, investigate ValueTask boundary (#210)\n\nCloses #175, closes #176.\n\n#175: Mediator.Send's hot path (RequestHandlerWrapperImpl) resolved\nIPipelineBehavior<,> via ResolveAll on every single dispatch, unlike\nPipelineExecutor which already skips that call once a request type is\nknown (via PipelineBehaviorRegistrationCache, #177) to have zero\nregistered behaviors. Extracted the shared cache-check + dispatch logic\ninto PipelineDispatch.ExecuteWithBehaviorCache, used by both\nPipelineExecutor and the wrapper, removing the duplicated inline copy\nthat previously existed only in PipelineExecutor. PipelineDispatch.Run's\nhandler-tail parameter was also simplified (Func<Task<TResponse>>\ninstead of Func<CancellationToken, Task<TResponse>>), removing one\nredundant closure per dispatch when at least one behavior is active —\nmeasured on DeepPipelineBenchmarks (10 chained behaviors): 1.52 KB ->\n1.45 KB allocated per Send.\n\nTrue compile-time behavior-chain composition (the literal ask of #175)\nwas considered and rejected after a design pass: the source generator\nonly sees types declared in the current compilation's syntax trees, so\nit cannot soundly know about IPipelineBehavior<,> implementations\nregistered from a referenced assembly — baking a \"no behaviors\" decision\ninto generated code would silently produce wrong results for that case.\nThe runtime cache is the sound alternative and delivers the same\npractical win for the common (no cross-assembly behaviors) case.\n\n#176: the internal (non-public) wrapper types RequestHandlerWrapper and\nRequestHandlerWrapperImpl now return ValueTask<TResponse> instead of\nTask<TResponse>; Mediator.Send (the public Task<TResponse>-returning\nboundary) converts once via ValueTask<TResponse>.AsTask(), which is\nallocation-free when backed by a real Task<TResponse> (always true on\nthis path). For a void command dispatch, the one remaining allocation\nis the handler's own Task<Unit> (Task.FromResult inside the\nIRequestHandler<TRequest> void-to-Unit adapter fixed by #169) -\nunavoidable without a breaking change to the Task-based\nIRequestHandler/IPipelineBehavior public contracts. Verified via\nextensive isolated benchmarking (including runs with dynamic PGO\ndisabled, and control tests where the changed code path was never\nexecuted) that the ValueTask conversion itself does not add measurable\nallocation; #176 is closed as investigated rather than yielding an\nadditional measurable win beyond the #175 cache fix.\n\nNo public API changes: ISender.Send, IPipelineExecutor.ExecuteAsync, and\nevery IRequestHandler/IPipelineBehavior signature are untouched. The\ntouched wrapper types are internal.\n\nVerified: full solution build (0 warnings/errors), full test suite\n(all assemblies green), DeepPipelineBenchmarks and CrossLibraryBenchmarks\nrun locally to confirm no regression.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(packaging): add Mediarq logo as the NuGet package icon (#211)\n\n* feat(packaging): add Mediarq logo and embed it as the NuGet package icon\n\nAdds assets/logo.svg (source) and assets/icon.png (256x256), wires\nPackageIcon into src/Directory.Build.props so every package under src/\nships the icon, and adds it to the Mediarq.Templates package as well.\n\n* docs(readme): display the Mediarq logo at the top of the README\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(grpc): add Mediarq.Grpc package for direct point-to-point notification transport (#212)\n\nCloses #36 (gRPC half; Quartz/Hangfire scheduled dispatch already shipped in #171).\n\nIGrpcNotificationEvent (static abstract string ServiceAddress) marks a\nnotification for gRPC delivery to another service. AddMediarqGrpcPublisher<T>()\nforwards it via GrpcNotificationForwarder<T>, which resolves a cached, reused\nHTTP/2 GrpcChannel per ServiceAddress (GrpcChannelCache, disposed by the\ncontainer at shutdown) and calls the generated NotificationServiceClient.\nAddMediarqGrpcSubscriptions() + MapMediarqGrpcNotificationService() +\nMapMediarqGrpcSubscription<T>() receive it back into the pipeline: every\nsubscribed type is multiplexed over one shared RPC method (Publish), keyed by\nthe envelope's type_name against a registry of compile-time-typed\ndeserialize-and-publish delegates (no runtime reflection on the dispatch\npath itself).\n\nShips its own compiled Protobuf/gRPC contract (Protos/notification.proto,\nGrpcServices=\"Both\") so consumers reference this package only, no protoc/\nGrpc.Tools needed downstream. Tried generating the contract with\n--csharp_opt=internal_access to avoid exposing it as public API surface;\nreverted after confirming it's a known limitation (the flag only applies to\nmessage types, not the grpc_csharp_plugin-generated service/client stubs,\ncausing an accessibility mismatch) — the generated surface is public,\ntracked in PublicAPI.Unshipped.txt like every other package's surface.\n\nGrpcChannelCache and GrpcNotificationForwarder's constructor had to be public\nrather than internal: Microsoft.Extensions.DependencyInjection's default\ncontainer only considers public constructors when activating a type, so an\ninternal-typed constructor parameter on a publicly-constructed type silently\nfails DI resolution (caught by two failing tests during development, fixed\nbefore this commit).\n\nVerified: full solution build (0 warnings/errors beyond pre-existing,\nunrelated ones), full test suite (18 new tests, all green), and an ad-hoc\nAOT publish scan of the subscribe side confirmed AddGrpc()/MapGrpcService()\nitself produces zero trim/AOT warnings — the only warning present is the\nreflection-based JsonSerializer.Deserialize<T> call, the same pre-existing,\nunannotated pattern already used in the Dapr/RabbitMQ/AzureServiceBus\npackages, not a new risk category introduced here.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(sourcegen): add MQ005/MQ006/MQ007 diagnostics for stream handlers and notifications (#217)\n\nMQ001/MQ002 already caught duplicate/missing IRequestHandler registrations for\ncommands and queries. The generator collects the same registration data for\nIStreamRequestHandler<,> and INotificationHandler<>, but never diagnosed it,\nso a missing stream handler or an orphan notification only surfaced as a\nruntime HandlerNotFoundException.\n\n- MQ005 (warning): multiple IStreamRequestHandler<,> for the same stream request\n- MQ006 (info): a declared IStreamRequest<T> with no handler in the assembly\n- MQ007 (info): a declared INotification with no handler in the assembly\n\nCloses #213.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(analyzers): add MQ202 to flag duplicate Mediarq routes at compile time (#218)\n\nMapMediarq() maps every [MediarqGet]/[MediarqPost]/[MediarqPut]/[MediarqPatch]/\n[MediarqDelete]-attributed request type as a minimal API endpoint with zero\nuniqueness check across types. Two types declaring the same (HTTP method,\nroute pattern) pair only collided at ASP.NET Core's routing time -- an\nambiguous-match error on the first matching request -- never at build time.\n\nDuplicateRouteAnalyzer (MQ202) collects every Mediarq route attribute in the\ncompilation and flags a duplicate (method, pattern) pair declared by more than\none type, same by-name/by-namespace attribute matching as the existing\nMQ200/MQ201/MQ204 analyzers.\n\nCloses #214.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(aspnetcore): declare OpenAPI response metadata for MapMediarq() endpoints (#219)\n\nMapMediarq() built its minimal-API delegates as Func<TRequest, ISender,\nCancellationToken, Task<IResult>>, returning a bare IResult from the Handle*\nhelpers. ASP.NET Core's built-in OpenAPI inference needs a statically-typed\nResults<...> union or explicit .Produces<T>() calls to infer a response\nschema -- neither was present, so every MapMediarq()-mapped endpoint showed\nup in Swagger with an untyped or absent response body.\n\nAttach explicit response metadata to the RouteHandlerBuilder returned by\neach MapGet/MapPost/etc call instead: 200/204 with the success type (driven\nby the response type -- Result, Result<T> or Unit), plus every failure\nstatus ResultError.Type can map to (400/401/403/404/409/500), using the\nexisting ErrorType -> HTTP status mapping in ResultHttpExtensions.\n\nCloses #215.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...) (#220)\n\n* feat(core): let a handler cascade follow-up notifications via Result.WithNotifications(...)\n\nA handler that needs to raise a notification after completing its own work\npreviously had to inject IPublisher and call Publish(...) itself, burying\n\"what does this handler cause to happen next\" in its body instead of its\nreturn type.\n\n- Result.WithNotifications(...) (virtual, covariant override on Result<T>)\n  attaches notifications to a result, fluent and mutation-based -- has no\n  effect on serialization (ResultJsonConverter never touches it).\n- PipelineDispatch publishes them via the resolved IPublisher (so the same\n  registered INotificationPublisher -- Parallel/Sequential/AggregateException\n  -- as an explicit Publish(...) call) once the request has finished\n  dispatching, after every behavior/exception handler/post-processor --\n  and only when the final response is a *successful* Result/Result<T>.\n- Not wired to Mediarq.Outbox: a cascaded notification goes through the same\n  IPublisher.Publish(...) as a manual call, not IOutbox.Enqueue(...). Combine\n  the two explicitly if a cascaded event needs the outbox's guarantee.\n- Zero overhead for any response type unrelated to Result (checked once per\n  closed TResponse type); a Result/Result<T> response that completes\n  synchronously with no attached notifications also pays nothing extra --\n  the async continuation is only used when there is something to await or\n  publish.\n\nCloses #216.\n\n* test(core): cover the async completion path of cascaded-notification publishing\n\ncodecov flagged PR #220's patch at 84% -- 6 missing lines and 1 partial\nbranch, all in PipelineDispatch.AwaitThenPublishAsync. Every existing test\ncompletes its handler's task synchronously (Moq's ReturnsAsync/.Returns(Result)\nalways yields an already-completed Task), so the async-await path\n(responseTask.IsCompletedSuccessfully == false) was never exercised.\n\nAdd two tests using a handler that awaits Task.Yield() before returning,\nforcing a genuinely incomplete task at the point WithCascadedNotifications\nchecks it -- confirmed locally via coverlet: PipelineDispatch.cs and all its\nasync state machines are now at 100% line/branch coverage.\n\n* docs: restore missing blank line before Routing section (merge artifact)\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* docs(samples): showcase Result.WithNotifications(...) cascading in the WebApi sample (#221)\n\nAddOrderNoteHandler now attaches an OrderNoteAddedEvent to its successful\nResult instead of just returning it -- a third, lightweight notification\npattern next to the transactional outbox (CreateOrder) and domain events\n(ConfirmOrder) this sample already demonstrates side by side.\n\nVerified manually: created an order, POSTed a note, confirmed the\n[cascaded notification] log line fires from the new INotificationHandler.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#223)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* feat(hangfire): add Mediarq.Hangfire package for delayed/scheduled dispatch (#171) (#172) (#224)\n\nEnqueue/Schedule extensions on IBackgroundJobClient run a Mediarq ICommand\nas a Hangfire background job through the real dispatch pipeline. Each\nextension is generic over the concrete command type (not ICommand itself)\nso Hangfire's job serializer can round-trip it correctly -- passing a\nvariable statically typed as ICommand would make Hangfire store the\ninterface as the parameter type and fail to deserialize the concrete\ncommand back.\n\nVerified end-to-end against a real (in-memory, Hangfire.InMemory) storage\nand worker, not just the Hangfire.Common.Job shape in isolation -- this was\nthe main open question (whether Hangfire actually supports serializing a\ngeneric job method call), now empirically confirmed rather than assumed.\n\nPartially addresses #36 (scheduled/delayed dispatch via Hangfire). Quartz\nand gRPC transport for cross-service notifications remain open.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n* fix(packaging): declare lib/ framework assets for Mediarq and Mediarq.Analyzers (#226)\n\nBoth packages intentionally ship no assembly of their own (Mediarq is a\nmeta-package bundling its dependencies, Mediarq.Analyzers ships its DLL\nonly under analyzers/dotnet/cs) — with no lib/ folder at all, NuGet.org\nshows 'There are no supported framework assets in this package' for\nboth, as seen live on v1.5.0.\n\nAdd empty lib/<tfm>/_._ marker files (the standard NuGet convention for\nthis exact case) so NuGet.org lists net8.0/net9.0/net10.0 for Mediarq\nand netstandard2.0 for Mediarq.Analyzers, without shipping a real\nassembly. Suppress the resulting NU5128 for Mediarq.Analyzers, whose\ndependencies are intentionally omitted from the nuspec.\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>\n\n---------\n\nCo-authored-by: Nicolas Rouffart <rouffart.nicolas@gmail.com>",
          "timestamp": "2026-07-26T08:38:57+02:00",
          "tree_id": "fa5daa5454b15a84da3f01500fc3e12a5217083f",
          "url": "https://github.com/rouffou/mediarq/commit/fdf88ad959f5833c36e62de2424708fd45f34ae5"
        },
        "date": 1785048027111,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "PublishBenchmarks.MediatR_Publish - Allocated",
            "value": 464,
            "unit": "B"
          },
          {
            "name": "PublishBenchmarks.Mediarq_Publish - Allocated",
            "value": 424,
            "unit": "B"
          }
        ]
      }
    ]
  }
}
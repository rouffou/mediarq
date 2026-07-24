window.BENCHMARK_DATA = {
  "lastUpdate": 1784917978308,
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
      }
    ]
  }
}
window.BENCHMARK_DATA = {
  "lastUpdate": 1784909900888,
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
      }
    ]
  }
}
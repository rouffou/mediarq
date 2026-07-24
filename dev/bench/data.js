window.BENCHMARK_DATA = {
  "lastUpdate": 1784903102779,
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
      }
    ]
  }
}
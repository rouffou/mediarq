window.BENCHMARK_DATA = {
  "lastUpdate": 1784900438396,
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
    ]
  }
}
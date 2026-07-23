# Benchmark baseline

`send-benchmarks.json` and `publish-benchmarks.json` are the committed baselines the
[`benchmarks` CI workflow](../../../.github/workflows/benchmarks.yml) compares each PR's run against
(via [`benchmark-action/github-action-benchmark`](https://github.com/benchmark-action/github-action-benchmark),
`external-data-json-path` mode — no `gh-pages` branch, so this doesn't collide with the DocFX site
already published from GitHub Pages).

Generated with:

```bash
dotnet run -c Release --project benchmarks/Mediarq.Benchmarks -- --filter '*' --job short --exporters json
```

(the same `--job short` invocation the CI workflow uses, so the baseline is comparable to what CI
measures — not the full-precision numbers from an unconstrained local run).

## Report-only, not a merge gate

CI never fails on a benchmark regression: GitHub-hosted runners are shared and noisy enough that a hard
threshold would produce false positives on a solo-maintained repo. The workflow comments on the PR when
a benchmark drifts past the alert threshold (150%) — treat that as a prompt to look closer, not as a
blocking check.

## Refreshing the baseline

These files are **not** updated automatically. After a deliberate performance change (improvement or an
accepted regression), refresh them by hand:

1. Run the `Benchmarks` workflow manually (`workflow_dispatch`) with `refresh-baseline: true`, or run the
   command above locally.
2. Download the `refreshed-baseline` artifact (or copy your local `BenchmarkDotNet.Artifacts/results/*
   -report-full-compressed.json` output) into this folder as `send-benchmarks.json` /
   `publish-benchmarks.json`.
3. Commit the updated files in the same PR as the change that caused the shift, so the baseline update is
   reviewed alongside the reason for it.

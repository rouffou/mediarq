#!/usr/bin/env node
// Converts a BenchmarkDotNet "full" JSON export (--exporters json) into the JSON array format
// github-action-benchmark's `customSmallerIsBetter` tool expects, tracking allocated bytes per
// operation. github-action-benchmark's built-in `benchmarkdotnet` tool only tracks the Mean time
// statistic, not BenchmarkDotNet's MemoryDiagnoser output, so this fills that gap as a second,
// independently-tracked metric.

import { readFileSync, writeFileSync } from "node:fs";

const [, , inputPath, outputPath] = process.argv;

if (!inputPath || !outputPath) {
  console.error("Usage: node benchmarkdotnet-alloc-report.mjs <input-full-report.json> <output.json>");
  process.exit(1);
}

const report = JSON.parse(readFileSync(inputPath, "utf8"));

const entries = (report.Benchmarks ?? [])
  .filter((b) => typeof b.Memory?.BytesAllocatedPerOperation === "number")
  .map((b) => ({
    name: `${b.FullName} - Allocated`,
    unit: "B",
    value: b.Memory.BytesAllocatedPerOperation,
  }));

writeFileSync(outputPath, JSON.stringify(entries, null, 2));

console.log(`Wrote ${entries.length} allocation entrie(s) from ${inputPath} to ${outputPath}`);

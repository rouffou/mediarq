# A ready-to-use dashboard

`Mediarq.Diagnostics` emits both a Mediarq-specific set of tags/metrics (`mediarq.*`, unchanged since
v1.0 — see [Concepts](concepts.md)) and, alongside them, the subset of the
[OpenTelemetry semantic conventions for messaging](https://opentelemetry.io/docs/specs/semconv/messaging/)
that make sense for an in-process mediator: `messaging.system` (`"mediarq"`), `messaging.operation.type`
(`"process"` for `Send`/streams, `"publish"` for `Publish`), `messaging.destination.name` (the request/
notification type name), `messaging.message.id`, `messaging.batch.message_count` (handler count for a
notification, item count for a stream) and the general `error.type` attribute. Nothing existing was
renamed or removed — this is purely additive, so a dashboard already built on the `mediarq.*` names keeps
working.

## Wiring

```csharp
using Mediarq.Diagnostics;
using Mediarq.OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

builder.Services.AddMediarqDiagnostics();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddMediarqInstrumentation() /* + your exporter, e.g. .AddOtlpExporter() */)
    .WithMetrics(metrics => metrics.AddMediarqInstrumentation() /* + your exporter */);
```

Export however you already do for the rest of your app — OTLP to a collector, or the
[`OpenTelemetry.Exporter.Prometheus.AspNetCore`](https://www.nuget.org/packages/OpenTelemetry.Exporter.Prometheus.AspNetCore)
package for a `/metrics` scrape endpoint. The dashboard below assumes Prometheus + Grafana, the most
common self-hosted combination for OTel metrics, but the underlying instruments work with any backend.

## Importing the dashboard

[`observability-dashboard.json`](../assets/observability-dashboard.json) is a Grafana dashboard you can
import as-is (Grafana → Dashboards → New → Import → upload the file, pick your Prometheus data source).
It has two rows:

- **Mediarq** (the original `mediarq.*` instruments): request rate, error rate, p50/p95/p99 duration —
  all split by `mediarq_request`.
- **OpenTelemetry messaging semantic conventions**: the same duration percentiles from
  `messaging_client_operation_duration_seconds`, split by `messaging_operation_type` and
  `messaging_destination_name`, plus a panel for `messaging_batch_message_count` (notification fan-out /
  stream item throughput).

> **Metric name suffixes depend on your exporter.** The queries below assume the standard OTel-to-
> Prometheus naming (dots → underscores, `_total` appended to counters, the unit appended to histograms —
> e.g. `messaging.client.operation.duration` with unit `s` becomes
> `messaging_client_operation_duration_seconds_bucket`). Check your own `/metrics` endpoint if a panel
> comes back empty and adjust the suffix to match.

## Example queries

Request rate, by request type:

```promql
sum by (mediarq_request) (rate(mediarq_requests_count_total[5m]))
```

Error rate:

```promql
sum(rate(mediarq_requests_count_total{mediarq_outcome="failure"}[5m]))
/ sum(rate(mediarq_requests_count_total[5m]))
```

p95 duration (OTel messaging semantic convention metric), by destination:

```promql
histogram_quantile(0.95,
  sum by (le, messaging_destination_name) (
    rate(messaging_client_operation_duration_seconds_bucket[5m])
  )
)
```

Notification fan-out (average handlers invoked per publish):

```promql
rate(messaging_batch_message_count_sum{messaging_operation_type="publish"}[5m])
/ rate(messaging_batch_message_count_count{messaging_operation_type="publish"}[5m])
```

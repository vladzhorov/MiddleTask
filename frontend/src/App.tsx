import { useEffect } from "react";
import { AggregationsByTypeCard } from "./features/metrics/AggregationsByTypeCard";
import { createMetricsConnection, type MetricReceivedEvent } from "./realtime/signalr";

export function App() {
  useEffect(() => {
    const connection = createMetricsConnection();

    connection.on("MetricReceived", (evt: MetricReceivedEvent) => {
      // For the first iteration we only log events.
      // Later we can refetch GraphQL queries or update Apollo cache.
      // eslint-disable-next-line no-console
      console.log("MetricReceived", evt);
    });

    void connection.start().catch((err) => {
      // eslint-disable-next-line no-console
      console.error("Failed to connect to SignalR hub", err);
    });

    return () => {
      connection.stop().catch(() => undefined);
    };
  }, []);

  return (
    <div className="container">
      <header className="pageHeader">
        <div>
          <h1 className="title">MiddleTask dashboard</h1>
          <p className="muted">GraphQL for queries, SignalR for real-time updates.</p>
        </div>
      </header>

      <main className="grid">
        <AggregationsByTypeCard />
      </main>
    </div>
  );
}


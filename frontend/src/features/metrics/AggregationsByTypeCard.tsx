import { useQuery } from "@apollo/client";
import { GET_AGG_BY_TYPE } from "../../graphql/queries";

type MetricTypeAggregation = {
  type: string;
  count: number;
  avgEnergy: number | null;
};

type GetMetricsByTypeAggregationsData = {
  metricsByTypeAggregations: MetricTypeAggregation[];
};

export function AggregationsByTypeCard() {
  const { data, loading, error, refetch } = useQuery<GetMetricsByTypeAggregationsData>(GET_AGG_BY_TYPE);

  return (
    <section className="card">
      <header className="cardHeader">
        <h2 className="cardTitle">Aggregations by type</h2>
        <button className="button" onClick={() => refetch()} disabled={loading}>
          Refresh
        </button>
      </header>

      {loading && <div className="muted">Loading aggregations…</div>}
      {error && (
        <div className="error">
          Failed to load aggregations: {error.message}
          <div style={{ marginTop: 8 }}>
            <button className="button" onClick={() => refetch()}>
              Retry
            </button>
          </div>
        </div>
      )}

      {!loading && !error && (
        <div className="tableWrap">
          <table className="table">
            <thead>
              <tr>
                <th>Type</th>
                <th style={{ textAlign: "right" }}>Count</th>
                <th style={{ textAlign: "right" }}>Avg energy</th>
              </tr>
            </thead>
            <tbody>
              {data?.metricsByTypeAggregations?.length ? (
                data.metricsByTypeAggregations.map((row) => (
                  <tr key={row.type}>
                    <td>{row.type}</td>
                    <td style={{ textAlign: "right" }}>{row.count}</td>
                    <td style={{ textAlign: "right" }}>
                      {row.avgEnergy === null ? "—" : row.avgEnergy.toFixed(2)}
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={3} className="muted">
                    No data yet.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}


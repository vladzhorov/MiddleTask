import { gql } from "@apollo/client";

export const GET_AGG_BY_TYPE = gql`
  query GetMetricsByTypeAggregations {
    metricsByTypeAggregations {
      type
      count
      avgEnergy
    }
  }
`;


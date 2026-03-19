const fallbackGraphqlUrl = "http://localhost:5050/graphql";
const fallbackSignalrUrl = "http://localhost:5001/hubs/metrics";

export const env = {
  graphqlUrl: (import.meta.env.VITE_GRAPHQL_URL as string | undefined) ?? fallbackGraphqlUrl,
  signalrUrl: (import.meta.env.VITE_SIGNALR_URL as string | undefined) ?? fallbackSignalrUrl,
};


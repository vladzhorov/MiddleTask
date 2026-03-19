import * as signalR from "@microsoft/signalr";
import { env } from "../env";

export type MetricReceivedEvent = {
  type: string;
  name: string;
  payload: {
    co2: number;
    pm25: number;
    humidity: number;
    energy: number;
  };
  timestamp: string;
};

export function createMetricsConnection() {
  return new signalR.HubConnectionBuilder()
    .withUrl(env.signalrUrl)
    .withAutomaticReconnect()
    .build();
}


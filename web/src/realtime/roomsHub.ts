import * as signalR from '@microsoft/signalr';
import { BASE_URL } from '../utilities/http';

export const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${BASE_URL}/roomsHub`, {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
  })
  .withAutomaticReconnect()
  .build();

connection.start()
  .catch(err => console.error(err));
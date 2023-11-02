export interface ConnectionMessage {
  action?: 'create' | 'delete' | 'status';
  connectorId?: string;
  connectorTicket?: string;
  location?: string;
}
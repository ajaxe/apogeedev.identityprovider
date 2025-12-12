export interface ClientListItem {
  displayName: string;
  clientId: string;
  applicationType: string;
  clientType: string;
  redirectUris: string[];
  postLogoutRedirectUris: string[];
  canEdit: boolean;
}

export type AppClientDataWithSecret = ClientListItem & { clientSecret: string };

export type ErrorResponseProps = Record<string, string[]>;

export interface ErrorsResponse {
  errors: ErrorResponseProps;
}

export interface AppClientDataWithSecretResponse extends ErrorsResponse{
  appClientData?: AppClientDataWithSecret;
}

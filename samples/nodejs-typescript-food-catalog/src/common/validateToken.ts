import * as jwt from 'jsonwebtoken';
import * as jwksClient from 'jwks-rsa';

const ExpectedMicrosoftApps = [
  '56c1da01-2129-48f7-9355-af6d59d42766', // Graph Connector Service
  '0bf30f3b-4a52-48df-9a82-234910c4a086', // Microsoft Graph Change Tracking
]

export async function validateToken(validationToken: string, tenantId: string, audience: string) {
  const getSigningKeys = (header, callback) => {
    const client = jwksClient({
      jwksUri: `https://login.microsoftonline.com/${tenantId}/discovery/keys`
    });

    client.getSigningKey(header.kid, function (err, key) {
      const signingKey = key.getPublicKey();
      callback(null, signingKey);
    });
  }

  const decodedToken: jwt.JwtPayload = jwt.decode(validationToken, { json: true });

  const isV2Token = decodedToken.ver === '2.0';

  const verifyOptions = {
    audience,
    issuer: isV2Token ? `https://login.microsoftonline.com/${tenantId}/v2.0` : `https://sts.windows.net/${tenantId}/`,
  }

  return new Promise<void>((resolve, reject) => {
    jwt.verify(validationToken, getSigningKeys, verifyOptions, (err, payload: jwt.JwtPayload) => {
      if (err) {
        reject(err);
        return;
      }

      const appId = isV2Token ? payload.azp : payload.appid;

      if (!ExpectedMicrosoftApps.includes(appId)) {
        reject('Not Expected Microsoft Apps.')
        return;
      }

      console.log('Token is validated.')
      resolve();
    });
  })
}
import amplitude from 'amplitude-js';

export const initAmplitude = (apiKey: string) => {
  amplitude.getInstance().init(apiKey);
};

export const setAmplitudeUserDevice = (installationToken: any) => {
  amplitude.getInstance().setDeviceId(installationToken);
};

export const setAmplitudeUserId = (userId: string) => {
  amplitude.getInstance().setUserId(userId);
};

export const setAmplitudeUserProperties = (properties: any) => {
  amplitude.getInstance().setUserProperties(properties);
};

export const sendAmplitudeData = (eventType: string, eventProperties?: any) => {
  amplitude.getInstance().logEvent(eventType, eventProperties);
};
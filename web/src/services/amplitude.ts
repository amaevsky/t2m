import amplitude from 'amplitude-js';

export const initAmplitude = () => {
  amplitude.getInstance().init('6f298f4d22f1b95c2067d17f08003727');
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
import { useEffect, useState } from 'react';
import { getThumbmark } from '@thumbmarkjs/thumbmarkjs';

export type FingerprintInfo = {
  ready: boolean;
  deviceId: string;
  deviceName: string;
};

const initialState: FingerprintInfo = {
  ready: false,
  deviceId: '',
  deviceName: '',
};

let cache: FingerprintInfo = initialState;
let promise: Promise<FingerprintInfo> | null = null;

function getOSName(platform?: string, userAgent?: string) {
  const text = `${platform || ''} ${userAgent || ''}`;

  if (/Windows|Win32|Win64/i.test(text)) return 'Windows';
  if (/Macintosh|MacIntel|Mac OS/i.test(text)) return 'macOS';
  if (/Android/i.test(text)) return 'Android';
  if (/iPhone/i.test(text)) return 'iPhone';
  if (/iPad/i.test(text)) return 'iPad';
  if (/Linux/i.test(text)) return 'Linux';

  return 'Unknown';
}

function getDeviceName(thumbmark: any) {
  const system = thumbmark?.components?.system;

  const browserName = system?.browser?.name || 'Unknown';
  const browserVersion = system?.browser?.version || '';

  const osName = getOSName(system?.platform, system?.useragent);

  return `${browserName}${browserVersion} on ${osName}`;
}

function getFallbackDeviceName() {
  return 'Unknown Device';
}

export async function getFingerprint(): Promise<FingerprintInfo> {
  if (cache.ready) return cache;

  if (promise) return promise;

  promise = getThumbmark()
    .then((thumbmark: any) => {
      cache = {
        ready: true,
        deviceId: thumbmark?.visitorId || thumbmark?.thumbmark || '',
        deviceName: getDeviceName(thumbmark),
      };

      return cache;
    })
    .catch(() => {
      cache = {
        ready: true,
        deviceId: 'unknown',
        deviceName: getFallbackDeviceName(),
      };

      return cache;
    });

  return promise;
}

export function useFingerprint(): FingerprintInfo {
  const [fingerprint, setFingerprint] = useState<FingerprintInfo>(cache);

  useEffect(() => {
    let mounted = true;

    getFingerprint().then((result) => {
      if (mounted) {
        setFingerprint(result);
      }
    });

    return () => {
      mounted = false;
    };
  }, []);

  return fingerprint;
}

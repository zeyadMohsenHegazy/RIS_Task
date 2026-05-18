export interface RuntimeEnv {
  production?: boolean;
  apiUrl?: string;
  appName?: string;
}

declare global {
  interface Window {
    __env?: RuntimeEnv;
  }
}

/** Merges Docker/runtime `env.js` values with build-time defaults. */
export function resolveRuntimeEnv(defaults: Required<RuntimeEnv>): Required<RuntimeEnv> {
  const runtime =
    typeof window !== 'undefined' && window.__env ? window.__env : {};

  return {
    production: runtime.production ?? defaults.production,
    apiUrl: runtime.apiUrl ?? defaults.apiUrl,
    appName: runtime.appName ?? defaults.appName,
  };
}

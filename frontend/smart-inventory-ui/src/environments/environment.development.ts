import { resolveRuntimeEnv } from './runtime-config';

const defaults = {
  production: false,
  apiUrl: 'http://localhost:5166/api',
  appName: 'Raya International Services',
};

export const environment = resolveRuntimeEnv(defaults);

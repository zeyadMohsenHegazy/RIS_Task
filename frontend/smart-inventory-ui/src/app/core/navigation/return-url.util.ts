/** Safe in-app path for post-login / guest redirects (blocks external and login loops). */
export function sanitizeReturnUrl(url: string | null | undefined): string {
  if (!url) {
    return '/home';
  }

  const path = url.split('?')[0].split('#')[0];

  if (!path.startsWith('/') || path.startsWith('//') || path.startsWith('/login')) {
    return '/home';
  }

  return path;
}

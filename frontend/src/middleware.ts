import createMiddleware from 'next-intl/middleware';

export default createMiddleware({
  locales: ['en', 'sv'],
  defaultLocale: 'en',
  // Default locale gets no prefix → / stays /, /sv/ is Swedish
  localePrefix: 'as-needed',
});

export const config = {
  // Exclude _next internals, static files, and /api/ proxy routes
  matcher: ['/((?!_next|api|.*\\..*).*)'],
};

import createMiddleware from 'next-intl/middleware';

export default createMiddleware({
  locales: ['en', 'sv'],
  defaultLocale: 'en',
  // Default locale gets no prefix → / stays /, /sv/ is Swedish
  localePrefix: 'as-needed',
});

export const config = {
  // Run on all routes except API, static files, and Next.js internals
  matcher: ['/((?!api|_next|_vercel|.*\\..*).*)'],
};

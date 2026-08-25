import createMiddleware from 'next-intl/middleware';
import { NextRequest, NextResponse } from 'next/server';

const intlMiddleware = createMiddleware({
  locales: ['en', 'sv'],
  defaultLocale: 'en',
  localePrefix: 'as-needed',
});

// Paths that REQUIRE authentication
const PROTECTED_PATHS = ['/loans', '/borrow'];

export default function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Skip static files and API routes
  if (pathname.startsWith('/_next') || pathname.startsWith('/api') || pathname.includes('.')) {
    return NextResponse.next();
  }

  // Strip locale prefix for route matching
  const strippedPath = pathname.replace(/^\/(en|sv)/, '') || '/';
  const token = request.cookies.get('frenda_token')?.value;

  // Protected route without auth → redirect to login
  const isProtected = PROTECTED_PATHS.some(p =>
    strippedPath === p || strippedPath.startsWith(p + '/')
  );
  if (isProtected && !token) {
    return NextResponse.redirect(new URL('/login', request.url));
  }

  // Logged in on auth pages → redirect to loans
  const isAuthPage = strippedPath === '/login' || strippedPath === '/register';
  if (isAuthPage && token) {
    return NextResponse.redirect(new URL('/loans', request.url));
  }

  // Let next-intl handle locale detection, cookie setting, and routing
  return intlMiddleware(request);
}

export const config = {
  matcher: ['/((?!_next|api|.*\\..*).*)'],
};

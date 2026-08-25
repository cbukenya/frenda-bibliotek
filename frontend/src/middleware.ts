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

  const token = request.cookies.get('frenda_token')?.value;

  // Check if path requires auth (with or without locale prefix)
  const strippedPath = pathname.replace(/^\/(en|sv)/, '') || '/';
  const isProtected = PROTECTED_PATHS.some(p => strippedPath === p || strippedPath.startsWith(p + '/') || strippedPath.endsWith(p));

  // If trying to access protected page without login → redirect to login
  if (isProtected && !token) {
    const loginUrl = new URL('/login', request.url);
    return NextResponse.redirect(loginUrl);
  }

  // If logged in and on login/register → redirect to loans
  const isAuthPage = ['/login', '/register'].some(p =>
    strippedPath === p || pathname.endsWith(p)
  );
  if (isAuthPage && token) {
    const homeUrl = new URL('/loans', request.url);
    return NextResponse.redirect(homeUrl);
  }

  return intlMiddleware(request);
}

export const config = {
  matcher: ['/((?!_next|api|.*\\..*).*)'],
};

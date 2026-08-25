import createMiddleware from 'next-intl/middleware';
import { NextRequest, NextResponse } from 'next/server';

const intlMiddleware = createMiddleware({
  locales: ['en', 'sv'],
  defaultLocale: 'en',
  localePrefix: 'as-needed',
});

// Public paths that don't require authentication
const PUBLIC_PATHS = ['/login', '/register'];

export default function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Skip static files and API routes
  if (pathname.startsWith('/_next') || pathname.startsWith('/api') || pathname.includes('.')) {
    return NextResponse.next();
  }

  // Check if path is public (with or without locale prefix)
  const isPublic = PUBLIC_PATHS.some(p =>
    pathname === p ||
    pathname.endsWith(p) ||
    pathname.match(new RegExp(`^/(en|sv)${p}$`))
  );

  const token = request.cookies.get('frenda_token')?.value;

  // If not logged in and trying to access protected page → redirect to login
  if (!isPublic && !token) {
    const loginUrl = new URL('/login', request.url);
    return NextResponse.redirect(loginUrl);
  }

  // If logged in and trying to access login/register → redirect to home
  if (isPublic && token) {
    const homeUrl = new URL('/', request.url);
    return NextResponse.redirect(homeUrl);
  }

  return intlMiddleware(request);
}

export const config = {
  matcher: ['/((?!_next|api|.*\\..*).*)'],
};

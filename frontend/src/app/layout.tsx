// Minimal root layout required by Next.js App Router.
// The real layout (with NavBar, locale provider, etc.) lives in app/[locale]/layout.tsx.
// The middleware redirects all requests to the appropriate locale segment.
export default function RootLayout({ children }: { children: React.ReactNode }) {
  return children;
}

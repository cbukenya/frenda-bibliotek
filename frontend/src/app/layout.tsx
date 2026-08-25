import './globals.css';

// Root layout: Next.js requires exactly one <html> and <body> in the tree.
// The locale-specific provider lives in app/[locale]/layout.tsx.
export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html>
      <body>{children}</body>
    </html>
  );
}

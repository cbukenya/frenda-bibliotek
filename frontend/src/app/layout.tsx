import type { Metadata } from 'next';
import './globals.css';
import NavBar from '@/components/NavBar';
import UserSwitcher from '@/components/UserSwitcher';

export const metadata: Metadata = {
  title: { default: 'Frenda Bibliotek', template: '%s | Frenda Bibliotek' },
  description: 'A curated library of technical and professional literature. Browse, borrow, and discover your next great read.',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <NavBar />
        <main className="page">
          {children}
        </main>
        <UserSwitcher />
      </body>
    </html>
  );
}

import type { Metadata } from 'next';
import { NextIntlClientProvider } from 'next-intl';
import { getMessages } from 'next-intl/server';
import '../globals.css';
import NavBar from '@/components/NavBar';
import UserSwitcher from '@/components/UserSwitcher';

export const metadata: Metadata = {
  title: { default: 'Frenda Bibliotek', template: '%s | Frenda Bibliotek' },
  description: 'A curated library of technical and professional literature. Browse, borrow, and discover your next great read.',
};

export default async function LocaleLayout({
  children,
  params: { locale },
}: {
  children: React.ReactNode;
  params: { locale: string };
}) {
  // Load messages for the current locale (server-side, no waterfall)
  const messages = await getMessages();

  return (
    <html lang={locale}>
      <body>
        <NextIntlClientProvider locale={locale} messages={messages}>
          <NavBar />
          <main className="page">{children}</main>
          <UserSwitcher />
        </NextIntlClientProvider>
      </body>
    </html>
  );
}

import type { Metadata } from 'next';
import { NextIntlClientProvider } from 'next-intl';
import { getMessages } from 'next-intl/server';
import NavBar from '@/components/NavBar';

export const metadata: Metadata = {
  title: { default: 'Frenda Bibliotek', template: '%s | Frenda Bibliotek' },
  description: 'A curated library of technical and professional literature.',
};

export default async function LocaleLayout({
  children,
  params: { locale },
}: {
  children: React.ReactNode;
  params: { locale: string };
}) {
  const messages = await getMessages();

  return (
    <NextIntlClientProvider locale={locale} messages={messages}>
      <NavBar />
      {children}
    </NextIntlClientProvider>
  );
}

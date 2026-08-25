'use client';

import { useEffect, useState, useCallback } from 'react';
import { useTranslations } from 'next-intl';
import { getTopBooks, getBooks, getMyLoans, isLoggedIn, type Book } from '@/lib/api';
import BookCard from '@/components/BookCard';
import TopListRow from '@/components/TopListRow';

function shuffle<T>(arr: T[]): T[] {
  const a = [...arr];
  for (let i = a.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [a[i], a[j]] = [a[j], a[i]];
  }
  return a;
}

export default function DiscoverPage() {
  const t = useTranslations('discover');
  const [topBooks, setTopBooks] = useState<Book[]>([]);
  const [recommendations, setRecommendations] = useState<Book[]>([]);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [allBooks, top] = await Promise.all([
        getBooks(),
        getTopBooks(),
      ]);
      setTopBooks(top);

      // If logged in, filter out books the user already has
      let notLoaned = allBooks;
      if (isLoggedIn()) {
        try {
          const loans = await getMyLoans();
          const loanedBookIds = new Set(
            loans.filter(l => !l.returnedAt).map(l => l.bookId)
          );
          notLoaned = allBooks.filter(b => !loanedBookIds.has(b.id));
        } catch { /* ignore auth errors */ }
      }
      setRecommendations(shuffle(notLoaned).slice(0, 3));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
    window.addEventListener('frenda_user_changed', load);
    return () => window.removeEventListener('frenda_user_changed', load);
  }, [load]);

  return (
    <div className="pt-24 pb-24 md:pb-8">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop pt-8 flex flex-col gap-12">

        {/* Header */}
        <header>
          <h1 className="font-headline-xl text-headline-xl text-primary mb-2 hidden md:block">{t('title')}</h1>
          <h1 className="font-headline-lg-mobile text-headline-lg-mobile text-primary mb-2 md:hidden">{t('title')}</h1>
          <p className="font-body-lg text-body-lg text-on-surface-variant max-w-2xl">{t('subtitle')}</p>
        </header>

        {/* Recommendations */}
        <section>
          <h2 className="font-headline-sm text-headline-sm text-on-surface mb-6">{t('recommended')}</h2>

          {loading ? (
            <div className="bento-grid">
              {Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="rounded-xl overflow-hidden bg-surface-container-lowest shadow-sm animate-pulse">
                  <div className="h-64 bg-surface-container-high" />
                  <div className="p-gutter space-y-3">
                    <div className="h-4 bg-surface-container-high rounded w-3/4" />
                    <div className="h-3 bg-surface-container-high rounded w-1/2" />
                    <div className="h-10 bg-surface-container-high rounded mt-4" />
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="bento-grid">
              {recommendations.map(book => (
                <BookCard key={book.id} book={book} onBorrowed={load} />
              ))}
            </div>
          )}
        </section>

        {/* Top list */}
        <section>
          <h2 className="font-headline-sm text-headline-sm text-on-surface mb-6">{t('topList')}</h2>
          {loading ? (
            <div className="flex flex-col gap-4">
              {Array.from({ length: 5 }).map((_, i) => (
                <div key={i} className="rounded-xl bg-surface-container-lowest shadow-sm h-20 animate-pulse" />
              ))}
            </div>
          ) : (
            <div className="flex flex-col gap-4">
              {topBooks.map((book, i) => (
                <TopListRow key={book.id} book={book} rank={i + 1} />
              ))}
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

'use client';

import { useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { useTranslations } from 'next-intl';
import { getBooks, type Book } from '@/lib/api';
import BookCard from '@/components/BookCard';

export default function BrowsePage() {
  const t = useTranslations('browse');
  const searchParams = useSearchParams();
  const [books, setBooks] = useState<Book[]>([]);
  const [loading, setLoading] = useState(true);
  const [query, setQuery] = useState(searchParams.get('q') ?? '');

  const load = () => {
    setLoading(true);
    getBooks().then(setBooks).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const filtered = useMemo(() => {
    const q = query.toLowerCase().trim();
    if (!q) return books;
    return books.filter(b =>
      b.title.toLowerCase().includes(q) ||
      b.author.toLowerCase().includes(q) ||
      b.genre.toLowerCase().includes(q) ||
      b.isbn.includes(q)
    );
  }, [books, query]);

  return (
    <div className="pt-24 pb-24 md:pb-8">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop pt-8">
        {/* Page header */}
        <header className="mb-12">
          <h1 className="font-headline-xl text-headline-xl text-primary mb-2 hidden md:block">{t('title')}</h1>
          <h1 className="font-headline-lg-mobile text-headline-lg-mobile text-primary mb-2 md:hidden">{t('title')}</h1>
          <p className="font-body-lg text-body-lg text-on-surface-variant max-w-2xl">{t('subtitle')}</p>
        </header>

        {/* Search bar */}
        <div className="relative mb-10 max-w-lg">
          <input
            type="search"
            placeholder={t('searchPlaceholder')}
            value={query}
            onChange={e => setQuery(e.target.value)}
            className="w-full bg-[#F1F3F5] border-none rounded-full py-2.5 pl-10 pr-4 text-sm focus:ring-2 focus:ring-primary focus:bg-white transition-all outline-none"
          />
          <span className="material-symbols-outlined absolute left-3 top-2.5 text-on-surface-variant text-[18px]">search</span>
        </div>

        {/* Book grid */}
        {loading ? (
          <div className="bento-grid">
            {Array.from({ length: 6 }).map((_, i) => (
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
        ) : filtered.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-24 text-on-surface-variant">
            <span className="material-symbols-outlined text-6xl mb-4 text-outline">inbox</span>
            <p className="font-headline-sm text-headline-sm mb-2">{t('noResults')}</p>
            <p className="font-body-md text-body-md">{t('noResultsHint')}</p>
          </div>
        ) : (
          <section className="bento-grid">
            {filtered.map(book => (
              <BookCard key={book.id} book={book} onBorrowed={load} />
            ))}
          </section>
        )}
      </main>
    </div>
  );
}
